using System;

namespace FunkyBot
{
	public partial class EventHandlers
	{
		// Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
		internal static void FunkyOnLeaveGame(object src, EventArgs mea)
		{
			Logger.Write(LogLevel.Event, "OnLeaveGame Event");

			Bot.Game.CurrentGameStats.CurrentProfile.UpdateRangeVariables();
			Funky.ResetGame();
			Funky.initTreeHooks = false;
		}
	}
}