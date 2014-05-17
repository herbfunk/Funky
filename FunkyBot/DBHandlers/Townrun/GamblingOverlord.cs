using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using FunkyBot.Game;
using FunkyBot.Movement;
using FunkyBot.Player;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;


namespace FunkyBot.DBHandlers
{

	internal static partial class TownRunManager
	{
		internal static BloodShardGambleItems nextItemType=BloodShardGambleItems.None;
		internal static bool PurchasedItem = false;
		private static int LastBloodShardCount = 0;
		private static List<CacheACDItem> GamblingItemList = new List<CacheACDItem>(); 


		internal static bool GamblingRunOverlord(object ret)
		{//Should we gamble?

			if (Bot.Settings.TownRun.EnableBloodShardGambling && Bot.Game.AdventureMode && 
				!Bot.Settings.TownRun.BloodShardGambleItems.Equals(BloodShardGambleItems.None) &&
				ValidGambleItems.Any(i => Bot.Settings.TownRun.BloodShardGambleItems.HasFlag(i)))
			{
				Bot.Character.Data.BackPack.Update();

				//If we still have any items (not potions or protected) then we don't start behavior yet.
				if (Bot.Character.Data.BackPack.CacheItemList.Values.Where(thisitem => !ItemManager.Current.ItemIsProtected(thisitem.ACDItem)).Any(thisitem => thisitem.ThisDBItemType != ItemType.Potion))
				{
					return false;
				}


				int currentBloodShardCount=ZetaDia.CPlayer.BloodshardCount;
				return currentBloodShardCount >= Bot.Settings.TownRun.MinimumBloodShards;
			}

			return false;
		}

		internal static RunStatus GamblingMovement(object ret)
		{
			if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || ZetaDia.Me.CommonData == null)
			{
				Logger.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			DiaUnit objGamblingNPC = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).FirstOrDefault<DiaUnit>(u => u.Name.StartsWith("X1_RandomItemNPC"));
			Vector3 vectorPlayerPosition = ZetaDia.Me.Position;
			Vector3 vectorGamblingPosition = new Vector3(0f, 0f, 0f);

			if (objGamblingNPC == null || objGamblingNPC.Distance > 50f)
			{
				Act curAct = ZetaDia.CurrentAct;
				if (curAct == Act.Invalid || curAct == Act.OpenWorld || curAct == Act.Test)
					curAct = Character.FindActByLevelID(Bot.Character.Data.iCurrentLevelID);
				vectorGamblingPosition = TownRunManager.ReturnMovementVector(TownRunBehavior.Gamble, curAct);
			}
			else
				vectorGamblingPosition = objGamblingNPC.Position;


			Bot.NavigationCache.RefreshMovementCache();
			//Wait until we are not moving
			if (Bot.NavigationCache.IsMoving) return RunStatus.Running;


			float fDistance = Vector3.Distance(vectorPlayerPosition, vectorGamblingPosition);
			//Out-Of-Range...
			if (objGamblingNPC == null || fDistance > 12f)//|| !GilesCanRayCast(vectorPlayerPosition, vectorSalvageLocation, Zeta.Internals.SNO.NavCellFlags.AllowWalk))
			{
				//Use our click movement
				Bot.NavigationCache.RefreshMovementCache();

				//Wait until we are not moving to send click again..
				if (Bot.NavigationCache.IsMoving)
					return RunStatus.Running;

				Navigator.PlayerMover.MoveTowards(vectorGamblingPosition);
				return RunStatus.Running;
			}

			if (!UI.ValidateUIElement(UI.BloodShardVendor.BloodShardVendorMainDialog))
			{
				objGamblingNPC.Interact();
				return RunStatus.Running;
			}

			//Now we need to update our item list so we can get the DynamicIDs.
			UpdateGambleItemList();
			

			return RunStatus.Success;
		}

		internal static RunStatus GamblingInteraction(object ret)
		{
			if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || ZetaDia.Me.CommonData == null)
			{
				Logger.DBLog.DebugFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}
			if (!UI.ValidateUIElement(UI.BloodShardVendor.BloodShardVendorMainDialog))
			{
				Logger.DBLog.DebugFormat("[Funky] Gambling UI Dialog Not Visible!");
				return RunStatus.Failure;
			}
			if (!UIElements.InventoryWindow.IsVisible)
			{
				Bot.Character.Data.BackPack.InventoryBackPackToggle(true);
				return RunStatus.Running;
			}
			


			int CurrentBloodShardCount = ZetaDia.CPlayer.BloodshardCount;
			if (CurrentBloodShardCount<5)
			{
				Logger.DBLog.DebugFormat("[Funky] Gambling Behavior Finished.");
				return RunStatus.Success;
			}

