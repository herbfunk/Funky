using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunkyBot.ProfileTracking
{
    public class TrackedProfile
    {
        public string ProfileName { get; set; }

        public DateTime DateStartedProfile;
        public TimeSpan TotalTimeSpan { get; set; }
        public int DeathCount { get; set; }
        public LootTracking LootTracker { get; set; }

        public TrackedProfile(string name)
        {
            DeathCount = 0;
            ProfileName = name;
            DateStartedProfile = DateTime.Now;
            LootTracker = new LootTracking();
            TotalTimeSpan = new TimeSpan();
        }

        public void UpdateTotalTimeSpan()
        {
            TotalTimeSpan=TotalTimeSpan.Add(DateTime.Now.Subtract(DateStartedProfile));
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
