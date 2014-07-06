using System;
using fItemPlugin.Items;
using fItemPlugin.Player;
using Zeta.Bot.Logic;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;

namespace fItemPlugin.Townrun
{
	internal static partial class TownRunManager
	{
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

					if (ZetaDia.Me.Inventory.NumFreeBackpackSlots <= 2)
					{
						_checkResult = true;
						FunkyTownRunPlugin.DBLog.Info("[Funky] Starting Town Run (No Space Left In Backpack)");
					}
					else
					{
						if (Backpack.ShouldRepairItems())
						{
							_checkResult = true;
							FunkyTownRunPlugin.DBLog.Info("[Funky] Starting Town Run (Items Need Repaired)");
						}
						else if (FunkyTownRunPlugin.PluginSettings.EnableBloodShardGambling && FunkyTownRunPlugin.PluginSettings.MinimumBloodShards > 5)
						{
							int curBloodShardCount = Backpack.GetBloodShardCount();
							if (curBloodShardCount != -1 && curBloodShardCount >= FunkyTownRunPlugin.PluginSettings.MinimumBloodShards)
							{
								_checkResult = true;
								FunkyTownRunPlugin.DBLog.Info("[Funky] Starting Town Run (Gambling)");
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
					bPreStashPauseDone = false;
					return true;
				}
			}

			return bWantToTownRun;
		}

		private static bool bPreStashPauseDone;

		internal static bool bLoggedAnythingThisStash = false;
		internal static bool bLoggedJunkThisStash = false;

		internal static bool bWantToTownRun = false;
		private static bool bLastTownRunCheckResult;
		private static DateTime TimeLastCheckedForTownRun = DateTime.Today;

		internal static ItemCache townRunItemCache = new ItemCache();
		internal static Delayer Delay = new Delayer();


		private static bool ActionsChecked = false;
		internal static bool ActionsEvaluatedOverlord(object ret)
		{
			return !ActionsChecked;
		}

		private static int CurrentQuestSNO = -1;
		private static bool IsInAdventureMode = false;
		private static bool RequiresRepair = false;
		private static Act CurrentAct = Act.Invalid;
		private static string VendorName = String.Empty;
		private static Vector3 SafetyVendorLocation, SafetySalvageLocation, SafetyStashLocation, SafetyIdenifyLocation, SafetyGambleLocation;

		internal static RunStatus ActionsEvaluatedBehavior(object ret)
		{
			//Player.UpdatePotions();
			//townRunItemCache.UpdateLists(Player.CacheItemList.Values.ToList());
			//FunkyTownRunPlugin.DBLog.Info(townRunItemCache.GetListString());
			CurrentQuestSNO = GetQuestSNO();
			IsInAdventureMode = (CurrentQuestSNO == 312429);
			CurrentAct = FindActByLevelID(ZetaDia.CurrentLevelAreaId);
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
			RequiresRepair = Backpack.ShouldRepairItems();
			SafetyVendorLocation = ReturnMovementVector(TownRunBehavior.Sell, CurrentAct);
			SafetySalvageLocation = ReturnMovementVector(TownRunBehavior.Salvage, CurrentAct);
			SafetyStashLocation = ReturnMovementVector(TownRunBehavior.Stash, CurrentAct);
			SafetyIdenifyLocation = ReturnMovementVector(TownRunBehavior.Idenify, CurrentAct);
			SafetyGambleLocation = ReturnMovementVector(TownRunBehavior.Gamble, CurrentAct);

			//Clear our item cache 
			Backpack.CacheItemList.Clear();

			FunkyTownRunPlugin.DBLog.DebugFormat("Current Act: {0} VendorName: {1} Requires Repair: {2}", CurrentAct, VendorName, RequiresRepair);
			ActionsChecked = true;
			return RunStatus.Success;
		}
		internal static RunStatus ActionsEvaluatedEndingBehavior(object ret)
		{
			FunkyTownRunPlugin.TownRunStats.TownRuns++;
			FunkyTownRunPlugin.LogTownRunStats();

			SafetyVendorLocation = Vector3.Zero;
			SafetySalvageLocation = Vector3.Zero;
			SafetyStashLocation = Vector3.Zero;
			SafetyIdenifyLocation = Vector3.Zero;
			SafetyGambleLocation = Vector3.Zero;

			_dictItemStashAttempted.Clear();
			VendorName = String.Empty;
			CurrentAct = Act.Invalid;
			ActionsChecked = false;
			return RunStatus.Success;
		}

