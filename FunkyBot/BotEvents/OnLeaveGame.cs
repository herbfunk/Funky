using System;
using Zeta;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.Actors;
using System.Threading;
using Zeta.CommonBot;

namespace FunkyBot
{
    public partial class Funky
    {
        // Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
        private static void FunkyOnLeaveGame(object src, EventArgs mea)
        {
				//Update Game Duration..
				//Bot.BotStatistics.ProfileStats.OutputReport();

            ResetGame();
				initTreeHooks=false;
        }
    }
}