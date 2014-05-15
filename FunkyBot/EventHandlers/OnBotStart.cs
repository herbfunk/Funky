using FunkyBot.Cache;
using FunkyBot.Misc;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Game;

namespace FunkyBot
{
	public partial class EventHandlers
	{
		internal static void FunkyBotStart(IBot bot)
		{
			if (!Funky.bPluginEnabled && bot != null)
			{
				Logger.DBLog.InfoFormat("WARNING: FunkyBot Plugin is NOT ENABLED. Bot start detected");
				return;
			}

			string FunkySettingsPath = System.IO.Path.Combine(FolderPaths.DemonBuddyPath, "Settings", "FunkyBot");
			if (!System.IO.Directory.Exists(FunkySettingsPath))
			{
				Logger.DBLog.DebugFormat("Creating Settings Folder at location {0}", FunkySettingsPath);
				System.IO.Directory.CreateDirectory(FunkySettingsPath);
			}
			Logger.DBLog.DebugFormat("[Funky] Plugin settings location=" + FunkySettingsPath);


			//Load Settings
			//Bot.Character.Account.UpdateCurrentAccountDetails();
			//
			Bot.Reset();
			


			Navigator.PlayerMover = new Funky.PlayerMover();
			Navigator.StuckHandler = new TrinityStuckHandler();
			GameEvents.OnPlayerDied += FunkyOnDeath;
			GameEvents.OnGameJoined += FunkyOnJoinGame;
			GameEvents.OnGameLeft += FunkyOnLeaveGame;
			GameEvents.OnGameChanged += FunkyOnGameChanged;
			ProfileManager.OnProfileLoaded += FunkyOnProfileChanged;

			ITargetingProvider newCombatTargetingProvider = new TrinityCombatTargetingReplacer();
			CombatTargeting.Instance.Provider = newCombatTargetingProvider;
			ITargetingProvider newLootTargetingProvider = new TrinityLootTargetingProvider();
			LootTargeting.Instance.Provider = newLootTargetingProvider;
			ITargetingProvider newObstacleTargetingProvider = new TrinityObstacleTargetingProvider();
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