using System;
using Zeta.Bot;
using Zeta.Common;
using System.Collections.Generic;
using FunkyBot.Game;

namespace FunkyBot
{
	public partial class EventHandlers
	{
		// When the bot stops, output a final item-stats report so it is as up-to-date as can be
		internal static void FunkyBotStop(IBot bot)
		{
			// Issue final reports
			Bot.Game.TrackingStats.GameStopped(ref Bot.Game.CurrentGameStats);

			Logger.WriteProfileTrackerOutput();
			Funky.PlayerMover.iTotalAntiStuckAttempts = 1;
			Funky.PlayerMover.vSafeMovementLocation = Vector3.Zero;
			Funky.PlayerMover.vOldPosition = Vector3.Zero;
			Funky.PlayerMover.iTimesReachedStuckPoint = 0;
			Funky.PlayerMover.timeLastRecordedPosition = DateTime.Today;
			Funky.PlayerMover.timeStartedUnstuckMeasure = DateTime.Today;
			ProfileCache.hashUseOnceID = new HashSet<int>();
			ProfileCache.dictUseOnceID = new Dictionary<int, int>();
			ProfileCache.dictRandomID = new Dictionary<int, int>();
			Funky.initTreeHooks = false;
			Funky.RemoveHandlers();
			Funky.ResetTreehooks();
			//Bot.Reset();

		}
	}
}