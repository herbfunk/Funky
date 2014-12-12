using System;
using System.Linq;
using fBaseXtensions.Behaviors;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Blacklist;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Game.Hero.Class;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using fBaseXtensions.Monitor;
using fBaseXtensions.Navigation;
using fBaseXtensions.Stats;
using fBaseXtensions.Targeting;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Service;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fBaseXtensions.Game
{
	public static class FunkyGame
	{
		public static bool GameIsInvalid
		{
			get 
			{ 
				return !ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || ZetaDia.Me.CommonData == null; 
			}
		}
		public static bool RunningTargetingBehavior = false;
		///<summary>
		///Checks behavioral flags that are considered OOC/Non-Combat
		///</summary>
		public static bool IsInNonCombatBehavior
		{
			get
			{

				//OOC IDing, Town Portal Casting, Town Run
				return ((Profile.CurrentProfileBehaviorType!= Profile.ProfileBehaviorTypes.Unknown 
							&& Profile.ProfileBehaviorIsOutOfCombat) || 
					ExitGameBehavior.BehaviorEngaged || 
					TownPortalBehavior.FunkyTPBehaviorFlag || 
					BrainBehavior.IsVendoring ||
					Game.ForceOutOfCombatBehavior);
			}
		}

		public static GameId CurrentGameID = new GameId();
		public static bool AdventureMode { get; set; }
		public static Profile Profile = new Profile();
		public static BountyCache Bounty = new BountyCache();


		private static GameCache _game=new GameCache();
		public static GameCache Game
		{
			get { return _game; }
			set { _game = value; }
		}

		private static ActiveHero _hero = new ActiveHero();
		///<summary>
		///Current Character
		///</summary>
		public static ActiveHero Hero
		{
			get { return _hero; }
			set { _hero = value; }
		}

		public static Navigation.Navigation Navigation = new Navigation.Navigation();
		private static TargetingClass targeting = new TargetingClass();
		public static TargetingClass Targeting
		{
			get { return targeting; }
			set { targeting = value; }
		}

        ///<summary>
        ///Tracking of All Game Stats 
        ///</summary>
	    public static Stats.Stats CurrentStats;


		internal static bool ShouldRefreshAccountDetails
		{
			set
			{
				ShouldRefreshAccountName = value;
				ShouldRefreshHeroName = value;
				ShouldRefreshClass = value;
				ShouldRefreshDifficulty = value;
				ShouldRefreshHeroLevel = value;
			}
		}

		private static bool ShouldRefreshAccountName = true;
		private static bool ShouldRefreshHeroName = true;
		internal static bool ShouldRefreshClass = true;
		private static bool ShouldRefreshDifficulty = true;
		private static bool ShouldRefreshHeroLevel = true;

		private static string _CurrentAccountName = String.Empty;
		public static string CurrentAccountName
		{
			get 
			{
				if (ShouldRefreshAccountName)
				{
					if (!BotMain.IsRunning)
						ZetaDia.Memory.ClearCache();

					string tmpAccountName = String.Empty;
					using (ZetaDia.Memory.AcquireFrame())
					{
						try
						{
							tmpAccountName = ZetaDia.Service.Hero.BattleTagName;
						}
						catch
						{
							Logger.DBLog.DebugFormat("[Funky] Exception Attempting to Update Current Account Name");
							return _CurrentAccountName;
						}
					}

					if (!tmpAccountName.Contains("#"))
					{
						Logger.DBLog.DebugFormat("[Funky] Account Name Invalid (Missing #) - {0}", tmpAccountName);
						return _CurrentAccountName;
					}
					if (tmpAccountName.Trim() == String.Empty)
					{
						Logger.DBLog.DebugFormat("[Funky] Account Details are Invalid");
						return _CurrentAccountName;
					}

					_CurrentAccountName = tmpAccountName;
					if (BotMain.IsRunning) ShouldRefreshAccountName = false;
				}

				return _CurrentAccountName;
			}
		}

		private static string _CurrentHeroName = String.Empty;
		public static string CurrentHeroName
		{
			get 
			{
				if (ShouldRefreshHeroName)
				{
					if (!BotMain.IsRunning)
						ZetaDia.Memory.ClearCache();

					string tmpHeroName = String.Empty;
					using (ZetaDia.Memory.AcquireFrame())
					{
						try
						{
							//Get current Names!
							tmpHeroName = ZetaDia.Service.Hero.Name;
						}
						catch
						{
							Logger.DBLog.DebugFormat("[Funky] Exception Attempting to Update Current Hero Name.");
							return _CurrentHeroName;
						}
					}

					//Check the values..
					if (tmpHeroName.ToCharArray().Any(c => !Char.IsLetter(c)))
					{
						Logger.DBLog.DebugFormat("[Funky] Hero Name Invalid (Contains Non-Letter Character) - {0}", tmpHeroName);
						return _CurrentHeroName;
					}
					if (tmpHeroName.Trim() == String.Empty)
					{
						Logger.DBLog.DebugFormat("[Funky] Hero Name is Invalid");
						return _CurrentHeroName;
					}

					_CurrentHeroName = tmpHeroName;
					if (BotMain.IsRunning) ShouldRefreshHeroName = false;
				}

				return _CurrentHeroName; 
			}
		}

		internal static ActorClass _CurrentActorClass = ActorClass.Invalid;
		public static ActorClass CurrentActorClass
		{
			get
			{
				if (ShouldRefreshClass)
				{
					if (!BotMain.IsRunning)
						ZetaDia.Memory.ClearCache();

					ActorClass tmpActorClass = ActorClass.Invalid;
					using (ZetaDia.Memory.AcquireFrame())
					{
						try
						{
							//Get current Names!
							tmpActorClass = ZetaDia.Service.Hero.Class;
						}
						catch
						{
							Logger.DBLog.DebugFormat("[Funky] Exception Attempting to Update Current Actor Class.");
							return _CurrentActorClass;
						}
					}

					if (tmpActorClass==ActorClass.Invalid)
					{
						Logger.DBLog.DebugFormat("[Funky] Invalid Actor Class Returned!");
						return _CurrentActorClass;
					}

					_CurrentActorClass = tmpActorClass;
					if (BotMain.IsRunning) ShouldRefreshClass = false;
				}

				return _CurrentActorClass;
			}
		}

		private static GameDifficulty _CurrentGameDifficulty = GameDifficulty.Normal;
		public static GameDifficulty CurrentGameDifficulty
		{
			get
			{
				if (ShouldRefreshDifficulty)
				{
					if (!BotMain.IsRunning)
						ZetaDia.Memory.ClearCache();

					GameDifficulty tmpGameDifficulty = GameDifficulty.Normal;
					using (ZetaDia.Memory.AcquireFrame())
					{
						try
						{
							//Get current Names!
							tmpGameDifficulty = ZetaDia.Service.Hero.CurrentDifficulty;
						}
						catch
						{
							Logger.DBLog.DebugFormat("[Funky] Exception Attempting to Update Current Game Difficulty.");
							return _CurrentGameDifficulty;
						}
					}

					_CurrentGameDifficulty = tmpGameDifficulty;
					if (BotMain.IsRunning) ShouldRefreshDifficulty = false;
				}

				return _CurrentGameDifficulty;
			}
		}

		private static int _CurrentHeroLevel = 0;
		public static int CurrentHeroLevel
		{
			get
			{
				if (ShouldRefreshHeroLevel)
				{
					if (!BotMain.IsRunning)
						ZetaDia.Memory.ClearCache();

					int tmpHeroLevel = 0;
					using (ZetaDia.Memory.AcquireFrame())
					{
						try
						{
							//Get current Names!
							tmpHeroLevel = ZetaDia.Service.Hero.Level;
						}
						catch
						{
							Logger.DBLog.DebugFormat("[Funky] Exception Attempting to Update Current Hero Level.");
							return _CurrentHeroLevel;
						}
					}

					_CurrentHeroLevel = tmpHeroLevel;
					if (BotMain.IsRunning) ShouldRefreshHeroLevel = false;
				}

				return _CurrentHeroLevel;
			}
		}

		public delegate void ItemPickupEvaluation(CacheItem item);
		public static event ItemPickupEvaluation OnItemPickupEvaluation;
		internal static void ItemPickupEval(CacheItem item)
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
			PluginItemTypes thisPluginItemType = PluginItemTypes.Unknown;
		    PluginBaseItemTypes thisGilesBaseType = PluginBaseItemTypes.Unknown;

		    if (item.BalanceID.HasValue && item.BalanceData != null)
		    {
                thisPluginItemType = item.BalanceData.PluginType;
		        thisGilesBaseType = item.BalanceData.PluginBase;
		    }
		    else
		    {
		        thisPluginItemType = ItemFunc.DetermineItemType(item.InternalName, ItemType.Unknown, FollowerType.None,
		            item.SNOID);
                thisGilesBaseType = ItemFunc.DetermineBaseType(thisPluginItemType);
            }


		    if (thisPluginItemType == PluginItemTypes.MiscBook)
				return FunkyBaseExtension.Settings.Loot.ExpBooks;

			if (thisPluginItemType == PluginItemTypes.RamaladnisGift)
				return true;

			// Error logging for DemonBuddy item mis-reading
			ItemType gilesDBItemType = ItemFunc.PluginItemTypeToDBItemType(thisPluginItemType);
            if (item.BalanceID.HasValue && item.BalanceData != null && gilesDBItemType != item.BalanceData.Type)
			{
				Logger.DBLog.InfoFormat("GSError: Item type mis-match detected: Item Internal=" + item.InternalName + ". DemonBuddy ItemType thinks item type is=" + item.BalanceData.Type + ". Giles thinks item type is=" +
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

			        if (FunkyBaseExtension.Settings.Loot.PickupWhiteItems == 1 &&
			            FunkyBaseExtension.Settings.Loot.PickupMagicItems == 1 &&
			            FunkyBaseExtension.Settings.Loot.PickupRareItems == 1 &&
			            FunkyBaseExtension.Settings.Loot.PickupLegendaryItems == 1)
			        {
			            return true;
			        }

                    if (item.Itemquality.HasValue && item.BalanceID.HasValue && item.BalanceData != null)
					{
						switch (item.Itemquality.Value)
						{
							case ItemQuality.Inferior:
							case ItemQuality.Normal:
							case ItemQuality.Superior:
								return FunkyBaseExtension.Settings.Loot.PickupWhiteItems > 0 && (item.BalanceData.ItemLevel >= FunkyBaseExtension.Settings.Loot.PickupWhiteItems);
							case ItemQuality.Magic1:
							case ItemQuality.Magic2:
							case ItemQuality.Magic3:
								return FunkyBaseExtension.Settings.Loot.PickupMagicItems > 0 && (item.BalanceData.ItemLevel >= FunkyBaseExtension.Settings.Loot.PickupMagicItems);
							case ItemQuality.Rare4:
							case ItemQuality.Rare5:
							case ItemQuality.Rare6:
								return FunkyBaseExtension.Settings.Loot.PickupRareItems > 0 && (item.BalanceData.ItemLevel >= FunkyBaseExtension.Settings.Loot.PickupRareItems);
							case ItemQuality.Legendary:
								return FunkyBaseExtension.Settings.Loot.PickupLegendaryItems > 0 && (item.BalanceData.ItemLevel >= FunkyBaseExtension.Settings.Loot.PickupLegendaryItems);
						}
					}

					return false;
				case PluginBaseItemTypes.Gem:
					if (thisPluginItemType == PluginItemTypes.LegendaryGem)
						return true;

                    GemQualityTypes qualityType = ItemFunc.ReturnGemQualityType(item.SNOID, item.BalanceData!=null?item.BalanceData.ItemLevel:-1);
					int qualityLevel = (int)qualityType;

					if (qualityLevel < FunkyBaseExtension.Settings.Loot.MinimumGemItemLevel ||
						(thisPluginItemType == PluginItemTypes.Ruby && !FunkyBaseExtension.Settings.Loot.PickupGems[0]) ||
						(thisPluginItemType == PluginItemTypes.Emerald && !FunkyBaseExtension.Settings.Loot.PickupGems[1]) ||
						(thisPluginItemType == PluginItemTypes.Amethyst && !FunkyBaseExtension.Settings.Loot.PickupGems[2]) ||
						(thisPluginItemType == PluginItemTypes.Topaz && !FunkyBaseExtension.Settings.Loot.PickupGems[3]) ||
						(thisPluginItemType == PluginItemTypes.Diamond && !FunkyBaseExtension.Settings.Loot.PickupGemDiamond))
					{
						return false;
					}
					break;
				case PluginBaseItemTypes.Misc:
					// Note; Infernal keys are misc, so should be picked up here - we aren't filtering them out, so should default to true at the end of this function
					if (thisPluginItemType == PluginItemTypes.CraftingMaterial)
					{
						return FunkyBaseExtension.Settings.Loot.PickupCraftMaterials;
					}
					if (thisPluginItemType == PluginItemTypes.LegendaryCraftingMaterial)
						return true;

					if (thisPluginItemType == PluginItemTypes.CraftingPlan)
					{
						if (!FunkyBaseExtension.Settings.Loot.PickupCraftPlans) return false;

                        if (item.BalanceID.HasValue && item.BalanceData != null)
                        {
                            if (item.BalanceData.IsBlacksmithPlanSixProperties && !FunkyBaseExtension.Settings.Loot.PickupBlacksmithPlanSix) return false;
                            if (item.BalanceData.IsBlacksmithPlanFiveProperties && !FunkyBaseExtension.Settings.Loot.PickupBlacksmithPlanFive) return false;
                            if (item.BalanceData.IsBlacksmithPlanFourProperties && !FunkyBaseExtension.Settings.Loot.PickupBlacksmithPlanFour) return false;

                            if (item.BalanceData.IsBlacksmithPlanArchonSpaulders && !FunkyBaseExtension.Settings.Loot.PickupBlacksmithPlanArchonSpaulders) return false;
                            if (item.BalanceData.IsBlacksmithPlanArchonGauntlets && !FunkyBaseExtension.Settings.Loot.PickupBlacksmithPlanArchonGauntlets) return false;
                            if (item.BalanceData.IsBlacksmithPlanRazorspikes && !FunkyBaseExtension.Settings.Loot.PickupBlacksmithPlanRazorspikes) return false;


                            if (item.BalanceData.IsJewelcraftDesignAmulet && !FunkyBaseExtension.Settings.Loot.PickupJewelerDesignAmulet) return false;
                            if (item.BalanceData.IsJewelcraftDesignFlawlessStarGem && !FunkyBaseExtension.Settings.Loot.PickupJewelerDesignFlawlessStar) return false;
                            if (item.BalanceData.IsJewelcraftDesignMarquiseGem && !FunkyBaseExtension.Settings.Loot.PickupJewelerDesignMarquise) return false;
                            if (item.BalanceData.IsJewelcraftDesignPerfectStarGem && !FunkyBaseExtension.Settings.Loot.PickupJewelerDesignPerfectStar) return false;
                            if (item.BalanceData.IsJewelcraftDesignRadiantStarGem && !FunkyBaseExtension.Settings.Loot.PickupJewelerDesignRadiantStar) return false;
                        }

					}

					if (thisPluginItemType == PluginItemTypes.InfernalKey)
					{
						return FunkyBaseExtension.Settings.Loot.PickupInfernalKeys;
					}

					if (thisPluginItemType == PluginItemTypes.KeyStone)
					{
						return FunkyBaseExtension.Settings.Loot.PickupKeystoneFragments;
					}

					if (thisPluginItemType == PluginItemTypes.BloodShard)
					{

						return Backpack.GetBloodShardCount() < 500;
					}

					// Potion filtering
                    if (thisPluginItemType == PluginItemTypes.HealthPotion || thisPluginItemType == PluginItemTypes.LegendaryHealthPotion)
					{
					    PotionTypes potionType = ItemFunc.ReturnPotionType(item.SNOID);
                        if (potionType== PotionTypes.Regular)
						{
							if (FunkyBaseExtension.Settings.Loot.MaximumHealthPotions <= 0)
								return false;


							var BestPotionToUse = Backpack.ReturnBestPotionToUse();
							if (BestPotionToUse == null)
								return true;

							var Potions = Backpack.ReturnRegularPotions();
							if (Potions.Sum(potions => potions.ThisItemStackQuantity) >= FunkyBaseExtension.Settings.Loot.MaximumHealthPotions)
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


		internal static string sStatusText = "";
		internal static bool bResetStatusText = false;

		public static void Reset()
		{
			Logger.DBLog.InfoFormat("Funky Reseting Bot");
			//TownRunManager.townRunItemCache=new TownRunManager.TownRunCache();
			Hero = new ActiveHero();
			Equipment.RefreshEquippedItemsList();
		    Backpack.ClearBackpackItemCache();

			Settings.PluginSettings.LoadSettings();
			Targeting = new TargetingClass();
			Navigation = new Navigation.Navigation();
		}
		public static void ResetBot()
		{
			Logger.DBLog.InfoFormat("Preforming reset of bot data...");
			BlacklistCache.ClearBlacklistCollections();
			GoldInactivity.LastCoinageUpdate = DateTime.Now;

			PlayerMover.iTotalAntiStuckAttempts = 1;
			PlayerMover.vSafeMovementLocation = Vector3.Zero;
			PlayerMover.vOldPosition = Vector3.Zero;
			PlayerMover.iTimesReachedStuckPoint = 0;
			PlayerMover.timeLastRecordedPosition = DateTime.Today;
			PlayerMover.timeStartedUnstuckMeasure = DateTime.Today;
			PlayerMover.iTimesReachedMaxUnstucks = 0;
			PlayerMover.iCancelUnstuckerForSeconds = 0;
			PlayerMover.timeCancelledUnstuckerFor = DateTime.Today;

			//Reset all data with bot (Playerdata, Combat Data)
			Reset();

			PlayerClass.CreateBotClass();
			//Update character info!
			FunkyGame.Hero.Update();

			//OOC ID Flags
			Targeting.Cache.ShouldCheckItemLooted = false;
			Targeting.Cache.CheckItemLootStackCount = 0;
			//ItemIdentifyBehavior.shouldPreformOOCItemIDing = false;

			//TP Behavior Reset
			TownPortalBehavior.ResetTPBehavior();

			//Sno Trim Timer Reset
			ObjectCache.cacheSnoCollection.ResetTrimTimer();
			//clear obstacles
			ObjectCache.Obstacles.Clear();
			ObjectCache.Objects.Clear();
			//EventHandlers.EventHandlers.DumpedDeathInfo = false;
		}
		public static void ResetGame()
		{
			SkipAheadCache.ClearCache();
			TownPortalBehavior.TownrunStartedInTown = true;
			//TownRunManager._dictItemStashAttempted = new Dictionary<int, int>();
		}
	}
}
