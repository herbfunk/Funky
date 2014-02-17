using System;
using System.Collections.Generic;
using System.Linq;
using FunkyBot.Cache;
using Zeta.CommonBot.Settings;

namespace FunkyBot.Game
{
	///<summary>
	///Tracker Game and Profile Stats 
	///</summary>
	public class GameStats
	{
		public int TotalGold
		{
			get
			{
				return Profiles.Sum(P => P.TotalGold);
			}
		}
		public int TotalXP
		{
			get
			{
				return Profiles.Sum(P => P.TotalXP);
			}
		}
		public int TotalDeaths
		{
			get
			{
				return Profiles.Sum(P => P.DeathCount);
			}
		}
		public TimeSpan TotalTimeRunning
		{
			get
			{
				var total = new TimeSpan();
				foreach (var P in Profiles)
				{
					if (P.Equals(CurrentProfile))
					{//Current Profile requires Live Data
						P.UpdateRangeVariables();
						P.RestartRangeVariables();
					}
					total += P.TotalTimeSpan;
				}
				return total;
			}
		}
		public List<TrackedProfile> Profiles { get; set; }

		private TrackedProfile currentprofile;
		public TrackedProfile CurrentProfile { get { return currentprofile; } }
		public LootTracking TotalLootTracker
		{
			get
			{
				var total = new LootTracking();
				foreach (var P in Profiles)
				{
					if (P.Equals(CurrentProfile))
					{//Current Profile requires Live Data
						P.UpdateRangeVariables();
						P.RestartRangeVariables();
					}
					total.Merge(P.LootTracker);
				}
				return total;
			}
		}
		public readonly int MonsterPower;
		public GameStats()
		{
			Profiles = new List<TrackedProfile>();
			MonsterPower = Bot.Settings.Demonbuddy.EnableDemonBuddyCharacterSettings ? Bot.Settings.Demonbuddy.MonsterPower : Funky.iDemonbuddyMonsterPowerLevel;
			//note: this will change on first ProfileChanged call!
			currentprofile = new TrackedProfile(GlobalSettings.Instance.LastProfile);
		}

		public void ProfileChanged(string sThisProfile)
		{
			//fresh profile?
			if (Profiles.Count == 0)
			{
				//Lets create a new profile
				Profiles.Add(new TrackedProfile(sThisProfile));

				//Reference it to the Collection
				currentprofile = Profiles[Profiles.Count - 1];
			}

			//Did we really change profiles?
			if (Profiles.Count > 0 && Profiles.Last().ProfileName != sThisProfile)
			{
				//Refresh Profile Target Blacklist 
				BlacklistCache.UpdateProfileBlacklist();

				//Set the "end" date for current profile
				Profiles.Last().UpdateRangeVariables();



				bool foundPreviousEntry = false;
				foreach (TrackedProfile item in Profiles)
				{
					//Found the profile so lets use that instead!
					if (item.ProfileName == sThisProfile)
					{
						foundPreviousEntry = true;
						item.RestartRangeVariables();//reset Starting variables

						//Reference it to the Collection
						currentprofile = item;
						break;
					}
				}


				//Profile was not found.. so lets add it and set it as current.
				if (!foundPreviousEntry)
				{
					//Lets create a new profile
					Profiles.Add(new TrackedProfile(sThisProfile));
					//Reference it to the Collection
					currentprofile = Profiles[Profiles.Count - 1];
				}
			}
		}


		//public string GenerateOutputString()
		//{
		//	string output = String.Format("Total Stats while running\r\nGames:{0} Deaths:{1} Gold:{2} Exp:{3}\r\nTotalTime: {4}\r\n{5}",
		//		Bot.Game.TrackingStats.GameCount,
		//		Bot.Game.TrackingStats.TotalDeaths,
		//		Bot.Game.TrackingStats.TotalGold,
		//		Bot.Game.TrackingStats.TotalXP,
		//		Bot.Game.TrackingStats.TotalTimeRunning.ToString(@"dd\ \d\ hh\ \h\ mm\ \m\ ss\ \s"),
		//		Bot.Game.TrackingStats.TotalLootTracker);

		//	double itemLootPerMin = Math.Round(TotalLootTracker.GetTotalLootStatCount(LootStatTypes.Looted) / TotalTimeRunning.TotalMinutes, 3);
		//	double itemDropPerMin = Math.Round(TotalLootTracker.GetTotalLootStatCount(LootStatTypes.Dropped) / TotalTimeRunning.TotalMinutes, 3);
		//	string PerHour = String.Format("~-~-~-~-~-~-~-~-~-~-~-~-~-~-\r\n" +
		//								  "Drops Per Minute: {0}\r\n" +
		//								  "Loot Per Minute: {1}",
		//								  itemDropPerMin,
		//								  itemLootPerMin);
		//	return String.Format("{0}{1}", output, PerHour);
		//}
	}
}
