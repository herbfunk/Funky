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
            ResetGame();
				Bot.RefreshGameID();

				if (Bot.Settings.Demonbuddy.EnableDemonBuddyCharacterSettings)
					 Zeta.CommonBot.Settings.CharacterSettings.Instance.MonsterPowerLevel=Funky.iDemonbuddyMonsterPowerLevel;



        }
    }
}