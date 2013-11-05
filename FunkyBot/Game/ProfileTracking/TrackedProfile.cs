using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zeta;

namespace FunkyBot.Game
{
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


		public void UpdateRangeVariables()
		{
			TotalTimeSpan = TotalTimeSpan.Add(DateTime.Now.Subtract(DateStartedProfile));

			if (StartingXP > 0)
				TotalXP += (Bot.Character.CurrentExp - StartingXP);
			else
				StartingXP = Bot.Character.CurrentExp;

			if (StartingGold > 0)
				TotalGold += (Bot.Character.Coinage - StartingGold);
			else
			{
				try
				{
					StartingGold = ZetaDia.Me.Inventory.Coinage;
				}
				catch (Exception)
				{
					Logger.Write(LogLevel.Execption, "Failed to get current coinage for tracked profile!");
					StartingGold = Bot.Character.Coinage;
				}
			}
		}
		public void RestartRangeVariables()
		{
			DateStartedProfile = DateTime.Now;
			StartingXP = Bot.Character.CurrentExp;
			StartingGold = Bot.Character.Coinage;
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
    }
}
