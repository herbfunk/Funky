using System.Linq;
using fBaseXtensions.Behaviors;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Settings;
using Zeta.Bot.Profile.Common;
using Zeta.Common;
using Zeta.Game;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;

namespace fBaseXtensions.Game
{
	//Tracks Stats, Profile related properties, and general in-game info
	public class GameCache
	{
		public GameCache()
		{
			QuestMode = false;
			ShouldNavigateMinimapPoints = false;
			AllowAnyUnitForLOSMovement = false;
		}

		internal bool QuestMode { get; set; }
		internal bool ShouldNavigateMinimapPoints { get; set; }
		internal bool AllowAnyUnitForLOSMovement { get; set; }
		public void ResetCombatModifiers()
		{
			SettingCluster.ClusterSettingsTag = FunkyBaseExtension.Settings.Cluster;
			SettingLOSMovement.LOSSettingsTag = FunkyBaseExtension.Settings.LOSMovement;
			QuestMode = false;
			AllowAnyUnitForLOSMovement = false;
			ShouldNavigateMinimapPoints = false;
		}
		public bool ShouldNavigatePointsOfInterest
		{
			get
			{

				return FunkyGame.AdventureMode && (FunkyBaseExtension.Settings.AdventureMode.NavigatePointsOfInterest || ShouldNavigateMinimapPoints) && FunkyGame.Profile.CurrentProfileBehaviorType == Profile.ProfileBehaviorTypes.ExploreDungeon;
			}
		}


		internal static void GoldInactivityTimerTrippedHandler()
		{
			Logger.DBLog.Info("[Funky] Gold Timeout Breached.. enabling exit behavior!");
			ExitGame.ShouldExitGame = true;
		}

		
		internal CacheObject InteractableCachedObject { get; set; }
		internal void OnProfileBehaviorChanged(Profile.ProfileBehaviorTypes type)
		{
			if (type == Profile.ProfileBehaviorTypes.Unknown)
			{
				InteractableCachedObject = null;

				return;
			}

			if (type == Profile.ProfileBehaviorTypes.SetQuestMode)
				QuestMode = true;
			else if(type.HasFlag(Profile.ProfileBehaviorTypes.Interactive))
			{
				Logger.DBLog.DebugFormat("Interactable Profile Tag!");

				InteractableCachedObject = GetInteractiveCachedObject(type);
				if (InteractableCachedObject != null)
					Logger.DBLog.DebugFormat("Found Cached Interactable Server Object");
			}
		}

		internal CacheObject GetInteractiveCachedObject(Profile.ProfileBehaviorTypes type)
		{

			if (type == Profile.ProfileBehaviorTypes.UseWaypoint)
			{
				UseWaypointTag tagWP = (UseWaypointTag)FunkyGame.Profile.CurrentProfileBehavior;
				var WaypointObjects = ObjectCache.InteractableObjectCache.Values.Where(obj => obj.SNOID == 6442);
				foreach (CacheObject item in WaypointObjects)
				{
					if (item.Position.Distance(tagWP.Position) < 100f)
					{
						//Found matching waypoint object!
						return item;
					}
				}
			}
			else if (type == Profile.ProfileBehaviorTypes.UseObject)
			{
				UseObjectTag tagUseObj = (UseObjectTag)FunkyGame.Profile.CurrentProfileBehavior;
				if (tagUseObj.ActorId > 0)
				{//Using SNOID..
					var Objects = ObjectCache.InteractableObjectCache.Values.Where(obj => obj.SNOID == tagUseObj.ActorId);
					foreach (CacheObject item in Objects.OrderBy(obj => obj.Position.Distance(FunkyGame.Hero.Position)))
					{
						//Found matching object!
						return item;
					}

				}
				else
				{//use position to match object
					Vector3 tagPosition = tagUseObj.Position;
					var Objects = ObjectCache.InteractableObjectCache.Values.Where(obj => obj.Position.Distance(tagPosition) <= 100f);
					foreach (CacheObject item in Objects)
					{
						//Found matching object!
						return item;
					}
				}
			}
			else if (type == Profile.ProfileBehaviorTypes.UsePortal)
			{
				UsePortalTag tagUsePortal = (UsePortalTag)FunkyGame.Profile.CurrentProfileBehavior;
				if (tagUsePortal.ActorId > 0)
				{//Using SNOID..
					var Objects = ObjectCache.InteractableObjectCache.Values.Where(obj => obj.SNOID == tagUsePortal.ActorId);
					foreach (CacheObject item in Objects.OrderBy(obj => obj.Position.Distance(FunkyGame.Hero.Position)))
					{
						//Found matching object!
						return item;
					}

				}
				else
				{//use position to match object
					Vector3 tagPosition = tagUsePortal.Position;
					var Objects = ObjectCache.InteractableObjectCache.Values.Where(obj => obj.Position.Distance(tagPosition) <= 100f);
					foreach (CacheObject item in Objects.OrderBy(obj => obj.Position.Distance(FunkyGame.Hero.Position)))
					{
						//Found matching object!
						return item;
					}
				}
			}


			return null;
		}


