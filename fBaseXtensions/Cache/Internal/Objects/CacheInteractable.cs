using System;
using System.Linq;
using fBaseXtensions.Cache.Internal.Blacklist;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Settings;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;

namespace fBaseXtensions.Cache.Internal.Objects
{
	public class CacheInteractable : CacheGizmo
	{
		public CacheInteractable(CacheObject baseobj)
			: base(baseobj)
		{
		}

		public bool IsEventSwitch 
		{ 
			get 
			{ 
				return Gizmotype.HasValue && Gizmotype.Value == GizmoType.Switch && internalNameLower.Contains("event_switch"); 
			}
		}
		public override bool ObjectIsSpecial
		{
			get
			{
				//Rep Chests
				return IsResplendantChest && FunkyBaseExtension.Settings.Targeting.UseExtendedRangeRepChest;
			}
		}

		public override int InteractionRange
		{
			get
			{
                if (GizmoTargetTypes.HasValue)
                {

                    if (ObjectCache.CheckFlag(GizmoTargetTypes.Value, Enums.GizmoTargetTypes.Containers))
                    {
                        if (GizmoTargetTypes.Value == Enums.GizmoTargetTypes.Resplendant)
                            return FunkyBaseExtension.Settings.Ranges.ContainerOpenRange * 3;

                        return FunkyBaseExtension.Settings.Ranges.ContainerOpenRange;
                    }

                    if (GizmoTargetTypes.Value == Enums.GizmoTargetTypes.PoolOfReflection)
                        return FunkyBaseExtension.Settings.Ranges.PoolsOfReflectionRange;

                    if (GizmoTargetTypes.Value == Enums.GizmoTargetTypes.Healthwell)
                        return FunkyBaseExtension.Settings.Ranges.ShrineRange;

                    if (GizmoTargetTypes.Value == Enums.GizmoTargetTypes.Shrine)
                        return FunkyBaseExtension.Settings.Ranges.ShrineRange;

                    if (GizmoTargetTypes.Value == Enums.GizmoTargetTypes.PylonShrine)
                        return FunkyBaseExtension.Settings.Ranges.ShrineRange * 3;
                }


				if (targetType.Value == TargetType.Shrine)
				{
					if (Gizmotype == GizmoType.PoolOfReflection)
						return FunkyBaseExtension.Settings.Ranges.PoolsOfReflectionRange;
					 
					return IsHealthWell ? FunkyBaseExtension.Settings.Ranges.ShrineRange * 2 : FunkyBaseExtension.Settings.Ranges.ShrineRange;
				}

				if (targetType.Value == TargetType.Container)
				{
					if (IsResplendantChest && FunkyBaseExtension.Settings.Targeting.UseExtendedRangeRepChest)
						return FunkyBaseExtension.Settings.Ranges.ContainerOpenRange * 2;
					
					return FunkyBaseExtension.Settings.Ranges.ContainerOpenRange;
				}

				if (targetType.Value == TargetType.CursedShrine)
					return FunkyBaseExtension.Settings.Ranges.CursedShrineRange;
				
				if (targetType.Value == TargetType.CursedChest)
					return FunkyBaseExtension.Settings.Ranges.CursedChestRange;

				if (targetType.Value==TargetType.Door)
					return FunkyBaseExtension.Settings.Ranges.DoorRange;

				return (int)FunkyGame.Targeting.Cache.iCurrentMaxLootRadius;
			}
		}

