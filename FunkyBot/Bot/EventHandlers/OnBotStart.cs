using FunkyBot.Settings;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Navigation;

namespace FunkyBot
{
    public partial class Funky
    {
		  private void FunkyBotStart(IBot bot)
		  {
				string FunkySettingsPath=System.IO.Path.Combine(FolderPaths.sDemonBuddyPath, "Settings", "FunkyBot");
				if (!System.IO.Directory.Exists(FunkySettingsPath))
				{
					 Logging.WriteDiagnostic("Creating Settings Folder at location {0}", FunkySettingsPath);
					 System.IO.Directory.CreateDirectory(FunkySettingsPath);
				}

				Logging.WriteDiagnostic("[Funky] Plugin settings location="+FunkySettingsPath);


				//Load Settings
				Settings_Funky.LoadFunkyConfiguration();
				Bot.ItemRulesEval=new Interpreter();


				Navigator.PlayerMover=new PlayerMover();
				Navigator.StuckHandler=new TrinityStuckHandler();
				GameEvents.OnPlayerDied+=FunkyOnDeath;
				GameEvents.OnGameJoined+=FunkyOnJoinGame;
				GameEvents.OnGameLeft+=FunkyOnLeaveGame;
				GameEvents.OnGameChanged+=FunkyOnGameChanged;
				ProfileManager.OnProfileLoaded+=FunkyOnProfileChanged;

				ITargetingProvider newCombatTargetingProvider=new TrinityCombatTargetingReplacer();
				CombatTargeting.Instance.Provider=newCombatTargetingProvider;
				ITargetingProvider newLootTargetingProvider=new TrinityLootTargetingProvider();
				LootTargeting.Instance.Provider=newLootTargetingProvider;
				ITargetingProvider newObstacleTargetingProvider=new TrinityObstacleTargetingProvider();
				ObstacleTargeting.Instance.Provider=newObstacleTargetingProvider;



				if (!bPluginEnabled&&bot!=null)
				{
					 Logging.Write("WARNING: FunkyBot Plugin is NOT ENABLED. Bot start detected");
					 return;
				}

				if (!initTreeHooks)
				{
					 HookBehaviorTree();
				}
			
				bool isingame=ZetaDia.IsInGame;
				if (isingame&&!BotMain.IsRunning)
				{
					 FunkyOnGameChanged(null, null);
				}


				

				Navigator.SearchGridProvider.Update();
		  }
    }
}