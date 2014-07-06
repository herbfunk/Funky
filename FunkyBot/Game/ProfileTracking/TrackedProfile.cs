using System;

namespace FunkyBot.Game.ProfileTracking
{
	///<summary>
	///Single Profile Stats
	///</summary>
	public class TrackedProfile
	{
		public string ProfileName { get; set; }

		private DateTime DateStartedProfile;
		public TimeSpan TotalTimeSpan { get; set; }

		public int DeathCount { get; set; }
		public LootTracking LootTracker { get; set; }

		public int BountiesCompleted { get; set; }
		public int HoradricCacheOpened { get; set; }

		public int TotalXP { get; set; }
		private int StartingXP;

		public int TotalGold { get; set; }
		private int StartingGold;

		public TrackedProfile(string name)
		{
			HoradricCacheOpened = 0;
			BountiesCompleted = 0;
			DeathCount = 0;
			TotalXP = 0;
			StartingXP = Bot.Character.Data.CurrentExp;
			StartingGold = Bot.Character.Data.Coinage;
			ProfileName = name;
			DateStartedProfile = DateTime.Now;
			LootTracker = new LootTracking();
			TotalTimeSpan = new TimeSpan();
		}

		///<summary>
		///Adds to the total values using Starting Values and Right Now Values
		///</summary>
		public void UpdateRangeVariables()
		{
			TotalTimeSpan = TotalTimeSpan.Add(DateTime.Now.Subtract(DateStartedProfile));
			TotalXP += (Bot.Character.Data.CurrentExp - StartingXP);
			TotalGold += (Bot.Character.Data.Coinage - StartingGold);
		}
		///<summary>
		///Sets the Starting Values
		///</summary>
		public void RestartRangeVariables()
		{
			DateStartedProfile = DateTime.Now;
			StartingXP = Bot.Character.Data.CurrentExp;
			StartingGold = Bot.Character.Data.Coinage;
		}

		///<summary>
		///Adds the sum of each property of the given TrackedProfile to this instance.
		///</summary>
		public void MergeStats(TrackedProfile other)
		{
			HoradricCacheOpened += other.HoradricCacheOpened;
			BountiesCompleted += other.BountiesCompleted;
			TotalGold += other.TotalGold;
			TotalXP += other.TotalXP;
			DeathCount += other.DeathCount;
			TotalTimeSpan = TotalTimeSpan.Add(other.TotalTimeSpan);
			LootTracker.Merge(other.LootTracker);
		}

		public string GenerateOutput()
		{
			return String.Format("{0} TotalTime:{2} \r\nDeaths:{1} ({6} dph) TotalGold:{3} ({8} gph) TotalXP:{4} ({7} xph)" +
								"\r\nHoradric Caches Opened: {9}" +
			                     "\r\nBounties Completed: {10}" +
			                    "\r\n{5}",
								ProfileName,
								DeathCount,
								TotalTimeSpan.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"),
								TotalGold,
								TotalXP,
								LootTracker,
								(DeathCount / TotalTimeSpan.TotalHours).ToString("#.##"),
								(TotalXP / TotalTimeSpan.TotalHours).ToString("#.##"),
								(TotalGold / TotalTimeSpan.TotalHours).ToString("#.##"),
								HoradricCacheOpened,  BountiesCompleted);
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
