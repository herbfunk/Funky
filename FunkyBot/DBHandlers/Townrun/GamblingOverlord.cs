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
		internal static bool ClickedItemUI = false;
		internal static bool ClickedBackpackUI = false;

		internal static bool GamblingRunOverlord(object ret)
		{//Should we gamble?

			if (Bot.Settings.TownRun.EnableBloodShardGambling && Bot.Game.AdventureMode && !Bot.Settings.TownRun.BloodShardGambleItems.Equals(BloodShardGambleItems.None))
			{
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

			return RunStatus.Success;
		}

		internal static RunStatus GamblingInteraction(object ret)
		{
			if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || ZetaDia.Me.CommonData == null)
			{
				Logger.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
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

			//Check if we should find a new item type to gamble for..
			if (nextItemType.Equals(BloodShardGambleItems.None) || GetGambleItemPrice(nextItemType) <= CurrentBloodShardCount)
			{
				nextItemType = BloodShardGambleItems.None;

				List<BloodShardGambleItems> freshList = ValidGambleItems.ToList();
				while (freshList.Count>0)
				{
					Random r = new Random();

					int curListCount=freshList.Count;
					int index = r.Next(0, curListCount-1);

					BloodShardGambleItems itemtype = freshList[index];
					if (Bot.Settings.TownRun.BloodShardGambleItems.HasFlag(itemtype) && GetGambleItemPrice(itemtype) <= CurrentBloodShardCount)
					{
						nextItemType = itemtype;
						break;
					}
					else
					{
						freshList.RemoveAt(index);
					}
				}
			}
			//Did we manage to find a new item to buy?
			if (nextItemType.Equals(BloodShardGambleItems.None))
			{
				Logger.DBLog.DebugFormat("[Funky] Gambling Behavior Finished -- No Valid Item Types Returned.");
				return RunStatus.Success;
			}

			//Check if the item UI element is valid
			UIElement UIItemType = GetGambleItemUIElement(nextItemType);
			if (!UI.ValidateUIElement(UIItemType))
			{
				UIElement UITab = GetGambleItemTabUIElement(nextItemType);
				if (!UI.ValidateUIElement(UITab))
				{
					//Error!!?!
					Logger.DBLog.DebugFormat("[Funky] Gambling Behavior UI Tab Not Valid!");
				}

				UITab.Click();
				return RunStatus.Running;
			}

			//Click Item UI..
			if (!ClickedItemUI)
			{
				if (!TownRunItemLoopsTest()) return RunStatus.Running;

				UIItemType.Click();
				ClickedItemUI = true;
				return RunStatus.Running;
			}
			//Click Inventory Slot..
			if (!ClickedBackpackUI)
			{
				if (!TownRunItemLoopsTest()) return RunStatus.Running;

				UIElement BackpackSlot = UI.Inventory_Backpack_TopLeftSlot;
				if (!UI.ValidateUIElement(BackpackSlot))
				{
					//Error!?!?
					Logger.DBLog.DebugFormat("[Funky] Gambling Behavior UI Item Not Valid!");
				}

				BackpackSlot.Click();
				ClickedBackpackUI = true;
				return RunStatus.Running;
			}
			

			//Confirm Item Purchase..
			if (!TownRunItemLoopsTest(2)) return RunStatus.Running;

			//Reset
			ClickedItemUI = false;
			ClickedBackpackUI = false;
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

			ClickedItemUI = false;
			ClickedBackpackUI = false;
			nextItemType = BloodShardGambleItems.None;
			iCurrentItemLoops = 0;
			RandomizeTheTimer();
			return RunStatus.Success;
		}

		internal static RunStatus GamblingFinish(object ret)
		{
			Logger.DBLog.DebugFormat("Debug: Gambling routine ending sequence...");
			ClickedItemUI = false;
			ClickedBackpackUI = false;
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

		internal static readonly List<BloodShardGambleItems> ValidGambleItems = new List<BloodShardGambleItems>
		{
			BloodShardGambleItems.OneHandItem,BloodShardGambleItems.TwoHandItem,
			BloodShardGambleItems.Amulet,BloodShardGambleItems.Belt,BloodShardGambleItems.Boots,
			BloodShardGambleItems.Bracers,BloodShardGambleItems.Chest,BloodShardGambleItems.Gloves,
			BloodShardGambleItems.Helm,BloodShardGambleItems.Mojo,BloodShardGambleItems.Orb,
			BloodShardGambleItems.Pants,BloodShardGambleItems.Quiver,BloodShardGambleItems.Ring,
			BloodShardGambleItems.Shield,BloodShardGambleItems.Shoulders
		};
	}

}
