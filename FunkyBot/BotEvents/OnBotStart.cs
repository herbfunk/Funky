using System;
using FunkyBot.Settings;
using Zeta;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.Actors;
using System.Threading;
using Zeta.CommonBot;
using Zeta.TreeSharp;
using Zeta.Navigation;
using FunkyBot.Cache;

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
				GameEvents.OnPlayerDied+=new EventHandler<EventArgs>(FunkyOnDeath);
				GameEvents.OnGameJoined+=new EventHandler<EventArgs>(FunkyOnJoinGame);
				GameEvents.OnGameLeft+=new EventHandler<EventArgs>(FunkyOnLeaveGame);
				GameEvents.OnGameChanged+=new EventHandler<EventArgs>(FunkyOnGameChanged);
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
				if (isingame&&!Zeta.CommonBot.BotMain.IsRunning)
				{
					 FunkyOnGameChanged(null, null);
				}


				Bot.Reset();

				Navigator.SearchGridProvider.Update();
		  }
    }
}