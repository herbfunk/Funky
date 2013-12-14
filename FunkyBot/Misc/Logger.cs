using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using FunkyBot.Cache.Objects;
using FunkyBot.DBHandlers;
using FunkyBot.Game;
using Zeta.Common;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Player;
using Zeta.Internals.Actors;
using Zeta;

namespace FunkyBot
{
	 [Flags]
	 public enum LogLevel
	 {
		  None=0,
		  User=1,
		  Execption=2, //doh?
		  Cluster=4,
		  Grouping=8,
		  Movement=16,
		  Ability=32,
		  Target=64,
		  Items=128,
		  OutOfGame=256,
		  OutOfCombat=512,
		  Event=1024,
		  Cache=2048,

		  All=User|Execption|Cluster|Grouping|Movement|Ability|Target|Items|OutOfGame|OutOfCombat|Event,
	 }
	 public static class Logger
	 {
		 internal static string LoggingPrefixString
		 {
			 get
			 {
				 return Bot.Character.Account.ActorClass.ToString() + " _ " + Bot.Character.Account.CurrentHeroName;
			 }
		 }
		 internal static string LoggingFolderPath
		 {
			 get
			 {
				 string folderpath = FolderPaths.sTrinityLogPath + Bot.Character.Account.CurrentAccountName + @"\";

				 if (!Directory.Exists(folderpath))
				 {
					 Logging.WriteDiagnostic("Creating Logging.Write Folder @ {0}", folderpath);
					 Directory.CreateDirectory(folderpath);
				 }

				 return folderpath;
			 }
		 }

		 internal static void LogGoodItems(CacheACDItem thisgooditem, GilesBaseItemType thisgilesbaseitemtype, GilesItemType thisgilesitemtype)
		 {

			 try
			 {
				 //Update this item
				 using (ZetaDia.Memory.AcquireFrame())
				 {
					 thisgooditem = new CacheACDItem(thisgooditem.ACDItem);
				 }
			 }
			 catch
			 {
				 Logging.WriteDiagnostic("Failure to update CacheACDItem during Logging");
			 }
			 double iThisItemValue = Backpack.ValueThisItem(thisgooditem, thisgilesitemtype);

			 FileStream LogStream = null;
			 try
			 {

				 LogStream = File.Open(LoggingFolderPath + LoggingPrefixString + " -- StashLog.log", FileMode.Append, FileAccess.Write, FileShare.Read);
				 using (StreamWriter LogWriter = new StreamWriter(LogStream))
				 {
					 if (!TownRunManager.bLoggedAnythingThisStash)
					 {
						 TownRunManager.bLoggedAnythingThisStash = true;
						 LogWriter.WriteLine(DateTime.Now.ToString() + ":");
						 LogWriter.WriteLine("====================");
					 }
					 string sLegendaryString = "";
					 if (thisgooditem.ThisQuality >= ItemQuality.Legendary)
					 {
						 if (!thisgooditem.IsUnidentified)
						 {
							 Funky.AddNotificationToQueue(thisgooditem.ThisRealName + " [" + thisgilesitemtype.ToString() + "] (Score=" + iThisItemValue.ToString() + ". " + TownRunManager.sValueItemStatString + ")", ZetaDia.Service.CurrentHero.Name + " new legendary!", Funky.ProwlNotificationPriority.Emergency);
							 sLegendaryString = " {legendary item}";
							 // Change made by bombastic
							 Logging.Write("+=+=+=+=+=+=+=+=+ LEGENDARY FOUND +=+=+=+=+=+=+=+=+");
							 Logging.Write("+  Name:       " + thisgooditem.ThisRealName + " (" + thisgilesitemtype.ToString() + ")");
							 Logging.Write("+  Score:       " + Math.Round(iThisItemValue).ToString());
							 Logging.Write("+  Attributes: " + thisgooditem.ItemStatString);
							 Logging.Write("+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+");
						 }
						 else
						 {
							 Logging.Write("+=+=+=+=+=+=+=+=+ LEGENDARY FOUND +=+=+=+=+=+=+=+=+");
							 Logging.Write("+  Unid:       " + thisgilesitemtype.ToString());
							 Logging.Write("+  Level:       " + thisgooditem.ThisLevel.ToString());
							 Logging.Write("+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+");
						 }


					 }
					 else
					 {
						 // Check for non-legendary notifications
						 bool bShouldNotify = false;
						 switch (thisgilesbaseitemtype)
						 {
							 case GilesBaseItemType.WeaponOneHand:
							 case GilesBaseItemType.WeaponRange:
							 case GilesBaseItemType.WeaponTwoHand:
								 //if (ithisitemvalue >= settings.iNeedPointsToNotifyWeapon)
								 //  bShouldNotify = true;
								 break;
							 case GilesBaseItemType.Armor:
							 case GilesBaseItemType.Offhand:
								 //if (ithisitemvalue >= settings.iNeedPointsToNotifyArmor)
								 //bShouldNotify = true;
								 break;
							 case GilesBaseItemType.Jewelry:
								 //if (ithisitemvalue >= settings.iNeedPointsToNotifyJewelry)
								 //bShouldNotify = true;
								 break;
						 }
						 if (bShouldNotify)
							 Funky.AddNotificationToQueue(thisgooditem.ThisRealName + " [" + thisgilesitemtype.ToString() + "] (Score=" + iThisItemValue.ToString() + ". " + TownRunManager.sValueItemStatString + ")", ZetaDia.Service.CurrentHero.Name + " new item!", Funky.ProwlNotificationPriority.Emergency);
					 }
					 if (!thisgooditem.IsUnidentified)
					 {
						 LogWriter.WriteLine(thisgilesbaseitemtype.ToString() + " - " + thisgilesitemtype.ToString() + " '" + thisgooditem.ThisRealName + "'. Score = " + Math.Round(iThisItemValue).ToString() + sLegendaryString);
						 LogWriter.WriteLine("  " + thisgooditem.ItemStatString);
						 LogWriter.WriteLine("");
					 }
					 else
					 {
						 LogWriter.WriteLine(thisgilesbaseitemtype.ToString() + " - " + thisgilesitemtype.ToString() + " '" + sLegendaryString);
						 LogWriter.WriteLine("  " + thisgooditem.ThisLevel.ToString());
						 LogWriter.WriteLine("");
					 }
				 }

			 }
			 catch (IOException)
			 {
				 Logging.Write("Fatal Error: File access error for stash log file.");
			 }
		 }