		public override bool ObjectIsValidForTargeting
		{
			get
			{
				if (!base.ObjectIsValidForTargeting) return false;
				if (!targetType.HasValue) return false;


				//Ignore Settings Corpses
				if (FunkyBaseExtension.Settings.Targeting.IgnoreCorpses && IsCorpseContainer)
				{
					IgnoredType = TargetingIgnoreTypes.IgnoredTargetType;
					NeedsRemoved = true;
					BlacklistFlag = BlacklistType.Permanent;
					return false;
				}
				//Ignore Settings Armor Rack
				if (FunkyBaseExtension.Settings.Targeting.IgnoreArmorRacks && GizmoTargetTypes.HasValue && GizmoTargetTypes.Value == Enums.GizmoTargetTypes.ArmorRack)
				{
					IgnoredType = TargetingIgnoreTypes.IgnoredTargetType;
					NeedsRemoved = true;
					BlacklistFlag = BlacklistType.Permanent;
					return false;
				}
				//Ignore Settings Weapon Rack
				if (FunkyBaseExtension.Settings.Targeting.IgnoreWeaponRacks && GizmoTargetTypes.HasValue && GizmoTargetTypes.Value == Enums.GizmoTargetTypes.WeaponRack)
				{
					IgnoredType = TargetingIgnoreTypes.IgnoredTargetType;
					NeedsRemoved = true;
					BlacklistFlag = BlacklistType.Permanent;
					return false;
				}
				//Ignore Settings Floor Container
				if (FunkyBaseExtension.Settings.Targeting.IgnoreFloorContainers && GizmoTargetTypes.HasValue && GizmoTargetTypes.Value == Enums.GizmoTargetTypes.FloorContainer)
				{
					IgnoredType = TargetingIgnoreTypes.IgnoredTargetType;
					NeedsRemoved = true;
					BlacklistFlag = BlacklistType.Permanent;
					return false;
				}
				//Ignore Settings Normal Chest
				if (FunkyBaseExtension.Settings.Targeting.IgnoreNormalChests && GizmoTargetTypes.HasValue && GizmoTargetTypes.Value == Enums.GizmoTargetTypes.Chest)
				{
					IgnoredType = TargetingIgnoreTypes.IgnoredTargetType;
					NeedsRemoved = true;
					BlacklistFlag = BlacklistType.Permanent;
					return false;
				}
				//Ignore Settings Rare Chest
				if (FunkyBaseExtension.Settings.Targeting.IgnoreRareChests && GizmoTargetTypes.HasValue && GizmoTargetTypes.Value == Enums.GizmoTargetTypes.Resplendant)
				{
					IgnoredType = TargetingIgnoreTypes.IgnoredTargetType;
					NeedsRemoved = true;
					BlacklistFlag = BlacklistType.Permanent;
					return false;
				}
				//Ignore Settings Normal Shrines
				if (GizmoTargetTypes.HasValue && GizmoTargetTypes.Value == Enums.GizmoTargetTypes.Shrine)
				{
					ShrineTypes shrinetype = CacheIDLookup.FindShrineType(SNOID);
					if (shrinetype.HasFlag(ShrineTypes.Normal) && !FunkyBaseExtension.Settings.Targeting.UseShrineTypes[(int)shrinetype])
					{
						IgnoredType = TargetingIgnoreTypes.IgnoredTargetType;
						NeedsRemoved = true;
						BlacklistFlag = BlacklistType.Permanent;
						return false;
					}
				}

				float centreDistance = CentreDistance;
				float radiusDistance = RadiusDistance;

				// Ignore it if it's not in range yet
				if (centreDistance > InteractionRange)
				{
					IgnoredType = TargetingIgnoreTypes.DistanceFailure;
					BlacklistLoops = 10;
					return false;
				}

				if (GizmoHasBeenUsed.HasValue && GizmoHasBeenUsed.Value)
				{
					if (targetType.Value == TargetType.CursedShrine)
					{//Cursed Shrine/Chest that is still valid we reset to keep active!
						FunkyGame.Targeting.Cache.lastSeenCursedShrine = DateTime.Now;
						LoopsUnseen = 0;
					}
					else
					{
					    NeedsRemoved = true;
                        RemovalType= RemovalTypes.DeadorUsed;
					}
					IgnoredType = TargetingIgnoreTypes.GizmoHasBeenUsed;
					return false;
				}


				//Post Activation of Cursed Chest turns it into a "chest" gizmo (pre-activation is a switch)
				if (targetType.Value==TargetType.CursedChest && Gizmotype.HasValue && Gizmotype.Value == GizmoType.Chest)
				{
					FunkyGame.Targeting.Cache.lastSeenCursedShrine = DateTime.Now;
					LoopsUnseen = 0;
					return false;
				}


				if (RequiresLOSCheck && !IgnoresLOSCheck && radiusDistance>0f)
				{
					//Get the wait time since last used LOSTest
					double lastLOSCheckMS = base.LineOfSight.LastLOSCheckMS;

					//unless its in front of us.. we wait 500ms mandatory.
					if (lastLOSCheckMS < 500 && centreDistance > 1f)
					{

						if ((IsResplendantChest && SettingLOSMovement.LOSSettingsTag.AllowRareLootContainer) ||
							((IsCursedChest||IsCursedShrine) && SettingLOSMovement.LOSSettingsTag.AllowCursedChestShrines)||
							(IsEventSwitch && SettingLOSMovement.LOSSettingsTag.AllowEventSwitches))
						{
							//if (FunkyBaseExtension.Settings.Debugging.FunkyLogFlags.HasFlag(LogLevel.Target))
							//	Logger.Write(LogLevel.Target, "Adding {0} to LOS Movement Objects", InternalName);
							FunkyGame.Targeting.Cache.Environment.LoSMovementObjects.Add(this);
						}
						IgnoredType = TargetingIgnoreTypes.LineOfSightFailure;
						return false;
					}
					else
					{
						//Set the maximum wait time
						double ReCheckTime = 3500;

						//Close Range.. we change the recheck timer based on how close
						if (CentreDistance < 25f)
							ReCheckTime = (CentreDistance * 125);
						else if (ObjectIsSpecial)
							ReCheckTime *= 0.25;

						if (lastLOSCheckMS < ReCheckTime)
						{
							if ((IsResplendantChest && SettingLOSMovement.LOSSettingsTag.AllowRareLootContainer) ||
								((IsCursedChest || IsCursedShrine) && SettingLOSMovement.LOSSettingsTag.AllowCursedChestShrines) ||
								 (IsEventSwitch && SettingLOSMovement.LOSSettingsTag.AllowEventSwitches))
							{
								//if (FunkyBaseExtension.Settings.Debugging.FunkyLogFlags.HasFlag(LogLevel.Target))
								//   Logger.Write(LogLevel.Target, "Adding {0} to LOS Movement Objects", InternalName);
								FunkyGame.Targeting.Cache.Environment.LoSMovementObjects.Add(this);
							}
							IgnoredType = TargetingIgnoreTypes.LineOfSightFailure;
							return false;
						}
					}

					//var testPosition = (Gizmotype.Value == GizmoType.Switch || Gizmotype.Value==GizmoType.Door) ? BotMeleeVector : Position;
					//if (!LineOfSight.LOSTest(FunkyGame.Hero.Position, testPosition, Gizmotype.Value!=GizmoType.Switch, Gizmotype.Value==GizmoType.Switch, false))
					
					if (!LineOfSight.LOSTest(FunkyGame.Hero.Position, BotMeleeVector, true, true, false))
					{
						if ((IsResplendantChest && SettingLOSMovement.LOSSettingsTag.AllowRareLootContainer) ||
							((IsCursedChest || IsCursedShrine) && SettingLOSMovement.LOSSettingsTag.AllowCursedChestShrines) ||
							(IsEventSwitch && SettingLOSMovement.LOSSettingsTag.AllowEventSwitches))
						{
							//if (FunkyBaseExtension.Settings.Debugging.FunkyLogFlags.HasFlag(LogLevel.Target))
							//	Logger.Write(LogLevel.Target, "Adding {0} to LOS Movement Objects", InternalName);
							FunkyGame.Targeting.Cache.Environment.LoSMovementObjects.Add(this);
						}
						IgnoredType = TargetingIgnoreTypes.LineOfSightFailure;
						return false;
					}

					RequiresLOSCheck = false;
				}



				// Now for the specifics
				switch (targetType.Value)
				{
					#region Interactable
					case TargetType.Interactable:
					case TargetType.Door:


                        //if (targetType.Value == TargetType.Door
                        //       && PriorityCounter == 0
                        //       && radiusDistance > 5f)
                        //{
                        //    Vector3 BarricadeTest = Position;
                        //    if (radiusDistance > 1f)
                        //    {

                        //        BarricadeTest = MathEx.GetPointAt(Position, 10f, Navigation.Navigation.FindDirection(FunkyGame.Hero.Position, Position, true));
                        //        bool intersectionTest = !MathEx.IntersectsPath(Position, CollisionRadius.Value, FunkyGame.Hero.Position, BarricadeTest);
                        //        if (!intersectionTest)
                        //        {
                        //            IgnoredType = TargetingIgnoreTypes.IntersectionFailed;
                        //            return false;
                        //        }

                        //    }
                        //}

						if (centreDistance > 60f)
						{
							IgnoredType = TargetingIgnoreTypes.DistanceFailure;
							BlacklistLoops = 3;
							return false;
						}

                        
						return true;
					#endregion
					#region Shrine
					case TargetType.Shrine:

						if (IsHealthWell)
						{
							//Health wells..
							if (FunkyGame.Hero.dCurrentHealthPct > FunkyBaseExtension.Settings.Combat.HealthWellHealthPercent)
							{
								BlacklistLoops = 5;
								return false;
							}
						}


						// Bag it!
						Radius = 5.1f;
						break;
					#endregion
					#region Container
					case TargetType.Container:

						//Vendor Run and DB Settings check
						if ((!IsChestContainer && !CharacterSettings.Instance.OpenLootContainers)
							 || (IsChestContainer && !CharacterSettings.Instance.OpenChests))
						{
							IgnoredType = TargetingIgnoreTypes.IgnoredTargetType;
							NeedsRemoved = true;
							BlacklistFlag = BlacklistType.Permanent;
							return false;
						}

						

						// Superlist for rare chests etc.
						if (IsResplendantChest)
						{
							//setup wait time. (Unlike Units, we blacklist right after we interact)
							if (FunkyGame.Targeting.Cache.LastCachedTarget.Equals(this))
							{
								FunkyGame.Targeting.Cache.lastHadContainerAsTarget = DateTime.Now;
								FunkyGame.Targeting.Cache.lastHadRareChestAsTarget = DateTime.Now;
							}
						}

						// Bag it!
						if (IsChestContainer)
							Radius = 5.1f;
						else
							Radius = 4f;

						break;
					#endregion
				} // Object switch on type (to seperate shrines, destructibles, barricades etc.)




				return true;

			}
		}

