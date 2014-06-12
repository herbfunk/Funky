using System.Globalization;
using System.IO;
using FunkyBot.Cache;
using FunkyBot.Misc;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Game;

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


			Bot.Reset();
	
			Navigator.PlayerMover = new PlayerMover();
			Navigator.StuckHandler = new Funky.TrinityStuckHandler();
			GameEvents.OnPlayerDied += FunkyOnDeath;
			GameEvents.OnGameJoined += FunkyOnJoinGame;
			GameEvents.OnGameLeft += FunkyOnLeaveGame;
			GameEvents.OnGameChanged += FunkyOnGameChanged;
			GameEvents.OnWorldChanged += FunkyOnWorldChange;
			ProfileManager.OnProfileLoaded += FunkyOnProfileChanged;

			//Attach Level Up Event for characters less than 70!
			if (Bot.Character.Account.CurrentLevel<70)
			{
				Logger.DBLog.Debug("[Funky] Attaching Level Up Event!");
				GameEvents.OnLevelUp += FunkyOnLevelUp;
			}

			ITargetingProvider newCombatTargetingProvider = new Funky.TrinityCombatTargetingReplacer();
			CombatTargeting.Instance.Provider = newCombatTargetingProvider;
			ITargetingProvider newLootTargetingProvider = new Funky.TrinityLootTargetingProvider();
			LootTargeting.Instance.Provider = newLootTargetingProvider;
			ITargetingProvider newObstacleTargetingProvider = new Funky.TrinityObstacleTargetingProvider();
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