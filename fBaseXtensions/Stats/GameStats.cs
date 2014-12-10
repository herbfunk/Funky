using System;
using System.Collections.Generic;
using System.Linq;
using fBaseXtensions.Game;
using Zeta.Bot.Settings;

namespace fBaseXtensions.Stats
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
		public int TotalHoradricCacheOpened
		{
			get
			{
				return Profiles.Sum(P => P.HoradricCacheOpened);
			}
		}
		public int TotalItemsGambled
		{
			get
			{
				return Profiles.Sum(P => P.ItemsGambled);
			}
		}
		public int TotalTownRuns
		{
			get
			{
				return Profiles.Sum(P => P.TownRuns);
			}
		}
		public int TotalBountiesCompleted
		{
			get
			{
				return Profiles.Sum(P => P.BountiesCompleted);
			}
		}
        public int TotalRiftBossKills
        {
            get
            {
                return Profiles.Sum(P => P.RiftBossKills);
            }
        }
        public int TotalRiftTrialsCompleted
        {
            get
            {
                return Profiles.Sum(P => P.RiftTrialsCompleted);
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

	    internal TrackedProfile GetFirstProfile()
	    {
            if (Profiles.Count>0)
	            return Profiles.OrderBy(p => p.DateStartedProfile_Real).First();

	        return null;
	    }

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
			//MonsterPower = FunkyBaseExtension.Settings.Demonbuddy.EnableDemonBuddyCharacterSettings ? FunkyBaseExtension.Settings.Demonbuddy.MonsterPower : Funky.iDemonbuddyMonsterPowerLevel;
			//note: this will change on first ProfileChanged call!
			string profileName = "none";

			if (GlobalSettings.Instance.LastProfile != null)
				profileName = GlobalSettings.Instance.LastProfile;

			currentprofile = new TrackedProfile(profileName);
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
				FunkyGame.Profile.UpdateProfileBlacklist();

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
				else
				{//Reorder the profiles!

				    int previousProfileIndex = Profiles.IndexOf(currentprofile);
				    TrackedProfile[] clone_profiles = new TrackedProfile[Profiles.Count];
				    Profiles.CopyTo(clone_profiles, 0);

                    //Set last profile to current!
                    Profiles[Profiles.Count-1] = currentprofile;

                    //Reorder..
                    for (int i = previousProfileIndex+1; i < Profiles.Count - 1; i++)
                    {
                        Profiles[i-1] = clone_profiles[i];
                    }
				}

			    TotalStats.WriteProfileTrackerOutput(ref FunkyGame.TrackingStats);
			}
		}


	}
}
