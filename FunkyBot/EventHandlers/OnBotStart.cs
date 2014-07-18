using System.Linq;
using fBaseXtensions;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Helpers;
using fBaseXtensions.Monitor;
using FunkyBot.Cache;
using FunkyBot.Game;
using FunkyBot.Misc;
using FunkyBot.Player.Class;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common.Plugins;
using Zeta.Game;
using Logger = fBaseXtensions.Helpers.Logger;

namespace FunkyBot.EventHandlers
{
	public partial class EventHandlers
	{
		internal static void FunkyBotStart(IBot bot)
		{
			string FunkySettingsPath = System.IO.Path.Combine(FolderPaths.DemonBuddyPath, "Settings", "FunkyBot");
			if (!System.IO.Directory.Exists(FunkySettingsPath))
			{
				Logger.DBLog.DebugFormat("Creating Settings Folder at location {0}", FunkySettingsPath);
				System.IO.Directory.CreateDirectory(FunkySettingsPath);
			}
			Logger.DBLog.DebugFormat("[Funky] Plugin settings location=" + FunkySettingsPath);

			var basePlugin = PluginManager.Plugins.First(p => p.Plugin.Name == "fBaseXtensions");
			if (basePlugin!=null)
			{
				if (!basePlugin.Enabled)
				{
					Logger.DBLog.Warn("FunkyBot requires fBaseXtensions to be enabled! -- Enabling it automatically.");
					basePlugin.Enabled = true;
				}
			}

			Bot.Reset();
	
			Navigator.PlayerMover = new PlayerMover();
			Navigator.StuckHandler = new Funky.PluginStuckHandler();
			GameEvents.OnPlayerDied += FunkyOnDeath;
			GameEvents.OnGameJoined += FunkyOnJoinGame;
			GameEvents.OnGameLeft += FunkyOnLeaveGame;
			GameEvents.OnGameChanged += FunkyOnGameChanged;
			GameEvents.OnWorldChanged += FunkyOnWorldChange;
			ProfileManager.OnProfileLoaded += FunkyOnProfileChanged;
			Hotbar.OnSkillsChanged += PlayerClass.HotbarSkillsChangedHandler;
			GoldInactivity.OnGoldTimeoutTripped += GameCache.GoldInactivityTimerTrippedHandler;
			EventHandling.OnGameIDChanged += Bot.Game.OnGameIDChangedHandler;
			FunkyGame.Profile.OnProfileBehaviorChange += Bot.Game.OnProfileBehaviorChanged;

			//Attach Level Up Event for characters less than 70!
			//if (Bot.Character.Account.CurrentLevel<70)
			//{
			//	Logger.DBLog.Debug("[Funky] Attaching Level Up Event!");
			//	GameEvents.OnLevelUp += FunkyOnLevelUp;
			//}

			ITargetingProvider newCombatTargetingProvider = new Funky.PluginCombatTargeting();
			CombatTargeting.Instance.Provider = newCombatTargetingProvider;
			ITargetingProvider newLootTargetingProvider = new Funky.PluginLootTargeting();
			LootTargeting.Instance.Provider = newLootTargetingProvider;
			ITargetingProvider newObstacleTargetingProvider = new Funky.PluginObstacleTargeting();
			ObstacleTargeting.Instance.Provider = newObstacleTargetingProvider;





			if (!Funky.initTreeHooks)
			{
				Funky.HookBehaviorTree();
			}

			bool isingame = ZetaDia.IsInGame;
			if (isingame && !BotMain.IsRunning)
			{
				FunkyOnGameChanged(null, null);
			}

			if (Bot.Settings.Debug.DebuggingData)
			{
				Logger.DBLog.Debug("Loading Debugging Data from Xml");
				ObjectCache.DebuggingData = DebugData.DeserializeFromXML();
			}


			Navigator.SearchGridProvider.Update();
		}
	}
}