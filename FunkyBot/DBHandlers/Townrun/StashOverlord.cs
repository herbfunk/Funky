using System;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Zeta.TreeSharp;
using System.Linq;
using System.Globalization;
using FunkyBot.Player;

namespace FunkyBot.DBHandlers
{

	internal static partial class TownRunManager
	{


		// **********************************************************************************************
		// *****       Stash Overlord values all items and checks if we have anything to stash      *****
		// **********************************************************************************************
		internal static bool GilesStashOverlord(object ret)
		{

			//Clear Cache Item List -- (This is the first to be ran so we want fresh data!)
			//Bot.Character_.Data.BackPack.CacheItemList.Clear();

			Bot.Character.Data.BackPack.townRunCache.hashGilesCachedKeepItems.Clear();

			//Get new list of current backpack
			Bot.Character.Data.BackPack.Update();

			//Refresh item manager if we are not using item rules nor giles scoring.
			if (!Bot.Settings.ItemRules.UseItemRules && !Bot.Settings.ItemRules.ItemRuleGilesScoring)
				ItemManager.Current.Refresh();

			//Update any Unidified items that we may have..
			var UnidItemACDGUIDs = Bot.Character.Data.BackPack.CacheItemList.Where(i => i.Value.IsUnidentified).Select(i => i.Key).ToList();
			foreach (int unidItemAcdguiD in UnidItemACDGUIDs)
			{
				using (ZetaDia.Memory.AcquireFrame())
				{
					Bot.Character.Data.BackPack.CacheItemList[unidItemAcdguiD] = new CacheACDItem(Bot.Character.Data.BackPack.CacheItemList[unidItemAcdguiD].ACDItem);

				}
			}

			foreach (var thisitem in Bot.Character.Data.BackPack.CacheItemList.Values)
			{
				if (thisitem.ACDItem.BaseAddress != IntPtr.Zero)
				{
					// Find out if this item's in a protected bag slot
					if (!ItemManager.Current.ItemIsProtected(thisitem.ACDItem))
					{
						//Don't Stash Potions.
						if (thisitem.ThisDBItemType== ItemType.Potion) continue;

						if (Bot.Settings.ItemRules.UseItemRules)
						{
							Interpreter.InterpreterAction action = Bot.Character.ItemRulesEval.checkItem(thisitem.ACDItem, ItemEvaluationType.Keep);
							switch (action)
							{
								case Interpreter.InterpreterAction.KEEP:
									Bot.Character.Data.BackPack.townRunCache.hashGilesCachedKeepItems.Add(thisitem);
									continue;
								case Interpreter.InterpreterAction.TRASH:
									continue;
							}
						}


						bool bShouldStashThis = (Bot.Settings.ItemRules.ItemRuleGilesScoring ? Backpack.ShouldWeStashThis(thisitem)
							: ItemManager.Current.ShouldStashItem(thisitem.ACDItem));


						//Logger.DBLog.InfoFormat("GilesTrinityScoring == "+Bot.SettingsFunky.ItemRules.ItemRuleGilesScoring.ToString());



						if (bShouldStashThis)
						{
							Bot.Character.Data.BackPack.townRunCache.hashGilesCachedKeepItems.Add(thisitem);
						}
					}
				}
				else
				{
					Logger.DBLog.DebugFormat("GSError: Diablo 3 memory read error, or item became invalid [StashOver-1]");
				}
			}

			return Bot.Character.Data.BackPack.townRunCache.hashGilesCachedKeepItems.Count > 0;
		}

		// **********************************************************************************************
		// *****                     Pre Stash prepares stuff for our stash run                     *****
		// **********************************************************************************************
		internal static RunStatus GilesOptimisedPreStash(object ret)
		{
			if (Bot.Settings.Debug.DebugStatusBar)
				BotMain.StatusText = "Town run: Stash routine started";
			Logger.DBLog.DebugFormat("GSDebug: Stash routine started.");
			bLoggedAnythingThisStash = false;
			bUpdatedStashMap = false;
			iCurrentItemLoops = 0;
			RandomizeTheTimer();

			return RunStatus.Success;
		}

