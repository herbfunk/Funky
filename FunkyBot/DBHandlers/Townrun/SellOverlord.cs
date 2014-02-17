using System;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using Zeta;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.Navigation;
using Zeta.Internals.Actors;
using Zeta.CommonBot;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using FunkyBot.Player;

namespace FunkyBot.DBHandlers
{

	internal static partial class TownRunManager
	{


		// **********************************************************************************************
		// *****  Sell Overlord - determines if we should visit the vendor for repairs or selling   *****
		// **********************************************************************************************
		internal static bool GilesSellOverlord(object ret)
		{
			Bot.Character.Data.BackPack.townRunCache.hashGilesCachedSellItems.Clear();




			//Get new list of current backpack
			Bot.Character.Data.BackPack.Update();
			//Setup any extra potions to sell.


			//Refresh item manager if we are not using item rules nor giles scoring.
			if (!Bot.Settings.ItemRules.UseItemRules && !Bot.Settings.ItemRules.ItemRuleGilesScoring)
				ItemManager.Current.Refresh();

			foreach (var thisitem in Bot.Character.Data.BackPack.CacheItemList.Values)
			{
				if (thisitem.ACDItem.BaseAddress != IntPtr.Zero)
				{
					// Find out if this item's in a protected bag slot
					if (!ItemManager.Current.ItemIsProtected(thisitem.ACDItem))
					{
						if (thisitem.IsPotion)
						{

							if (thisitem.ACDGUID != Bot.Character.Data.BackPack.CurrentPotionACDGUID && Bot.Character.Data.BackPack.CurrentPotionACDGUID != -1)
							{
								Bot.Character.Data.BackPack.townRunCache.hashGilesCachedSellItems.Add(thisitem);
								Logging.Write("Selling Potion -- Current PotionACDGUID=={0}", Bot.Character.Data.BackPack.CurrentPotionACDGUID);
							}
							continue;
						}

						if (Bot.Settings.ItemRules.ItemRulesSalvaging)
							if (Bot.Character.ItemRulesEval.checkSalvageItem(thisitem.ACDItem) == Interpreter.InterpreterAction.SALVAGE)
								continue;


						if (Bot.Settings.ItemRules.UseItemRules)
						{
							Interpreter.InterpreterAction action = Bot.Character.ItemRulesEval.checkItem(thisitem.ACDItem, ItemEvaluationType.Keep);
							switch (action)
							{
								case Interpreter.InterpreterAction.TRASH:
									Bot.Character.Data.BackPack.townRunCache.hashGilesCachedSellItems.Add(thisitem);
									continue;
							}
						}



						//Logging.Write("GilesTrinityScoring == "+Bot.SettingsFunky.ItemRules.ItemRuleGilesScoring.ToString());

						bool bShouldSellThis = Bot.Settings.ItemRules.ItemRuleGilesScoring ? Backpack.GilesSellValidation(thisitem.ThisInternalName, thisitem.ThisLevel, thisitem.ThisQuality, thisitem.ThisDBItemType, thisitem.ThisFollowerType) : ItemManager.Current.ShouldSellItem(thisitem.ACDItem);

						if (bShouldSellThis)
						{
							Bot.Character.Data.BackPack.townRunCache.hashGilesCachedSellItems.Add(thisitem);
						}

					}
				}
				else
				{
					Logging.WriteDiagnostic("GSError: Diablo 3 memory read error, or item became invalid [StashOver-1]");
				}
			}

			bool bShouldVisitVendor = Bot.Character.Data.BackPack.townRunCache.hashGilesCachedSellItems.Count > 0;

			if (bShouldVisitVendor)
			{
				Bot.Character.Data.BackPack.townRunCache.sortSellList();
			}
			else
			{
				if (!bCheckedItemDurability)
				{
					bCheckedItemDurability = true;
					// Check durability percentages
					bNeedsEquipmentRepairs = Bot.Character.Data.BackPack.ShouldRepairItems();
					bShouldVisitVendor = bNeedsEquipmentRepairs;
				}
			}

			return bShouldVisitVendor;
		}

		// **********************************************************************************************
		// *****             Pre Sell sets everything up ready for running to vendor                *****
		// **********************************************************************************************

		internal static RunStatus GilesOptimisedPreSell(object ret)
		{
			if (Bot.Settings.Debug.DebugStatusBar)
				BotMain.StatusText = "Town run: Sell routine started";
			Logging.WriteDiagnostic("GSDebug: Sell routine started.");
			if (ZetaDia.Actors.Me == null)
			{
				Logging.WriteDiagnostic("GSError: Diablo 3 memory read error, or item became invalid [PreSell-1]");
				return RunStatus.Failure;
			}
			bLoggedJunkThisStash = false;
			bCurrentlyMoving = false;
			PotionCheck = false;

			iCurrentItemLoops = 0;
			RandomizeTheTimer();
			bFailedToLootLastItem = false;

			List<CacheACDItem> potions = Bot.Character.Data.BackPack.ReturnCurrentPotions();
			if (potions != null) Bot.Character.Data.iTotalPotions = potions.Any() ? potions.Sum(p => p.ThisItemStackQuantity) : 0;

			return RunStatus.Success;
		}

