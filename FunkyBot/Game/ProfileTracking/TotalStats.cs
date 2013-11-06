using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunkyBot.Game
{
    public class TotalStats
    {
        //The profiles of current game
        public static HashSet<TrackedProfile> ProfilesTracked = new HashSet<TrackedProfile>();
        //Current profile being used
        public static TrackedProfile CurrentTrackingProfile = new TrackedProfile("null");

		public int TotalGold { get; set; }
		public int TotalXP { get; set; }
        public int GameCount { get; set; }
        public int TotalDeaths { get; set; }
        public TimeSpan TotalTimeRunning { get; set; }
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
        public LootTracking TotalLootTracker { get; set; }

        public TotalStats()
        {
			TotalGold = 0;
			TotalXP = 0;
            GameCount = 0;
            TotalDeaths = 0;
            TotalTimeRunning = new TimeSpan();
            TotalLootTracker = new LootTracking();
        }

        public void ProfileChanged(string sThisProfile)
        {
            //Did we really change profiles?
            if (CurrentTrackingProfile.ProfileName != sThisProfile)
            {
                //Refresh Profile Target Blacklist 
                FunkyBot.Cache.BlacklistCache.UpdateProfileBlacklist();

                //Set the "end" date for current profile
                CurrentTrackingProfile.UpdateRangeVariables();

                //Lets create a new profile to test with..
                TrackedProfile changedProfile = new TrackedProfile(sThisProfile);

                bool foundPreviousEntry = false;
                foreach (TrackedProfile item in ProfilesTracked)
                {
                    //Found the profile so lets use that instead!
                    if (item.ProfileName == sThisProfile)
                    {
                        foundPreviousEntry = true;
						item.RestartRangeVariables();//reset start date
                        CurrentTrackingProfile = item;
                        break;
                    }
                }

                //Profile was not found.. so lets add it and set it as current.
                if (!foundPreviousEntry)
                {
                    ProfilesTracked.Add(changedProfile);
                    CurrentTrackingProfile = changedProfile;
                }
            }
        }

        public void GameChanged()
        {
            this.GameCount++;

            //TODO:: Finalize Stats of Profiles Tracked
            if (ProfilesTracked.Count > 0)
            {
                foreach (var item in ProfilesTracked)
                {
					this.TotalGold += item.TotalGold;
					this.TotalXP += item.TotalXP;
                    this.TotalDeaths += item.DeathCount;
                    this.TotalTimeRunning = this.TotalTimeRunning.Add(item.TotalTimeSpan);
                    this.TotalLootTracker.Merge(item.LootTracker);
                }
            }
			ProfilesTracked.Clear();
			Logger.WriteProfileTrackerOutput();
        }
    }
}
