using System;
using System.Linq;
using Zeta;
using System.Windows;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.Internals.Actors.Gizmos;
using Zeta.Internals.SNO;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  internal class CacheGizmo : CacheObject
		  {

				public CacheGizmo(CacheObject baseobj)
					 : base(baseobj)
				{

				}

				public DiaGizmo ref_Gizmo { get; set; }

				///<summary>
				///For Shrines/Healthwells the value is set to (GizmoHasBeenOperated). For Containers the value is set to (ChestOpen).
				///</summary>
				public bool? GizmoHasBeenUsed { get; set; }
				public int? PhysicsSNO { get; set; }

				//public bool? IsEnviromentalActor { get; set; }

				internal bool? HandleAsObstacle { get; set; }


				public AnimationState AnimState
				{
					 //Return live data.
					 get
					 {
						  using (ZetaDia.Memory.AcquireFrame())
						  {
								try
								{
									 return (base.ref_DiaObject.CommonData.AnimationState);
								} catch (NullReferenceException)
								{
									 return AnimationState.Invalid;
								}
						  }
					 }
				}




				public override void UpdateWeight()
				{
					 base.UpdateWeight();

					 if (this.Weight!=1)
					 {
						  Vector3 BotPosition=Bot.Character.Position;
						  switch (this.targetType.Value)
						  {
								case TargetType.Shrine:
									 this.Weight=14500d-(Math.Floor(this.CentreDistance)*170d);
									 // Very close shrines get a weight increase
									 if (this.CentreDistance<=20f)
										  this.Weight+=1000d;

									 // health pool
									 if (base.IsHealthWell)
									 {
										  if (Bot.Character.dCurrentHealthPct>0.75)
												this.Weight=0;
										  else
												//Give weight based upon current health percent.
												this.Weight+=1000/(Bot.Character.dCurrentHealthPct);
									 }

									 if (this.Weight>0)
									 {
										  // Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
										  if (this==Bot.Character.LastCachedTarget&&this.CentreDistance<=25f)
												this.Weight+=400;
										  // Are we prioritizing close-range stuff atm? If so limit it at a value 3k lower than monster close-range priority
										  if ((Bot.Combat.bForceCloseRangeTarget||Bot.Character.bIsRooted))
												this.Weight=18500d-(Math.Floor(this.CentreDistance)*200);
										  // If there's a monster in the path-line to the item, reduce the weight by 25%
										  if (ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(this, BotPosition)))
												this.Weight*=0.75;
									 }
									 break;
								case TargetType.Interactable:
								case TargetType.Door:
									 this.Weight=15000d-(Math.Floor(this.CentreDistance)*170d);
									 if (this.CentreDistance<=12f)
										  this.Weight+=800d;
									 // Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
									 if (this==Bot.Character.LastCachedTarget&&this.CentreDistance<=25f)
										  this.Weight+=400;
									 // If there's a monster in the path-line to the item, reduce the weight by 50%
									 if (ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(this, BotPosition)))
										  this.Weight*=0.5;
									 break;
								case TargetType.Destructible:
								case TargetType.Barricade:
									 this.Weight=12000d-(Math.Floor(this.CentreDistance)*175d);
									 // Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
									 if (this==Bot.Character.LastCachedTarget&&this.CentreDistance<=25f)
										  this.Weight+=400;
									 // Close destructibles get a weight increase
									 if (this.CentreDistance<=16f)
										  this.Weight+=1500d;
									 // If there's a monster in the path-line to the item, reduce the weight by 50%
									 if (ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(this, BotPosition)))
										  this.Weight*=0.5;
									 // Are we prioritizing close-range stuff atm? If so limit it at a value 3k lower than monster close-range priority
									 if ((Bot.Combat.bForceCloseRangeTarget||Bot.Character.bIsRooted))
										  this.Weight=19200d-(Math.Floor(this.CentreDistance)*200d);
									 // Very close destructibles get a final weight increase
									 if (this.CentreDistance<=12f)
										  this.Weight+=2000d;
									 break;
								case TargetType.Container:
									 this.Weight=11000d-(Math.Floor(this.CentreDistance)*190d);
									 if (this.CentreDistance<=12f)
										  this.Weight+=600d;
									 // Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
									 if (this==Bot.Character.LastCachedTarget&&this.CentreDistance<=25f)
									 {
										  this.Weight+=400;
									 }
									 // If there's a monster in the path-line to the item, reduce the weight by 50%
									 if (ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(this, BotPosition)))
									 {
										  this.Weight*=0.5;
									 }
									 // See if there's any AOE avoidance in that spot, if so reduce the weight to 1
									 if (ObjectCache.Obstacles.Values.OfType<CacheAvoidance>().Any(cp => cp.TestIntersection(this, BotPosition)))
										  this.Weight=1;
									 if (SnoCacheLookup.hashSNOContainerResplendant.Contains(this.SNOID))
									 {
										  this.Weight+=1500;
									 }
									 break;
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

						  // Check the primary object blacklist
						  if (hashSNOIgnoreBlacklist.Contains(this.SNOID))
						  {
								this.NeedsRemoved=true;
								this.BlacklistFlag=BlacklistType.Permanent;
								return false;
						  }

						  if (this.RequiresLOSCheck&&!this.IgnoresLOSCheck)
						  {
								//Preform Test every 2500ms on normal objects, 1250ms on special objects.
								double lastLOSCheckMS=this.LastLOSCheckMS;
								if (lastLOSCheckMS<1250)
									 return false;
								else if (lastLOSCheckMS<2500&&!this.ObjectIsSpecial)
									 return false;

								NavCellFlags LOSNavFlags=NavCellFlags.None;
								if (Bot.Class.IsMeleeClass||!this.WithinInteractionRange())
								{
									 LOSNavFlags=NavCellFlags.AllowWalk;
								}

								if (!base.LOSTest(Bot.Character.Position, true, (!Bot.Class.IsMeleeClass), LOSNavFlags))
								{
									 return false;
								}

								this.RequiresLOSCheck=false;
						  }

						  float centreDistance=this.CentreDistance;

						  // Ignore it if it's not in range yet
						  if (centreDistance>Bot.Combat.iCurrentMaxLootRadius)
						  {
								//Containers that are Rep Chests within 75f, or shrines within open container range setting are not ignored here.
								if ((this.targetType==TargetType.Container&&
									(this.IsResplendantChest&&SettingsFunky.UseExtendedRangeRepChest&&centreDistance<75f))||
									this.targetType==TargetType.Shrine&&centreDistance<SettingsFunky.ShrineRange) //&&centreDistance<(settings.iContainerOpenRange*1.25))
								{

								}
								else
									 return false;
						  }

						  if (this.InternalName.ToLower().StartsWith("minimapicon"))
						  {
								// Minimap icons caused a few problems in the past, so this force-blacklists them
								this.BlacklistFlag=BlacklistType.Permanent;
								this.NeedsRemoved=true;
								hashActorSNOIgnoreBlacklist.Add(this.SNOID); //SNO blacklist.
								return false;
						  }

						  if (!this.targetType.HasValue)
								return false;

						  // Now for the specifics
						  double iMinDistance;
						  switch (this.targetType.Value)
						  {
								#region Interactable
								case TargetType.Interactable:
								case TargetType.Door:
									 if (this.GizmoHasBeenUsed.HasValue&&this.GizmoHasBeenUsed.Value==true)
									 {
										  this.NeedsRemoved=true;
										  this.BlacklistFlag=BlacklistType.Permanent;
										  return false;
									 }

									 if (this.targetType.Value==TargetType.Door)
									 {
										  Vector3 InteractionTest;
										  if (PlayerMover.CurrentMovementPosition==vNullLocation)
										  {
												InteractionTest=this.Position;
										  }
										  else
												InteractionTest=PlayerMover.CurrentMovementPosition;

										  if ((Difference(InteractionTest.Z, Bot.Character.Position.Z)<15f//Ignore things blocked by z difference
												&&!GilesIntersectsPath(this.Position, this.Radius, Bot.Character.Position, InteractionTest)))
										  {
												return false;
										  }
									 }

									 if (centreDistance>30f)
									 {
										  this.BlacklistLoops=3;
										  return false;
									 }

									 break;
								#endregion
								#region Shrine
								case TargetType.Shrine:
									 if (this.GizmoHasBeenUsed.HasValue&&this.GizmoHasBeenUsed.Value==true)
									 {
										  this.NeedsRemoved=true;
										  this.BlacklistFlag=BlacklistType.Permanent;
										  return false;
									 }

									 bool IgnoreThis=false;
									 if (this.ref_Gizmo is GizmoHealthwell)
									 {
										  //Health wells..
										  if (Bot.Character.dCurrentHealthPct>0.50)
												IgnoreThis=true;
									 }
									 else
									 {
										  //Ignore XP Shrines at MAX Paragon Level!
										  if (this.SNOID==176075&&Bot.Character.iMyParagonLevel==100)
												IgnoreThis=true;
									 }

									 //Ignoring..?
									 if (SettingsFunky.ShrineRange<=0||IgnoreThis)
									 {
										  this.NeedsRemoved=true;
										  this.BlacklistFlag=BlacklistType.Permanent;
										  return false;
									 }

									 // Bag it!
									 this.Radius=5.1f;
									 break;
								#endregion
								#region Barricade/Destructible
								case TargetType.Barricade:
								case TargetType.Destructible:
									 // No physics mesh? Ignore this destructible altogether
									 if (this.PhysicsSNO.HasValue&&this.PhysicsSNO.Value<=0)
									 {
										  // No physics mesh on a destructible, probably bugged
										  this.NeedsRemoved=true;
										  this.BlacklistFlag=BlacklistType.Permanent;
										  return false;
									 }

									 //We don't cache unless its 40f, so if its out of range we remove it!
									 if (base.CentreDistance>40f)
									 {
										  this.NeedsRemoved=true;
										  return false;
									 }

									 //Ignore Non Important Destructibles -- Setting!
									 if (!Zeta.CommonBot.Settings.CharacterSettings.Instance.DestroyEnvironment&&
										  this.targetType.Value==TargetType.Destructible)
									 {//Check for NoLoot/NoXP flags..
										  if ((this.DropsNoLoot.HasValue&&this.DropsNoLoot.Value||this.GrantsNoXP.HasValue&&this.GrantsNoXP.Value)&&
												this.LastPriortized>1500)
										  {
												//ignore from being a targeted right now..
												this.BlacklistLoops=25;
												//We dont want to complete ignore this since the object may pose a navigational obstacle.
												return false;
										  }
									 }

									 Vector3 BarricadeTest;
									 if (PlayerMover.CurrentMovementPosition==vNullLocation)
									 {
										  BarricadeTest=this.Position;
									 }
									 else
										  BarricadeTest=PlayerMover.CurrentMovementPosition;

									 //Barricade and path intersects the actorsphere radius..
									 //Some barricades may be lower than ourself, or our destination is high enough to raycast past the object. So we add a little to the Z of the obstacle.
									 //The best method would be to get the hight of the object and compare it to our current Z-height if we are nearly within radius distance of the object.
									 if ((this.targetType.Value==TargetType.Barricade||this.IsBarricade.HasValue&&this.IsBarricade.Value)&&
										  (!PlayerMover.ShouldHandleObstacleObject  //Have special flag from unstucker to destroy nearby barricade.
										  &&Difference(BarricadeTest.Z, Bot.Character.Position.Z)<15f//Ignore things blocked by z difference
										  &&!MathEx.IntersectsPath(this.Position, this.Radius, Bot.Character.Position, BarricadeTest)))
									 {
										  return false;
									 }


									 if (this.targetType.Value==TargetType.Destructible&&
										 this.CentreDistance>(SettingsFunky.DestructibleRange+(this.Radius)))
									 {
										  return false;
									 }



									 break;
								#endregion
								#region Container
								case TargetType.Container:
									 if (this.GizmoHasBeenUsed.HasValue&&this.GizmoHasBeenUsed.Value==true)
									 {
										  this.NeedsRemoved=true;
										  this.BlacklistFlag=BlacklistType.Permanent;
										  return false;
									 }

									 //Vendor Run and DB Settings check
									 if (TownRunManager.bWantToTownRun
										  ||!this.IsChestContainer&&!Zeta.CommonBot.Settings.CharacterSettings.Instance.OpenLootContainers
										  ||this.IsChestContainer&&!Zeta.CommonBot.Settings.CharacterSettings.Instance.OpenChests)
									 {
										  this.BlacklistLoops=25;
										  return false;
									 }

									 if (!this.IsChestContainer&&SettingsFunky.IgnoreCorpses)
									 {
										  this.BlacklistLoops=-1;
										  return false;
									 }

									 iMinDistance=0f;
									 // Any physics mesh? Give a minimum distance of 5 feet
									 if (this.PhysicsSNO.HasValue&&this.PhysicsSNO>0)
										  iMinDistance=SettingsFunky.ContainerOpenRange;

									 // Superlist for rare chests etc.

									 if (this.IsResplendantChest&&SettingsFunky.UseExtendedRangeRepChest)
									 {
										  iMinDistance=75;
										  //setup wait time. (Unlike Units, we blacklist right after we interact)
										  if (Bot.Character.LastCachedTarget==this)
										  {
												Bot.Combat.lastHadContainerAsTarget=DateTime.Now;
												Bot.Combat.lastHadRareChestAsTarget=DateTime.Now;
										  }
									 }

									 if (iMinDistance<=0||centreDistance>iMinDistance)
									 {
										  this.BlacklistLoops=5;
										  return false;
									 }

									 // Bag it!
									 if (this.IsChestContainer)
										  this.Radius=5.1f;
									 else
										  this.Radius=4f;

									 break;
								#endregion
						  } // Object switch on type (to seperate shrines, destructibles, barricades etc.)

						  return true;
					 }
				}

				public override bool UpdateData()
				{

					 if (this.ref_Gizmo==null)
					 {
						  try
						  {
								this.ref_Gizmo=(DiaGizmo)base.ref_DiaObject;
						  } catch (NullReferenceException) { Logging.WriteVerbose("Failure to convert obj to DiaItem!"); return false; }
					 }

					 //Destructibles are not important unless they are close.. 40f is minimum range!
					 if ((this.targetType.Value==TargetType.Destructible||this.targetType.Value==TargetType.Barricade)
						  &&this.CentreDistance>40f)
						  return false;

					 if ((TargetType.Interactables.HasFlag(base.targetType.Value))
						  &&(!this.GizmoHasBeenUsed.HasValue||!this.GizmoHasBeenUsed.Value))
					 {
						  try
						  {
								if (base.Gizmotype.Value==Zeta.Internals.SNO.GizmoType.Shrine)
								{
									 this.HandleAsObstacle=true;
									 GizmoShrine gizmoShrine=this.ref_Gizmo as GizmoShrine;
									 this.GizmoHasBeenUsed=gizmoShrine.HasBeenOperated;
								}
								else if (base.Gizmotype.Value==Zeta.Internals.SNO.GizmoType.Healthwell)
								{
									 this.HandleAsObstacle=true;
									 GizmoHealthwell gizmoHealthWell=this.ref_Gizmo as GizmoHealthwell;
									 this.GizmoHasBeenUsed=gizmoHealthWell.HasBeenOperated;
								}
								else if (base.Gizmotype.Value==Zeta.Internals.SNO.GizmoType.Door)
								{
									 GizmoDoor gizmoDoor=this.ref_Gizmo as GizmoDoor;
									 this.GizmoHasBeenUsed=gizmoDoor.HasBeenOperated;
									 this.HandleAsObstacle=true;
								}
								else if (base.Gizmotype.Value==Zeta.Internals.SNO.GizmoType.LootContainer)
								{
									 if (this.IsChestContainer)
										  this.HandleAsObstacle=true;

									 GizmoLootContainer gizmoContainer=this.ref_Gizmo as GizmoLootContainer;
									 this.GizmoHasBeenUsed=gizmoContainer.HasBeenOperated;
								}
						  } catch (AccessViolationException)
						  {
								Logging.WriteVerbose("Safely handled getting attribute GizmoHasBeenOperated gizmo {0}", this.InternalName);
								return false;
						  }

						  //Blacklist used gizmos.
						  if (this.GizmoHasBeenUsed.HasValue&&this.GizmoHasBeenUsed.Value)
						  {
								this.BlacklistFlag=BlacklistType.Permanent;
								this.NeedsRemoved=true;
								return false;
						  }
					 }

					 //only shrines and "chests" would have set this value true.. so if no value than we set it false!
					 if (!this.HandleAsObstacle.HasValue)
						  this.HandleAsObstacle=false;
					 else if (this.HandleAsObstacle.Value)
						  base.Obstacletype=ObstacleType.ServerObject;

					 //PhysicsSNO -- (continiously updated) excluding shrines/interactables
					 if (base.targetType.Value==TargetType.Destructible||base.targetType.Value==TargetType.Barricade||base.targetType.Value==TargetType.Container)
					 {
						  try
						  {
								this.PhysicsSNO=base.ref_DiaObject.PhysicsSNO;
						  } catch (NullReferenceException ex)
						  {
								Logging.WriteVerbose("[Funky] Safely handled exception getting physics SNO for object "+this.InternalName+" ["+this.SNOID.ToString()+"]\r\n"+ex.Message);
								return false;
						  }
					 }

					 if (this.targetType.Value==TargetType.Destructible||this.targetType.Value==TargetType.Barricade||this.targetType.Value==TargetType.Interactable)
					 {
						  if (this.IsBarricade.HasValue&&this.IsBarricade.Value&&!this.targetType.Value.HasFlag(TargetType.Barricade))
						  {
								Logging.WriteVerbose("Changing Gizmo {0} target type from {1} to Barricade!", this.InternalName, this.targetType.Value.ToString());
								//Change "barricade" attribute gizmos into barricade targeting!
								this.targetType=TargetType.Barricade;
						  }
					 }

					 if (this.Radius==0f)
					 {
						  if (base.ActorSphereRadius.HasValue)
						  {
								this.Radius=base.ActorSphereRadius.Value;

								this.Radius*=0.80f;

								if (this.Radius<0f)
									 this.Radius=0f;
						  }
					 }

					 return true;
				}

				public override bool IsStillValid()
				{
					 if (ref_Gizmo==null||!ref_Gizmo.IsValid||ref_Gizmo.BaseAddress==IntPtr.Zero)
						  return false;

					 return base.IsStillValid();
				}
		  }

		  internal class CacheDestructable : CacheGizmo
		  {

				public CacheDestructable(CacheObject baseobj)
					 : base(baseobj)
				{
				}

				public override bool IsZDifferenceValid
				{
					 get
					 {
						  float fThisHeightDifference=Difference(Bot.Character.Position.Z, this.Position.Z);
						  if (fThisHeightDifference>=15f)
						  {
								if (!PlayerMover.ShouldHandleObstacleObject)
									 return false;
								else if (fThisHeightDifference>=20f)//Was stuck.. so give extra range..
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
					 if (Bot.Combat.powerPrime.Power!=SNOPower.None)
					 {
						  if (this.targetType.Value==TargetType.Barricade)
								Logging.WriteDiagnostic("[Funky] Barricade: Name="+this.InternalName+". SNO="+this.SNOID.ToString()+
									", Range="+this.CentreDistance.ToString()+". Needed range="+Bot.Combat.powerPrime.MinimumRange.ToString()+". Radius="+
									this.Radius.ToString()+". SphereRadius="+this.ActorSphereRadius.Value.ToString()+". Type="+this.targetType.ToString()+". Using power="+Bot.Combat.powerPrime.Power.ToString());
						  else
								Logging.WriteDiagnostic("[Funky] Destructible: Name="+this.InternalName+". SNO="+this.SNOID.ToString()+
									", Range="+this.CentreDistance.ToString()+". Needed range="+Bot.Combat.powerPrime.MinimumRange.ToString()+". Radius="+
									this.Radius.ToString()+". SphereRadius="+this.ActorSphereRadius.Value.ToString()+". Type="+this.targetType.ToString()+". Using power="+Bot.Combat.powerPrime.Power.ToString());

						  //Actual interaction
						  WaitWhileAnimating(12, true);

						  if (hashDestructableLocationTarget.Contains(this.SNOID)
								||(Bot.Character.LastCachedTarget==this
								&&this.RadiusDistance<6f))//Bot.Class.IsMeleeClass
						  {//Melee class and no change in target.. we attempt to attack by shift-clicking..
								// Location attack - attack the Vector3/map-area (equivalent of holding shift and left-clicking the object in-game to "force-attack")
								Vector3 vAttackPoint;
								if (this.CentreDistance>=6f)
									 vAttackPoint=MathEx.CalculatePointFrom(this.Position, Bot.Character.Position, 6f);
								else
									 vAttackPoint=this.Position;
								vAttackPoint.Z+=1.5f;
								Logging.WriteDiagnostic("[Funky] (NB: Attacking location of destructable)");
								ZetaDia.Me.UsePower(Bot.Combat.powerPrime.Power, vAttackPoint, Bot.Character.iCurrentWorldID, -1);
						  }
						  else
						  {
								// Standard attack - attack the ACDGUID (equivalent of left-clicking the object in-game)
								ZetaDia.Me.UsePower(Bot.Combat.powerPrime.Power, vNullLocation, Bot.Character.iCurrentWorldID, base.AcdGuid.Value);
						  }
						  this.InteractionAttempts++;

						  // If we've tried interacting too many times, blacklist this for a while
						  if (this.InteractionAttempts>3)
						  {
								this.BlacklistLoops=50;
								this.InteractionAttempts=0;
						  }

						  dictAbilityLastUse[Bot.Combat.powerPrime.Power]=DateTime.Now;
						  Bot.Combat.powerPrime.Power=SNOPower.None;
						  WaitWhileAnimating(6, true);
					 }

					 //Get current animation state! (Idle = Untouched, Dead = Destroyed)
					 AnimationState currentAnimState=this.AnimState;
					 if (currentAnimState!=AnimationState.Idle)
					 {
						  // Now tell Trinity to get a new target!
						  Bot.Combat.bForceTargetUpdate=true;
						  Bot.Combat.lastChangedZigZag=DateTime.Today;
						  Bot.Combat.vPositionLastZigZagCheck=Vector3.Zero;
						  PlayerMover.ShouldHandleObstacleObject=false;

						  //Blacklist all destructibles surrounding this obj
						  ObjectCache.Objects.BlacklistObjectsSurroundingObject<CacheDestructable>(this, 20f, 15);
					 }

					 return RunStatus.Running;
				}

				public override bool WithinInteractionRange()
				{
					 float fRangeRequired=0f;
					 float fDistanceReduction=0f;

					 fRangeRequired=Bot.Combat.powerPrime.Power==SNOPower.None?6f:Bot.Combat.powerPrime.MinimumRange;

					 if (dictSNOExtendedDestructRange.ContainsKey(this.SNOID))
					 {
						  fRangeRequired=this.CollisionRadius.Value;

						  //Increase Range for Ranged Classes
						  if (!Bot.Class.IsMeleeClass)
								fRangeRequired*=1.5f;
					 }

					 fDistanceReduction=(this.Radius*0.33f);

					 if (Bot.Combat.bForceCloseRangeTarget)
						  fDistanceReduction-=3f;

					 if (fDistanceReduction<=0f)
						  fDistanceReduction=0f;

					 if (GridPoint.GetDistanceBetweenPoints(Bot.Character.PointPosition, this.Position)<=1.5f)
						  fDistanceReduction+=1f;

					 base.DistanceFromTarget=Vector3.Distance(Bot.Character.Position, this.Position)-fDistanceReduction;

					 return (fRangeRequired<=0f||base.DistanceFromTarget<=fRangeRequired);
				}

		  }

		  internal class CacheInteractable : CacheGizmo
		  {
				public CacheInteractable(CacheObject baseobj)
					 : base(baseobj)
				{
				}

				public override string DebugString
				{
					 get
					 {
						  return String.Format("{0}\r\n PhysSNO={1} HandleAsObstacle={2} Operated={3} ",
								base.DebugString, this.PhysicsSNO.HasValue?this.PhysicsSNO.Value.ToString():"NULL",
								this.HandleAsObstacle.HasValue?this.HandleAsObstacle.Value.ToString():"NULL",
								this.GizmoHasBeenUsed.HasValue?this.GizmoHasBeenUsed.Value.ToString():"NULL");
					 }
				}


				public override bool IsZDifferenceValid
				{
					 get
					 {
						  float fThisHeightDifference=Difference(Bot.Character.Position.Z, this.Position.Z);
						  if (fThisHeightDifference>=10f)
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
					 WaitWhileAnimating(10, true);
					 ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, base.Position, Bot.Character.iCurrentWorldID, base.AcdGuid.Value);
					 this.InteractionAttempts++;

					 if (this.InteractionAttempts==1)
					 {
						  // Force waiting AFTER power use for certain abilities
						  Bot.Combat.bWaitingAfterPower=true;
						  Bot.Combat.powerPrime.WaitLoopsAfter=5;
					 }

					 // Interactables can have a long channeling time...
					 if (this.targetType.Value.HasFlag(TargetType.Interactable))
					 {
						  WaitWhileAnimating(1500, true);
						  this.BlacklistLoops=20;
					 }

					 WaitWhileAnimating(175, true);

					 // If we've tried interacting too many times, blacklist this for a while
					 if (this.InteractionAttempts>5)
					 {
						  this.BlacklistLoops=100;
					 }

					 if (!Bot.Combat.bWaitingAfterPower)
					 {
						  // Now tell Trinity to get a new target!
						  Bot.Combat.lastChangedZigZag=DateTime.Today;
						  Bot.Combat.vPositionLastZigZagCheck=Vector3.Zero;
						  Bot.Combat.bForceTargetUpdate=true;
					 }
					 return RunStatus.Running;
				}

				public override bool WithinInteractionRange()
				{
					 float fRangeRequired=0f;
					 float fDistanceReduction=0f;

					 if (targetType.Value==TargetType.Interactable)
					 {
						  // Treat the distance as closer based on the radius of the object
						  //fDistanceReduction=ObjectData.Radius;
						  fRangeRequired=8f;

						  if (Bot.Combat.bForceCloseRangeTarget)
								fRangeRequired-=2f;
						  // Check if it's in our interactable range dictionary or not
						  int iTempRange;
						  if (dictInteractableRange.TryGetValue(this.SNOID, out iTempRange))
						  {
								fRangeRequired=(float)iTempRange;
						  }
						  // Treat the distance as closer if the X & Y distance are almost point-blank, for objects
						  if (Bot.Character.Position.Distance(this.Position)<=1.5f)
								fDistanceReduction+=1f;

						  base.DistanceFromTarget=Vector3.Distance(Bot.Character.Position, this.Position)-fDistanceReduction;

					 }
					 else
					 {
						  fDistanceReduction=(this.Radius*0.33f);
						  fRangeRequired=8f;

						  if (Bot.Combat.bForceCloseRangeTarget)
								fRangeRequired-=2f;

						  if (Bot.Character.Position.Distance(this.Position)<=1.5f)
								fDistanceReduction+=1f;

						  base.DistanceFromTarget=base.RadiusDistance-fDistanceReduction;

					 }



					 return (fRangeRequired<=0f||base.DistanceFromTarget<=fRangeRequired);
				}
		  }
	 }
}