		// **********************************************************************************************
		// *****            Post Stash tidies up and signs off log file after a stash run           *****
		// **********************************************************************************************
		internal static RunStatus GilesOptimisedPostStash(object ret)
		{
			Logger.DBLog.DebugFormat("GSDebug: Stash routine ending sequence...");
			// See if there's any legendary items we should send Prowl notifications about
			while (Funky.pushQueue.Count > 0) { Funky.SendNotification(Funky.pushQueue.Dequeue()); }
			/*
			 if (bLoggedAnythingThisStash)
			 {
				   FileStream LogStream = null;
				   try
				   {
						LogStream = File.Open(sTrinityPluginPath + ZetaDia.Service.CurrentHero.BattleTagName + " - StashLog - " + ZetaDia.Actors.Me.ActorClass.ToString() + ".log", FileMode.Append, FileAccess.Write, FileShare.Read);
						using (StreamWriter LogWriter = new StreamWriter(LogStream))
							 LogWriter.WriteLine("");
						LogStream.Close();
				   }
				   catch (IOException)
				   {
						Logger.DBLog.InfoFormat("Fatal Error: File access error for signing off the stash log file.");
						if (LogStream != null)
							 LogStream.Close();
				   }
				   bLoggedAnythingThisStash = false;
			 }
			   */
			Bot.Character.Data.lastPreformedNonCombatAction = DateTime.Now;
			bFailedToLootLastItem = false;

			Logger.DBLog.DebugFormat("GSDebug: Stash routine finished.");
			return RunStatus.Success;
		}

		// **********************************************************************************************
		// *****                  Lovely smooth one-at-a-time stashing routine                      *****
		// **********************************************************************************************
		internal static RunStatus GilesOptimisedStash(object ret)
		{
			if (ZetaDia.Actors.Me == null)
			{
				Logger.DBLog.DebugFormat("GSError: Diablo 3 memory read error, or item became invalid [CoreStash-1]");
				return RunStatus.Failure;
			}
			Vector3 vectorPlayerPosition = ZetaDia.Me.Position;
			Vector3 vectorStashLocation = new Vector3(0f, 0f, 0f);
			DiaObject objPlayStash = ZetaDia.Actors.GetActorsOfType<GizmoPlayerSharedStash>(true).FirstOrDefault<GizmoPlayerSharedStash>();
			if (objPlayStash != null)
				vectorStashLocation = objPlayStash.Position;
			else if (!ZetaDia.IsInTown)
				return RunStatus.Failure;
			else
			{
				//Setup vector for movement
				switch (ZetaDia.CurrentAct)
				{
					case Act.A1:
						vectorStashLocation = new Vector3(2967.146f, 2799.459f, 24.04533f); break;
					case Act.A2:
						vectorStashLocation = new Vector3(323.4543f, 228.5806f, 0.1f); break;
					case Act.A3:
					case Act.A4:
						vectorStashLocation = new Vector3(389.3798f, 390.7143f, 0.3321428f); break;
				}
			}

			float iDistanceFromStash = Vector3.Distance(vectorPlayerPosition, vectorStashLocation);
			if (iDistanceFromStash > 120f)
				return RunStatus.Failure;

			//Out-Of-Range...
			if (objPlayStash == null)
			{
				Navigator.PlayerMover.MoveTowards(vectorStashLocation);
				return RunStatus.Running;
			}
			if (iDistanceFromStash > 40f)
			{
				ZetaDia.Me.UsePower(SNOPower.Walk, vectorStashLocation, ZetaDia.Me.WorldDynamicId);
				return RunStatus.Running;
			}
			if (iDistanceFromStash > 7.5f && !UIElements.StashWindow.IsVisible)
			{
				//Use our click movement
				Bot.NavigationCache.RefreshMovementCache();

				//Wait until we are not moving to send click again..
				if (Bot.NavigationCache.IsMoving) return RunStatus.Running;

				ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, vectorStashLocation, ZetaDia.Me.WorldDynamicId, objPlayStash.ACDGuid);
				return RunStatus.Running;
			}

			if (!UIElements.StashWindow.IsVisible)
			{
				objPlayStash.Interact();
				return RunStatus.Running;
			}

