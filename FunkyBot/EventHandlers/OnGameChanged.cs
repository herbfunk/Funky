using System;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Game;

namespace FunkyBot
{
	public partial class EventHandlers
	{
		internal static void FunkyOnGameChanged(object sender, EventArgs e)
		{
			Logger.Write(LogLevel.Event, "OnGameChanged Event");
			Funky.ResetGame();
			Bot.Game.RefreshGameId();

			string currentProfilePath = ProfileManager.CurrentProfile.Path;
			ProfileManager.Load(currentProfilePath);
			Navigator.SearchGridProvider.Update();
		}
	}
}