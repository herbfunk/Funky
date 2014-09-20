using System;
using System.Linq;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using fItemPlugin.ItemRules;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;

namespace fItemPlugin.Townrun
{

	internal static partial class TownRunManager
	{
		private static bool bSalvageAllMagic, bSalvageAllNormal, bSalvageAllRare;

		internal static bool GilesSalvageOverlord(object ret)
		{
			townRunItemCache.SalvageItems.Clear();

			//Doing Greater Rift? (But not completed yet..) then skip salvaging.
			if (IsParticipatingInTieredLootRun)
				return false;


			UpdateSalvageItemList();


			if (townRunItemCache.SalvageItems.Count > 0)
			{
				bSalvageAllMagic = false;
				bSalvageAllNormal = false;
				bSalvageAllRare = false;

				townRunItemCache.sortSalvagelist();
				return true;
			}

			return false;
		}

		private static bool bShouldSalvageAllNormal, bShouldSalvageAllMagical, bShouldSalvageAllRare;

		private static void UpdateSalvageItemList()
		{
			//Get new list of current backpack
			Backpack.UpdateItemList();

			bShouldSalvageAllNormal = true;
			bShouldSalvageAllMagical = true;
			bShouldSalvageAllRare = true;

			foreach (var thisitem in Backpack.CacheItemList.Values)
			{
				if (thisitem.ACDItem.BaseAddress != IntPtr.Zero)
				{
					// Find out if this item's in a protected bag slot
					if (!ItemManager.Current.ItemIsProtected(thisitem.ACDItem))
					{
						if (!thisitem.IsSalvagable) continue;

						if (FunkyTownRunPlugin.PluginSettings.UseItemManagerEvaluation)
						{
							//Check if we should keep it!?
							if (ItemManager.Current.ShouldStashItem(thisitem.ACDItem))
								continue;

							if (ItemManager.Current.ShouldSalvageItem(thisitem.ACDItem))
							{
								townRunItemCache.SalvageItems.Add(thisitem);
								continue;
							}

							if (thisitem.ThisQuality < ItemQuality.Magic1)
								bShouldSalvageAllNormal = false;
							else if (thisitem.ThisQuality < ItemQuality.Rare4)
								bShouldSalvageAllMagical = false;
							else if (thisitem.ThisQuality < ItemQuality.Legendary)
								bShouldSalvageAllRare = false;

							continue;

						}
						else
						{
							//Check if we should keep it!?
							if (FunkyTownRunPlugin.PluginSettings.UseItemRules)
							{
								Interpreter.InterpreterAction action = FunkyTownRunPlugin.ItemRulesEval.checkItem(thisitem.ACDItem, ItemEvaluationType.Keep);
								switch (action)
								{
									case Interpreter.InterpreterAction.SALVAGE:
										townRunItemCache.SalvageItems.Add(thisitem);
										continue;
									case Interpreter.InterpreterAction.TRASH:
										if (SalvageValidation(thisitem))
										{
											townRunItemCache.SalvageItems.Add(thisitem);
											continue;
										}
										break;
								}
							}

							if (StashValidation(thisitem))
							{
								continue;
							}
						}



						if (SalvageValidation(thisitem))
						{
							townRunItemCache.SalvageItems.Add(thisitem);
						}
						else
						{
							if (thisitem.ThisQuality < ItemQuality.Magic1)
								bShouldSalvageAllNormal = false;
							else if (thisitem.ThisQuality < ItemQuality.Rare4)
								bShouldSalvageAllMagical = false;
							else if (thisitem.ThisQuality < ItemQuality.Legendary)
								bShouldSalvageAllRare = false;
						}
					}
				}
				else
				{
					FunkyTownRunPlugin.DBLog.DebugFormat("GSError: Diablo 3 memory read error, or item became invalid [StashOver-1]");
				}
			}
		}

		// **********************************************************************************************
		// *****             Pre Salvage sets everything up ready for our blacksmith run            *****
		// **********************************************************************************************

		internal static RunStatus GilesOptimisedPreSalvage(object ret)
		{
			//if (Bot.Settings.Debug.DebugStatusBar)
			BotMain.StatusText = "Town run: Salvage routine started";
			FunkyTownRunPlugin.DBLog.DebugFormat("GSDebug: Salvage routine started.");

			if (ZetaDia.Actors.Me == null)
			{
				ActionsChecked = false;
				FunkyTownRunPlugin.DBLog.DebugFormat("GSError: Diablo 3 memory read error, or item became invalid [PreSalvage-1]");
				return RunStatus.Failure;
			}

			bLoggedJunkThisStash = false;
			MovedToSafetyLocation = false;
			Delay.Reset();
			return RunStatus.Success;
		}