			if (!bUpdatedStashMap)
			{
				// Array for what blocks are or are not blocked
				for (int iRow = 0; iRow <= 29; iRow++)
					for (int iColumn = 0; iColumn <= 6; iColumn++)
						GilesStashSlotBlocked[iColumn, iRow] = false;
				// Block off the entire of any "protected stash pages"
				foreach (int iProtPage in CharacterSettings.Instance.ProtectedStashPages)
					for (int iProtRow = 0; iProtRow <= 9; iProtRow++)
						for (int iProtColumn = 0; iProtColumn <= 6; iProtColumn++)
							GilesStashSlotBlocked[iProtColumn, iProtRow + (iProtPage * 10)] = true;
				// Remove rows we don't have
				for (int iRow = (ZetaDia.Me.NumSharedStashSlots / 7); iRow <= 29; iRow++)
					for (int iColumn = 0; iColumn <= 6; iColumn++)
						GilesStashSlotBlocked[iColumn, iRow] = true;

				//StashedItems.Clear();
				// Map out all the items already in the stash
				foreach (ACDItem tempitem in ZetaDia.Me.Inventory.StashItems)
				{
					if (tempitem.BaseAddress != IntPtr.Zero)
					{
						//StashedItems.Add(new CacheACDItem(tempitem));
						int inventoryRow = tempitem.InventoryRow;
						int inventoryColumn = tempitem.InventoryColumn;
						// Mark this slot as not-free
						GilesStashSlotBlocked[inventoryColumn, inventoryRow] = true;
						// Try and reliably find out if this is a two slot item or not
						GilesItemType tempItemType = Backpack.DetermineItemType(tempitem.InternalName, tempitem.ItemType, tempitem.FollowerSpecialType);
						if (Backpack.DetermineIsTwoSlot(tempItemType) && inventoryRow != 19 && inventoryRow != 9 && inventoryRow != 29)
						{
							GilesStashSlotBlocked[inventoryColumn, inventoryRow + 1] = true;
						}
						else if (Backpack.DetermineIsTwoSlot(tempItemType) && (inventoryRow == 19 || inventoryRow == 9 || inventoryRow == 29))
						{
							Logger.DBLog.DebugFormat("GSError: DemonBuddy thinks this item is 2 slot even though it's at bottom row of a stash page: " + tempitem.Name + " [" + tempitem.InternalName +
								  "] type=" + tempItemType.ToString() + " @ slot " + (inventoryRow + 1).ToString(CultureInfo.InvariantCulture) + "/" +
								  (inventoryColumn + 1).ToString(CultureInfo.InvariantCulture));
						}
					}
				} // Loop through all stash items
				bUpdatedStashMap = true;
			} // Need to update the stash map?


			if (Bot.Character.Data.BackPack.townRunCache.hashGilesCachedKeepItems.Count > 0)
			{
				iCurrentItemLoops++;
				if (iCurrentItemLoops < iItemDelayLoopLimit)
					return RunStatus.Running;
				iCurrentItemLoops = 0;
				RandomizeTheTimer();
				CacheACDItem thisitem = Bot.Character.Data.BackPack.townRunCache.hashGilesCachedKeepItems.FirstOrDefault();
				if (LastStashPoint[0] < 0 && LastStashPoint[1] < 0 && LastStashPage < 0)
				{
					bool bDidStashSucceed = GilesStashAttempt(thisitem, out LastStashPoint, out LastStashPage);
					if (!bDidStashSucceed)
					{
						Logger.DBLog.DebugFormat("There was an unknown error stashing an item.");
						if (OutOfGame.MuleBehavior)
							return RunStatus.Success;
					}
					else
						return RunStatus.Running;
				}
				else
				{
					//We have a valid place to stash.. so lets check if stash page is currently open
					if (ZetaDia.Me.Inventory.CurrentStashPage == LastStashPage)
					{
						//Herbfunk: Current Game Stats
						Bot.Game.CurrentGameStats.CurrentProfile.LootTracker.StashedItemLog(thisitem);

						ZetaDia.Me.Inventory.MoveItem(thisitem.ThisDynamicID, ZetaDia.Me.CommonData.DynamicId, InventorySlot.SharedStash, LastStashPoint[0], LastStashPoint[1]);
						LastStashPoint = new[] { -1, -1 };
						LastStashPage = -1;

						if (thisitem != null)
							Bot.Character.Data.BackPack.townRunCache.hashGilesCachedKeepItems.Remove(thisitem);
						if (Bot.Character.Data.BackPack.townRunCache.hashGilesCachedKeepItems.Count > 0)
							return RunStatus.Running;
					}
					else
					{
						//Lets switch the current page..
						ZetaDia.Me.Inventory.SwitchStashPage(LastStashPage);
						return RunStatus.Running;
					}
				}
			}
			return RunStatus.Success;
		}

