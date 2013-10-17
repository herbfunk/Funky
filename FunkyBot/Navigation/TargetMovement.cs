using System;
using System.Linq;
using FunkyBot.AbilityFunky;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using Zeta;
using Zeta.TreeSharp;
using Zeta.Common;
using Zeta.Internals.SNO;
using Zeta.Internals.Actors;
using Zeta.Navigation;

namespace FunkyBot.Movement
{

		  public static class TargetMovement
		  {
				//TargetMovement -- Used during Target Handling to move the bot into Interaction Range.

				internal static Vector3 LastPlayerLocation=Vector3.Zero;
				internal static int BlockedMovementCounter=0;
				internal static int NonMovementCounter=0;



				internal static DateTime LastMovementDuringCombat=DateTime.Today;
				internal static DateTime LastMovementAttempted=DateTime.Today;
				internal static DateTime LastMovementCommand=DateTime.Today;

				internal static bool IsAlreadyMoving=false;

				internal static Vector3 LastTargetLocation=Vector3.Zero;
				internal static Vector3 CurrentTargetLocation=Vector3.Zero;

				//internal bool UsedAutoMovementCommand=false;

				internal static RunStatus TargetMoveTo(CacheObject obj)
				{

					 #region DebugInfo
					 if (Bot.Settings.Debug.DebugStatusBar)
					 {
						  string Action="[Move-";
						  switch (obj.targetType.Value)
						  {
								case TargetType.Avoidance:
									 Action+="Avoid] ";
									 break;
								case TargetType.Fleeing:
									 Action+="Flee] ";
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
						  Bot.Targeting.UpdateStatusText(Action);
					 }
					 #endregion

					 // Are we currently incapacitated? If so then wait...
					 if (Bot.Character.bIsIncapacitated||Bot.Character.bIsRooted)
						  return RunStatus.Running;

					 if (Bot.Settings.Debug.SkipAhead)
						  SkipAheadCache.RecordSkipAheadCachePoint();

					 // Some stuff to avoid spamming usepower EVERY loop, and also to detect stucks/staying in one place for too long
					 bool bForceNewMovement=false;

					 //Herbfunk: Added this to prevent stucks attempting to move to a target blocked. (Case: 3 champs behind a wall, within range but could not engage due to being on the other side.)
					 if (NonMovementCounter>Bot.Settings.Plugin.MovementNonMovementCount)
					 {
						  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
								Logger.Write(LogLevel.Movement,"non movement counter reached {0}", NonMovementCounter);

						  if (obj.Actortype.HasValue&&obj.Actortype.Value.HasFlag(ActorType.Item))
						  {
								if (NonMovementCounter>250)
								{
									//Are we stuck?
									 if (!Navigation.MGP.CanStandAt(Bot.Character.Position))
									 {
										  Logging.Write("Character is stuck inside non-standable location.. attempting townportal cast..");
										  ZetaDia.Me.UseTownPortal();
										  NonMovementCounter=0;
										  return RunStatus.Running;
									 }
								}

								Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
								Bot.Combat.timeCancelledFleeMove=DateTime.Now;

								//Check if we can walk to this location from current location..
								if (!Navigation.CanRayCast(Bot.Character.Position, CurrentTargetLocation, UseSearchGridProvider: true))
								{
									 obj.RequiresLOSCheck=true;
									 obj.BlacklistLoops=50;

									 if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Target))
										  Logger.Write(LogLevel.Movement, "Ignoring Item {0} -- due to RayCast Failure!", obj.InternalName);

									 Bot.Targeting.bForceTargetUpdate=true;
									 return RunStatus.Running;
								}
						  }
						  else if(obj.targetType.Value == TargetType.LineOfSight)
						  {

								if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Target))
									 Logger.Write(LogLevel.Movement, "Line of Sight Movement Stalled!");

