using System;
using System.Linq;
using Zeta;
using System.Windows;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.CommonBot;
using Zeta.TreeSharp;
using System.Collections.Generic;
using FunkyTrinity.Enums;
using FunkyTrinity.Cache;
using FunkyTrinity.Movement;
using Zeta.Internals.SNO;

namespace FunkyTrinity.Cache
{

		  public class CacheItem : CacheObject
		  {
				public CacheItem(CacheObject baseobj)
					 : base(baseobj)
				{
					 
				}

				private bool requiresLOSCheck=false;
				public override bool RequiresLOSCheck
				{
					 get { return requiresLOSCheck; }
					 set { requiresLOSCheck=value; }
				}

				public DiaItem ref_DiaItem { get; set; }
				public int? DynamicID { get; set; }
				public ItemQuality? Itemquality { get; set; }
				public bool ItemQualityRechecked { get; set; }
				public GilesItemType GilesItemType { get; set; }
				public bool? ShouldPickup { get; set; }
				private DateTime LastAvoidanceIgnored=DateTime.Today;

				[System.Diagnostics.DebuggerNonUserCode]
				public int? GoldAmount { get; set; }

				public int? BalanceID { get; set; }



				public CacheBalance BalanceData
				{
					 get
					 {
						  if (BalanceID.HasValue&&CacheIDLookup.dictGameBalanceCache.ContainsKey(BalanceID.Value))
								 return CacheIDLookup.dictGameBalanceCache[BalanceID.Value];
						  else
								return null;
					 }
				}

