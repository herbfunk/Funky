using System;
using fBaseXtensions.Game;

namespace fBaseXtensions.Stats
{
	///<summary>
	///Single Profile Stats
	///</summary>
	public class TrackedProfile
	{
		public string ProfileName { get; set; }

		private DateTime DateStartedProfile;
        internal readonly DateTime DateStartedProfile_Real;
		public TimeSpan TotalTimeSpan { get; set; }

		public int DeathCount { get; set; }
		public LootTracking LootTracker { get; set; }

		public int BountiesCompleted { get; set; }
		public int HoradricCacheOpened { get; set; }
		public int ItemsGambled { get; set; }
		public int TownRuns { get; set; }
        public int RiftBossKills { get; set; }
        public int RiftTrialsCompleted { get; set; }


		public TrackedProfile(string name)
		{
			TownRuns = 0;
			ItemsGambled = 0;
			HoradricCacheOpened = 0;
			DeathCount = 0;
            //TotalXP = 0;
            //TotalGold = 0;

            //StartingXP = FunkyGame.Hero.CurrentExp;
            //StartingGold = FunkyGame.Hero.Coinage;
			ProfileName = name;
			DateStartedProfile = DateTime.Now;
		    DateStartedProfile_Real = DateTime.Now;
			LootTracker = new LootTracking();
			TotalTimeSpan = new TimeSpan();
		}

		///<summary>
		///Adds to the total values using Starting Values and Right Now Values
		///</summary>
		public void UpdateRangeVariables()
		{
			TotalTimeSpan = TotalTimeSpan.Add(DateTime.Now.Subtract(DateStartedProfile));
            //TotalXP += (FunkyGame.Hero.CurrentExp - StartingXP);
            //TotalGold += (FunkyGame.Hero.Coinage - StartingGold);
		}
		///<summary>
		///Sets the Starting Values
		///</summary>
		public void RestartRangeVariables()
		{
			DateStartedProfile = DateTime.Now;
            //StartingXP = FunkyGame.Hero.CurrentExp;
            //StartingGold = FunkyGame.Hero.Coinage;
		}

		///<summary>
		///Adds the sum of each property of the given TrackedProfile to this instance.
		///</summary>
        //public void MergeStats(TrackedProfile other)
        //{
        //    TownRuns += other.TownRuns;
        //    ItemsGambled += other.ItemsGambled;
        //    HoradricCacheOpened += other.HoradricCacheOpened;
        //    BountiesCompleted += other.BountiesCompleted;
        //    TotalGold += other.TotalGold;
        //    TotalXP += other.TotalXP;
        //    DeathCount += other.DeathCount;
        //    RiftBossKills += other.RiftBossKills;
        //    RiftTrialsCompleted += other.RiftTrialsCompleted;
        //    TotalTimeSpan = TotalTimeSpan.Add(other.TotalTimeSpan);
        //    LootTracker.Merge(other.LootTracker);
        //}

		public string GenerateOutput()
		{
			return String.Format("{0} TotalTime:{2}" +
			                     "\r\nDeaths:{1} ({4} dph)" +
								"\r\nTotal Town Runs: {7}" +
								"\r\nHoradric Caches Opened: {5}" +
								"\r\nItems Gambled: {6}" +
								"\r\n{3}",
								ProfileName,
								DeathCount,
								TotalTimeSpan.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"),
								LootTracker,
								(DeathCount / TotalTimeSpan.TotalHours).ToString("#.##"),
								HoradricCacheOpened, ItemsGambled, TownRuns);
		}

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			var p = (TrackedProfile)obj;
			return ProfileName.Equals(p.ProfileName);
		}

		public override int GetHashCode()
		{
			return ProfileName.GetHashCode();
		}

		public static bool Equals(TrackedProfile P, string name)
		{
			//Check for null and compare run-time types. 
			if (P == null)
			{
				return false;
			}
			return P.ProfileName.Equals(name);
		}
	}
}
