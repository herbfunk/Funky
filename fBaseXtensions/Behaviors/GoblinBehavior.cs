using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Common;
using Zeta.Game;
using Zeta.TreeSharp;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fBaseXtensions.Behaviors
{
	public static class GoblinBehavior
	{

		/*
		 * 
		 * 
		 * 
		 * Rainbow Goblin
		   worldid 409093
		   levelareaid 409094
		   
		 * Goblin Realm
WorldID: 379962
LevelAreaID: 380773
		 * 
		 * WorldID: 380753
LevelAreaID: 380774
Name: p1_TGoblin_Realm_BossRoom
The Inner Sanctum
		 */

		internal static bool BehaviorEngaged = false;


		private static CacheObject _portal;
		public static CacheObject Portal
		{
			get { return _portal; }
			set 
			{ 
				 
				if (value!=null)
				{
					if (BlacklistedRAGUIDs.Contains(value.RAGUID))
					{
						_portal = null; 
						return;
					}
						
					
					BehaviorEngaged = true;
					_portal = new CacheObject(value);
				}
			}
		}

		private static List<int> BlacklistedRAGUIDs = new List<int>(); 

		private static Delayer delayer = new Delayer();
		internal static RunStatus Behavior()
		{
			if (ZetaDia.IsLoadingWorld)
				return RunStatus.Running;

			if (FunkyGame.GameIsInvalid) 
				return RunStatus.Success;


			FunkyGame.Hero.Update(false,true);


			//Are we inside goblin world?
			if (FunkyGame.Hero.CurrentWorldID != 409093 &&
				FunkyGame.Hero.CurrentWorldID != 379962 &&
				FunkyGame.Hero.CurrentWorldID != 380774)
			{
				//Already Loaded Profile?
				if (LoadedGoblinProfile)
				{
					ProfileManager.Load(LastUsedProfile);
					ProfileManager.UpdateCurrentProfileBehavior();
					LoadedGoblinProfile = false;
					LastUsedProfile = String.Empty;
					BehaviorEngaged = false;
					BlacklistedRAGUIDs.Add(Portal.RAGUID);
					Portal = null;
					Logger.DBLog.Info("Finished Running Goblin World!");
					return RunStatus.Success;
				}

				if (Portal == null)
				{
					Logger.DBLog.Info("Portal is null!");
					BehaviorEngaged = false;
					return RunStatus.Success;
				}

				if (Portal.WorldID!=FunkyGame.Hero.CurrentWorldID)
				{
					Logger.DBLog.InfoFormat("Portal World ID Mismatched! Portal {0} Hero {1}", Portal.WorldID, FunkyGame.Hero.CurrentWorldID);

					//Not in the same world as portal object!
					Portal = null;
					BehaviorEngaged = false;
					return RunStatus.Success;
				}
				else
				{
					//Move to portal..

					//Handle Targeting..
					if (FunkyGame.Targeting.CheckHandleTarget() == RunStatus.Running)
						return RunStatus.Running;

					if (Portal.CentreDistance>10f)
					{
						Logger.DBLog.Info("Moving To Portal!");
						Navigation.Navigation.NP.MoveTo(Portal.Position, "", true);
						return RunStatus.Running;
					}
					else
					{
						if (Portal.IsStillValid())
						{
							if (!delayer.Test()) return RunStatus.Running;

							Logger.DBLog.Info("At Portal!");
							Portal.ref_DiaObject.Interact();
							return RunStatus.Running;
						}
						else
						{
							Logger.DBLog.Info("Portal No Longer Valid!");
							return RunStatus.Running;
						}
					}
				}
			}
			else
			{
				Logger.DBLog.Info("Inside Goblin World!");
				if (!ProfileManager.CurrentProfile.Path.Contains(FolderPaths.PluginPath + @"\Behaviors\Profiles\"))
				{
					Logger.DBLog.Info("Loading Profile..");
					LastUsedProfile = ProfileManager.CurrentProfile.Path;

					if (FunkyGame.Hero.CurrentWorldID == 409093)
					{
						ProfileManager.Load(FolderPaths.PluginPath + @"\Behaviors\Profiles\Goblin_Whimsydale.xml");
						ProfileManager.UpdateCurrentProfileBehavior();
						LoadedGoblinProfile = true;
					}
					if (FunkyGame.Hero.CurrentWorldID == 379962)
					{
						Logger.DBLog.Info("Profile for world 379962 not supported!");
					}
					if (FunkyGame.Hero.CurrentWorldID == 380774)
					{
						Logger.DBLog.Info("Profile for world 380774 not supported!");
					}
				}
				

				return RunStatus.Success;
			}

			return RunStatus.Running;
		}

		private static string LastUsedProfile = String.Empty;
		private static bool LoadedGoblinProfile = false;

		internal static bool ShouldRunBehavior()
		{
			if (FunkyGame.Hero.CurrentWorldID != 409093 &&
				FunkyGame.Hero.CurrentWorldID != 379962 &&
				FunkyGame.Hero.CurrentWorldID != 380774 &&
				!FunkyGame.Hero.bIsInTown &&
				!BrainBehavior.IsVendoring)
			{
				return true;
			}

			if (!ProfileManager.CurrentProfile.Path.Contains(FolderPaths.PluginPath + @"\Behaviors\Profiles\"))
			{
				return true;
			}

			return false;
		}

		

	}
}
