using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fBaseXtensions.Cache;
using fBaseXtensions.Game;
using fBaseXtensions.Monitor;
using fBaseXtensions.Settings;
using fBaseXtensions.Stats;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Service;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fBaseXtensions
{
	public static class EventHandling
	{
		internal static void OnBotStart(IBot bot)
		{
			TheCache.ObjectIDCache = new IDCache();
			HookEvents();
			if (ZetaDia.IsInGame) CheckGameIDChange();
		}
		internal static void OnBotStop(IBot bot)
		{
			// Issue final reports
			FunkyGame.TrackingStats.GameStopped(ref FunkyGame.CurrentGameStats);
			TotalStats.WriteProfileTrackerOutput(FunkyGame.TrackingStats);
			
			FunkyGame.CurrentGameID = new GameId();
			FunkyGame.AdventureMode = false;
			UnhookEvents();
		}
		private static void OnGameChanged(object obj, EventArgs args)
		{
			CheckGameIDChange();
		}
		private static void CheckGameIDChange()
		{
			GameId curgameID = FunkyGame.CurrentGameID;
		
			using (ZetaDia.Memory.AcquireFrame())
			{
				try
				{
					curgameID = ZetaDia.Service.CurrentGameId;
				}
				catch
				{

				}
			}
			

			if (!curgameID.Equals(FunkyGame.CurrentGameID))
			{
				int questId = 0;
				using (ZetaDia.Memory.AcquireFrame())
				{
					try
					{
						questId = ZetaDia.CurrentQuest.QuestSNO;
					}
					catch
					{

					}
				}
				//Merge last GameStats with the Total
				FunkyGame.TrackingStats.GameChanged(ref FunkyGame.CurrentGameStats);
				//Create new GameStats
				FunkyGame.CurrentGameStats = new Stats.GameStats();

				FunkyGame.AdventureMode = (questId == 312429);
				if (FunkyGame.AdventureMode)
					Logger.DBLog.InfoFormat("Adventure Mode Active!");

				FunkyGame.CurrentGameID=curgameID;
				FunkyGame.ShouldRefreshAccountDetails = true;
				GoldInactivity.LastCoinageUpdate = DateTime.Now;
				Navigator.SearchGridProvider.Update();

				if (OnGameIDChanged != null)
					OnGameIDChanged();
			}
		}
		public delegate void GameIDChanged();
		/// <summary>
		/// Will fire when the Game ID has changed
		/// </summary>
		public static event GameIDChanged OnGameIDChanged;

		private static void OnGameJoined(object obj, EventArgs args)
		{
			CheckGameIDChange();
		}
		private static void OnGameLeft(object obj, EventArgs args)
		{
			FunkyGame.CurrentGameStats.CurrentProfile.UpdateRangeVariables();
			FunkyGame.CurrentGameID = new GameId();
			FunkyGame.AdventureMode = false;
			FunkyGame.ShouldRefreshAccountDetails = true;
		}
		private static void OnProfileChanged(object obj, EventArgs args)
		{
			string sThisProfile = ProfileManager.CurrentProfile.Path;
			FunkyGame.CurrentGameStats.ProfileChanged(sThisProfile);
		}
		private static void HookEvents()
		{
			Zeta.Bot.GameEvents.OnGameChanged += OnGameChanged;
			Zeta.Bot.GameEvents.OnGameJoined += OnGameJoined;
			Zeta.Bot.GameEvents.OnGameLeft += OnGameLeft;
			ProfileManager.OnProfileLoaded += OnProfileChanged;
		}
		private static void UnhookEvents()
		{
			Zeta.Bot.GameEvents.OnGameChanged -= OnGameChanged;
			Zeta.Bot.GameEvents.OnGameJoined -= OnGameJoined;
			Zeta.Bot.GameEvents.OnGameLeft -= OnGameLeft;
			ProfileManager.OnProfileLoaded -= OnProfileChanged;
		}
	}
}