		// **********************************************************************************************
		// *****    Sell Routine replacement for smooth one-at-a-time item selling and handling     *****
		// **********************************************************************************************

		internal static RunStatus GilesOptimisedSell(object ret)
		{
			string sVendorName = "";
			switch (ZetaDia.CurrentAct)
			{
				case Act.A1:
					sVendorName = "a1_uniquevendor_miner"; break;
				case Act.A2:
					sVendorName = "a2_uniquevendor_peddler"; break;
				case Act.A3:
					sVendorName = "a3_uniquevendor_collector"; break;
				case Act.A4:
					sVendorName = "a4_uniquevendor_collector"; break;
			}

			#region Navigation
			DiaUnit objSellNavigation = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).FirstOrDefault<DiaUnit>(u => u.Name.ToLower().StartsWith(sVendorName));
			Vector3 vectorPlayerPosition = ZetaDia.Me.Position;
			Vector3 vectorSellLocation = new Vector3(0f, 0f, 0f);

			if (objSellNavigation == null)
			{
				switch (ZetaDia.CurrentAct)
				{
					case Act.A1:
						vectorSellLocation = new Vector3(2912.775f, 2803.896f, 24.04533f); break;
					case Act.A2:
						vectorSellLocation = new Vector3(295.2101f, 265.1436f, 0.1000002f); break;
					case Act.A3:
					case Act.A4:
						vectorSellLocation = new Vector3(410.6073f, 355.8762f, 0.1000005f); break;
				}
			}
			else
				vectorSellLocation = objSellNavigation.Position;


			float iDistanceFromSell = Vector3.Distance(vectorPlayerPosition, vectorSellLocation);
			//Out-Of-Range...
			if (objSellNavigation == null)
			//!GilesCanRayCast(vectorPlayerPosition, vectorSellLocation, NavCellFlags.AllowWalk))
			{
				Logging.WriteVerbose("Vendor Obj is Null or Raycast Failed.. using Navigator to move!");
				Navigator.PlayerMover.MoveTowards(vectorSellLocation);
				return RunStatus.Running;
			}
			if (iDistanceFromSell > 40f)
			{
				ZetaDia.Me.UsePower(SNOPower.Walk, vectorSellLocation, ZetaDia.Me.WorldDynamicId);
				return RunStatus.Running;
			}
			if (iDistanceFromSell > 7.5f && !Zeta.Internals.UIElements.VendorWindow.IsValid)
			{
				//Use our click movement
				Bot.NavigationCache.RefreshMovementCache();

				//Wait until we are not moving to send click again..
				if (Bot.NavigationCache.IsMoving)
					return RunStatus.Running;

				objSellNavigation.Interact();
				//ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, vectorSellLocation, ZetaDia.Me.WorldDynamicId, objSellNavigation.ACDGuid);
				return RunStatus.Running;
			}

			if (!Zeta.Internals.UIElements.VendorWindow.IsVisible)
			{
				objSellNavigation.Interact();
				return RunStatus.Running;
			}

			if (!Zeta.Internals.UIElements.InventoryWindow.IsVisible)
			{
				Bot.Character.Data.BackPack.InventoryBackPackToggle(true);
				return RunStatus.Running;
			}
			#endregion

			#region SellItem
			if (Bot.Character.Data.BackPack.townRunCache.hashGilesCachedSellItems.Count > 0)
			{
				iCurrentItemLoops++;
				if (iCurrentItemLoops < iItemDelayLoopLimit)
					return RunStatus.Running;
				iCurrentItemLoops = 0;
				RandomizeTheTimer();

				CacheACDItem thisitem = Bot.Character.Data.BackPack.townRunCache.hashGilesCachedSellItems.FirstOrDefault();
				// Item log for cool stuff sold
				if (thisitem != null)
				{
					GilesItemType OriginalGilesItemType = Backpack.DetermineItemType(thisitem.ThisInternalName, thisitem.ThisDBItemType, thisitem.ThisFollowerType);
					GilesBaseItemType thisGilesBaseType = Backpack.DetermineBaseType(OriginalGilesItemType);
					if (thisGilesBaseType == GilesBaseItemType.WeaponTwoHand || thisGilesBaseType == GilesBaseItemType.WeaponOneHand || thisGilesBaseType == GilesBaseItemType.WeaponRange ||
						 thisGilesBaseType == GilesBaseItemType.Armor || thisGilesBaseType == GilesBaseItemType.Jewelry || thisGilesBaseType == GilesBaseItemType.Offhand ||
						 thisGilesBaseType == GilesBaseItemType.FollowerItem)
					{
						double iThisItemValue = Backpack.ValueThisItem(thisitem, OriginalGilesItemType);
						Logger.LogJunkItems(thisitem, thisGilesBaseType, OriginalGilesItemType, iThisItemValue);
					}
					Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.VendoredItemLog(thisitem);
					ZetaDia.Me.Inventory.SellItem(thisitem.ACDItem);
				}
				if (thisitem != null)
					Bot.Character.Data.BackPack.townRunCache.hashGilesCachedSellItems.Remove(thisitem);
				if (Bot.Character.Data.BackPack.townRunCache.hashGilesCachedSellItems.Count > 0)
					return RunStatus.Running;
			}
			#endregion

