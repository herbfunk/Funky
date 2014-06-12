﻿using System;
using FunkyBot.Misc;
using Zeta.Bot;
using Zeta.Bot.Navigation;

namespace FunkyBot.EventHandlers
{
	public partial class EventHandlers
	{
		internal static void FunkyOnGameChanged(object sender, EventArgs e)
		{
			Logger.Write(LogLevel.Event, "OnGameChanged Event");
			Bot.ResetGame();
			Bot.Game.RefreshGameId();

			string currentProfilePath = ProfileManager.CurrentProfile.Path;
			ProfileManager.Load(currentProfilePath);
			Navigator.SearchGridProvider.Update();
		}
	}
}