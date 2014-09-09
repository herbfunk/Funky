using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;

namespace fBaseXtensions.XML
{
	[XmlElement("MoveItem")]
	public class MoveItemTag : ProfileBehavior
	{
		[XmlAttribute("Sno")]
		[XmlAttribute("SNO")]
		[XmlAttribute("sno")]
		public int Sno
		{
			get { return _sno; }
			set { _sno = value; }
		}
		private int _sno = -1;

		public enum KeystoneType
		{
			None,
			Fragment,
			Trial,
			Tiered
		}
		[XmlAttribute("Keystone")]
		public KeystoneType KeyType { get; set; }

		[XmlAttribute("All")]
		[XmlAttribute("all")]
		public bool All
		{
			get { return _all; }
			set { _all = value; }
		}
		private bool _all = false;

		public enum ItemSource
		{
			Backpack,
			Stash
		}

		[XmlAttribute("Itemsource")]
		[XmlAttribute("itemsource")]
		[XmlAttribute("ItemSource")]
		public ItemSource Itemsource
		{
			get { return _itemsource; }
			set { _itemsource = value; }
		}
		private ItemSource _itemsource = ItemSource.Backpack;

		private bool intWait = false;
		protected override Composite CreateBehavior()
		{
			return new PrioritySelector
			(
				new Decorator(ret => FunkyGame.GameIsInvalid,
					new Action(ret => m_IsDone = true)),

				new Decorator(ret => !intWait,
					new Sequence
					(
						new Sleep(2000),
						new Action(ret => intWait=true)
					)
				),

				new Decorator(ret => !UIElements.StashWindow.IsVisible,
					new Action(ret => m_IsDone=true)),

				new Decorator(ret => !UIElements.InventoryWindow.IsVisible,
					new Action(ret => UIManager.ToggleInventoryMenu())),

				//Update Item List
				new Decorator(ret => !updatedItemList,
					new Action(ret => UpdateMovingItemList())),

				new Decorator(ret => Itemsource==ItemSource.Stash && !bUpdatedStashMap,
					new Action(ret => UpdateStashSlots())),

				new Decorator(ret => Itemsource==ItemSource.Backpack && !bUpdatedStashMap,
					new Action(ret => UpdateBackpackSlots())),

				new Decorator(ret => MovingItemList.Count==0,
					new Sequence
					(
						new Sleep(2000),
						new Action(ret => m_IsDone = true)
					)
				),

				new Decorator(ret => MovingItemList.Count>0,
					new Action(ret => MoveItems()))

			);
		}

		private List<CacheACDItem> MovingItemList = new List<CacheACDItem>();
		private bool updatedItemList = false;
		private void UpdateMovingItemList()
		{
			Logger.DBLog.DebugFormat("Updating Moving Items!");

			List<ACDItem> Items = 
				Itemsource == ItemSource.Stash ? ZetaDia.Me.Inventory.StashItems.ToList() : 
				ZetaDia.Me.Inventory.Backpack.ToList();
			

			if (KeyType != KeystoneType.None)
				Items = Items.OrderBy(i => i.TieredLootRunKeyLevel).ThenByDescending(i => i.ItemStackQuantity).ToList();

			foreach (ACDItem tempitem in Items)
			{
				if (tempitem.BaseAddress != IntPtr.Zero)
				{
					if (KeyType != KeystoneType.None)
					{
						int tieredLevel = tempitem.TieredLootRunKeyLevel;
						if (KeyType == KeystoneType.Fragment)
						{
							if (tieredLevel == -1)
							{
								MovingItemList.Add(new CacheACDItem(tempitem));
								if (!All) break;
							}

							continue;
						}

						if (KeyType == KeystoneType.Trial)
						{
							if (tieredLevel == 0)
							{
								MovingItemList.Add(new CacheACDItem(tempitem));
								if (!All) break;
							}
							continue;
						}

						if (KeyType == KeystoneType.Tiered)
						{
							if (tieredLevel > 0)
							{
								MovingItemList.Add(new CacheACDItem(tempitem));
								if (!All) break;
							}
						}
					}
					else if (tempitem.ActorSNO == Sno)
					{
						MovingItemList.Add(new CacheACDItem(tempitem));
						if (!All) break;
					}
				}
			}

			updatedItemList = true;
			Logger.DBLog.InfoFormat("Found a total of {0} items to be moved!", MovingItemList.Count);
		}

