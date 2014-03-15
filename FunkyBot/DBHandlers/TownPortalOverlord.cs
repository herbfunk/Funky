using System;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Action = Zeta.TreeSharp.Action;

namespace FunkyBot.DBHandlers
{
	public static class TownPortalBehavior
	{
		internal static bool SafetyCheckForTownRun()
		{
			//This is called only if we want to townrun... basically a pre-check to if we should proceede.
			Logger.Write(LogLevel.OutOfCombat, "Precheck running for town run");

			//Avoidance Flag
			Bot.Character.Data.CriticalAvoidance = true;

			if (Bot.Targeting.ShouldRefreshObjectList)
			{
				Bot.Targeting.RefreshDiaObjects();
				// Check for death / player being dead
				if (Bot.Character.Data.dCurrentHealthPct <= 0)
				{
					return false;
				}
			}

			//Checks
			if (Bot.Targeting.CurrentTarget != null)
			{
				return false;
			}

			//All checks passed.. so continue behavior!
			return true;
		}

		internal static DateTime FunkyTP_LastCastAttempt = DateTime.MinValue;
		internal static Vector3 StartingPosition = Vector3.Zero;
		internal static bool MovementOccured = false;

		public static bool FunkyTPBehaviorFlag = false;

		public static bool FunkyTPOverlord(object ret)
		{
			//Ingame and not dead?
			if (TPActionIsValid())
			{
				//If not already in town, check if we can cast..
				if (!ZetaDia.IsInTown)
				{
					return CanCastTP();
				}
			}

			//No reason to run behavior..
			return false;
		}

		internal static bool TPActionIsValid()
		{
			try
			{
				if (!ZetaDia.IsInGame || ZetaDia.Me.IsDead)
					return false;
				return true;
			}
			catch (Exception)
			{
				return false;
			}

		}

		internal static bool CanCastTP()
		{
			string TPcastTest;
			bool cancast;
			try
			{
				cancast = ZetaDia.Me.CanUseTownPortal(out TPcastTest);
			}
			catch (NullReferenceException)
			{
				TPcastTest = "Exception during CanUseTownPortal";
				cancast = false;
			}

			if (!cancast)
			{
				Logger.Write(LogLevel.OutOfCombat, "Cannot cast TP: " + TPcastTest);
				return false;
			}
			return true;
		}

		public static void ResetTPBehavior()
		{
			worldtransferStarted = false;
			FunkyTPBehaviorFlag = false;
			worldChanged = false;
			initizedTPBehavior = false;
			FunkyTP_LastCastAttempt = DateTime.MinValue;
			//GameEvents.OnWorldChanged-=OnWorldChanged;
			//GameEvents.OnWorldTransferStart-=OnWorldChangeStart;
			StartingPosition = Vector3.Zero;
		}

		internal static bool initizedTPBehavior = false;
		internal static void InitTPBehavior()
		{
			Logger.Write(LogLevel.OutOfCombat, "Starting TP Behavior", true);
			worldtransferStarted = false;
			worldChanged = false;
			//GameEvents.OnWorldChanged+=OnWorldChanged;
			//GameEvents.OnWorldTransferStart+=OnWorldChangeStart;
			initizedTPBehavior = true;
			StartingPosition = Bot.Character.Data.Position;
			MovementOccured = false;
		}

		//Events below are not used ATM.
		internal static bool worldtransferStarted = false;
		internal static bool worldChanged = false;

		internal static bool CastingRecall()
		{
			if (Bot.Character.Data.CurrentAnimationState != AnimationState.Idle)
			{
				switch (Bot.Character.Data.CurrentSNOAnim)
				{
					case SNOAnim.barbarian_male_HTH_Recall_Channel_01:
					case SNOAnim.Barbarian_Female_HTH_Recall_Channel_01:
					case SNOAnim.Monk_Male_recall_channel:
					case SNOAnim.Monk_Female_recall_channel:
					case SNOAnim.WitchDoctor_Male_recall_channel:
					case SNOAnim.WitchDoctor_Female_recall_channel:
					case SNOAnim.Wizard_Male_HTH_recall_channel:
					case SNOAnim.Wizard_Female_HTH_recall_channel:
					case SNOAnim.Demonhunter_Male_HTH_recall_channel:
					case SNOAnim.Demonhunter_Female_HTH_recall_channel:
						return true;
				}
			}
			return false;
		}

