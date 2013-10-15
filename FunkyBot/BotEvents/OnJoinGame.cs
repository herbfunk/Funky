using System;
using Zeta;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.Actors;
using System.Threading;
using Zeta.CommonBot;
using System.IO;

namespace FunkyBot
{
    public partial class Funky
    {
		  
        // Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
        private static void FunkyOnJoinGame(object src, EventArgs mea)
        {
				Bot.Stats.iTotalJoinGames++;
				Bot.Stats.LastJoinedGame=DateTime.Now;

				Bot.BotStatistics.GameStats.Update();

            ResetGame();
            //Start new current game stats
				Bot.BotStatistics.ItemStats.CurrentGame.Reset();
				Bot.BotStatistics.GameStats.CurrentGame.Reset();

				if (Bot.Settings.Demonbuddy.EnableDemonBuddyCharacterSettings)
					 Zeta.CommonBot.Settings.CharacterSettings.Instance.MonsterPowerLevel=Funky.iDemonbuddyMonsterPowerLevel;



        }
    }
}