		private readonly bool[,] StashSlotBlocked = new bool[7, 50];

		private int[] LastStashPoint = { -1, -1 };
		private int LastStashPage = -1;
		private bool bUpdatedStashMap = false;
		private void UpdateStashSlots()
		{
			Logger.DBLog.DebugFormat("Updating Stash Slots!");

			// Array for what blocks are or are not blocked
			for (int iRow = 0; iRow <= 49; iRow++)
				for (int iColumn = 0; iColumn <= 6; iColumn++)
					StashSlotBlocked[iColumn, iRow] = false;
			// Block off the entire of any "protected stash pages"
			foreach (int iProtPage in CharacterSettings.Instance.ProtectedStashPages)
				for (int iProtRow = 0; iProtRow <= 9; iProtRow++)
					for (int iProtColumn = 0; iProtColumn <= 6; iProtColumn++)
						StashSlotBlocked[iProtColumn, iProtRow + (iProtPage * 10)] = true;
			// Remove rows we don't have
			for (int iRow = (ZetaDia.Me.NumSharedStashSlots / 7); iRow <= 49; iRow++)
				for (int iColumn = 0; iColumn <= 6; iColumn++)
					StashSlotBlocked[iColumn, iRow] = true;


			// Map out all the items already in the stash
			foreach (ACDItem tempitem in ZetaDia.Me.Inventory.StashItems)
			{
				if (tempitem.BaseAddress != IntPtr.Zero)
				{
					//StashedItems.Add(new CacheACDItem(tempitem));
					int inventoryRow = tempitem.InventoryRow;
					int inventoryColumn = tempitem.InventoryColumn;
					// Mark this slot as not-free
					StashSlotBlocked[inventoryColumn, inventoryRow] = true;
					// Try and reliably find out if this is a two slot item or not
					PluginItemTypes tempItemType = ItemFunc.DetermineItemType(tempitem.InternalName, tempitem.ItemType, tempitem.FollowerSpecialType, tempitem.ActorSNO);

					if (ItemFunc.DetermineIsTwoSlot(tempItemType) && inventoryRow != 19 && inventoryRow != 9 && inventoryRow != 29 && inventoryRow != 39 && inventoryRow != 49)
					{
						StashSlotBlocked[inventoryColumn, inventoryRow + 1] = true;
					}
					else if (ItemFunc.DetermineIsTwoSlot(tempItemType) && (inventoryRow == 19 || inventoryRow == 9 || inventoryRow == 29 || inventoryRow == 39 || inventoryRow == 49))
					{
						Logger.DBLog.DebugFormat("GSError: DemonBuddy thinks this item is 2 slot even though it's at bottom row of a stash page: " + tempitem.Name + " [" + tempitem.InternalName +
							  "] type=" + tempItemType.ToString() + " @ slot " + (inventoryRow + 1).ToString(CultureInfo.InvariantCulture) + "/" +
							  (inventoryColumn + 1).ToString(CultureInfo.InvariantCulture));
					}
				}
			} // Loop through all stash items

			bUpdatedStashMap = true;
		}

