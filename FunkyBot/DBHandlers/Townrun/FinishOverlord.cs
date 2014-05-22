using System;
using FunkyBot.Player;
using Zeta.Bot;
using Zeta.Game.Internals.Actors;
using System.Linq;
using Zeta.TreeSharp;

namespace FunkyBot.DBHandlers
{

	internal static partial class TownRunManager
	{
		internal static bool TalliedTownRun = false;
		internal static bool FinishOverlord(object ret)
		{
			return !TalliedTownRun;
		}

		internal static RunStatus FinishBehavior(object ret)
		{
			Bot.Game.CurrentGameStats.CurrentProfile.TownRuns++;
			TalliedTownRun = true;
			Logger.DBLog.Info("Finished Town Run Behavior!");
			return RunStatus.Success;
		}


	}

}