		public override void UpdateWeight()
		{
			base.UpdateWeight();

			if (CentreDistance >= 4f && FunkyGame.Targeting.Cache.Environment.NearbyAvoidances.Count > 0)
			{
				Vector3 TestPosition = Position;
				if (ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(TestPosition))
					Weight = 1;
				else if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(TestPosition)) //intersecting avoidances..
					Weight = 1;
			}

			if (Weight != 1)
			{
				float centreDistance = CentreDistance;
				Vector3 BotPosition = FunkyGame.Hero.Position;
				switch (targetType.Value)
				{
					case TargetType.Shrine:
						Weight = 14500d - (Math.Floor(centreDistance) * 170d);
						// Very close shrines get a weight increase
						if (centreDistance <= 20f)
							Weight += 1000d;

						// health pool
						if (base.IsHealthWell)
						{
							if (FunkyGame.Hero.dCurrentHealthPct > 0.75d)
								Weight = 0;
							else
								//Give weight based upon current health percent.
								Weight += 1000d / (FunkyGame.Hero.dCurrentHealthPct);
						}
						else if (this.Gizmotype.Value == GizmoType.PoolOfReflection)
						{
							Weight += 1000;
						}

						if (Weight > 0)
						{
							// Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
							if (Equals(FunkyGame.Targeting.Cache.LastCachedTarget))
								Weight += 600;
							// Are we prioritizing close-range stuff atm? If so limit it at a value 3k lower than monster close-range priority
							if (FunkyGame.Hero.bIsRooted)
								Weight = 18500d - (Math.Floor(centreDistance) * 200);
							// If there's a monster in the path-line to the item, reduce the weight by 25%
							if (!Equipment.NoMonsterCollision && ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(this, BotPosition)))
								Weight *= 0.75;
						}
						break;
					case TargetType.Interactable:
					case TargetType.Door:
						Weight = 15000d - (Math.Floor(centreDistance) * 170d);
						if (centreDistance <= 30f && RadiusDistance <= 5f)
							Weight += 8000d;
						// Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
						if (Equals(FunkyGame.Targeting.Cache.LastCachedTarget) && centreDistance <= 25f)
							Weight += 1000;

                        //Check doors against intersection of current target unit when using ranged skill
				        if (targetType.Value == TargetType.Door)
				        {
				            if (FunkyGame.Targeting.Cache.CurrentTarget != null &&
				                FunkyGame.Targeting.Cache.CurrentUnitTarget != null &&
				                FunkyGame.Hero.Class.LastUsedAbility.IsRanged)
				            {
				                if (MathEx.IntersectsPath(Position, CollisionRadius.Value, FunkyGame.Hero.Position,
				                    FunkyGame.Targeting.Cache.CurrentTarget.Position))
				                {
				                    Helpers.Logger.DBLog.InfoFormat("[Funky] Door Blocking current target when using ranged skill!");
				                    Weight += 10000;
				                }
				            }
				        }
				        else
				        {
				            if (GizmoTargetTypes.HasValue && 
                                GizmoTargetTypes.Value == Enums.GizmoTargetTypes.Bounty &&
                                centreDistance<25f)
				            {
				                Weight += 10000;
				            }
				        }
						

						break;
					case TargetType.Container:
						Weight = 11000d - (Math.Floor(centreDistance) * 190d);
						if (centreDistance <= 12f)
							Weight += 600d;
						// Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
						if (Equals(FunkyGame.Targeting.Cache.LastCachedTarget) && centreDistance <= 25f)
						{
							Weight += 400;
						}
						// If there's a monster in the path-line to the item, reduce the weight by 50%
						if (!Equipment.NoMonsterCollision && ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(this, BotPosition)))
						{
							Weight *= 0.5;
						}
						if (IsResplendantChest)
							Weight += 1500;