		// **********************************************************************************************
		// *****          Stash replacement accurately and neatly finds a free stash location       *****
		// **********************************************************************************************
		private static bool GilesStashAttempt(CacheACDItem item, out int[] XY, out int StashPage)
		{
			XY = new[] { -1, -1 };
			StashPage = -1;

			int iPlayerDynamicID = ZetaDia.Me.CommonData.DynamicId;
			int iOriginalGameBalanceId = item.ThisBalanceID;
			int iOriginalDynamicID = item.ThisDynamicID;
			int iOriginalStackQuantity = item.ThisItemStackQuantity;
			string sOriginalItemName = item.ThisRealName;
			string sOriginalInternalName = item.ThisInternalName;
			GilesItemType OriginalGilesItemType = Backpack.DetermineItemType(item.ThisInternalName, item.ThisDBItemType, item.ThisFollowerType);
			GilesBaseItemType thisGilesBaseType = Backpack.DetermineBaseType(OriginalGilesItemType);
			bool bOriginalTwoSlot = Backpack.DetermineIsTwoSlot(OriginalGilesItemType);
			bool bOriginalIsStackable = Backpack.DetermineIsStackable(OriginalGilesItemType);
			int iAttempts;
			if (_dictItemStashAttempted.TryGetValue(iOriginalDynamicID, out iAttempts))
			{
				Logger.DBLog.InfoFormat("GSError: Detected a duplicate stash attempt, DB item mis-read error, now forcing this item as a 2-slot item");
				_dictItemStashAttempted[iOriginalDynamicID] = iAttempts + 1;
				bOriginalTwoSlot = true;
				bOriginalIsStackable = false;
				if (iAttempts > 6)
				{
					Logger.DBLog.InfoFormat("GSError: Detected an item stash loop risk, now re-mapping stash treating everything as 2-slot and re-attempting");
					// Array for what blocks are or are not blocked
					for (int iRow = 0; iRow <= 29; iRow++)
						for (int iColumn = 0; iColumn <= 6; iColumn++)
							GilesStashSlotBlocked[iColumn, iRow] = false;
					// Block off the entire of any "protected stash pages"
					foreach (int iProtPage in CharacterSettings.Instance.ProtectedStashPages)
						for (int iProtRow = 0; iProtRow <= 9; iProtRow++)
							for (int iProtColumn = 0; iProtColumn <= 6; iProtColumn++)
								GilesStashSlotBlocked[iProtColumn, iProtRow + (iProtPage * 10)] = true;
					// Remove rows we don't have
					for (int iRow = (ZetaDia.Me.NumSharedStashSlots / 7); iRow <= 29; iRow++)
						for (int iColumn = 0; iColumn <= 6; iColumn++)
							GilesStashSlotBlocked[iColumn, iRow] = true;
					// Map out all the items already in the stash
					foreach (ACDItem tempitem in ZetaDia.Me.Inventory.StashItems)
					{
						if (tempitem.BaseAddress != IntPtr.Zero)
						{
							int inventoryRow = tempitem.InventoryRow;
							int inventoryColumn = tempitem.InventoryColumn;
							// Mark this slot as not-free
							GilesStashSlotBlocked[inventoryColumn, inventoryRow] = true;
							// Try and reliably find out if this is a two slot item or not
							GilesStashSlotBlocked[inventoryColumn, inventoryRow + 1] = true;
							if (inventoryRow != 19 && inventoryRow != 9 && inventoryRow != 29)
							{
								GilesStashSlotBlocked[inventoryColumn, inventoryRow + 1] = true;
							}
						}
					}
				}
				if (iAttempts > 15)
				{
					Logger.DBLog.InfoFormat("***************************");
					Logger.DBLog.InfoFormat("GSError: Emergency Stop: No matter what we tried, we couldn't prevent an infinite stash loop. Sorry. Now stopping the bot.");
					BotMain.Stop();
					return false;
				}
			}
			else
			{
				_dictItemStashAttempted.Add(iOriginalDynamicID, 1);
			}
			// Safety incase it's not actually in the backpack anymore
			/*if (item.InventorySlot != InventorySlot.PlayerBackpack)
			{
				 Logger.DBLog.InfoFormat("GSError: Diablo 3 memory read error, or item became invalid [StashAttempt-4]", true);
				 return false;
			}*/
			int iLeftoverStackQuantity;
			// Item log for cool stuff stashed

			if (thisGilesBaseType == GilesBaseItemType.WeaponTwoHand || thisGilesBaseType == GilesBaseItemType.WeaponOneHand || thisGilesBaseType == GilesBaseItemType.WeaponRange ||
				 thisGilesBaseType == GilesBaseItemType.Armor || thisGilesBaseType == GilesBaseItemType.Jewelry || thisGilesBaseType == GilesBaseItemType.Offhand ||
				 thisGilesBaseType == GilesBaseItemType.FollowerItem)
			{

				Logger.LogGoodItems(item, thisGilesBaseType, OriginalGilesItemType);
			}

			int iPointX = -1;
			int iPointY = -1;
			// First check if we can top-up any already-existing stacks in the stash
			if (bOriginalIsStackable)
			{
				foreach (ACDItem tempitem in ZetaDia.Me.Inventory.StashItems)
				{
					if (tempitem.BaseAddress == IntPtr.Zero)
					{
						Logger.DBLog.InfoFormat("GSError: Diablo 3 memory read error, or stash item became invalid [StashAttempt-5]");
						return false;
					}
					// Check if we combine the stacks, we won't overfill them
					if ((tempitem.GameBalanceId == iOriginalGameBalanceId) && (tempitem.ItemStackQuantity < tempitem.MaxStackCount))
					{
						iLeftoverStackQuantity = (tempitem.ItemStackQuantity + iOriginalStackQuantity) - tempitem.MaxStackCount;
						iPointX = tempitem.InventoryColumn;
						iPointY = tempitem.InventoryRow;

						// Will we have leftovers?
						if (iLeftoverStackQuantity <= 0)
							goto FoundStashLocation;
						goto HandleStackMovement;
					}
				}
			HandleStackMovement:
				if ((iPointX >= 0) && (iPointY >= 0))
				{
					ZetaDia.Me.Inventory.MoveItem(iOriginalDynamicID, iPlayerDynamicID, InventorySlot.SharedStash, iPointX, iPointY);
				}
			}
			iPointX = -1;
			iPointY = -1;
			// If it's a 2-square item, find a double-slot free
			if (bOriginalTwoSlot)
			{
				for (int iRow = 0; iRow <= 29; iRow++)
				{
					bool bBottomPageRow = (iRow == 9 || iRow == 19 || iRow == 29);
					for (int iColumn = 0; iColumn <= 6; iColumn++)
					{
						// If nothing in the 1st row 
						if (!GilesStashSlotBlocked[iColumn, iRow])
						{
							bool bNotEnoughSpace = false;
							// Bottom row of a page = no room
							if (bBottomPageRow)
								bNotEnoughSpace = true;
							// Already something in the stash in the 2nd row)
							else if (GilesStashSlotBlocked[iColumn, iRow + 1])
								bNotEnoughSpace = true;
							if (!bNotEnoughSpace)
							{
								iPointX = iColumn;
								iPointY = iRow;
								goto FoundStashLocation;
							}
						}
					}
				}
			} // 2 slot item?
			// Now deal with any leftover 1-slot items
			else
			{
				// First we try and find somewhere "sensible"
				for (int iRow = 0; iRow <= 29; iRow++)
				{
					bool bTopPageRow = (iRow == 0 || iRow == 10 || iRow == 20);
					bool bBottomPageRow = (iRow == 9 || iRow == 19 || iRow == 29);
					for (int iColumn = 0; iColumn <= 6; iColumn++)
					{
						// Nothing in this slot
						if (!GilesStashSlotBlocked[iColumn, iRow])
						{
							bool bSensibleLocation = false;
							if (!bTopPageRow && !bBottomPageRow)
							{
								// Something above and below this slot, or an odd-numbered row, so put something here
								if ((GilesStashSlotBlocked[iColumn, iRow + 1] && GilesStashSlotBlocked[iColumn, iRow - 1]) ||
									 (iRow) % 2 != 0)
									bSensibleLocation = true;
							}
							// Top page row with something directly underneath already blocking
							else if (bTopPageRow)
							{
								if (GilesStashSlotBlocked[iColumn, iRow + 1])
									bSensibleLocation = true;
							}
							// Bottom page row with something directly over already blocking
							else
							{
								bSensibleLocation = true;
							}
							// Sensible location? Yay, stash it here!
							if (bSensibleLocation)
							{
								iPointX = iColumn;
								iPointY = iRow;
								// Keep looking for places if it's a stackable to try to stick it at the end
								if (!bOriginalIsStackable)
									goto FoundStashLocation;
							}
						}
					}
				}
				// Didn't find a "sensible" place, let's try and force it in absolutely anywhere
				if ((iPointX < 0) || (iPointY < 0))
				{
					for (int iRow = 0; iRow <= 29; iRow++)
					{
						for (int iColumn = 0; iColumn <= 6; iColumn++)
						{
							// Nothing in this spot, we're good!
							if (!GilesStashSlotBlocked[iColumn, iRow])
							{
								iPointX = iColumn;
								iPointY = iRow;
								// Keep looking for places if it's a stackable to try to stick it at the end
								if (!bOriginalIsStackable)
									goto FoundStashLocation;
							}
						}
					}
				}
			}
		FoundStashLocation:
			if ((iPointX < 0) || (iPointY < 0))
			{
				Logger.DBLog.DebugFormat("Fatal Error: No valid stash location found for '" + sOriginalItemName + "' [" + sOriginalInternalName + " - " + OriginalGilesItemType.ToString() + "]");
				Logger.DBLog.InfoFormat("***************************");
				Logger.DBLog.InfoFormat("GSError: Emergency Stop: You need to stash an item but no valid space could be found. Stash is full? Stopping the bot to prevent infinite town-run loop.");
				OutOfGame.MuleBehavior = true;
				ZetaDia.Service.Party.LeaveGame();
				return false;
			}
			// We have two valid points that are empty, move the object here!
			GilesStashSlotBlocked[iPointX, iPointY] = true;
			if (bOriginalTwoSlot)
				GilesStashSlotBlocked[iPointX, iPointY + 1] = true;

			XY = new[] { iPointX, iPointY };

			if (iPointY < 10)
				StashPage = 0;
			else if (iPointY < 20)
				StashPage = 1;
			else
				StashPage = 2;

			return true;
		} // Custom stashing routine

		// **********************************************************************************************
		// *****  Safety pauses to make sure we aren't still coming through the portal or selling   *****
		// **********************************************************************************************

		internal static bool GilesPreStashPauseOverlord(object ret)
		{
			return (!bPreStashPauseDone);
		}
		internal static RunStatus GilesStashPrePause(object ret)
		{
			bPreStashPauseDone = true;
			iPreStashLoops = 0;
			return RunStatus.Success;
		}
		internal static RunStatus GilesStashPause(object ret)
		{
			iPreStashLoops++;
			if (iPreStashLoops < 20)
				return RunStatus.Running;
			return RunStatus.Success;
		}

		// **********************************************************************************************
		// *****                Randomize the timer between stashing/salvaging etc.                 *****
		// **********************************************************************************************
		private static void RandomizeTheTimer()
		{
			Random rndNum = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), NumberStyles.HexNumber));
			int rnd = rndNum.Next(7);
			iItemDelayLoopLimit = 4 + rnd;
		}
	}

}