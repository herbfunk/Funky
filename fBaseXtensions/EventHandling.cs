using System;
using fBaseXtensions.Behaviors;
using fBaseXtensions.Cache.External.Debugging;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Monitor;
using fBaseXtensions.Settings;
using fBaseXtensions.Stats;
using fBaseXtensions.XML;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Game;
using Zeta.Game.Internals.Service;
using Logger = fBaseXtensions.Helpers.Logger;
using fBaseXtensions.Helpers;

namespace fBaseXtensions
{
	public static class EventHandling
	{
		internal static void OnBotStart(IBot bot)
		{
			if (ZetaDia.IsInGame) 
				CheckGameIDChange();

			if (FunkyBaseExtension.PluginIsEnabled)
			{
				if (FunkyBaseExtension.Settings.Debugging.DebuggingData)
				{
					Logger.DBLog.Debug("Loading Debugging Data from Xml");
					ObjectCache.DebuggingData = new DebugData();
				}

				FunkyGame.Reset();
				
				//Hotbar.OnSkillsChanged += PlayerClass.HotbarSkillsChangedHandler;
				
				GoldInactivity.OnGoldTimeoutTripped += GameCache.GoldInactivityTimerTrippedHandler;
				Equipment.OnEquippedItemsChanged += Equipment.EquippmentChangedHandler;
				
				if (!HookHandler.initTreeHooks)
					HookHandler.HookBehaviorTree();

			}

			HookEvents();
		}
		internal static void OnBotStop(IBot bot)
		{
			FunkyGame.CurrentGameID = new GameId();
			FunkyGame.AdventureMode = false;
			FunkyGame.ShouldRefreshAccountDetails = true;
			ExitGame.ShouldExitGame = false;
			ExitGame.BehaviorEngaged = false;
			SetVariableTag.VariableDictionary.Clear();

			if (FunkyBaseExtension.PluginIsEnabled)
			{
				//Hotbar.OnSkillsChanged -= PlayerClass.HotbarSkillsChangedHandler;
				Equipment.OnEquippedItemsChanged -= Equipment.EquippmentChangedHandler;
				// Issue final reports
				FunkyGame.TrackingStats.GameStopped(ref FunkyGame.CurrentGameStats);
				TotalStats.WriteProfileTrackerOutput(ref FunkyGame.TrackingStats);
			}

			

			if (HookHandler.initTreeHooks)
				HookHandler.ResetTreehooks();

			UnhookEvents();

			ZetaDia.Memory.ClearCache();
		}
		private static void OnGameChanged(object obj, EventArgs args)
		{
			Logger.Write(LogLevel.Event, "OnGameChanged Event");

			if (FunkyBaseExtension.PluginIsEnabled)
				FunkyGame.ResetBot();

			CheckGameIDChange();

			string currentProfilePath = ProfileManager.CurrentProfile.Path;
			ProfileManager.Load(currentProfilePath);
			Navigator.SearchGridProvider.Update();
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

				if (FunkyBaseExtension.PluginIsEnabled)
				{
					if (FunkyGame.CurrentGameStats == null)
					{
						FunkyGame.Hero.Update();
						FunkyGame.CurrentGameStats = new Stats.GameStats();
					}
					else
					{
						//Merge last GameStats with the Total
						FunkyGame.TrackingStats.GameChanged(ref FunkyGame.CurrentGameStats);
						//Create new GameStats
						FunkyGame.CurrentGameStats = new Stats.GameStats();
					}
				}


				FunkyGame.AdventureMode = (questId == 312429);
				if (FunkyGame.AdventureMode)
				{
					Logger.DBLog.InfoFormat("Adventure Mode Active!");
					FunkyGame.Bounty.Reset();
					FunkyGame.Bounty.RefreshBountyInfo();
					FunkyGame.Bounty.RefreshActiveQuests();
				}

				FunkyGame.CurrentGameID=curgameID;
				FunkyGame.ShouldRefreshAccountDetails = true;
				GoldInactivity.LastCoinageUpdate = DateTime.Now;
				//Navigator.SearchGridProvider.Update();

				if (OnGameIDChanged != null)
					OnGameIDChanged();
			}
		}
		public delegate void GameIDChanged();
		/// <summary>
		/// Will fire when the Game ID has changed
		/// </summary>
		public static event GameIDChanged OnGameIDChanged;

