using System;
using System.Collections.Generic;
using Zeta;
using FunkyTrinity.Cache;
using System.IO;

namespace FunkyTrinity
{

		  public static partial class Bot
		  {
				public class BotStatistics
				{
					 public BotStatistics()
					 {
						  iMaxDeathsAllowed=0;
						  iDeathsThisRun=0;
						  lastDied=DateTime.Today;
						  iTotalDeaths=0;

						  iTotalJoinGames=0;
						  iTotalLeaveGames=0;
						  iTotalProfileRecycles=0;

						  listProfilesLoaded=new List<string>();
						  sLastProfileSeen=String.Empty;
						  sFirstProfileSeen=String.Empty;
						  lastProfileCheck=DateTime.Today;

						  LastJoinedGame=DateTime.MinValue;

						  ItemStats=new ItemStatistics();
						  GameStats=new GameStatistics();
						  ProfileStats=new ProfileStatisics();
					 }

					 public DateTime LastJoinedGame { get; set; }
					 // Death counts
					 public int iMaxDeathsAllowed { get; set; }
					 public int iDeathsThisRun { get; set; }
					 // On death, clear the timers for all abilities
					 public DateTime lastDied { get; set; }
					 public int iTotalDeaths { get; set; }
					 // How many total leave games, for stat-tracking?
					 public int iTotalJoinGames { get; set; }
					 // How many total leave games, for stat-tracking?
					 public int iTotalLeaveGames { get; set; }
					 public int iTotalProfileRecycles { get; set; }

					 // Related to the profile reloaded when restarting games, to pick the FIRST profile.
					 // Also storing a list of all profiles, for experimental reasons/incase I want to use them down the line
					 public List<string> listProfilesLoaded { get; set; }
					 public string sLastProfileSeen { get; set; }
					 public string sFirstProfileSeen { get; set; }
					 
					 public GameStatistics GameStats { get; set; }
					 public ItemStatistics ItemStats { get; set; }
					 public ProfileStatisics ProfileStats { get; set; }

					 private DateTime lastProfileCheck { get; set; }
					 ///<summary>
					 ///Checks if current profile has changed and updates vars accordingly!
					 ///</summary>
					 public void CheckProfile()
					 {
						  if (DateTime.Now.Subtract(Bot.Stats.lastProfileCheck).TotalMilliseconds>1000)
						  {
								Bot.Stats.lastProfileCheck=DateTime.Now;
								string sThisProfile=Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile;
								if (sThisProfile!=Bot.Stats.sLastProfileSeen)
								{
									 //herbfunk stats
									 Bot.Stats.ProfileStats.UpdateProfileChanged();

									 // See if we appear to have started a new game
									 if (!String.IsNullOrEmpty(Bot.Stats.sFirstProfileSeen)&&sThisProfile==Bot.Stats.sFirstProfileSeen)
									 {
										  Bot.Stats.iTotalProfileRecycles++;
										  if (Bot.Stats.iTotalProfileRecycles>Bot.Stats.iTotalJoinGames&&Bot.Stats.iTotalProfileRecycles>Bot.Stats.iTotalLeaveGames)
										  {
												Funky.Log("Reseting Game Data -- Total Profile Recycles exceedes join and leave count!");
												Funky.ResetGame();
										  }
									 }
									 Bot.Stats.listProfilesLoaded.Add(sThisProfile);
									 Bot.Stats.sLastProfileSeen=sThisProfile;
									 if (String.IsNullOrEmpty(Bot.Stats.sFirstProfileSeen))
										  Bot.Stats.sFirstProfileSeen=sThisProfile;

									 //Refresh Profile Target Blacklist 
									 ObjectCache.hashProfileSNOTargetBlacklist=new HashSet<int>();
									 foreach (var item in Zeta.CommonBot.ProfileManager.CurrentProfile.TargetBlacklists)
									 {
										  ObjectCache.hashProfileSNOTargetBlacklist.Add(item.ActorId);
									 }
								}
						  }
					 }


					 public class ItemStatistics
					 {
						  public ItemStatistics()
						  {
								//misc, magical, rare, legendary
								lootedItemTotals=new int[] { 0, 0, 0, 0, 0, 0, 0 };
								stashedItemTotals=new int[] { 0, 0, 0, 0, 0, 0, 0 };
								timeTracked=new TimeSpan(0, 0, 0, 0);
								droppedItemTotals=new SortedList<int, int[]>();
								CurrentGame=new CurrentGameItemStats();
						  }