		internal void OnGameIDChangedHandler()
		{
			Logger.Write(LogLevel.OutOfCombat, "New Game Started");


			if (FunkyGame.AdventureMode && FunkyBaseExtension.Settings.AdventureMode.EnableAdventuringMode)
			{
				ResetCombatModifiers();
			}


			//Clear Interactable Cache
			ObjectCache.InteractableObjectCache.Clear();

			//Clear Health Average
			ObjectCache.Objects.ClearHealthAverageStats();

			//Renew bot
			FunkyGame.ResetBot();


		}

		public static int GetWaypointBySNOLevelID(SNOLevelArea id)
		{
			switch (id)
			{
				case SNOLevelArea.A1_trOut_Old_Tristram_Road_Cath:
					return 1;
				case SNOLevelArea.A1_trDun_Level01:
					return 2;
				case SNOLevelArea.A1_trDun_Level04:
					return 3;
				case SNOLevelArea.A1_trDun_Level06:
					return 4;
				case SNOLevelArea.A1_trDun_Level07B:
					return 5;
				case SNOLevelArea.A1_trOut_TristramWilderness:
					return 6;
				case SNOLevelArea.A1_trOut_Wilderness_BurialGrounds:
					return 7;
				case SNOLevelArea.A1_trOut_TristramFields_A:
					return 9;
				case SNOLevelArea.A1_trOut_TristramFields_B:
					return 10;
				case SNOLevelArea.A1_trOut_TownAttack_ChapelCellar:
					return 11;
				case SNOLevelArea.A1_C6_SpiderCave_01_Main:
					return 12;
				case SNOLevelArea.A1_trOUT_Highlands_Bridge:
					return 13;
				case SNOLevelArea.A1_trOUT_Highlands2:
					return 14;
				case SNOLevelArea.A1_trDun_Leoric01:
					return 15;
				case SNOLevelArea.A1_trDun_Leoric02:
					return 16;
				case SNOLevelArea.A1_trDun_Leoric03:
					return 17;

				case SNOLevelArea.A2_caOUT_StingingWinds_Canyon:
					return 19;
				case SNOLevelArea.A2_Caldeum_Uprising:
					return 20;
				case SNOLevelArea.A2_caOUT_BorderlandsKhamsin:
					return 21;
				case SNOLevelArea.A2_caOUT_StingingWinds:
					return 22;
				case SNOLevelArea.A2_caOut_Oasis:
					return 23;
				case SNOLevelArea.A2_caOUT_Boneyard_01:
					return 24;
				case SNOLevelArea.A2_Dun_Zolt_Lobby:
					return 25;

				case SNOLevelArea.A3_dun_rmpt_Level02:
					return 27;
				case SNOLevelArea.A3_Dun_Keep_Level03:
					return 28;
				case SNOLevelArea.A3_Dun_Keep_Level04:
					return 29;
				case SNOLevelArea.A3_Dun_Keep_Level05:
					return 30;
				case SNOLevelArea.A3_Dun_Battlefield_Gate:
					return 31;
				case SNOLevelArea.A3_Bridge_Choke_A:
					return 32;
				case SNOLevelArea.A3_Battlefield_B:
					return 33;
				case SNOLevelArea.A3_Dun_Crater_Level_01:
					return 34;
				case SNOLevelArea.A3_dun_Crater_ST_Level01:
					return 35;
				case SNOLevelArea.A3_Dun_Crater_Level_02:
					return 36;
				case SNOLevelArea.A3_dun_Crater_ST_Level01B:
					return 37;
				case SNOLevelArea.A3_Dun_Crater_Level_03:
					return 38;

				case SNOLevelArea.A4_dun_Heaven_1000_Monsters_Fight_Entrance:
					return 40;
				case SNOLevelArea.A4_dun_Garden_of_Hope_01:
					return 41;
				case SNOLevelArea.A4_dun_Garden_of_Hope_02:
					return 42;
				case SNOLevelArea.A4_dun_Hell_Portal_01:
					return 43;
				case SNOLevelArea.A4_dun_Spire_01:
					return 44;
				case SNOLevelArea.A4_dun_Spire_02:
					return 45;

				case SNOLevelArea.X1_WESTM_ZONE_01:
					return 47;
				case SNOLevelArea.X1_Westm_Graveyard_DeathOrb:
					return 48;
				case SNOLevelArea.X1_WESTM_ZONE_03:
					return 49;
				case SNOLevelArea.x1_Bog_01_Part2:
					return 50;
				case SNOLevelArea.x1_Catacombs_Level01:
					return 51;
				case SNOLevelArea.x1_Catacombs_Level02:
					return 52;
				case SNOLevelArea.x1_fortress_level_01:
					return 53;
				case SNOLevelArea.x1_fortress_level_02_Intro:
					return 54;
				case SNOLevelArea.X1_Pand_Ext_2_Battlefields:
					return 55;
			}

			return -1;
		}
	}
}