		private readonly bool[,] BackpackSlotBlocked = new bool[10, 6];
		private void UpdateBackpackSlots()
		{
			Logger.DBLog.DebugFormat("Updating Backpack Slots!");

			// Array for what blocks are or are not blocked
			for (int iRow = 0; iRow <= 5; iRow++)
				for (int iColumn = 0; iColumn <= 9; iColumn++)
					BackpackSlotBlocked[iColumn, iRow] = false;
			// Block off the entire of any "protected"
			foreach (InventorySquare iProtPage in CharacterSettings.Instance.ProtectedBagSlots)
				StashSlotBlocked[iProtPage.Column, iProtPage.Row] = true;



			// Map out all the items already in the stash
			foreach (ACDItem tempitem in ZetaDia.Me.Inventory.Backpack)
			{
				if (tempitem.BaseAddress != IntPtr.Zero)
				{
					//StashedItems.Add(new CacheACDItem(tempitem));
					int inventoryRow = tempitem.InventoryRow;
					int inventoryColumn = tempitem.InventoryColumn;
					// Mark this slot as not-free
					BackpackSlotBlocked[inventoryColumn, inventoryRow] = true;
					// Try and reliably find out if this is a two slot item or not
					PluginItemTypes tempItemType = ItemFunc.DetermineItemType(tempitem.InternalName, tempitem.ItemType, tempitem.FollowerSpecialType, tempitem.ActorSNO);

					if (ItemFunc.DetermineIsTwoSlot(tempItemType) && inventoryRow != 5)
					{
						BackpackSlotBlocked[inventoryColumn, inventoryRow + 1] = true;
					}
					else if (ItemFunc.DetermineIsTwoSlot(tempItemType) && inventoryRow == 5)
					{
						Logger.DBLog.DebugFormat("GSError: DemonBuddy thinks this item is 2 slot even though it's at bottom row of a stash page: " + tempitem.Name + " [" + tempitem.InternalName +
							  "] type=" + tempItemType.ToString() + " @ slot " + (inventoryRow + 1).ToString(CultureInfo.InvariantCulture) + "/" +
							  (inventoryColumn + 1).ToString(CultureInfo.InvariantCulture));
					}
				}
			} // Loop through all stash items

			bUpdatedStashMap = true;
		}