						  public int[] lootedItemTotals { get; set; }
						  public int[] stashedItemTotals { get; set; }
						  public SortedList<int, int[]> droppedItemTotals { get; set; }
						  public TimeSpan timeTracked { get; set; }
						  public CurrentGameItemStats CurrentGame { get; set; }

						  public TimeSpan TotalTimeTracked()
						  {
								if (ZetaDia.IsInGame)
								{
									 TimeSpan tmpTimeSpan=timeTracked;
									 return DateTime.Now.Subtract(CurrentGame.TimeTrackingBegan).Add(tmpTimeSpan);
								}

								return timeTracked;
						  }

						  public int[] lootedTotals()
						  {
								//Returns a temp total of looted items.
								int[] tmp_LootedTotals=(int[])lootedItemTotals.Clone();
								int[] tmp_CurrentGameTotals=CurrentGame.temp_LootedTotals();
								for (int i=0; i<6; i++)
								{
									 tmp_LootedTotals[i]+=tmp_CurrentGameTotals[i];
								}

								return tmp_LootedTotals;
						  }

						  public int[] stashedTotals()
						  {
								//Returns a temp total of looted items.
								int[] tmp_StashedTotals=(int[])stashedItemTotals.Clone();
								int[] tmp_CurrentGameTotals=CurrentGame.temp_StashedTotals();

								for (int i=0; i<6; i++)
								{
									 tmp_StashedTotals[i]+=tmp_CurrentGameTotals[i];
								}

								return tmp_StashedTotals;
						  }

						  //public int lootedTOTAL()
						  //{
						  //	 int[] totals = lootedTotals();
						  //	 int count = 0;
						  //	 for (int i = 0; i < 4; i++)
						  //	 {
						  //		  count += totals[i];
						  //	 }

						  //	 return count;
						  //}

						  //public int stashedTOTAL()
						  //{
						  //	 int[] totals = stashedTotals();
						  //	 int count = 0;
						  //	 for (int i = 0; i < 4; i++)
						  //	 {
						  //		  count += totals[i];
						  //	 }

						  //	 return count;
						  //}

						  private void UpdateTotals()
						  {
								for (int i=0; i<6; i++)
								{
									 lootedItemTotals[i]+=CurrentGame.lootedItemTotals[i];
									 stashedItemTotals[i]+=CurrentGame.stashedItemTotals[i];
								}

								//update dropped stats
								foreach (int ilvl in CurrentGame.droppedItemTotals.Keys)
								{
									 if (!droppedItemTotals.ContainsKey(ilvl))
										  droppedItemTotals.Add(ilvl, CurrentGame.droppedItemTotals[ilvl]);
									 else
									 {
										  for (int i=0; i<6; i++)
										  {
												droppedItemTotals[ilvl][i]+=CurrentGame.droppedItemTotals[ilvl][i];
										  }
									 }
								}

						  }

						  //public void Reset()
						  //{
						  //	 //Reset current game stats
						  //	 CurrentGame.Reset();
						  //}

						  public void Update()
						  {
								//Update our total time tracked
								timeTracked=TotalTimeTracked();
								//Perm Update Stats
								UpdateTotals();
						  }

						  public class CurrentGameItemStats
						  {

								public CurrentGameItemStats() { Reset(); }

								public int[] lootedItemTotals { get; set; }
								public int[] stashedItemTotals { get; set; }
								public SortedList<int, int[]> droppedItemTotals { get; set; }
								public DateTime TimeTrackingBegan { get; set; }

								public void ProfileChanged(ProfileStatisics.ProfileStats.ProfileItemStats stats)
								{
									 //Update Current Game Stats using the profile stats.
									 for (int i=0; i<6; i++)
									 {
										  lootedItemTotals[i]+=stats.lootedItemTotals[i];
										  stashedItemTotals[i]+=stats.stashedItemTotals[i];
									 }

									 //update dropped stats
									 foreach (int ilvl in stats.droppedItemTotals.Keys)
									 {
										  if (!droppedItemTotals.ContainsKey(ilvl))
												droppedItemTotals.Add(ilvl, stats.droppedItemTotals[ilvl]);
										  else
										  {
												for (int i=0; i<6; i++)
												{
													 droppedItemTotals[ilvl][i]+=stats.droppedItemTotals[ilvl][i];
												}
										  }
									 }
								}