		 internal static void LogJunkItems(CacheACDItem thisgooditem, GilesBaseItemType thisgilesbaseitemtype, GilesItemType thisgilesitemtype, double ithisitemvalue)
		 {
			 FileStream LogStream = null;
			 try
			 {
				 LogStream = File.Open(LoggingFolderPath + LoggingPrefixString + " -- JunkLog.log", FileMode.Append, FileAccess.Write, FileShare.Read);
				 using (StreamWriter LogWriter = new StreamWriter(LogStream))
				 {
					 if (!TownRunManager.bLoggedJunkThisStash)
					 {
						 TownRunManager.bLoggedJunkThisStash = true;
						 LogWriter.WriteLine(DateTime.Now.ToString() + ":");
						 LogWriter.WriteLine("====================");
					 }
					 string sLegendaryString = "";
					 if (thisgooditem.ThisQuality >= ItemQuality.Legendary)
						 sLegendaryString = " {legendary item}";
					 LogWriter.WriteLine(thisgilesbaseitemtype.ToString() + " - " + thisgilesitemtype.ToString() + " '" + thisgooditem.ThisRealName + "'. Score = " + Math.Round(ithisitemvalue).ToString() + sLegendaryString);
					 if (!String.IsNullOrEmpty(TownRunManager.sJunkItemStatString))
						 LogWriter.WriteLine("  " + TownRunManager.sJunkItemStatString);
					 else
						 LogWriter.WriteLine("  (no scorable attributes)");
					 LogWriter.WriteLine("");
				 }

			 }
			 catch (IOException)
			 {
				 Logging.Write("Fatal Error: File access error for junk log file.");
			 }
		 }

