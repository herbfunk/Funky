using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zeta;

namespace FunkyBot.Game
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

		public int TotalXP { get; set; }
		private int StartingXP;

		public int TotalGold { get; set; }
		private int StartingGold;

        public TrackedProfile(string name)
        {
            DeathCount = 0;
			TotalXP = 0;
			StartingXP = Bot.Character.CurrentExp;
			StartingGold = Bot.Character.Coinage;
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
			TotalXP += (Bot.Character.CurrentExp - StartingXP);
			TotalGold += (Bot.Character.Coinage - StartingGold);
		}
		///<summary>
		///Sets the Starting Values
		///</summary>
		public void RestartRangeVariables()
		{
			DateStartedProfile = DateTime.Now;
			StartingXP = Bot.Character.CurrentExp;
			StartingGold = Bot.Character.Coinage;
		}

		///<summary>
		///Adds the sum of each property of the given TrackedProfile to this instance.
		///</summary>
		public void MergeStats(TrackedProfile other)
		{
			this.TotalGold += other.TotalGold;
			this.TotalXP += other.TotalXP;
			this.DeathCount += other.DeathCount;
			this.TotalTimeSpan = this.TotalTimeSpan.Add(other.TotalTimeSpan);
			this.LootTracker.Merge(other.LootTracker);
		}

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types. 
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                TrackedProfile p = (TrackedProfile)obj;
                return this.ProfileName.Equals(p.ProfileName);
            }
        }

        public override int GetHashCode()
        {
            return this.ProfileName.GetHashCode();
        }

		public static bool Equals(TrackedProfile P, string name)
		{
			//Check for null and compare run-time types. 
			if (P == null)
			{
				return false;
			}
			else
			{
				return P.ProfileName.Equals(name);
			}
		}
    }
}