			#region BuyPotion
			//Check if settings for potion buy is enabled, with less than 99 potions existing!
			if (Bot.Settings.BuyPotionsDuringTownRun && Bot.Character.Data.iTotalPotions < Bot.Settings.Loot.MaximumHealthPotions &&
				 !PotionCheck)
			{
				//Obey the timer, so we don't buy 100 potions in 3 seconds.
				iCurrentItemLoops++;
				if (iCurrentItemLoops < iItemDelayLoopLimit)
					return RunStatus.Running;
				iCurrentItemLoops = 0;
				RandomizeTheTimer();

				//Buy Potions
				int BestPotionID = 0;
				int LastHPValue = 0;
				foreach (ACDItem item in ZetaDia.Me.Inventory.MerchantItems)
				{
					if (item.IsPotion && item.HitpointsGranted > LastHPValue
						 && item.RequiredLevel <= Bot.Character.Data.iMyLevel
						 && item.Gold < ZetaDia.Me.Inventory.Coinage)
					{
						LastHPValue = item.HitpointsGranted;
						BestPotionID = item.DynamicId;
					}
				}
				if (BestPotionID != 0)
				{
					ZetaDia.Me.Inventory.BuyItem(BestPotionID);
					//Update counter
					Bot.Character.Data.iTotalPotions++;
					return RunStatus.Running;
				}
				PotionCheck = true;
			}
			else
				PotionCheck = true;
			#endregion

			if (bNeedsEquipmentRepairs)
			{
				iCurrentItemLoops++;
				if (iCurrentItemLoops < iItemDelayLoopLimit)
					return RunStatus.Running;
				iCurrentItemLoops = 0;
				RandomizeTheTimer();

				if (ZetaDia.Me.Inventory.Coinage < 40000)
				{
					Logging.Write("Emergency Stop: You need repairs but don't have enough money. Stopping the bot to prevent infinite death loop.");
					BotMain.Stop(false, "Not enough gold to repair item(s)!");
				}

				ZetaDia.Me.Inventory.RepairEquippedItems();
				bNeedsEquipmentRepairs = false;
			}


			bCurrentlyMoving = false;
			bReachedSafety = false;
			return RunStatus.Success;
		}

		// **********************************************************************************************
		// *****        Post Sell tidies everything up and signs off junk log after selling         *****
		// **********************************************************************************************



		internal static RunStatus GilesOptimisedPostSell(object ret)
		{
			iCurrentItemLoops++;
			if (iCurrentItemLoops < iItemDelayLoopLimit)
				return RunStatus.Running;

			Logging.WriteDiagnostic("GSDebug: Sell routine ending sequence...");


			if (bLoggedJunkThisStash)
			{
				FileStream LogStream;
				try
				{
					string sLogFileName = Logger.LoggingPrefixString + " -- JunkLog.log";
					LogStream = File.Open(Logger.LoggingFolderPath + sLogFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
					using (StreamWriter LogWriter = new StreamWriter(LogStream))
						LogWriter.WriteLine("");
					//LogStream.Close();
				}
				catch (IOException)
				{
					Logging.WriteDiagnostic("Fatal Error: File access error for signing off the junk log file.");
				}
				bLoggedJunkThisStash = false;
			}

			// See if we can close the inventory window
			if (Zeta.Internals.UIElement.IsValidElement(0x368FF8C552241695))
			{
				try
				{
					var el = Zeta.Internals.UIElement.FromHash(0x368FF8C552241695);
					if (el != null && el.IsValid && el.IsVisible && el.IsEnabled)
						el.Click();
				}
				catch (Exception)
				{
					// Do nothing if it fails, just catching to prevent any big errors/plugin crashes from this
				}
			}

			iLastDistance = 0f;
			iCurrentItemLoops = 0;
			RandomizeTheTimer();
			Logging.WriteDiagnostic("GSDebug: Sell routine finished.");
			Bot.Character.Data.lastPreformedNonCombatAction = DateTime.Now;
			return RunStatus.Success;
		}

	}

}