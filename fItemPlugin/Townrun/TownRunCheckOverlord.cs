using System;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Helpers;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fItemPlugin.Townrun
{
	internal static partial class TownRunManager
	{
        /// <summary>
        /// The method for checking if we should start the town run behavior.
        /// </summary>
		internal static bool TownRunCheckOverlord(object ret)
		{
			bWantToTownRun = false;

			// Check if we should be forcing a town-run
			if (BrainBehavior.IsVendoring)
			{
				bWantToTownRun = true;
			}
			else
			{
				int recheckDelay = ZetaDia.IsInTown ? 1 : 6;
				if (DateTime.Now.Subtract(TimeLastCheckedForTownRun).TotalSeconds > recheckDelay)
				{
					TimeLastCheckedForTownRun = DateTime.Now;

					//our result of checking various things for town run.
					bool _checkResult = false;

                    if (!BountyCache.IsParticipatingInTieredLootRun && ZetaDia.Me.Inventory.NumFreeBackpackSlots <= 2)
					{
						_checkResult = true;
						FunkyTownRunPlugin.DBLog.Info("[Funky] Starting Town Run (No Space Left In Backpack)");
					}
					else
					{
						if (Equipment.ShouldRepairItems(CharacterSettings.Instance.RepairWhenDurabilityBelow))
						{
							_checkResult = true;
							FunkyTownRunPlugin.DBLog.Info("[Funky] Starting Town Run (Items Need Repaired)");
						}
                        else if (!BountyCache.IsParticipatingInTieredLootRun && FunkyTownRunPlugin.PluginSettings.EnableBloodShardGambling && FunkyTownRunPlugin.PluginSettings.MinimumBloodShards > 5)
						{
							int curBloodShardCount = Backpack.GetBloodShardCount();
							if (curBloodShardCount != -1 && curBloodShardCount >= FunkyTownRunPlugin.PluginSettings.MinimumBloodShards)
							{
							    if (FunkyTownRunPlugin.PluginSettings.UseAltGambling)
							    {
							        fBaseXtensions.Behaviors.CharacterControl.GamblingCharacterSwitch = true;
							        fBaseXtensions.Behaviors.ExitGameBehavior.ShouldExitGame = true;
							    }
							    else
							    {
                                    _checkResult = true;
                                    FunkyTownRunPlugin.DBLog.Info("[Funky] Starting Town Run (Gambling)");
							    }
							}
						}
					}

					if (_checkResult)
					{
						bWantToTownRun = true;
					}
				}
			}

			bLastTownRunCheckResult = bWantToTownRun;

			//Precheck prior to casting TP..
			if (bLastTownRunCheckResult)
			{
				CheckedVendorActions = false;
				bNeedsEquipmentRepairs = false;
				bBuyingPotions = false;

				if (!ZetaDia.IsInTown)
				{
				    return true;
				}
			}

			return bWantToTownRun;
		}

	    internal static bool bLoggedAnythingThisStash = false;
		internal static bool bLoggedJunkThisStash = false;
		internal static bool bWantToTownRun = false;
		private static bool bLastTownRunCheckResult;
		private static DateTime TimeLastCheckedForTownRun = DateTime.Today;
		internal static ItemCache townRunItemCache = new ItemCache();
		internal static Delayer Delay = new Delayer();
		

	    private static bool RequiresRepair = false;
		private static Act CurrentAct = Act.Invalid;
		private static string VendorName = String.Empty;
	    private const string SalvageName = "PT_Blacksmith_RepairShortcut";
	    private static Vector3 SafetyVendorLocation, SafetySalvageLocation, SafetyStashLocation, SafetyIdenifyLocation, SafetyGambleLocation;
		private static bool MovedToSafetyLocation = false;
        private static bool ActionsChecked = false;

        /// <summary>
        /// Check if we should update our town run variables
        /// </summary>
        internal static bool ActionsEvaluatedOverlord(object ret)
        {
            return !ActionsChecked;
        }
        /// <summary>
        /// Refreshes the town run variables based on current act
        /// </summary>
		internal static RunStatus ActionsEvaluatedBehavior(object ret)
		{
			Navigator.Clear();
			Navigator.SearchGridProvider.Update();
			MovedToSafetyLocation = false;


			if (FunkyGame.AdventureMode)
				CurrentAct = GameCache.FindActByTownLevelAreaID(ZetaDia.CurrentLevelAreaId);
			else
				CurrentAct = ZetaDia.CurrentAct;
			
			

			switch (CurrentAct)
			{
				case Act.A1:
					VendorName = "a1_uniquevendor_miner"; break;
				case Act.A2:
					VendorName = "a2_uniquevendor_peddler"; break;
				case Act.A3:
					VendorName = "a3_uniquevendor_collector"; break;
				case Act.A4:
					VendorName = "a4_uniquevendor_collector"; break;
				case Act.A5:
					VendorName = "x1_a5_uniquevendor_collector"; break;
			}

			RequiresRepair = Equipment.ShouldRepairItems(CharacterSettings.Instance.RepairWhenDurabilityBelow);
			SafetyVendorLocation = GameCache.ReturnTownRunMovementVector(GameCache.TownRunBehavior.Sell, CurrentAct);
			SafetySalvageLocation = GameCache.ReturnTownRunMovementVector(GameCache.TownRunBehavior.Salvage, CurrentAct);
			SafetyStashLocation = GameCache.ReturnTownRunMovementVector(GameCache.TownRunBehavior.Stash, CurrentAct);
			SafetyIdenifyLocation = GameCache.ReturnTownRunMovementVector(GameCache.TownRunBehavior.Idenify, CurrentAct);
			SafetyGambleLocation = GameCache.ReturnTownRunMovementVector(GameCache.TownRunBehavior.Gamble, CurrentAct);

			//Clear our item cache 
			Backpack.CacheItemList.Clear();

			bSalvageAllMagic = false;
			bSalvageAllNormal = false;
			bSalvageAllRare = false;

			FunkyGame.Bounty.RefreshActiveQuests();

			FunkyTownRunPlugin.DBLog.DebugFormat("Current Act: {0} VendorName: {1} Requires Repair: {2}", CurrentAct, VendorName, RequiresRepair);
			
            ActionsChecked = true;
			return RunStatus.Success;
		}
        /// <summary>
        /// Resets the town run variables!
        /// </summary>
		internal static RunStatus ActionsEvaluatedEndingBehavior(object ret)
		{
			//FunkyTownRunPlugin.TownRunStats.TownRuns++;
			if (FunkyGame.CurrentGameStats != null)
				FunkyGame.CurrentGameStats.CurrentProfile.TownRuns++;
			//FunkyTownRunPlugin.LogTownRunStats();

			SafetyVendorLocation = Vector3.Zero;
			SafetySalvageLocation = Vector3.Zero;
			SafetyStashLocation = Vector3.Zero;
			SafetyIdenifyLocation = Vector3.Zero;
			SafetyGambleLocation = Vector3.Zero;
			MovedToSafetyLocation = false;

			_dictItemStashAttempted.Clear();
			VendorName = String.Empty;
			CurrentAct = Act.Invalid;
			ActionsChecked = false;

			Logger.DBLog.InfoFormat("[FunkyTownRun] Finished Behavior.");
			return RunStatus.Success;
		}


		internal static bool StashValidation(CacheACDItem thisitem)
		{
			// Stash all unidentified items - assume we want to keep them since we are using an identifier over-ride
			if (thisitem.IsUnidentified)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] = (autokeep unidentified items)");
				return true;
			}
			// Now look for Misc items we might want to keep
			PluginItemTypes TrueItemType = ItemFunc.DetermineItemType(thisitem);
			if (TrueItemType == PluginItemTypes.KeyStone)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep keystone fragments)");
				return true;
			}
			if (TrueItemType == PluginItemTypes.HoradricCache)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep cache)");
				return FunkyTownRunPlugin.PluginSettings.StashHoradricCache;
			}
			if (TrueItemType == PluginItemTypes.StaffOfHerding)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep staff of herding)");
				return true;
			}
			if (TrueItemType == PluginItemTypes.CraftingMaterial)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep craft materials)");
				return true;
			}
			if (TrueItemType == PluginItemTypes.CraftingPlan)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep plans)");
				return true;
			}
			if (TrueItemType == PluginItemTypes.Emerald)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep gems)");
				return true;
			}
			if (TrueItemType == PluginItemTypes.Amethyst)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep gems)");
				return true;
			}
			if (TrueItemType == PluginItemTypes.Topaz)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep gems)");
				return true;
			}
			if (TrueItemType == PluginItemTypes.Ruby)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep gems)");
				return true;
			}
			if (TrueItemType == PluginItemTypes.Diamond)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep gems)");
				return true;
			}
			if (TrueItemType == PluginItemTypes.LegendaryGem)
			{
				return true;
			}
			if (TrueItemType==PluginItemTypes.RamaladnisGift)
			{
				return true;
			}
			if (TrueItemType == PluginItemTypes.CraftTome)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep tomes)");
				return true;
			}
			if (TrueItemType == PluginItemTypes.InfernalKey)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep infernal key)");
				return true;
			}
			if (TrueItemType == PluginItemTypes.HealthPotion)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (ignoring potions)");
				return false;
			}

			if (thisitem.ThisQuality >= ItemQuality.Legendary)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep legendaries)");
				return true;
			}

			if (TrueItemType == PluginItemTypes.Dye)
			{
				return true;
			}

			// Ok now try to do some decent item scoring based on item types
			//double iNeedScore = ScoreNeeded(TrueItemType);
			//double iMyScore = ValueThisItem(thisitem, TrueItemType);
			//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = " + iMyScore);
			//if (iMyScore >= iNeedScore) return true;

			// If we reached this point, then we found no reason to keep the item!
			return false;
		}

		internal static bool SalvageValidation(CacheACDItem thisitem)
		{
			if (thisitem.IsVendorBought || thisitem.IsUnidentified || thisitem.ItemType == PluginItemTypes.HealthPotion || thisitem.IsHoradricCache || thisitem.ThisLevel==1)
			{
				return false;
			}

			//Only Check items we can actually salvage!
			if (thisitem.BaseItemType == PluginBaseItemTypes.FollowerItem || thisitem.BaseItemType == PluginBaseItemTypes.Armor ||
				thisitem.BaseItemType == PluginBaseItemTypes.Jewelry || thisitem.BaseItemType == PluginBaseItemTypes.Offhand ||
				thisitem.BaseItemType == PluginBaseItemTypes.WeaponOneHand || thisitem.BaseItemType == PluginBaseItemTypes.WeaponRange ||
				thisitem.BaseItemType == PluginBaseItemTypes.WeaponTwoHand)
			{
				if (thisitem.ThisQuality == ItemQuality.Legendary)
				{
					if (FunkyTownRunPlugin.PluginSettings.SalvageLegendaryItemLevel > 0)
					{
						return FunkyTownRunPlugin.PluginSettings.SalvageLegendaryItemLevel <= thisitem.ThisLevel;
					}
				}
				else if (thisitem.ThisQuality == ItemQuality.Rare4 || thisitem.ThisQuality == ItemQuality.Rare5 || thisitem.ThisQuality == ItemQuality.Rare6)
				{
					if (FunkyTownRunPlugin.PluginSettings.SalvageRareItemLevel > 0)
					{
						return FunkyTownRunPlugin.PluginSettings.SalvageRareItemLevel <= thisitem.ThisLevel;
					}
				}
				else if (thisitem.ThisQuality == ItemQuality.Magic1 || thisitem.ThisQuality == ItemQuality.Magic2 || thisitem.ThisQuality == ItemQuality.Magic3)
				{
					if (FunkyTownRunPlugin.PluginSettings.SalvageMagicItemLevel > 0)
					{
						return FunkyTownRunPlugin.PluginSettings.SalvageMagicItemLevel <= thisitem.ThisLevel;
					}
				}
				else if (thisitem.ThisQuality == ItemQuality.Superior || thisitem.ThisQuality == ItemQuality.Inferior || thisitem.ThisQuality == ItemQuality.Normal)
				{
					if (FunkyTownRunPlugin.PluginSettings.SalvageWhiteItemLevel > 0)
					{
						return FunkyTownRunPlugin.PluginSettings.SalvageWhiteItemLevel <= thisitem.ThisLevel;
					}
				}
			}
			// If we reached this point, then we found no reason to keep the item!
			return false;
		}

		internal static bool SellValidation(string thisinternalname, int thislevel, ItemQuality thisquality, ItemType thisdbitemtype, FollowerType thisfollowertype)
		{
			PluginItemTypes thisPluginItemType = ItemFunc.DetermineItemType(thisinternalname, thisdbitemtype, thisfollowertype);
			PluginBaseItemTypes thisGilesBaseType = ItemFunc.DetermineBaseType(thisPluginItemType);

			switch (thisGilesBaseType)
			{
				case PluginBaseItemTypes.WeaponRange:
				case PluginBaseItemTypes.WeaponOneHand:
				case PluginBaseItemTypes.WeaponTwoHand:
				case PluginBaseItemTypes.Armor:
				case PluginBaseItemTypes.Offhand:
				case PluginBaseItemTypes.Jewelry:
				case PluginBaseItemTypes.FollowerItem:
					return true;
				case PluginBaseItemTypes.Gem:
				case PluginBaseItemTypes.Misc:
				case PluginBaseItemTypes.Unknown:
					if (thisPluginItemType == PluginItemTypes.LegendaryCraftingMaterial)
						return true;
					//Sell any plans not already stashed.
					return thisdbitemtype == ItemType.CraftingPlan;
			} // Switch giles base item type
			return false;
		}



	}

}