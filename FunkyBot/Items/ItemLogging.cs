using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using Zeta.Common;
using Zeta.Internals.Actors;
using System.IO;
using Zeta;
using System.Collections.Generic;

namespace FunkyBot
{
	 public partial class Funky
	 {
		  // Readable names of the above stats that get output into the trash/stash log files
		  private static readonly string[] StatNames=new string[29] { 
            "Dexterity", "Intelligence", "Strength", "Vitality", 
            "Life %", "Life On Hit", "Life Steal %", "Life Regen", 
            "Magic Find %", "Gold Find   %", "Movement Speed %", "Pickup Radius", "Sockets", 
            "Crit Chance %", "Crit Damage %", "Attack Speed %", "+Min Damage", "+Max Damage",
            "Total Block %", "Thorns", "+All Resist", "+Highest Single Resist", "DPS", "Armor", "Max Disc.", "Max Mana", "Arcane-On-Crit", "Mana Regen", "Globe Bonus"};



		  internal static string LoggingPrefixString
		  {
				get
				{
					 return Bot.ActorClass.ToString()+" _ "+Bot.CurrentHeroName;
				}
		  }
		  internal static string LoggingFolderPath
		  {
				get
				{
					 string folderpath =FolderPaths.sTrinityLogPath+Bot.CurrentAccountName+@"\";

					 if (!System.IO.Directory.Exists(folderpath))
					 {
						  Logging.WriteDiagnostic("Creating Log Folder @ {0}", folderpath);
						  System.IO.Directory.CreateDirectory(folderpath);
					 }

					 return folderpath;
				}
		  }


		  // **********************************************************************************************
		  // *****                      Log the nice items we found and stashed                       *****
		  // **********************************************************************************************
		  internal static void LogGoodItems(CacheACDItem thisgooditem, GilesBaseItemType thisgilesbaseitemtype, GilesItemType thisgilesitemtype)
		  {

			  try
			  {
				  //Update this item
				  using (ZetaDia.Memory.AcquireFrame())
				  {
					  thisgooditem=new CacheACDItem(thisgooditem.ACDItem);
				  }
			  }
			  catch
			  {
					Logging.WriteDiagnostic("Failure to update CacheACDItem during Logging");
			  }
			  double iThisItemValue=ValueThisItem(thisgooditem, thisgilesitemtype);

				FileStream LogStream=null;
				try
				{

					 LogStream=File.Open(LoggingFolderPath+LoggingPrefixString+" -- StashLog.log", FileMode.Append, FileAccess.Write, FileShare.Read);
					 using (StreamWriter LogWriter=new StreamWriter(LogStream))
					 {
						  if (!TownRunManager.bLoggedAnythingThisStash)
						  {
								TownRunManager.bLoggedAnythingThisStash=true;
								LogWriter.WriteLine(DateTime.Now.ToString()+":");
								LogWriter.WriteLine("====================");
						  }
						  string sLegendaryString="";
						  if (thisgooditem.ThisQuality>=ItemQuality.Legendary)
						  {
								if (!thisgooditem.IsUnidentified)
								{
									 AddNotificationToQueue(thisgooditem.ThisRealName+" ["+thisgilesitemtype.ToString()+"] (Score="+iThisItemValue.ToString()+". "+TownRunManager.sValueItemStatString+")", ZetaDia.Service.CurrentHero.Name+" new legendary!", ProwlNotificationPriority.Emergency);
									 sLegendaryString=" {legendary item}";
									 // Change made by bombastic
									 Logging.Write("+=+=+=+=+=+=+=+=+ LEGENDARY FOUND +=+=+=+=+=+=+=+=+");
									 Logging.Write("+  Name:       "+thisgooditem.ThisRealName+" ("+thisgilesitemtype.ToString()+")");
									 Logging.Write("+  Score:       "+Math.Round(iThisItemValue).ToString());
									 Logging.Write("+  Attributes: "+ thisgooditem.ItemStatString);
									 Logging.Write("+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+");
								}
								else
								{
									 Logging.Write("+=+=+=+=+=+=+=+=+ LEGENDARY FOUND +=+=+=+=+=+=+=+=+");
									 Logging.Write("+  Unid:       "+thisgilesitemtype.ToString());
									 Logging.Write("+  Level:       "+thisgooditem.ThisLevel.ToString());
									 Logging.Write("+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+");
								}


						  }
						  else
						  {
								// Check for non-legendary notifications
								bool bShouldNotify=false;
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
									 AddNotificationToQueue(thisgooditem.ThisRealName+" ["+thisgilesitemtype.ToString()+"] (Score="+iThisItemValue.ToString()+". "+TownRunManager.sValueItemStatString+")", ZetaDia.Service.CurrentHero.Name+" new item!", ProwlNotificationPriority.Emergency);
						  }
						  if (!thisgooditem.IsUnidentified)
						  {
								LogWriter.WriteLine(thisgilesbaseitemtype.ToString()+" - "+thisgilesitemtype.ToString()+" '"+thisgooditem.ThisRealName+"'. Score = "+Math.Round(iThisItemValue).ToString()+sLegendaryString);
								LogWriter.WriteLine("  "+thisgooditem.ItemStatString);
								LogWriter.WriteLine("");
						  }
						  else
						  {
								LogWriter.WriteLine(thisgilesbaseitemtype.ToString()+" - "+thisgilesitemtype.ToString()+" '"+sLegendaryString);
								LogWriter.WriteLine("  "+thisgooditem.ThisLevel.ToString());
								LogWriter.WriteLine("");
						  }
					 }

				} catch (IOException)
				{
					 Log("Fatal Error: File access error for stash log file.");
				}
		  }

