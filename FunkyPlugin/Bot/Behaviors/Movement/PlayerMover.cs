﻿using System;
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


namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  // When we last FOUND a safe spot
		  private static DateTime lastFoundSafeSpot=DateTime.Today;
		  private static Vector3 vlastSafeSpot=Vector3.Zero;

		  // **********************************************************************************************
		  // *****                             Player Mover Class                                     *****
		  // **********************************************************************************************
		  public class PlayerMover : IPlayerMover
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
					 ZetaDia.Me.UsePower(SNOPower.Walk, ZetaDia.Me.Position, Funky.Bot.Character.iCurrentWorldID, -1);
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
						  Logging.Write("[Funky] Your bot got stuck! Trying to unstuck (attempt #"+iTotalAntiStuckAttempts.ToString()+" of 8 attempts)");
						  Logging.WriteDiagnostic("(destination="+vOriginalDestination.ToString()+", which is "+Vector3.Distance(vOriginalDestination, vMyCurrentPosition).ToString()+" distance away)");
						  vSafeMovementLocation=Funky.FindSafeZone(true, iTotalAntiStuckAttempts, Vector3.Zero);

						  // Temporarily log stuff
						  if (iTotalAntiStuckAttempts==1&&Funky.SettingsFunky.LogStuckLocations)
						  {
								FileStream LogStream=File.Open(Funky.FolderPaths.sTrinityLogPath+ZetaDia.Service.CurrentHero.BattleTagName+" - Stucks - "+ZetaDia.Actors.Me.ActorClass.ToString()+".log", FileMode.Append, FileAccess.Write, FileShare.Read);
								using (StreamWriter LogWriter=new StreamWriter(LogStream))
								{
									 LogWriter.WriteLine(DateTime.Now.ToString()+": Original Destination="+vOldMoveToTarget.ToString()+". Current player position when stuck="+vMyCurrentPosition.ToString());
									 LogWriter.WriteLine("Profile Name="+ProfileManager.CurrentProfile.Name);
								}
								//LogStream.Close();
						  }

						  //Check cache for barricades..
						  if (Funky.ObjectCache.Objects.OfType<CacheGizmo>()
								.Where(CO => CO.IsBarricade.HasValue&&CO.IsBarricade.Value&&CO.RadiusDistance<=10f).Any())
						  {
								Logging.Write("[Funky] Found nearby barricade, flagging barricade destruction!");
								ShouldHandleObstacleObject=true;
						  }

						  // Now count up our stuck attempt generations
						  iTotalAntiStuckAttempts++;
						  return vSafeMovementLocation;
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
								Funky.WaitWhileAnimating(5, true);
								ZetaDia.Me.UsePower(SNOPower.UseStoneOfRecall, ZetaDia.Me.Position, ZetaDia.Me.WorldDynamicId, -1);
								Thread.Sleep(1000);
								Funky.WaitWhileAnimating(1000, true);
								if (iSafetyLoops>5)
									 break;
						  }
						  Thread.Sleep(1000);
						  // As long as we successfully reached town, reload the profile
						  if (ZetaDia.Me.IsInTown)
						  {

								string profile=Funky.sFirstProfileSeen;

								if (!string.IsNullOrEmpty(profile))
								{
									 ProfileManager.Load(profile);

									 Logging.Write("[Funky] Exiting game to continue with next profile.");
									 // Attempt to teleport to town first for a quicker exit
									 iSafetyLoops=0;
									 while (!ZetaDia.Me.IsInTown)
									 {
										  iSafetyLoops++;
										  Funky.WaitWhileAnimating(5, true);
										  ZetaDia.Me.UsePower(SNOPower.UseStoneOfRecall, ZetaDia.Me.Position, ZetaDia.Me.WorldDynamicId, -1);
										  Thread.Sleep(1000);
										  Funky.WaitWhileAnimating(1000, true);
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
					 if (Funky.SettingsFunky.RestartGameOnLongStucks&&DateTime.Now.Subtract(timeLastRestartedGame).TotalMinutes>=15)
					 {
						  HadDisconnectError=true;
						  timeLastRestartedGame=DateTime.Now;
						  string sUseProfile=Funky.sFirstProfileSeen;
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

					 if (vLastMoveTo==Vector3.Zero)
						  vLastMoveTo=vMoveToTarget;

					 if (vMoveToTarget!=vLastMoveTo)
					 {
						  if (Funky.SettingsFunky.LogStuckLocations)
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
										  FileStream LogStream=File.Open(Funky.FolderPaths.sTrinityLogPath+ZetaDia.Service.CurrentHero.BattleTagName+" - LongPaths - "+ZetaDia.Actors.Me.ActorClass.ToString()+".log", FileMode.Append, FileAccess.Write, FileShare.Read);
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
					 if (Funky.Bot.Combat.DontMove)
						  return;

					 // Store player current position
					 Vector3 vMyCurrentPosition=ZetaDia.Me.Position;
					 if (navigation.CurrentPath.Count>0&&CurrentMovementPosition!=navigation.CurrentPath.Current)
					 {
						  CurrentMovementPosition=navigation.CurrentPath.Current;
					 }

					 //Check GPC entry (backtracking cache) -- only when not in town!
					 if (GridPointAreaCache.EnableBacktrackGPRcache&&!ZetaDia.Me.IsInTown)
					 {
						  if (DateTime.Now.Subtract(LastCombatPointChecked).TotalMilliseconds>1250)
						  {
								if (GridPointAreaCache.cacheMovementGPRs.Count==0)
									 GridPointAreaCache.StartNewGPR(vMyCurrentPosition);
								else
								{//Add new point only if distance is 25f difference
									 if (GridPointAreaCache.cacheMovementGPRs.Count==1)
									 {
										  if (GridPointAreaCache.cacheMovementGPRs[0].CreationVector.Distance(vMyCurrentPosition)>=GridPointAreaCache.MinimumRangeBetweenMovementGPRs)
										  {
												GridPointAreaCache.StartNewGPR(vMyCurrentPosition);
										  }
									 }
									 else
									 {
										  //Only if no GPCs currently are less than 20f from us..
										  if (!GridPointAreaCache.cacheMovementGPRs.Any(GPC => GPC.CreationVector.Distance(vMyCurrentPosition)<GridPointAreaCache.MinimumRangeBetweenMovementGPRs))
										  {
												GridPointAreaCache.StartNewGPR(vMyCurrentPosition);
										  }
									 }
								}
								//Reset Timer
								LastCombatPointChecked=DateTime.Now;
						  }
					 }

					 //Special cache for skipping locations visited.
					 if (CacheMovementTracking.bSkipAheadAGo)
						  CacheMovementTracking.RecordSkipAheadCachePoint();

					 // Store distance to current moveto target
					 float fDistanceFromTarget;

					 #region Unstucker
					 // Do unstuckery things
					 if (Funky.SettingsFunky.EnableUnstucker)
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
									 //Trinity.Bot.Character.vCurrentPosition=vMyCurrentPosition;
									 vSafeMovementLocation=Funky.FindSafeZone(true, iTotalAntiStuckAttempts, Vector3.Zero);
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

					 #region ObstacleCheck

					 if (vShiftedPosition==vNullLocation)
					 {
						  Vector3 obstacleV3;
						  // See if there's an obstacle in our way, if so try to navigate around it
						  if (CurrentMovementPosition!=vNullLocation
								&&ObstacleCheck(out obstacleV3, CurrentMovementPosition))
						  {

								lastShiftedPosition=DateTime.Now;
								iShiftPositionFor=2500;
								vShiftedPosition=obstacleV3;

								if (vShiftedPosition!=vNullLocation)
								{
									 Logging.WriteDiagnostic("Using altered navigation vector {0} to bypass obstacle", vShiftedPosition.ToString());
									 vMoveToTarget=vShiftedPosition;
								}
						  }
					 }
					 else
					 {
						  if (Bot.Character.Position.Distance2D(vShiftedPosition)<=2.5f||DateTime.Now.Subtract(lastShiftedPosition).TotalMilliseconds>iShiftPositionFor)
								vShiftedPosition=vNullLocation;
						  else
								vMoveToTarget=vShiftedPosition;
					 }
					 #endregion

					 #region MovementAbilities
					 // See if we can use abilities like leap etc. for movement out of combat, but not in town and only if we can raycast.
					 if (Funky.SettingsFunky.OutOfCombatMovement&&!ZetaDia.Me.IsInTown)
					 //&&)
					 {

						  bool bTooMuchZChange=((vMyCurrentPosition.Z-vMoveToTarget.Z)>=4f);

						  SNOPower MovementPower;
						  if (Bot.Class.FindSpecialMovementPower(out MovementPower))
						  {
								double lastUsedAbilityMS=AbilityLastUseMS(MovementPower);
								bool foundMovementPower=false;
								bool checkShrines=false;
								float pointDistance=0f;
								Vector3 vTargetAimPoint=vMoveToTarget;

								switch (MovementPower)
								{
									 case SNOPower.Monk_TempestRush:
										  vTargetAimPoint=MathEx.CalculatePointFrom(vMoveToTarget, vMyCurrentPosition, 10f);
										  Bot.Character.UpdateAnimationState(false, true);
										  Bot.Character.UpdateMovementData();

										  bool isHobbling=Bot.Character.CurrentSNOAnim.HasFlag(SNOAnim.Monk_Female_Hobble_Run|SNOAnim.Monk_Male_HTH_Hobble_Run);

										  foundMovementPower=(!bTooMuchZChange&&(((!isHobbling||lastUsedAbilityMS>200)&&Bot.Character.dCurrentEnergy>=50)||((isHobbling||lastUsedAbilityMS<400&&Bot.Character.isMoving)&&Bot.Character.dCurrentEnergy>15))
												&&!ObjectCache.Obstacles.DoesPositionIntersectAny(vTargetAimPoint, ObstacleType.ServerObject));

										  break;
									 case SNOPower.DemonHunter_Vault:
										  foundMovementPower=(!bTooMuchZChange&&fDistanceFromTarget>=18f&&
																	 (lastUsedAbilityMS>=Funky.SettingsFunky.Class.iDHVaultMovementDelay));


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

									 if ((MovementPower==SNOPower.Monk_TempestRush&&lastUsedAbilityMS<250)||
										  GilesCanRayCast(vMyCurrentPosition, vTargetAimPoint))
									 {
										  ZetaDia.Me.UsePower(MovementPower, vTargetAimPoint, Funky.Bot.Character.iCurrentWorldID, -1);
										  Funky.dictAbilityLastUse[MovementPower]=DateTime.Now;
										  return;
									 }
								}
						  }
						  #region "OldMovement"
						  /*
						  // Leap movement for a barb
						  if (Funky.HotbarAbilitiesContainsPower(SNOPower.Barbarian_Leap)&&
								DateTime.Now.Subtract(Funky.dictAbilityLastUse[SNOPower.Barbarian_Leap]).TotalMilliseconds>=Funky.dictAbilityRepeatDelay[SNOPower.Barbarian_Leap]&&
								fDistanceFromTarget>=20f&&
								PowerManager.CanCast(SNOPower.Barbarian_Leap)&&!ShrinesInArea(vMoveToTarget))
						  {
								Vector3 vThisTarget=vMoveToTarget;
								if (fDistanceFromTarget>35f)
									 vThisTarget=MathEx.CalculatePointFrom(vMoveToTarget, vMyCurrentPosition, 35f);
								ZetaDia.Me.UsePower(SNOPower.Barbarian_Leap, vThisTarget, Funky.Bot.Character.iCurrentWorldID, -1);
								Funky.dictAbilityLastUse[SNOPower.Barbarian_Leap]=DateTime.Now;
								return;
						  }
						  // Furious Charge movement for a barb
						  if (Funky.HotbarAbilitiesContainsPower(SNOPower.Barbarian_FuriousCharge)&&!bTooMuchZChange&&
								DateTime.Now.Subtract(Funky.dictAbilityLastUse[SNOPower.Barbarian_FuriousCharge]).TotalMilliseconds>=Funky.dictAbilityRepeatDelay[SNOPower.Barbarian_FuriousCharge]&&
								fDistanceFromTarget>=20f&&
								PowerManager.CanCast(SNOPower.Barbarian_FuriousCharge)&&!ShrinesInArea(vMoveToTarget))
						  {
								Vector3 vThisTarget=vMoveToTarget;
								if (fDistanceFromTarget>35f)
									 vThisTarget=MathEx.CalculatePointFrom(vMoveToTarget, vMyCurrentPosition, 35f);
								ZetaDia.Me.UsePower(SNOPower.Barbarian_FuriousCharge, vThisTarget, Funky.Bot.Character.iCurrentWorldID, -1);
								Funky.dictAbilityLastUse[SNOPower.Barbarian_FuriousCharge]=DateTime.Now;
								return;
						  }
						  // Vault for a DH - maximum set by user-defined setting
						  if (Funky.HotbarAbilitiesContainsPower(SNOPower.DemonHunter_Vault)&&!bTooMuchZChange&&
								DateTime.Now.Subtract(Funky.dictAbilityLastUse[SNOPower.DemonHunter_Vault]).TotalMilliseconds>=Funky.SettingsFunky.Class.iDHVaultMovementDelay&&
								fDistanceFromTarget>=18f&&
								PowerManager.CanCast(SNOPower.DemonHunter_Vault)&&!ShrinesInArea(vMoveToTarget))
						  {
								Vector3 vThisTarget=vMoveToTarget;
								if (fDistanceFromTarget>35f)
									 vThisTarget=MathEx.CalculatePointFrom(vMoveToTarget, vMyCurrentPosition, 35f);
								ZetaDia.Me.UsePower(SNOPower.DemonHunter_Vault, vThisTarget, Funky.Bot.Character.iCurrentWorldID, -1);
								Funky.dictAbilityLastUse[SNOPower.DemonHunter_Vault]=DateTime.Now;
								return;
						  }
						  // Tempest rush for a monk
						  if (Funky.HotbarAbilitiesContainsPower(SNOPower.Monk_TempestRush)&&!bTooMuchZChange&&ZetaDia.Me.CurrentPrimaryResource>=20)
						  {
								Vector3 vTargetAimPoint=MathEx.CalculatePointFrom(vMoveToTarget, vMyCurrentPosition, 10f);
								ZetaDia.Me.UsePower(SNOPower.Monk_TempestRush, vTargetAimPoint, Funky.Bot.Character.iCurrentWorldID, -1);
								return;
						  }
						  // Teleport for a wizard (need to be able to check skill rune in DB for a 3-4 teleport spam in a row)
						  if (Funky.HotbarAbilitiesContainsPower(SNOPower.Wizard_Teleport)&&
								DateTime.Now.Subtract(Funky.dictAbilityLastUse[SNOPower.Wizard_Teleport]).TotalMilliseconds>=Funky.dictAbilityRepeatDelay[SNOPower.Wizard_Teleport]&&
								fDistanceFromTarget>=20f&&
								PowerManager.CanCast(SNOPower.Wizard_Teleport)&&!ShrinesInArea(vMoveToTarget))
						  {
								Vector3 vThisTarget=vMoveToTarget;
								if (fDistanceFromTarget>35f)
									 vThisTarget=MathEx.CalculatePointFrom(vMoveToTarget, vMyCurrentPosition, 35f);
								ZetaDia.Me.UsePower(SNOPower.Wizard_Teleport, vThisTarget, Funky.Bot.Character.iCurrentWorldID, -1);
								Funky.dictAbilityLastUse[SNOPower.Wizard_Teleport]=DateTime.Now;
								return;
						  }
						  // Archon Teleport for a wizard 
						  if (Funky.HotbarAbilitiesContainsPower(SNOPower.Wizard_Archon_Teleport)&&
								DateTime.Now.Subtract(Funky.dictAbilityLastUse[SNOPower.Wizard_Archon_Teleport]).TotalMilliseconds>=Funky.dictAbilityRepeatDelay[SNOPower.Wizard_Archon_Teleport]&&
								fDistanceFromTarget>=20f&&
								PowerManager.CanCast(SNOPower.Wizard_Archon_Teleport)&&!ShrinesInArea(vMoveToTarget))
						  {
								Vector3 vThisTarget=vMoveToTarget;
								if (fDistanceFromTarget>35f)
									 vThisTarget=MathEx.CalculatePointFrom(vMoveToTarget, vMyCurrentPosition, 35f);
								ZetaDia.Me.UsePower(SNOPower.Wizard_Archon_Teleport, vThisTarget, Funky.Bot.Character.iCurrentWorldID, -1);
								Funky.dictAbilityLastUse[SNOPower.Wizard_Archon_Teleport]=DateTime.Now;
								return;
						  }
						  
						  */
						  #endregion

					 } // Allowed to use movement powers to move out-of-combat? 
					 #endregion



					 ZetaDia.Me.Movement.MoveActor(vMoveToTarget);
				}

				internal static MoveResult NavigateTo(Vector3 moveTarget, string destinationName="")
				{
					 Vector3 MyPos=ZetaDia.Me.Position;

					 float distanceToTarget=moveTarget.Distance2D(ZetaDia.Me.Position);

					 bool MoveTargetIsInLoS=distanceToTarget<=90f&&!Navigator.Raycast(MyPos, moveTarget);

					 if (distanceToTarget<=5f||MoveTargetIsInLoS)
					 {
						  Navigator.PlayerMover.MoveTowards(moveTarget);
						  return MoveResult.Moved;
					 }

					 return Navigator.MoveTo(moveTarget, destinationName, true);
				}
		  }


	 }
}