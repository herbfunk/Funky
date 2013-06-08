using System;
using Zeta;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.Actors;
using System.Threading;
using Zeta.CommonBot;
using System.IO;

namespace FunkyTrinity
{
    public partial class Funky
    {
		  internal static DateTime LastJoinedGame=DateTime.MinValue;
        // Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
        private static void FunkyOnJoinGame(object src, EventArgs mea)
        {
				Bot.iTotalJoinGames++;
            ResetGame();
            //Start new current game stats
            Statistics.ItemStats.CurrentGame.Reset();
            Statistics.GameStats.CurrentGame.Reset();
				ResetProfileVars();

				LastJoinedGame=DateTime.Now;

				//Disconnect -- Starting Profile Setup.
				if (HadDisconnectError)
				{
					 Logging.Write("Disconnect Error Last Game.. attempting to find starting profile.");
					 ReloadStartingProfile();
					 HadDisconnectError=false;
				}

        }
    }
}