using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
				int total = 0;
				foreach(var P in Profiles)
				{
					total += P.TotalGold;
				}
				return total;
			}
		}
		public int TotalXP
		{
			get
			{
				int total = 0;
				foreach (var P in Profiles)
				{
					total += P.TotalXP;
				}
				return total;
			}
		}
		public int TotalDeaths
		{
			get
			{
				int total = 0;
				foreach (var P in Profiles)
				{
					total += P.DeathCount;
				}
				return total;
			}
		}
		public TimeSpan TotalTimeRunning
		{
			get
			{
				TimeSpan total = new TimeSpan();
				foreach (var P in Profiles)
				{
					if (P==CurrentProfile)
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
				LootTracking total = new LootTracking();
				foreach (var P in Profiles)
				{
					if (P == CurrentProfile)
					{//Current Profile requires Live Data
						P.UpdateRangeVariables();
						P.RestartRangeVariables();
					}
					total.Merge(P.LootTracker);
				}
				return total;
			}
		}
		public GameStats()
		{
			Profiles = new List<TrackedProfile>();

			//note: this will change on first ProfileChanged call!
			currentprofile = new TrackedProfile(Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile);
		}

		public void ProfileChanged(string sThisProfile)
		{
			//fresh profile?
			if (this.Profiles.Count == 0)
			{
				//Lets create a new profile
				this.Profiles.Add(new TrackedProfile(sThisProfile));

				//Reference it to the Collection
				this.currentprofile = this.Profiles[this.Profiles.Count - 1];
			}

			//Did we really change profiles?
			if (this.Profiles.Count > 0 && this.Profiles.Last().ProfileName != sThisProfile)
			{
				//Refresh Profile Target Blacklist 
				FunkyBot.Cache.BlacklistCache.UpdateProfileBlacklist();

				//Set the "end" date for current profile
				this.Profiles.Last().UpdateRangeVariables();



				bool foundPreviousEntry = false;
				foreach (TrackedProfile item in this.Profiles)
				{
					//Found the profile so lets use that instead!
					if (item.ProfileName == sThisProfile)
					{
						foundPreviousEntry = true;
						item.RestartRangeVariables();//reset Starting variables

						//Reference it to the Collection
						this.currentprofile = item;
						break;
					}
				}


				//Profile was not found.. so lets add it and set it as current.
				if (!foundPreviousEntry)
				{
					//Lets create a new profile
					this.Profiles.Add(new TrackedProfile(sThisProfile));
					//Reference it to the Collection
					this.currentprofile = this.Profiles[this.Profiles.Count - 1];
				}
			}
		}


		public string GenerateOutputString()
		{
			string output = String.Format("Total Stats while running\r\nGames:{0} Deaths:{1} Gold:{2} Exp:{3}\r\nTotalTime: {4}\r\n{5}",
				Bot.Game.TrackingStats.GameCount,
				Bot.Game.TrackingStats.TotalDeaths,
				Bot.Game.TrackingStats.TotalGold,
				Bot.Game.TrackingStats.TotalXP,
				Bot.Game.TrackingStats.TotalTimeRunning.ToString(@"dd\ \d\ hh\ \h\ mm\ \m\ ss\ \s"),
				Bot.Game.TrackingStats.TotalLootTracker.ToString());

			double itemLootPerMin = Math.Round(TotalLootTracker.GetTotalLootStatCount(LootStatTypes.Looted) / TotalTimeRunning.TotalMinutes, 3);
			double itemDropPerMin = Math.Round(TotalLootTracker.GetTotalLootStatCount(LootStatTypes.Dropped) / TotalTimeRunning.TotalMinutes, 3);
			string PerHour = String.Format("~-~-~-~-~-~-~-~-~-~-~-~-~-~-\r\n" +
										  "Drops Per Minute: {0}\r\n" +
										  "Loot Per Minute: {1}",
										  itemDropPerMin.ToString(),
										  itemLootPerMin.ToString());
			return String.Format("{0}{1}", output, PerHour);
		}
	}
}
