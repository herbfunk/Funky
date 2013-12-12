using System;
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
				UpdateTotals(ref lastGame);
			}

			Logger.WriteProfileTrackerOutput();
        }
		public void GameStopped(ref GameStats lastGame)
		{
			if (lastGame.Profiles.Count > 0)
			{
				UpdateTotals(ref lastGame);
			}
			Logger.WriteProfileTrackerOutput();
		}

		private void UpdateTotals(ref GameStats lastGame)
		{
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

		internal string GenerateOutputString()
		{
			LootTracking totalloottracker = TotalLootTracker;
			return String.Format("Games:{0}\r\nTime {3}\r\nUnique Profiles:{1}\r\nDeaths:{2} ({7} dph) -- Gold:{4} ({8} gph) -- EXP:{5} ({9} xph)\r\n{6}" +
							 "Drops Per Hour: {10} -- Looted Per Hour: {11}\r\n" + "Stash Per Hour: {12} -- Vendored Per Hour: {13} -- Salvaged Per Hour: {14}",
							 GameCount,
							 Profiles.Count,
							 TotalDeaths,
							 TotalTimeRunning.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"),
							 TotalGold,
							 TotalXP,
							 totalloottracker,
							 (TotalDeaths / TotalTimeRunning.TotalHours).ToString("#.##"),
							 (TotalGold / TotalTimeRunning.TotalHours).ToString("#.##"),
							 (TotalXP / TotalTimeRunning.TotalHours).ToString("#.##"),
							 (totalloottracker.GetTotalLootStatCount(LootStatTypes.Dropped) / TotalTimeRunning.TotalHours).ToString("#.##"),
							 (totalloottracker.GetTotalLootStatCount(LootStatTypes.Looted) / TotalTimeRunning.TotalHours).ToString("#.##"),
							 (totalloottracker.GetTotalLootStatCount(LootStatTypes.Stashed) / TotalTimeRunning.TotalHours).ToString("#.##"),
							 (totalloottracker.GetTotalLootStatCount(LootStatTypes.Vendored) / TotalTimeRunning.TotalHours).ToString("#.##"),
							 (totalloottracker.GetTotalLootStatCount(LootStatTypes.Salvaged) / TotalTimeRunning.TotalHours).ToString("#.##"));
		}
    }
}
