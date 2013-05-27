using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using Zeta.Common;
using Zeta.Internals.Actors;
using System.IO;
using Zeta;

namespace FunkyTrinity
{
    public partial class Funky
    {
		  // **********************************************************************************************
		  // *****    Item Stats Class and Variables - for the detailed item drop/pickup etc. stats   *****
		  // **********************************************************************************************
		  private class GilesItemStats
		  {
				public double iTotal { get; set; }
				public double[] iTotalPerQuality { get; set; }
				public double[] iTotalPerLevel { get; set; }
				public double[,] iTotalPerQPerL { get; set; }
				public double iTotalPotions { get; set; }
				public double[] iPotionsPerLevel { get; set; }
				public double iTotalGems { get; set; }
				public double[] iGemsPerType { get; set; }
				public double[] iGemsPerLevel { get; set; }
				public double[,] iGemsPerTPerL { get; set; }
				public double iTotalInfernalKeys { get; set; }

				public GilesItemStats(double total, double[] totalperq, double[] totalperl, double[,] totalperqperl, double totalpotions, double[] potionsperlevel, double totalgems,
					 double[] gemspertype, double[] gemsperlevel, double[,] gemspertperl, double totalkeys)
				{
					 iTotal=total;
					 iTotalPerQuality=totalperq;
					 iTotalPerLevel=totalperl;
					 iTotalPerQPerL=totalperqperl;
					 iTotalPotions=totalpotions;
					 iPotionsPerLevel=potionsperlevel;
					 iTotalGems=totalgems;
					 iGemsPerType=gemspertype;
					 iGemsPerLevel=gemsperlevel;
					 iGemsPerTPerL=gemspertperl;
					 iTotalInfernalKeys=totalkeys;
				}
		  }

