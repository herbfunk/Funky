using System;
using System.Diagnostics;
using System.Linq;
using FunkyBot.Cache.Enums;
using FunkyBot.DBHandlers;
using Zeta.Bot.Logic;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;

namespace FunkyBot.Cache.Objects
{

	public class CacheItem : CacheObject
	{
		public CacheItem(CacheObject baseobj)
			: base(baseobj)
		{

		}

		public override bool RequiresLOSCheck { get; set; }

		public DiaItem ref_DiaItem { get; set; }
		public int? DynamicID { get; set; }
		public ItemQuality? Itemquality { get; set; }
		public bool ItemQualityRechecked { get; set; }
		public GilesItemType GilesItemType { get; set; }
		public bool? ShouldPickup { get; set; }
		private DateTime LastAvoidanceIgnored = DateTime.Today;

		[DebuggerNonUserCode]
		public int? GoldAmount { get; set; }

		public int? BalanceID { get; set; }

		private double LootRadius
		{
			get
			{
				if (targetType.Value == TargetType.Item)
				{
					return Bot.Targeting.iCurrentMaxLootRadius + Bot.Settings.Ranges.ItemRange;
				}
				if (targetType.Value == TargetType.Gold)
				{
					return Bot.Targeting.iCurrentMaxLootRadius + Bot.Settings.Ranges.GoldRange;
				}
				return Bot.Targeting.iCurrentMaxLootRadius + Bot.Settings.Ranges.GlobeRange;
			}
		}

		public CacheBalance BalanceData
		{
			get
			{
				if (BalanceID.HasValue && CacheIDLookup.dictGameBalanceCache.ContainsKey(BalanceID.Value))
					return CacheIDLookup.dictGameBalanceCache[BalanceID.Value];
				return null;
			}
		}

		public override bool IsZDifferenceValid
		{
			get
			{
				float fThisHeightDifference = Funky.Difference(Bot.Character.Data.Position.Z, Position.Z);

				if (targetType.HasValue && targetType.Value == TargetType.Item)
				{
					if (fThisHeightDifference >= 26f)
						return false;
				}
				else
				{
					// Gold/Globes at 11+ z-height difference
					if (fThisHeightDifference >= 11f)
						return false;
				}

				return base.IsZDifferenceValid;
			}
		}

		public override bool ObjectShouldBeRecreated
		{
			get
			{
				return false;
			}
		}

		public override void UpdateWeight()
		{
			base.UpdateWeight();

			Vector3 TestPosition = Position;

			//Use modified Test Position for Gold/Globe
			if (Bot.Character.Data.PickupRadius > 0f && ObjectCache.CheckTargetTypeFlag(targetType.Value, TargetType.Globe | TargetType.Gold))
				TestPosition = MathEx.CalculatePointFrom(Bot.Character.Data.Position, Position, Math.Max(0f, CentreDistance - Bot.Character.Data.PickupRadius));

			if (Bot.Character.Data.Position.Distance(TestPosition) >= 2f)
			{
				//If we are already ignored this recently.. lets just assume its still being ignored!
				if (DateTime.Now.Subtract(LastAvoidanceIgnored).TotalMilliseconds < 1000 && Bot.Targeting.Environment.NearbyAvoidances.Count > 0)
				{
					Weight = 1;
				}
				else
				{
					//Test if this object is within any avoidances
					if (ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(TestPosition))
						Weight = 1;

					//Test intersection of avoidances
					else if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(TestPosition))
						Weight = 1;



				}
			}