				public override bool IsZDifferenceValid
				{
					 get
					 {
							float fThisHeightDifference=Funky.Difference(Bot.Character.Position.Z, this.Position.Z);

						  if (this.targetType.HasValue&&this.targetType.Value==TargetType.Item)
						  {
								if (fThisHeightDifference>=26f)
									 return false;
						  }
						  else
						  {
								// Gold/Globes at 11+ z-height difference
								if (fThisHeightDifference>=11f)
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

					 Vector3 TestPosition=this.Position;

					 //Use modified Test Position for Gold/Globe
					 if ((TargetType.Globe|TargetType.Gold).HasFlag(this.targetType.Value))
						  TestPosition=MathEx.CalculatePointFrom(Bot.Character.Position,this.Position,Math.Max(0f,this.CentreDistance-Bot.Character.PickupRadius));

					 if (this.CentreDistance>=2f)
					 {
						  //If we are already ignored this recently.. lets just assume its still being ignored!
						  if (DateTime.Now.Subtract(LastAvoidanceIgnored).TotalMilliseconds<1000&&Bot.Combat.NearbyAvoidances.Count>0)
						  {
								this.Weight=1;
						  }
						  else
						  {
								//Test if this object is within any avoidances
								if (ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(TestPosition))
									 this.Weight=1;

								//Test intersection of avoidances
								else if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(TestPosition))
									 this.Weight=1;



						  }
					 }

					 if (this.Weight!=1)
					 {

						  Vector3 BotPosition=Bot.Character.Position;
						  float centreDistance=BotPosition.Distance(TestPosition);

						  switch (this.targetType.Value)
						  {

								case TargetType.Item:
									 //this.Weight=(Bot.ItemRange*275)-(Math.Floor(centreDistance)*1000d);
									 this.Weight=15000d-(Math.Floor(centreDistance)*190d);
									 // Point-blank items get a weight increase 
									 if (centreDistance<=12f)
										  this.Weight+=600d;
									 // Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
									 if (this==Bot.Character.LastCachedTarget)
										  this.Weight+=600;
									 // Give yellows more weight
									 if (this.Itemquality.Value>=ItemQuality.Rare4)
										  this.Weight+=6000d;
									 // Give legendaries more weight
									 if (this.Itemquality.Value>=ItemQuality.Legendary)
									 {
										  this.Weight+=10000d;

										  double rangelimitRatio=this.CentreDistance/Bot.ItemRange;
										  if (rangelimitRatio>0.5d&&this.PriorityCounter==0)
										  {
												//prioritize!
												this.Weight+=25000d;
												this.PrioritizedDate=DateTime.Now;
												this.PriorityCounter=5;
										  }
									 }
									 // Are we prioritizing close-range stuff atm? If so limit it at a value 3k lower than monster close-range priority
									 if ((Bot.Combat.bForceCloseRangeTarget||Bot.Character.bIsRooted))
										  this.Weight=18000-(Math.Floor(centreDistance)*200);
									 // If there's a monster in the path-line to the item, reduce the weight
									 if (ObjectCache.Obstacles.Monsters.Any(cp => cp.PointInside(this.Position)))
										  this.Weight*=0.25;
									 //Finally check if we should reduce the weight when more then 2 monsters are nearby..
									 if (Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_25]>2&&
										  //But Only when we are low in health..
											 (Bot.Character.dCurrentHealthPct<0.25||
										  //Or we havn't changed targets after 2.5 secs
											 DateTime.Now.Subtract(Bot.Combat.dateSincePickedTarget).TotalSeconds>2.5))
										  this.Weight*=0.10;

									 //Test if there are nearby units that will trigger kite action..
									 if (Bot.Character.ShouldFlee)
									 {
										  if (ObjectCache.Objects.OfType<CacheUnit>().Any(m=>m.ShouldBeKited&&m.IsPositionWithinRange(this.Position, Bot.SettingsFunky.FleeMaxMonsterDistance)))
												this.Weight=1;
									 }

									 //Did we have a target last time? and if so was it a goblin?
									 if (Bot.Character.LastCachedTarget.RAGUID!=-1)
									 {
										  if (CacheIDLookup.hashActorSNOGoblins.Contains(Bot.Character.LastCachedTarget.RAGUID))
												this.Weight=0;
									 }
									 break;
								case TargetType.Gold:
									 if (this.GoldAmount>0)
										  this.Weight=11000d-(Math.Floor(centreDistance)*200d);
									 // Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
									 if (this==Bot.Character.LastCachedTarget&&centreDistance<=25f)
										  this.Weight+=600;
									 // Are we prioritizing close-range stuff atm? If so limit it at a value 3k lower than monster close-range priority
									 if ((Bot.Combat.bForceCloseRangeTarget||Bot.Character.bIsRooted))
										  this.Weight=18000-(Math.Floor(centreDistance)*200);
									 // If there's a monster in the path-line to the item, reduce the weight by 25%
									 if (ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(this, BotPosition)))
										  this.Weight*=0.75;
									 //Did we have a target last time? and if so was it a goblin?
									 if (Bot.Character.LastCachedTarget.RAGUID!=-1)
									 {
										  if (CacheIDLookup.hashActorSNOGoblins.Contains(Bot.Character.LastCachedTarget.RAGUID))
												this.Weight=0;
									 }
									 break;
								case TargetType.Globe:
									 if (Bot.Character.dCurrentHealthPct>Bot.EmergencyHealthGlobeLimit)
									 {
										  this.Weight=0;
									 }
									 else
									 {
										  // Ok we have globes enabled, and our health is low...!
										  this.Weight=17000d-(Math.Floor(centreDistance)*90d);

										  // Point-blank items get a weight increase
										  if (centreDistance<=15f)
												this.Weight+=3000d;

										  // Close items get a weight increase
										  if (centreDistance<=60f)
												this.Weight+=1500d;

										  // Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
										  if (this==Bot.Character.LastCachedTarget&&centreDistance<=25f)
												this.Weight+=400;

										  // If there's a monster in the path-line to the item, reduce the weight
										  if (ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(BotPosition, TestPosition, true)))
										  {
												this.Weight*=0.25f;
										  }

										  // Calculate a spot reaching a little bit further out from the globe, to help globe-movements
										  //if (this.Weight>0)
										  //    this.Position=MathEx.CalculatePointFrom(this.Position, Bot.Character.Position, this.CentreDistance+3f);
									 }
									 break;
						  }
					 }
					 else
					 {
						  LastAvoidanceIgnored=DateTime.Now;

						  //Skipped Due To Avoidances -- Gold and Globes, we blacklist them for a bit so we don't rerun the tests over and over!
						  if (this.targetType.Value!=TargetType.Item)
						  {
								this.Weight=0; //Zero will ignore the object completely! (Target)
								this.BlacklistLoops=10;
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
						  if (!this.IsZDifferenceValid)
						  {
								this.BlacklistLoops=3;
								return false;
						  }

						  if (this.targetType.Value==TargetType.Item)
						  {
								if (!this.ShouldPickup.Value)
								{
									 this.NeedsRemoved=true;
									 this.BlacklistFlag=BlacklistType.Temporary;
									 return false;
								}

								//Attempted to loot previously but failed due to inventory full.
								if (Bot.IsInNonCombatBehavior&&Funky.TownRunManager.bFailedToLootLastItem) return false;


								// Ignore it if it's not in range yet - allow legendary items to have 15 feet extra beyond our profile max loot radius
								double dMultiplier=1d;
								if (this.Itemquality>=ItemQuality.Rare4) dMultiplier+=0.25d;
								if (this.Itemquality>=ItemQuality.Legendary) dMultiplier+=0.45d;
								double lootDistance=Bot.ItemRange*dMultiplier;

								if (Bot.IsInNonCombatBehavior) lootDistance=50f;

								float centredistance=this.CentreDistance;

								if (centredistance>lootDistance) 
									 return false;


								//Check if we require LOS
								if (this.RequiresLOSCheck)
								{
									 Vector3 testPosition=new Vector3(this.Position.X, this.Position.Y, this.Position.Z+1f);
									 if (!Navigation.CanRayCast(Bot.Character.Position, testPosition, NavCellFlags.AllowWalk))
									 {
										  int blacklistloopCount=50;
										  if (this.CentreDistance<25f)
												blacklistloopCount/=2;
										  if (this.Itemquality>=ItemQuality.Rare4)
												blacklistloopCount/=4;

										  return false;
									 }

									 this.RequiresLOSCheck=false;
								}

						  }
						  else
						  {
								// Blacklist objects already in pickup radius range
								if (this.CentreDistance+2.5f<Bot.Character.PickupRadius)
								{
									 this.NeedsRemoved=true;
									 this.BlacklistFlag=BlacklistType.Temporary;
									 return false;
								}

								if (this.targetType==TargetType.Gold)
								{
									 if (this.GoldAmount.Value<Bot.SettingsFunky.MinimumGoldPile)
									 {
										  this.NeedsRemoved=true;
										  this.BlacklistFlag=BlacklistType.Temporary;
										  return false;
									 }

									 float lootRange=Bot.GoldRange;

									 if (Bot.IsInNonCombatBehavior) lootRange=50f;

									 if (this.CentreDistance>lootRange)
									 {
										  this.BlacklistLoops=20;
										  return false;
									 }
								}
								else
								{
									 //GLOBE
									 // Ignore it if it's not in range yet
									 if (this.CentreDistance>Bot.iCurrentMaxLootRadius||this.CentreDistance>Bot.GlobeRange)
									 {
										  this.BlacklistLoops=20;
										  return false;
									 }
								}
						  }


						  return true;
					 }
				}

