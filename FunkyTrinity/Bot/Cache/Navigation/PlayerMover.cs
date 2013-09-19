using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals;
using Zeta.Internals.Actors;
using Zeta.Internals.SNO;
using Zeta.Navigation;
using Zeta.TreeSharp;
using FunkyTrinity.Enums;
using FunkyTrinity.ability;
using FunkyTrinity.Cache;
using FunkyTrinity.Movement;


namespace FunkyTrinity
{
	 public partial class Funky
	 {


		  // **********************************************************************************************
		  // *****                             Player Mover Class                                     *****
		  // **********************************************************************************************
		  internal class PlayerMover : IPlayerMover
		  {

				private static bool ShrinesInArea(Vector3 targetpos)
				{
					 List<CacheInteractable> objects;
					 ObjectCache.Objects.FindSurroundingObjects(targetpos, 10f, out objects);
					 objects.RemoveAll(obj => obj.targetType.Value!=TargetType.Shrine);
					 objects.TrimExcess();
					 return objects.Count>0;

				} // ShrinesInArea checker

				public void MoveStop()
				{
					 ZetaDia.Me.UsePower(SNOPower.Walk, ZetaDia.Me.Position, Bot.Character.iCurrentWorldID, -1);
				}
				// Anti-stuck variables
				private static Vector3 vOldMoveToTarget=Vector3.Zero;
				internal static int iTimesReachedStuckPoint=0;
				internal static int iTotalAntiStuckAttempts=1;
				internal static Vector3 vSafeMovementLocation=Vector3.Zero;
				internal static DateTime timeLastRecordedPosition=DateTime.Today;
				internal static Vector3 vOldPosition=Vector3.Zero;
				internal static DateTime timeStartedUnstuckMeasure=DateTime.Today;
				internal static int iTimesReachedMaxUnstucks=0;
				internal static DateTime timeCancelledUnstuckerFor=DateTime.Today;
				internal static DateTime timeLastReportedAnyStuck=DateTime.Today;
				internal static int iCancelUnstuckerForSeconds=60;
				internal static DateTime timeLastRestartedGame=DateTime.Today;
				internal static bool ShouldHandleObstacleObject=false;
				// **********************************************************************************************
				// *****                         Check if we are stuck or not                               *****
				// **********************************************************************************************
				// Simply checks for position changing max once every 3 seconds, to decide on stuck
				public static bool UnstuckChecker(Vector3 vMyCurrentPosition)
				{
					 // Keep checking distance changes every 3 seconds
					 if (DateTime.Now.Subtract(timeLastRecordedPosition).TotalMilliseconds>=3500) //herbfunk: added 500ms
					 {
						  timeLastRecordedPosition=DateTime.Now;
						  if (vOldPosition!=Vector3.Zero&&vOldPosition.Distance(vMyCurrentPosition)<=4f)
						  {
								return true;
						  }
						  vOldPosition=vMyCurrentPosition;
					 }

					 return false;
				}