		  // **********************************************************************************************
		  // *****                   Log the rubbish junk items we salvaged or sold                   *****
		  // **********************************************************************************************
		  internal static void LogJunkItems(CacheACDItem thisgooditem, GilesBaseItemType thisgilesbaseitemtype, GilesItemType thisgilesitemtype, double ithisitemvalue)
		  {
				FileStream LogStream=null;
				try
				{
					 LogStream=File.Open(LoggingFolderPath+LoggingPrefixString+" -- JunkLog.log", FileMode.Append, FileAccess.Write, FileShare.Read);
					 using (StreamWriter LogWriter=new StreamWriter(LogStream))
					 {
						  if (!TownRunManager.bLoggedJunkThisStash)
						  {
								TownRunManager.bLoggedJunkThisStash=true;
								LogWriter.WriteLine(DateTime.Now.ToString()+":");
								LogWriter.WriteLine("====================");
						  }
						  string sLegendaryString="";
						  if (thisgooditem.ThisQuality>=ItemQuality.Legendary)
								sLegendaryString=" {legendary item}";
						  LogWriter.WriteLine(thisgilesbaseitemtype.ToString()+" - "+thisgilesitemtype.ToString()+" '"+thisgooditem.ThisRealName+"'. Score = "+Math.Round(ithisitemvalue).ToString()+sLegendaryString);
						  if (!String.IsNullOrEmpty(TownRunManager.sJunkItemStatString))
								LogWriter.WriteLine("  "+TownRunManager.sJunkItemStatString);
						  else
								LogWriter.WriteLine("  (no scorable attributes)");
						  LogWriter.WriteLine("");
					 }

				} catch (IOException)
				{
					 Log("Fatal Error: File access error for junk log file.");
				}
		  }

		  // **********************************************************************************************
		  // *****                              Full Output Of Item Stats                             *****
		  // **********************************************************************************************
          //private static void OutputReport()
          //{
          //      TimeSpan TotalRunningTime=DateTime.Now.Subtract(ItemStatsWhenStartedBot);
          //      string sLogFileName=LoggingPrefixString+" -- Stats.log";

