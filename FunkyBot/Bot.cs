using System;
using FunkyBot.Cache;
using FunkyBot.Config.Settings;
using FunkyBot.DBHandlers;
using FunkyBot.DBHandlers.Townrun;
using FunkyBot.Movement;
using FunkyBot.Player.Class;
using FunkyBot.Targeting;
using FunkyBot.Player;
using FunkyBot.Game;
using Zeta.Bot.Logic;
using Zeta.Common;
using Zeta.Game;
using Logger = FunkyBot.Misc.Logger;


namespace FunkyBot
{

	//This class is used to hold the data
	public static class Bot
	{
		public static Settings_Funky Settings { get; set; }

		private static readonly Character character = new Character();
		public static Character Character { get { return character; } }

		public static TargetingClass Targeting { get; set; }

		///<summary>
		///Game Stats and Values of Current Character
		///</summary>
		public static GameCache Game = new GameCache();
		//Initalized once for total stats tracking

		///<summary>
		///Contains movement related properties and methods pretaining to the Bot itself.
		///</summary>
		public static Navigation NavigationCache { get; set; }





		public static bool RunningTargetingBehavior = false;
		///<summary>
		///Checks behavioral flags that are considered OOC/Non-Combat
		///</summary>
		internal static bool IsInNonCombatBehavior
		{
			get
			{
				//OOC IDing, Town Portal Casting, Town Run
				return (Game.Profile.IsRunningOOCBehavior || ExitGame.BehaviorEngaged || TownPortalBehavior.FunkyTPBehaviorFlag || BrainBehavior.IsVendoring);
			}
		}

		public static bool GameIsInvalid()
		{
			return !ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || ZetaDia.Me.CommonData == null;
		}

		//Recreate Bot Classes
		public static void Reset()
		{
			Logger.DBLog.InfoFormat("Funky Reseting Bot");
			Game.Bounty.Reset();
			//TownRunManager.townRunItemCache=new TownRunManager.TownRunCache();
			Character.Reset();
			Settings_Funky.LoadFunkyConfiguration();
			Targeting = new TargetingClass();
			NavigationCache = new Navigation();
		}
		public static void ResetBot()
		{

			Logger.DBLog.InfoFormat("Preforming reset of bot data...");
			BlacklistCache.ClearBlacklistCollections();

			PlayerMover.iTotalAntiStuckAttempts = 1;
			PlayerMover.vSafeMovementLocation = Vector3.Zero;
			PlayerMover.vOldPosition = Vector3.Zero;
			PlayerMover.iTimesReachedStuckPoint = 0;
			PlayerMover.timeLastRecordedPosition = DateTime.Today;
			PlayerMover.timeStartedUnstuckMeasure = DateTime.Today;
			PlayerMover.iTimesReachedMaxUnstucks = 0;
			PlayerMover.iCancelUnstuckerForSeconds = 0;
			PlayerMover.timeCancelledUnstuckerFor = DateTime.Today;

			//Reset all data with bot (Playerdata, Combat Data)
			Reset();

			PlayerClass.CreateBotClass();
			//Update character info!
			Bot.Character.Data.Update();

			//OOC ID Flags
			Bot.Targeting.Cache.ShouldCheckItemLooted = false;
			Bot.Targeting.Cache.CheckItemLootStackCount = 0;
			//ItemIdentifyBehavior.shouldPreformOOCItemIDing = false;

			//TP Behavior Reset
			TownPortalBehavior.ResetTPBehavior();

			//Sno Trim Timer Reset
			ObjectCache.cacheSnoCollection.ResetTrimTimer();
			//clear obstacles
			ObjectCache.Obstacles.Clear();
			ObjectCache.Objects.Clear();
			EventHandlers.EventHandlers.DumpedDeathInfo = false;
		}
		public static void ResetGame()
		{
			SkipAheadCache.ClearCache();
			TownRunManager.TalliedTownRun = false;
			TownPortalBehavior.TownrunStartedInTown = true;
			//TownRunManager._dictItemStashAttempted = new Dictionary<int, int>();
		}
	}

}