								public int LootTotal()
								{
									 int count=0;
									 for (int i=0; i<6; i++)
									 {
										  count+=lootedItemTotals[i];
									 }

									 return count;
								}

								public int StashTotal()
								{
									 int count=0;
									 for (int i=0; i<6; i++)
									 {
										  count+=stashedItemTotals[i];
									 }

									 return count;
								}

								public int[] temp_LootedTotals()
								{
									 //Returns a temp total of looted items.
									 int[] tmp_LootedTotals=(int[])lootedItemTotals.Clone();

									 if (Bot.Stats.ProfileStats.CurrentProfile==null)
										  return tmp_LootedTotals;

									 for (int i=0; i<6; i++)
									 {
										  tmp_LootedTotals[i]+=Bot.Stats.ProfileStats.CurrentProfile.ItemStats.lootedItemTotals[i];
									 }

									 return tmp_LootedTotals;
								}

								public int[] temp_StashedTotals()
								{
									 int[] tmp_StashedTotals=(int[])stashedItemTotals.Clone();

									 if (Bot.Stats.ProfileStats.CurrentProfile==null)
										  return tmp_StashedTotals;

									 for (int i=0; i<6; i++)
									 {
										  tmp_StashedTotals[i]+=Bot.Stats.ProfileStats.CurrentProfile.ItemStats.stashedItemTotals[i];
									 }

									 return tmp_StashedTotals;
								}

								public void Reset()
								{
									 //misc, magical, rare, legendary
									 lootedItemTotals=new int[] { 0, 0, 0, 0, 0, 0, 0 };
									 stashedItemTotals=new int[] { 0, 0, 0, 0, 0, 0, 0 };
									 droppedItemTotals=new SortedList<int, int[]>();
									 TimeTrackingBegan=DateTime.Now;
								}
						  }

					 }

					 public class GameStatistics
					 {
						  public GameStatistics()
						  {
								TotalTimeRunning=new TimeSpan(0, 0, 0, 0);
								TotalGames=0;
								TotalDeaths=0;
								CurrentGame=new CurrentGameStats();
						  }

						  public TimeSpan TotalTimeRunning { get; set; }
						  public int TotalGames { get; set; }
						  public int TotalDeaths { get; set; }
						  public CurrentGameStats CurrentGame { get; set; }

						  public void Update()
						  {
								TotalGames++;
								TotalDeaths+=CurrentGame.Deaths;
								TotalTimeRunning=DateTime.Now.Subtract(CurrentGame.StartTime).Add(TotalTimeRunning);
						  }

						  public class CurrentGameStats
						  {
								public CurrentGameStats() { Reset(); }

								public int Deaths { get; set; }
								public DateTime StartTime { get; set; }

								public void Reset()
								{
									 Deaths=0;
									 StartTime=DateTime.Now;
								}

						  }
					 }

					 public class ProfileStatisics
					 {

						  public ProfileStats CurrentProfile { get; set; }

						  //public bool shouldUpdateXPCache = true;

						  private List<ProfileStats> completedProfiles { get; set; }
						  public ProfileStatisics()
						  {
								CurrentProfile=null;
								completedProfiles=new List<ProfileStats>();
						  }

						  public void UpdateProfileChanged()
						  {
								string currentProfile=Zeta.CommonBot.Settings.GlobalSettings.Instance.LastProfile;
								if (CurrentProfile!=null&&CurrentProfile.ProfileName!=currentProfile)
								{
									 //Update last profile, start tracking a new one
									 CurrentProfile.FinalizeStats();
									 completedProfiles.Add(CurrentProfile);
									 Bot.Stats.ItemStats.CurrentGame.ProfileChanged(CurrentProfile.ItemStats);
									 CurrentProfile=new ProfileStats(currentProfile);
								}
								else if (CurrentProfile==null)
									 CurrentProfile=new ProfileStats(currentProfile);

								//shouldUpdateXPCache = true;
						  }