          //      // Create whole new file
          //      FileStream LogStream=File.Open(LoggingFolderPath+sLogFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
          //      using (StreamWriter LogWriter=new StreamWriter(LogStream))
          //      {
          //           LogWriter.WriteLine("===== Misc Statistics =====");
          //           LogWriter.WriteLine("Total tracking time: "+TotalRunningTime.Hours.ToString()+"h "+TotalRunningTime.Minutes.ToString()+
          //                "m "+TotalRunningTime.Seconds.ToString()+"s");
          //           LogWriter.WriteLine("Total deaths: "+Bot.Stats.iTotalDeaths.ToString()+" ["+Math.Round(Bot.Stats.iTotalDeaths/TotalRunningTime.TotalHours, 2).ToString()+" per hour]");
          //           LogWriter.WriteLine("Total games (approx): "+Bot.Stats.iTotalLeaveGames.ToString()+" ["+Math.Round(Bot.Stats.iTotalLeaveGames/TotalRunningTime.TotalHours, 2).ToString()+" per hour]");
          //           if (Bot.Stats.iTotalLeaveGames==0&&Bot.Stats.iTotalJoinGames>0)
          //           {
          //                if (Bot.Stats.iTotalJoinGames==1&&Bot.Profile.iTotalProfileRecycles>1)
          //                {
          //                      LogWriter.WriteLine("(a profile manager/death handler is interfering with join/leave game events, attempting to guess total runs based on profile-loops)");
          //                      LogWriter.WriteLine("Total full profile cycles: "+Bot.Profile.iTotalProfileRecycles.ToString()+" ["+Math.Round(Bot.Profile.iTotalProfileRecycles/TotalRunningTime.TotalHours, 2).ToString()+" per hour]");
          //                }
          //                else
          //                {
          //                      LogWriter.WriteLine("(your games left value may be bugged @ 0 due to profile managers/routines etc., now showing games joined instead:)");
          //                      LogWriter.WriteLine("Total games joined: "+Bot.Stats.iTotalJoinGames.ToString()+" ["+Math.Round(Bot.Stats.iTotalJoinGames/TotalRunningTime.TotalHours, 2).ToString()+" per hour]");
          //                }
          //           }
          //           LogWriter.WriteLine("");

          //           LogWriter.WriteLine("===== Item DROP Statistics =====");
          //           // Item stats
          //           if (ItemsDroppedStats.iTotal>0)
          //           {
          //                LogWriter.WriteLine("Items:");
          //                LogWriter.WriteLine("Total items dropped: "+ItemsDroppedStats.iTotal.ToString()+" ["+
          //                      Math.Round(ItemsDroppedStats.iTotal/TotalRunningTime.TotalHours, 2).ToString()+" per hour]");

          //                LogWriter.WriteLine("Items dropped by ilvl: ");
          //                for (int iThisLevel=1; iThisLevel<=63; iThisLevel++)
          //                      if (ItemsDroppedStats.iTotalPerLevel[iThisLevel]>0)
          //                           LogWriter.WriteLine("- ilvl"+iThisLevel.ToString()+": "+ItemsDroppedStats.iTotalPerLevel[iThisLevel].ToString()+" ["+
          //                                Math.Round(ItemsDroppedStats.iTotalPerLevel[iThisLevel]/TotalRunningTime.TotalHours, 2).ToString()+" per hour] {"+
          //                                Math.Round((ItemsDroppedStats.iTotalPerLevel[iThisLevel]/ItemsDroppedStats.iTotal)*100, 2).ToString()+" %}");

          //                LogWriter.WriteLine("");

          //                LogWriter.WriteLine("Items dropped by quality: ");
          //                for (int iThisQuality=0; iThisQuality<=3; iThisQuality++)
          //                {
          //                      if (ItemsDroppedStats.iTotalPerQuality[iThisQuality]>0)
          //                      {
          //                           LogWriter.WriteLine("- "+sQualityString[iThisQuality]+": "+ItemsDroppedStats.iTotalPerQuality[iThisQuality].ToString()+" ["+Math.Round(ItemsDroppedStats.iTotalPerQuality[iThisQuality]/TotalRunningTime.TotalHours, 2).ToString()+" per hour] {"+Math.Round((ItemsDroppedStats.iTotalPerQuality[iThisQuality]/ItemsDroppedStats.iTotal)*100, 2).ToString()+" %}");
          //                           for (int iThisLevel=1; iThisLevel<=63; iThisLevel++)
          //                                if (ItemsDroppedStats.iTotalPerQPerL[iThisQuality, iThisLevel]>0)
          //                                      LogWriter.WriteLine("--- ilvl "+iThisLevel.ToString()+" "+sQualityString[iThisQuality]+": "+ItemsDroppedStats.iTotalPerQPerL[iThisQuality, iThisLevel].ToString()+" ["+Math.Round(ItemsDroppedStats.iTotalPerQPerL[iThisQuality, iThisLevel]/TotalRunningTime.TotalHours, 2).ToString()+" per hour] {"+Math.Round((ItemsDroppedStats.iTotalPerQPerL[iThisQuality, iThisLevel]/ItemsDroppedStats.iTotal)*100, 2).ToString()+" %}");
          //                      } // Any at all this quality?
          //                } // For loop on quality
          //                LogWriter.WriteLine("");