								Bot.NavigationCache.LOSmovementObject=null;
								Bot.Targeting.bForceTargetUpdate=true;
								NonMovementCounter=0;
								// Reset the emergency loop counter and return success
								return RunStatus.Running;
						  }
						  else
						  {
								if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Target))
									 Logger.Write(LogLevel.Movement, "Ignoring obj {0} ",obj.InternalName+" _ SNO:"+obj.SNOID);
								obj.BlacklistLoops=50;
								obj.RequiresLOSCheck=true;
								Bot.Targeting.bForceTargetUpdate=true;
								NonMovementCounter=0;

								// Reset the emergency loop counter and return success
								return RunStatus.Running;
						  }
					 }

					 Bot.NavigationCache.RefreshMovementCache();
					 Bot.NavigationCache.ObstaclePrioritizeCheck();

					 #region Evaluate Last Action

					 // Count how long we have failed to move - body block stuff etc.
					 if (!Bot.NavigationCache.IsMoving||Bot.NavigationCache.currentMovementState==MovementState.WalkingInPlace||Bot.NavigationCache.currentMovementState.Equals(MovementState.None))
					 {
						  bForceNewMovement=true;
						  if (DateTime.Now.Subtract(LastMovementDuringCombat).TotalMilliseconds>=250)
						  {
								LastMovementDuringCombat=DateTime.Now;
								BlockedMovementCounter++;

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
												if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
													 Logger.Write(LogLevel.Movement, "Grouping Behavior stopped due to blocking counter");

												Bot.NavigationCache.GroupingFinishBehavior();
												Bot.NavigationCache.groupingSuspendedDate=DateTime.Now.AddMilliseconds(2500);
												Bot.Targeting.bForceTargetUpdate=true;
												return RunStatus.Running;
										  }

										  if (!ObjectCache.CheckTargetTypeFlag(obj.targetType.Value,TargetType.AvoidanceMovements))
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
																if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Target))
																	 Logger.Write(LogLevel.Movement, "Ignoring object "+obj.InternalName+" due to not moving and raycast failure!", true);
																
																Bot.Targeting.bForceTargetUpdate=true;
																return RunStatus.Running;
														  }
													 }
												}
												else if (obj.targetType.Value==TargetType.Item)
												{
													 obj.BlacklistLoops=1;
													 Bot.Targeting.bForceTargetUpdate=true;
												}
										  }
										  else
										  {
												if (!Navigation.CanRayCast(Bot.Character.Position, CurrentTargetLocation, NavCellFlags.AllowWalk))
												{
													 if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
														  Logger.Write(LogLevel.Movement, "Cannot continue with avoidance movement due to raycast failure!");
													 BlockedMovementCounter=0;

													 Bot.Combat.iMillisecondsCancelledEmergencyMoveFor/=2;
													 Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
													 //Ignore avoidance movements.
													 Bot.Combat.iMillisecondsCancelledFleeMoveFor/=2;
													 Bot.Combat.timeCancelledFleeMove=DateTime.Now;

													 Bot.NavigationCache.CurrentGPArea.BlacklistLastSafespot();
													 Bot.Targeting.bForceTargetUpdate=true;
													 return RunStatus.Running;
												}
										  }
										  break;
								}
								#endregion




								return RunStatus.Running;
						  } // Been 250 milliseconds of non-movement?
					 }
					 else
					 {
						  // Movement has been made, so count the time last moved!
						  LastMovementDuringCombat=DateTime.Now;
					 }
					 #endregion

					 // See if we want to ACTUALLY move, or are just waiting for the last move command...
					 if (!bForceNewMovement&&IsAlreadyMoving&&CurrentTargetLocation==LastTargetLocation&&DateTime.Now.Subtract(LastMovementCommand).TotalMilliseconds<=100)
					 {
						  return RunStatus.Running;
					 }

					 // If we're doing avoidance, globes or backtracking, try to use special abilities to move quicker
					 #region SpecialMovementChecks
					 if (ObjectCache.CheckTargetTypeFlag(obj.targetType.Value,TargetType.AvoidanceMovements|TargetType.Gold|TargetType.Globe))
					 {

						  bool bTooMuchZChange=((Bot.Character.Position.Z-CurrentTargetLocation.Z)>=4f);

						  Ability MovementPower;
						  Vector3 MovementVector=Bot.Class.FindCombatMovementPower(out MovementPower, obj.Position);
						  if (MovementVector!=Vector3.Zero)
						  {
								ZetaDia.Me.UsePower(MovementPower.Power, MovementVector, Bot.Character.iCurrentWorldID, -1);
								MovementPower.OnSuccessfullyUsed();

								// Store the current destination for comparison incase of changes next loop
								LastTargetLocation=CurrentTargetLocation;
								// Reset total body-block count, since we should have moved
								//if (DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>=2000)
								BlockedMovementCounter=0;

								return RunStatus.Running;
						  }

						  //Special Whirlwind Code
							if (Bot.Class.AC==Zeta.Internals.Actors.ActorClass.Barbarian&&Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_Whirlwind))
						  {
								// Whirlwind against everything within range (except backtrack points)
								if (Bot.Character.dCurrentEnergy>=10
									 &&Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_20]>=1
									 &&obj.DistanceFromTarget<=12f
									 &&(!Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_Sprint)||Bot.Class.HasBuff(SNOPower.Barbarian_Sprint))
									 &&(ObjectCache.CheckTargetTypeFlag(obj.targetType.Value,TargetType.AvoidanceMovements|TargetType.Gold|TargetType.Globe)==false)
									 &&(obj.targetType.Value!=TargetType.Unit
									 ||(obj.targetType.Value==TargetType.Unit&&!obj.IsTreasureGoblin
										  &&(!Bot.Settings.Class.bSelectiveWhirlwind
												||Bot.Combat.bAnyNonWWIgnoreMobsInRange
												||!CacheIDLookup.hashActorSNOWhirlwindIgnore.Contains(obj.SNOID)))))
								{
									 // Special code to prevent whirlwind double-spam, this helps save fury
									 bool bUseThisLoop=SNOPower.Barbarian_Whirlwind!=Bot.Class.LastUsedAbility.Power;
									 if (!bUseThisLoop)
									 {
											if (Bot.Class.Abilities[SNOPower.Barbarian_Whirlwind].LastUsedMilliseconds>=200)
												bUseThisLoop=true;
									 }
									 if (bUseThisLoop)
									 {
										  ZetaDia.Me.UsePower(SNOPower.Barbarian_Whirlwind, CurrentTargetLocation, Bot.Character.iCurrentWorldID, -1);
										  Bot.Class.Abilities[SNOPower.Barbarian_Whirlwind].OnSuccessfullyUsed();
									 }
									 // Store the current destination for comparison incase of changes next loop
									 LastTargetLocation=CurrentTargetLocation;
									 BlockedMovementCounter=0;
									 return RunStatus.Running;
								}
						  }
					 }
					 #endregion


					 // Now for the actual movement request stuff
					 IsAlreadyMoving=true;
					 LastMovementCommand=DateTime.Now;

					 UseTargetMovement(obj, bForceNewMovement);
					 return RunStatus.Running;
				}

				internal static void UseTargetMovement(CacheObject obj, bool bForceNewMovement=false)
				{
					 float currentDistance=Vector3.Distance(LastTargetLocation, CurrentTargetLocation);
					 if (DateTime.Now.Subtract(LastMovementAttempted).TotalMilliseconds>=250||(currentDistance>=2f&&!Bot.NavigationCache.IsMoving)||bForceNewMovement)
					 {
						  if (ObjectCache.CheckTargetTypeFlag(obj.targetType.Value,TargetType.AvoidanceMovements))
						  {
								if (NonMovementCounter<10||currentDistance>50f)
									 ZetaDia.Me.Movement.MoveActor(CurrentTargetLocation);
								else
									 ZetaDia.Me.UsePower(SNOPower.Walk, CurrentTargetLocation, Bot.Character.iCurrentWorldID, -1);
						  }
						  else if(obj.targetType.Value.Equals(TargetType.LineOfSight))
						  {
								//MoveResult LastMovementResult=Funky.PlayerMover.NavigateTo(CurrentTargetLocation, "Line-Of-Sight");
								//if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
								//	 Logger.Write(LogLevel.Movement, "Last Line-Of-Sight Move Result=={0}", LastMovementResult.ToString());
								if (Navigation.NP.CurrentPath.Count>0)
									 ZetaDia.Me.UsePower(SNOPower.Walk, Navigation.NP.CurrentPath.Current, Bot.Character.iCurrentWorldID, -1);
								
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

						  //Herbfunk: Quick fix for stuck occuring on above average mob who is damaged..
						  if (LastPlayerLocation.Distance(Bot.Character.Position)<=5f)
								NonMovementCounter++;
						  else
						  {
								NonMovementCounter=0;
								BlockedMovementCounter=0;
						  }
								

						  LastPlayerLocation=Bot.Character.Position;
					 }
				}

				internal static void ResetTargetMovementVars()
				{
					 BlockedMovementCounter=0;
					 NonMovementCounter=0;
					 NewTargetResetVars();
				}
				internal static void NewTargetResetVars()
				{
					 IsAlreadyMoving=false;
					 LastMovementCommand=DateTime.Today;
				}
		  }
    
}