				// **********************************************************************************************
				// *****                Actually deal with a stuck - find an unstuck point etc.             *****
				// **********************************************************************************************
				public static Vector3 UnstuckHandler(Vector3 vMyCurrentPosition, Vector3 vOriginalDestination)
				{
					 // Update the last time we generated a path
					 timeStartedUnstuckMeasure=DateTime.Now;



					 // If we got stuck on a 2nd/3rd/4th "chained" anti-stuck route, then return the old move to target to keep movement of some kind going
					 if (iTimesReachedStuckPoint>0)
					 {
						  vSafeMovementLocation=Vector3.Zero;
						  iTimesReachedStuckPoint++;
						  // Reset the path and allow a whole "New" unstuck generation next cycle
						  iTimesReachedStuckPoint=0;
						  // And cancel unstucking for 9 seconds so DB can try to navigate
						  iCancelUnstuckerForSeconds=(9*iTotalAntiStuckAttempts);
						  if (iCancelUnstuckerForSeconds<20)
								iCancelUnstuckerForSeconds=20;
						  timeCancelledUnstuckerFor=DateTime.Now;
						  Navigator.Clear();
						  Logging.WriteDiagnostic("[Funky] Clearing old route and trying new path find to: "+vOldMoveToTarget.ToString());
						  Navigator.MoveTo(vOldMoveToTarget, "original destination", false);
						  return vSafeMovementLocation;
					 }

					 // Only try an unstuck 10 times maximum in XXX period of time
					 if (Vector3.Distance(vOriginalDestination, vMyCurrentPosition)>=700f)
					 {
						  Logging.Write("[Funky] You are "+Vector3.Distance(vOriginalDestination, vMyCurrentPosition).ToString()+" distance away from your destination.");
						  Logging.Write("[Funky] This is too far for the unstucker, and is likely a sign of ending up in the wrong map zone.");
						  ReloadCurrentProfile();

					 }
					 if (iTotalAntiStuckAttempts<=8)
					 {
						  //Check cache for barricades..
						  if (ObjectCache.Objects.OfType<CacheDestructable>().Any(CO => CO.RadiusDistance<=10f))
						  {
								Logging.Write("[Funky] Found nearby barricade, flagging barricade destruction!");
								ShouldHandleObstacleObject=true;
						  }

						  Logging.Write("[Funky] Your bot got stuck! Trying to unstuck (attempt #"+iTotalAntiStuckAttempts.ToString()+" of 8 attempts)");
						  Logging.WriteDiagnostic("(destination="+vOriginalDestination.ToString()+", which is "+Vector3.Distance(vOriginalDestination, vMyCurrentPosition).ToString()+" distance away)");
						  
						  if (Bot.SettingsFunky.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
								Logger.Write(LogLevel.Movement, "Stuck Flags: {0}", Bot.NavigationCache.Stuckflags.ToString());

						  bool FoundRandomMovementLocation=Bot.NavigationCache.AttemptFindSafeSpot(out vSafeMovementLocation, Vector3.Zero);

						  // Temporarily log stuff
						  if (iTotalAntiStuckAttempts==1&&Bot.SettingsFunky.Debug.LogStuckLocations)
						  {
								string sLogFileName=LoggingPrefixString+" -- Stucks.log";
								FileStream LogStream=File.Open(LoggingFolderPath+sLogFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
								using (StreamWriter LogWriter=new StreamWriter(LogStream))
								{
									 LogWriter.WriteLine(DateTime.Now.ToString()+": Original Destination="+vOldMoveToTarget.ToString()+". Current player position when stuck="+vMyCurrentPosition.ToString());
									 LogWriter.WriteLine("Profile Name="+ProfileManager.CurrentProfile.Name);
								}
						  }

						  // Now count up our stuck attempt generations
						  iTotalAntiStuckAttempts++;

						  if (FoundRandomMovementLocation)
						  {
								return vSafeMovementLocation;
						  }
						  else
						  {
								Navigator.Clear();
								Navigator.MoveTo(vOriginalDestination, "original destination", false);
								iCancelUnstuckerForSeconds=40;
								timeCancelledUnstuckerFor=DateTime.Now;
								return Vector3.Zero;
						  }
					 }

					 iTimesReachedMaxUnstucks++;
					 iTotalAntiStuckAttempts=1;
					 vSafeMovementLocation=Vector3.Zero;
					 vOldPosition=Vector3.Zero;
					 iTimesReachedStuckPoint=0;
					 timeLastRecordedPosition=DateTime.Today;
					 timeStartedUnstuckMeasure=DateTime.Today;
					 int iSafetyLoops=0;

					 if (iTimesReachedMaxUnstucks==1)
					 {
						  Navigator.Clear();

						  Logging.Write("[Funky] Anti-stuck measures now attempting to kickstart DB's path-finder into action.");
						  if (Vector3.Distance(vOriginalDestination, vMyCurrentPosition)>=200f)
						  {
								iTimesReachedMaxUnstucks++;
						  }
						  else
						  {

								Navigator.MoveTo(vOriginalDestination, "original destination", false);
								iCancelUnstuckerForSeconds=40;
								timeCancelledUnstuckerFor=DateTime.Now;
								return vSafeMovementLocation;
						  }
					 }

					 if (iTimesReachedMaxUnstucks==2)
					 {
						  Logging.Write("[Funky] Anti-stuck measures failed. Now attempting to reload current profile.");
						  // First see if we need to, and can, teleport to town
						  while (!ZetaDia.Me.IsInTown)
						  {
								iSafetyLoops++;
								Bot.Character.WaitWhileAnimating(5, true);
								ZetaDia.Me.UsePower(SNOPower.UseStoneOfRecall, ZetaDia.Me.Position, ZetaDia.Me.WorldDynamicId, -1);
								Thread.Sleep(1000);
								Bot.Character.WaitWhileAnimating(1000, true);
								if (iSafetyLoops>5)
									 break;
						  }
						  Thread.Sleep(1000);
						  // As long as we successfully reached town, reload the profile
						  if (ZetaDia.Me.IsInTown)
						  {

								string profile=Bot.Stats.sFirstProfileSeen;

								if (!string.IsNullOrEmpty(profile))
								{
									 ProfileManager.Load(profile);

									 Logging.Write("[Funky] Exiting game to continue with next profile.");
									 // Attempt to teleport to town first for a quicker exit
									 iSafetyLoops=0;
									 while (!ZetaDia.Me.IsInTown)
									 {
										  iSafetyLoops++;
										  Bot.Character.WaitWhileAnimating(5, true);
										  ZetaDia.Me.UsePower(SNOPower.UseStoneOfRecall, ZetaDia.Me.Position, ZetaDia.Me.WorldDynamicId, -1);
										  Thread.Sleep(1000);
										  Bot.Character.WaitWhileAnimating(1000, true);
										  if (iSafetyLoops>5)
												break;
									 }
									 Thread.Sleep(1000);
									 ZetaDia.Service.Party.LeaveGame();
									 //ZetaDia.Service.Games.LeaveGame();
									 Funky.ResetGame();
									 // Wait for 10 second log out timer if not in town, else wait for 3 seconds instead
									 Thread.Sleep(!ZetaDia.Me.IsInTown?10000:3000);
								}
								else
								{
									 ProfileManager.Load(Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile);
								}


								Logging.Write("[Funky] Anti-stuck successfully reloaded current profile, DemonBuddy now navigating again.");
								Thread.Sleep(3000);
								return vSafeMovementLocation;
						  }
						  // Didn't make it to town, so skip instantly to the exit game system
						  iTimesReachedMaxUnstucks=3;
					 }
					 // Exit the game and reload the profile
					 if (Bot.SettingsFunky.Debug.RestartGameOnLongStucks&&DateTime.Now.Subtract(timeLastRestartedGame).TotalMinutes>=15)
					 {
						  HadDisconnectError=true;
						  timeLastRestartedGame=DateTime.Now;
						  string sUseProfile=Bot.Stats.sFirstProfileSeen;
						  Logging.Write("[Funky] Anti-stuck measures exiting current game.");
						  // Load the first profile seen last run
						  ProfileManager.Load(!string.IsNullOrEmpty(sUseProfile)
														  ?sUseProfile
														  :Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile);
						  Thread.Sleep(1000);
						  Funky.ResetGame();
						  ZetaDia.Service.Party.LeaveGame();
						  // Wait for 10 second log out timer if not in town
						  if (!ZetaDia.Me.IsInTown)
						  {
								Thread.Sleep(10000);
						  }
					 }
					 else
					 {
						  Logging.Write("[Funky] Unstucking measures failed. Now stopping Trinity unstucker for 12 minutes to inactivity timers to kick in or DB to auto-fix.");
						  iCancelUnstuckerForSeconds=720;
						  timeCancelledUnstuckerFor=DateTime.Now;
						  return vSafeMovementLocation;
					 }
					 return vSafeMovementLocation;
				}




				// **********************************************************************************************
				// *****               Handle moveto requests from the current routine/profile              *****
				// **********************************************************************************************
				// This replaces DemonBuddy's own built-in "Basic movement handler" with a custom one
				internal static Vector3 vLastMoveTo=Vector3.Zero;
				private static bool bLastWaypointWasTown=false;
				private static HashSet<Vector3> hashDoneThisVector=new HashSet<Vector3>();
				internal static DateTime LastCombatPointChecked=DateTime.Today;
				internal static Vector3 CurrentMovementPosition=Vector3.Zero;
				private static Vector3 vShiftedPosition=Vector3.Zero;
				private static DateTime lastShiftedPosition=DateTime.Today;
				private static int iShiftPositionFor=0;

				public void MoveTowards(Vector3 vMoveToTarget)
				{
					 if (Bot.Class==null)
					 {
						  Logging.Write("Bot did not properly initilize, stopping bot!");
						  BotMain.Stop(false, "Bot Init Failure");
						  return;
					 }

					 #region LogStucks
					 // The below code is to help profile/routine makers avoid waypoints with a long distance between them.
					 // Long-distances between waypoints is bad - it increases stucks, and forces the DB nav-server to be called.

					 //if (vLastMoveTo==Vector3.Zero) vLastMoveTo=vMoveToTarget;

					 if (vMoveToTarget!=vLastMoveTo)
					 {
						  vLastMoveTo=vMoveToTarget;

						  if (Bot.SettingsFunky.Debug.LogStuckLocations)
						  {
								vLastMoveTo=vMoveToTarget;
								bLastWaypointWasTown=false;

								float fDistance=Vector3.Distance(vMoveToTarget, vLastMoveTo);
								// Log if not in town, last waypoint wasn't FROM town, and the distance is >200 but <2000 (cos 2000+ probably means we changed map zones!)
								if (!ZetaDia.Me.IsInTown&&!bLastWaypointWasTown&&fDistance>=200&fDistance<=2000)
								{
									 if (!hashDoneThisVector.Contains(vMoveToTarget))
									 {
										  // Log it
										  string sLogFileName=LoggingPrefixString+" -- LongPaths.log";
										  FileStream LogStream=File.Open(LoggingFolderPath+sLogFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
										  using (StreamWriter LogWriter=new StreamWriter(LogStream))
										  {
												LogWriter.WriteLine(DateTime.Now.ToString()+":");
												LogWriter.WriteLine("Profile Name="+ProfileManager.CurrentProfile.Name);
												LogWriter.WriteLine("'From' Waypoint="+vLastMoveTo.ToString()+". 'To' Waypoint="+vMoveToTarget.ToString()+". Distance="+fDistance.ToString());
										  }
										  //LogStream.Close();
										  hashDoneThisVector.Add(vMoveToTarget);
									 }
								}
						  }

						  if (ZetaDia.Me.IsInTown)
								bLastWaypointWasTown=true;
					 }

					 #endregion



					 // Make sure GilesTrinity doesn't want us to avoid routine-movement
					 if (Bot.Combat.DontMove)
						  return;

					 // Store player current position
					 Vector3 vMyCurrentPosition=ZetaDia.Me.Position;
					 CurrentMovementPosition=Bot.NavigationCache.CurrentPathVector;
					 

					 //Check GPC entry (backtracking cache) -- only when not in town!
					 if (BackTrackCache.EnableBacktrackGPRcache&&!ZetaDia.Me.IsInTown)
					 {
						  if (DateTime.Now.Subtract(LastCombatPointChecked).TotalMilliseconds>1250)
						  {
								if (BackTrackCache.cacheMovementGPRs.Count==0)
									 BackTrackCache.StartNewGPR(vMyCurrentPosition);
								else
								{//Add new point only if distance is 25f difference
									 if (BackTrackCache.cacheMovementGPRs.Count==1)
									 {
										  if (BackTrackCache.cacheMovementGPRs[0].CreationVector.Distance(vMyCurrentPosition)>=BackTrackCache.MinimumRangeBetweenMovementGPRs)
										  {
												BackTrackCache.StartNewGPR(vMyCurrentPosition);
										  }
									 }
									 else
									 {
										  //Only if no GPCs currently are less than 20f from us..
										  if (!BackTrackCache.cacheMovementGPRs.Any(GPC => GPC.CreationVector.Distance(vMyCurrentPosition)<BackTrackCache.MinimumRangeBetweenMovementGPRs))
										  {
												BackTrackCache.StartNewGPR(vMyCurrentPosition);
										  }
									 }
								}
								//Reset Timer
								LastCombatPointChecked=DateTime.Now;
						  }
					 }

					 //Special cache for skipping locations visited.
					 if (Bot.SettingsFunky.Debug.SkipAhead) SkipAheadCache.RecordSkipAheadCachePoint();

					 // Store distance to current moveto target
					 float fDistanceFromTarget;

					 #region Unstucker
					 // Do unstuckery things
					 if (Bot.SettingsFunky.Debug.EnableUnstucker)
					 {
						  // Store the "real" (not anti-stuck) destination
						  vOldMoveToTarget=vMoveToTarget;

						  // See if we can reset the 10-limit unstuck counter, if >120 seconds since we last generated an unstuck location
						  if (iTotalAntiStuckAttempts>1&&DateTime.Now.Subtract(timeStartedUnstuckMeasure).TotalSeconds>=120)
						  {
								iTotalAntiStuckAttempts=1;
								iTimesReachedStuckPoint=0;
								vSafeMovementLocation=Vector3.Zero;
						  }

						  // See if we need to, and can, generate unstuck actions
						  if (DateTime.Now.Subtract(timeCancelledUnstuckerFor).TotalSeconds>iCancelUnstuckerForSeconds&&UnstuckChecker(vMyCurrentPosition))
						  {
								// Record the time we last apparently couldn't move for a brief period of time
								timeLastReportedAnyStuck=DateTime.Now;
								// See if there's any stuck position to try and navigate to generated by random mover
								vSafeMovementLocation=UnstuckHandler(vMyCurrentPosition, vOldMoveToTarget);
								if (vSafeMovementLocation==Vector3.Zero)
									 return;
						  }

						  // See if we can clear the total unstuckattempts if we haven't been stuck in over 6 minutes.
						  if (DateTime.Now.Subtract(timeLastReportedAnyStuck).TotalSeconds>=360)
						  {
								iTimesReachedMaxUnstucks=0;
						  }

						  // Did we have a safe point already generated (eg from last loop through), if so use it as our current location instead
						  if (vSafeMovementLocation!=Vector3.Zero)
						  {
								// Set our current movement target to the safe point we generated last cycle
								vMoveToTarget=vSafeMovementLocation;
						  }

						  // Get distance to current destination
						  fDistanceFromTarget=Vector3.Distance(vMyCurrentPosition, vMoveToTarget);

						  // Remove the stuck position if it's been reached, this bit of code also creates multiple stuck-patterns in an ever increasing amount
						  if (vSafeMovementLocation!=Vector3.Zero&&fDistanceFromTarget<=3f)
						  {
								vSafeMovementLocation=Vector3.Zero;
								iTimesReachedStuckPoint++;
								// Do we want to immediately generate a 2nd waypoint to "chain" anti-stucks in an ever-increasing path-length?
								if (iTimesReachedStuckPoint<=iTotalAntiStuckAttempts)
								{
									 Bot.NavigationCache.AttemptFindSafeSpot(out vSafeMovementLocation, Vector3.Zero);
									 vMoveToTarget=vSafeMovementLocation;
								}
								else
								{
									 Logging.WriteDiagnostic("[Funky] Clearing old route and trying new path find to: "+vOldMoveToTarget.ToString());
									 // Reset the path and allow a whole "New" unstuck generation next cycle
									 iTimesReachedStuckPoint=0;
									 // And cancel unstucking for 9 seconds so DB can try to navigate
									 iCancelUnstuckerForSeconds=(9*iTotalAntiStuckAttempts);
									 if (iCancelUnstuckerForSeconds<20)
										  iCancelUnstuckerForSeconds=20;
									 timeCancelledUnstuckerFor=DateTime.Now;
									 Navigator.Clear();
									 Navigator.MoveTo(vOldMoveToTarget, "original destination", false);
									 return;
								}
						  }
					 }
					 else
					 {
						  // Get distance to current destination
						  fDistanceFromTarget=vMyCurrentPosition.Distance(vMoveToTarget);
					 } // Is the built-in unstucker enabled or not? 
					 #endregion


					 //Prioritize "blocking" objects.
					 if (!Bot.Character.bIsInTown) Bot.NavigationCache.ObstaclePrioritizeCheck();
					 

					 


					 #region MovementAbilities
					 // See if we can use abilities like leap etc. for movement out of combat, but not in town and only if we can raycast.
					 if (Bot.SettingsFunky.OutOfCombatMovement&&!ZetaDia.Me.IsInTown)
					 {
						  bool bTooMuchZChange=((vMyCurrentPosition.Z-vMoveToTarget.Z)>=4f);

						  Ability MovementPower;
						  if (Bot.Class.FindMovementPower(out MovementPower))
						  {
								double lastUsedAbilityMS=MovementPower.LastUsedMilliseconds;
								bool foundMovementPower=false;
								bool checkShrines=false;
								float pointDistance=0f;
								Vector3 vTargetAimPoint=vMoveToTarget;

								switch (MovementPower.Power)
								{
									 case SNOPower.Monk_TempestRush:
										  vTargetAimPoint=MathEx.CalculatePointFrom(vMoveToTarget, vMyCurrentPosition, 10f);
										  Bot.Character.UpdateAnimationState(false, true);
										  Bot.NavigationCache.RefreshMovementCache();

										  bool isHobbling=Bot.Character.CurrentSNOAnim.HasFlag(SNOAnim.Monk_Female_Hobble_Run|SNOAnim.Monk_Male_HTH_Hobble_Run);

										  foundMovementPower=(!bTooMuchZChange&&!Bot.Class.bWaitingForSpecial&&(((!isHobbling||lastUsedAbilityMS>200)&&Bot.Character.dCurrentEnergy>=50)||((isHobbling||lastUsedAbilityMS<400&&Bot.NavigationCache.IsMoving)&&Bot.Character.dCurrentEnergy>15))
												&&!ObjectCache.Obstacles.DoesPositionIntersectAny(vTargetAimPoint, ObstacleType.ServerObject));

										  break;
									 case SNOPower.DemonHunter_Vault:
										  foundMovementPower=(!bTooMuchZChange&&fDistanceFromTarget>=18f&&
																	 (lastUsedAbilityMS>=Bot.SettingsFunky.Class.iDHVaultMovementDelay));


										  pointDistance=35f;
										  if (fDistanceFromTarget>pointDistance)
												vTargetAimPoint=MathEx.CalculatePointFrom(vMoveToTarget, vMyCurrentPosition, pointDistance);



										  break;

									 case SNOPower.Barbarian_FuriousCharge:
									 case SNOPower.Barbarian_Leap:
									 case SNOPower.Wizard_Archon_Teleport:
									 case SNOPower.Wizard_Teleport:
										  foundMovementPower=(!bTooMuchZChange&&fDistanceFromTarget>20f);



										  checkShrines=true;
										  pointDistance=35f;
										  if (fDistanceFromTarget>pointDistance)
												vTargetAimPoint=MathEx.CalculatePointFrom(vMoveToTarget, vMyCurrentPosition, pointDistance);

										  break;
								}

								if (foundMovementPower&&(!checkShrines||!ShrinesInArea(vTargetAimPoint)))
								{

									 if ((MovementPower.Power==SNOPower.Monk_TempestRush&&lastUsedAbilityMS<250)||
										  Navigation.CanRayCast(vMyCurrentPosition, vTargetAimPoint))
									 {
										  ZetaDia.Me.UsePower(MovementPower.Power, vTargetAimPoint, Bot.Character.iCurrentWorldID, -1);
										  MovementPower.LastUsed=DateTime.Now;
										  return;
									 }
								}
						  }
					 } // Allowed to use movement powers to move out-of-combat? 
					 #endregion


					 //Send Movement Command!
					 ZetaDia.Me.Movement.MoveActor(vMoveToTarget);
				}

			

				internal static MoveResult NavigateTo(Vector3 moveTarget, string destinationName="")
				{
					 Vector3 MyPos=ZetaDia.Me.Position;

					 float distanceToTarget=moveTarget.Distance2D(ZetaDia.Me.Position);

					 bool MoveTargetIsInLoS=distanceToTarget<=90f&&!Navigator.Raycast(MyPos, moveTarget);

					 if (distanceToTarget<=5f||MoveTargetIsInLoS)
					 {
						  //Special cache for skipping locations visited.
						  if (Bot.SettingsFunky.Debug.SkipAhead)
								SkipAheadCache.RecordSkipAheadCachePoint();

						  Navigator.PlayerMover.MoveTowards(moveTarget);
						  return MoveResult.Moved;
					 }

					 return Navigator.MoveTo(moveTarget, destinationName, true);
				}
		  }


	 }
}