			//set default
			if (LastBloodShardCount == 0 && LastBloodShardCount != CurrentBloodShardCount)
			{
				LastBloodShardCount = CurrentBloodShardCount;
			}

			//Check if we should find a new item type to gamble for..
			if (nextItemType.Equals(BloodShardGambleItems.None) || GetGambleItemPrice(nextItemType) > CurrentBloodShardCount)
			{
				nextItemType = BloodShardGambleItems.None;

				List<BloodShardGambleItems> freshList = ValidGambleItems.Where(t => Bot.Settings.TownRun.BloodShardGambleItems.HasFlag(t)).ToList();

				while (freshList.Count>0)
				{
					Random r = new Random();

					int curListCount=freshList.Count;
					int index = r.Next(0, curListCount-1);

					BloodShardGambleItems itemtype = freshList[index];
					if (Bot.Settings.TownRun.BloodShardGambleItems.HasFlag(itemtype) && GetGambleItemPrice(itemtype) <= CurrentBloodShardCount)
					{
						nextItemType = itemtype;
						Logger.DBLog.DebugFormat("Next Item Type: {0}", nextItemType);
						break;
					}
					else
					{
						freshList.RemoveAt(index);
					}
				}
			}
			
			if (nextItemType.Equals(BloodShardGambleItems.None))
			{
				Logger.DBLog.DebugFormat("[Funky] Gambling Behavior Finished -- No Valid Item Types Returned.");
				return RunStatus.Success;
			}


			//Get Current Tab We Should Have Visible..
			UIElement UITab = GetGambleItemTabUIElement(nextItemType);
			if (!UI.ValidateUIElement(UITab))
			{
				//Error!!?!
				Logger.DBLog.DebugFormat("[Funky] Gambling Behavior UI Tab Not Valid!");
				return RunStatus.Success;
			}

			if (!PurchasedItem)
			{
				//Waiting..
				if (!TownRunItemLoopsTest(2)) return RunStatus.Running;

				//Retrieve the DynamicID based on our type selection
				int dynamicID = GetGambleItemDynamicID(nextItemType);
				if (dynamicID == -1)
				{
					//Error!?
					Logger.DBLog.DebugFormat("[Funky] Gambling Behavior Item DynamicID was not found!");
					return RunStatus.Success;
				}

				Logger.DBLog.DebugFormat("[Funky] Buying Item {0}", nextItemType);
				ZetaDia.Actors.Me.Inventory.BuyItem(dynamicID);
				PurchasedItem = true;
				return RunStatus.Running;
			}




			if (LastBloodShardCount != CurrentBloodShardCount)
			{
				LastBloodShardCount=CurrentBloodShardCount;
				Bot.Game.CurrentGameStats.CurrentProfile.ItemsGambled++;
			}
			else if (ZetaDia.Me.Inventory.NumFreeBackpackSlots < 3)
			{
				Logger.DBLog.DebugFormat("[Funky] Gambling Behavior Finished -- Backpack Is Nearly Full!");
				return RunStatus.Success;
			}
			

			//Reset
			PurchasedItem = false;
			nextItemType = BloodShardGambleItems.None;

