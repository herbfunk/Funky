using System;
using Zeta;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.Actors;
using System.Threading;
using Zeta.CommonBot;
using FunkyBot.Game;

namespace FunkyBot
{
    public partial class Funky
    {
        // Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
        internal static void FunkyOnLeaveGame(object src, EventArgs mea)
        {
			if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Event))
				Logger.Write(LogLevel.Event, "OnLeaveGame Event");

            TotalStats.CurrentTrackingProfile.UpdateRangeVariables();
            ResetGame();
			initTreeHooks=false;
        }
    }
}