				public override bool UpdateData()
				{


					 if (this.ref_DiaItem==null)
					 {
						  try
						  {
								this.ref_DiaItem=(DiaItem)base.ref_DiaObject;
						  } catch (NullReferenceException) { Logging.WriteVerbose("Failure to convert obj to DiaItem!"); return false; }
					 }

					 if (this.targetType.Value==TargetType.Item)
					 {
						  #region Item
						  #region DynamicID
						  if (!this.DynamicID.HasValue)
						  {
								try
								{
									 this.DynamicID=base.ref_DiaObject.CommonData.DynamicId;
								} catch (NullReferenceException ex) { Logger.Write(LogLevel.Exception, "Failure to get Dynamic ID for {0} \r\n Exception: {1}", this.InternalName, ex.Message); return false; }
						  }
						  #endregion

						  //Gamebalance Update
						  if (!this.BalanceID.HasValue)
						  {
								try
								{
									 this.BalanceID=base.ref_DiaObject.CommonData.GameBalanceId;
								} catch (NullReferenceException) { Logger.Write(LogLevel.Exception, "Failure to get gamebalance ID for item {0}", this.InternalName); return false; }
						  }

						  if (!this.BalanceID.HasValue) return false;

						  //Check if game balance needs updated
						  #region GameBalance
						  if (this.BalanceData==null||this.BalanceData.bNeedsUpdated)
						  {
								CacheBalance thisnewGamebalance;


								try
								{
									 int tmp_Level=this.ref_DiaItem.CommonData.Level;
									 ItemType tmp_ThisType=this.ref_DiaItem.CommonData.ItemType;
									 ItemBaseType tmp_ThisDBItemType=this.ref_DiaItem.CommonData.ItemBaseType;
									 FollowerType tmp_ThisFollowerType=this.ref_DiaItem.CommonData.FollowerSpecialType;

									 bool tmp_bThisOneHanded=false;
									 bool tmp_bThisTwoHanded=false;
									 if (tmp_ThisDBItemType==ItemBaseType.Weapon)
									 {
										  tmp_bThisOneHanded=this.ref_DiaItem.CommonData.IsOneHand;
										  tmp_bThisTwoHanded=this.ref_DiaItem.CommonData.IsTwoHand;
									 }

									 thisnewGamebalance=new CacheBalance(tmp_Level, tmp_ThisType, tmp_ThisDBItemType, tmp_bThisOneHanded, tmp_bThisTwoHanded, tmp_ThisFollowerType);
								} catch (NullReferenceException)
								{
									 Logger.Write(LogLevel.Exception, "Failure to add/update gamebalance data for item {0}", this.InternalName);
									 return false;
								}


								if (this.BalanceData==null)
								{
									 CacheIDLookup.dictGameBalanceCache.Add(this.BalanceID.Value, thisnewGamebalance);
								}
								else
								{
									 CacheIDLookup.dictGameBalanceCache[this.BalanceID.Value]=thisnewGamebalance;
								}

						  }
						  #endregion

						  //Item Quality / Recheck
						  #region ItemQuality
						  if (!this.Itemquality.HasValue||this.ItemQualityRechecked==false)
						  {

								try
								{
									 this.Itemquality=this.ref_DiaItem.CommonData.ItemQualityLevel;
								} catch (NullReferenceException) { Logger.Write(LogLevel.Exception, "Failure to get item quality for {0}", this.InternalName); return false; }


								if (!this.ItemQualityRechecked)
									 this.ItemQualityRechecked=true;
								else
									 this.NeedsUpdate=false;
						  }
						  #endregion


						  //Pickup?
						  // Now see if we actually want it
						  #region PickupValidation
						  if (!this.ShouldPickup.HasValue)
						  {
								if (Bot.SettingsFunky.UseItemRules)
								{
									 Interpreter.InterpreterAction action=Funky.ItemRulesEval.checkPickUpItem(this, ItemEvaluationType.PickUp);
									 switch (action)
									 {
										  case Interpreter.InterpreterAction.PICKUP:
												this.ShouldPickup=true;
												break;
										  case Interpreter.InterpreterAction.IGNORE:
												this.ShouldPickup=false;
												break;
									 }
								}

								if (!this.ShouldPickup.HasValue)
								{
									 //Use Giles Scoring or DB Weighting..
									 this.ShouldPickup=
											Bot.SettingsFunky.ItemRuleGilesScoring?Funky.GilesPickupItemValidation(this)
										  :ItemManager.Current.EvaluateItem((ACDItem)this.ref_DiaItem.CommonData, Zeta.CommonBot.ItemEvaluationType.PickUp); ;
								}

								//Low Level Evaluation
								if (Bot.SettingsFunky.UseLevelingLogic&&Bot.Character.iMyLevel<60)
								{
									 if (this.Itemquality.HasValue&&this.Itemquality.Value>=ItemQuality.Magic1)
									 {
										  //Check if we currently use this type of item.
											if (!CacheIDLookup.RestrictedItemTypes.Contains(this.BalanceData.thisItemType))
												this.ShouldPickup=true;
									 }
								}
						  }
						  else
								this.NeedsUpdate=false;

						  #endregion

						  #endregion
					 }
					 else
					 {
						  #region Gold
						  //Get gold value..
						  if (!this.GoldAmount.HasValue)
						  {
								try
								{
									 this.GoldAmount=this.ref_DiaItem.CommonData.GetAttribute<int>(ActorAttributeType.Gold);
								} catch (NullReferenceException) { Logger.Write(LogLevel.Exception, "Failure to get gold amount for gold pile!"); return false; }
						  }

						  this.NeedsUpdate=false;
						  #endregion
					 }
					 return true;
				}

