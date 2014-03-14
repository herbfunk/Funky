using FunkyBot.Settings;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Game;

namespace FunkyBot
{
	public partial class EventHandlers
	{
		internal static void FunkyBotStart(IBot bot)
		{
			string FunkySettingsPath = System.IO.Path.Combine(FolderPaths.sDemonBuddyPath, "Settings", "FunkyBot");
			if (!System.IO.Directory.Exists(FunkySettingsPath))
			{
				Logger.DBLog.DebugFormat("Creating Settings Folder at location {0}", FunkySettingsPath);
				System.IO.Directory.CreateDirectory(FunkySettingsPath);
			}

			Logger.DBLog.DebugFormat("[Funky] Plugin settings location=" + FunkySettingsPath);


			//Load Settings
			Settings_Funky.LoadFunkyConfiguration();
			Bot.Character.ItemRulesEval = new Interpreter();


			Navigator.PlayerMover = new Funky.PlayerMover();
			Navigator.StuckHandler = new TrinityStuckHandler();
			GameEvents.OnPlayerDied += EventHandlers.FunkyOnDeath;
			GameEvents.OnGameJoined += EventHandlers.FunkyOnJoinGame;
			GameEvents.OnGameLeft += EventHandlers.FunkyOnLeaveGame;
			GameEvents.OnGameChanged += EventHandlers.FunkyOnGameChanged;
			ProfileManager.OnProfileLoaded += EventHandlers.FunkyOnProfileChanged;

			ITargetingProvider newCombatTargetingProvider = new TrinityCombatTargetingReplacer();
			CombatTargeting.Instance.Provider = newCombatTargetingProvider;
			ITargetingProvider newLootTargetingProvider = new TrinityLootTargetingProvider();
			LootTargeting.Instance.Provider = newLootTargetingProvider;
			ITargetingProvider newObstacleTargetingProvider = new TrinityObstacleTargetingProvider();
			ObstacleTargeting.Instance.Provider = newObstacleTargetingProvider;



			if (!Funky.bPluginEnabled && bot != null)
			{
				Logger.DBLog.InfoFormat("WARNING: FunkyBot Plugin is NOT ENABLED. Bot start detected");
				return;
			}

			if (!Funky.initTreeHooks)
			{
				Funky.HookBehaviorTree();
			}

			bool isingame = ZetaDia.IsInGame;
			if (isingame && !BotMain.IsRunning)
			{
				EventHandlers.FunkyOnGameChanged(null, null);
			}




			Navigator.SearchGridProvider.Update();
		}
	}
}