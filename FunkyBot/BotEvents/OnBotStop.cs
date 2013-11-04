using System;
using FunkyBot.Cache;
using Zeta;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.Actors;
using System.Threading;
using Zeta.CommonBot;
using Zeta.Navigation;
using FunkyBot.Game;

namespace FunkyBot
{
    public partial class Funky
    {
        // When the bot stops, output a final item-stats report so it is as up-to-date as can be
        private void FunkyBotStop(IBot bot)
        {
            // Issue final reports
            //OutputReport();

            PlayerMover.iTotalAntiStuckAttempts = 1;
            PlayerMover.vSafeMovementLocation = Vector3.Zero;
            PlayerMover.vOldPosition = Vector3.Zero;
            PlayerMover.iTimesReachedStuckPoint = 0;
            PlayerMover.timeLastRecordedPosition = DateTime.Today;
            PlayerMover.timeStartedUnstuckMeasure = DateTime.Today;
				ProfileCache.hashUseOnceID=new HashSet<int>();
				ProfileCache.dictUseOnceID=new Dictionary<int, int>();
				ProfileCache.dictRandomID=new Dictionary<int, int>();
                //Bot.Stats.iMaxDeathsAllowed=0;
                //Bot.Stats.iDeathsThisRun=0;
				initTreeHooks=false;
            //Total Stats
                //Bot.BotStatistics.ItemStats.Update();
                //Bot.BotStatistics.GameStats.Update();
                //Bot.BotStatistics.ProfileStats.OutputReport();

				RemoveHandlers();
				ResetTreehooks();


		  }
    }
}