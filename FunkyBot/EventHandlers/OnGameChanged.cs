using System;
using Zeta.Bot.Navigation;
using Zeta.Game;

namespace FunkyBot
{
	public partial class EventHandlers
	{
		internal static void FunkyOnGameChanged(object sender, EventArgs e)
		{
			Logger.Write(LogLevel.Event, "OnGameChanged Event");

			ZetaDia.Memory.ClearCache();
			Navigator.SearchGridProvider.Update();
			Funky.ResetGame();
			Bot.Game.RefreshGameId();
		}
	}
}