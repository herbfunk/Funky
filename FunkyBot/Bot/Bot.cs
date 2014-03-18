using FunkyBot.DBHandlers;
using FunkyBot.Movement;
using FunkyBot.Settings;
using FunkyBot.Targeting;
using FunkyBot.Player;
using FunkyBot.Game;


namespace FunkyBot
{

	//This class is used to hold the data

	public static class Bot
	{
		public static Settings_Funky Settings = new Settings_Funky();

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
				return (Game.Profile.IsRunningOOCBehavior || ExitGame.BehaviorEngaged || TownPortalBehavior.FunkyTPBehaviorFlag || TownRunManager.bWantToTownRun);
			}
		}

		//Recreate Bot Classes
		public static void Reset()
		{
			Logger.DBLog.InfoFormat("Funky Reseting Bot");
			Character.Reset();
			Targeting = new TargetingClass();
			NavigationCache = new Navigation();
		}

	}

}