        // **********************************************************************************************
        // *****                      Log the nice items we found and stashed                       *****
        // **********************************************************************************************
		  public static void LogGoodItems(CacheACDItem thisgooditem, GilesBaseItemType thisgilesbaseitemtype, GilesItemType thisgilesitemtype, double ithisitemvalue)
        {
            FileStream LogStream = null;
            try
            {
					 
                LogStream = File.Open(sTrinityLogPath + ZetaDia.Service.CurrentHero.BattleTagName + " - StashLog - " + ZetaDia.Actors.Me.ActorClass.ToString() + ".log", FileMode.Append, FileAccess.Write, FileShare.Read);
                using (StreamWriter LogWriter = new StreamWriter(LogStream))
                {
                    if (!bLoggedAnythingThisStash)
                    {
                        bLoggedAnythingThisStash = true;
                        LogWriter.WriteLine(DateTime.Now.ToString() + ":");
                        LogWriter.WriteLine("====================");
                    }
                    string sLegendaryString = "";
                    if (thisgooditem.ThisQuality >= ItemQuality.Legendary)
                    {
                        if (!thisgooditem.IsUnidentified)
                        {
                            AddNotificationToQueue(thisgooditem.ThisRealName + " [" + thisgilesitemtype.ToString() + "] (Score=" + ithisitemvalue.ToString() + ". " + sValueItemStatString + ")", ZetaDia.Service.CurrentHero.Name + " new legendary!", ProwlNotificationPriority.Emergency);
                            sLegendaryString = " {legendary item}";
                            // Change made by bombastic
                            Logging.Write("+=+=+=+=+=+=+=+=+ LEGENDARY FOUND +=+=+=+=+=+=+=+=+");
                            Logging.Write("+  Name:       " + thisgooditem.ThisRealName + " (" + thisgilesitemtype.ToString() + ")");
                            Logging.Write("+  Score:       " + Math.Round(ithisitemvalue).ToString());
                            Logging.Write("+  Attributes: " + sValueItemStatString);
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
                                if (ithisitemvalue >= settings.iNeedPointsToNotifyWeapon)
                                    bShouldNotify = true;
                                break;
                            case GilesBaseItemType.Armor:
                            case GilesBaseItemType.Offhand:
                                if (ithisitemvalue >= settings.iNeedPointsToNotifyArmor)
                                    bShouldNotify = true;
                                break;
                            case GilesBaseItemType.Jewelry:
                                if (ithisitemvalue >= settings.iNeedPointsToNotifyJewelry)
                                    bShouldNotify = true;
                                break;
                        }
                        if (bShouldNotify)
                            AddNotificationToQueue(thisgooditem.ThisRealName + " [" + thisgilesitemtype.ToString() + "] (Score=" + ithisitemvalue.ToString() + ". " + sValueItemStatString + ")", ZetaDia.Service.CurrentHero.Name + " new item!", ProwlNotificationPriority.Emergency);
                    }
                    if (!thisgooditem.IsUnidentified)
                    {
                        LogWriter.WriteLine(thisgilesbaseitemtype.ToString() + " - " + thisgilesitemtype.ToString() + " '" + thisgooditem.ThisRealName + "'. Score = " + Math.Round(ithisitemvalue).ToString() + sLegendaryString);
                        LogWriter.WriteLine("  " + sValueItemStatString);
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
                Log("Fatal Error: File access error for stash log file.");
            }
        }

        // **********************************************************************************************
        // *****                   Log the rubbish junk items we salvaged or sold                   *****
        // **********************************************************************************************
		  public static void LogJunkItems(CacheACDItem thisgooditem, GilesBaseItemType thisgilesbaseitemtype, GilesItemType thisgilesitemtype, double ithisitemvalue)
        {
            FileStream LogStream = null;
            try
            {
                LogStream = File.Open(sTrinityLogPath + ZetaDia.Service.CurrentHero.BattleTagName + " - JunkLog - " + ZetaDia.Actors.Me.ActorClass.ToString() + ".log", FileMode.Append, FileAccess.Write, FileShare.Read);
                using (StreamWriter LogWriter = new StreamWriter(LogStream))
                {
                    if (!bLoggedJunkThisStash)
                    {
                        bLoggedJunkThisStash = true;
                        LogWriter.WriteLine(DateTime.Now.ToString() + ":");
                        LogWriter.WriteLine("====================");
                    }
                    string sLegendaryString = "";
                    if (thisgooditem.ThisQuality >= ItemQuality.Legendary)
                        sLegendaryString = " {legendary item}";
                    LogWriter.WriteLine(thisgilesbaseitemtype.ToString() + " - " + thisgilesitemtype.ToString() + " '" + thisgooditem.ThisRealName + "'. Score = " + Math.Round(ithisitemvalue).ToString() + sLegendaryString);
                    if (!String.IsNullOrEmpty(sJunkItemStatString))
                        LogWriter.WriteLine("  " + sJunkItemStatString);
                    else
                        LogWriter.WriteLine("  (no scorable attributes)");
                    LogWriter.WriteLine("");
                }

            }
            catch (IOException)
            {
                Log("Fatal Error: File access error for junk log file.");
            }
        }

        // **********************************************************************************************
        // *****                              Full Output Of Item Stats                             *****
        // **********************************************************************************************
        private static void OutputReport()
        {
            TimeSpan TotalRunningTime = DateTime.Now.Subtract(ItemStatsWhenStartedBot);
            string sLogFileName = ZetaDia.Service.CurrentHero.BattleTagName + " - Stats - " + ZetaDia.Actors.Me.ActorClass.ToString() + ".log";

            // Create whole new file
            FileStream LogStream = File.Open(sTrinityLogPath + sLogFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            using (StreamWriter LogWriter = new StreamWriter(LogStream))
            {
                LogWriter.WriteLine("===== Misc Statistics =====");
                LogWriter.WriteLine("Total tracking time: " + TotalRunningTime.Hours.ToString() + "h " + TotalRunningTime.Minutes.ToString() +
                    "m " + TotalRunningTime.Seconds.ToString() + "s");
                LogWriter.WriteLine("Total deaths: " + iTotalDeaths.ToString() + " [" + Math.Round(iTotalDeaths / TotalRunningTime.TotalHours, 2).ToString() + " per hour]");
                LogWriter.WriteLine("Total games (approx): " + iTotalLeaveGames.ToString() + " [" + Math.Round(iTotalLeaveGames / TotalRunningTime.TotalHours, 2).ToString() + " per hour]");
                if (iTotalLeaveGames == 0 && iTotalJoinGames > 0)
                {
                    if (iTotalJoinGames == 1 && iTotalProfileRecycles > 1)
                    {
                        LogWriter.WriteLine("(a profile manager/death handler is interfering with join/leave game events, attempting to guess total runs based on profile-loops)");
                        LogWriter.WriteLine("Total full profile cycles: " + iTotalProfileRecycles.ToString() + " [" + Math.Round(iTotalProfileRecycles / TotalRunningTime.TotalHours, 2).ToString() + " per hour]");
                    }
                    else
                    {
                        LogWriter.WriteLine("(your games left value may be bugged @ 0 due to profile managers/routines etc., now showing games joined instead:)");
                        LogWriter.WriteLine("Total games joined: " + iTotalJoinGames.ToString() + " [" + Math.Round(iTotalJoinGames / TotalRunningTime.TotalHours, 2).ToString() + " per hour]");
                    }
                }
                LogWriter.WriteLine("");

                LogWriter.WriteLine("===== Item DROP Statistics =====");
                // Item stats
                if (ItemsDroppedStats.iTotal > 0)
                {
                    LogWriter.WriteLine("Items:");
                    LogWriter.WriteLine("Total items dropped: " + ItemsDroppedStats.iTotal.ToString() + " [" +
                        Math.Round(ItemsDroppedStats.iTotal / TotalRunningTime.TotalHours, 2).ToString() + " per hour]");

                    LogWriter.WriteLine("Items dropped by ilvl: ");
                    for (int iThisLevel = 1; iThisLevel <= 63; iThisLevel++)
                        if (ItemsDroppedStats.iTotalPerLevel[iThisLevel] > 0)
                            LogWriter.WriteLine("- ilvl" + iThisLevel.ToString() + ": " + ItemsDroppedStats.iTotalPerLevel[iThisLevel].ToString() + " [" +
                                Math.Round(ItemsDroppedStats.iTotalPerLevel[iThisLevel] / TotalRunningTime.TotalHours, 2).ToString() + " per hour] {" +
                                Math.Round((ItemsDroppedStats.iTotalPerLevel[iThisLevel] / ItemsDroppedStats.iTotal) * 100, 2).ToString() + " %}");

                    LogWriter.WriteLine("");

                    LogWriter.WriteLine("Items dropped by quality: ");
                    for (int iThisQuality = 0; iThisQuality <= 3; iThisQuality++)
                    {
                        if (ItemsDroppedStats.iTotalPerQuality[iThisQuality] > 0)
                        {
                            LogWriter.WriteLine("- " + sQualityString[iThisQuality] + ": " + ItemsDroppedStats.iTotalPerQuality[iThisQuality].ToString() + " [" + Math.Round(ItemsDroppedStats.iTotalPerQuality[iThisQuality] / TotalRunningTime.TotalHours, 2).ToString() + " per hour] {" + Math.Round((ItemsDroppedStats.iTotalPerQuality[iThisQuality] / ItemsDroppedStats.iTotal) * 100, 2).ToString() + " %}");
                            for (int iThisLevel = 1; iThisLevel <= 63; iThisLevel++)
                                if (ItemsDroppedStats.iTotalPerQPerL[iThisQuality, iThisLevel] > 0)
                                    LogWriter.WriteLine("--- ilvl " + iThisLevel.ToString() + " " + sQualityString[iThisQuality] + ": " + ItemsDroppedStats.iTotalPerQPerL[iThisQuality, iThisLevel].ToString() + " [" + Math.Round(ItemsDroppedStats.iTotalPerQPerL[iThisQuality, iThisLevel] / TotalRunningTime.TotalHours, 2).ToString() + " per hour] {" + Math.Round((ItemsDroppedStats.iTotalPerQPerL[iThisQuality, iThisLevel] / ItemsDroppedStats.iTotal) * 100, 2).ToString() + " %}");
                        } // Any at all this quality?
                    } // For loop on quality
                    LogWriter.WriteLine("");


                } // End of item stats
                // Potion stats
                if (ItemsDroppedStats.iTotalPotions > 0)
                {
                    LogWriter.WriteLine("Potion Drops:");
                    LogWriter.WriteLine("Total potions: " + ItemsDroppedStats.iTotalPotions.ToString() + " [" + Math.Round(ItemsDroppedStats.iTotalPotions / TotalRunningTime.TotalHours, 2).ToString() + " per hour]");
                    for (int iThisLevel = 1; iThisLevel <= 63; iThisLevel++) if (ItemsDroppedStats.iPotionsPerLevel[iThisLevel] > 0)
                            LogWriter.WriteLine("- ilvl " + iThisLevel.ToString() + ": " + ItemsDroppedStats.iPotionsPerLevel[iThisLevel].ToString() + " [" + Math.Round(ItemsDroppedStats.iPotionsPerLevel[iThisLevel] / TotalRunningTime.TotalHours, 2).ToString() + " per hour] {" + Math.Round((ItemsDroppedStats.iPotionsPerLevel[iThisLevel] / ItemsDroppedStats.iTotalPotions) * 100, 2).ToString() + " %}");
                    LogWriter.WriteLine("");
                } // End of potion stats
                // Gem stats
                if (ItemsDroppedStats.iTotalGems > 0)
                {
                    LogWriter.WriteLine("Gem Drops:");
                    LogWriter.WriteLine("Total gems: " + ItemsDroppedStats.iTotalGems.ToString() + " [" + Math.Round(ItemsDroppedStats.iTotalGems / TotalRunningTime.TotalHours, 2).ToString() + " per hour]");
                    for (int iThisGemType = 0; iThisGemType <= 3; iThisGemType++)
                    {
                        if (ItemsDroppedStats.iGemsPerType[iThisGemType] > 0)
                        {
                            LogWriter.WriteLine("- " + sGemString[iThisGemType] + ": " + ItemsDroppedStats.iGemsPerType[iThisGemType].ToString() + " [" + Math.Round(ItemsDroppedStats.iGemsPerType[iThisGemType] / TotalRunningTime.TotalHours, 2).ToString() + " per hour] {" + Math.Round((ItemsDroppedStats.iGemsPerType[iThisGemType] / ItemsDroppedStats.iTotalGems) * 100, 2).ToString() + " %}");
                            for (int iThisLevel = 1; iThisLevel <= 63; iThisLevel++)
                                if (ItemsDroppedStats.iGemsPerTPerL[iThisGemType, iThisLevel] > 0)
                                    LogWriter.WriteLine("--- ilvl " + iThisLevel.ToString() + " " + sGemString[iThisGemType] + ": " + ItemsDroppedStats.iGemsPerTPerL[iThisGemType, iThisLevel].ToString() + " [" + Math.Round(ItemsDroppedStats.iGemsPerTPerL[iThisGemType, iThisLevel] / TotalRunningTime.TotalHours, 2).ToString() + " per hour] {" + Math.Round((ItemsDroppedStats.iGemsPerTPerL[iThisGemType, iThisLevel] / ItemsDroppedStats.iTotalGems) * 100, 2).ToString() + " %}");
                        } // Any at all this quality?
                    } // For loop on quality
                } // End of gem stats
                // Key stats
                if (ItemsDroppedStats.iTotalInfernalKeys > 0)
                {
                    LogWriter.WriteLine("Infernal Key Drops:");
                    LogWriter.WriteLine("Total Keys: " + ItemsDroppedStats.iTotalInfernalKeys.ToString() + " [" + Math.Round(ItemsDroppedStats.iTotalInfernalKeys / TotalRunningTime.TotalHours, 2).ToString() + " per hour]");
                } // End of key stats
                LogWriter.WriteLine("");
                LogWriter.WriteLine("");
                LogWriter.WriteLine("===== Item PICKUP Statistics =====");
                // Item stats
                if (ItemsPickedStats.iTotal > 0)
                {
                    LogWriter.WriteLine("Items:");
                    LogWriter.WriteLine("Total items picked up: " + ItemsPickedStats.iTotal.ToString() + " [" + Math.Round(ItemsPickedStats.iTotal / TotalRunningTime.TotalHours, 2).ToString() + " per hour]");


                    LogWriter.WriteLine("Item picked up by ilvl: ");
                    for (int iThisLevel = 1; iThisLevel <= 63; iThisLevel++)
                        if (ItemsPickedStats.iTotalPerLevel[iThisLevel] > 0)
                            LogWriter.WriteLine("- ilvl" + iThisLevel.ToString() + ": " + ItemsPickedStats.iTotalPerLevel[iThisLevel].ToString() + " [" + Math.Round(ItemsPickedStats.iTotalPerLevel[iThisLevel] / TotalRunningTime.TotalHours, 2).ToString() + " per hour] {" + Math.Round((ItemsPickedStats.iTotalPerLevel[iThisLevel] / ItemsPickedStats.iTotal) * 100, 2).ToString() + " %}");
                    LogWriter.WriteLine("");

                    LogWriter.WriteLine("Items picked up by quality: ");
                    for (int iThisQuality = 0; iThisQuality <= 3; iThisQuality++)
                    {
                        if (ItemsPickedStats.iTotalPerQuality[iThisQuality] > 0)
                        {
                            LogWriter.WriteLine("- " + sQualityString[iThisQuality] + ": " + ItemsPickedStats.iTotalPerQuality[iThisQuality].ToString() + " [" + Math.Round(ItemsPickedStats.iTotalPerQuality[iThisQuality] / TotalRunningTime.TotalHours, 2).ToString() + " per hour] {" + Math.Round((ItemsPickedStats.iTotalPerQuality[iThisQuality] / ItemsPickedStats.iTotal) * 100, 2).ToString() + " %}");
                            for (int iThisLevel = 1; iThisLevel <= 63; iThisLevel++)
                                if (ItemsPickedStats.iTotalPerQPerL[iThisQuality, iThisLevel] > 0)
                                    LogWriter.WriteLine("--- ilvl " + iThisLevel.ToString() + " " + sQualityString[iThisQuality] + ": " + ItemsPickedStats.iTotalPerQPerL[iThisQuality, iThisLevel].ToString() + " [" + Math.Round(ItemsPickedStats.iTotalPerQPerL[iThisQuality, iThisLevel] / TotalRunningTime.TotalHours, 2).ToString() + " per hour] {" + Math.Round((ItemsPickedStats.iTotalPerQPerL[iThisQuality, iThisLevel] / ItemsPickedStats.iTotal) * 100, 2).ToString() + " %}");
                        } // Any at all this quality?
                    } // For loop on quality
                    LogWriter.WriteLine("");
                    if (iTotalFollowerItemsIgnored > 0)
                    {
                        LogWriter.WriteLine("  (note: " + iTotalFollowerItemsIgnored.ToString() + " follower items ignored for being ilvl <60 or blue)");
                    }

                } // End of item stats
                // Potion stats
                if (ItemsPickedStats.iTotalPotions > 0)
                {
                    LogWriter.WriteLine("Potion Pickups:");
                    LogWriter.WriteLine("Total potions: " + ItemsPickedStats.iTotalPotions.ToString() + " [" + Math.Round(ItemsPickedStats.iTotalPotions / TotalRunningTime.TotalHours, 2).ToString() + " per hour]");
                    for (int iThisLevel = 1; iThisLevel <= 63; iThisLevel++) if (ItemsPickedStats.iPotionsPerLevel[iThisLevel] > 0)
                            LogWriter.WriteLine("- ilvl " + iThisLevel.ToString() + ": " + ItemsPickedStats.iPotionsPerLevel[iThisLevel].ToString() + " [" + Math.Round(ItemsPickedStats.iPotionsPerLevel[iThisLevel] / TotalRunningTime.TotalHours, 2).ToString() + " per hour] {" + Math.Round((ItemsPickedStats.iPotionsPerLevel[iThisLevel] / ItemsPickedStats.iTotalPotions) * 100, 2).ToString() + " %}");
                    LogWriter.WriteLine("");
                } // End of potion stats
                // Gem stats
                if (ItemsPickedStats.iTotalGems > 0)
                {
                    LogWriter.WriteLine("Gem Pickups:");
                    LogWriter.WriteLine("Total gems: " + ItemsPickedStats.iTotalGems.ToString() + " [" + Math.Round(ItemsPickedStats.iTotalGems / TotalRunningTime.TotalHours, 2).ToString() + " per hour]");
                    for (int iThisGemType = 0; iThisGemType <= 3; iThisGemType++)
                    {
                        if (ItemsPickedStats.iGemsPerType[iThisGemType] > 0)
                        {
                            LogWriter.WriteLine("- " + sGemString[iThisGemType] + ": " + ItemsPickedStats.iGemsPerType[iThisGemType].ToString() + " [" + Math.Round(ItemsPickedStats.iGemsPerType[iThisGemType] / TotalRunningTime.TotalHours, 2).ToString() + " per hour] {" + Math.Round((ItemsPickedStats.iGemsPerType[iThisGemType] / ItemsPickedStats.iTotalGems) * 100, 2).ToString() + " %}");
                            for (int iThisLevel = 1; iThisLevel <= 63; iThisLevel++)
                                if (ItemsPickedStats.iGemsPerTPerL[iThisGemType, iThisLevel] > 0)
                                    LogWriter.WriteLine("--- ilvl " + iThisLevel.ToString() + " " + sGemString[iThisGemType] + ": " + ItemsPickedStats.iGemsPerTPerL[iThisGemType, iThisLevel].ToString() + " [" + Math.Round(ItemsPickedStats.iGemsPerTPerL[iThisGemType, iThisLevel] / TotalRunningTime.TotalHours, 2).ToString() + " per hour] {" + Math.Round((ItemsPickedStats.iGemsPerTPerL[iThisGemType, iThisLevel] / ItemsPickedStats.iTotalGems) * 100, 2).ToString() + " %}");
                        } // Any at all this quality?
                    } // For loop on quality
                } // End of gem stats
                // Key stats
                if (ItemsPickedStats.iTotalInfernalKeys > 0)
                {
                    LogWriter.WriteLine("Infernal Key Pickups:");
                    LogWriter.WriteLine("Total Keys: " + ItemsPickedStats.iTotalInfernalKeys.ToString() + " [" + Math.Round(ItemsPickedStats.iTotalInfernalKeys / TotalRunningTime.TotalHours, 2).ToString() + " per hour]");
                } // End of key stats
                LogWriter.WriteLine("===== End Of Report =====");
            }
        }



        //Called during the confirmed loot stage of target handling.
        private static void LogItemInformation()
        {
                                    // Store item pickup stats
            if (!_hashsetItemPicksLookedAt.Contains(Bot.Target.ObjectData.RAGUID))
            {
					 CacheItem thisCacheItem=(CacheItem)Bot.Target.ObjectData;
				GilesItemType thisgilesitemtype = DetermineItemType(thisCacheItem.InternalName, thisCacheItem.BalanceData.thisItemType, thisCacheItem.BalanceData.thisFollowerType);
                GilesBaseItemType thisgilesbasetype = DetermineBaseType(thisgilesitemtype);

                if (thisgilesbasetype == GilesBaseItemType.Armor || thisgilesbasetype == GilesBaseItemType.WeaponOneHand || thisgilesbasetype == GilesBaseItemType.WeaponTwoHand ||
                    thisgilesbasetype == GilesBaseItemType.WeaponRange || thisgilesbasetype == GilesBaseItemType.Jewelry || thisgilesbasetype == GilesBaseItemType.FollowerItem ||
                    thisgilesbasetype == GilesBaseItemType.Offhand)
                {
                    int iQuality;
                    ItemsPickedStats.iTotal++;
						  if (thisCacheItem.Itemquality.Value>=ItemQuality.Legendary)
                        iQuality = QUALITYORANGE;
					else if (thisCacheItem.Itemquality.Value>=ItemQuality.Rare4)
                        iQuality = QUALITYYELLOW;
					else if (thisCacheItem.Itemquality.Value >= ItemQuality.Magic1)
                        iQuality = QUALITYBLUE;
                    else
                        iQuality = QUALITYWHITE;
                    ItemsPickedStats.iTotalPerQuality[iQuality]++;
						  ItemsPickedStats.iTotalPerLevel[thisCacheItem.BalanceData.iThisItemLevel]++;
						  ItemsPickedStats.iTotalPerQPerL[iQuality, thisCacheItem.BalanceData.iThisItemLevel]++;
                }
                else if (thisgilesbasetype == GilesBaseItemType.Gem)
                {
                    int iGemType = 0;
                    ItemsPickedStats.iTotalGems++;
                    if (thisgilesitemtype == GilesItemType.Topaz)
                        iGemType = GEMTOPAZ;
                    if (thisgilesitemtype == GilesItemType.Ruby)
                        iGemType = GEMRUBY;
                    if (thisgilesitemtype == GilesItemType.Emerald)
                        iGemType = GEMEMERALD;
                    if (thisgilesitemtype == GilesItemType.Amethyst)
                        iGemType = GEMAMETHYST;

                    ItemsPickedStats.iGemsPerType[iGemType]++;
					ItemsPickedStats.iGemsPerLevel[thisCacheItem.BalanceData.iThisItemLevel]++;
					ItemsPickedStats.iGemsPerTPerL[iGemType, thisCacheItem.BalanceData.iThisItemLevel]++;
                }
                else if (thisgilesitemtype == GilesItemType.HealthPotion)
                {
                    ItemsPickedStats.iTotalPotions++;
						  ItemsPickedStats.iPotionsPerLevel[thisCacheItem.BalanceData.iThisItemLevel]++;
                }
                else if (thisgilesitemtype == GilesItemType.InfernalKey)
                {
                    ItemsPickedStats.iTotalInfernalKeys++;
                }
                // See if we should update the stats file
                if (DateTime.Now.Subtract(ItemStatsLastPostedReport).TotalSeconds > 10)
                {
                    ItemStatsLastPostedReport = DateTime.Now;
                    OutputReport();
                }

                //Herbfunk -- Live loot stats keeping.
				LootedItemLog(thisgilesitemtype, thisgilesbasetype, thisCacheItem.Itemquality.Value);
            }
        }

    }
}