using System;
using FunkyBot.Misc;

namespace FunkyBot.EventHandlers
{
	public partial class EventHandlers
	{
		// Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
		internal static void FunkyOnWorldChange(object src, EventArgs mea)
		{
			Logger.Write(LogLevel.Event, "OnWorldChange Event");
			//Funky.ResetGame();
			//Bot.Game.RefreshGameId();

			//if (Bot.Settings.Demonbuddy.EnableDemonBuddyCharacterSettings)
			//	CharacterSettings.Instance.MonsterPowerLevel = Funky.iDemonbuddyMonsterPowerLevel;
		}
	}
}
