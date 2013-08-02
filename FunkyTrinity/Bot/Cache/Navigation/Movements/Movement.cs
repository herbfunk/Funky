using System;
using System.Linq;
using Zeta;
using Zeta.TreeSharp;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.Internals.SNO;

namespace FunkyTrinity
{
	 public partial class Funky
	 {

			public abstract class Movement
			{
				 //Each individual movement class will be created and reused at runtime.

				 internal Vector3 LastPlayerLocation=Vector3.Zero;
				 internal int BlockedMovementCounter=0;
				 internal int NonMovementCounter=0;

				 internal DateTime LastMovementDuringCombat=DateTime.Today;
				 internal DateTime LastMovementAttempted=DateTime.Today;
				 internal DateTime LastMovementCommand=DateTime.Today;

				 internal bool IsAlreadyMoving=false;
				 internal float LastDistanceFromTarget=-1f;


				 internal Vector3 LastTargetLocation=Vector3.Zero;
				 internal Vector3 CurrentTargetLocation=Vector3.Zero;

				 internal void NewTargetResetVars()
				 {
						LastDistanceFromTarget=-1f;
						IsAlreadyMoving=false;
				 }
				 internal void ResetTargetMovementVars()
				 {
						BlockedMovementCounter=0;
						NonMovementCounter=0;
						NewTargetResetVars();
				 }

				 //Evaluation
				 public virtual Func<bool> Condition { get { return new Func<bool>(() => { return !CurrentMovementObject.WithinInteractionRange(); }); } }
				 //Pre-Behavior Execution Check
				 public virtual Func<bool> Break { get { return new Func<bool>(() => { return DateTime.Compare(DateTime.Now, this.ResumeDate)<0; }); } }

