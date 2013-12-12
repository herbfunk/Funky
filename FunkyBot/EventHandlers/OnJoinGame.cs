using System;

namespace FunkyBot
{
	public partial class EventHandlers
    {
		  
        // Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
        internal static void FunkyOnJoinGame(object src, EventArgs mea)
		{
			if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Event))
				Logger.Write(LogLevel.Event, "OnJoinGame Event");
			Funky.ResetGame();
			Bot.Game.RefreshGameId();

			if (Bot.Settings.Demonbuddy.EnableDemonBuddyCharacterSettings)
				Zeta.CommonBot.Settings.CharacterSettings.Instance.MonsterPowerLevel = Funky.iDemonbuddyMonsterPowerLevel;
        }
    }
}