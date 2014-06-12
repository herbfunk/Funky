using System;
using FunkyBot.Misc;

namespace FunkyBot.EventHandlers
{
	public partial class EventHandlers
	{
		// Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
		internal static void FunkyOnLeaveGame(object src, EventArgs mea)
		{
			Logger.Write(LogLevel.Event, "OnLeaveGame CursedEvent");

			Bot.Game.CurrentGameStats.CurrentProfile.UpdateRangeVariables();
			Bot.ResetGame();
			Funky.initTreeHooks = false;
		}
	}
}