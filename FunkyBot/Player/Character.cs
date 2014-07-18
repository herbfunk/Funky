using System;
using System.Linq;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Helpers;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using fBaseXtensions.Monitor;
using FunkyBot.Cache;
using FunkyBot.Cache.Objects;
using FunkyBot.DBHandlers.Townrun;
using FunkyBot.Misc;
using FunkyBot.Movement;
using FunkyBot.Player.Class;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player
{
	public class Character
	{
		public Character()
		{
			FunkyGame.Hero = new ActiveHero();
			FunkyGame.Hero.OnLevelAreaIDChanged += LevelAreaIDChangeHandler;
			Class = null;
			Equipment.OnEquippedItemsChanged += EquippmentChangedHandler;
		}
		public void RefreshHotBar()
		{
			Class = null;
		}
		public PlayerClass Class { get; set; }
		




		public delegate void ItemPickupEvaluation(CacheItem item);
		public static event ItemPickupEvaluation OnItemPickupEvaluation;
		internal void ItemPickupEval(CacheItem item)
		{
			if (OnItemPickupEvaluation == null)
			{//If no event hooked then use default evaluation

				if (!item.ShouldPickup.HasValue)
				{
					//Use Giles Scoring or DB Weighting..
					item.ShouldPickup = PickupItemValidation(item);
				}
			}
			else
			{
				OnItemPickupEvaluation(item);
			}
		}
		internal static bool PickupItemValidation(CacheItem item)
		{
			//if (FunkyTownRunPlugin.ItemRulesEval != null)
			//{
			//	Interpreter.InterpreterAction action = FunkyTownRunPlugin.ItemRulesEval.checkPickUpItem(item.DynamicID.Value, item.BalanceData.thisItemType, item.ref_DiaItem.Name, item.InternalName, item.Itemquality.Value, item.BalanceData.iThisItemLevel, item.BalanceData.bThisOneHand, item.BalanceData.bThisTwoHand, item.BalanceID.Value, ItemEvaluationType.PickUp);
			//	if (action== Interpreter.InterpreterAction.PICKUP)
			//	{
			//		return true;
			//	}
			//}

			// Calculate giles item types and base types etc.
			PluginItemTypes thisPluginItemType = ItemFunc.DetermineItemType(item.InternalName, item.BalanceData.thisItemType, item.BalanceData.thisFollowerType, item.SNOID);
			PluginBaseItemTypes thisGilesBaseType = ItemFunc.DetermineBaseType(thisPluginItemType);

			if (thisPluginItemType == PluginItemTypes.MiscBook)
				return Bot.Settings.Loot.ExpBooks;


			// Error logging for DemonBuddy item mis-reading
			ItemType gilesDBItemType = ItemFunc.PluginItemTypeToDBItemType(thisPluginItemType);
			if (gilesDBItemType != item.BalanceData.thisItemType)
			{
				Logger.DBLog.InfoFormat("GSError: Item type mis-match detected: Item Internal=" + item.InternalName + ". DemonBuddy ItemType thinks item type is=" + item.BalanceData.thisItemType + ". Giles thinks item type is=" +
					 gilesDBItemType + ". [pickup]", true);
			}

			switch (thisGilesBaseType)
			{
				case PluginBaseItemTypes.WeaponTwoHand:
				case PluginBaseItemTypes.WeaponOneHand:
				case PluginBaseItemTypes.WeaponRange:
				case PluginBaseItemTypes.Armor:
				case PluginBaseItemTypes.Offhand:
				case PluginBaseItemTypes.Jewelry:
				case PluginBaseItemTypes.FollowerItem:
					if (item.Itemquality.HasValue)
					{
						switch (item.Itemquality.Value)
						{
							case ItemQuality.Inferior:
							case ItemQuality.Normal:
							case ItemQuality.Superior:
								return Bot.Settings.Loot.PickupWhiteItems > 0 && (item.BalanceData.iThisItemLevel >= Bot.Settings.Loot.PickupWhiteItems);
							case ItemQuality.Magic1:
							case ItemQuality.Magic2:
							case ItemQuality.Magic3:
								return Bot.Settings.Loot.PickupMagicItems > 0 && (item.BalanceData.iThisItemLevel >= Bot.Settings.Loot.PickupMagicItems);
							case ItemQuality.Rare4:
							case ItemQuality.Rare5:
							case ItemQuality.Rare6:
								return Bot.Settings.Loot.PickupRareItems > 0 && (item.BalanceData.iThisItemLevel >= Bot.Settings.Loot.PickupRareItems);
							case ItemQuality.Legendary:
								return Bot.Settings.Loot.PickupLegendaryItems > 0 && (item.BalanceData.iThisItemLevel >= Bot.Settings.Loot.PickupLegendaryItems);
						}
					}

					return false;
				case PluginBaseItemTypes.Gem:
					GemQualityTypes qualityType = ItemFunc.ReturnGemQualityType(item.SNOID, item.BalanceData.iThisItemLevel);
					int qualityLevel = (int)qualityType;

					if (qualityLevel < Bot.Settings.Loot.MinimumGemItemLevel ||
						(thisPluginItemType == PluginItemTypes.Ruby && !Bot.Settings.Loot.PickupGems[0]) ||
						(thisPluginItemType == PluginItemTypes.Emerald && !Bot.Settings.Loot.PickupGems[1]) ||
						(thisPluginItemType == PluginItemTypes.Amethyst && !Bot.Settings.Loot.PickupGems[2]) ||
						(thisPluginItemType == PluginItemTypes.Topaz && !Bot.Settings.Loot.PickupGems[3]) ||
						(thisPluginItemType == PluginItemTypes.Diamond && !Bot.Settings.Loot.PickupGemDiamond))
					{
						return false;
					}
					break;
				case PluginBaseItemTypes.Misc:
					// Note; Infernal keys are misc, so should be picked up here - we aren't filtering them out, so should default to true at the end of this function
					if (thisPluginItemType == PluginItemTypes.CraftingMaterial)
					{
						return Bot.Settings.Loot.PickupCraftMaterials;
					}

					if (thisPluginItemType == PluginItemTypes.CraftingPlan)
					{
						if (!Bot.Settings.Loot.PickupCraftPlans) return false;

						int gamebalanceID = item.BalanceID.Value;

						if (item.BalanceData.IsBlacksmithPlanSixProperties && !Bot.Settings.Loot.PickupBlacksmithPlanSix) return false;
						if (item.BalanceData.IsBlacksmithPlanFiveProperties && !Bot.Settings.Loot.PickupBlacksmithPlanFive) return false;
						if (item.BalanceData.IsBlacksmithPlanFourProperties && !Bot.Settings.Loot.PickupBlacksmithPlanFour) return false;

						if (item.BalanceData.IsBlacksmithPlanArchonSpaulders && !Bot.Settings.Loot.PickupBlacksmithPlanArchonSpaulders) return false;
						if (item.BalanceData.IsBlacksmithPlanArchonGauntlets && !Bot.Settings.Loot.PickupBlacksmithPlanArchonGauntlets) return false;
						if (item.BalanceData.IsBlacksmithPlanRazorspikes && !Bot.Settings.Loot.PickupBlacksmithPlanRazorspikes) return false;


						if (item.BalanceData.IsJewelcraftDesignAmulet && !Bot.Settings.Loot.PickupJewelerDesignAmulet) return false;
						if (item.BalanceData.IsJewelcraftDesignFlawlessStarGem && !Bot.Settings.Loot.PickupJewelerDesignFlawlessStar) return false;
						if (item.BalanceData.IsJewelcraftDesignMarquiseGem && !Bot.Settings.Loot.PickupJewelerDesignMarquise) return false;
						if (item.BalanceData.IsJewelcraftDesignPerfectStarGem && !Bot.Settings.Loot.PickupJewelerDesignPerfectStar) return false;
						if (item.BalanceData.IsJewelcraftDesignRadiantStarGem && !Bot.Settings.Loot.PickupJewelerDesignRadiantStar) return false;


					}

					if (thisPluginItemType == PluginItemTypes.InfernalKey)
					{
						return Bot.Settings.Loot.PickupInfernalKeys;
					}

					if (thisPluginItemType == PluginItemTypes.KeyStone)
					{
						return Bot.Settings.Loot.PickupKeystoneFragments;
					}

					if (thisPluginItemType == PluginItemTypes.BloodShard)
					{

						return Backpack.GetBloodShardCount() < 500;
					}

					// Potion filtering
					if (thisPluginItemType == PluginItemTypes.HealthPotion)
					{
						if (item.BalanceData.IsRegularPotion)
						{
							if (Bot.Settings.Loot.MaximumHealthPotions <= 0)
								return false;


							var BestPotionToUse = Backpack.ReturnBestPotionToUse();
							if (BestPotionToUse == null)
								return true;

							var Potions = Backpack.ReturnRegularPotions();
							if (Potions.Sum(potions => potions.ThisItemStackQuantity) >= Bot.Settings.Loot.MaximumHealthPotions)
								return false;
						}
					}


					break;
				case PluginBaseItemTypes.HealthGlobe:
					return false;
				case PluginBaseItemTypes.Unknown:
					return false;
				default:
					return false;
			} // Switch giles base item type
			// Didn't cancel it, so default to true!
			return true;
		}


		private int LastWorldID = -1;
		private bool LastLevelIDChangeWasTownRun;
		private void LevelAreaIDChangeHandler(int ID)
		{
			Logger.Write(LogLevel.Event, "Level Area ID has Changed");



			if (!BrainBehavior.IsVendoring)
			{
				//Check for World ID change!
				if (FunkyGame.Hero.CurrentWorldDynamicID != LastWorldID)
				{
					Logger.Write(LogLevel.Event, "World ID changed.. clearing Profile Interactable Cache.");
					LastWorldID = FunkyGame.Hero.CurrentWorldDynamicID;
					ObjectCache.InteractableObjectCache.Clear();
					Navigator.SearchGridProvider.Update();

					//Gold Inactivity
					GoldInactivity.LastCoinageUpdate = DateTime.Now;
				}

				if (!LastLevelIDChangeWasTownRun)
				{//Do full clear

					BackTrackCache.cacheMovementGPRs.Clear();
					Bot.NavigationCache.LOSBlacklistedRAGUIDs.Clear();
					Bot.Game.InteractableCachedObject = null;
				}
				else
				{
					//Gold Inactivity
					GoldInactivity.LastCoinageUpdate = DateTime.Now;
					TownRunManager.TalliedTownRun = false;
				}

				//Clear the object cache!
				ObjectCache.Objects.Clear();
				//ObjectCache.cacheSnoCollection.ClearDictionaryCacheEntries();
				Bot.Targeting.Cache.RemovalCheck = false;

				//Reset Skip Ahead Cache
				SkipAheadCache.ClearCache();

				FunkyGame.Hero.UpdateCoinage = true;

				//ZetaDia.ActInfo.ActiveBounty.Info.QuestSNO
				//Adventure Mode?
				if (FunkyGame.AdventureMode && Bot.Settings.AdventureMode.EnableAdventuringMode)
				{
					Bot.Game.Bounty.RefreshLevelChanged();
				}

				LastLevelIDChangeWasTownRun = false;
			}
			else if (FunkyGame.Hero.bIsInTown)
			{
				LastLevelIDChangeWasTownRun = true;
			}
		}

		internal void Reset()
		{
			
			FunkyGame.Hero = new ActiveHero();
			FunkyGame.Hero.OnLevelAreaIDChanged += LevelAreaIDChangeHandler;

			//Data = new ActiveHero();
			//Data.OnLevelAreaIDChanged += LevelAreaIDChangeHandler;
			Equipment.RefreshEquippedItemsList();
			Class = null;
			Backpack.CacheItemList.Clear();
		}

		///<summary>
		///To Find Town Areas
		///</summary>
		public static Act FindActByLevelID(int ID)
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

		internal static void EquippmentChangedHandler()
		{
			Logger.DBLog.InfoFormat("Equippment has changed!");

			if (!PlayerClass.ShouldRecreatePlayerClass)
				PlayerClass.ShouldRecreatePlayerClass = true;
		}

	}
}