		private Delayer Delay = new Delayer();
		private bool MoveItems()
		{
			if (MovingItemList.Count > 0)
			{
				Logger.DBLog.DebugFormat("Moving Items!");

				if (!Delay.Test()) return true;

				CacheACDItem thisitem = MovingItemList.FirstOrDefault();

				if (Itemsource == ItemSource.Backpack)
				{
					if (LastStashPoint[0] < 0 && LastStashPoint[1] < 0 && LastStashPage < 0)
					{
						bool bDidStashSucceed = StashAttempt(thisitem, out LastStashPoint, out LastStashPage);
						if (!bDidStashSucceed)
						{
							Logger.DBLog.DebugFormat("There was an unknown error stashing an item.");
							//if (OutOfGame.MuleBehavior) return RunStatus.Success;
						}
						else
							return true;
					}
					else
					{
						//We have a valid place to stash.. so lets check if stash page is currently open
						if (ZetaDia.Me.Inventory.CurrentStashPage == LastStashPage)
						{
							//FunkyTownRunPlugin.TownRunStats.StashedItemLog(thisitem);
							if (FunkyGame.CurrentGameStats != null)
								FunkyGame.CurrentGameStats.CurrentProfile.LootTracker.StashedItemLog(thisitem);
							ZetaDia.Me.Inventory.MoveItem(thisitem.ThisDynamicID, ZetaDia.Me.CommonData.DynamicId, InventorySlot.SharedStash, LastStashPoint[0], LastStashPoint[1]);
							LastStashPoint = new[] { -1, -1 };
							LastStashPage = -1;

							MovingItemList.Remove(thisitem);
							if (MovingItemList.Count > 0)
								return true;
						}
						else
						{
							//Lets switch the current page..
							ZetaDia.Me.Inventory.SwitchStashPage(LastStashPage);
							return true;
						}
					}
				}
				else
				{
					if (LastStashPoint[0] < 0 && LastStashPoint[1] < 0)
					{
						bool bDidStashSucceed = BackpackStashAttempt(thisitem, out LastStashPoint);
						if (!bDidStashSucceed)
						{
							Logger.DBLog.DebugFormat("There was an unknown error stashing an item.");
						}
						else
							return true;
					}
					else
					{
						if (FunkyGame.CurrentGameStats != null)
							FunkyGame.CurrentGameStats.CurrentProfile.LootTracker.StashedItemLog(thisitem);
						ZetaDia.Me.Inventory.MoveItem(thisitem.ThisDynamicID, ZetaDia.Me.CommonData.DynamicId, InventorySlot.BackpackItems, LastStashPoint[0], LastStashPoint[1]);
						LastStashPoint = new[] { -1, -1 };
						
						MovingItemList.Remove(thisitem);
						if (MovingItemList.Count > 0)
							return true;
					}
				}

			}

			m_IsDone = true;
			return false;
		}
		private Dictionary<int, int> _dictItemStashAttempted = new Dictionary<int, int>();
		private bool StashAttempt(CacheACDItem item, out int[] XY, out int StashPage)
		{
			XY = new[] { -1, -1 };
			StashPage = -1;

			int iPlayerDynamicID = ZetaDia.Me.CommonData.DynamicId;
			int iOriginalGameBalanceId = item.ThisBalanceID;
			int iOriginalDynamicID = item.ThisDynamicID;
			int iOriginalStackQuantity = item.ThisItemStackQuantity;
			string sOriginalItemName = item.ThisRealName;
			string sOriginalInternalName = item.ThisInternalName;
			PluginItemTypes OriginalPluginItemType = ItemFunc.DetermineItemType(item);
			PluginBaseItemTypes thisGilesBaseType = ItemFunc.DetermineBaseType(OriginalPluginItemType);
			bool bOriginalTwoSlot = ItemFunc.DetermineIsTwoSlot(OriginalPluginItemType);
			bool bOriginalIsStackable = ItemFunc.DetermineIsStackable(OriginalPluginItemType);
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
					for (int iRow = 0; iRow <= 49; iRow++)
						for (int iColumn = 0; iColumn <= 6; iColumn++)
							StashSlotBlocked[iColumn, iRow] = false;
					// Block off the entire of any "protected stash pages"
					foreach (int iProtPage in CharacterSettings.Instance.ProtectedStashPages)
						for (int iProtRow = 0; iProtRow <= 9; iProtRow++)
							for (int iProtColumn = 0; iProtColumn <= 6; iProtColumn++)
								StashSlotBlocked[iProtColumn, iProtRow + (iProtPage * 10)] = true;
					// Remove rows we don't have
					for (int iRow = (ZetaDia.Me.NumSharedStashSlots / 7); iRow <= 49; iRow++)
						for (int iColumn = 0; iColumn <= 6; iColumn++)
							StashSlotBlocked[iColumn, iRow] = true;
					// Map out all the items already in the stash
					foreach (ACDItem tempitem in ZetaDia.Me.Inventory.StashItems)
					{
						if (tempitem.BaseAddress != IntPtr.Zero)
						{
							int inventoryRow = tempitem.InventoryRow;
							int inventoryColumn = tempitem.InventoryColumn;
							// Mark this slot as not-free
							StashSlotBlocked[inventoryColumn, inventoryRow] = true;
							// Try and reliably find out if this is a two slot item or not
							StashSlotBlocked[inventoryColumn, inventoryRow + 1] = true;
							if (inventoryRow != 19 && inventoryRow != 9 && inventoryRow != 29 && inventoryRow != 39 && inventoryRow != 49)
							{
								StashSlotBlocked[inventoryColumn, inventoryRow + 1] = true;
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
				 FunkyTownRunPlugin.DBLog.InfoFormat("GSError: Diablo 3 memory read error, or item became invalid [StashAttempt-4]", true);
				 return false;
			}*/
			int iLeftoverStackQuantity;


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
				for (int iRow = 0; iRow <= 49; iRow++)
				{
					bool bBottomPageRow = (iRow == 9 || iRow == 19 || iRow == 29 || iRow == 39 || iRow == 49);
					for (int iColumn = 0; iColumn <= 6; iColumn++)
					{
						// If nothing in the 1st row 
						if (!StashSlotBlocked[iColumn, iRow])
						{
							bool bNotEnoughSpace = false;
							// Bottom row of a page = no room
							if (bBottomPageRow)
								bNotEnoughSpace = true;
							// Already something in the stash in the 2nd row)
							else if (StashSlotBlocked[iColumn, iRow + 1])
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
				for (int iRow = 0; iRow <= 49; iRow++)
				{
					bool bTopPageRow = (iRow == 0 || iRow == 10 || iRow == 20 || iRow == 30 || iRow == 40);
					bool bBottomPageRow = (iRow == 9 || iRow == 19 || iRow == 29 || iRow == 39 || iRow == 49);
					for (int iColumn = 0; iColumn <= 6; iColumn++)
					{
						// Nothing in this slot
						if (!StashSlotBlocked[iColumn, iRow])
						{
							bool bSensibleLocation = false;
							if (!bTopPageRow && !bBottomPageRow)
							{
								// Something above and below this slot, or an odd-numbered row, so put something here
								if ((StashSlotBlocked[iColumn, iRow + 1] && StashSlotBlocked[iColumn, iRow - 1]) ||
									 (iRow) % 2 != 0)
									bSensibleLocation = true;
							}
							// Top page row with something directly underneath already blocking
							else if (bTopPageRow)
							{
								if (StashSlotBlocked[iColumn, iRow + 1])
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
					for (int iRow = 0; iRow <= 49; iRow++)
					{
						for (int iColumn = 0; iColumn <= 6; iColumn++)
						{
							// Nothing in this spot, we're good!
							if (!StashSlotBlocked[iColumn, iRow])
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
				Logger.DBLog.DebugFormat("Fatal Error: No valid stash location found for '" + sOriginalItemName + "' [" + sOriginalInternalName + " - " + OriginalPluginItemType.ToString() + "]");
				Logger.DBLog.InfoFormat("***************************");
				Logger.DBLog.InfoFormat("GSError: Emergency Stop: You need to stash an item but no valid space could be found. Stash is full? Stopping the bot to prevent infinite town-run loop.");

				BotMain.Stop(true, "No Room To Stash!");
				ZetaDia.Service.Party.LeaveGame();
				return false;
			}
			// We have two valid points that are empty, move the object here!
			StashSlotBlocked[iPointX, iPointY] = true;
			if (bOriginalTwoSlot)
				StashSlotBlocked[iPointX, iPointY + 1] = true;

			XY = new[] { iPointX, iPointY };

			if (iPointY < 10)
				StashPage = 0;
			else if (iPointY < 20)
				StashPage = 1;
			else if (iPointY < 30)
				StashPage = 2;
			else if (iPointY < 40)
				StashPage = 3;
			else
				StashPage = 4;

			return true;
		} // Custom stashing routine
		private bool BackpackStashAttempt(CacheACDItem item, out int[] XY)
		{
			XY = new[] { -1, -1 };

			int iPlayerDynamicID = ZetaDia.Me.CommonData.DynamicId;
			int iOriginalGameBalanceId = item.ThisBalanceID;
			int iOriginalDynamicID = item.ThisDynamicID;
			int iOriginalStackQuantity = item.ThisItemStackQuantity;
			string sOriginalItemName = item.ThisRealName;
			string sOriginalInternalName = item.ThisInternalName;
			PluginItemTypes OriginalPluginItemType = ItemFunc.DetermineItemType(item);
			PluginBaseItemTypes thisGilesBaseType = ItemFunc.DetermineBaseType(OriginalPluginItemType);
			bool bOriginalTwoSlot = ItemFunc.DetermineIsTwoSlot(OriginalPluginItemType);
			bool bOriginalIsStackable = ItemFunc.DetermineIsStackable(OriginalPluginItemType);
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
					for (int iRow = 0; iRow <= 5; iRow++)
						for (int iColumn = 0; iColumn <= 9; iColumn++)
							BackpackSlotBlocked[iColumn, iRow] = false;
					// Block off the entire of any "protected stash pages"
					foreach (InventorySquare iProtPage in CharacterSettings.Instance.ProtectedBagSlots)
						BackpackSlotBlocked[iProtPage.Column, iProtPage.Row] = true;

					// Map out all the items already in the stash
					foreach (ACDItem tempitem in ZetaDia.Me.Inventory.StashItems)
					{
						if (tempitem.BaseAddress != IntPtr.Zero)
						{
							int inventoryRow = tempitem.InventoryRow;
							int inventoryColumn = tempitem.InventoryColumn;
							// Mark this slot as not-free
							BackpackSlotBlocked[inventoryColumn, inventoryRow] = true;
							// Try and reliably find out if this is a two slot item or not
							BackpackSlotBlocked[inventoryColumn, inventoryRow + 1] = true;
							if (inventoryRow != 19 && inventoryRow != 9 && inventoryRow != 29 && inventoryRow != 39 && inventoryRow != 49)
							{
								BackpackSlotBlocked[inventoryColumn, inventoryRow + 1] = true;
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
				 FunkyTownRunPlugin.DBLog.InfoFormat("GSError: Diablo 3 memory read error, or item became invalid [StashAttempt-4]", true);
				 return false;
			}*/
			int iLeftoverStackQuantity;


			int iPointX = -1;
			int iPointY = -1;
			// First check if we can top-up any already-existing stacks in the stash
			if (bOriginalIsStackable)
			{
				foreach (ACDItem tempitem in ZetaDia.Me.Inventory.Backpack)
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
					ZetaDia.Me.Inventory.MoveItem(iOriginalDynamicID, iPlayerDynamicID, InventorySlot.BackpackItems, iPointX, iPointY);
				}
			}
			iPointX = -1;
			iPointY = -1;
			// If it's a 2-square item, find a double-slot free
			if (bOriginalTwoSlot)
			{
				for (int iRow = 0; iRow <= 49; iRow++)
				{
					bool bBottomPageRow = iRow == 5;
					for (int iColumn = 0; iColumn <= 9; iColumn++)
					{
						// If nothing in the 1st row 
						if (!BackpackSlotBlocked[iColumn, iRow])
						{
							bool bNotEnoughSpace = false;
							// Bottom row of a page = no room
							if (bBottomPageRow)
								bNotEnoughSpace = true;
							// Already something in the stash in the 2nd row)
							else if (BackpackSlotBlocked[iColumn, iRow + 1])
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
				for (int iRow = 0; iRow <= 5; iRow++)
				{
					bool bTopPageRow = iRow == 0;
					bool bBottomPageRow = (iRow == 5);
					for (int iColumn = 0; iColumn <= 9; iColumn++)
					{
						// Nothing in this slot
						if (!BackpackSlotBlocked[iColumn, iRow])
						{
							bool bSensibleLocation = false;
							if (!bTopPageRow && !bBottomPageRow)
							{
								// Something above and below this slot, or an odd-numbered row, so put something here
								if ((BackpackSlotBlocked[iColumn, iRow + 1] && BackpackSlotBlocked[iColumn, iRow - 1]) ||
									 (iRow) % 2 != 0)
									bSensibleLocation = true;
							}
							// Top page row with something directly underneath already blocking
							else if (bTopPageRow)
							{
								if (BackpackSlotBlocked[iColumn, iRow + 1])
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
					for (int iRow = 0; iRow <= 5; iRow++)
					{
						for (int iColumn = 0; iColumn <= 9; iColumn++)
						{
							// Nothing in this spot, we're good!
							if (!BackpackSlotBlocked[iColumn, iRow])
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
				Logger.DBLog.DebugFormat("Fatal Error: No valid stash location found for '" + sOriginalItemName + "' [" + sOriginalInternalName + " - " + OriginalPluginItemType.ToString() + "]");
				Logger.DBLog.InfoFormat("***************************");
				Logger.DBLog.InfoFormat("GSError: Emergency Stop: You need to stash an item but no valid space could be found. Stash is full? Stopping the bot to prevent infinite town-run loop.");

				BotMain.Stop(true, "No Room To Stash!");
				//ZetaDia.Service.Party.LeaveGame();
				return false;
			}
			// We have two valid points that are empty, move the object here!
			BackpackSlotBlocked[iPointX, iPointY] = true;
			if (bOriginalTwoSlot)
				BackpackSlotBlocked[iPointX, iPointY + 1] = true;

			XY = new[] { iPointX, iPointY };



			return true;
		} // Custom stashing routine


		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}
		public override void ResetCachedDone()
		{
			m_IsDone = false;
			base.ResetCachedDone();
		}
	}
}
