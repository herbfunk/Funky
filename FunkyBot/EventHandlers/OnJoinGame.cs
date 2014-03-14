using System;
using Zeta.Bot.Settings;

namespace FunkyBot
{
	public partial class EventHandlers
	{

		// Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
		internal static void FunkyOnJoinGame(object src, EventArgs mea)
		{
			Logger.Write(LogLevel.Event, "OnJoinGame Event");
			Funky.ResetGame();
			Bot.Game.RefreshGameId();

			if (Bot.Settings.Demonbuddy.EnableDemonBuddyCharacterSettings)
				CharacterSettings.Instance.MonsterPowerLevel = Funky.iDemonbuddyMonsterPowerLevel;
		}
	}
}