		// **********************************************************************************************
		// *****                 Nice smooth one-at-a-time salvaging replacement                    *****
		// **********************************************************************************************
		internal static RunStatus GilesOptimisedSalvage(object ret)
		{
			if (FunkyGame.GameIsInvalid)
			{
				ActionsChecked = false;
				FunkyTownRunPlugin.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			DiaUnit objBlacksmith = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).FirstOrDefault<DiaUnit>(u => u.Name.StartsWith(SalvageName));
			Vector3 vectorPlayerPosition = ZetaDia.Me.Position;
			Vector3 vectorSalvageLocation = Vector3.Zero;


			//Normal distance we use to move to specific location before moving to NPC
			float _distanceRequired = CurrentAct != Act.A5 ? 50f : 14f; //Act 5 we want short range only!

			if (Vector3.Distance(vectorPlayerPosition, SafetySalvageLocation) <= 2.5f)
				MovedToSafetyLocation = true;

			if (objBlacksmith == null || (!MovedToSafetyLocation && objBlacksmith.Distance > _distanceRequired))
			{
				vectorSalvageLocation = SafetySalvageLocation;
			}
			else
			{
				//MovedToSafetyLocation = true;
				vectorSalvageLocation = objBlacksmith.Position;
			}

			//if (vectorSalvageLocation == Vector3.Zero)
			//	Character.FindActByLevelID(Bot.Character.Data.CurrentWorldDynamicID);


			//Wait until we are not moving
			if (FunkyGame.Hero.IsMoving) return RunStatus.Running;


			float iDistanceFromSell = Vector3.Distance(vectorPlayerPosition, vectorSalvageLocation);
			//Out-Of-Range...
			if (objBlacksmith == null || iDistanceFromSell > 12f)//|| !GilesCanRayCast(vectorPlayerPosition, vectorSalvageLocation, Zeta.Internals.SNO.NavCellFlags.AllowWalk))
			{
				Navigator.PlayerMover.MoveTowards(vectorSalvageLocation);
				return RunStatus.Running;
			}


			if (!UIElements.SalvageWindow.IsVisible)
			{
				objBlacksmith.Interact();
				return RunStatus.Running;
			}

			if (!UIElements.InventoryWindow.IsVisible)
			{
				Backpack.InventoryBackPackToggle(true);
				return RunStatus.Running;
			}

			if (!Delay.Test(1.15)) return RunStatus.Running;

			if (UI.Game.Dialog_Confirmation_OK.IsVisible)
			{
				UI.Game.Dialog_Confirmation_OK.Click();
				return RunStatus.Running;
			}

			if (UI.Game.SalvageAllNormal.IsEnabled && bShouldSalvageAllNormal && townRunItemCache.SalvageItems.Any(i => i.IsSalvagable && i.ThisQuality < ItemQuality.Magic1) && !bSalvageAllNormal)
			{
				//UI.Game.SalvageAllNormal.Click();
				ZetaDia.Me.Inventory.SalvageItemsOfRarity(SalvageRarity.Normal);
				bSalvageAllNormal = true;
				var removalList = townRunItemCache.SalvageItems.Where(i => i.IsSalvagable && i.ThisQuality < ItemQuality.Magic1).ToList();
				foreach (var cacheAcdItem in removalList)
				{
					LogSalvagedItem(cacheAcdItem);
				}
				townRunItemCache.SalvageItems = townRunItemCache.SalvageItems.Except(removalList).ToList();
				return RunStatus.Running;
			}

			if (UI.Game.SalvageAllMagical.IsEnabled && bShouldSalvageAllMagical && townRunItemCache.SalvageItems.Any(i => i.IsSalvagable && i.ThisQuality < ItemQuality.Rare4) && !bSalvageAllMagic)
			{
				//UI.Game.SalvageAllMagical.Click();
				ZetaDia.Me.Inventory.SalvageItemsOfRarity(SalvageRarity.Magic);
				bSalvageAllMagic = true;
				var removalList = townRunItemCache.SalvageItems.Where(i => i.IsSalvagable && i.ThisQuality < ItemQuality.Rare4).ToList();
				foreach (var cacheAcdItem in removalList)
				{
					LogSalvagedItem(cacheAcdItem);
				}
				townRunItemCache.SalvageItems = townRunItemCache.SalvageItems.Except(removalList).ToList();
				return RunStatus.Running;
			}

			if (UI.Game.SalvageAllRare.IsEnabled && bShouldSalvageAllRare && townRunItemCache.SalvageItems.Any(i => i.IsSalvagable && i.ThisQuality < ItemQuality.Legendary) && !bSalvageAllRare)
			{
				//UI.Game.SalvageAllRare.Click();
				ZetaDia.Me.Inventory.SalvageItemsOfRarity(SalvageRarity.Rare);
				bSalvageAllRare = true;
				var removalList = townRunItemCache.SalvageItems.Where(i => i.IsSalvagable && i.ThisQuality < ItemQuality.Legendary).ToList();
				foreach (var cacheAcdItem in removalList)
				{
					LogSalvagedItem(cacheAcdItem);
				}
				townRunItemCache.SalvageItems=townRunItemCache.SalvageItems.Except(removalList).ToList();
				return RunStatus.Running;
			}


			if (townRunItemCache.SalvageItems.Count > 0)
			{
				CacheACDItem thisitem = townRunItemCache.SalvageItems.FirstOrDefault();
				if (thisitem != null && thisitem.ACDItem != null)
				{
					if ((thisitem.IsSalvagable && thisitem.ThisQuality < ItemQuality.Magic1 && bSalvageAllNormal) ||
						(thisitem.IsSalvagable && thisitem.ThisQuality < ItemQuality.Rare4 && bSalvageAllMagic) ||
						(thisitem.IsSalvagable && thisitem.ThisQuality < ItemQuality.Legendary && bSalvageAllRare))
					{
						townRunItemCache.SalvageItems.Remove(thisitem);
						return RunStatus.Running;
					}

					LogSalvagedItem(thisitem);

					ZetaDia.Me.Inventory.SalvageItem(thisitem.ThisDynamicID);

				}
				townRunItemCache.SalvageItems.Remove(thisitem);
				if (townRunItemCache.SalvageItems.Count > 0)
				{
					thisitem = townRunItemCache.SalvageItems.FirstOrDefault();
					if (thisitem != null)
						return RunStatus.Running;
				}
				else
				{
					Delay.Reset();
					return RunStatus.Running;
				}
			}

			//if (RequiresRepair)
			//{
			//	BotMain.StatusText = "Town run: Salvage Routine Interaction Repairing";

			//	if (!Delay.Test()) return RunStatus.Running;

			//	int playerCoinage = ZetaDia.Me.Inventory.Coinage;
			//	int repairCost = ZetaDia.Me.Inventory.GetRepairCost(false);
			//	if (playerCoinage < 100000)
			//	{
			//		FunkyTownRunPlugin.DBLog.InfoFormat("Emergency Stop: You need repairs but don't have enough money. Current Coinage {0} -- Repair Cost {1}", playerCoinage, repairCost);
			//		BotMain.Stop(false, "Not enough gold to repair item(s)!");
			//	}

			//	ZetaDia.Me.Inventory.RepairEquippedItems();
			//	RequiresRepair = false;
			//}


			return RunStatus.Success;
		}