          //           } // End of item stats
          //           // Potion stats
          //           if (ItemsDroppedStats.iTotalPotions>0)
          //           {
          //                LogWriter.WriteLine("Potion Drops:");
          //                LogWriter.WriteLine("Total potions: "+ItemsDroppedStats.iTotalPotions.ToString()+" ["+Math.Round(ItemsDroppedStats.iTotalPotions/TotalRunningTime.TotalHours, 2).ToString()+" per hour]");
          //                for (int iThisLevel=1; iThisLevel<=63; iThisLevel++) if (ItemsDroppedStats.iPotionsPerLevel[iThisLevel]>0)
          //                           LogWriter.WriteLine("- ilvl "+iThisLevel.ToString()+": "+ItemsDroppedStats.iPotionsPerLevel[iThisLevel].ToString()+" ["+Math.Round(ItemsDroppedStats.iPotionsPerLevel[iThisLevel]/TotalRunningTime.TotalHours, 2).ToString()+" per hour] {"+Math.Round((ItemsDroppedStats.iPotionsPerLevel[iThisLevel]/ItemsDroppedStats.iTotalPotions)*100, 2).ToString()+" %}");
          //                LogWriter.WriteLine("");
          //           } // End of potion stats
          //           // Gem stats
          //           if (ItemsDroppedStats.iTotalGems>0)
          //           {
          //                LogWriter.WriteLine("Gem Drops:");
          //                LogWriter.WriteLine("Total gems: "+ItemsDroppedStats.iTotalGems.ToString()+" ["+Math.Round(ItemsDroppedStats.iTotalGems/TotalRunningTime.TotalHours, 2).ToString()+" per hour]");
          //                for (int iThisGemType=0; iThisGemType<=3; iThisGemType++)
          //                {
          //                      if (ItemsDroppedStats.iGemsPerType[iThisGemType]>0)
          //                      {
          //                           LogWriter.WriteLine("- "+sGemString[iThisGemType]+": "+ItemsDroppedStats.iGemsPerType[iThisGemType].ToString()+" ["+Math.Round(ItemsDroppedStats.iGemsPerType[iThisGemType]/TotalRunningTime.TotalHours, 2).ToString()+" per hour] {"+Math.Round((ItemsDroppedStats.iGemsPerType[iThisGemType]/ItemsDroppedStats.iTotalGems)*100, 2).ToString()+" %}");
          //                           for (int iThisLevel=1; iThisLevel<=63; iThisLevel++)
          //                                if (ItemsDroppedStats.iGemsPerTPerL[iThisGemType, iThisLevel]>0)
          //                                      LogWriter.WriteLine("--- ilvl "+iThisLevel.ToString()+" "+sGemString[iThisGemType]+": "+ItemsDroppedStats.iGemsPerTPerL[iThisGemType, iThisLevel].ToString()+" ["+Math.Round(ItemsDroppedStats.iGemsPerTPerL[iThisGemType, iThisLevel]/TotalRunningTime.TotalHours, 2).ToString()+" per hour] {"+Math.Round((ItemsDroppedStats.iGemsPerTPerL[iThisGemType, iThisLevel]/ItemsDroppedStats.iTotalGems)*100, 2).ToString()+" %}");
          //                      } // Any at all this quality?
          //                } // For loop on quality
          //           } // End of gem stats
          //           // Key stats
          //           if (ItemsDroppedStats.iTotalInfernalKeys>0)
          //           {
          //                LogWriter.WriteLine("Infernal Key Drops:");
          //                LogWriter.WriteLine("Total Keys: "+ItemsDroppedStats.iTotalInfernalKeys.ToString()+" ["+Math.Round(ItemsDroppedStats.iTotalInfernalKeys/TotalRunningTime.TotalHours, 2).ToString()+" per hour]");
          //           } // End of key stats
          //           LogWriter.WriteLine("");
          //           LogWriter.WriteLine("");
          //           LogWriter.WriteLine("===== Item PICKUP Statistics =====");
          //           // Item stats
          //           if (ItemsPickedStats.iTotal>0)
          //           {
          //                LogWriter.WriteLine("Items:");
          //                LogWriter.WriteLine("Total items picked up: "+ItemsPickedStats.iTotal.ToString()+" ["+Math.Round(ItemsPickedStats.iTotal/TotalRunningTime.TotalHours, 2).ToString()+" per hour]");


