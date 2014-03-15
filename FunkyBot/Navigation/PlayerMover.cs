using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using FunkyBot.Cache.Objects;
using FunkyBot.DBHandlers;
using FunkyBot.Player.HotBar.Skills;
using FunkyBot.Cache;
using FunkyBot.Movement;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;


namespace FunkyBot
{
	public partial class Funky
	{


		// **********************************************************************************************
		// *****                             Player Mover Class                                     *****
		// **********************************************************************************************
		internal class PlayerMover : IPlayerMover
		{

			public void MoveStop()
			{
				ZetaDia.Me.UsePower(SNOPower.Walk, ZetaDia.Me.Position, Bot.Character.Data.iCurrentWorldID);
			}
			// Anti-stuck variables
			private static Vector3 vOldMoveToTarget = Vector3.Zero;
			internal static int iTimesReachedStuckPoint = 0;
			internal static int iTotalAntiStuckAttempts = 1;
			internal static Vector3 vSafeMovementLocation = Vector3.Zero;
			internal static DateTime timeLastRecordedPosition = DateTime.Today;
			internal static Vector3 vOldPosition = Vector3.Zero;
			internal static DateTime timeStartedUnstuckMeasure = DateTime.Today;
			internal static int iTimesReachedMaxUnstucks = 0;
			internal static DateTime timeCancelledUnstuckerFor = DateTime.Today;
			internal static DateTime timeLastReportedAnyStuck = DateTime.Today;
			internal static int iCancelUnstuckerForSeconds = 60;
			internal static DateTime timeLastRestartedGame = DateTime.Today;
			internal static bool ShouldHandleObstacleObject = false;
			// **********************************************************************************************
			// *****                         Check if we are stuck or not                               *****
			// **********************************************************************************************
			// Simply checks for position changing max once every 3 seconds, to decide on stuck
			public static bool UnstuckChecker(Vector3 vMyCurrentPosition)
			{
				// Keep checking distance changes every 3 seconds
				if (DateTime.Now.Subtract(timeLastRecordedPosition).TotalMilliseconds >= 3500) //herbfunk: added 500ms
				{
					timeLastRecordedPosition = DateTime.Now;
					if (vOldPosition != Vector3.Zero && vOldPosition.Distance(vMyCurrentPosition) <= 4f)
					{
						return true;
					}
					vOldPosition = vMyCurrentPosition;
				}

				return false;
			}