				public override bool IsStillValid()
				{
					 if (ref_DiaItem==null||!ref_DiaItem.IsValid||ref_DiaItem.BaseAddress==IntPtr.Zero)
						  return false;

					 return base.IsStillValid();
				}

				public override RunStatus Interact()
				{
					 //Only validate if we can pickup if slots are minimum
					 if (Bot.Character.FreeBackpackSlots<=8)
					 {
						  if (this.ref_DiaItem!=null&&this.ref_DiaItem.BaseAddress!=IntPtr.Zero)
						  {

								if (!Zeta.CommonBot.Logic.BrainBehavior.CanPickUpItem(this.ref_DiaItem))
								{
									 Funky.TownRunManager.bFailedToLootLastItem=true;
									 Zeta.CommonBot.Logic.BrainBehavior.ForceTownrun("No more space to pickup item");
									 return RunStatus.Success;
								}


						  }
						  else
						  {
								return RunStatus.Success;
						  }
					 }

					 // Force waiting for global cooldown timer or long-animation abilities
					 if (Bot.Combat.powerPrime.WaitLoopsBefore>=1)
					 {
						  //Logging.WriteDiagnostic("Debug: Force waiting BEFORE ability " + powerPrime.powerThis.ToString() + "...");
						  Bot.Combat.bWaitingForPower=true;
						  if (Bot.Combat.powerPrime.WaitLoopsBefore>=1)
								Bot.Combat.powerPrime.WaitLoopsBefore--;
						  return Zeta.TreeSharp.RunStatus.Running;
					 }
					 Bot.Combat.bWaitingForPower=false;

					 // Pick the item up the usepower way, and "blacklist" for a couple of loops
					 Bot.Character.WaitWhileAnimating(20, false);
					 ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, Vector3.Zero, Bot.Character.iCurrentWorldID, this.AcdGuid.Value);
					 Bot.Combat.lastChangedZigZag=DateTime.Today;
					 Bot.Combat.vPositionLastZigZagCheck=Vector3.Zero;