          //                LogWriter.WriteLine("Item picked up by ilvl: ");
          //                for (int iThisLevel=1; iThisLevel<=63; iThisLevel++)
          //                      if (ItemsPickedStats.iTotalPerLevel[iThisLevel]>0)
          //                           LogWriter.WriteLine("- ilvl"+iThisLevel.ToString()+": "+ItemsPickedStats.iTotalPerLevel[iThisLevel].ToString()+" ["+Math.Round(ItemsPickedStats.iTotalPerLevel[iThisLevel]/TotalRunningTime.TotalHours, 2).ToString()+" per hour] {"+Math.Round((ItemsPickedStats.iTotalPerLevel[iThisLevel]/ItemsPickedStats.iTotal)*100, 2).ToString()+" %}");
          //                LogWriter.WriteLine("");

          //                LogWriter.WriteLine("Items picked up by quality: ");
          //                for (int iThisQuality=0; iThisQuality<=3; iThisQuality++)
          //                {
          //                      if (ItemsPickedStats.iTotalPerQuality[iThisQuality]>0)
          //                      {
          //                           LogWriter.WriteLine("- "+sQualityString[iThisQuality]+": "+ItemsPickedStats.iTotalPerQuality[iThisQuality].ToString()+" ["+Math.Round(ItemsPickedStats.iTotalPerQuality[iThisQuality]/TotalRunningTime.TotalHours, 2).ToString()+" per hour] {"+Math.Round((ItemsPickedStats.iTotalPerQuality[iThisQuality]/ItemsPickedStats.iTotal)*100, 2).ToString()+" %}");
          //                           for (int iThisLevel=1; iThisLevel<=63; iThisLevel++)
          //                                if (ItemsPickedStats.iTotalPerQPerL[iThisQuality, iThisLevel]>0)
          //                                      LogWriter.WriteLine("--- ilvl "+iThisLevel.ToString()+" "+sQualityString[iThisQuality]+": "+ItemsPickedStats.iTotalPerQPerL[iThisQuality, iThisLevel].ToString()+" ["+Math.Round(ItemsPickedStats.iTotalPerQPerL[iThisQuality, iThisLevel]/TotalRunningTime.TotalHours, 2).ToString()+" per hour] {"+Math.Round((ItemsPickedStats.iTotalPerQPerL[iThisQuality, iThisLevel]/ItemsPickedStats.iTotal)*100, 2).ToString()+" %}");
          //                      } // Any at all this quality?
          //                } // For loop on quality
          //                LogWriter.WriteLine("");
          //                if (iTotalFollowerItemsIgnored>0)
          //                {
          //                      LogWriter.WriteLine("  (note: "+iTotalFollowerItemsIgnored.ToString()+" follower items ignored for being ilvl <60 or blue)");
          //                }