		public static Composite FunkyTownPortal()
		{
			CanRunDecoratorDelegate canRunDelegateReturnToTown = FunkyTPOverlord;
			ActionDelegate actionDelegateReturnTown = FunkyTPBehavior;
			Sequence sequenceReturnTown = new Sequence(
				new Action(actionDelegateReturnTown)
				);
			return new Decorator(canRunDelegateReturnToTown, sequenceReturnTown);
		}
		internal static bool CastAttempted = false;
		public static RunStatus FunkyTPBehavior(object ret)
		{
			//Init
			if (!initizedTPBehavior)
			{
				InitTPBehavior();
				return RunStatus.Running;
			}

			double ElapsedTime = DateTime.Now.Subtract(FunkyTP_LastCastAttempt).TotalSeconds;

			//Check world transfer start
			if (worldtransferStarted)
			{
				if (ElapsedTime < 10 || worldChanged)
				{
					//Logger.Write(LogLevel.OutOfCombat,"Waiting for world change!");

					if (!ZetaDia.IsInTown)
						return RunStatus.Running;
					Logger.Write(LogLevel.OutOfCombat, "Casting Behavior Finished, we are in town!", true);
					ResetTPBehavior();
					//UpdateSearchGridProvider(true);
					return RunStatus.Success;
				}
				if (ElapsedTime >= 10 && !ZetaDia.IsInTown)
				{
					//Retry?
					worldtransferStarted = false;
					CastAttempted = false;
					Vector3 UnstuckPos;
					if (Bot.NavigationCache.AttemptFindSafeSpot(out UnstuckPos, Vector3.Zero, Bot.Settings.Plugin.AvoidanceFlags))
					{
						Logger.Write(LogLevel.OutOfCombat, "Generated Unstuck Position at {0}", UnstuckPos.ToString());
						ZetaDia.Me.UsePower(SNOPower.Walk, UnstuckPos, Bot.Character.Data.iCurrentWorldID);
					}

				}

				return RunStatus.Running;
			}

			//Precheck - Ingame, not dead..
			if (!TPActionIsValid())
			{
				ResetTPBehavior();
				return RunStatus.Success;
			}
			if (ZetaDia.IsLoadingWorld) //Loading.. we just wait!
				return RunStatus.Running;
			if (!CanCastTP()) //Not loading but is valid.. see if we can cast?
			{
				ResetTPBehavior();
				return RunStatus.Success;
			}

			//Set our flag which is used to setup the refreshing specific for this/similar behaviors.
			FunkyTPBehaviorFlag = true;

			//Refresh?
			if (Bot.Targeting.ShouldRefreshObjectList)
			{
				Bot.Targeting.RefreshDiaObjects();
			}

			//Check if we have any NEW targets to deal with.. 
			//Note: Refresh will filter targets to units and avoidance ONLY.
			if (Bot.Targeting.CurrentTarget != null)
			{
				Bot.Targeting.TargetMover.RestartTracking();

				//Directly Handle Target..
				RunStatus targetHandler = Bot.Targeting.HandleThis();

				//Only return failure if handling failed..
				if (targetHandler == RunStatus.Failure)
				{
					ResetTPBehavior();
					return RunStatus.Success;
				}
				if (targetHandler == RunStatus.Success)
				{
					Bot.Targeting.ResetTargetHandling();
				}

				return RunStatus.Running;
			}
			if (MovementOccured)
			{
				//Backtrack to orginal location...

				bool isMoving = false;
				try
				{
					isMoving = ZetaDia.Me.Movement.IsMoving;
				}
				catch (NullReferenceException) { }

				//Use simple checking of movement, with UsePower on our last location.
				if (!isMoving)
				{
					double DistanceFromStart = StartingPosition.Distance(Bot.Character.Data.Position);

					if (DistanceFromStart > 15f && DistanceFromStart < 50f)
					{
						//Logger.DBLog.InfoFormat("[FunkyTP] Backtracking!");
						//Move back to starting position..
						//ZetaDia.Me.UsePower(SNOPower.Walk, StartingPosition);
						//return RunStatus.Running;
					}
					else if (DistanceFromStart >= 50f)
					{
						//Logger.DBLog.InfoFormat("[FunkyTP] Range from our starting position is {0}. Now using Navigator to move.", DistanceFromStart);
						//Navigator.MoveTo(StartingPosition, "Backtracking to Orginal Position", true);
					}
				}
				else
					return RunStatus.Running;

				MovementOccured = false;
			}

			//Update Movement Data
			Bot.NavigationCache.RefreshMovementCache();

			//Make sure we are not moving..
			if (Bot.NavigationCache.IsMoving)
				return RunStatus.Running;

			//Check if we are casting, if not cast, else if casting but time has elapsed then cancel cast.

			if (!CastingRecall())
			{
				//Check last time cast..
				if (ElapsedTime > 5 && CastAttempted)
				{
					worldtransferStarted = true;
					return RunStatus.Running;
				}
				if (ElapsedTime > 8 || !CastAttempted)
				{
					//Recast
					Logger.Write(LogLevel.OutOfCombat, "Casting TP..");
					ZetaDia.Me.UseTownPortal();
					CastAttempted = true;
					FunkyTP_LastCastAttempt = DateTime.Now;
				}

				return RunStatus.Running;
			}
			if (ElapsedTime > 8)
			{
				//Void Cast?
				Logger.Write(LogLevel.OutOfCombat, "Attempting to void cast with movement..");
				Vector3 V3loc;
				bool success = Bot.NavigationCache.AttemptFindSafeSpot(out V3loc, Vector3.Zero, Bot.Settings.Plugin.AvoidanceFlags);
				if (success)
				{
					Navigator.MoveTo(V3loc, "Void Cast Movement", false);
				}

				return RunStatus.Running;
			}

			return RunStatus.Running;
		}

		internal static RunStatus FunkyTownPortalTownRun(object ret)
		{
			TownRunManager.TownrunStartedInTown = false;
			return RunStatus.Success;
		}
	}
}