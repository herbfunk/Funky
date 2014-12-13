using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using Zeta.Bot.Settings;

namespace fBaseXtensions.Stats
{
    public class Stats
    {
        public List<TrackedProfile> Profiles { get; set; }

        public TrackedProfile CurrentProfile { get { return currentprofile; } }
        private TrackedProfile currentprofile;

        public int GameCount { get; set; }

        public readonly string Hero;
       

        public Stats()
        {
            Profiles = new List<TrackedProfile>();
          
            //note: this will change on first ProfileChanged call!
            string profileName = "none";

            if (GlobalSettings.Instance.LastProfile != null)
                profileName = GlobalSettings.Instance.LastProfile;

            currentprofile = new TrackedProfile(profileName);

            Hero = FunkyGame.CurrentHeroName;
        }


        internal TrackedProfile GetFirstProfile()
        {
            if (Profiles.Count > 0)
                return Profiles.OrderBy(p => p.DateStartedProfile_Real).First();

            return null;
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


        public int RiftTrialsCompleted { get; set; }
        public int RiftBossKills { get; set; }
        public int BountiesCompleted { get; set; }

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
            if (Profiles.Count > 0 && currentprofile.ProfileName != sThisProfile)
            {
                //Refresh Profile Target Blacklist 
                FunkyGame.Profile.UpdateProfileBlacklist();

                //Set the "end" date for current profile
                currentprofile.UpdateRangeVariables();



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

                    //Copy profiles
                    TrackedProfile[] clone_profiles = new TrackedProfile[Profiles.Count];
                    Profiles.CopyTo(clone_profiles, 0);

                    //Reorder..
                    for (int i = previousProfileIndex; i < Profiles.Count - 1; i++)
                    {
                        Profiles[i] = clone_profiles[i+1];
                    }

                    //Finally Set the last profile to current!
                    Profiles[Profiles.Count - 1] = currentprofile;
                }

                WriteProfileTrackerOutput(ref FunkyGame.CurrentStats);
            }
        }



        internal bool _createdOldFile = false;
        internal string GenerateOutputString()
        {
            LootTracking totalloottracker = TotalLootTracker;
            return String.Format("Games:{0}" +
                                 "\r\nTime {3}" +
                                 "\r\nUnique Profiles:{1}" +
                                 "\r\nDeaths:{2} ({5} dph)" +
                                 "\r\nTown Runs: {13}  Items Gambled: {12}  Horadric Cache Opened: {11}" +
                                 "\r\nBounties Completed: {14}" +
                                 "\r\nRifts Completed: {15} Trials Completed: {16}" +
                                 "\r\n{4}" +
                             "Drops Per Hour: {6} -- Looted Per Hour: {7}\r\n" + "Stash Per Hour: {8} -- Vendored Per Hour: {9} -- Salvaged Per Hour: {10}",
                             GameCount,
                             Profiles.Count,
                             TotalDeaths,
                             TotalTimeRunning.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"),
                             totalloottracker,
                             (TotalDeaths / TotalTimeRunning.TotalHours).ToString("#.##"),
                             (totalloottracker.GetTotalLootStatCount(LootStatTypes.Dropped) / TotalTimeRunning.TotalHours).ToString("#.##"),
                             (totalloottracker.GetTotalLootStatCount(LootStatTypes.Looted) / TotalTimeRunning.TotalHours).ToString("#.##"),
                             (totalloottracker.GetTotalLootStatCount(LootStatTypes.Stashed) / TotalTimeRunning.TotalHours).ToString("#.##"),
                             (totalloottracker.GetTotalLootStatCount(LootStatTypes.Vendored) / TotalTimeRunning.TotalHours).ToString("#.##"),
                             (totalloottracker.GetTotalLootStatCount(LootStatTypes.Salvaged) / TotalTimeRunning.TotalHours).ToString("#.##"),
                             TotalHoradricCacheOpened, TotalItemsGambled, TotalTownRuns,
                             BountiesCompleted, RiftBossKills, RiftTrialsCompleted);
        }
        internal static void WriteProfileTrackerOutput(ref Stats stats)
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
