using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunkyBot.ProfileTracking
{
    public class TotalStats
    {
        //The profiles of current game
        public static HashSet<TrackedProfile> ProfilesTracked = new HashSet<TrackedProfile>();
        //Current profile being used
        public static TrackedProfile CurrentTrackingProfile = new TrackedProfile("null");

        public int GameCount { get; set; }
        public int TotalDeaths { get; set; }
        public TimeSpan TotalTimeRunning { get; set; }
        public LootTracking TotalLootTracker { get; set; }

        public TotalStats()
        {
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
                CurrentTrackingProfile.UpdateTotalTimeSpan();

                //Lets create a new profile to test with..
                TrackedProfile changedProfile = new TrackedProfile(sThisProfile);

                bool foundPreviousEntry = false;
                foreach (TrackedProfile item in ProfilesTracked)
                {
                    //Found the profile so lets use that instead!
                    if (item.ProfileName == sThisProfile)
                    {
                        foundPreviousEntry = true;
                        item.DateStartedProfile = DateTime.Now;//reset start date
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
                    this.TotalDeaths += item.DeathCount;
                    this.TotalTimeRunning = this.TotalTimeRunning.Add(item.TotalTimeSpan);
                    this.TotalLootTracker.Merge(item.LootTracker);
                }
            }
            ProfilesTracked = new HashSet<TrackedProfile>();
        }
    }
}
