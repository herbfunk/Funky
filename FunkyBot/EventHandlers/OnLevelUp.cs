using System;

namespace FunkyBot
{
	public partial class EventHandlers
	{
		internal static bool LeveledUp = false;
		// Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
		internal static void FunkyOnLevelUp(object src, EventArgs mea)
		{
			Logger.Write(LogLevel.Event, "OnLevelUp Event");
			LeveledUp = true;
		}
	}
}