					 Bot.Combat.ShouldCheckItemLooted=true;

					 Bot.Character.WaitWhileAnimating(5, true);
					 return Zeta.TreeSharp.RunStatus.Running;
				}

				public override bool WithinInteractionRange()
				{
					 float fRangeRequired=0f;
					 float fDistanceReduction=0f;

					 if (this.targetType.Value==TargetType.Item)
					 {
						  fRangeRequired=5f;
						  fDistanceReduction=0f;

						  // If we're having stuck issues, try forcing us to get closer to this item
						  if (Bot.Combat.bForceCloseRangeTarget)
								fRangeRequired-=1f;

							//if (Bot.Character.Position.Distance(TargetMovement.CurrentTargetLocation)<=1.5f)
							//   fDistanceReduction+=1f;
					 }
					 else
					 {
						  if (targetType.Value==TargetType.Gold)
						  {
								fRangeRequired=Bot.Character.PickupRadius;
								if (fRangeRequired<2f)
									 fRangeRequired=2f;
						  }
						  else
						  {
								fRangeRequired=Bot.Character.PickupRadius;
								if (fRangeRequired<2f)
									 fRangeRequired=2f;
								if (fRangeRequired>5f)
									 fRangeRequired=5f;
						  }
					 }

					 base.DistanceFromTarget=Vector3.Distance(Bot.Character.Position, this.Position)-fDistanceReduction;
					 return (fRangeRequired<=0f||base.DistanceFromTarget<=fRangeRequired);
				}

				public override string DebugString
				{
					 get
					 {
						  return String.Format("{0}\r\n InteractAttempts={1} {2} {3}",
								base.DebugString, this.InteractionAttempts.ToString(),
								this.GoldAmount.HasValue?"Gold:"+this.GoldAmount.Value.ToString():"",
								this.ShouldPickup.HasValue?"PickUp="+this.ShouldPickup.Value.ToString():"");
					 }
				}

		  }
	 
}