						break;
					case TargetType.CursedShrine:
					case TargetType.CursedChest:
						Weight = 11000d - (Math.Floor(centreDistance) * 190d);
						break;
				}

                //Special Custom Weight Check (From Profile Tags)
                if (FunkyGame.Game.ObjectCustomWeights.ContainsKey(SNOID))
                    Weight += FunkyGame.Game.ObjectCustomWeights[SNOID];
			}
			else
			{
				Weight = 0;
				BlacklistLoops = 15;
			}
		}

		public override bool IsZDifferenceValid
		{
			get
			{
				float fThisHeightDifference = FunkyBaseExtension.Difference(FunkyGame.Hero.Position.Z, Position.Z);
				if (fThisHeightDifference >= 15f)
				{
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

		public override RunStatus Interact()
		{
			// Force waiting for global cooldown timer or long-animation abilities
			if (FunkyGame.Hero.Class.PowerPrime.WaitLoopsBefore >= 1)
			{
				//Logger.DBLog.DebugFormat("Debug: Force waiting BEFORE Ability " + powerPrime.powerThis.ToString() + "...");
				FunkyGame.Targeting.Cache.bWaitingForPower = true;
				if (FunkyGame.Hero.Class.PowerPrime.WaitLoopsBefore >= 1)
					FunkyGame.Hero.Class.PowerPrime.WaitLoopsBefore--;
				return RunStatus.Running;
			}
			FunkyGame.Targeting.Cache.bWaitingForPower = false;


			FunkyGame.Hero.WaitWhileAnimating(20);
			ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, Position, FunkyGame.Hero.CurrentWorldDynamicID, base.AcdGuid.Value);
			InteractionAttempts++;

			if (IsCursedShrine || IsCursedChest)
			{
				FunkyGame.Targeting.Cache.lastSeenCursedShrine = DateTime.Now;
			}

			if (InteractionAttempts == 1)
			{
				FunkyGame.Targeting.Cache.bWaitingAfterPower = true;
			}


			// Wait!!
			if (ObjectCache.CheckFlag(targetType.Value, TargetType.Interactable))
			{
				FunkyGame.Hero.WaitWhileAnimating(1500);
			}
			else if(ObjectCache.CheckFlag(targetType.Value,TargetType.Door))
			{
				FunkyGame.Hero.WaitWhileAnimating(500);
			}
			else
			{
				FunkyGame.Hero.WaitWhileAnimating(75);
			}
			

			// If we've tried interacting too many times, blacklist this for a while
			if (InteractionAttempts > 5 && !IsCursedShrine && !IsCursedChest)
			{
				if (Gizmotype == GizmoType.Door)
				{
					BlacklistLoops = 20;
				}
				else
				{
					BlacklistLoops = 50;
				}
				
				InteractionAttempts = 0;
			}

			if (!FunkyGame.Targeting.Cache.bWaitingAfterPower)
			{
				// Now tell Trinity to get a new target!
				FunkyGame.Navigation.lastChangedZigZag = DateTime.Today;
				FunkyGame.Navigation.vPositionLastZigZagCheck = Vector3.Zero;
				FunkyGame.Targeting.Cache.bForceTargetUpdate = true;
			}
			return RunStatus.Running;
		}

		public override bool WithinInteractionRange()
		{
			float fRangeRequired = 0f;
			float fDistanceReduction = 0f;

			if (targetType.Value == TargetType.Interactable)
			{
				// Treat the distance as closer based on the radius of the object
				//fDistanceReduction=ObjectData.Radius;
				fRangeRequired = CollisionRadius.Value * 0.95f;

				// Check if it's in our interactable range dictionary or not
				int iTempRange;
				if (InteractRange.HasValue)
				{
					fRangeRequired = (float)InteractRange.Value;
				}
				// Treat the distance as closer if the X & Y distance are almost point-blank, for objects
				if (RadiusDistance <= 2f)
					fDistanceReduction += 1f;

				base.DistanceFromTarget = Vector3.Distance(FunkyGame.Hero.Position, Position) - fDistanceReduction;

			}
			else
			{
				fDistanceReduction = (Radius * 0.33f);
				fRangeRequired = 8f;


				if (FunkyGame.Hero.Position.Distance(Position) <= 1.5f)
					fDistanceReduction += 1f;

				base.DistanceFromTarget = base.RadiusDistance - fDistanceReduction;

			}



			return (fRangeRequired <= 0f || base.DistanceFromTarget <= fRangeRequired);
		}



		public override string DebugString
		{
			get
			{
				return String.Format("{0}\r\nPhysSNO={1} HandleAsObstacle={2} Operated={3} Disabled={4}",
					base.DebugString,
					PhysicsSNO.HasValue ? PhysicsSNO.Value.ToString() : "NULL",
					HandleAsObstacle.HasValue ? HandleAsObstacle.Value.ToString() : "NULL",
					GizmoHasBeenUsed.HasValue ? GizmoHasBeenUsed.Value.ToString() : "NULL",
					GizmoDisabledByScript.HasValue ? GizmoDisabledByScript.Value.ToString() : "NULL");
			}
		}
	}
}