		private static int GetQuestSNO()
		{
			int retValue = -1;
			using (ZetaDia.Memory.AcquireFrame())
			{
				try
				{
					retValue = ZetaDia.CurrentQuest.QuestSNO;
				}
				catch
				{

				}
			}

			return retValue;
		}

		///<summary>
		///To Find Town Areas
		///</summary>
		private static Act FindActByLevelID(int ID)
		{
			switch (ID)
			{
				case 332339:
					return Act.A1;
				case 168314:
					return Act.A2;
				case 92945:
					return Act.A3;
				case 270011:
					return Act.A5;
			}

			return Act.Invalid;
		}

		enum TownRunBehavior
		{
			Stash,
			Sell,
			Salvage,
			Gamble,
			Interaction,
			Idenify
		}

		private static Vector3 ReturnMovementVector(TownRunBehavior type, Act act)
		{
			switch (type)
			{
				case TownRunBehavior.Salvage:
					switch (act)
					{
						case Act.A1:
							if (!IsInAdventureMode)
								return new Vector3(2958.418f, 2823.037f, 24.04533f);
							else
								return new Vector3(375.5075f, 563.1337f, 24.04533f);
						case Act.A2:
							return new Vector3(289.6358f, 232.1146f, 0.1f);
						case Act.A3:
						case Act.A4:
							return new Vector3(379.6096f, 415.6198f, 0.3321424f);
						case Act.A5://x="538.4665" y="479.0026" z="2.620764" (new Vector3())
							return new Vector3(560.1434f, 501.5706f, 2.685907f);
					}
					break;
				case TownRunBehavior.Sell:
					switch (act)
					{
						case Act.A1:
							if (!IsInAdventureMode)
								return new Vector3(2901.399f, 2809.826f, 24.04533f);
							else
								return new Vector3(320.8555f, 524.6776f, 24.04532f);
						case Act.A2:
							return new Vector3(295.2101f, 265.1436f, 0.1000002f);
						case Act.A3:
						case Act.A4:
							return new Vector3(418.9743f, 351.0592f, 0.1000005f);
						case Act.A5:
							return new Vector3(560.1434f, 501.5706f, 2.685907f);
					}
					break;
				case TownRunBehavior.Stash:
					switch (act)
					{
						case Act.A1:
							if (!IsInAdventureMode)
								return new Vector3(2967.146f, 2799.459f, 24.04533f);
							else
								return new Vector3(386.8494f, 524.2585f, 24.04533f);
						case Act.A2:
							return new Vector3(323.4543f, 228.5806f, 0.1f);
						case Act.A3:
						case Act.A4:
							return new Vector3(389.3798f, 390.7143f, 0.3321428f);
						case Act.A5:
							return new Vector3(510.6552f, 502.1889f, 2.620764f);
					}
					break;
				case TownRunBehavior.Gamble:
					switch (act)
					{
						case Act.A1:
							return new Vector3(376.3878f, 561.3141f, 24.04533f);
						case Act.A2:
							return new Vector3(334.8506f, 267.2392f, 0.1000038f);
						case Act.A3:
						case Act.A4:
							return new Vector3(458.5429f, 416.3311f, 0.2663189f);
						case Act.A5:
							return new Vector3(592.5067f, 535.6719f, 2.74532f);
					}
					break;
				case TownRunBehavior.Interaction:
					switch (act)
					{
						case Act.A1:
							if (!IsInAdventureMode)
								return new Vector3(2959.277f, 2811.887f, 24.04533f);
							else
								return new Vector3(386.6582f, 534.2561f, 24.04533f);
						case Act.A2:
							return new Vector3(299.5841f, 250.1721f, 0.1000036f);
						case Act.A3:
						case Act.A4:
							return new Vector3(403.7034f, 395.9311f, 0.5069602f);
						case Act.A5:
							return new Vector3(532.3179f, 521.8536f, 2.662077f);
					}
					break;
				case TownRunBehavior.Idenify:
					switch (act)
					{
						case Act.A1:
							if (!IsInAdventureMode)
								return new Vector3(2955.026f, 2817.4f, 24.04533f);
							else
								return new Vector3(372.3016f, 532.6918f, 24.04532f);
						case Act.A2:
							return new Vector3(326.9954f, 250.1623f, -0.3242276f);
						case Act.A3:
						case Act.A4:
							return new Vector3(398.9163f, 393.4324f, 0.3577437f);
						case Act.A5:
							return new Vector3(523.6658f, 525.9195f, 2.662077f);
					}
					break;
			}

			return Vector3.Zero;
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
			PluginItemType TrueItemType = ItemFunc.DetermineItemType(thisitem);
			if (TrueItemType == PluginItemType.KeyStone)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep keystone fragments)");
				return true;
			}
			if (TrueItemType == PluginItemType.HoradricCache)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep cache)");
				return FunkyTownRunPlugin.PluginSettings.StashHoradricCache;
			}
			if (TrueItemType == PluginItemType.StaffOfHerding)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep staff of herding)");
				return true;
			}
			if (TrueItemType == PluginItemType.CraftingMaterial)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep craft materials)");
				return true;
			}
			if (TrueItemType == PluginItemType.CraftingPlan)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep plans)");
				return true;
			}
			if (TrueItemType == PluginItemType.Emerald)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep gems)");
				return true;
			}
			if (TrueItemType == PluginItemType.Amethyst)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep gems)");
				return true;
			}
			if (TrueItemType == PluginItemType.Topaz)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep gems)");
				return true;
			}
			if (TrueItemType == PluginItemType.Ruby)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep gems)");
				return true;
			}
			if (TrueItemType == PluginItemType.Diamond)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep gems)");
				return true;
			}
			if (TrueItemType == PluginItemType.CraftTome)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep tomes)");
				return true;
			}
			if (TrueItemType == PluginItemType.InfernalKey)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep infernal key)");
				return true;
			}
			if (TrueItemType == PluginItemType.HealthPotion)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (ignoring potions)");
				return false;
			}

			if (thisitem.ThisQuality >= ItemQuality.Legendary)
			{
				//if (bOutputItemScores) FunkyTownRunPlugin.DBLog.InfoFormat(thisitem.ThisRealName + " [" + thisitem.ThisInternalName + "] [" + TrueItemType + "] = (autokeep legendaries)");
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
			//Only Check items we can actually salvage!
			if (thisitem.BaseItemType == PluginBaseItemType.FollowerItem || thisitem.BaseItemType == PluginBaseItemType.Armor ||
				thisitem.BaseItemType == PluginBaseItemType.Jewelry || thisitem.BaseItemType == PluginBaseItemType.Offhand ||
				thisitem.BaseItemType == PluginBaseItemType.WeaponOneHand || thisitem.BaseItemType == PluginBaseItemType.WeaponRange ||
				thisitem.BaseItemType == PluginBaseItemType.WeaponTwoHand)
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
			PluginItemType thisPluginItemType = ItemFunc.DetermineItemType(thisinternalname, thisdbitemtype, thisfollowertype);
			PluginBaseItemType thisGilesBaseType = ItemFunc.DetermineBaseType(thisPluginItemType);

			switch (thisGilesBaseType)
			{
				case PluginBaseItemType.WeaponRange:
				case PluginBaseItemType.WeaponOneHand:
				case PluginBaseItemType.WeaponTwoHand:
				case PluginBaseItemType.Armor:
				case PluginBaseItemType.Offhand:
				case PluginBaseItemType.Jewelry:
				case PluginBaseItemType.FollowerItem:
					return true;
				case PluginBaseItemType.Gem:
				case PluginBaseItemType.Misc:
				case PluginBaseItemType.Unknown:
					if (thisPluginItemType == PluginItemType.LegendaryCraftingMaterial)
						return true;
					//Sell any plans not already stashed.
					return thisdbitemtype == ItemType.CraftingPlan;
			} // Switch giles base item type
			return false;
		}


	}

}