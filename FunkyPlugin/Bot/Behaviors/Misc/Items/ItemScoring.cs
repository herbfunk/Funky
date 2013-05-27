using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta;

namespace FunkyTrinity
{
    public partial class Funky
    {

        // **********************************************************************************************
        // *****             Return the score needed to keep something by the item type             *****
        // **********************************************************************************************
        private static double ScoreNeeded(GilesItemType thisGilesItemType)
        {
            double iThisNeedScore = 0;
            // Weapons
				if (thisGilesItemType==GilesItemType.Axe||thisGilesItemType==GilesItemType.CeremonialKnife||thisGilesItemType==GilesItemType.Dagger||
					 thisGilesItemType==GilesItemType.FistWeapon||thisGilesItemType==GilesItemType.Mace||thisGilesItemType==GilesItemType.MightyWeapon||
					 thisGilesItemType==GilesItemType.Spear||thisGilesItemType==GilesItemType.Sword||thisGilesItemType==GilesItemType.Wand||
					 thisGilesItemType==GilesItemType.TwoHandDaibo||thisGilesItemType==GilesItemType.TwoHandCrossbow||thisGilesItemType==GilesItemType.TwoHandMace||
					 thisGilesItemType==GilesItemType.TwoHandMighty||thisGilesItemType==GilesItemType.TwoHandPolearm||thisGilesItemType==GilesItemType.TwoHandStaff||
					 thisGilesItemType==GilesItemType.TwoHandSword||thisGilesItemType==GilesItemType.TwoHandAxe||thisGilesItemType==GilesItemType.HandCrossbow||thisGilesItemType==GilesItemType.TwoHandBow)
					 iThisNeedScore=SettingsFunky.GilesMinimumWeaponScore;
            // Jewelry
				if (thisGilesItemType==GilesItemType.Ring||thisGilesItemType==GilesItemType.Amulet||thisGilesItemType==GilesItemType.FollowerEnchantress||
					 thisGilesItemType==GilesItemType.FollowerScoundrel||thisGilesItemType==GilesItemType.FollowerTemplar)
					 iThisNeedScore=SettingsFunky.GilesMinimumJeweleryScore;

            // Armor
				if (thisGilesItemType==GilesItemType.Mojo||thisGilesItemType==GilesItemType.Source||thisGilesItemType==GilesItemType.Quiver||
					 thisGilesItemType==GilesItemType.Shield||thisGilesItemType==GilesItemType.Belt||thisGilesItemType==GilesItemType.Boots||
					 thisGilesItemType==GilesItemType.Bracers||thisGilesItemType==GilesItemType.Chest||thisGilesItemType==GilesItemType.Cloak||
					 thisGilesItemType==GilesItemType.Gloves||thisGilesItemType==GilesItemType.Helm||thisGilesItemType==GilesItemType.Pants||
					 thisGilesItemType==GilesItemType.MightyBelt||thisGilesItemType==GilesItemType.Shoulders||thisGilesItemType==GilesItemType.SpiritStone||
					 thisGilesItemType==GilesItemType.VoodooMask||thisGilesItemType==GilesItemType.WizardHat)
					 iThisNeedScore=SettingsFunky.GilesMinimumArmorScore;
            return Math.Round(iThisNeedScore);
        }

