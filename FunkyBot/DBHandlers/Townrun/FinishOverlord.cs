using System;
using FunkyBot.Movement;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.TreeSharp;
using Logger = FunkyBot.Misc.Logger;

namespace FunkyBot.DBHandlers.Townrun
{

	internal static partial class TownRunManager
	{
		internal static bool bFailedToLootLastItem = false;
		internal static bool TalliedTownRun = false;
		internal static bool StatsOverlord(object ret)
		{
			return !TalliedTownRun;
		}

		internal static RunStatus StatsBehavior(object ret)
		{
			TalliedTownRun = true;
			return RunStatus.Success;
		}

		private static bool ResetCache = false;
		internal static RunStatus FinishBehavior(object ret)
		{
			if (!ResetCache)
			{
				ZetaDia.Memory.ClearCache();
				ZetaDia.Actors.Update();
				ResetCache = true;
			}

			//If not in town.. lets check if there are any valid targets we could handle!
			if (!ZetaDia.IsInTown)
			{
				if (Bot.Targeting.CheckHandleTarget() == RunStatus.Running) return RunStatus.Running;

				float distanceFromStart=Bot.Character.Data.Position.Distance(TownPortalBehavior.StartingPosition);

				//Backtrack to our starting location!
				if (TownPortalBehavior.StartingPosition != Vector3.Zero && distanceFromStart > 25f)
				{
					Logger.DBLog.InfoFormat("Returning to Start Location -- Distance of {0}", distanceFromStart);
					Bot.NavigationCache.RefreshMovementCache();
					if (Bot.NavigationCache.IsMoving) return RunStatus.Running;
					Navigator.MoveTo(TownPortalBehavior.StartingPosition, "TownPortalStart");
					return RunStatus.Running;
				}
			}

			Bot.Game.GoldTimeoutChecker.LastCoinageUpdate = DateTime.Now;
			Navigation.MGP.Update();

			ResetCache = false;
			bFailedToLootLastItem = false;

			Logger.DBLog.Info("Finished Town Run Behavior!");
			return RunStatus.Success;
		}


	}

}