		private static void LogSalvagedItem(CacheACDItem item)
		{
			// Item log for cool stuff stashed
			PluginItemTypes OriginalGilesItemType = ItemFunc.DetermineItemType(item);
			PluginBaseItemTypes thisGilesBaseType = ItemFunc.DetermineBaseType(OriginalGilesItemType);
			if (thisGilesBaseType == PluginBaseItemTypes.WeaponTwoHand || thisGilesBaseType == PluginBaseItemTypes.WeaponOneHand || thisGilesBaseType == PluginBaseItemTypes.WeaponRange ||
				 thisGilesBaseType == PluginBaseItemTypes.Armor || thisGilesBaseType == PluginBaseItemTypes.Jewelry || thisGilesBaseType == PluginBaseItemTypes.Offhand ||
				 thisGilesBaseType == PluginBaseItemTypes.FollowerItem)
			{
				FunkyTownRunPlugin.LogJunkItems(item, thisGilesBaseType, OriginalGilesItemType);
			}
			if (FunkyGame.CurrentGameStats != null)
				FunkyGame.CurrentGameStats.CurrentProfile.LootTracker.SalvagedItemLog(item);
		}

		// **********************************************************************************************
		// *****         Post salvage cleans up and signs off junk log file after salvaging         *****
		// **********************************************************************************************

		internal static RunStatus GilesOptimisedPostSalvage(object ret)
		{
			FunkyTownRunPlugin.DBLog.DebugFormat("GSDebug: Salvage routine ending sequence...");

			FunkyTownRunPlugin.DBLog.DebugFormat("GSDebug: Salvage routine finished.");
			return RunStatus.Success;
		}


		//
	}

}