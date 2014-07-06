using System;
using System.Linq;
using fItemPlugin.ItemRules;
using fItemPlugin.Items;
using fItemPlugin.Player;
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
		// **********************************************************************************************
		// *****         Salvage Overlord determines if we should visit the blacksmith or not       *****
		// **********************************************************************************************
		internal static bool GilesSalvageOverlord(object ret)
		{
			townRunItemCache.SalvageItems.Clear();


			//Get new list of current backpack
			Backpack.UpdateItemList();


			foreach (var thisitem in Backpack.CacheItemList.Values)
			{
				if (thisitem.ACDItem.BaseAddress != IntPtr.Zero)
				{
					// Find out if this item's in a protected bag slot
					if (!ItemManager.Current.ItemIsProtected(thisitem.ACDItem))
					{
						if (thisitem.IsUnidentified || thisitem.ItemType == PluginItemType.HealthPotion || thisitem.IsVendorBought || thisitem.IsHoradricCache) continue;

						bool bShouldVisitSalvage = false;

						if (FunkyTownRunPlugin.PluginSettings.UseItemManagerEvaluation)
						{
							//Check if we should keep it!?
							if (ItemManager.Current.ShouldStashItem(thisitem.ACDItem))
								continue;

							bShouldVisitSalvage = ItemManager.Current.ShouldSalvageItem(thisitem.ACDItem);
						}
						else
						{
							//Check if we should keep it!?
							if (FunkyTownRunPlugin.PluginSettings.UseItemRules)
							{
								Interpreter.InterpreterAction action = FunkyTownRunPlugin.ItemRulesEval.checkItem(thisitem.ACDItem, ItemEvaluationType.Keep);
								switch (action)
								{
									case Interpreter.InterpreterAction.KEEP:
										continue;
									case Interpreter.InterpreterAction.TRASH:
										townRunItemCache.SalvageItems.Add(thisitem);
										continue;
								}
							}

							if (StashValidation(thisitem))
								continue;

							bShouldVisitSalvage = SalvageValidation(thisitem);
						}
							
						

						if (bShouldVisitSalvage)
							townRunItemCache.SalvageItems.Add(thisitem);

					}
				}
				else
				{
					FunkyTownRunPlugin.DBLog.DebugFormat("GSError: Diablo 3 memory read error, or item became invalid [StashOver-1]");
				}
			}

			if (townRunItemCache.SalvageItems.Count > 0)
			{
				townRunItemCache.sortSalvagelist();
				return true;
			}

			return false;
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

			Delay.Reset();
			return RunStatus.Success;
		}

		// **********************************************************************************************
		// *****                 Nice smooth one-at-a-time salvaging replacement                    *****
		// **********************************************************************************************
		internal static RunStatus GilesOptimisedSalvage(object ret)
		{
			if (Character.GameIsInvalid())
			{
				ActionsChecked = false;
				FunkyTownRunPlugin.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			DiaUnit objBlacksmith = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).FirstOrDefault<DiaUnit>(u => u.IsSalvageShortcut);
			Vector3 vectorPlayerPosition = ZetaDia.Me.Position;
			Vector3 vectorSalvageLocation = Vector3.Zero;


			//Normal distance we use to move to specific location before moving to NPC
			float _distanceRequired=CurrentAct!=Act.A5?50f:14f; //Act 5 we want short range only!

			if (objBlacksmith == null || objBlacksmith.Distance > _distanceRequired)
			{
				vectorSalvageLocation = SafetySalvageLocation;
			}
			else
				vectorSalvageLocation = objBlacksmith.Position;

			//if (vectorSalvageLocation == Vector3.Zero)
			//	Character.FindActByLevelID(Bot.Character.Data.CurrentWorldDynamicID);


			//Wait until we are not moving
			if (Character.IsMoving) return RunStatus.Running;


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


			if (townRunItemCache.SalvageItems.Count > 0)
			{
				CacheACDItem thisitem = townRunItemCache.SalvageItems.FirstOrDefault();
				if (thisitem != null)
				{
					// Item log for cool stuff stashed
					PluginItemType OriginalGilesItemType = ItemFunc.DetermineItemType(thisitem);
					PluginBaseItemType thisGilesBaseType = ItemFunc.DetermineBaseType(OriginalGilesItemType);
					if (thisGilesBaseType == PluginBaseItemType.WeaponTwoHand || thisGilesBaseType == PluginBaseItemType.WeaponOneHand || thisGilesBaseType == PluginBaseItemType.WeaponRange ||
						 thisGilesBaseType == PluginBaseItemType.Armor || thisGilesBaseType == PluginBaseItemType.Jewelry || thisGilesBaseType == PluginBaseItemType.Offhand ||
						 thisGilesBaseType == PluginBaseItemType.FollowerItem)
					{
						FunkyTownRunPlugin.LogJunkItems(thisitem, thisGilesBaseType, OriginalGilesItemType);
					}
					FunkyTownRunPlugin.TownRunStats.SalvagedItemLog(thisitem);
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