			return RunStatus.Running;
		}

		internal static RunStatus GamblingStart(object ret)
		{
			if (Bot.Settings.Debug.DebugStatusBar)
				BotMain.StatusText = "Town run: Gambling routine started";
			Logger.DBLog.DebugFormat("Debug: Gambling routine started.");

			if (ZetaDia.Actors.Me == null)
			{
				Logger.DBLog.DebugFormat("Error: Diablo 3 memory read error, or item became invalid [PreSalvage-1]");
				return RunStatus.Failure;
			}


			PurchasedItem = false;
			LastBloodShardCount = 0;
			nextItemType = BloodShardGambleItems.None;
			iCurrentItemLoops = 0;
			RandomizeTheTimer();
			return RunStatus.Success;
		}

		internal static RunStatus GamblingFinish(object ret)
		{
			Logger.DBLog.DebugFormat("Debug: Gambling routine ending sequence...");
			LastBloodShardCount = 0;
			PurchasedItem = false;
			nextItemType = BloodShardGambleItems.None;
			return RunStatus.Success;
		}



		internal static int GetGambleItemPrice(BloodShardGambleItems item)
		{
			switch (item)
			{
				case BloodShardGambleItems.OneHandItem:
				case BloodShardGambleItems.TwoHandItem:
					return 15;
				case BloodShardGambleItems.Quiver:
				case BloodShardGambleItems.Orb:
				case BloodShardGambleItems.Mojo:
				case BloodShardGambleItems.Helm:
				case BloodShardGambleItems.Gloves:
				case BloodShardGambleItems.Boots:
				case BloodShardGambleItems.Chest:
				case BloodShardGambleItems.Belt:
				case BloodShardGambleItems.Shoulders:
				case BloodShardGambleItems.Pants:
				case BloodShardGambleItems.Bracers:
				case BloodShardGambleItems.Shield:
					return 5;
				case BloodShardGambleItems.Ring:
					return 10;
				case BloodShardGambleItems.Amulet:
					return 20;
			}

			return -1;
		}

		internal static UIElement GetGambleItemUIElement(BloodShardGambleItems item)
		{
			switch (item)
			{
				case BloodShardGambleItems.OneHandItem:
					return UI.BloodShardVendor.BloodShardVendorOneHandItem;
				case BloodShardGambleItems.TwoHandItem:
					return UI.BloodShardVendor.BloodShardVendorTwoHandItem;
				case BloodShardGambleItems.Quiver:
					return UI.BloodShardVendor.BloodShardVendorQuiver;
				case BloodShardGambleItems.Orb:
					return UI.BloodShardVendor.BloodShardVendorOrb;
				case BloodShardGambleItems.Mojo:
					return UI.BloodShardVendor.BloodShardVendorMojo;
				case BloodShardGambleItems.Helm:
					return UI.BloodShardVendor.BloodShardVendorHelm;
				case BloodShardGambleItems.Gloves:
					return UI.BloodShardVendor.BloodShardVendorGloves;
				case BloodShardGambleItems.Boots:
					return UI.BloodShardVendor.BloodShardVendorBoots;
				case BloodShardGambleItems.Chest:
					return UI.BloodShardVendor.BloodShardVendorChestArmor;
				case BloodShardGambleItems.Belt:
					return UI.BloodShardVendor.BloodShardVendorBelt;
				case BloodShardGambleItems.Shoulders:
					return UI.BloodShardVendor.BloodShardVendorShoulders;
				case BloodShardGambleItems.Pants:
					return UI.BloodShardVendor.BloodShardVendorPants;
				case BloodShardGambleItems.Bracers:
					return UI.BloodShardVendor.BloodShardVendorBracers;
				case BloodShardGambleItems.Shield:
					return UI.BloodShardVendor.BloodShardVendorShield;
				case BloodShardGambleItems.Ring:
					return UI.BloodShardVendor.BloodShardVendorRing;
				case BloodShardGambleItems.Amulet:
					return UI.BloodShardVendor.BloodShardVendorAmulet;
			}

			return null;
		}

		internal static UIElement GetGambleItemTabUIElement(BloodShardGambleItems item)
		{
			switch (item)
			{
				case BloodShardGambleItems.OneHandItem:
				case BloodShardGambleItems.TwoHandItem:
					return UI.BloodShardVendor.BloodShardVendorWeaponTab;

				case BloodShardGambleItems.Quiver:
				case BloodShardGambleItems.Orb:
				case BloodShardGambleItems.Mojo:
				case BloodShardGambleItems.Helm:
				case BloodShardGambleItems.Gloves:
				case BloodShardGambleItems.Boots:
				case BloodShardGambleItems.Chest:
				case BloodShardGambleItems.Belt:
				case BloodShardGambleItems.Shoulders:
				case BloodShardGambleItems.Pants:
				case BloodShardGambleItems.Bracers:
				case BloodShardGambleItems.Shield:
					return UI.BloodShardVendor.BloodShardVendorArmorTab;

				case BloodShardGambleItems.Ring:
				case BloodShardGambleItems.Amulet:
					return UI.BloodShardVendor.BloodShardVendorTrinketsTab;
			}

			return null;
		}

		internal static void UpdateGambleItemList()
		{
			GamblingItemList.Clear();

			//Clear and Update
			ZetaDia.Memory.ClearCache();
			ZetaDia.Actors.Update();

			foreach (var i in ZetaDia.Actors.ACDList)
			{
				try
				{
					ACDItem item;
					item = (ACDItem)i;
					int balanceID = item.GameBalanceId;

					if(GamblingItemBalanceIDs.Contains(balanceID))  //(item.Name.Contains("Mystery"))
					{
						string itemString = String.Format("Item Name {0} DynamicID {1}", item.Name, item.DynamicId);
						Logger.DBLog.Info(itemString);
						GamblingItemList.Add(new CacheACDItem(item));
						//LBDebug.Items.Add(itemString);
					}
				}
				catch (Exception)
				{

				}
			}
			Logger.DBLog.InfoFormat("Gambling items found {0}", GamblingItemList.Count);
		}

		internal static int GetGambleItemDynamicID(BloodShardGambleItems type)
		{
			//string ItemName = ReturnGamblingItemName(type);
			int ItemBalanceID = GetGambleItemItemDynamicIDBalanceID(type);
			//Search the gambling item list for the correct name
			foreach (var acdItem in GamblingItemList )
			{
				if (acdItem.ThisBalanceID==ItemBalanceID) //(acdItem.ThisRealName.Contains(ItemName))
					return acdItem.ThisDynamicID;
			}

			return -1;
		}

		private static string ReturnGamblingItemName(BloodShardGambleItems type)
		{
			if (type == BloodShardGambleItems.OneHandItem) 
			{
				return "1-H Mystery Weapon";
			}
			if (type == BloodShardGambleItems.TwoHandItem)
			{
				return "2-H Mystery Weapon";
			}
			if (type == BloodShardGambleItems.Chest)
			{
				return "Mystery Chest Armor";
			}

			return String.Format("Mystery {0}", type);
		}
		private static int GetGambleItemItemDynamicIDBalanceID(BloodShardGambleItems type)
		{
			switch (type)
			{
				case BloodShardGambleItems.OneHandItem:
					return -767866790;
				case BloodShardGambleItems.TwoHandItem:
					return -1099096773;
				case BloodShardGambleItems.Quiver:
					return -1843121997;
				case BloodShardGambleItems.Orb:
					return 215071258;
				case BloodShardGambleItems.Mojo:
					return -1492657844;
				case BloodShardGambleItems.Helm:
					return -1492848355;
				case BloodShardGambleItems.Gloves:
					return 2050033703;
				case BloodShardGambleItems.Boots:
					return -2026108002;
				case BloodShardGambleItems.Chest:
					return -594428401;
				case BloodShardGambleItems.Belt:
					return -1493063970;
				case BloodShardGambleItems.Shoulders:
					return -537237168;
				case BloodShardGambleItems.Pants:
					return -2010009315;
				case BloodShardGambleItems.Bracers:
					return 1281756953;
				case BloodShardGambleItems.Shield:
					return -1780286480;
				case BloodShardGambleItems.Ring:
					return -1492484569;
				case BloodShardGambleItems.Amulet:
					return 1816611999;
			}

			return -1;
		}
		private static readonly HashSet<int> GamblingItemBalanceIDs = new HashSet<int> 
		{
			-1780286480,-1099096773,-1492484569,215071258,1492657844,-1843121997,-2026108002,
			1816611999,-1493063970,1281756953,2050033703,-537237168,-1492848355,-2010009315,
			-767866790,-594428401
		};

		/*
		 *  Item Name Mystery Shield BalanceID -1780286480
			Item Name 2-H Mystery Weapon BalanceID -1099096773
			Item Name Mystery Ring BalanceID -1492484569
			Item Name Mystery Orb BalanceID 215071258
			Item Name Mystery Mojo BalanceID -1492657844
			Item Name Mystery Quiver BalanceID -1843121997
			Item Name Mystery Boots BalanceID -2026108002
			Item Name Mystery Amulet BalanceID 1816611999
			Item Name Mystery Belt BalanceID -1493063970
			Item Name Mystery Bracers BalanceID 1281756953
			Item Name Mystery Gloves BalanceID 2050033703
			Item Name Mystery Shoulders BalanceID -537237168
			Item Name Mystery Helm BalanceID -1492848355
			Item Name Mystery Pants BalanceID -2010009315
			Item Name 1-H Mystery Weapon BalanceID -767866790
			Item Name Mystery Chest Armor BalanceID -594428401
		 */

		//TODO:: Tab Switching Not Occuring For Bloodshard Vendor (Only Weapons Are Valid Options!)
		internal static readonly List<BloodShardGambleItems> ValidGambleItems = new List<BloodShardGambleItems>
		{
			BloodShardGambleItems.OneHandItem,BloodShardGambleItems.TwoHandItem,
			BloodShardGambleItems.Mojo,BloodShardGambleItems.Orb,BloodShardGambleItems.Quiver,

			BloodShardGambleItems.Belt,BloodShardGambleItems.Boots,
			BloodShardGambleItems.Bracers,BloodShardGambleItems.Chest,BloodShardGambleItems.Gloves,
			BloodShardGambleItems.Helm,
			BloodShardGambleItems.Pants,
			BloodShardGambleItems.Shield,BloodShardGambleItems.Shoulders,

			BloodShardGambleItems.Amulet,BloodShardGambleItems.Ring,
		};
	}

}