		 internal static void LogItemInformation()
		 {
			 // Store item pickup stats
			 //if (!_hashsetItemPicksLookedAt.Contains(Bot.Targeting.CurrentTarget.RAGUID))
			 //{
			 CacheItem thisCacheItem = (CacheItem)Bot.Targeting.CurrentTarget;
			 GilesItemType thisgilesitemtype = Backpack.DetermineItemType(thisCacheItem.InternalName, thisCacheItem.BalanceData.thisItemType, thisCacheItem.BalanceData.thisFollowerType);
			 GilesBaseItemType thisgilesbasetype = Backpack.DetermineBaseType(thisgilesitemtype);

			 //Herbfunk -- Live loot stats keeping.
			 Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.LootedItemLog(thisgilesitemtype, thisgilesbasetype, thisCacheItem.Itemquality.Value);
			 //}
		 }

		  internal static void CleanLogs()
		  {
				List<string> deleteList=new List<string>();
				if (string.IsNullOrEmpty(FolderPaths.sDemonBuddyPath))
				{
					 Logging.Write("Failure to reconigze demon buddy path!");

				}
				else
				{
					 foreach (string file in Directory.GetFiles(FolderPaths.sDemonBuddyPath+@"\Logs\"))
					 {
						  DateTime curFileCreated=Directory.GetCreationTime(file);
						  if (DateTime.Now.Subtract(curFileCreated).TotalHours>=24)
						  {
								deleteList.Add(file);
						  }
					 }

					 if (deleteList.Count>0)
					 {
						  foreach (string item in deleteList)
						  {
								File.Delete(item);
						  }
						  Logging.WriteDiagnostic("Total DB logs deleted "+deleteList.Count);
					 }
				}

				string ItemRulesPath=@"\Plugins\FunkyBot\ItemRules\Log\Archive\";
				deleteList=new List<string>();
				try
				{
					 foreach (string file in Directory.GetFiles(FolderPaths.sDemonBuddyPath+ItemRulesPath))
					 {
						  DateTime curFileCreated=Directory.GetCreationTime(file);
						  if (DateTime.Now.Subtract(curFileCreated).TotalHours>=24)
						  {
								deleteList.Add(file);
						  }
					 }

					 if (deleteList.Count>0)
					 {
						  foreach (string item in deleteList)
						  {
								File.Delete(item);
						  }
						  Logging.WriteDiagnostic("Total item rule logs deleted "+deleteList.Count);
					 }

				} catch { Logging.WriteDiagnostic("Failure to clean log files @ path: "+ItemRulesPath); }

				string ProfileLogs=@"\Plugins\FunkyBot\Log\ProfileStats\";
				deleteList=new List<string>();
				try
				{
					 foreach (string file in Directory.GetFiles(FolderPaths.sDemonBuddyPath+ProfileLogs))
					 {
						  DateTime curFileCreated=Directory.GetCreationTime(file);
						  if (DateTime.Now.Subtract(curFileCreated).TotalDays>=1)
						  {
								deleteList.Add(file);
						  }
					 }

					 if (deleteList.Count>0)
					 {
						  foreach (string item in deleteList)
						  {
								File.Delete(item);
						  }
						  Logging.WriteDiagnostic("Total game stat logs deleted "+deleteList.Count);
					 }

				} catch { Logging.WriteDiagnostic("Failure to clean log files @ path: "+ProfileLogs); }

		  }

		 internal static void WriteProfileTrackerOutput()
		  {
			  string outputPath = Path.Combine(FolderPaths.sTrinityLogPath, "ProfileStats", Bot.Character.Account.CurrentHeroName + " - " + LoggingStamp);

			  try
			  {
				  try
				  {
					  using (StreamWriter LogWriter = new StreamWriter(outputPath, false))
					  {
						  LogWriter.WriteLine("====================");
						  LogWriter.WriteLine("== TOTAL SUMMARY ==");

						  TotalStats all = Bot.Game.TrackingStats;
						  LogWriter.WriteLine(all.GenerateOutputString());
						  //LogWriter.WriteLine("Total Games:{0} -- Total Unique Profiles:{1}\r\nDeaths:{2} TotalTime:{3} TotalGold:{4} TotalXP:{5}\r\n{6}",
						  //	all.GameCount, all.Profiles.Count, all.TotalDeaths, all.TotalTimeRunning.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"), all.TotalGold, all.TotalXP, all.TotalLootTracker);
						  
						  LogWriter.WriteLine("====================");
						  LogWriter.WriteLine("== PROFILE SUMMARY ==");
						  foreach (var item in all.Profiles)
						  {
							  LogWriter.WriteLine(item.GenerateOutput());
							  //LogWriter.WriteLine("{0}\r\nDeaths:{1} TotalTime:{2} TotalGold:{3} TotalXP:{4}\r\n{5}",
							  //	item.ProfileName, item.DeathCount, item.TotalTimeSpan.ToString(@"hh\ \h\ mm\ \m\ ss\ \s"), item.TotalGold, item.TotalXP, item.LootTracker);
						  }
					  }

				  }
				  catch (IOException)
				  {
					  Logging.Write("Fatal Error: File access error for junk log file.");
				  }
			  }
			  catch
			  {

			  }
		  }

		  private static string dbLogFile;
		  internal static string DBLogFile
		  {
				get { return dbLogFile; }
				set { dbLogFile=value; Init(); }
		  }

		  internal delegate string GetLogLevelName(object obj);

		  private static string[] FilePath;

		  internal static readonly string FileNamePrefix="FunkyLog - ";

		  internal static string LoggingStamp;

		  private static string filename;
		  public static string FunkyLogFilename
		  {
				get { return filename; }
				set { filename=value; }
		  }

		  public static void Init()
		  {
				FilePath=Path.GetFileName(DBLogFile).Split(Char.Parse(" "));
				LoggingStamp = FilePath[1] + " " + FilePath[2];
				filename = Path.Combine(FolderPaths.sDemonBuddyPath, "Logs", FileNamePrefix + LoggingStamp);
		  }
		  public static void Write(LogLevel level, string Message,bool WriteToMainLog, params object[] args)
		  {
				string prefix="["+DateTime.Now.ToString("hh:mm:ss.fff")+" "+level.ToString()+"]";
				string message=String.Format(Message, args);
				WriteLine(String.Format("{0} {1}", prefix, message), true);
				if (WriteToMainLog)
					 Logging.Write("{0} {1}","[Funky]", message);
		  }
		  public static void Write(LogLevel level, string Message, params object[] args)
		  {
				string prefix="["+DateTime.Now.ToString("hh:mm:ss.fff")+" "+level.ToString()+"]";
				string message=String.Format(Message, args);
				WriteLine(String.Format("{0} {1}", prefix, message), true);
		  }
		  private static void WriteLine(string text, bool append)
		  {
				// open file
				// If an error occurs throw it to the caller.
				try
				{

					 StreamWriter Writer=new StreamWriter(FunkyLogFilename, append, Encoding.UTF8);
					 if (!String.IsNullOrEmpty(text)) Writer.WriteLine(text);
					 Writer.Flush();
					 Writer.Close();
				} catch
				{
					 throw;
				}
		  }
	 }



	 public static class FolderPaths
	 {
		  internal static string sDemonBuddyPath=Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		  internal static string sTrinityPluginPath=sDemonBuddyPath+@"\Plugins\FunkyBot\";
		  internal static string sTrinityLogPath=sDemonBuddyPath+@"\Plugins\FunkyBot\Log\";
		  internal static string SettingsDefaultPath=Path.Combine(sTrinityPluginPath, "Config", "Defaults");


		  internal static string sFunkySettingsPath
		  {
				get
				{
					string sFunkyCharacterFolder = Path.Combine(sDemonBuddyPath, "Settings", "FunkyBot", Bot.Character.Account.CurrentAccountName);
					 if (!Directory.Exists(sFunkyCharacterFolder))
					 {
						  Logging.WriteDiagnostic("Creating Funky Settings Folder @ {0}", sFunkyCharacterFolder);
						  Directory.CreateDirectory(sFunkyCharacterFolder);
					 }

					 return sFunkyCharacterFolder;
				}
		  }
		  internal static string sFunkySettingsCurrentPath
		  {
				get
				{
					return Path.Combine(sFunkySettingsPath, Bot.Character.Account.CurrentHeroName + ".xml");

				}
		  }
	 }
}
