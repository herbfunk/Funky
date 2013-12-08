using System;

namespace FunkyBot
{
    public partial class Funky
    {
		  
        // Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
        private static void FunkyOnJoinGame(object src, EventArgs mea)
		{
			if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Event))
				Logger.Write(LogLevel.Event, "OnJoinGame Event");
			ResetGame();
			Bot.Game.RefreshGameId();

			if (Bot.Settings.Demonbuddy.EnableDemonBuddyCharacterSettings)
				Zeta.CommonBot.Settings.CharacterSettings.Instance.MonsterPowerLevel = iDemonbuddyMonsterPowerLevel;
        }
    }
}