          //           } // End of item stats
          //           // Potion stats
          //           if (ItemsPickedStats.iTotalPotions>0)
          //           {
          //                LogWriter.WriteLine("Potion Pickups:");
          //                LogWriter.WriteLine("Total potions: "+ItemsPickedStats.iTotalPotions.ToString()+" ["+Math.Round(ItemsPickedStats.iTotalPotions/TotalRunningTime.TotalHours, 2).ToString()+" per hour]");
          //                for (int iThisLevel=1; iThisLevel<=63; iThisLevel++) if (ItemsPickedStats.iPotionsPerLevel[iThisLevel]>0)
          //                           LogWriter.WriteLine("- ilvl "+iThisLevel.ToString()+": "+ItemsPickedStats.iPotionsPerLevel[iThisLevel].ToString()+" ["+Math.Round(ItemsPickedStats.iPotionsPerLevel[iThisLevel]/TotalRunningTime.TotalHours, 2).ToString()+" per hour] {"+Math.Round((ItemsPickedStats.iPotionsPerLevel[iThisLevel]/ItemsPickedStats.iTotalPotions)*100, 2).ToString()+" %}");
          //                LogWriter.WriteLine("");
          //           } // End of potion stats
          //           // Gem stats
          //           if (ItemsPickedStats.iTotalGems>0)
          //           {
          //                LogWriter.WriteLine("Gem Pickups:");
          //                LogWriter.WriteLine("Total gems: "+ItemsPickedStats.iTotalGems.ToString()+" ["+Math.Round(ItemsPickedStats.iTotalGems/TotalRunningTime.TotalHours, 2).ToString()+" per hour]");
          //                for (int iThisGemType=0; iThisGemType<=3; iThisGemType++)
          //                {
          //                      if (ItemsPickedStats.iGemsPerType[iThisGemType]>0)
          //                      {
          //                           LogWriter.WriteLine("- "+sGemString[iThisGemType]+": "+ItemsPickedStats.iGemsPerType[iThisGemType].ToString()+" ["+Math.Round(ItemsPickedStats.iGemsPerType[iThisGemType]/TotalRunningTime.TotalHours, 2).ToString()+" per hour] {"+Math.Round((ItemsPickedStats.iGemsPerType[iThisGemType]/ItemsPickedStats.iTotalGems)*100, 2).ToString()+" %}");
          //                           for (int iThisLevel=1; iThisLevel<=63; iThisLevel++)
          //                                if (ItemsPickedStats.iGemsPerTPerL[iThisGemType, iThisLevel]>0)
          //                                      LogWriter.WriteLine("--- ilvl "+iThisLevel.ToString()+" "+sGemString[iThisGemType]+": "+ItemsPickedStats.iGemsPerTPerL[iThisGemType, iThisLevel].ToString()+" ["+Math.Round(ItemsPickedStats.iGemsPerTPerL[iThisGemType, iThisLevel]/TotalRunningTime.TotalHours, 2).ToString()+" per hour] {"+Math.Round((ItemsPickedStats.iGemsPerTPerL[iThisGemType, iThisLevel]/ItemsPickedStats.iTotalGems)*100, 2).ToString()+" %}");
          //                      } // Any at all this quality?
          //                } // For loop on quality
          //           } // End of gem stats
          //           // Key stats
          //           if (ItemsPickedStats.iTotalInfernalKeys>0)
          //           {
          //                LogWriter.WriteLine("Infernal Key Pickups:");
          //                LogWriter.WriteLine("Total Keys: "+ItemsPickedStats.iTotalInfernalKeys.ToString()+" ["+Math.Round(ItemsPickedStats.iTotalInfernalKeys/TotalRunningTime.TotalHours, 2).ToString()+" per hour]");
          //           } // End of key stats
          //           LogWriter.WriteLine("===== End Of Report =====");
          //      }
          //}



		  //Called during the confirmed loot stage of target handling.
		 internal static void LogItemInformation()
		  {
				// Store item pickup stats
				//if (!_hashsetItemPicksLookedAt.Contains(Bot.Targeting.CurrentTarget.RAGUID))
				//{
					 CacheItem thisCacheItem=(CacheItem)Bot.Targeting.CurrentTarget;
					 GilesItemType thisgilesitemtype=DetermineItemType(thisCacheItem.InternalName, thisCacheItem.BalanceData.thisItemType, thisCacheItem.BalanceData.thisFollowerType);
					 GilesBaseItemType thisgilesbasetype=DetermineBaseType(thisgilesitemtype);

					 //Herbfunk -- Live loot stats keeping.
					 ProfileTracking.TotalStats.CurrentTrackingProfile.LootTracker.LootedItemLog(thisgilesitemtype, thisgilesbasetype, thisCacheItem.Itemquality.Value);
				//}
		  }

	 }
}