		private static void OnGameIDChangedHandler()
		{
			Logger.Write(LogLevel.OutOfCombat, "New Game Started");

			SettingAdventureMode.AdventureModeSettingsTag = FunkyBaseExtension.Settings.AdventureMode;
			if (FunkyGame.AdventureMode && SettingAdventureMode.AdventureModeSettingsTag.EnableAdventuringMode)
			{
				FunkyGame.Game.ResetCombatModifiers();
			}


			//Clear Interactable Cache
			ObjectCache.InteractableObjectCache.Clear();

			//Clear Health Average
			ObjectCache.Objects.ClearHealthAverageStats();

			//Renew bot
			FunkyGame.ResetBot();
		}

		private static void OnGameJoined(object obj, EventArgs args)
		{
			Logger.Write(LogLevel.Event, "OnJoinGame Event");
			if (FunkyBaseExtension.PluginIsEnabled)
				FunkyGame.ResetGame();

			CheckGameIDChange();
		}
		private static void OnGameLeft(object obj, EventArgs args)
		{
			Logger.Write(LogLevel.Event, "OnLeaveGame Event");

			FunkyGame.CurrentGameStats.CurrentProfile.UpdateRangeVariables();
			FunkyGame.CurrentGameID = new GameId();
			FunkyGame.AdventureMode = false;
			FunkyGame.ShouldRefreshAccountDetails = true;

			if (FunkyBaseExtension.PluginIsEnabled)
			{
				FunkyGame.ResetGame();
			}
		}
		private static void OnProfileChanged(object obj, EventArgs args)
		{
			Logger.Write(LogLevel.Event, "OnProfileChanged Event");
			string sThisProfile = ProfileManager.CurrentProfile.Path;
			FunkyGame.CurrentGameStats.ProfileChanged(sThisProfile);
			SettingCluster.ClusterSettingsTag = FunkyBaseExtension.Settings.Cluster;
			FunkyGame.Game.QuestMode = false;
			SettingLOSMovement.LOSSettingsTag = FunkyBaseExtension.Settings.LOSMovement;
			MonitorSettings.MonitorSettingsTag = FunkyBaseExtension.Settings.Monitoring;
			SettingAdventureMode.AdventureModeSettingsTag = FunkyBaseExtension.Settings.AdventureMode;
		}
		private static void OnPlayerDeath(object obj, EventArgs args)
		{
			Logger.Write(LogLevel.Event, "OnPlayerDied Event");
			FunkyGame.ResetBot();
		}
		private static void HookEvents()
		{
			GameEvents.OnGameChanged += OnGameChanged;
			GameEvents.OnGameJoined += OnGameJoined;
			GameEvents.OnGameLeft += OnGameLeft;

			if (FunkyBaseExtension.PluginIsEnabled)
			{
				OnGameIDChanged += FunkyGame.Game.OnGameIDChangedHandler;
				ProfileManager.OnProfileLoaded += OnProfileChanged;
				GameEvents.OnPlayerDied += OnPlayerDeath;
			}
		}
		private static void UnhookEvents()
		{
			GameEvents.OnGameChanged -= OnGameChanged;
			GameEvents.OnGameJoined -= OnGameJoined;
			GameEvents.OnGameLeft -= OnGameLeft;
			GameEvents.OnPlayerDied -= OnPlayerDeath;
			OnGameIDChanged -= FunkyGame.Game.OnGameIDChangedHandler;
			ProfileManager.OnProfileLoaded -= OnProfileChanged;
		}
	}
}
