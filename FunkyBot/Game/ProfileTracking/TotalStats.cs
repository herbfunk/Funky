using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunkyBot.Game
{
	///<summary>
	///Extended GameStats to merge stats as Container
	///</summary>
    public class TotalStats : GameStats
    {
        public int GameCount { get; set; }

        public void GameChanged(ref GameStats lastGame)
        {
			if (lastGame.Profiles.Count > 0)
			{
				this.GameCount++;
				lastGame.Profiles.Last().UpdateRangeVariables();
				foreach (var item in lastGame.Profiles)
				{
					if (!this.Profiles.Contains(item))
					{
						this.Profiles.Add(item);
					}
					else
					{
						this.Profiles[this.Profiles.IndexOf(item)].MergeStats(item);
					}
				}
			}

			Logger.WriteProfileTrackerOutput();
        }
    }
}