			if (Weight != 1)
			{

				Vector3 BotPosition = Bot.Character.Data.Position;
				float centreDistance = BotPosition.Distance(TestPosition);

				switch (targetType.Value)
				{

					case TargetType.Item:
						//this.Weight=(Bot.ItemRange*275)-(Math.Floor(centreDistance)*1000d);
						Weight = 15000d - (Math.Floor(centreDistance) * 190d);
						// Point-blank items get a weight increase 
						if (centreDistance <= 12f)
							Weight += 600d;
						// Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
						if (this == Bot.Targeting.LastCachedTarget)
							Weight += 600;
						// Give yellows more weight
						if (Itemquality.Value >= ItemQuality.Rare4)
							Weight += 6000d;
						// Give legendaries more weight
						if (Itemquality.Value >= ItemQuality.Legendary)
						{
							Weight += 10000d;

							double rangelimitRatio = CentreDistance / LootRadius;
							if (rangelimitRatio > 0.5d && PriorityCounter == 0)
							{
								//prioritize!
								Weight += 25000d;
								PrioritizedDate = DateTime.Now;
								PriorityCounter = 5;
							}
						}
						// Are we prioritizing close-range stuff atm? If so limit it at a value 3k lower than monster close-range priority
						if (Bot.Character.Data.bIsRooted)
							Weight = 18000 - (Math.Floor(centreDistance) * 200);
						// If there's a monster in the path-line to the item, reduce the weight
						if (ObjectCache.Obstacles.Monsters.Any(cp => cp.PointInside(Position)))
							Weight *= 0.75;
						//Finally check if we should reduce the weight when more then 2 monsters are nearby..
						//if (Bot.Targeting.Environment.SurroundingUnits>2&&
						//     //But Only when we are low in health..
						//        (Bot.Character_.Data.dCurrentHealthPct<0.25||
						//     //Or we havn't changed targets after 2.5 secs
						//        DateTime.Now.Subtract(Bot.Targeting.LastChangeOfTarget).TotalSeconds>2.5))
						//     this.Weight*=0.10;

						//Test if there are nearby units that will trigger kite action..
						if (Bot.Character.Data.ShouldFlee)
						{
							if (ObjectCache.Objects.OfType<CacheUnit>().Any(m => m.ShouldFlee && m.IsPositionWithinRange(Position, Bot.Settings.Fleeing.FleeMaxMonsterDistance)))
								Weight = 1;
						}

						//Did we have a target last time? and if so was it a goblin?
						if (Bot.Targeting.LastCachedTarget.RAGUID != -1 && Bot.Settings.Targeting.GoblinPriority > 1)
						{
							if (Bot.Targeting.LastCachedTarget.IsTreasureGoblin)
								Weight = 0;
						}
						break;
					case TargetType.Gold:
						if (GoldAmount > 0)
							Weight = 11000d - (Math.Floor(centreDistance) * 200d);
						// Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
						if (this == Bot.Targeting.LastCachedTarget)
							Weight += 600;
						// Are we prioritizing close-range stuff atm? If so limit it at a value 3k lower than monster close-range priority
						if (Bot.Character.Data.bIsRooted)
							Weight = 18000 - (Math.Floor(centreDistance) * 200);
						// If there's a monster in the path-line to the item, reduce the weight by 25%
						if (ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(this, BotPosition)))
							Weight *= 0.75;
						//Did we have a target last time? and if so was it a goblin?
						if (Bot.Targeting.LastCachedTarget.RAGUID != -1)
						{
							if (Bot.Targeting.LastCachedTarget.IsTreasureGoblin)
								Weight = 0;
						}
						break;
					case TargetType.Globe:
					case TargetType.PowerGlobe:
						if (targetType.Equals(TargetType.Globe) && Bot.Character.Data.dCurrentHealthPct > Bot.Settings.Combat.GlobeHealthPercent)
						{
							Weight = 0;
						}
						else
						{
							// Ok we have globes enabled, and our health is low...!
							Weight = 17000d - (Math.Floor(centreDistance) * 90d);

							// Point-blank items get a weight increase
							if (centreDistance <= 15f)
								Weight += 3000d;

							// Close items get a weight increase
							if (centreDistance <= 60f)
								Weight += 1500d;

							if (targetType == TargetType.PowerGlobe)
								Weight += 5000d;

							// Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
							if (this == Bot.Targeting.LastCachedTarget && centreDistance <= 25f)
								Weight += 400;

							// If there's a monster in the path-line to the item, reduce the weight
							if (ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(BotPosition, TestPosition)))
							{
								Weight *= 0.25f;
							}

							// Calculate a spot reaching a little bit further out from the globe, to help globe-movements
							//if (this.Weight>0)
							//    this.Position=MathEx.CalculatePointFrom(this.Position, Bot.Character_.Data.Position, this.CentreDistance+3f);
						}
						break;
				}
			}
			else
			{
				LastAvoidanceIgnored = DateTime.Now;

				//Skipped Due To Avoidances -- Gold and Globes, we blacklist them for a bit so we don't rerun the tests over and over!
				if (targetType.Value != TargetType.Item)
				{
					Weight = 0; //Zero will ignore the object completely! (Target)
					BlacklistLoops = 10;
				}
			}
		}

		public override bool ObjectIsValidForTargeting
		{
			get
			{
				if (!base.ObjectIsValidForTargeting)
					return false;



				//Z-Height Difference Check
				if (!IsZDifferenceValid)
				{
					BlacklistLoops = 3;
					return false;
				}

				if (targetType.Value == TargetType.Item)
				{
					if (!ShouldPickup.Value)
					{
						NeedsRemoved = true;
						BlacklistFlag = BlacklistType.Temporary;
						return false;
					}

					//Attempted to loot previously but failed due to inventory full.
					if (Bot.IsInNonCombatBehavior && TownRunManager.bFailedToLootLastItem) return false;


					// Ignore it if it's not in range yet - allow legendary items to have 15 feet extra beyond our profile max loot radius
					double dMultiplier = 1d;
					if (Itemquality >= ItemQuality.Rare4) dMultiplier += 0.25d;
					if (Itemquality >= ItemQuality.Legendary) dMultiplier += 0.45d;
					double lootDistance = LootRadius * dMultiplier;

					if (Bot.IsInNonCombatBehavior) lootDistance = Bot.Settings.Plugin.OutofCombatMaxDistance;

					float centredistance = CentreDistance;

					if (centredistance > lootDistance)
					{
						//Add to LOS Movement..
						if (Bot.Settings.Backtracking.TrackLootableItems && this.CentreDistance <= Bot.Settings.LOSMovement.MaximumRange)
						{
							LoopsUnseen = 0;
							Logger.Write(LogLevel.Items, "Adding Item {0} to LOS Movement Objects", InternalName);
							Bot.Targeting.Environment.LoSMovementObjects.Add(this);
						}

						return false;
					}

					//Check if we require LOS
					if (RequiresLOSCheck)
					{
						if (!LineOfSight.LOSTest(Bot.Character.Data.Position, true, false))
						{
							return false;
						}

						RequiresLOSCheck = false;
					}

					Bot.Targeting.Environment.bAnyLootableItemsNearby = true;
				}
				else
				{
					// Blacklist objects already in pickup radius range
					if (CentreDistance + 2.5f < Bot.Character.Data.PickupRadius)
					{
						NeedsRemoved = true;
						BlacklistFlag = BlacklistType.Temporary;
						Bot.Character.Data.UpdateCoinage = true;
						return false;
					}

					if (targetType == TargetType.Gold)
					{
						if (GoldAmount.Value < Bot.Settings.Loot.MinimumGoldPile)
						{
							NeedsRemoved = true;
							BlacklistFlag = BlacklistType.Temporary;
							Bot.Character.Data.UpdateCoinage = true;
							return false;
						}

						double lootRange = LootRadius;

						if (Bot.IsInNonCombatBehavior) lootRange = Bot.Settings.Plugin.OutofCombatMaxDistance;

						if (CentreDistance > lootRange)
						{
							BlacklistLoops = 20;
							return false;
						}
						Bot.Character.Data.UpdateCoinage = true;
					}
					else
					{
						//GLOBE
						// Ignore it if it's not in range yet
						if (CentreDistance > LootRadius)
						{
							//Blacklist Health Globes 10 loops
							if (targetType!=TargetType.PowerGlobe)
								BlacklistLoops = 10;

							return false;
						}
					}
				}


				return true;
			}
		}

		public override bool UpdateData()
		{


			if (ref_DiaItem == null)
			{
				try
				{
					ref_DiaItem = (DiaItem)ref_DiaObject;
				}
				catch
				{
					Logger.Write(LogLevel.Cache, "Failure to convert obj {0} to DiaItem!", InternalName);
					NeedsRemoved = true;
					return false;
				}
			}

			if (targetType.Value == TargetType.Item)
			{
				#region Item
				#region DynamicID
				if (!DynamicID.HasValue)
				{
					try
					{
						
						DynamicID = ref_DiaObject.CommonData.DynamicId;
					}
					catch
					{
						Logger.Write(LogLevel.Cache, "Failure to get Dynamic ID for {0}", InternalName);

						return false;
					}
				}
				#endregion

				//Gamebalance Update
				if (!BalanceID.HasValue)
				{
					try
					{
						BalanceID = ref_DiaObject.CommonData.GameBalanceId;
					}
					catch
					{
						Logger.Write(LogLevel.Cache, "Failure to get gamebalance ID for item {0}", InternalName);
						return false;
					}
				}

				//Check if game balance needs updated
				#region GameBalance
				if (BalanceData == null || BalanceData.bNeedsUpdated)
				{
					CacheBalance thisnewGamebalance;


					try
					{
						int balanceid = BalanceID.Value;
						int tmp_Level = ref_DiaItem.CommonData.Level;
						ItemType tmp_ThisType = ref_DiaItem.CommonData.ItemType;
						ItemBaseType tmp_ThisDBItemType = ref_DiaItem.CommonData.ItemBaseType;
						FollowerType tmp_ThisFollowerType = ref_DiaItem.CommonData.FollowerSpecialType;

						bool tmp_bThisOneHanded = false;
						bool tmp_bThisTwoHanded = false;
						if (tmp_ThisDBItemType == ItemBaseType.Weapon)
						{
							tmp_bThisOneHanded = ref_DiaItem.CommonData.IsOneHand;
							tmp_bThisTwoHanded = ref_DiaItem.CommonData.IsTwoHand;
						}

						thisnewGamebalance = new CacheBalance(balanceid, itemlevel: tmp_Level, itemtype: tmp_ThisType, itembasetype: tmp_ThisDBItemType, onehand: tmp_bThisOneHanded, twohand: tmp_bThisTwoHanded, followertype: tmp_ThisFollowerType);
					}
					catch
					{
						Logger.Write(LogLevel.Cache, "Failure to add/update gamebalance data for item {0}", InternalName);
						return false;
					}


					if (BalanceData == null)
					{
						CacheIDLookup.dictGameBalanceCache.Add(BalanceID.Value, thisnewGamebalance);
					}
					else
					{
						CacheIDLookup.dictGameBalanceCache[BalanceID.Value] = thisnewGamebalance;
					}

				}
				#endregion

				//Item Quality / Recheck
				#region ItemQuality
				if (!Itemquality.HasValue || ItemQualityRechecked == false)
				{

					try
					{
						Itemquality = ref_DiaItem.CommonData.ItemQualityLevel;
					}
					catch
					{
						Logger.Write(LogLevel.Cache, "Failure to get item quality for {0}", InternalName);
						return false;
					}


					if (!ItemQualityRechecked)
						ItemQualityRechecked = true;
					else
						NeedsUpdate = false;
				}
				#endregion


				//Pickup?
				// Now see if we actually want it
				#region PickupValidation
				if (!ShouldPickup.HasValue)
				{
					//Logger.DBLog.InfoFormat Dropped Items Here!!
					Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.DroppedItemLog(this);
					Bot.Character.ItemPickupEval(this);
					//if (Bot.Settings.ItemRules.UseItemRules)
					//{
					//	Interpreter.InterpreterAction action = Bot.ItemRulesEval.checkPickUpItem(this, ItemEvaluationType.PickUp);
					//	switch (action)
					//	{
					//		case Interpreter.InterpreterAction.PICKUP:
					//			ShouldPickup = true;
					//			break;
					//		case Interpreter.InterpreterAction.IGNORE:
					//			ShouldPickup = false;
					//			break;
					//	}
					//}

					//if (!ShouldPickup.HasValue)
					//{
					//	//Use Giles Scoring or DB Weighting..
					//	ShouldPickup =
					//		   Bot.Settings.ItemRules.ItemRuleGilesScoring ? Backpack.GilesPickupItemValidation(this)
					//		 : ItemManager.Current.EvaluateItem(ref_DiaItem.CommonData, ItemEvaluationType.PickUp); ;
					//}
				}
				else
				{
					NeedsUpdate = false;
				}
				#endregion

				#endregion
			}
			else
			{
				#region Gold
				//Get gold value..
				if (!GoldAmount.HasValue)
				{
					try
					{
						GoldAmount = ref_DiaItem.CommonData.GetAttribute<int>(ActorAttributeType.Gold);
					}
					catch
					{
						Logger.Write(LogLevel.Cache, "Failure to get gold amount for gold pile!");
						return false;
					}
				}
				Bot.Character.Data.UpdateCoinage = true;
				NeedsUpdate = false;
				#endregion
			}
			return true;
		}

		public override bool IsStillValid()
		{
			if (ref_DiaItem == null || !ref_DiaItem.IsValid || ref_DiaItem.BaseAddress == IntPtr.Zero)
				return false;

			return base.IsStillValid();
		}

		public override RunStatus Interact()
		{
			//Only validate if we can pickup if slots are minimum
			if (Bot.Character.Data.FreeBackpackSlots <= 8)
			{
				if (ref_DiaItem != null && ref_DiaItem.BaseAddress != IntPtr.Zero)
				{

					if (!BrainBehavior.CanPickUpItem(ref_DiaItem))
					{
						TownRunManager.bFailedToLootLastItem = true;
						BrainBehavior.ForceTownrun("No more space to pickup item");
						return RunStatus.Success;
					}


				}
				else
				{
					return RunStatus.Success;
				}
			}

			// Force waiting for global cooldown timer or long-animation abilities
			if (Bot.Character.Class.PowerPrime.WaitLoopsBefore >= 1)
			{
				//Logger.DBLog.DebugFormat("Debug: Force waiting BEFORE Ability " + powerPrime.powerThis.ToString() + "...");
				Bot.Targeting.bWaitingForPower = true;
				if (Bot.Character.Class.PowerPrime.WaitLoopsBefore >= 1)
					Bot.Character.Class.PowerPrime.WaitLoopsBefore--;
				return RunStatus.Running;
			}
			Bot.Targeting.bWaitingForPower = false;

			if (!Bot.Targeting.ShouldCheckItemLooted)
			{
				Bot.Targeting.ShouldCheckItemLooted = true;
				Bot.Character.Data.BackPack.Update();
				if (Bot.Character.Data.BackPack.CacheItemList.ContainsKey(AcdGuid.Value))
				{
					Bot.Targeting.CheckItemLootStackCount = Bot.Character.Data.BackPack.CacheItemList[AcdGuid.Value].ThisItemStackQuantity;
				}
			}

			// Pick the item up the usepower way, and "blacklist" for a couple of loops
			Bot.Character.Data.WaitWhileAnimating(20);
			ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, Vector3.Zero, Bot.Character.Data.iCurrentWorldID, AcdGuid.Value);
			Bot.NavigationCache.lastChangedZigZag = DateTime.Today;
			Bot.NavigationCache.vPositionLastZigZagCheck = Vector3.Zero;




			Bot.Character.Data.WaitWhileAnimating(5, true);
			return RunStatus.Running;
		}

		public override bool WithinInteractionRange()
		{
			float fRangeRequired;
			float fDistanceReduction = 0f;

			if (targetType.Value == TargetType.Item)
			{
				fRangeRequired = 5f;
				fDistanceReduction = 0f;
			}
			else
			{
				if (targetType.Value == TargetType.Gold)
				{
					fRangeRequired = Bot.Character.Data.PickupRadius;
					if (fRangeRequired == 0f)
						fRangeRequired = 0.5f;
				}
				else
				{
					fRangeRequired = Bot.Character.Data.PickupRadius;
					if (fRangeRequired == 0f)
						fRangeRequired = 0.5f;
					if (fRangeRequired > 5f)
						fRangeRequired = 5f;
				}
			}

			DistanceFromTarget = Bot.Character.Data.Position.Distance2D(Position) - fDistanceReduction;
			return (fRangeRequired <= 0f || base.DistanceFromTarget <= fRangeRequired);
		}

		public override string DebugString
		{
			get
			{
				return String.Format("{0}\r\n InteractAttempts={1} {2} {3}",
					  base.DebugString, InteractionAttempts,
					  GoldAmount.HasValue ? "Gold:" + GoldAmount.Value : "",
					  ShouldPickup.HasValue ? "PickUp=" + ShouldPickup.Value : "");
			}
		}

	}

}