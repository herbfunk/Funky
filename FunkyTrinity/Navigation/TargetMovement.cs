using System;
using System.Linq;
using Zeta;
using Zeta.TreeSharp;
using Zeta.Common;
using Zeta.Internals.SNO;
using Zeta.Internals.Actors;

using FunkyTrinity.Ability;
using FunkyTrinity.Cache;
using Zeta.Navigation;
using FunkyTrinity.Cache.Enums;

namespace FunkyTrinity.Movement
{

		  public static class TargetMovement
		  {
				//TargetMovement -- Used during Target Handling to move the bot into Interaction Range.

				internal static Vector3 LastPlayerLocation=Vector3.Zero;
				internal static int BlockedMovementCounter=0;
				internal static int NonMovementCounter=0;


				internal static Zeta.Navigation.MoveResult LastMoveResult=Zeta.Navigation.MoveResult.Failed;
				internal static bool UpdatedNavigator=false;
				internal static Vector3 LastNavigatorMoveTo=Vector3.Zero;

				internal static DateTime LastMovementDuringCombat=DateTime.Today;
				internal static DateTime LastMovementAttempted=DateTime.Today;
				internal static DateTime LastMovementCommand=DateTime.Today;

				internal static bool IsAlreadyMoving=false;
				internal static float LastDistanceFromTarget=-1f;

				internal static Vector3 LastTargetLocation=Vector3.Zero;
				internal static Vector3 CurrentTargetLocation=Vector3.Zero;

				//internal bool UsedAutoMovementCommand=false;

