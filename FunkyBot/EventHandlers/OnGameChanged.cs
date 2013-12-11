using System;
using Zeta.Navigation;

namespace FunkyBot
{
	public partial class EventHandlers
    {
		  internal static void FunkyOnGameChanged(object sender, EventArgs e)
		  {
			  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Event))
				  Logger.Write(LogLevel.Event, "OnGameChanged Event");

			  Navigator.SearchGridProvider.Update();
			  Funky.ResetGame();
			  Bot.Game.RefreshGameId();
		  }
    }
}