				 //The Movement Behavior
				 public virtual void Behavior()
				 {
						// Are we currently incapacitated? If so then wait...
						if (Bot.Character.bIsIncapacitated||Bot.Character.bIsRooted) return;

						// Some stuff to avoid spamming usepower EVERY loop, and also to detect stucks/staying in one place for too long
						bool bForceNewMovement=false;

						if (NonMovementCounter>50)
						{
							 if (CurrentMovementObject.targetType.Value.Equals(TargetType.Avoidance))
							 {
									Logging.WriteDiagnostic("NonMovementCounter exceeded 50 for movement target");
							 }
							 else
							 {
									Logging.WriteDiagnostic("Blacklisting Mob due to NonMovementCoutner exceeding 50.");
									CurrentMovementObject.BlacklistLoops=50;
							 }

							 Bot.Combat.bForceTargetUpdate=true;
							 NonMovementCounter=0;
							 return;
						}

						Bot.NavigationCache.ObstaclePrioritizeCheck();

						float DistanceMoved=CurrentMovementObject.DistanceFromTarget;

						#region DistanceChecks
						// Count how long we have failed to move - body block stuff etc.
						if (DistanceMoved==LastDistanceFromTarget)
						{
							 bForceNewMovement=true;
							 if (DateTime.Now.Subtract(LastMovementDuringCombat).TotalMilliseconds>=250)
							 {
									LastMovementDuringCombat=DateTime.Now;
									// We've been stuck at least 250 ms, let's go and pick new targets etc.
									BlockedMovementCounter++;
									Bot.Combat.bForceCloseRangeTarget=true;
									Bot.Combat.lastForcedKeepCloseRange=DateTime.Now;



									// Tell target finder to prioritize close-combat targets incase we were bodyblocked
									#region TargetingPriortize
									switch (BlockedMovementCounter)
									{
										 case 2:
										 case 3:
												//Than check our movement state
												//If we are moving to a LOS location.. nullify it!
												if (CurrentMovementObject.LOSV3!=Vector3.Zero)
												{
													 Logging.WriteVerbose("Blockcounter Reseting LOS Movement Vector");
													 CurrentMovementObject.LOSV3=Vector3.Zero;
												}

												var intersectingObstacles=Bot.Combat.NearbyObstacleObjects //ObjectCache.Obstacles.Values.OfType<CacheServerObject>()
																						.Where(obstacle =>
																							 !Bot.Combat.PrioritizedRAGUIDs.Contains(obstacle.RAGUID)//Only objects not already prioritized
																							 &&obstacle.Obstacletype.HasValue
																							 &&ObstacleType.Navigation.HasFlag(obstacle.Obstacletype.Value)//only navigation/intersection blocking objects!
																							 &&obstacle.RadiusDistance<=10f);

												if (intersectingObstacles.Any())
												{
													 var intersectingObjectRAGUIDs=(from objs in intersectingObstacles
																													select objs.RAGUID);

													 Bot.Combat.PrioritizedRAGUIDs.AddRange(intersectingObjectRAGUIDs);
												}


												if (CurrentMovementObject.targetType.Value==TargetType.Avoidance)
												{//Avoidance Movement..
													 Bot.Combat.timeCancelledKiteMove=DateTime.Now;
													 Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
													 Bot.NavigationCache.BlacklistLastSafespot();
													 Bot.UpdateAvoidKiteRates();
													 Bot.Combat.bForceTargetUpdate=true;
													 return;
												}



												break;
										 default:
												if (CurrentMovementObject.targetType.Value!=TargetType.Avoidance)
												{
													 //Finally try raycasting to see if navigation is possible..
													 if (CurrentMovementObject.Actortype.HasValue&&
															(CurrentMovementObject.Actortype.Value==ActorType.Gizmo||CurrentMovementObject.Actortype.Value==ActorType.Unit))
													 {
															Vector3 hitTest;
															// No raycast available, try and force-ignore this for a little while, and blacklist for a few seconds
															if (Zeta.Navigation.Navigator.Raycast(Bot.Character.Position, CurrentMovementObject.Position, out hitTest))
															{
																 if (hitTest!=Vector3.Zero)
																 {
																		CurrentMovementObject.RequiresLOSCheck=true;
																		CurrentMovementObject.BlacklistLoops=10;
																		Log("Ignoring object "+CurrentMovementObject.InternalName+" due to not moving and raycast failure!", true);
																		Bot.Combat.bForceTargetUpdate=true;
																		return;
																 }
															}
													 }
													 else if (CurrentMovementObject.Actortype.HasValue&&CurrentMovementObject.Actortype.Value.HasFlag(ActorType.Item))
													 {
															Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
															Bot.Combat.timeCancelledKiteMove=DateTime.Now;

															//Check if we can walk to this location from current location..
															if (!Navigation.CanRayCast(Bot.Character.Position, CurrentMovementObject.Position, NavCellFlags.AllowWalk))
															{
																 CurrentMovementObject.BlacklistLoops=10;
																 Log("Ignoring Item due to AllowWalk RayCast Failure!", true);
																 Bot.Combat.bForceTargetUpdate=true;
																 return;
															}
													 }
												}
												else
												{
													 if (!Navigation.CanRayCast(Bot.Character.Position, CurrentMovementObject.Position, NavCellFlags.AllowWalk))
													 {
															Logging.WriteVerbose("Cannot continue with avoidance movement due to raycast failure!");
															BlockedMovementCounter=0;

															Bot.Combat.iMillisecondsCancelledEmergencyMoveFor/=2;
															Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
															//Ignore avoidance movements.
															Bot.Combat.iMillisecondsCancelledKiteMoveFor/=2;
															Bot.Combat.timeCancelledKiteMove=DateTime.Now;

															Bot.NavigationCache.BlacklistLastSafespot();
															Bot.Combat.bForceTargetUpdate=true;
															return;
													 }
												}
												break;
									}
									#endregion


									if (CurrentMovementObject.targetType.Value==TargetType.Item)
									{
										 CurrentMovementObject.BlacklistLoops=1;
										 Bot.Combat.bForceTargetUpdate=true;
									}

									return;
							 } // Been 250 milliseconds of non-movement?
						}
						else
						{
							 // Movement has been made, so count the time last moved!
							 LastMovementDuringCombat=DateTime.Now;
						}
						#endregion

						// Update the last distance stored
						LastDistanceFromTarget=DistanceMoved;

						// See if we want to ACTUALLY move, or are just waiting for the last move command...
						if (!bForceNewMovement&&IsAlreadyMoving&&CurrentMovementObject.Position==LastTargetLocation&&DateTime.Now.Subtract(LastMovementCommand).TotalMilliseconds<=100)
						{
							 return;
						}

						float currentDistance=Vector3.Distance(LastTargetLocation, CurrentMovementObject.Position);


						// If we're doing avoidance, globes or backtracking, try to use special abilities to move quicker
						#region SpecialMovementChecks
						if ((TargetType.Avoidance|TargetType.Gold|TargetType.Globe).HasFlag(CurrentMovementObject.targetType.Value))
						{

							 bool bTooMuchZChange=((Bot.Character.Position.Z-CurrentMovementObject.Position.Z)>=4f);

							 Ability MovementPower;
							 if (Bot.Class.FindSpecialMovementPower(out MovementPower))
							 {
									double lastUsedAbilityMS=MovementPower.LastUsedMilliseconds;
									bool foundMovementPower=false;
									float pointDistance=0f;
									Vector3 vTargetAimPoint=CurrentMovementObject.Position;
									bool ignoreLOSTest=false;

									switch (MovementPower.Power)
									{
										 case SNOPower.Monk_TempestRush:
												vTargetAimPoint=MathEx.CalculatePointFrom(CurrentMovementObject.Position, Bot.Character.Position, 10f);
												Bot.Character.UpdateAnimationState(false, true);
												bool isHobbling=Bot.Character.CurrentSNOAnim.HasFlag(SNOAnim.Monk_Female_Hobble_Run|SNOAnim.Monk_Male_HTH_Hobble_Run);
												foundMovementPower=(!bTooMuchZChange&&currentDistance<15f&&((isHobbling||lastUsedAbilityMS<300)&&Bot.Character.dCurrentEnergy>15)
													&&!ObjectCache.Obstacles.DoesPositionIntersectAny(vTargetAimPoint, ObstacleType.ServerObject));

												break;
										 case SNOPower.DemonHunter_Vault:
												foundMovementPower=(CurrentMovementObject.targetType.Value!=TargetType.Destructible&&!bTooMuchZChange&&currentDistance>=18f&&
																		 (lastUsedAbilityMS>=Funky.Bot.SettingsFunky.Class.iDHVaultMovementDelay));
												pointDistance=35f;
												if (currentDistance>pointDistance)
													 vTargetAimPoint=MathEx.CalculatePointFrom(CurrentMovementObject.Position, Bot.Character.Position, pointDistance);
												break;
										 case SNOPower.Barbarian_FuriousCharge:
												foundMovementPower=(CurrentMovementObject.targetType.Value!=TargetType.Destructible&&!bTooMuchZChange
																		 &&(currentDistance>20f||CurrentMovementObject.targetType.Value.HasFlag(TargetType.Avoidance)&&(NonMovementCounter>0||BlockedMovementCounter>0)));

												pointDistance=35f;
												if (currentDistance>pointDistance)
													 vTargetAimPoint=MathEx.CalculatePointFrom(CurrentMovementObject.Position, Bot.Character.Position, pointDistance);

												break;
										 case SNOPower.Barbarian_Leap:
										 case SNOPower.Wizard_Archon_Teleport:
										 case SNOPower.Wizard_Teleport:
												foundMovementPower=(CurrentMovementObject.targetType.Value!=TargetType.Destructible&&!bTooMuchZChange
																		 &&(currentDistance>20f||CurrentMovementObject.targetType.Value.HasFlag(TargetType.Avoidance)&&(NonMovementCounter>0||BlockedMovementCounter>0)));

												pointDistance=35f;
												if (currentDistance>pointDistance)
													 vTargetAimPoint=MathEx.CalculatePointFrom(CurrentMovementObject.Position, Bot.Character.Position, pointDistance);

												//Teleport and Leap ignores raycast testing below!
												ignoreLOSTest=true;
												break;
										 case SNOPower.Barbarian_Whirlwind:
												break;
										 default:
												Bot.Character.WaitWhileAnimating(3, true);

												ZetaDia.Me.UsePower(MovementPower.Power, CurrentMovementObject.Position, Bot.Character.iCurrentWorldID, -1);
												MovementPower.LastUsed=DateTime.Now;

												Bot.Character.WaitWhileAnimating(6, true);
												// Store the current destination for comparison incase of changes next loop
												LastTargetLocation=CurrentMovementObject.Position;
												// Reset total body-block count, since we should have moved
												if (DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>=2000)
													 BlockedMovementCounter=0;
												return;
									}

									if (foundMovementPower)
									{

										 if ((MovementPower.Power==SNOPower.Monk_TempestRush&&lastUsedAbilityMS>500)||
												(ignoreLOSTest||ZetaDia.Physics.Raycast(Bot.Character.Position, vTargetAimPoint, NavCellFlags.AllowWalk)))
										 {
												ZetaDia.Me.UsePower(MovementPower.Power, vTargetAimPoint, Funky.Bot.Character.iCurrentWorldID, -1);
												MovementPower.LastUsed=DateTime.Now;

												// Store the current destination for comparison incase of changes next loop
												LastTargetLocation=CurrentMovementObject.Position;

												// Reset total body-block count, since we should have moved
												if (DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>=2000)
													 BlockedMovementCounter=0;
												return;
										 }
									}
							 }

							 //Special Whirlwind Code
							 if (Bot.Class.AC==Zeta.Internals.Actors.ActorClass.Barbarian&&Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_Whirlwind))
							 {
									// Whirlwind against everything within range (except backtrack points)
									if (Bot.Character.dCurrentEnergy>=10
										 &&Bot.Combat.iAnythingWithinRange[RANGE_20]>=1
										 &&CurrentMovementObject.DistanceFromTarget<=12f
										 &&(!Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_Sprint)||Bot.Class.HasBuff(SNOPower.Barbarian_Sprint))
										 &&(!(TargetType.Avoidance|TargetType.Gold|TargetType.Globe).HasFlag(CurrentMovementObject.targetType.Value)&CurrentMovementObject.DistanceFromTarget>=6f)
										 &&(CurrentMovementObject.targetType.Value!=TargetType.Unit
										 ||(CurrentMovementObject.targetType.Value==TargetType.Unit&&!CurrentMovementObject.IsTreasureGoblin
												&&(!Bot.SettingsFunky.Class.bSelectiveWhirlwind
													||Bot.Combat.bAnyNonWWIgnoreMobsInRange
													||!SnoCacheLookup.hashActorSNOWhirlwindIgnore.Contains(CurrentMovementObject.SNOID)))))
									{
										 // Special code to prevent whirlwind double-spam, this helps save fury
										 bool bUseThisLoop=SNOPower.Barbarian_Whirlwind!=Bot.Combat.powerLastSnoPowerUsed;
										 if (!bUseThisLoop)
										 {
												Bot.Combat.powerLastSnoPowerUsed=SNOPower.None;
												if (DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.Barbarian_Whirlwind]).TotalMilliseconds>=200)
													 bUseThisLoop=true;
										 }
										 if (bUseThisLoop)
										 {
												ZetaDia.Me.UsePower(SNOPower.Barbarian_Whirlwind, CurrentMovementObject.Position, Bot.Character.iCurrentWorldID, -1);
												Bot.Combat.powerLastSnoPowerUsed=SNOPower.Barbarian_Whirlwind;
												dictAbilityLastUse[SNOPower.Barbarian_Whirlwind]=DateTime.Now;
										 }
										 // Store the current destination for comparison incase of changes next loop
										 LastTargetLocation=CurrentMovementObject.Position;
										 // Reset total body-block count
										 if ((!Bot.Combat.bForceCloseRangeTarget||DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>Bot.Combat.iMillisecondsForceCloseRange)&&
											 DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>=2000)
												BlockedMovementCounter=0;
										 return;
									}
							 }
						}
						#endregion


						// Now for the actual movement request stuff
						IsAlreadyMoving=true;
						LastMovementCommand=DateTime.Now;


						if (DateTime.Now.Subtract(LastMovementAttempted).TotalMilliseconds>=250||currentDistance>=2f||bForceNewMovement)
						{
							 if (CurrentMovementObject.targetType.Value.Equals(TargetType.Avoidance))
							 {
									if (NonMovementCounter<10)
										 ZetaDia.Me.Movement.MoveActor(CurrentMovementObject.Position);
									else
										 ZetaDia.Me.UsePower(SNOPower.Walk, CurrentMovementObject.Position, Bot.Character.iCurrentWorldID, -1);
							 }
							 else
							 {
									//Use Walk Power when not using LOS Movement, target is not an item and target does not ignore LOS.
									bool UsePower=(NonMovementCounter<10&&CurrentMovementObject.targetType.Value!=TargetType.Item&&!CurrentMovementObject.IgnoresLOSCheck);
									if (UsePower)
									{
										 ZetaDia.Me.UsePower(SNOPower.Walk, CurrentMovementObject.Position, Bot.Character.iCurrentWorldID, -1);
									}
									else
										 ZetaDia.Me.Movement.MoveActor(CurrentMovementObject.Position);
							 }

							 if (Bot.SettingsFunky.SkipAhead)
									SkipAheadCache.RecordSkipAheadCachePoint();

							 LastMovementAttempted=DateTime.Now;
							 // Store the current destination for comparison incase of changes next loop
							 LastTargetLocation=CurrentMovementObject.Position;
							 // Reset total body-block count, since we should have moved
							 if (DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>=2000)
									BlockedMovementCounter=0;

							 //Herbfunk: Quick fix for stuck occuring on above average mob who is damaged..
							 if (LastPlayerLocation==Bot.Character.Position)
									NonMovementCounter++;
							 else
									NonMovementCounter=0;

							 LastPlayerLocation=Bot.Character.Position;
						}
				 }

				 private DateTime lastgeneratedamovement=DateTime.Today;
				 public DateTime LastGeneratedaMovement
				 {
						get { return this.lastgeneratedamovement; }
				 }

				 private Vector3 currentdestination=Vector3.Zero;
				 public virtual Vector3 CurrentDestination
				 {
						get { return currentdestination; }
						set
						{
							 currentdestination=value;
							 this.lastgeneratedamovement=DateTime.Now;

						}
				 }

				 private DateTime resumeDate=DateTime.Now;
				 public DateTime ResumeDate
				 {
						get { return resumeDate; }
						set { resumeDate=value; }
				 }

				 public virtual CacheObject CurrentMovementObject
				 {
						get { return Bot.Target.CurrentTarget; }
				 }

				 public Movement()
				 {
				 }
			}



			public static TargetMovement targetMovement=new TargetMovement();
			public class TargetMovement : Movement
			{
				 public TargetMovement() : base() { }
			}



			public static Avoidance avoidanceMovement=new Avoidance();
			public class Avoidance : Movement
			{
				 public Avoidance()
						: base()
				 {
						condition=new Func<bool>(() =>
						{
							 return (Bot.Combat.RequiresAvoidance&&(!Bot.Combat.bAnyTreasureGoblinsPresent||Bot.SettingsFunky.GoblinPriority<2));
						});

						Break_=new Func<bool>(() =>
						{
							 return this.CurrentDestination.Equals(Vector3.Zero);
						});
				 }

				 private Func<bool> condition;
				 public override Func<bool> Condition
				 {
						get
						{
							 return condition;
						}
				 }

				 private Func<bool> Break_;
				 public override Func<bool> Break
				 {
						get
						{
							 return Break_;
						}
				 }

				 private DateTime LastGeneratedAvoidanceMovement=DateTime.Today;
				 private double ReuseVectorDurationMS=750;
				 private Vector3 currentdestination=Vector3.Zero;
				 public override Vector3 CurrentDestination
				 {
						get
						{
							 //Reuse the last generated safe spot...
							 if (this.currentdestination!=Vector3.Zero
								 &&DateTime.Now.Subtract(base.LastGeneratedaMovement).TotalMilliseconds>=this.ReuseVectorDurationMS)
							 {
									//Check how close we are..
									if (Bot.Character.Position.Distance2D(this.currentdestination)<2.5f)
									{
										 Log("Avoid Movement distance is less than 2.5f", true);
										 currentdestination=Vector3.Zero;
										 return currentdestination;
									}
									//Check if position is still safe..
									else if (ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(this.currentdestination))
									{
										 Log("Avoid Movement is no longer valid", true);
									}
									else
									{
										 //Reuse
										 return currentdestination;
									}
							 }

							 Bot.NavigationCache.RefreshGPArea();
							 //Generate new Destination
							 currentdestination=Bot.NavigationCache.CurrentGPArea.AttemptFindSafeSpot(Bot.Character.Position, Vector3.Zero);

							 return currentdestination;
						}
						set
						{
							 base.CurrentDestination=value;
						}
				 }

				 private CacheObject currentmovementobject=new CacheObject(Vector3.Zero);
				 public override CacheObject CurrentMovementObject
				 {
						get
						{
							 if (!currentmovementobject.Position.Equals(currentdestination))
							 {
									base.NewTargetResetVars();
									currentmovementobject=new CacheObject(this.CurrentDestination);
							 }

							 return currentmovementobject;
						}
				 }
			}











			public static Kiting kitingMovement=new Kiting();
			public class Kiting : Movement
			{
				 public Kiting()
						: base()
				 {
						condition=new Func<bool>(() =>
						{
							 return Bot.KiteDistance>0
										 &&(!Bot.Combat.bAnyTreasureGoblinsPresent||Bot.SettingsFunky.GoblinPriority<2)
										 &&(Bot.Class.AC!=ActorClass.Wizard||(Bot.Class.AC==ActorClass.Wizard&&(!Bot.Class.HasBuff(SNOPower.Wizard_Archon)||!Bot.SettingsFunky.Class.bKiteOnlyArchon)));
						});

						Break_=new Func<bool>(() => { return this.CurrentDestination.Equals(Vector3.Zero); });
				 }

				 private Func<bool> condition;
				 public override Func<bool> Condition
				 {
						get
						{
							 return condition;
						}
				 }

				 private Func<bool> Break_;
				 public override Func<bool> Break
				 {
						get
						{
							 return Break_;
						}
				 }

				 private DateTime LastGeneratedAvoidanceMovement=DateTime.Today;
				 private double ReuseVectorDurationMS=750;
				 private Vector3 currentdestination=Vector3.Zero;
				 public override Vector3 CurrentDestination
				 {
						get
						{
							 //Resuse last safespot until timer expires!
							 //Reuse the last generated safe spot...
							 if (this.currentdestination!=Vector3.Zero
								 &&DateTime.Now.Subtract(base.LastGeneratedaMovement).TotalMilliseconds>=this.ReuseVectorDurationMS)
							 {
									if (ObjectCache.Objects.IsPointNearbyMonsters(this.currentdestination, Bot.KiteDistance))
									{
										 Log("Kite Movement is no longer valid", true);
										 this.currentdestination=Vector3.Zero;
									}
									else if (Bot.Character.Position.Distance2D(this.currentdestination)<2.5f)
									{
										 Log("Kite Movement distance is less than 2.5f", true);
										 this.currentdestination=Vector3.Zero;
										 return this.currentdestination;
									}
									else
									{
										 return this.currentdestination;
									}
							 }

							 Bot.NavigationCache.RefreshGPArea();
							 //Generate new Destination
							 currentdestination=Bot.NavigationCache.CurrentGPArea.AttemptFindSafeSpot(Bot.Character.Position, Vector3.Zero, true);

							 return this.currentdestination;
						}
						set
						{
							 base.CurrentDestination=value;
						}
				 }
			}


			public static bool MovementEvaluation(out Movement movement)
			{
				 movement=null;
				 bool found=false;

				 //Test each movement condition -- from least important to most important.

				 if (avoidanceMovement.Condition.Invoke())
				 {
						found=true;
						movement=avoidanceMovement;
				 }

				 if (kitingMovement.Condition.Invoke())
				 {
						found=true;
						movement=kitingMovement;
				 }



				 return found;
			}
	 }
}