				internal static RunStatus TargetMoveTo(CacheObject obj)
				{

					 #region DebugInfo
					 if (Bot.SettingsFunky.Debug.DebugStatusBar)
					 {
						  string Action="[Move-";
						  switch (obj.targetType.Value)
						  {
								case TargetType.Avoidance:
									 Action+="Avoid] ";
									 break;

								case TargetType.LineOfSight:
									 Action+="LOS] ";
									 break;

								case TargetType.Unit:
									 if (Bot.NavigationCache.groupRunningBehavior&&Bot.NavigationCache.groupingCurrentUnit!=null&&Bot.NavigationCache.groupingCurrentUnit==obj)
										  Action+="Grouping] ";
									 else
										  Action+="Combat] ";

									 break;
								case TargetType.Item:
								case TargetType.Gold:
								case TargetType.Globe:
									 Action+="Pickup] ";
									 break;
								case TargetType.Interactable:
									 Action+="Interact] ";
									 break;
								case TargetType.Container:
									 Action+="Open] ";
									 break;
								case TargetType.Destructible:
								case TargetType.Barricade:
									 Action+="Destroy] ";
									 break;
								case TargetType.Shrine:
									 Action+="Click] ";
									 break;
						  }
						  Bot.Target.UpdateStatusText(Action);
					 }
					 #endregion

					 // Are we currently incapacitated? If so then wait...
					 if (Bot.Character.bIsIncapacitated||Bot.Character.bIsRooted)
						  return RunStatus.Running;

					 if (Bot.SettingsFunky.Debug.SkipAhead)
						  SkipAheadCache.RecordSkipAheadCachePoint();

					 // Some stuff to avoid spamming usepower EVERY loop, and also to detect stucks/staying in one place for too long
					 bool bForceNewMovement=false;

					 //Herbfunk: Added this to prevent stucks attempting to move to a target blocked. (Case: 3 champs behind a wall, within range but could not engage due to being on the other side.)
					 if (NonMovementCounter>50)
					 {
						  if (Bot.SettingsFunky.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
								Logger.Write(LogLevel.Movement,"non movement counter reached {0}", NonMovementCounter);

						  if (obj.Actortype.HasValue&&obj.Actortype.Value.HasFlag(ActorType.Item))
						  {
								Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
								Bot.Combat.timeCancelledFleeMove=DateTime.Now;

								//Check if we can walk to this location from current location..
								if (!Navigation.CanRayCast(Bot.Character.Position, CurrentTargetLocation, NavCellFlags.AllowWalk))
								{
									 obj.RequiresLOSCheck=true;
									 if (Bot.SettingsFunky.Debug.FunkyLogFlags.HasFlag(LogLevel.Target))
										  Logger.Write(LogLevel.Movement, "Ignoring Item {0} -- due to AllowWalk RayCast Failure!", obj.InternalName);
									 Bot.Combat.bForceTargetUpdate=true;
									 return RunStatus.Running;
								}
						  }
						  else
						  {
								if (Bot.SettingsFunky.Debug.FunkyLogFlags.HasFlag(LogLevel.Target))
									 Logger.Write(LogLevel.Movement, "Ignoring obj {0} ",obj.InternalName+" _ SNO:"+obj.SNOID);
								obj.BlacklistLoops=50;
								obj.RequiresLOSCheck=true;
								Bot.Combat.bForceTargetUpdate=true;
								NonMovementCounter=0;

								// Reset the emergency loop counter and return success
								return RunStatus.Running;
						  }
					 }

					 Bot.NavigationCache.RefreshMovementCache();
					 Bot.NavigationCache.ObstaclePrioritizeCheck();

					 #region Evaluate Last Action

					 // Count how long we have failed to move - body block stuff etc.
					 if (!Bot.NavigationCache.IsMoving||Bot.NavigationCache.currentMovementState.HasFlag(MovementState.WalkingInPlace)||Bot.NavigationCache.currentMovementState.Equals(MovementState.None))
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

										  if (Bot.NavigationCache.groupRunningBehavior)
										  {
												if (Bot.SettingsFunky.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
													 Logger.Write(LogLevel.Movement, "Grouping Behavior stopped due to blocking counter");

												Bot.NavigationCache.GroupingFinishBehavior();
												Bot.NavigationCache.groupingSuspendedDate=DateTime.Now.AddMilliseconds(2500);
												Bot.Combat.bForceTargetUpdate=true;
												return RunStatus.Running;
										  }

										  if (obj.targetType.Value==TargetType.Avoidance)
										  {//Avoidance Movement..
												Bot.Combat.timeCancelledFleeMove=DateTime.Now;
												Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
												Bot.NavigationCache.CurrentGPArea.BlacklistLastSafespot();
												Bot.UpdateAvoidKiteRates();
												Bot.Combat.bForceTargetUpdate=true;
												return RunStatus.Running;
										  }



										  break;
									 default:
										  if (obj.targetType.Value!=TargetType.Avoidance)
										  {
												//Finally try raycasting to see if navigation is possible..
												if (obj.Actortype.HasValue&&
													 (obj.Actortype.Value==ActorType.Gizmo||obj.Actortype.Value==ActorType.Unit))
												{
													 Vector3 hitTest;
													 // No raycast available, try and force-ignore this for a little while, and blacklist for a few seconds
													 if (Zeta.Navigation.Navigator.Raycast(Bot.Character.Position, obj.Position, out hitTest))
													 {
														  if (hitTest!=Vector3.Zero)
														  {
																obj.RequiresLOSCheck=true;
																obj.BlacklistLoops=10;
																if (Bot.SettingsFunky.Debug.FunkyLogFlags.HasFlag(LogLevel.Target))
																	 Logger.Write(LogLevel.Movement, "Ignoring object "+obj.InternalName+" due to not moving and raycast failure!", true);
																
																Bot.Combat.bForceTargetUpdate=true;
																return RunStatus.Running;
														  }
													 }
												}
												
										  }
										  else
										  {
												if (!Navigation.CanRayCast(Bot.Character.Position, CurrentTargetLocation, NavCellFlags.AllowWalk))
												{
													 Logger.Write(LogLevel.Movement, "Cannot continue with avoidance movement due to raycast failure!");
													 BlockedMovementCounter=0;

													 Bot.Combat.iMillisecondsCancelledEmergencyMoveFor/=2;
													 Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
													 //Ignore avoidance movements.
													 Bot.Combat.iMillisecondsCancelledFleeMoveFor/=2;
													 Bot.Combat.timeCancelledFleeMove=DateTime.Now;

													 Bot.NavigationCache.CurrentGPArea.BlacklistLastSafespot();
													 Bot.Combat.bForceTargetUpdate=true;
													 return RunStatus.Running;
												}
										  }
										  break;
								}
								#endregion


								if (obj.targetType.Value==TargetType.Item)
								{
									 obj.BlacklistLoops=1;
									 Bot.Combat.bForceTargetUpdate=true;
								}

								return RunStatus.Running;
						  } // Been 250 milliseconds of non-movement?
					 }
					 else
					 {
						  // Movement has been made, so count the time last moved!
						  LastMovementDuringCombat=DateTime.Now;
					 }
					 #endregion