			// **********************************************************************************************
			// *****                Actually deal with a stuck - find an unstuck point etc.             *****
			// **********************************************************************************************
			public static Vector3 UnstuckHandler(Vector3 vMyCurrentPosition, Vector3 vOriginalDestination)
			{
				// Update the last time we generated a path
				timeStartedUnstuckMeasure = DateTime.Now;



				// If we got stuck on a 2nd/3rd/4th "chained" anti-stuck route, then return the old move to target to keep movement of some kind going
				if (iTimesReachedStuckPoint > 0)
				{
					vSafeMovementLocation = Vector3.Zero;
					iTimesReachedStuckPoint++;
					// Reset the path and allow a whole "New" unstuck generation next cycle
					iTimesReachedStuckPoint = 0;
					// And cancel unstucking for 9 seconds so DB can try to navigate
					iCancelUnstuckerForSeconds = (9 * iTotalAntiStuckAttempts);
					if (iCancelUnstuckerForSeconds < 20)
						iCancelUnstuckerForSeconds = 20;
					timeCancelledUnstuckerFor = DateTime.Now;
					Navigator.Clear();
					Logger.DBLog.DebugFormat("[Funky] Clearing old route and trying new path find to: " + vOldMoveToTarget.ToString());
					Navigator.MoveTo(vOldMoveToTarget, "original destination", false);
					return vSafeMovementLocation;
				}

				// Only try an unstuck 10 times maximum in XXX period of time
				if (Vector3.Distance(vOriginalDestination, vMyCurrentPosition) >= 700f)
				{
					Logger.DBLog.InfoFormat("[Funky] You are " + Vector3.Distance(vOriginalDestination, vMyCurrentPosition).ToString(CultureInfo.InvariantCulture) + " distance away from your destination.");
					Logger.DBLog.InfoFormat("[Funky] This is too far for the unstucker, and is likely a sign of ending up in the wrong map zone.");
					Logger.DBLog.InfoFormat("Reloading current profile");
					ProfileManager.Load(ProfileManager.CurrentProfile.Path);

				}
				if (iTotalAntiStuckAttempts <= 8)
				{
					//Check cache for barricades..
					if (ObjectCache.Objects.OfType<CacheDestructable>().Any(CO => CO.RadiusDistance <= 12f))
					{
						Logger.DBLog.InfoFormat("[Funky] Found nearby barricade, flagging barricade destruction!");
						ShouldHandleObstacleObject = true;
					}

					Logger.DBLog.InfoFormat("[Funky] Your bot got stuck! Trying to unstuck (attempt #" + iTotalAntiStuckAttempts.ToString(CultureInfo.InvariantCulture) + " of 8 attempts)");
					Logger.DBLog.DebugFormat("(destination=" + vOriginalDestination.ToString() + ", which is " + Vector3.Distance(vOriginalDestination, vMyCurrentPosition).ToString(CultureInfo.InvariantCulture) + " distance away)");

					Logger.Write(LogLevel.Movement, "Stuck Flags: {0}", Bot.NavigationCache.Stuckflags.ToString());

					bool FoundRandomMovementLocation = Bot.NavigationCache.AttemptFindSafeSpot(out vSafeMovementLocation, Vector3.Zero, Bot.Settings.Plugin.AvoidanceFlags);

					// Temporarily log stuff
					if (iTotalAntiStuckAttempts == 1 && Bot.Settings.Debug.LogStuckLocations)
					{
						string sLogFileName = Logger.LoggingPrefixString + " -- Stucks.log";
						FileStream LogStream = File.Open(Logger.LoggingFolderPath + sLogFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
						using (StreamWriter LogWriter = new StreamWriter(LogStream))
						{
							LogWriter.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + ": Original Destination=" + vOldMoveToTarget.ToString() + ". Current player position when stuck=" + vMyCurrentPosition.ToString());
							LogWriter.WriteLine("Profile Name=" + ProfileManager.CurrentProfile.Name);
						}
					}

					if (iTotalAntiStuckAttempts > 1)
						Navigator.Clear();

					// Now count up our stuck attempt generations
					iTotalAntiStuckAttempts++;

					if (FoundRandomMovementLocation)
					{
						return vSafeMovementLocation;
					}
					Navigator.Clear();
					Navigator.MoveTo(vOriginalDestination, "original destination", false);
					iCancelUnstuckerForSeconds = 40;
					timeCancelledUnstuckerFor = DateTime.Now;
					return Vector3.Zero;
				}

				iTimesReachedMaxUnstucks++;
				iTotalAntiStuckAttempts = 1;
				vSafeMovementLocation = Vector3.Zero;
				vOldPosition = Vector3.Zero;
				iTimesReachedStuckPoint = 0;
				timeLastRecordedPosition = DateTime.Today;
				timeStartedUnstuckMeasure = DateTime.Today;
				int iSafetyLoops = 0;

				if (iTimesReachedMaxUnstucks == 1)
				{
					Navigator.Clear();

					Logger.DBLog.InfoFormat("[Funky] Anti-stuck measures now attempting to kickstart DB's path-finder into action.");
					if (Vector3.Distance(vOriginalDestination, vMyCurrentPosition) >= 200f)
					{
						iTimesReachedMaxUnstucks++;
					}
					else
					{

						Navigator.MoveTo(vOriginalDestination, "original destination", false);
						iCancelUnstuckerForSeconds = 40;
						timeCancelledUnstuckerFor = DateTime.Now;
						return vSafeMovementLocation;
					}
				}

				if (iTimesReachedMaxUnstucks == 2)
				{
					Logger.DBLog.InfoFormat("[Funky] Anti-stuck measures failed. Now attempting to reload current profile.");
					ExitGame.ShouldExitGame = true;
				}
				// Exit the game and reload the profile
				if (Bot.Settings.Debug.RestartGameOnLongStucks && DateTime.Now.Subtract(timeLastRestartedGame).TotalMinutes >= 15)
				{
					timeLastRestartedGame = DateTime.Now;
					ExitGame.ShouldExitGame = true;
				}
				else
				{
					Logger.DBLog.InfoFormat("[Funky] Unstucking measures failed. Now stopping Trinity unstucker for 12 minutes to inactivity timers to kick in or DB to auto-fix.");
					iCancelUnstuckerForSeconds = 720;
					timeCancelledUnstuckerFor = DateTime.Now;
					return vSafeMovementLocation;
				}
				return vSafeMovementLocation;
			}


			internal static bool ShouldRestartGame = false;

			// **********************************************************************************************
			// *****               Handle moveto requests from the current routine/profile              *****
			// **********************************************************************************************
			// This replaces DemonBuddy's own built-in "Basic movement handler" with a custom one
			internal static Vector3 vLastMoveTo = Vector3.Zero;
			private static bool bLastWaypointWasTown;
			private static HashSet<Vector3> hashDoneThisVector = new HashSet<Vector3>();
			internal static DateTime LastCombatPointChecked = DateTime.Today;

