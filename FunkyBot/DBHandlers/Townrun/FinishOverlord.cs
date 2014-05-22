using System;
using FunkyBot.Player;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using System.Linq;
using Zeta.TreeSharp;

namespace FunkyBot.DBHandlers
{

	internal static partial class TownRunManager
	{
		internal static bool TalliedTownRun = false;
		internal static bool StatsOverlord(object ret)
		{
			return !TalliedTownRun;
		}

		internal static RunStatus StatsBehavior(object ret)
		{
			Bot.Game.CurrentGameStats.CurrentProfile.TownRuns++;
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
				if (Bot.Targeting.CheckHandleTarget() == RunStatus.Running)
					return RunStatus.Running;
			}

			ResetCache = false;
			Logger.DBLog.Info("Finished Town Run Behavior!");
			return RunStatus.Success;
		}


	}

}