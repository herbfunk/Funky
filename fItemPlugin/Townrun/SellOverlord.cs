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
		private static bool CheckedVendorActions;
		private static bool bNeedsEquipmentRepairs;
		private static bool bBuyingPotions;
		private static int PotionCount = 0;
		private static int PotionDynamicID = 0;
		private static ACDItem PotionMerchantACDItem;

		// **********************************************************************************************
		// *****  Sell Overlord - determines if we should visit the vendor for repairs or selling   *****
		// **********************************************************************************************
		internal static bool GilesSellOverlord(object ret)
		{
			townRunItemCache.SellItems.Clear();




			//Get new list of current backpack
			Backpack.ReturnRegularPotions();


			foreach (var thisitem in Backpack.CacheItemList.Values)
			{
				if (thisitem.ACDItem.BaseAddress != IntPtr.Zero)
				{
					// Find out if this item's in a protected bag slot
					if (!ItemManager.Current.ItemIsProtected(thisitem.ACDItem))
					{
						if (thisitem.ItemType == PluginItemType.HealthPotion || thisitem.ItemType == PluginItemType.HoradricCache)
						{
							if (thisitem.IsRegularPotion)
							{
								if (thisitem.ACDGUID != Backpack.CurrentPotionACDGUID && Backpack.CurrentPotionACDGUID != -1)
								{
									townRunItemCache.SellItems.Add(thisitem);
									FunkyTownRunPlugin.DBLog.InfoFormat("Selling Potion -- Current PotionACDGUID=={0}", Backpack.CurrentPotionACDGUID);
								}
							}

							continue;
						}

						if (FunkyTownRunPlugin.PluginSettings.UseItemRules)
						{
							Interpreter.InterpreterAction action = FunkyTownRunPlugin.ItemRulesEval.checkItem(thisitem.ACDItem, ItemEvaluationType.Keep);
							switch (action)
							{
								case Interpreter.InterpreterAction.TRASH:
									townRunItemCache.SellItems.Add(thisitem);
									continue;
							}
						}

						bool bShouldSellThis = false;
						if (FunkyTownRunPlugin.PluginSettings.UseItemManagerEvaluation)
						{
							bShouldSellThis = ItemManager.Current.ShouldSellItem(thisitem.ACDItem);
						}
						else
						{
							if (!thisitem.IsVendorBought && !thisitem.IsUnidentified && thisitem.ItemType != PluginItemType.HealthPotion && !thisitem.IsHoradricCache)
							{
								if (SalvageValidation(thisitem)) continue;
							}
							else if (thisitem.IsUnidentified)
							{
								continue;
							}

							bShouldSellThis = SellValidation(thisitem.ThisInternalName, thisitem.ThisLevel, thisitem.ThisQuality, thisitem.ThisDBItemType, thisitem.ThisFollowerType);
						}

						if (bShouldSellThis)
							townRunItemCache.SellItems.Add(thisitem);
					}
				}
				else
				{
					FunkyTownRunPlugin.DBLog.DebugFormat("GSError: Diablo 3 memory read error, or item became invalid [StashOver-1]");
				}
			}

			bool bShouldVisitVendor = townRunItemCache.SellItems.Count > 0;
			if (bShouldVisitVendor)
			{
				FunkyTownRunPlugin.DBLog.InfoFormat("[Funky] Town Run Behavior Vendor Started (Selling Items)");
				townRunItemCache.sortSellList();
				foreach (var item in townRunItemCache.SellItems)
				{
					FunkyTownRunPlugin.DBLog.DebugFormat("Selling Item: {0}({1}) Sno {2}", item.ThisRealName,item.ThisInternalName,item.SNO);
				}
			}

			if (!CheckedVendorActions)
			{
				CheckedVendorActions = true;

				// Check durability percentages
				bNeedsEquipmentRepairs = Backpack.ShouldRepairItems();
				if (bNeedsEquipmentRepairs)
					FunkyTownRunPlugin.DBLog.InfoFormat("[Funky] Town Run Behavior Vendor Started (Repair Items)");

				if (!bShouldVisitVendor)
					bShouldVisitVendor = bNeedsEquipmentRepairs;

				// Check Buying Potions?
				if (FunkyTownRunPlugin.PluginSettings.BuyPotionsDuringTownRun && FunkyTownRunPlugin.PluginSettings.PotionsCount > Backpack.CurrentPotionCount)
				{
					bBuyingPotions = true;
					PotionCount = Backpack.CurrentPotionCount;
					FunkyTownRunPlugin.DBLog.InfoFormat("[Funky] Town Run Behavior Vendor Started (Buying Potions)");
					if (!bShouldVisitVendor)
						bShouldVisitVendor = true;
				}
			}


			return bShouldVisitVendor;
		}

		internal static RunStatus VendorMovement(object ret)
		{
			if (Character.GameIsInvalid())
			{
				ActionsChecked = false;
				FunkyTownRunPlugin.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			BotMain.StatusText = "Town run: Vendor Routine Movement";




			DiaUnit objSellNavigation = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).FirstOrDefault<DiaUnit>(u => u.Name.ToLower().StartsWith(VendorName));
			Vector3 vectorPlayerPosition = ZetaDia.Me.Position;
			Vector3 vectorSellLocation = Vector3.Zero;

			if (objSellNavigation == null)
			{
				vectorSellLocation = SafetyVendorLocation;
			}
			else
				vectorSellLocation = objSellNavigation.Position;


			

			//Out-Of-Range...
			if (objSellNavigation == null)
			{
				FunkyTownRunPlugin.DBLog.InfoFormat("Vendor Obj is Null or Raycast Failed.. using Navigator to move!");
				Navigator.PlayerMover.MoveTowards(vectorSellLocation);
				return RunStatus.Running;
			}

			float iDistanceFromSell = Vector3.Distance(vectorPlayerPosition, vectorSellLocation);

			if (Character.IsMoving) return RunStatus.Running;

			if (iDistanceFromSell > 40f)
			{
				Navigator.MoveTo(vectorSellLocation, "Vendor");
				return RunStatus.Running;
			}

			if (iDistanceFromSell > 7.5f && !UIElements.VendorWindow.IsValid)
			{
				objSellNavigation.Interact();
				return RunStatus.Running;
			}

			if (!UIElements.VendorWindow.IsVisible)
			{
				objSellNavigation.Interact();
				return RunStatus.Running;
			}

			if (!UIElements.InventoryWindow.IsVisible)
			{
				Backpack.InventoryBackPackToggle(true);
				return RunStatus.Running;
			}

			return RunStatus.Success;
		}

		internal static RunStatus GilesOptimisedSell(object ret)
		{
			if (Character.GameIsInvalid())
			{
				ActionsChecked = false;
				FunkyTownRunPlugin.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			BotMain.StatusText = "Town run: Vendor Routine Interaction";

			if (!UIElements.VendorWindow.IsVisible)
			{
				FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Town Run Vednor Behavior Failed! (Vendor Window not visible)");
				return RunStatus.Failure;
			}


			#region SellItem
			if (townRunItemCache.SellItems.Count > 0)
			{
				BotMain.StatusText = "Town run: Vendor Routine Interaction Selling Items";

				if (!Delay.Test()) return RunStatus.Running;

				CacheACDItem thisitem = townRunItemCache.SellItems.FirstOrDefault();
				// Item log for cool stuff sold
				if (thisitem != null)
				{
					PluginItemType OriginalPluginItemType = ItemFunc.DetermineItemType(thisitem);
					PluginBaseItemType thisGilesBaseType = ItemFunc.DetermineBaseType(OriginalPluginItemType);
					if (thisGilesBaseType == PluginBaseItemType.WeaponTwoHand || thisGilesBaseType == PluginBaseItemType.WeaponOneHand || thisGilesBaseType == PluginBaseItemType.WeaponRange ||
						 thisGilesBaseType == PluginBaseItemType.Armor || thisGilesBaseType == PluginBaseItemType.Jewelry || thisGilesBaseType == PluginBaseItemType.Offhand ||
						 thisGilesBaseType == PluginBaseItemType.FollowerItem)
					{
						FunkyTownRunPlugin.LogJunkItems(thisitem, thisGilesBaseType, OriginalPluginItemType);
					}
					
					FunkyTownRunPlugin.TownRunStats.VendoredItemLog(thisitem);

					ZetaDia.Me.Inventory.SellItem(thisitem.ACDItem);
				}
				if (thisitem != null)
					townRunItemCache.SellItems.Remove(thisitem);
				if (townRunItemCache.SellItems.Count > 0)
					return RunStatus.Running;
			}
			#endregion

			#region BuyPotion
			//Check if settings for potion buy is enabled, with less than 99 potions existing!
			if (bBuyingPotions)
			{
				BotMain.StatusText = "Town run: Sell Routine Interaction Buying Potions";
				if (PotionCount >= FunkyTownRunPlugin.PluginSettings.PotionsCount)
				{
					FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Vendor Potion Buying Finished (Potion Count Greater Than Setting) Count {0} Setting {1}", PotionCount, FunkyTownRunPlugin.PluginSettings.PotionsCount);
					bBuyingPotions = false;
					return RunStatus.Running;
				}

				//Obey the timer, so we don't buy 100 potions in 3 seconds.
				if (!Delay.Test(1.5)) return RunStatus.Running;


				//Update Dynamic ID
				if (PotionDynamicID == 0)
				{
					FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Vendor Potion updating Dynamic ID!");

					foreach (ACDItem item in ZetaDia.Me.Inventory.MerchantItems)
					{
						if (item.IsPotion)
						{
							PotionMerchantACDItem = item;
							PotionDynamicID = item.DynamicId;
							break;
						}
					}

					//Check we found a potion..
					if (PotionDynamicID == 0)
					{
						FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Vendor Potion Buying Finished (Failed to find Potion Dynamic ID)");
						bBuyingPotions = false;
					}
					else
					{
						FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Vendor Buying Potion Dynamic ID {0}", PotionDynamicID);
					}

					return RunStatus.Running;
				}


				//Check we have enough gold!
				if (PotionMerchantACDItem.Gold > ZetaDia.Me.Inventory.Coinage)
				{
					FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Vendor Potion Buying Finished (Not Enough Gold to buy Potions)");
					bBuyingPotions = false;
					return RunStatus.Running;
				}

				FunkyTownRunPlugin.DBLog.DebugFormat("[Funky] Vendor Buying Potion!");
				ZetaDia.Actors.Me.Inventory.BuyItem(PotionDynamicID);
				//Update counter
				PotionCount++;
				return RunStatus.Running;

			}

			#endregion

			if (bNeedsEquipmentRepairs)
			{
				BotMain.StatusText = "Town run: Vendor Routine Interaction Repairing";

				if (!Delay.Test()) return RunStatus.Running;

				int playerCoinage = ZetaDia.Me.Inventory.Coinage;
				int repairCost = ZetaDia.Me.Inventory.GetRepairCost(false);
				if (playerCoinage < 100000)
				{
					FunkyTownRunPlugin.DBLog.InfoFormat("Emergency Stop: You need repairs but don't have enough money. Current Coinage {0} -- Repair Cost {1}", playerCoinage, repairCost);
					BotMain.Stop(false, "Not enough gold to repair item(s)!");
				}

				ZetaDia.Me.Inventory.RepairEquippedItems();
				bNeedsEquipmentRepairs = false;
			}


			
			
			return RunStatus.Success;
		}

		internal static RunStatus GilesOptimisedPreSell(object ret)
		{
			//if (Bot.Settings.Debug.DebugStatusBar)
			BotMain.StatusText = "Town run: Vendor routine started";
			FunkyTownRunPlugin.DBLog.DebugFormat("GSDebug: Sell routine started.");
			if (ZetaDia.Actors.Me == null)
			{
				ActionsChecked = false;
				FunkyTownRunPlugin.DBLog.DebugFormat("GSError: Diablo 3 memory read error, or item became invalid [PreSell-1]");
				return RunStatus.Failure;
			}
			bLoggedJunkThisStash = false;
			PotionMerchantACDItem = null;
			PotionDynamicID = 0;
			PotionMerchantACDItem = null;
			Delay.Reset();

			Backpack.ReturnRegularPotions();
			//if (potions != null) Bot.Character.Data.iTotalPotions = potions.Any() ? potions.Sum(p => p.ThisItemStackQuantity) : 0;

			return RunStatus.Success;
		}
		internal static RunStatus GilesOptimisedPostSell(object ret)
		{
			if (!Delay.Test()) return RunStatus.Running;

			FunkyTownRunPlugin.DBLog.DebugFormat("GSDebug: Sell routine ending sequence...");


			//if (bLoggedJunkThisStash)
			//{
			//	FileStream LogStream;
			//	try
			//	{
			//		string sLogFileName = FunkyTownRunPlugin.LoggingPrefixString + " -- JunkLog.log";
			//		LogStream = File.Open(FolderPaths.LoggingFolderPath + sLogFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
			//		using (StreamWriter LogWriter = new StreamWriter(LogStream))
			//			LogWriter.WriteLine("");
			//		//LogStream.Close();
			//	}
			//	catch (IOException)
			//	{
			//		FunkyTownRunPlugin.DBLog.DebugFormat("Fatal Error: File access error for signing off the junk log file.");
			//	}
			//	bLoggedJunkThisStash = false;
			//}

			// See if we can close the inventory window
			if (UIElement.IsValidElement(0x368FF8C552241695))
			{
				try
				{
					var el = UIElement.FromHash(0x368FF8C552241695);
					if (el != null && el.IsValid && el.IsVisible && el.IsEnabled)
						el.Click();
				}
				catch (Exception)
				{
					// Do nothing if it fails, just catching to prevent any big errors/plugin crashes from this
				}
			}


			Delay.Reset();
			

			PotionDynamicID = 0;
			PotionMerchantACDItem = null;

			FunkyTownRunPlugin.DBLog.DebugFormat("GSDebug: Sell routine finished.");
			//Bot.Character.Data.lastPreformedNonCombatAction = DateTime.Now;
			return RunStatus.Success;
		}

	}

}