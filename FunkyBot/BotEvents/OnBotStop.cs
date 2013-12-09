using System;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.CommonBot;
using FunkyBot.Game;

namespace FunkyBot
{
	public partial class Funky
	{
		// When the bot stops, output a final item-stats report so it is as up-to-date as can be
		private void FunkyBotStop(IBot bot)
		{
			// Issue final reports
			Logger.WriteProfileTrackerOutput();

			PlayerMover.iTotalAntiStuckAttempts = 1;
			PlayerMover.vSafeMovementLocation = Vector3.Zero;
			PlayerMover.vOldPosition = Vector3.Zero;
			PlayerMover.iTimesReachedStuckPoint = 0;
			PlayerMover.timeLastRecordedPosition = DateTime.Today;
			PlayerMover.timeStartedUnstuckMeasure = DateTime.Today;
			ProfileCache.hashUseOnceID = new HashSet<int>();
			ProfileCache.dictUseOnceID = new Dictionary<int, int>();
			ProfileCache.dictRandomID = new Dictionary<int, int>();


			initTreeHooks = false;
			RemoveHandlers();
			ResetTreehooks();


		}
	}
}