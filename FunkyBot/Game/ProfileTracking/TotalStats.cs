using System.Linq;

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
				GameCount++;
				lastGame.Profiles.Last().UpdateRangeVariables();
				foreach (var item in lastGame.Profiles)
				{
					if (!Profiles.Contains(item))
					{
						Profiles.Add(item);
					}
					else
					{
						Profiles[Profiles.IndexOf(item)].MergeStats(item);
					}
				}
			}

			//Logger.WriteProfileTrackerOutput();
        }
    }
}
