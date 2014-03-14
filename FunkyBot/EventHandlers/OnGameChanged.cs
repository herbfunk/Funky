using System;
using Zeta.Bot.Navigation;

namespace FunkyBot
{
	public partial class EventHandlers
	{
		internal static void FunkyOnGameChanged(object sender, EventArgs e)
		{
			Logger.Write(LogLevel.Event, "OnGameChanged Event");

			Navigator.SearchGridProvider.Update();
			Funky.ResetGame();
			Bot.Game.RefreshGameId();
		}
	}
}