        // **********************************************************************************************
        // *****             The bizarre mystery function to score your lovely items!               *****
        // **********************************************************************************************
		  private static double ValueThisItem(CacheACDItem thisitem, GilesItemType thisGilesItemType)
        {
            double iTotalPoints = 0;
            bool bAbandonShip = true;
            double[] iThisItemsMaxStats = new double[TOTALSTATS];
            double[] iThisItemsMaxPoints = new double[TOTALSTATS];
            GilesBaseItemType thisGilesBaseType = DetermineBaseType(thisGilesItemType);

            #region CopyTotalStats

            // One Handed Weapons 
            if (thisGilesItemType == GilesItemType.Axe || thisGilesItemType == GilesItemType.CeremonialKnife || thisGilesItemType == GilesItemType.Dagger ||
                 thisGilesItemType == GilesItemType.FistWeapon || thisGilesItemType == GilesItemType.Mace || thisGilesItemType == GilesItemType.MightyWeapon ||
                 thisGilesItemType == GilesItemType.Spear || thisGilesItemType == GilesItemType.Sword || thisGilesItemType == GilesItemType.Wand)
            {
                Array.Copy(iMaxWeaponOneHand, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iWeaponPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            // Two Handed Weapons
            if (thisGilesItemType == GilesItemType.TwoHandAxe || thisGilesItemType == GilesItemType.TwoHandDaibo || thisGilesItemType == GilesItemType.TwoHandMace ||
                thisGilesItemType == GilesItemType.TwoHandMighty || thisGilesItemType == GilesItemType.TwoHandPolearm || thisGilesItemType == GilesItemType.TwoHandStaff ||
                thisGilesItemType == GilesItemType.TwoHandSword)
            {
                Array.Copy(iMaxWeaponTwoHand, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iWeaponPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            // Ranged Weapons
            if (thisGilesItemType == GilesItemType.TwoHandCrossbow || thisGilesItemType == GilesItemType.TwoHandBow || thisGilesItemType == GilesItemType.HandCrossbow)
            {
                Array.Copy(iMaxWeaponRanged, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iWeaponPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                if (thisGilesItemType == GilesItemType.HandCrossbow)
                {
                    iThisItemsMaxStats[TOTALDPS] -= 150;
                }
                bAbandonShip = false;
            }
            // Off-handed stuff
            // Mojo, Source, Quiver
            if (thisGilesItemType == GilesItemType.Mojo || thisGilesItemType == GilesItemType.Source || thisGilesItemType == GilesItemType.Quiver)
            {
                Array.Copy(iMaxOffHand, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            // Shields
            if (thisGilesItemType == GilesItemType.Shield)
            {
                Array.Copy(iMaxShield, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            // Jewelry
            // Ring
            if (thisGilesItemType == GilesItemType.Amulet)
            {
                Array.Copy(iMaxAmulet, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iJewelryPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            // Ring
            if (thisGilesItemType == GilesItemType.Ring)
            {
                Array.Copy(iMaxRing, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iJewelryPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            // Armor
            // Belt
            if (thisGilesItemType == GilesItemType.Belt)
            {
                Array.Copy(iMaxBelt, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            // Boots
            if (thisGilesItemType == GilesItemType.Boots)
            {
                Array.Copy(iMaxBoots, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            // Bracers
            if (thisGilesItemType == GilesItemType.Bracers)
            {
                Array.Copy(iMaxBracer, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            // Chest
            if (thisGilesItemType == GilesItemType.Chest)
            {
                Array.Copy(iMaxChest, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            if (thisGilesItemType == GilesItemType.Cloak)
            {
                Array.Copy(iMaxCloak, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            // Gloves
            if (thisGilesItemType == GilesItemType.Gloves)
            {
                Array.Copy(iMaxGloves, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            // Helm
            if (thisGilesItemType == GilesItemType.Helm)
            {
                Array.Copy(iMaxHelm, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            // Pants
            if (thisGilesItemType == GilesItemType.Pants)
            {
                Array.Copy(iMaxPants, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            if (thisGilesItemType == GilesItemType.MightyBelt)
            {
                Array.Copy(iMaxMightyBelt, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            // Shoulders
            if (thisGilesItemType == GilesItemType.Shoulders)
            {
                Array.Copy(iMaxShoulders, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            if (thisGilesItemType == GilesItemType.SpiritStone)
            {
                Array.Copy(iMaxSpiritStone, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            if (thisGilesItemType == GilesItemType.VoodooMask)
            {
                Array.Copy(iMaxVoodooMask, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            // Wizard Hat
            if (thisGilesItemType == GilesItemType.WizardHat)
            {
                Array.Copy(iMaxWizardHat, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iArmorPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            // Follower Items
            if (thisGilesItemType == GilesItemType.FollowerEnchantress || thisGilesItemType == GilesItemType.FollowerScoundrel || thisGilesItemType == GilesItemType.FollowerTemplar)
            {
                Array.Copy(iMaxFollower, iThisItemsMaxStats, TOTALSTATS);
                Array.Copy(iJewelryPointsAtMax, iThisItemsMaxPoints, TOTALSTATS);
                bAbandonShip = false;
            }
            #endregion

            // Constants for convenient stat names
            double[] iHadStat = new double[TOTALSTATS] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] iHadPoints = new double[TOTALSTATS] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            double iSafeLifePercentage = 0;

            bool bSocketsCanReplacePrimaries = false;

            double iHighestScoringPrimary = 0;
            int iWhichPrimaryIsHighest = 0;
            double iAmountHighestScoringPrimary = 0;

            // Double safety check for unidentified items
            if (thisitem.IsUnidentified) bAbandonShip = true;

            // Make sure we got a valid item here, otherwise score it a big fat 0
            if (bAbandonShip)
            {
                
                return 0;
            }

            double iGlobalMultiplier = 1;

            sValueItemStatString = "";
            sJunkItemStatString = "";
            // We loop through all of the stats, in a particular order. The order *IS* important, because it pulls up primary stats first, BEFORE other stats
            for (int i = 0; i <= (TOTALSTATS - 1); i++)
            {
                double iTempStatistic = 0;
                // Now we lookup each stat on this item we are scoring, and store it in the variable "iTempStatistic" - which is used for calculations further down

                #region statSwitch
                switch (i)
                {
                    case DEXTERITY: iTempStatistic = thisitem.Dexterity; break;
                    case INTELLIGENCE: iTempStatistic = thisitem.Intelligence; break;
                    case STRENGTH: iTempStatistic = thisitem.Strength; break;
                    case VITALITY: iTempStatistic = thisitem.Vitality; break;
                    case LIFEPERCENT: iTempStatistic = thisitem.LifePercent; break;
                    case LIFEONHIT: iTempStatistic = thisitem.LifeOnHit; break;
                    case LIFESTEAL: iTempStatistic = thisitem.LifeSteal; break;
                    case LIFEREGEN: iTempStatistic = thisitem.HealthPerSecond; break;
                    case MAGICFIND: iTempStatistic = thisitem.MagicFind; break;
                    case GOLDFIND: iTempStatistic = thisitem.GoldFind; break;
                    case MOVEMENTSPEED: iTempStatistic = thisitem.MovementSpeed; break;
                    case PICKUPRADIUS: iTempStatistic = thisitem.PickUpRadius; break;
                    case SOCKETS: iTempStatistic = thisitem.Sockets; break;
                    case CRITCHANCE: iTempStatistic = thisitem.CritPercent; break;
                    case CRITDAMAGE: iTempStatistic = thisitem.CritDamagePercent; break;
                    case ATTACKSPEED: iTempStatistic = thisitem.AttackSpeedPercent; break;
                    case MINDAMAGE: iTempStatistic = thisitem.MinDamage; break;
                    case MAXDAMAGE: iTempStatistic = thisitem.MaxDamage; break;
                    case BLOCKCHANCE: iTempStatistic = thisitem.BlockChance; break;
                    case THORNS: iTempStatistic = thisitem.Thorns; break;
                    case ALLRESIST: iTempStatistic = thisitem.ResistAll; break;
                    case RANDOMRESIST:
                        if (thisitem.ResistArcane > iTempStatistic) iTempStatistic = thisitem.ResistArcane;
                        if (thisitem.ResistCold > iTempStatistic) iTempStatistic = thisitem.ResistCold;
                        if (thisitem.ResistFire > iTempStatistic) iTempStatistic = thisitem.ResistFire;
                        if (thisitem.ResistHoly > iTempStatistic) iTempStatistic = thisitem.ResistHoly;
                        if (thisitem.ResistLightning > iTempStatistic) iTempStatistic = thisitem.ResistLightning;
                        if (thisitem.ResistPhysical > iTempStatistic) iTempStatistic = thisitem.ResistPhysical;
                        if (thisitem.ResistPoison > iTempStatistic) iTempStatistic = thisitem.ResistPoison;
                        break;
                    case TOTALDPS: iTempStatistic = thisitem.WeaponDamagePerSecond; break;
                    case ARMOR: iTempStatistic = thisitem.ArmorBonus; break;
                    case MAXDISCIPLINE: iTempStatistic = thisitem.MaxDiscipline; break;
                    case MAXMANA: iTempStatistic = thisitem.MaxMana; break;
                    case ARCANECRIT: iTempStatistic = thisitem.ArcaneOnCrit; break;
                    case MANAREGEN: iTempStatistic = thisitem.ManaRegen; break;
                    case GLOBEBONUS: iTempStatistic = thisitem.GlobeBonus; break;
                }
                #endregion

                iHadStat[i] = iTempStatistic;
                iHadPoints[i] = 0;
                // Now we check that the current statistic in the "for" loop, actually exists on this item, and is a stat we are measuring (has >0 in the "max stats" array)
                if (iThisItemsMaxStats[i] > 0 && iTempStatistic > 0)
                #region AttributeScoring
                {
                    // Final bonus granted is an end-of-score multiplier. 1 = 100%, so all items start off with 100%, of course!
                    double iFinalBonusGranted = 1;

                    // Temp percent is what PERCENTAGE of the *MAXIMUM POSSIBLE STAT*, this stat is at.
                    // Note that stats OVER the max will get a natural score boost, since this value will be over 1!
                    double iTempPercent = iTempStatistic / iThisItemsMaxStats[i];
                    // Now multiply the "max points" value, by that percentage, as the start/basis of the scoring for this statistic
                    double iTempPoints = iThisItemsMaxPoints[i] * iTempPercent;

                    // Check if this statistic is over the "bonus threshold" array value for this stat - if it is, then it gets a score bonus when over a certain % of max-stat
                    if (iTempPercent > iBonusThreshold[i] && iBonusThreshold[i] > 0f)
                    {
                        iFinalBonusGranted += ((iTempPercent - iBonusThreshold[i]) * 0.9);
                    }

                    // We're going to store the life % stat here for quick-calculations against other stats. Don't edit this bit!
                    if (i == LIFEPERCENT)
                    {
                        if (iThisItemsMaxStats[LIFEPERCENT] > 0)
                        {
                            iSafeLifePercentage = (iTempStatistic / iThisItemsMaxStats[LIFEPERCENT]);
                        }
                        else
                        {
                            iSafeLifePercentage = 0;
                        }
                    }

                    // This *REMOVES* score from follower items for stats that followers don't care about
                    if (thisGilesBaseType == GilesBaseItemType.FollowerItem && (i == CRITDAMAGE || i == LIFEONHIT || i == ALLRESIST))
                        iFinalBonusGranted -= 0.9;

                    // Bonus 15% for being *at* the stat cap (ie - completely maxed out, or very very close to), but not for the socket stat (since sockets are usually 0 or 1!)
                    if (i != SOCKETS)
                    {
                        if ((iTempStatistic / iThisItemsMaxStats[i]) >= 0.99)
                            iFinalBonusGranted += 0.15;
                        // Else bonus 10% for being in final 95%
                        else if ((iTempStatistic / iThisItemsMaxStats[i]) >= 0.95)
                            iFinalBonusGranted += 0.10;
                    }

                    // ***************
                    // Socket handling
                    // ***************
                    // Sockets give special bonuses for certain items, depending how close to the max-socket-count it is for that item
                    // It also enables bonus scoring for stats which usually rely on a high primary stat - since a socket can make up for a lack of a high primary (you can socket a +primary stat!)
                    if (i == SOCKETS)
                    {
                        // Off-handers get less value from sockets
                        if (thisGilesBaseType == GilesBaseItemType.Offhand)
                        {
                            iFinalBonusGranted -= 0.35;
                        }

                        // Chest
                        if (thisGilesItemType == GilesItemType.Chest || thisGilesItemType == GilesItemType.Cloak)
                        {
                            if (iTempStatistic >= 2)
                            {
                                bSocketsCanReplacePrimaries = true;
                                if (iTempStatistic >= 3)
                                    iFinalBonusGranted += 0.25;
                            }
                        }

                        // Pants
                        if (thisGilesItemType == GilesItemType.Pants)
                        {
                            if (iTempStatistic >= 2)
                            {
                                bSocketsCanReplacePrimaries = true;
                                iFinalBonusGranted += 0.25;
                            }
                        }
                        // Helmets can have a bonus for a socket since it gives amazing MF/GF
                        if (iTempStatistic >= 1 && (thisGilesItemType == GilesItemType.Helm || thisGilesItemType == GilesItemType.WizardHat || thisGilesItemType == GilesItemType.VoodooMask ||
                            thisGilesItemType == GilesItemType.SpiritStone))
                        {
                            bSocketsCanReplacePrimaries = true;
                        }
                        // And rings and amulets too
                        if (iTempStatistic >= 1 && (thisGilesItemType == GilesItemType.Ring || thisGilesItemType == GilesItemType.Amulet))
                        {
                            bSocketsCanReplacePrimaries = true;
                        }

                    }

                    // Right, here's quite a long bit of code, but this is basically all about granting all sorts of bonuses based on primary stat values of all different ranges
                    // For all item types *EXCEPT* weapons
                    if (thisGilesBaseType != GilesBaseItemType.WeaponRange && thisGilesBaseType != GilesBaseItemType.WeaponOneHand && thisGilesBaseType != GilesBaseItemType.WeaponTwoHand)
                    {
                        double iSpecialBonus = 0;
                        if (i > LIFEPERCENT)
                        {
                            // Knock off points for being particularly low
                            if ((iTempStatistic / iThisItemsMaxStats[i]) < 0.2 && (iBonusThreshold[i] <= 0f || iBonusThreshold[i] >= 0.2))
                                iFinalBonusGranted -= 0.35;
                            else if ((iTempStatistic / iThisItemsMaxStats[i]) < 0.4 && (iBonusThreshold[i] <= 0f || iBonusThreshold[i] >= 0.4))
                                iFinalBonusGranted -= 0.15;
                            // Remove 80% if below minimum threshold
                            if ((iTempStatistic / iThisItemsMaxStats[i]) < iMinimumThreshold[i] && iMinimumThreshold[i] > 0f)
                                iFinalBonusGranted -= 0.8;

                            // Primary stat/vitality minimums or zero-check reductions on other stats
                            if (iStatMinimumPrimary[i] > 0)
                            {
                                // Remove 40% from all stats if there is no prime stat present or vitality/life present and this is below 90% of max
                                if (((iTempStatistic / iThisItemsMaxStats[i]) < .90) && ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) < iStatMinimumPrimary[i]) &&
                                    ((iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) < (iStatMinimumPrimary[i] + 0.1)) && ((iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) < iStatMinimumPrimary[i]) &&
                                    ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) < iStatMinimumPrimary[i]) && (iSafeLifePercentage < (iStatMinimumPrimary[i] * 2.5)) && !bSocketsCanReplacePrimaries)
                                {
                                    if (thisGilesItemType != GilesItemType.Ring && thisGilesItemType != GilesItemType.Amulet)
                                        iFinalBonusGranted -= 0.4;
                                    else
                                        iFinalBonusGranted -= 0.3;
                                    // And another 25% off for armor and all resist which are more useful with primaries, as long as not jewelry
                                    if ((i == ARMOR || i == ALLRESIST || i == RANDOMRESIST) && thisGilesItemType != GilesItemType.Ring && thisGilesItemType != GilesItemType.Amulet && !bSocketsCanReplacePrimaries)
                                        iFinalBonusGranted -= 0.15;
                                }
                            }
                            else
                            {
                                // Almost no primary stats or health at all
                                if (iHadStat[DEXTERITY] <= 60 && iHadStat[STRENGTH] <= 60 && iHadStat[INTELLIGENCE] <= 60 && iHadStat[VITALITY] <= 60 && iSafeLifePercentage < 0.9 && !bSocketsCanReplacePrimaries)
                                {
                                    // So 35% off for all items except jewelry which is 20% off
                                    if (thisGilesItemType != GilesItemType.Ring && thisGilesItemType != GilesItemType.Amulet)
                                    {
                                        iFinalBonusGranted -= 0.35;
                                        // And another 25% off for armor and all resist which are more useful with primaries
                                        if (i == ARMOR || i == ALLRESIST)
                                            iFinalBonusGranted -= 0.15;
                                    }
                                    else
                                    {
                                        iFinalBonusGranted -= 0.20;
                                    }

                                }
                            }

                            if (thisGilesBaseType == GilesBaseItemType.Armor || thisGilesBaseType == GilesBaseItemType.Jewelry)
                            {
                                // Grant a 50% bonus to stats if a primary is above 200 AND (vitality above 200 or life% within 90% max)
                                if ((iHadStat[DEXTERITY] > 200 || iHadStat[STRENGTH] > 200 || iHadStat[INTELLIGENCE] > 200) && (iHadStat[VITALITY] > 200 || iSafeLifePercentage > .97))
                                {
                                    if (0.5 > iSpecialBonus) iSpecialBonus = 0.5;
                                }
                                // Else grant a 40% bonus to stats if a primary is above 200
                                if (iHadStat[DEXTERITY] > 200 || iHadStat[STRENGTH] > 200 || iHadStat[INTELLIGENCE] > 200)
                                {
                                    if (0.4 > iSpecialBonus) iSpecialBonus = 0.4;
                                }
                                // Grant a 30% bonus if vitality > 200 or life percent within 90% of max
                                if (iHadStat[VITALITY] > 200 || iSafeLifePercentage > .97)
                                {
                                    if (0.3 > iSpecialBonus) iSpecialBonus = 0.3;
                                }
                            }

                            // Checks for various primary & health levels
                            if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > .85 || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > .85
                                || (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > .85)
                            {
                                if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .6 || iSafeLifePercentage > .90)
                                {
                                    if (0.5 > iSpecialBonus) iSpecialBonus = 0.5;
                                }
                                else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .35 || iSafeLifePercentage > .85)
                                {
                                    if (0.4 > iSpecialBonus) iSpecialBonus = 0.4;
                                }
                                else
                                {
                                    if (0.2 > iSpecialBonus) iSpecialBonus = 0.2;
                                }
                            }
                            if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > .75 || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > .75
                                || (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > .75)
                            {
                                if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .6 || iSafeLifePercentage > .90)
                                {
                                    if (0.35 > iSpecialBonus) iSpecialBonus = 0.35;
                                }
                                else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .35 || iSafeLifePercentage > .85)
                                {
                                    if (0.30 > iSpecialBonus) iSpecialBonus = 0.30;
                                }
                                else
                                {
                                    if (0.15 > iSpecialBonus) iSpecialBonus = 0.15;
                                }
                            }
                            if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > .65 || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > .65
                                || (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > .65)
                            {
                                if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .6 || iSafeLifePercentage > .90)
                                {
                                    if (0.26 > iSpecialBonus) iSpecialBonus = 0.26;
                                }
                                else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .35 || iSafeLifePercentage > .85)
                                {
                                    if (0.22 > iSpecialBonus) iSpecialBonus = 0.22;
                                }
                                else
                                {
                                    if (0.11 > iSpecialBonus) iSpecialBonus = 0.11;
                                }
                            }
                            if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > .55 || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > .55
                                || (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > .55)
                            {
                                if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .6 || iSafeLifePercentage > .90)
                                {
                                    if (0.18 > iSpecialBonus) iSpecialBonus = 0.18;
                                }
                                else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .35 || iSafeLifePercentage > .85)
                                {
                                    if (0.14 > iSpecialBonus) iSpecialBonus = 0.14;
                                }
                                else
                                {
                                    if (0.08 > iSpecialBonus) iSpecialBonus = 0.08;
                                }
                            }
                            if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > .5 || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > .5
                                || (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > .5)
                            {
                                if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .6 || iSafeLifePercentage > .90)
                                {
                                    if (0.12 > iSpecialBonus) iSpecialBonus = 0.12;
                                }
                                else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .35 || iSafeLifePercentage > .85)
                                {
                                    if (0.05 > iSpecialBonus) iSpecialBonus = 0.05;
                                }
                                else
                                {
                                    if (0.03 > iSpecialBonus) iSpecialBonus = 0.03;
                                }
                            }
                            if (thisGilesItemType == GilesItemType.Ring || thisGilesItemType == GilesItemType.Amulet)
                            {
                                if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > .4 || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > .4
                                    || (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > .4)
                                {
                                    if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .6 || iSafeLifePercentage > .90)
                                    {
                                        if (0.10 > iSpecialBonus) iSpecialBonus = 0.10;
                                    }
                                    else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .35 || iSafeLifePercentage > .85)
                                    {
                                        if (0.08 > iSpecialBonus) iSpecialBonus = 0.08;
                                    }
                                    else
                                    {
                                        if (0.05 > iSpecialBonus) iSpecialBonus = 0.05;
                                    }
                                }
                            }
                            if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .8 || iSafeLifePercentage > .98)
                            {
                                if (0.20 > iSpecialBonus) iSpecialBonus = 0.20;
                            }
                            if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .7 || iSafeLifePercentage > .95)
                            {
                                if (0.16 > iSpecialBonus) iSpecialBonus = 0.16;
                            }
                            if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .6 || iSafeLifePercentage > .92)
                            {
                                if (0.12 > iSpecialBonus) iSpecialBonus = 0.12;
                            }
                            if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .55 || iSafeLifePercentage > .89)
                            {
                                if (0.07 > iSpecialBonus) iSpecialBonus = 0.07;
                            }
                            else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .5 || iSafeLifePercentage > .87)
                            {
                                if (0.05 > iSpecialBonus) iSpecialBonus = 0.05;
                            }
                            else if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > .45 || iSafeLifePercentage > .86)
                            {
                                if (0.02 > iSpecialBonus) iSpecialBonus = 0.02;
                            }
                        } // This stat is one after life percent stat
                        // Shields get less of a special bonus from high prime stats
                        if (thisGilesItemType == GilesItemType.Shield)
                            iSpecialBonus *= 0.7;
                        
                        iFinalBonusGranted += iSpecialBonus;
                    } // NOT A WEAPON!?

                    // Knock off points for being particularly low
                    if ((iTempStatistic / iThisItemsMaxStats[i]) < iMinimumThreshold[i] && iMinimumThreshold[i] > 0f)
                        iFinalBonusGranted -= 0.35;
                    // Grant a 20% bonus to vitality or Life%, for being paired with any prime stat above minimum threshold +.1
                    if (((i == VITALITY && (iTempStatistic / iThisItemsMaxStats[VITALITY]) > iMinimumThreshold[VITALITY]) ||
                          i == LIFEPERCENT && (iTempStatistic / iThisItemsMaxStats[LIFEPERCENT]) > iMinimumThreshold[LIFEPERCENT]) &&
                        ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > (iMinimumThreshold[DEXTERITY] + 0.1)
                        || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > (iMinimumThreshold[STRENGTH] + 0.1) ||
                         (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > (iMinimumThreshold[INTELLIGENCE] + 0.1)))
                        iFinalBonusGranted += 0.2;

                    // Blue item point reduction for non-weapons
                    if (thisitem.ThisQuality < ItemQuality.Rare4 && (thisGilesBaseType == GilesBaseItemType.Armor || thisGilesBaseType == GilesBaseItemType.Offhand ||
                        thisGilesBaseType == GilesBaseItemType.Jewelry || thisGilesBaseType == GilesBaseItemType.FollowerItem) && ((iTempStatistic / iThisItemsMaxStats[i]) < 0.88))
                        iFinalBonusGranted -= 0.9;

                    // Special all-resist bonuses
                    if (i == ALLRESIST)
                    {
                        // Shields with < 60% max all resist, lost some all resist score
                        if (thisGilesItemType == GilesItemType.Shield && (iTempStatistic / iThisItemsMaxStats[i]) <= 0.6)
                            iFinalBonusGranted -= 0.30;

                        double iSpecialBonus = 0;
                        // All resist gets a special bonus if paired with good strength and some vitality
                        if ((iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > 0.7 && (iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > 0.3)
                            if (0.45 > iSpecialBonus) iSpecialBonus = 0.45;
                        // All resist gets a smaller special bonus if paired with good dexterity and some vitality
                        if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > 0.7 && (iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > 0.3)
                            if (0.35 > iSpecialBonus) iSpecialBonus = 0.35;
                        // All resist gets a slight special bonus if paired with good intelligence and some vitality
                        if ((iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > 0.7 && (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > 0.3)
                            if (0.25 > iSpecialBonus) iSpecialBonus = 0.25;

                        // Smaller bonuses for smaller stats
                        // All resist gets a special bonus if paired with good strength and some vitality
                        if ((iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > 0.55 && (iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > 0.3)
                            if (0.45 > iSpecialBonus) iSpecialBonus = 0.20;
                        // All resist gets a smaller special bonus if paired with good dexterity and some vitality
                        if ((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > 0.55 && (iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > 0.3)
                            if (0.35 > iSpecialBonus) iSpecialBonus = 0.15;
                        // All resist gets a slight special bonus if paired with good intelligence and some vitality
                        if ((iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > 0.55 && (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > 0.3)
                            if (0.25 > iSpecialBonus) iSpecialBonus = 0.10;

                        // This stat is one after life percent stat
                        iFinalBonusGranted += iSpecialBonus;

                        // Global bonus to everything
                        if ((iThisItemsMaxStats[i] - iTempStatistic) < 10.2f)
                            iGlobalMultiplier += 0.05;
                    } // All resist special bonuses

                    if (thisGilesItemType != GilesItemType.Ring && thisGilesItemType != GilesItemType.Amulet)
                    {
                        // Shields get 10% less on everything
                        if (thisGilesItemType == GilesItemType.Shield)
                            iFinalBonusGranted -= 0.10;

                        // Prime stat gets a 20% bonus if 50 from max possible
                        if ((i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE || i == VITALITY) && (iThisItemsMaxStats[i] - iTempStatistic) < 50.5f)
                            iFinalBonusGranted += 0.25;

                        // Reduce a prime stat by 75% if less than 100 *OR* less than 50% max
                        if ((i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE) && (iTempStatistic < 100 || ((iTempStatistic / iThisItemsMaxStats[i]) < 0.5)))
                            iFinalBonusGranted -= 0.75;
                        // Reduce a vitality/life% stat by 60% if less than 80 vitality/less than 60% max possible life%
                        if ((i == VITALITY && iTempStatistic < 80) || (i == LIFEPERCENT && ((iTempStatistic / iThisItemsMaxStats[LIFEPERCENT]) < 0.6)))
                            iFinalBonusGranted -= 0.6;
                        // Grant 10% to any 4 main stat above 200
                        if ((i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE || i == VITALITY) && iTempStatistic > 200)
                            iFinalBonusGranted += 0.1;

                        // *************************************************
                        // Special stat handling stuff for non-jewelry types
                        // *************************************************
                        // Within 2 block chance
                        if (i == BLOCKCHANCE && (iThisItemsMaxStats[i] - iTempStatistic) < 2.3f)
                            iFinalBonusGranted += 1;

                        // Within final 5 gold find
                        if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 5.3f)
                        {
                            iFinalBonusGranted += 0.04;
                            // Even bigger bonus if got prime stat & vit
                            if (((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > iMinimumThreshold[DEXTERITY] || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > iMinimumThreshold[STRENGTH] ||
                                (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > iMinimumThreshold[INTELLIGENCE]) && (iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > iMinimumThreshold[VITALITY])
                                iFinalBonusGranted += 0.02;
                        }
                        // Within final 3 gold find
                        if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 3.3f)
                        {
                            iFinalBonusGranted += 0.04;
                        }
                        // Within final 2 gold find
                        if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 2.3f)
                        {
                            iFinalBonusGranted += 0.05;
                        }
                        // Within final 3 magic find
                        if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 3.3f)
                            iFinalBonusGranted += 0.08;
                        // Within final 2 magic find
                        if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 2.3f)
                        {
                            iFinalBonusGranted += 0.04;
                            // Even bigger bonus if got prime stat & vit
                            if (((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > iMinimumThreshold[DEXTERITY] || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > iMinimumThreshold[STRENGTH] ||
                                (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > iMinimumThreshold[INTELLIGENCE]) && (iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > iMinimumThreshold[VITALITY])
                                iFinalBonusGranted += 0.03;
                        }
                        // Within final magic find
                        if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 1.3f)
                        {
                            iFinalBonusGranted += 0.05;
                        }
                        // Within final 10 all resist
                        if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) < 10.2f)
                        {
                            iFinalBonusGranted += 0.05;
                            // Even bigger bonus if got prime stat & vit
                            if (((iHadStat[DEXTERITY] / iThisItemsMaxStats[DEXTERITY]) > iMinimumThreshold[DEXTERITY] || (iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > iMinimumThreshold[STRENGTH] ||
                                (iHadStat[INTELLIGENCE] / iThisItemsMaxStats[INTELLIGENCE]) > iMinimumThreshold[INTELLIGENCE]) && (iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY]) > iMinimumThreshold[VITALITY])
                                iFinalBonusGranted += 0.20;
                        }
                        // Within final 50 armor
                        if (i == ARMOR && (iThisItemsMaxStats[i] - iTempStatistic) < 50.2f)
                        {
                            iFinalBonusGranted += 0.10;
                            if ((iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > iMinimumThreshold[STRENGTH])
                                iFinalBonusGranted += 0.10;
                        }
                        // Within final 15 armor
                        if (i == ARMOR && (iThisItemsMaxStats[i] - iTempStatistic) < 15.2f)
                            iFinalBonusGranted += 0.15;

                        // Within final 5 critical hit damage
                        if (i == CRITDAMAGE && (iThisItemsMaxStats[i] - iTempStatistic) < 5.2f)
                            iFinalBonusGranted += 0.25;
                        // More than 2.5 crit chance out
                        if (i == CRITCHANCE && (iThisItemsMaxStats[i] - iTempStatistic) > 2.45f)
                            iFinalBonusGranted -= 0.35;
                        // More than 20 crit damage out
                        if (i == CRITDAMAGE && (iThisItemsMaxStats[i] - iTempStatistic) > 19.95f)
                            iFinalBonusGranted -= 0.35;
                        // More than 2 attack speed out
                        if (i == ATTACKSPEED && (iThisItemsMaxStats[i] - iTempStatistic) > 1.95f)
                            iFinalBonusGranted -= 0.35;
                        // More than 2 move speed
                        if (i == MOVEMENTSPEED && (iThisItemsMaxStats[i] - iTempStatistic) > 1.95f)
                            iFinalBonusGranted -= 0.35;
                        // More than 5 gold find out
                        if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 5.2f)
                            iFinalBonusGranted -= 0.40;
                        // More than 8 gold find out
                        if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 8.2f)
                            iFinalBonusGranted -= 0.1;
                        // More than 5 magic find out
                        if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 5.2f)
                            iFinalBonusGranted -= 0.40;
                        // More than 7 magic find out
                        if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 7.2f)
                            iFinalBonusGranted -= 0.1;
                        // More than 20 all resist out
                        if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) > 20.2f)
                            iFinalBonusGranted -= 0.50;
                        // More than 30 all resist out
                        if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) > 30.2f)
                            iFinalBonusGranted -= 0.20;
                    }
                    // And now for jewelry checks...
                    else
                    {
                        // Global bonus to everything if jewelry has an all resist above 50%
                        if (i == ALLRESIST && (iTempStatistic / iThisItemsMaxStats[i]) > 0.5)
                            iGlobalMultiplier += 0.08;
                        // Within final 10 all resist
                        if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) < 10.2f)
                            iFinalBonusGranted += 0.10;

                        // Within final 5 critical hit damage
                        if (i == CRITDAMAGE && (iThisItemsMaxStats[i] - iTempStatistic) < 5.2f)
                            iFinalBonusGranted += 0.25;

                        // Within 3 block chance
                        if (i == BLOCKCHANCE && (iThisItemsMaxStats[i] - iTempStatistic) < 3.3f)
                            iFinalBonusGranted += 0.15;

                        // Reduce a prime stat by 60% if less than 60
                        if ((i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE) && (iTempStatistic < 60 || ((iTempStatistic / iThisItemsMaxStats[i]) < 0.3)))
                            iFinalBonusGranted -= 0.6;
                        // Reduce a vitality/life% stat by 50% if less than 50 vitality/less than 40% max possible life%
                        if ((i == VITALITY && iTempStatistic < 50) || (i == LIFEPERCENT && ((iTempStatistic / iThisItemsMaxStats[LIFEPERCENT]) < 0.4)))
                            iFinalBonusGranted -= 0.5;
                        // Grant 20% to any 4 main stat above 150
                        if ((i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE || i == VITALITY) && iTempStatistic > 150)
                            iFinalBonusGranted += 0.2;


                        // ***************************************
                        // Special stat handling stuff for jewelry
                        // ***************************************
                        if (thisGilesItemType == GilesItemType.Ring)
                        {
                            // Prime stat gets a 25% bonus if 30 from max possible
                            if ((i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE || i == VITALITY) && (iThisItemsMaxStats[i] - iTempStatistic) < 30.5f)
                                iFinalBonusGranted += 0.25;

                            // Within final 5 magic find
                            if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 5.2f)
                                iFinalBonusGranted += 0.4;
                            // Within final 5 gold find
                            if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 5.2f)
                                iFinalBonusGranted += 0.35;
                            // Within final 45 life on hit
                            if (i == LIFEONHIT && (iThisItemsMaxStats[i] - iTempStatistic) < 45.2f)
                                iFinalBonusGranted += 1.2;
                            // Within final 50 armor
                            if (i == ARMOR && (iThisItemsMaxStats[i] - iTempStatistic) < 50.2f)
                            {
                                iFinalBonusGranted += 0.30;
                                if ((iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > iMinimumThreshold[STRENGTH])
                                    iFinalBonusGranted += 0.30;
                            }
                            // Within final 15 armor
                            if (i == ARMOR && (iThisItemsMaxStats[i] - iTempStatistic) < 15.2f)
                                iFinalBonusGranted += 0.20;

                            // More than 2.5 crit chance out
                            if (i == CRITCHANCE && (iThisItemsMaxStats[i] - iTempStatistic) > 5.55f)
                                iFinalBonusGranted -= 0.20;
                            // More than 20 crit damage out
                            if (i == CRITDAMAGE && (iThisItemsMaxStats[i] - iTempStatistic) > 19.95f)
                                iFinalBonusGranted -= 0.20;
                            // More than 2 attack speed out
                            if (i == ATTACKSPEED && (iThisItemsMaxStats[i] - iTempStatistic) > 1.95f)
                                iFinalBonusGranted -= 0.20;
                            // More than 15 gold find out
                            if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 15.2f)
                                iFinalBonusGranted -= 0.1;
                            // More than 15 magic find out
                            if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 15.2f)
                                iFinalBonusGranted -= 0.1;
                            // More than 30 all resist out
                            if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) > 20.2f)
                                iFinalBonusGranted -= 0.1;
                            // More than 40 all resist out
                            if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) > 30.2f)
                                iFinalBonusGranted -= 0.1;
                        }
                        else
                        {
                            // Prime stat gets a 25% bonus if 60 from max possible
                            if ((i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE || i == VITALITY) && (iThisItemsMaxStats[i] - iTempStatistic) < 60.5f)
                                iFinalBonusGranted += 0.25;

                            // Within final 10 magic find
                            if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 10.2f)
                                iFinalBonusGranted += 0.4;
                            // Within final 10 gold find
                            if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) < 10.2f)
                                iFinalBonusGranted += 0.35;
                            // Within final 40 life on hit
                            if (i == LIFEONHIT && (iThisItemsMaxStats[i] - iTempStatistic) < 40.2f)
                                iFinalBonusGranted += 1.2;
                            // Within final 50 armor
                            if (i == ARMOR && (iThisItemsMaxStats[i] - iTempStatistic) < 50.2f)
                            {
                                iFinalBonusGranted += 0.30;
                                if ((iHadStat[STRENGTH] / iThisItemsMaxStats[STRENGTH]) > iMinimumThreshold[STRENGTH])
                                    iFinalBonusGranted += 0.30;
                            }
                            // Within final 15 armor
                            if (i == ARMOR && (iThisItemsMaxStats[i] - iTempStatistic) < 15.2f)
                                iFinalBonusGranted += 0.20;

                            // More than 2.5 crit chance out
                            if (i == CRITCHANCE && (iThisItemsMaxStats[i] - iTempStatistic) > 5.55f)
                                iFinalBonusGranted -= 0.20;
                            // More than 20 crit damage out
                            if (i == CRITDAMAGE && (iThisItemsMaxStats[i] - iTempStatistic) > 19.95f)
                                iFinalBonusGranted -= 0.20;
                            // More than 2 attack speed out
                            if (i == ATTACKSPEED && (iThisItemsMaxStats[i] - iTempStatistic) > 1.95f)
                                iFinalBonusGranted -= 0.20;
                            // More than 15 gold find out
                            if (i == GOLDFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 15.2f)
                                iFinalBonusGranted -= 0.1;
                            // More than 15 magic find out
                            if (i == MAGICFIND && (iThisItemsMaxStats[i] - iTempStatistic) > 15.2f)
                                iFinalBonusGranted -= 0.1;
                            // More than 30 all resist out
                            if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) > 20.2f)
                                iFinalBonusGranted -= 0.1;
                            // More than 40 all resist out
                            if (i == ALLRESIST && (iThisItemsMaxStats[i] - iTempStatistic) > 30.2f)
                                iFinalBonusGranted -= 0.1;
                        }
                    }

                    // *****************************
                    // All the "set to 0" checks now
                    // *****************************

                    // Disable specific primary stat scoring for certain class-specific item types
                    if ((thisGilesItemType == GilesItemType.VoodooMask || thisGilesItemType == GilesItemType.WizardHat || thisGilesItemType == GilesItemType.Wand ||
                        thisGilesItemType == GilesItemType.CeremonialKnife || thisGilesItemType == GilesItemType.Mojo || thisGilesItemType == GilesItemType.Source)
                        && (i == STRENGTH || i == DEXTERITY))
                        iFinalBonusGranted = 0;
                    if ((thisGilesItemType == GilesItemType.Quiver || thisGilesItemType == GilesItemType.HandCrossbow || thisGilesItemType == GilesItemType.Cloak ||
                        thisGilesItemType == GilesItemType.SpiritStone || thisGilesItemType == GilesItemType.TwoHandDaibo || thisGilesItemType == GilesItemType.FistWeapon)
                        && (i == STRENGTH || i == INTELLIGENCE))
                        iFinalBonusGranted = 0;
                    if ((thisGilesItemType == GilesItemType.MightyBelt || thisGilesItemType == GilesItemType.MightyWeapon || thisGilesItemType == GilesItemType.TwoHandMighty)
                        && (i == DEXTERITY || i == INTELLIGENCE))
                        iFinalBonusGranted = 0;
                    // Remove unwanted follower stats for specific follower types
                    if (thisGilesItemType == GilesItemType.FollowerEnchantress && (i == STRENGTH || i == DEXTERITY))
                        iFinalBonusGranted = 0;
                    if (thisGilesItemType == GilesItemType.FollowerEnchantress && (i == INTELLIGENCE || i == VITALITY))
                        iFinalBonusGranted -= 0.4;
                    if (thisGilesItemType == GilesItemType.FollowerScoundrel && (i == STRENGTH || i == INTELLIGENCE))
                        iFinalBonusGranted = 0;
                    if (thisGilesItemType == GilesItemType.FollowerScoundrel && (i == DEXTERITY || i == VITALITY))
                        iFinalBonusGranted -= 0.4;
                    if (thisGilesItemType == GilesItemType.FollowerTemplar && (i == DEXTERITY || i == INTELLIGENCE))
                        iFinalBonusGranted = 0;
                    if (thisGilesItemType == GilesItemType.FollowerTemplar && (i == STRENGTH || i == VITALITY))
                        iFinalBonusGranted -= 0.4;
                    // Attack speed is always on a quiver so forget it
                    if ((thisGilesItemType == GilesItemType.Quiver) && (i == ATTACKSPEED))
                        iFinalBonusGranted = 0;
                    // Single resists worth nothing without all-resist
                    if (i == RANDOMRESIST && (iHadStat[ALLRESIST] / iThisItemsMaxStats[ALLRESIST]) < iMinimumThreshold[ALLRESIST])
                        iFinalBonusGranted = 0;

                    if (iFinalBonusGranted < 0)
                        iFinalBonusGranted = 0;

                    // ***************************
                    // Grant the final bonus total
                    // ***************************
                    iTempPoints *= iFinalBonusGranted;

                    // If it's a primary stat, log the highest scoring primary... else add these points to the running total
                    if (i == DEXTERITY || i == STRENGTH || i == INTELLIGENCE)
                    {
                        if (iTempPoints > iHighestScoringPrimary)
                        {
                            iHighestScoringPrimary = iTempPoints;
                            iWhichPrimaryIsHighest = i;
                            iAmountHighestScoringPrimary = iTempStatistic;
                        }
                    }
                    else
                    {
                      
                        iTotalPoints += iTempPoints;
                    }

                    iHadPoints[i] = iTempPoints;

                    // For item logs
                    if (i != DEXTERITY && i != STRENGTH && i != INTELLIGENCE)
                    {
								if (String.IsNullOrEmpty(sValueItemStatString))
                            sValueItemStatString += ". ";
                        sValueItemStatString += StatNames[i] + "=" + Math.Round(iTempStatistic).ToString();
								if (!String.IsNullOrEmpty(sJunkItemStatString))
                            sJunkItemStatString += ". ";
                        sJunkItemStatString += StatNames[i] + "=" + Math.Round(iTempStatistic).ToString();
                    }
                }
                #endregion
            } // End of main 0-TOTALSTATS stat loop



            int iTotalRequirements;
            // Now add on one of the three primary stat scores, whichever was higher
            if (iHighestScoringPrimary > 0)
            {
                // Give a 30% of primary-stat-score-possible bonus to the primary scoring if paired with a good amount of life % or vitality
                if ((iHadStat[VITALITY] / iThisItemsMaxStats[VITALITY] > (iMinimumThreshold[VITALITY] + 0.1)) || iSafeLifePercentage > 0.85)
                    iHighestScoringPrimary += iThisItemsMaxPoints[iWhichPrimaryIsHighest] * 0.3;
                // Reduce a primary a little if there is no vitality or life
                if ((iHadStat[VITALITY] < 40) || iSafeLifePercentage < 0.7)
                    iHighestScoringPrimary *= 0.8;

                iTotalPoints += iHighestScoringPrimary;
                sValueItemStatString = StatNames[iWhichPrimaryIsHighest] + "=" + Math.Round(iAmountHighestScoringPrimary).ToString() + ". " + sValueItemStatString;
                sJunkItemStatString = StatNames[iWhichPrimaryIsHighest] + "=" + Math.Round(iAmountHighestScoringPrimary).ToString() + ". " + sJunkItemStatString;
            }


            // Global multiplier
            iTotalPoints *= iGlobalMultiplier;

            // 2 handed weapons and ranged weapons lose a large score for low DPS
            if (thisGilesBaseType == GilesBaseItemType.WeaponRange || thisGilesBaseType == GilesBaseItemType.WeaponTwoHand)
            {
                if ((iHadStat[TOTALDPS] / iThisItemsMaxStats[TOTALDPS]) <= 0.7)
                    iTotalPoints *= 0.75;
            }
            else if (thisGilesBaseType == GilesBaseItemType.WeaponOneHand)
            {
                if ((iHadStat[TOTALDPS] / iThisItemsMaxStats[TOTALDPS]) < 0.6)
                    iTotalPoints *= 0.75;
            }

            // Weapons should get a nice 15% bonus score for having very high primaries
            if (thisGilesBaseType == GilesBaseItemType.WeaponRange || thisGilesBaseType == GilesBaseItemType.WeaponOneHand || thisGilesBaseType == GilesBaseItemType.WeaponTwoHand)
            {
                if (iHighestScoringPrimary > 0 && (iHighestScoringPrimary >= iThisItemsMaxPoints[iWhichPrimaryIsHighest] * 0.9))
                {
                    iTotalPoints *= 1.15;
                }
                // And an extra 15% for a very high vitality
                if (iHadStat[VITALITY] > 0 && (iHadStat[VITALITY] >= iThisItemsMaxPoints[VITALITY] * 0.9))
                {
                    iTotalPoints *= 1.15;
                }
                // And an extra 15% for a very high life-on-hit
                if (iHadStat[LIFEONHIT] > 0 && (iHadStat[LIFEONHIT] >= iThisItemsMaxPoints[LIFEONHIT] * 0.9))
                {
                    iTotalPoints *= 1.15;
                }
            }

            // Shields 
            if (thisGilesItemType == GilesItemType.Shield)
            {
                // Strength/Dex based shield calculations
                if (iWhichPrimaryIsHighest == STRENGTH || iWhichPrimaryIsHighest == DEXTERITY)
                {
                    if (iHadStat[BLOCKCHANCE] < 20)
                    {
                        iTotalPoints *= 0.7;
                    }
                    else if (iHadStat[BLOCKCHANCE] < 25)
                    {
                        iTotalPoints *= 0.9;
                    }
                }
                // Intelligence/no primary based shields
                else
                {
                    if (iHadStat[BLOCKCHANCE] < 28)
                        iTotalPoints -= iHadPoints[BLOCKCHANCE];
                }
            }

            // Quivers
            if (thisGilesItemType == GilesItemType.Quiver)
            {
                iTotalRequirements = 0;
                if (iHadStat[DEXTERITY] >= 100)
                    iTotalRequirements++;
                else
                    iTotalRequirements -= 3;
                if (iHadStat[DEXTERITY] >= 160)
                    iTotalRequirements++;
                if (iHadStat[DEXTERITY] >= 250)
                    iTotalRequirements++;
                if (iHadStat[ATTACKSPEED] < 14)
                    iTotalRequirements -= 2;
                if (iHadStat[VITALITY] >= 70 || iSafeLifePercentage >= 0.85)
                    iTotalRequirements++;
                else
                    iTotalRequirements--;
                if (iHadStat[VITALITY] >= 260)
                    iTotalRequirements++;
                if (iHadStat[MAXDISCIPLINE] >= 8)
                    iTotalRequirements++;
                if (iHadStat[MAXDISCIPLINE] >= 10)
                    iTotalRequirements++;
                if (iHadStat[SOCKETS] >= 1)
                    iTotalRequirements++;
                if (iHadStat[CRITCHANCE] >= 6)
                    iTotalRequirements++;
                if (iHadStat[CRITCHANCE] >= 8)
                    iTotalRequirements++;
                if (iHadStat[LIFEPERCENT] >= 8)
                    iTotalRequirements++;
                if (iHadStat[MAGICFIND] >= 18)
                    iTotalRequirements++;
                if (iTotalRequirements < 4)
                    iTotalPoints *= 0.4;
                else if (iTotalRequirements < 5)
                    iTotalPoints *= 0.5;
                if (iTotalRequirements >= 7)
                    iTotalPoints *= 1.2;
            }
            // Mojos and Sources
            if (thisGilesItemType == GilesItemType.Source || thisGilesItemType == GilesItemType.Mojo)
            {
                iTotalRequirements = 0;
                if (iHadStat[INTELLIGENCE] >= 100)
                    iTotalRequirements++;
                else if (iHadStat[INTELLIGENCE] < 80)
                    iTotalRequirements -= 3;
                else if (iHadStat[INTELLIGENCE] < 100)
                    iTotalRequirements -= 1;
                if (iHadStat[INTELLIGENCE] >= 160)
                    iTotalRequirements++;
                if (iHadStat[MAXDAMAGE] >= 250)
                    iTotalRequirements++;
                else
                    iTotalRequirements -= 2;
                if (iHadStat[MAXDAMAGE] >= 340)
                    iTotalRequirements++;
                if (iHadStat[MINDAMAGE] >= 50)
                    iTotalRequirements++;
                else
                    iTotalRequirements--;
                if (iHadStat[MINDAMAGE] >= 85)
                    iTotalRequirements++;
                if (iHadStat[VITALITY] >= 70)
                    iTotalRequirements++;
                if (iHadStat[SOCKETS] >= 1)
                    iTotalRequirements++;
                if (iHadStat[CRITCHANCE] >= 6)
                    iTotalRequirements++;
                if (iHadStat[CRITCHANCE] >= 8)
                    iTotalRequirements++;
                if (iHadStat[LIFEPERCENT] >= 8)
                    iTotalRequirements++;
                if (iHadStat[MAGICFIND] >= 15)
                    iTotalRequirements++;
                if (iHadStat[MAXMANA] >= 60)
                    iTotalRequirements++;
                if (iHadStat[ARCANECRIT] >= 8)
                    iTotalRequirements++;
                if (iHadStat[ARCANECRIT] >= 10)
                    iTotalRequirements++;
                if (iTotalRequirements < 4)
                    iTotalPoints *= 0.4;
                else if (iTotalRequirements < 5)
                    iTotalPoints *= 0.5;
                if (iTotalRequirements >= 8)
                    iTotalPoints *= 1.2;
            }

            // Chests/cloaks/pants without a socket lose 17% of total score
            if ((thisGilesItemType == GilesItemType.Chest || thisGilesItemType == GilesItemType.Cloak || thisGilesItemType == GilesItemType.Pants) && iHadStat[SOCKETS] == 0)
                iTotalPoints *= 0.83;

            // Boots with no movement speed get reduced score
            if ((thisGilesItemType == GilesItemType.Boots) && iHadStat[MOVEMENTSPEED] <= 6)
                iTotalPoints *= 0.75;

            // Helmets
            if (thisGilesItemType == GilesItemType.Helm || thisGilesItemType == GilesItemType.WizardHat || thisGilesItemType == GilesItemType.VoodooMask || thisGilesItemType == GilesItemType.SpiritStone)
            {
                // Helmets without a socket lose 20% of total score, and most of any MF/GF bonus
                if (iHadStat[SOCKETS] == 0)
                {
                    iTotalPoints *= 0.8;
                    if (iHadStat[MAGICFIND] > 0 || iHadStat[GOLDFIND] > 0)
                    {
                        if (iHadStat[MAGICFIND] > 0 && iHadStat[GOLDFIND] > 0)
                            iTotalPoints -= ((iHadPoints[MAGICFIND] * 0.25) + (iHadPoints[GOLDFIND] * 0.25));
                        else
                            iTotalPoints -= ((iHadPoints[MAGICFIND] * 0.65) + (iHadPoints[GOLDFIND] * 0.65));
                    }
                }
            }

            // Gold-find and pickup radius combined
            if ((iHadStat[GOLDFIND] / iThisItemsMaxStats[GOLDFIND] > 0.55) && (iHadStat[PICKUPRADIUS] / iThisItemsMaxStats[PICKUPRADIUS] > 0.5))
                iTotalPoints += (((iThisItemsMaxPoints[PICKUPRADIUS] + iThisItemsMaxPoints[GOLDFIND]) / 2) * 0.25);

            // All-resist and pickup radius combined
            if ((iHadStat[ALLRESIST] / iThisItemsMaxStats[ALLRESIST] > 0.55) && (iHadStat[PICKUPRADIUS] > 0))
                iTotalPoints += (((iThisItemsMaxPoints[PICKUPRADIUS] + iThisItemsMaxPoints[ALLRESIST]) / 2) * 0.65);

            // Special crit hit/crit chance/attack speed combos
            double dBestFinalBonus = 1d;
            if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.8)) && (iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.8)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.8)))
            {
                if (dBestFinalBonus < 3.2 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 3.2;
            }
            if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.8)) && (iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.8)))
            {
                if (dBestFinalBonus < 2.3) dBestFinalBonus = 2.3;
            }
            if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.8)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.8)))
            {
                if (dBestFinalBonus < 2.1 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 2.1;
            }
            if ((iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.8)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.8)))
            {
                if (dBestFinalBonus < 1.8 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 1.8;
            }
            if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.65)) && (iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.65)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.65)))
            {
                if (dBestFinalBonus < 2.1 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 2.1;
            }
            if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.65)) && (iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.65)))
            {
                if (dBestFinalBonus < 1.9) dBestFinalBonus = 1.9;
            }
            if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.65)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.65)))
            {
                if (dBestFinalBonus < 1.7 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 1.7;
            }
            if ((iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.65)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.65)))
            {
                if (dBestFinalBonus < 1.5 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 1.5;
            }
            if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.45)) && (iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.45)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.45)))
            {
                if (dBestFinalBonus < 1.7 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 1.7;
            }
            if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.45)) && (iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.45)))
            {
                if (dBestFinalBonus < 1.4) dBestFinalBonus = 1.4;
            }
            if ((iHadStat[CRITCHANCE] > (iThisItemsMaxStats[CRITCHANCE] * 0.45)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.45)))
            {
                if (dBestFinalBonus < 1.3 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 1.3;
            }
            if ((iHadStat[CRITDAMAGE] > (iThisItemsMaxStats[CRITDAMAGE] * 0.45)) && (iHadStat[ATTACKSPEED] > (iThisItemsMaxStats[ATTACKSPEED] * 0.45)))
            {
                if (dBestFinalBonus < 1.1 && thisGilesItemType != GilesItemType.Quiver) dBestFinalBonus = 1.1;
            }


            iTotalPoints *= dBestFinalBonus;

            //1h Weapons that fall below the 60% range get docked!




            return Math.Round(iTotalPoints);
        }


        // **********************************************************************************************
        // *****                 Output test scores for everything in the backpack                  *****
        // **********************************************************************************************
        private static void TestScoring()
        {
            if (bTestingBackpack) return;
            bTestingBackpack = true;
            ZetaDia.Actors.Update();
            if (ZetaDia.Actors.Me == null)
            {
                Logging.Write("Error testing scores - not in game world?");
                return;
            }
            if (ZetaDia.IsInGame && !ZetaDia.IsLoadingWorld)
            {
                bOutputItemScores = true;
                Logging.Write("===== Outputting Test Scores =====");
                foreach (ACDItem item in ZetaDia.Actors.Me.Inventory.Backpack)
                {
                    if (item.BaseAddress == IntPtr.Zero)
                    {
                        Logging.Write("GSError: Diablo 3 memory read error, or item became invalid [TestScore-1]");
                    }
                    else
                    {
								CacheACDItem thiscacheditem=new CacheACDItem(item.InternalName, item.Name, item.Level, item.ItemQualityLevel, item.Gold, item.GameBalanceId, item.DynamicId,
                            item.Stats.WeaponDamagePerSecond, item.IsOneHand, item.DyeType, item.ItemType, item.FollowerSpecialType, item.IsUnidentified, item.ItemStackQuantity,
                            item.Stats, item, item.InventoryRow, item.InventoryColumn, item.IsPotion, item.ACDGuid);

                        bool bShouldStashTest = ShouldWeStashThis(thiscacheditem);
                        Logging.Write(bShouldStashTest ? "***** KEEP *****" : "-- TRASH --");
                    }
                }
                Logging.Write("===== Finished Test Score Outputs =====");
                Logging.Write("Note: See bad scores? Wrong item types? Known DB bug - restart DB before using the test button!");
                bOutputItemScores = false;
            }
            else
            {
                Logging.Write("Error testing scores - not in game world?");
            }
            bTestingBackpack = false;
        }

        // **********************************************************************************************
        // *****      Determine if we should stash this item or not based on item type and score    *****
        // **********************************************************************************************
		  private static bool ShouldWeStashThis(CacheACDItem thisitem)
        {
            // Stash all unidentified items - assume we want to keep them since we are using an identifier over-ride
            if (thisitem.IsUnidentified)
            {
                if (bOutputItemScores) Log(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] = (autokeep unidentified items)");
                return true;
            }
            // Now look for Misc items we might want to keep
            GilesItemType TrueItemType = DetermineItemType(thisitem.ThisInternalName, thisitem.ThisDBItemType, thisitem.ThisFollowerType);

            if (TrueItemType == GilesItemType.StaffOfHerding)
            {
                if (bOutputItemScores) Log(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType.ToString() + "] = (autokeep staff of herding)");
                return true;
            }
            if (TrueItemType == GilesItemType.CraftingMaterial)
            {
                if (bOutputItemScores) Log(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType.ToString() + "] = (autokeep craft materials)");
                return true;
            }
            if (TrueItemType == GilesItemType.CraftingPlan)
            {
                // Logging.Write(thisitem.ThisRealName + " " + thisitem.ThisInternalName);

                if (bOutputItemScores) Log(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType.ToString() + "] = (autokeep plans)");


                //Keep only 6 property plans, jeweler plans, and legendary plans.
                if (thisitem.ThisRealName.Contains("Exalted Grand"))
                    return true;
                if (thisitem.ThisQuality == ItemQuality.Legendary)
                    return true;
                if (thisitem.ThisInternalName.Contains("Jeweler"))
                    return true;



                return false;
            }
            if (TrueItemType == GilesItemType.Emerald)
            {
                if (bOutputItemScores) Log(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType.ToString() + "] = (autokeep gems)");
                return true;
            }
            if (TrueItemType == GilesItemType.Amethyst)
            {
                if (bOutputItemScores) Log(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType.ToString() + "] = (autokeep gems)");
                return true;
            }
            if (TrueItemType == GilesItemType.Topaz)
            {
                if (bOutputItemScores) Log(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType.ToString() + "] = (autokeep gems)");
                return true;
            }
            if (TrueItemType == GilesItemType.Ruby)
            {
                if (bOutputItemScores) Log(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType.ToString() + "] = (autokeep gems)");
                return true;
            }
            if (TrueItemType == GilesItemType.CraftTome)
            {
                if (bOutputItemScores) Log(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType.ToString() + "] = (autokeep tomes)");
                return true;
            }
            if (TrueItemType == GilesItemType.InfernalKey)
            {
                if (bOutputItemScores) Log(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType.ToString() + "] = (autokeep infernal key)");
                return true;
            }
            if (TrueItemType == GilesItemType.HealthPotion)
            {
                if (bOutputItemScores) Log(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType.ToString() + "] = (ignoring potions)");
                return false;
            }

            if (thisitem.ThisQuality >= ItemQuality.Legendary)
            {
                if (bOutputItemScores) Log(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType.ToString() + "] = (autokeep legendaries)");
                return true;
            }

            // Ok now try to do some decent item scoring based on item types
            double iNeedScore = ScoreNeeded(TrueItemType);
            double iMyScore = ValueThisItem(thisitem, TrueItemType);
            if (bOutputItemScores) Log(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType.ToString() + "] = " + iMyScore.ToString());
            if (iMyScore >= iNeedScore) return true;

            // If we reached this point, then we found no reason to keep the item!
            return false;
        }
    }
}