						  public void OutputReport()
						  {
								//Don't report a thing if no profile was set!
								if (CurrentProfile==null) return;

								//Add current profile to collection..
								CurrentProfile.FinalizeStats();
								completedProfiles.Add(CurrentProfile);

								Bot.Stats.GameStats.Update();
								Bot.Stats.ItemStats.CurrentGame.ProfileChanged(CurrentProfile.ItemStats);
								Bot.Stats.ItemStats.Update();


								string outputPath=Funky.FolderPaths.sTrinityLogPath+@"ProfileStats\";
								if (!System.IO.Directory.Exists(outputPath))
									 System.IO.Directory.CreateDirectory(outputPath);
								//ddMMyyyyHHmmss
								string outputFileName=ZetaDia.Service.CurrentHero.Name+"_"+DateTime.Now.ToString("yyyy-MM-dd-HH-mm")+".txt";
								FileStream LogStream=File.Open(outputPath+outputFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
								using (StreamWriter LogWriter=new StreamWriter(LogStream))
								{
									 TimeSpan gameDuration=DateTime.Now.Subtract(Bot.Stats.ItemStats.CurrentGame.TimeTrackingBegan).Duration();
									 LogWriter.WriteLine("===== Profile Output =====");
									 LogWriter.WriteLine("Total of "+completedProfiles.Count+" profiles tracked");
									 LogWriter.WriteLine("Duration of the game: "+gameDuration.ToString());
									 LogWriter.WriteLine("Date: "+DateTime.Now.ToString());
									 LogWriter.WriteLine("Death Count: "+Bot.Stats.GameStats.CurrentGame.Deaths);
									 LogWriter.WriteLine("");
									 LogWriter.WriteLine("Item Totals: Looted ("+Bot.Stats.ItemStats.CurrentGame.LootTotal()+") / Stashed ("+Bot.Stats.ItemStats.CurrentGame.StashTotal()+")");
									 double itemLootedPerMinTotal=Math.Round(Bot.Stats.ItemStats.CurrentGame.LootTotal()/gameDuration.TotalMinutes, 3);
									 double itemStashedPerMinTotal=Math.Round(Bot.Stats.ItemStats.CurrentGame.StashTotal()/gameDuration.TotalMinutes, 3);
									 LogWriter.WriteLine("IMP: Looted("+itemLootedPerMinTotal+") / Stashed("+itemStashedPerMinTotal+")");
									 LogWriter.WriteLine("");
									 LogWriter.WriteLine("=== Seen Dropped Loot (Armor/Weapon/Jewelery) ===");
									 LogWriter.WriteLine("ILvl \t White \t Magic \t Rare \t Legendary");
									 int[] droppedQualityTotals=new int[] { 0, 0, 0, 0 };
									 foreach (var item in Bot.Stats.ItemStats.CurrentGame.droppedItemTotals.Keys)
									 {
										  int[] totals=Bot.Stats.ItemStats.CurrentGame.droppedItemTotals[item];
										  droppedQualityTotals[0]+=totals[0];
										  droppedQualityTotals[1]+=totals[1];
										  droppedQualityTotals[2]+=totals[2];
										  droppedQualityTotals[3]+=totals[3];
										  LogWriter.WriteLine("{0} \t {1} \t {2} \t {3} \t {4}", item.ToString(), totals[0], totals[1], totals[2], totals[3]);
									 }
									 LogWriter.WriteLine("------------------------------------");
									 LogWriter.WriteLine("{0} \t {1} \t {2} \t {3} \t {4}", "Total", droppedQualityTotals[0], droppedQualityTotals[1], droppedQualityTotals[2], droppedQualityTotals[3]);

									 LogWriter.WriteLine("");
									 LogWriter.WriteLine("");
									 foreach (var item in completedProfiles)
									 {
										  LogWriter.WriteLine("Profile: "+item.ProfileName);
										  LogWriter.WriteLine("Total Time: "+item.TimeSpent.ToString());
										  LogWriter.WriteLine("Total Deaths: "+item.DeathCount);
										  //LogWriter.WriteLine("Total XP: " + item.ExperienceGained);
										  LogWriter.WriteLine("============================================");
										  System.TimeSpan diff1=item.TimeSpent;
										  int TotalStashed, TotalLooted;
										  TotalStashed=item.ItemStats.stashedTOTAL();
										  TotalLooted=item.ItemStats.lootedTOTAL();
										  double itemLootPerMin=Math.Round(TotalLooted/diff1.TotalMinutes, 3);
										  double itemStashPerMin=Math.Round(TotalStashed/diff1.TotalMinutes, 3);

										  LogWriter.WriteLine("Total Looted ("+TotalLooted+") / Stashed ("+TotalStashed+")");
										  LogWriter.WriteLine("Legendaries Looted ("+item.ItemStats.lootedItemTotals[3]+") / Stashed ("+item.ItemStats.stashedItemTotals[3]+")");
										  LogWriter.WriteLine("Rares Looted ("+item.ItemStats.lootedItemTotals[2]+") / Stashed ("+item.ItemStats.stashedItemTotals[2]+")");
										  LogWriter.WriteLine("Magical Looted ("+item.ItemStats.lootedItemTotals[1]+") / Stashed ("+item.ItemStats.stashedItemTotals[1]+")");
										  LogWriter.WriteLine("Misc Looted ("+item.ItemStats.lootedItemTotals[0]+") / Stashed ("+item.ItemStats.stashedItemTotals[0]+")");
										  LogWriter.WriteLine("Items Per Minute Looted ("+itemLootPerMin+") / Stashed ("+itemStashPerMin+")");
										  LogWriter.WriteLine("=== Seen Dropped Items ===");
										  LogWriter.WriteLine("ILvl \t White \t Magic \t Rare \t Legendary");
										  foreach (var stats in item.ItemStats.droppedItemTotals.Keys)
										  {
												int[] totals=item.ItemStats.droppedItemTotals[stats];
												LogWriter.WriteLine("{0} \t {1} \t {2} \t {3} \t {4}", stats.ToString(), totals[0], totals[1], totals[2], totals[3]);
										  }

										  LogWriter.WriteLine("============================================");

										  LogWriter.WriteLine("");
										  LogWriter.WriteLine("");
									 }
								}
								//LogStream.Close();

								//Clear out profiles
								CurrentProfile=null;
								completedProfiles.Clear();
						  }

						  public class ProfileStats
						  {
								public string ProfileName { get; set; }
								public TimeSpan TimeSpent { get; set; }
								//public int ExperienceGained { get; set; }
								private DateTime timeStartedProfile;
								//private int startingXP;
								public ProfileItemStats ItemStats { get; set; }
								public int DeathCount { get; set; }

								public ProfileStats(string name)
								{
									 DeathCount=0;
									 //ExperienceGained = 0;
									 ProfileName=name;
									 TimeSpent=new TimeSpan(0, 0, 0, 0);
									 timeStartedProfile=DateTime.Now;
									 ItemStats=new ProfileItemStats();
									 //startingXP = ZetaDia.Me.CurrentExperience;
								}

								public void FinalizeStats()
								{
									 TimeSpent=DateTime.Now.Subtract(timeStartedProfile).Duration();
								}



								public class ProfileItemStats
								{
									 public ProfileItemStats() { Reset(); }

									 public int[] lootedItemTotals { get; set; }
									 public int[] stashedItemTotals { get; set; }
									 public SortedList<int, int[]> droppedItemTotals { get; set; }
									 public DateTime TimeTrackingBegan { get; set; }

									 public void Reset()
									 {
										  //misc, magical, rare, legendary
										  lootedItemTotals=new int[] { 0, 0, 0, 0, 0, 0, 0 };
										  stashedItemTotals=new int[] { 0, 0, 0, 0, 0, 0, 0 };
										  droppedItemTotals=new SortedList<int, int[]>();
										  TimeTrackingBegan=DateTime.Now;
									 }

									 public int lootedTOTAL()
									 {
										  int count=0;
										  for (int i=0; i<6; i++)
										  {
												count+=lootedItemTotals[i];
										  }

										  return count;
									 }

									 public int stashedTOTAL()
									 {
										  int count=0;
										  for (int i=0; i<6; i++)
										  {
												count+=stashedItemTotals[i];
										  }

										  return count;
									 }

								}

						  }

					 }
				}


		  }
    
}