					 // Update the last distance stored
					 LastDistanceFromTarget=obj.DistanceFromTarget;

					 // See if we want to ACTUALLY move, or are just waiting for the last move command...
					 if (!bForceNewMovement&&IsAlreadyMoving&&CurrentTargetLocation==LastTargetLocation&&DateTime.Now.Subtract(LastMovementCommand).TotalMilliseconds<=100)
					 {
						  return RunStatus.Running;
					 }

					 float currentDistance=Vector3.Distance(LastTargetLocation, CurrentTargetLocation);

					 // If we're doing avoidance, globes or backtracking, try to use special abilities to move quicker
					 #region SpecialMovementChecks
					 if ((TargetType.Avoidance|TargetType.Gold|TargetType.Globe).HasFlag(obj.targetType.Value))
					 {

						  bool bTooMuchZChange=((Bot.Character.Position.Z-CurrentTargetLocation.Z)>=4f);

						  ability MovementPower;
						  if (Bot.Class.FindMovementPower(out MovementPower))
						  {
								double lastUsedAbilityMS=MovementPower.LastUsedMilliseconds;
								bool foundMovementPower=false;
								float pointDistance=0f;
								Vector3 vTargetAimPoint=CurrentTargetLocation;
								bool ignoreLOSTest=false;

								switch (MovementPower.Power)
								{
									 case SNOPower.Monk_TempestRush:
										  vTargetAimPoint=MathEx.CalculatePointFrom(CurrentTargetLocation, Bot.Character.Position, 10f);
										  Bot.Character.UpdateAnimationState(false, true);
										  bool isHobbling=Bot.Character.CurrentSNOAnim.HasFlag(SNOAnim.Monk_Female_Hobble_Run|SNOAnim.Monk_Male_HTH_Hobble_Run);
										  foundMovementPower=(!bTooMuchZChange&&!Bot.Class.bWaitingForSpecial&&currentDistance<15f&&((isHobbling||lastUsedAbilityMS<300)&&Bot.Character.dCurrentEnergy>15)
												&&!ObjectCache.Obstacles.DoesPositionIntersectAny(vTargetAimPoint, ObstacleType.ServerObject));

										  break;
									 case SNOPower.DemonHunter_Vault:
										  foundMovementPower=(obj.targetType.Value!=TargetType.Destructible&&!bTooMuchZChange&&currentDistance>=18f&&
																	 (lastUsedAbilityMS>=Bot.SettingsFunky.Class.iDHVaultMovementDelay));
										  pointDistance=35f;
										  if (currentDistance>pointDistance)
												vTargetAimPoint=MathEx.CalculatePointFrom(CurrentTargetLocation, Bot.Character.Position, pointDistance);
										  break;
									 case SNOPower.Barbarian_FuriousCharge:
										  foundMovementPower=(obj.targetType.Value!=TargetType.Destructible&&!bTooMuchZChange
																	 &&(currentDistance>20f||obj.targetType.Value.HasFlag(TargetType.Avoidance)&&(NonMovementCounter>0||BlockedMovementCounter>0)));

										  pointDistance=35f;
										  if (currentDistance>pointDistance)
												vTargetAimPoint=MathEx.CalculatePointFrom(CurrentTargetLocation, Bot.Character.Position, pointDistance);

										  break;
									 case SNOPower.Barbarian_Leap:
									 case SNOPower.Wizard_Archon_Teleport:
									 case SNOPower.Wizard_Teleport:
										  foundMovementPower=(obj.targetType.Value!=TargetType.Destructible&&!bTooMuchZChange
																	 &&(currentDistance>20f||obj.targetType.Value.HasFlag(TargetType.Avoidance)&&(NonMovementCounter>0||BlockedMovementCounter>0)));

										  pointDistance=35f;
										  if (currentDistance>pointDistance)
												vTargetAimPoint=MathEx.CalculatePointFrom(CurrentTargetLocation, Bot.Character.Position, pointDistance);

										  //Teleport and Leap ignores raycast testing below!
										  ignoreLOSTest=true;
										  break;
									 case SNOPower.Barbarian_Whirlwind:
										  break;
									 default:
										  Bot.Character.WaitWhileAnimating(3, true);

										  ZetaDia.Me.UsePower(MovementPower.Power, CurrentTargetLocation, Bot.Character.iCurrentWorldID, -1);
										  MovementPower.SuccessfullyUsed();

										  Bot.Character.WaitWhileAnimating(6, true);
										  // Store the current destination for comparison incase of changes next loop
										  LastTargetLocation=CurrentTargetLocation;
										  // Reset total body-block count, since we should have moved
										  if (DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>=2000)
												BlockedMovementCounter=0;
										  return RunStatus.Running;
								}

								if (foundMovementPower)
								{

									 if ((MovementPower.Power==SNOPower.Monk_TempestRush&&lastUsedAbilityMS>500)||
										  (ignoreLOSTest||ZetaDia.Physics.Raycast(Bot.Character.Position, vTargetAimPoint, NavCellFlags.AllowWalk)))
									 {
										  ZetaDia.Me.UsePower(MovementPower.Power, vTargetAimPoint, Bot.Character.iCurrentWorldID, -1);
										  MovementPower.SuccessfullyUsed();

										  // Store the current destination for comparison incase of changes next loop
										  LastTargetLocation=CurrentTargetLocation;

										  // Reset total body-block count, since we should have moved
										  if (DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>=2000)
												BlockedMovementCounter=0;
										  return RunStatus.Running;
									 }
								}
						  }

						  //Special Whirlwind Code
							if (Bot.Class.AC==Zeta.Internals.Actors.ActorClass.Barbarian&&Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_Whirlwind))
						  {
								// Whirlwind against everything within range (except backtrack points)
								if (Bot.Character.dCurrentEnergy>=10
									 &&Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_20]>=1
									 &&obj.DistanceFromTarget<=12f
									 &&(!Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_Sprint)||Bot.Class.HasBuff(SNOPower.Barbarian_Sprint))
									 &&(!(TargetType.Avoidance|TargetType.Gold|TargetType.Globe).HasFlag(obj.targetType.Value)&obj.DistanceFromTarget>=6f)
									 &&(obj.targetType.Value!=TargetType.Unit
									 ||(obj.targetType.Value==TargetType.Unit&&!obj.IsTreasureGoblin
										  &&(!Bot.SettingsFunky.Class.bSelectiveWhirlwind
												||Bot.Combat.bAnyNonWWIgnoreMobsInRange
												||!CacheIDLookup.hashActorSNOWhirlwindIgnore.Contains(obj.SNOID)))))
								{
									 // Special code to prevent whirlwind double-spam, this helps save fury
									 bool bUseThisLoop=SNOPower.Barbarian_Whirlwind!=Bot.Combat.powerLastSnoPowerUsed;
									 if (!bUseThisLoop)
									 {
										  Bot.Combat.powerLastSnoPowerUsed=SNOPower.None;
											if (DateTime.Now.Subtract(PowerCacheLookup.dictAbilityLastUse[SNOPower.Barbarian_Whirlwind]).TotalMilliseconds>=200)
												bUseThisLoop=true;
									 }
									 if (bUseThisLoop)
									 {
										  ZetaDia.Me.UsePower(SNOPower.Barbarian_Whirlwind, CurrentTargetLocation, Bot.Character.iCurrentWorldID, -1);
										  Bot.Combat.powerLastSnoPowerUsed=SNOPower.Barbarian_Whirlwind;
											PowerCacheLookup.dictAbilityLastUse[SNOPower.Barbarian_Whirlwind]=DateTime.Now;
									 }
									 // Store the current destination for comparison incase of changes next loop
									 LastTargetLocation=CurrentTargetLocation;
									 // Reset total body-block count
									 if ((!Bot.Combat.bForceCloseRangeTarget||DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>Bot.Combat.iMillisecondsForceCloseRange)&&
										 DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>=2000)
										  BlockedMovementCounter=0;
									 return RunStatus.Running;
								}
						  }
					 }
					 #endregion


					 // Now for the actual movement request stuff
					 IsAlreadyMoving=true;
					 LastMovementCommand=DateTime.Now;

					 UseTargetMovement(obj, currentDistance, bForceNewMovement);
					 Bot.Target.LastHealthChange=DateTime.Now;
					 return RunStatus.Running;
				}

				internal static void UseTargetMovement(CacheObject obj, float currentDistance, bool bForceNewMovement=false)
				{
					 if (DateTime.Now.Subtract(LastMovementAttempted).TotalMilliseconds>=250||currentDistance>=2f||bForceNewMovement)
					 {
						  if (obj.targetType.Value.Equals(TargetType.Avoidance))
						  {
								if (NonMovementCounter<10||currentDistance>50f)
									 ZetaDia.Me.Movement.MoveActor(CurrentTargetLocation);
								else
									 ZetaDia.Me.UsePower(SNOPower.Walk, CurrentTargetLocation, Bot.Character.iCurrentWorldID, -1);
						  }
						  else
						  {
								//Use Walk Power when not using LOS Movement, target is not an item and target does not ignore LOS.
								bool UsePower=(NonMovementCounter<10&&!obj.UsingLOSV3&&!obj.IgnoresLOSCheck&&
									 obj.targetType.Value!=TargetType.Item&&obj.targetType.Value!=TargetType.Interactable);
								if (UsePower)
								{
									 ZetaDia.Me.UsePower(SNOPower.Walk, CurrentTargetLocation, Bot.Character.iCurrentWorldID, -1);
								}
								else
									 ZetaDia.Me.Movement.MoveActor(CurrentTargetLocation);
						  }

						  LastMovementAttempted=DateTime.Now;
						  // Store the current destination for comparison incase of changes next loop
						  LastTargetLocation=CurrentTargetLocation;
						  // Reset total body-block count, since we should have moved
						  if (DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>=2000)
								BlockedMovementCounter=0;

						  //Herbfunk: Quick fix for stuck occuring on above average mob who is damaged..
						  if (LastPlayerLocation.Distance(Bot.Character.Position)<=5f)
								NonMovementCounter++;
						  else
								NonMovementCounter=0;

						  LastPlayerLocation=Bot.Character.Position;
					 }
				}

				internal static void UseNavigationMovement(CacheObject obj)
				{
					 if (LastNavigatorMoveTo==Vector3.Zero||LastMoveResult.HasFlag(MoveResult.PathGenerationFailed)||LastNavigatorMoveTo!=obj.Position)
					 {
						  Zeta.Navigation.Navigator.Clear();
						  LastNavigatorMoveTo=obj.Position;
						  LastMoveResult=Zeta.Navigation.Navigator.MoveTo(LastNavigatorMoveTo, obj.InternalName, true);
					 }
					 else if (!LastMoveResult.HasFlag(MoveResult.ReachedDestination)||Bot.Character.Position.Distance2D(LastNavigatorMoveTo)<7.5f)
					 {
						  LastMoveResult=Zeta.Navigation.Navigator.MoveTo(LastNavigatorMoveTo, obj.InternalName, true);
					 }
					 else
					 {
						  //
					 }
				}


				internal static void ResetTargetMovementVars()
				{
					 BlockedMovementCounter=0;
					 NonMovementCounter=0;
					 LastNavigatorMoveTo=Vector3.Zero;
					 LastMoveResult=Zeta.Navigation.MoveResult.Failed;

					 NewTargetResetVars();
				}
				internal static void NewTargetResetVars()
				{
					 LastDistanceFromTarget=-1f;
					 IsAlreadyMoving=false;
					 LastMovementCommand=DateTime.Today;
				}
		  }
    
}