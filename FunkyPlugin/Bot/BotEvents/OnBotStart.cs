﻿using System;
using Zeta;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.Actors;
using System.Threading;
using Zeta.CommonBot;
using Zeta.TreeSharp;
using Zeta.Navigation;

namespace FunkyTrinity
{
    public partial class Funky
    {
		  private void FunkyBotStart(IBot bot)
		  {
				string FunkySettingsPath=System.IO.Path.Combine(FolderPaths.sDemonBuddyPath, "Settings", "FunkyTrinity");
				if (!System.IO.Directory.Exists(FunkySettingsPath))
				{
					 Logging.WriteDiagnostic("Creating Settings Folder at location {0}", FunkySettingsPath);
					 System.IO.Directory.CreateDirectory(FunkySettingsPath);
				}

				Logging.WriteDiagnostic("[Funky] Plugin settings location="+FunkySettingsPath);

				//LoadConfiguration();
				LoadFunkyConfiguration();
				ItemRulesEval=new Interpreter();

				Navigator.PlayerMover=new PlayerMover();
				Navigator.StuckHandler=new TrinityStuckHandler();

				GameEvents.OnPlayerDied+=new EventHandler<EventArgs>(FunkyOnDeath);
				GameEvents.OnGameJoined+=new EventHandler<EventArgs>(FunkyOnJoinGame);
				GameEvents.OnGameLeft+=new EventHandler<EventArgs>(FunkyOnLeaveGame);
				GameEvents.OnGameChanged+=new EventHandler<EventArgs>(FunkyOnGameChanged);
				GameEvents.OnLevelUp+=new EventHandler<EventArgs>(OnPlayerLevelUp);

				ITargetingProvider newCombatTargetingProvider=new TrinityCombatTargetingReplacer();
				CombatTargeting.Instance.Provider=newCombatTargetingProvider;
				ITargetingProvider newLootTargetingProvider=new TrinityLootTargetingProvider();
				LootTargeting.Instance.Provider=newLootTargetingProvider;
				ITargetingProvider newObstacleTargetingProvider=new TrinityObstacleTargetingProvider();
				ObstacleTargeting.Instance.Provider=newObstacleTargetingProvider;

				


				if (!bPluginEnabled&&bot!=null)
				{
					 Logging.Write("WARNING: FunkyTrinity Plugin is NOT ENABLED. Bot start detected");
					 return;
				}

				

				// Recording of all the XML's in use this run

					 string sThisProfile=Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile;
					 if (sThisProfile!=sLastProfileSeen)
					 {
						  listProfilesLoaded.Add(sThisProfile);
						  sLastProfileSeen=sThisProfile;
						  if (String.IsNullOrEmpty(sFirstProfileSeen))
								sFirstProfileSeen=sThisProfile;
					 }

					 //herbfunk stats
					 Statistics.ProfileStats.CurrentProfile=new ProfileStatisics.ProfileStats(sThisProfile);



				if (!bMaintainStatTracking)
				{
					 ItemStatsWhenStartedBot=DateTime.Now;
					 ItemStatsLastPostedReport=DateTime.Now;
					 bMaintainStatTracking=true;
				}
				else
				{
					 Log("Note: Maintaining item stats from previous run. To reset stats fully, please restart DB.");
				}



				if (!initTreeHooks)
				{
					 HookBehaviorTree();
				}


				if (ZetaDia.IsInGame&&!Zeta.CommonBot.BotMain.IsRunning)
				{
					 FunkyOnGameChanged(null, null);
				}

				Bot.Reset();
		  }
    }
}