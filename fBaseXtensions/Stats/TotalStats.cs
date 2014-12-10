using System;
using System.IO;
using System.Linq;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;

namespace fBaseXtensions.Stats
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

			WriteProfileTrackerOutput(ref FunkyGame.TrackingStats);
		}
		public void GameStopped(ref GameStats lastGame)
		{
			if (lastGame.Profiles.Count > 0)
			{
				UpdateTotals(ref lastGame);
			}
			WriteProfileTrackerOutput(ref FunkyGame.TrackingStats);
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

		internal bool _createdOldFile = false;

		internal string GenerateOutputString()
		{
			LootTracking totalloottracker = TotalLootTracker;
			return String.Format("Games:{0}" +
								 "\r\nTime {3}" +
								 "\r\nUnique Profiles:{1}" +
								 "\r\nDeaths:{2} ({7} dph)" +
								 "\r\nGold:{4} ({8} gph) -- EXP:{5} ({9} xph)" +
								 "\r\nTown Runs: {17}  Items Gambled: {16}  Horadric Cache Opened: {15}" +
								 "\r\nBounties Completed: {18}" +
			                     "\r\nRifts Completed: {19} Trials Completed: {20}" +
								 "\r\n{6}" +
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
							 (totalloottracker.GetTotalLootStatCount(LootStatTypes.Salvaged) / TotalTimeRunning.TotalHours).ToString("#.##"),
							 TotalHoradricCacheOpened, TotalItemsGambled, TotalTownRuns, 
                             TotalBountiesCompleted, TotalRiftBossKills, TotalRiftTrialsCompleted);
		}
		internal static void WriteProfileTrackerOutput(ref TotalStats stats)
		{
			string outputPath = FolderPaths.LoggingFolderPath + @"\ProfileStats.log";

			//Copy last profile stats file _old_
			if (!stats._createdOldFile)
			{
				if (File.Exists(outputPath))
				{
					if (File.Exists(FolderPaths.LoggingFolderPath + @"\old_ProfileStats.log"))
						File.Delete(FolderPaths.LoggingFolderPath + @"\old_ProfileStats.log");

					File.Copy(outputPath, FolderPaths.LoggingFolderPath + @"\old_ProfileStats.log");
				}

				stats._createdOldFile = true;
			}

			try
			{
				try
				{
					using (StreamWriter LogWriter = new StreamWriter(outputPath, false))
					{
						LogWriter.WriteLine("====================");
						LogWriter.WriteLine("== TOTAL SUMMARY ==");


						LogWriter.WriteLine(stats.GenerateOutputString());
						//LogWriter.WriteLine("Total Games:{0} -- Total Unique Profiles:{1}\r\nDeaths:{2} TotalTime:{3} TotalGold:{4} TotalXP:{5}\r\n{6}",
						//	all.GameCount, all.Profiles.Count, all.TotalDeaths, all.TotalTimeRunning.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"), all.TotalGold, all.TotalXP, all.TotalLootTracker);

						LogWriter.WriteLine("====================");
						LogWriter.WriteLine("== PROFILE SUMMARY ==");
						foreach (var item in stats.Profiles)
						{
							LogWriter.WriteLine(item.GenerateOutput());
							//LogWriter.WriteLine("{0}\r\nDeaths:{1} TotalTime:{2} TotalGold:{3} TotalXP:{4}\r\n{5}",
							//	item.ProfileName, item.DeathCount, item.TotalTimeSpan.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"), item.TotalGold, item.TotalXP, item.LootTracker);
						}
					}

				}
				catch (IOException)
				{
					Logger.DBLog.Info("Fatal Error: File access error for Stats log file.");
				}
			}
			catch (Exception ex)
			{
                Logger.DBLog.InfoFormat("Fatal Error:Stats log file\r\n{0}\r\n{1}",
                                            ex.Message, ex.StackTrace);
			}
		}
	}
}
