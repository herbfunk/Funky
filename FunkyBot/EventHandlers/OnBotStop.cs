using System;
using Zeta.Bot;
using Zeta.Common;

namespace FunkyBot.EventHandlers
{
	public partial class EventHandlers
	{
		// When the bot stops, output a final item-stats report so it is as up-to-date as can be
		internal static void FunkyBotStop(IBot bot)
		{
			// Issue final reports
			//Bot.Game.TrackingStats.GameStopped(ref Bot.Game.CurrentGameStats);
			//TotalStats.WriteProfileTrackerOutput(Bot.Game.TrackingStats);

			PlayerMover.iTotalAntiStuckAttempts = 1;
			PlayerMover.vSafeMovementLocation = Vector3.Zero;
			PlayerMover.vOldPosition = Vector3.Zero;
			PlayerMover.iTimesReachedStuckPoint = 0;
			PlayerMover.timeLastRecordedPosition = DateTime.Today;
			PlayerMover.timeStartedUnstuckMeasure = DateTime.Today;
			//ProfileCache.hashUseOnceID = new HashSet<int>();
			//ProfileCache.dictUseOnceID = new Dictionary<int, int>();
			//ProfileCache.dictRandomID = new Dictionary<int, int>();
			Funky.initTreeHooks = false;
			Funky.RemoveHandlers();
			Funky.ResetTreehooks();
			//Bot.Reset();

		}
	}
}