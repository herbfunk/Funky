using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Windows.Documents;
using FunkyBot.Cache;
using FunkyBot.Game.Bounty;
using FunkyBot.XMLTags;
using Zeta.Bot.Navigation;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Service;

namespace FunkyBot.Game
{
	//Tracks Stats, Profile related properties, and general in-game info
	public class GameCache
	{
		///<summary>
		///Tracking of All Game Stats 
		///</summary>
		internal TotalStats TrackingStats = new TotalStats();

		///<summary>
		///Tracking of current Game Stats
		///</summary>
		internal GameStats CurrentGameStats = new GameStats();

		internal GoldInactivity GoldTimeoutChecker = new GoldInactivity();

		internal ProfileCache Profile = new ProfileCache();

		internal BountyCache Bounty = new BountyCache();


		public bool AdventureMode { get { return _adventureMode; } }
		private bool _adventureMode = false;

		private GameId _currentGameId = new GameId();
		internal bool RefreshGameId()
		{
			GameId curgameID = _currentGameId;
			int questId = 0;
			using (ZetaDia.Memory.AcquireFrame())
			{
				curgameID = ZetaDia.Service.CurrentGameId;
				questId = ZetaDia.CurrentQuest.QuestSNO;
			}

			if (!curgameID.Equals(_currentGameId))
			{

				Logger.Write(LogLevel.OutOfCombat, "New Game Started");

				ZetaDia.Memory.ClearCache();
				Navigator.SearchGridProvider.Update();

				//Adventure Mode (QuestID == 312429)
				_adventureMode = questId == 312429;
				if (AdventureMode && Bot.Settings.AdventureMode.EnableAdventuringMode)
				{
					Logger.DBLog.Info("Adventure Mode Enabled!");
					Bounty.Reset();
					Bounty.RefreshBountyInfo();
				}

				//Merge last GameStats with the Total
				TrackingStats.GameChanged(ref CurrentGameStats);

				//Create new GameStats
				CurrentGameStats = new GameStats();

				//Update Account Details
				Bot.Character.Account.UpdateCurrentAccountDetails();

				//Clear Interactable Cache
				Profile.InteractableObjectCache.Clear();

				//Clear Health Average
				ObjectCache.Objects.ClearHealthAverageStats();

				//Renew bot
				Funky.ResetBot();

				//Gold Inactivity
				GoldTimeoutChecker.LastCoinageUpdate = DateTime.Now;

				_currentGameId = curgameID;
				return true;
			}

			return false;
		}

		//
		internal void CheckUI()
		{
			if (UI.BountyCompleteContinue != null && UI.BountyCompleteContinue.IsValid && UI.BountyCompleteContinue.IsVisible)
			{
				Logger.DBLog.Info("Funky Clicking Bounty Dialog!");
				UI.BountyCompleteContinue.Click();
			}
		}

		public static int GetWaypointBySNOLevelID(SNOLevelArea id)
		{
			switch(id)
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
