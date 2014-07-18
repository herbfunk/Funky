using System;
using fBaseXtensions.Helpers;
using FunkyBot.Misc;
using FunkyBot.Player.Class;

namespace FunkyBot.EventHandlers
{
	public partial class EventHandlers
	{
		// Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
		internal static void FunkyOnLevelUp(object src, EventArgs mea)
		{
			Logger.Write(LogLevel.Event, "OnLevelUp Event");
			PlayerClass.ShouldRecreatePlayerClass = true;
		}
	}
}
