using System;
using System.Linq;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Helpers;
using fBaseXtensions.Stats;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Service;
using GameStats = Zeta.Bot.GameStats;

namespace fBaseXtensions.Game
{
	public static class FunkyGame
	{
		public static bool GameIsInvalid
		{
			get 
			{ 
				return !ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || ZetaDia.Me.CommonData == null; 
			}
		}
		public static ActiveHero CurrentActiveHero = new ActiveHero();
		public static GameId CurrentGameID = new GameId();
		public static bool AdventureMode { get; set; }
		public static Profile Profile = new Profile();
		private static ActiveHero _hero = new ActiveHero();
		///<summary>
		///Current Character
		///</summary>
		public static ActiveHero Hero
		{
			get { return _hero; }
			set { _hero = value; }
		}

		///<summary>
		///Tracking of All Game Stats 
		///</summary>
		public static TotalStats TrackingStats = new TotalStats();
		///<summary>
		///Tracking of current Game Stats
		///</summary>
		public static Stats.GameStats CurrentGameStats = new Stats.GameStats();



		internal static bool ShouldRefreshAccountDetails
		{
			set
			{
				ShouldRefreshAccountName = value;
				ShouldRefreshHeroName = value;
				ShouldRefreshClass = value;
				ShouldRefreshDifficulty = value;
				ShouldRefreshHeroLevel = value;
			}
		}

		private static bool ShouldRefreshAccountName = true;
		private static bool ShouldRefreshHeroName = true;
		private static bool ShouldRefreshClass = true;
		private static bool ShouldRefreshDifficulty = true;
		private static bool ShouldRefreshHeroLevel = true;

		private static string _CurrentAccountName = String.Empty;
		public static string CurrentAccountName
		{
			get 
			{
				if (ShouldRefreshAccountName)
				{
					string tmpAccountName = String.Empty;
					using (ZetaDia.Memory.AcquireFrame())
					{
						try
						{
							tmpAccountName = ZetaDia.Service.Hero.BattleTagName;
						}
						catch
						{
							Logger.DBLog.DebugFormat("[Funky] Exception Attempting to Update Current Account Name");
							return _CurrentAccountName;
						}
					}

					if (!tmpAccountName.Contains("#"))
					{
						Logger.DBLog.DebugFormat("[Funky] Account Name Invalid (Missing #) - {0}", tmpAccountName);
						return _CurrentAccountName;
					}
					if (tmpAccountName.Trim() == String.Empty)
					{
						Logger.DBLog.DebugFormat("[Funky] Account Details are Invalid");
						return _CurrentAccountName;
					}

					_CurrentAccountName = tmpAccountName;
					if (BotMain.IsRunning) ShouldRefreshAccountName = false;
				}

				return _CurrentAccountName;
			}
		}

		private static string _CurrentHeroName = String.Empty;
		public static string CurrentHeroName
		{
			get 
			{
				if (ShouldRefreshHeroName)
				{
					string tmpHeroName = String.Empty;
					using (ZetaDia.Memory.AcquireFrame())
					{
						try
						{
							//Get current Names!
							tmpHeroName = ZetaDia.Service.Hero.Name;
						}
						catch
						{
							Logger.DBLog.DebugFormat("[Funky] Exception Attempting to Update Current Hero Name.");
							return _CurrentHeroName;
						}
					}

					//Check the values..
					if (tmpHeroName.ToCharArray().Any(c => !Char.IsLetter(c)))
					{
						Logger.DBLog.DebugFormat("[Funky] Hero Name Invalid (Contains Non-Letter Character) - {0}", tmpHeroName);
						return _CurrentHeroName;
					}
					if (tmpHeroName.Trim() == String.Empty)
					{
						Logger.DBLog.DebugFormat("[Funky] Hero Name is Invalid");
						return _CurrentHeroName;
					}

					_CurrentHeroName = tmpHeroName;
					if (BotMain.IsRunning) ShouldRefreshHeroName = false;
				}

				return _CurrentHeroName; 
			}
		}

		private static ActorClass _CurrentActorClass = ActorClass.Invalid;
		public static ActorClass CurrentActorClass
		{
			get
			{
				if (ShouldRefreshClass)
				{
					ActorClass tmpActorClass = ActorClass.Invalid;
					using (ZetaDia.Memory.AcquireFrame())
					{
						try
						{
							//Get current Names!
							tmpActorClass = ZetaDia.Service.Hero.Class;
						}
						catch
						{
							Logger.DBLog.DebugFormat("[Funky] Exception Attempting to Update Current Actor Class.");
							return _CurrentActorClass;
						}
					}

					if (tmpActorClass==ActorClass.Invalid)
					{
						Logger.DBLog.DebugFormat("[Funky] Invalid Actor Class Returned!");
						return _CurrentActorClass;
					}

					_CurrentActorClass = tmpActorClass;
					if (BotMain.IsRunning) ShouldRefreshClass = false;
				}

				return _CurrentActorClass;
			}
		}

		private static GameDifficulty _CurrentGameDifficulty = GameDifficulty.Normal;
		public static GameDifficulty CurrentGameDifficulty
		{
			get
			{
				if (ShouldRefreshDifficulty)
				{
					GameDifficulty tmpGameDifficulty = GameDifficulty.Normal;
					using (ZetaDia.Memory.AcquireFrame())
					{
						try
						{
							//Get current Names!
							tmpGameDifficulty = ZetaDia.Service.Hero.CurrentDifficulty;
						}
						catch
						{
							Logger.DBLog.DebugFormat("[Funky] Exception Attempting to Update Current Game Difficulty.");
							return _CurrentGameDifficulty;
						}
					}

					_CurrentGameDifficulty = tmpGameDifficulty;
					if (BotMain.IsRunning) ShouldRefreshDifficulty = false;
				}

				return _CurrentGameDifficulty;
			}
		}

		private static int _CurrentHeroLevel = 0;
		public static int CurrentHeroLevel
		{
			get
			{
				if (ShouldRefreshHeroLevel)
				{
					int tmpHeroLevel = 0;
					using (ZetaDia.Memory.AcquireFrame())
					{
						try
						{
							//Get current Names!
							tmpHeroLevel = ZetaDia.Service.Hero.Level;
						}
						catch
						{
							Logger.DBLog.DebugFormat("[Funky] Exception Attempting to Update Current Hero Level.");
							return _CurrentHeroLevel;
						}
					}

					_CurrentHeroLevel = tmpHeroLevel;
					if (BotMain.IsRunning) ShouldRefreshHeroLevel = false;
				}

				return _CurrentHeroLevel;
			}
		}


	}
}
