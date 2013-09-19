using System;
using Zeta;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.Actors;
using System.Threading;
using Zeta.CommonBot;
using Zeta.Navigation;

namespace FunkyTrinity
{
    public partial class Funky
    {
        // When the bot stops, output a final item-stats report so it is as up-to-date as can be
        private void FunkyBotStop(IBot bot)
        {
            // Issue final reports
            OutputReport();

            PlayerMover.iTotalAntiStuckAttempts = 1;
            PlayerMover.vSafeMovementLocation = Vector3.Zero;
            PlayerMover.vOldPosition = Vector3.Zero;
            PlayerMover.iTimesReachedStuckPoint = 0;
            PlayerMover.timeLastRecordedPosition = DateTime.Today;
            PlayerMover.timeStartedUnstuckMeasure = DateTime.Today;
            hashUseOnceID = new HashSet<int>();
            dictUseOnceID = new Dictionary<int, int>();
            dictRandomID = new Dictionary<int, int>();
				Bot.Stats.iMaxDeathsAllowed=0;
				Bot.Stats.iDeathsThisRun=0;
				initTreeHooks=false;
            //Total Stats
				Bot.BotStatistics.ItemStats.Update();
				Bot.BotStatistics.GameStats.Update();
				Bot.BotStatistics.ProfileStats.OutputReport();
				LeveledUpEventFired=false;


				RemoveHandlers();
				ResetTreehooks();


		  }
    }
}