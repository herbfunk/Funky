using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FunkyBot.Cache.Objects;
using FunkyBot.Player;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Logger = FunkyBot.Misc.Logger;

namespace FunkyBot.DBHandlers.Townrun
{

	internal static partial class TownRunManager
	{
		internal static bool IgnoreVendoring = false;
		private static bool InteractionMovementMade = false;
		private static List<CacheACDItem> InteractItems=new List<CacheACDItem>();

		internal static bool InteractionOverlord(object ret)
		{//Should we preform town run finishing behavior?

			InteractItems.Clear();
			//Get new list of current backpack
			Bot.Character.Data.BackPack.Update();

			foreach (var thisitem in Bot.Character.Data.BackPack.CacheItemList.Values)
			{
				if (thisitem.ACDItem.BaseAddress != IntPtr.Zero)
				{
					// Find out if this item's in a protected bag slot
					if (!ItemManager.Current.ItemIsProtected(thisitem.ACDItem))
					{
						if (thisitem.ThisDBItemType == ItemType.Potion) continue;

						if (thisitem.IsHoradricCache)
						{
							InteractItems.Add(thisitem);
						}
						else
						{
							return false;
						}
					}
				}
				else
				{
					Logger.DBLog.DebugFormat("GSError: Diablo 3 memory read error, or item became invalid [FinishOver-1]");
				}
			}

			return InteractItems.Count>0;
		}

		internal static RunStatus InteractionMovement(object ret)
		{//Move to nice open space!
			if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || ZetaDia.Me.CommonData == null)
			{
				Logger.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			Act curAct = ZetaDia.CurrentAct;
			if (curAct == Act.Invalid || curAct == Act.OpenWorld || curAct == Act.Test)
				curAct = Character.FindActByLevelID(Bot.Character.Data.iCurrentLevelID);
			var vectorLocation = ReturnMovementVector(curAct);

			Vector3 vectorPlayerPosition = ZetaDia.Me.Position;
			float fDistance = Vector3.Distance(vectorPlayerPosition, vectorLocation);
			if (fDistance>7.5f)
			{
				Bot.NavigationCache.RefreshMovementCache();

				//Wait until we are not moving to send click again..
				if (Bot.NavigationCache.IsMoving)
					return RunStatus.Running;

				Navigator.PlayerMover.MoveTowards(vectorLocation);
				return RunStatus.Running;
			}

			iCurrentItemLoops = 0;
			RandomizeTheTimer();
			return RunStatus.Success;
		}

		internal static RunStatus InteractionClickBehavior(object ret)
		{
			if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || ZetaDia.Me.CommonData == null)
			{
				Logger.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			if (!UIElements.InventoryWindow.IsVisible)
			{
				Bot.Character.Data.BackPack.InventoryBackPackToggle(true);
				return RunStatus.Running;
			}

			//Interact with items..
			if (InteractItems.Count > 0)
			{
				if (!TownRunItemLoopsTest(2.5)) return RunStatus.Running;

				CacheACDItem thisitem = InteractItems.FirstOrDefault();

				if (thisitem != null)
				{
					ZetaDia.Me.Inventory.UseItem(thisitem.ThisDynamicID);
					if (thisitem.IsHoradricCache) Bot.Game.CurrentGameStats.CurrentProfile.HoradricCacheOpened++;
				}
				if (thisitem != null)
					InteractItems.Remove(thisitem);
				if (InteractItems.Count > 0)
					return RunStatus.Running;
			}

			//Add a long wait after interaction!
			if (!TownRunItemLoopsTest(10)) return RunStatus.Running;

			//Reset so we can loot!
			bFailedToLootLastItem = false;

			return RunStatus.Success;
		}

		internal static RunStatus InteractionLootingBehavior(object ret)
		{
			if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.Me == null || ZetaDia.Me.CommonData == null)
			{
				Logger.DBLog.InfoFormat("[Funky] Town Run Behavior Failed! (Not In Game/Invalid Actor/misc)");
				return RunStatus.Failure;
			}

			//Move!!
			if (!InteractionMovementMade)
			{
				var loc = Navigator.StuckHandler.GetUnstuckPos();
				Navigator.MoveTo(loc);
				InteractionMovementMade = true;
				return RunStatus.Running;
			}

			IgnoreVendoring = true;


			if (Bot.Targeting.CheckHandleTarget()==RunStatus.Running)
				return RunStatus.Running;

			IgnoreVendoring = false;


			return RunStatus.Success;
		}

		internal static RunStatus InteractionFinishBehavior(object ret)
		{
			InteractionMovementMade = false;
			IgnoreVendoring = false;
			return RunStatus.Success;
		}

		private static Vector3 ReturnMovementVector(Act act)
		{
			switch (act)
			{
				case Act.A1:
					if (!Bot.Game.AdventureMode)
						return new Vector3(2959.277f, 2811.887f, 24.04533f);
					else
						return new Vector3(386.6582f, 534.2561f, 24.04533f);
				case Act.A2:
					return new Vector3(299.5841f, 250.1721f, 0.1000036f);
				case Act.A3:
				case Act.A4:
					return new Vector3(403.7034f, 395.9311f, 0.5069602f);
				case Act.A5:
					return new Vector3(532.3179f, 521.8536f, 2.662077f);
			}

			return Vector3.Zero;
		}

		private static int iItemDelayLoopLimit;
		private static int iCurrentItemLoops;
		private static bool TownRunItemLoopsTest(double multiplier = 1)
		{
			iCurrentItemLoops++;
			if (iCurrentItemLoops < iItemDelayLoopLimit * multiplier) return false;

			iCurrentItemLoops = 0;
			RandomizeTheTimer();
			return true;
		}
		private static void RandomizeTheTimer()
		{
			Random rndNum = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), NumberStyles.HexNumber));
			int rnd = rndNum.Next(5);
			iItemDelayLoopLimit = 2 + rnd;
		}
	}

}