			public void MoveTowards(Vector3 vMoveToTarget)
			{
				if (Bot.Character.Class == null)
				{
					Logger.DBLog.InfoFormat("Bot did not properly initilize, stopping bot!");
					BotMain.Stop(false, "Bot Init Failure");
					return;
				}

				#region LogStucks
				// The below code is to help profile/routine makers avoid waypoints with a long distance between them.
				// Long-distances between waypoints is bad - it increases stucks, and forces the DB nav-server to be called.

				//if (vLastMoveTo==Vector3.Zero) vLastMoveTo=vMoveToTarget;

				if (vMoveToTarget != vLastMoveTo)
				{
					vLastMoveTo = vMoveToTarget;

					if (Bot.Settings.Debug.LogStuckLocations)
					{
						vLastMoveTo = vMoveToTarget;
						bLastWaypointWasTown = false;

						float fDistance = Vector3.Distance(vMoveToTarget, vLastMoveTo);
						// Logger.DBLog.InfoFormat if not in town, last waypoint wasn't FROM town, and the distance is >200 but <2000 (cos 2000+ probably means we changed map zones!)
						if (!ZetaDia.IsInTown && !bLastWaypointWasTown && fDistance >= 200 & fDistance <= 2000)
						{
							if (!hashDoneThisVector.Contains(vMoveToTarget))
							{
								// Logger.DBLog.InfoFormat it
								string sLogFileName = Logger.LoggingPrefixString + " -- LongPaths.log";
								FileStream LogStream = File.Open(Logger.LoggingFolderPath + sLogFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
								using (StreamWriter LogWriter = new StreamWriter(LogStream))
								{
									LogWriter.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + ":");
									LogWriter.WriteLine("Profile Name=" + ProfileManager.CurrentProfile.Name);
									LogWriter.WriteLine("'From' Waypoint=" + vLastMoveTo + ". 'To' Waypoint=" + vMoveToTarget + ". Distance=" + fDistance.ToString(CultureInfo.InvariantCulture));
								}
								//LogStream.Close();
								hashDoneThisVector.Add(vMoveToTarget);
							}
						}
					}

					if (ZetaDia.IsInTown)
						bLastWaypointWasTown = true;
				}

				#endregion



				// Make sure GilesTrinity doesn't want us to avoid routine-movement
				if (Bot.Targeting.DontMove)
					return;

				// Store player current position
				Vector3 vMyCurrentPosition = ZetaDia.Me.Position;

				//Check GPC entry (backtracking cache) -- only when not in town!
				if (BackTrackCache.EnableBacktrackGPRcache && !ZetaDia.IsInTown)
				{
					if (DateTime.Now.Subtract(LastCombatPointChecked).TotalMilliseconds > 1250)
					{
						if (BackTrackCache.cacheMovementGPRs.Count == 0)
							BackTrackCache.StartNewGPR(vMyCurrentPosition);
						else
						{//Add new point only if distance is 25f difference
							if (BackTrackCache.cacheMovementGPRs.Count == 1)
							{
								if (BackTrackCache.cacheMovementGPRs[0].CreationVector.Distance(vMyCurrentPosition) >= BackTrackCache.MinimumRangeBetweenMovementGPRs)
								{
									BackTrackCache.StartNewGPR(vMyCurrentPosition);
								}
							}
							else
							{
								//Only if no GPCs currently are less than 20f from us..
								if (!BackTrackCache.cacheMovementGPRs.Any(GPC => GPC.CreationVector.Distance(vMyCurrentPosition) < BackTrackCache.MinimumRangeBetweenMovementGPRs))
								{
									BackTrackCache.StartNewGPR(vMyCurrentPosition);
								}
							}
						}
						//Reset Timer
						LastCombatPointChecked = DateTime.Now;
					}
				}

				//Special cache for skipping locations visited.
				if (Bot.Settings.Debug.SkipAhead) SkipAheadCache.RecordSkipAheadCachePoint();

				// Store distance to current moveto target
				float fDistanceFromTarget;

				#region Unstucker
				// Do unstuckery things
				if (Bot.Settings.Debug.EnableUnstucker)
				{
					// Store the "real" (not anti-stuck) destination
					vOldMoveToTarget = vMoveToTarget;

					// See if we can reset the 10-limit unstuck counter, if >120 seconds since we last generated an unstuck location
					if (iTotalAntiStuckAttempts > 1 && DateTime.Now.Subtract(timeStartedUnstuckMeasure).TotalSeconds >= 120)
					{
						iTotalAntiStuckAttempts = 1;
						iTimesReachedStuckPoint = 0;
						vSafeMovementLocation = Vector3.Zero;
					}

					// See if we need to, and can, generate unstuck actions
					if (DateTime.Now.Subtract(timeCancelledUnstuckerFor).TotalSeconds > iCancelUnstuckerForSeconds && UnstuckChecker(vMyCurrentPosition))
					{
						// Record the time we last apparently couldn't move for a brief period of time
						timeLastReportedAnyStuck = DateTime.Now;
						// See if there's any stuck position to try and navigate to generated by random mover
						vSafeMovementLocation = UnstuckHandler(vMyCurrentPosition, vOldMoveToTarget);
						if (vSafeMovementLocation == Vector3.Zero)
							return;
					}

					// See if we can clear the total unstuckattempts if we haven't been stuck in over 6 minutes.
					if (DateTime.Now.Subtract(timeLastReportedAnyStuck).TotalSeconds >= 360)
					{
						iTimesReachedMaxUnstucks = 0;
					}

					// Did we have a safe point already generated (eg from last loop through), if so use it as our current location instead
					if (vSafeMovementLocation != Vector3.Zero)
					{
						// Set our current movement target to the safe point we generated last cycle
						vMoveToTarget = vSafeMovementLocation;
					}

					// Get distance to current destination
					fDistanceFromTarget = Vector3.Distance(vMyCurrentPosition, vMoveToTarget);

					// Remove the stuck position if it's been reached, this bit of code also creates multiple stuck-patterns in an ever increasing amount
					if (vSafeMovementLocation != Vector3.Zero && fDistanceFromTarget <= 3f)
					{
						vSafeMovementLocation = Vector3.Zero;
						iTimesReachedStuckPoint++;
						// Do we want to immediately generate a 2nd waypoint to "chain" anti-stucks in an ever-increasing path-length?
						if (iTimesReachedStuckPoint <= iTotalAntiStuckAttempts)
						{
							Bot.NavigationCache.AttemptFindSafeSpot(out vSafeMovementLocation, Vector3.Zero, Bot.Settings.Plugin.AvoidanceFlags);
							vMoveToTarget = vSafeMovementLocation;
						}
						else
						{
							Logger.DBLog.DebugFormat("[Funky] Clearing old route and trying new path find to: " + vOldMoveToTarget.ToString());
							// Reset the path and allow a whole "New" unstuck generation next cycle
							iTimesReachedStuckPoint = 0;
							// And cancel unstucking for 9 seconds so DB can try to navigate
							iCancelUnstuckerForSeconds = (9 * iTotalAntiStuckAttempts);
							if (iCancelUnstuckerForSeconds < 20)
								iCancelUnstuckerForSeconds = 20;
							timeCancelledUnstuckerFor = DateTime.Now;
							Navigator.Clear();
							Navigator.MoveTo(vOldMoveToTarget, "original destination", false);
							return;
						}
					}
				}
				else
				{
					// Get distance to current destination
					fDistanceFromTarget = vMyCurrentPosition.Distance(vMoveToTarget);
				} // Is the built-in unstucker enabled or not? 
				#endregion


				//Prioritize "blocking" objects.
				if (!Bot.Character.Data.bIsInTown) Bot.NavigationCache.ObstaclePrioritizeCheck();





				#region MovementAbilities
				// See if we can use abilities like leap etc. for movement out of combat, but not in town and only if we can raycast.
				if (Bot.Settings.OutOfCombatMovement && !ZetaDia.IsInTown && !Bot.IsInNonCombatBehavior)
				{
					Skill MovementPower;
					Vector3 MovementVector = Bot.Character.Class.FindOutOfCombatMovementPower(out MovementPower, vMoveToTarget);
					if (MovementVector != Vector3.Zero)
					{
						ZetaDia.Me.UsePower(MovementPower.Power, MovementVector, Bot.Character.Data.iCurrentWorldID);
						MovementPower.OnSuccessfullyUsed();
						return;
					}
				} // Allowed to use movement powers to move out-of-combat? 
				#endregion


				//Send Movement Command!
				//ZetaDia.Me.Movement.MoveActor(vMoveToTarget);
				ZetaDia.Me.UsePower(SNOPower.Walk, vMoveToTarget, Bot.Character.Data.iCurrentWorldID);
			}



			internal static MoveResult NavigateTo(Vector3 moveTarget, string destinationName = "")
			{
				return Navigator.MoveTo(moveTarget, destinationName);
			}
		}


	}
}