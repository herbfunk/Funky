using System;
using System.Globalization;
using System.Linq;
using FunkyBot.Cache.Objects;
using Zeta.Bot.Logic;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Game;

namespace FunkyBot.DBHandlers
{
	[Flags]
	public enum BloodShardGambleItems
	{
		None = 0,
		OneHandItem = 1,
		TwoHandItem = 2,
		Quiver = 4,
		Orb = 8,
		Mojo = 16,
		Helm = 32,
		Gloves = 64,
		Boots = 128,
		Chest = 256,
		Belt = 512,
		Shoulders = 1024,
		Pants = 2048,
		Bracers = 4096,
		Shield = 8192,
		Ring = 16384,
		Amulet = 32768,

		All = OneHandItem | TwoHandItem | Quiver | Orb | Mojo | Helm | Gloves | Boots | Chest | Belt | Shoulders | Pants | Bracers | Shield | Ring | Amulet
	}

	internal static partial class TownRunManager
	{
		private static bool PotionCheck = false;

		private static bool bPreStashPauseDone;
		private static double iPreStashLoops;

		private static int[] LastStashPoint = { -1, -1 };
		private static int LastStashPage = -1;


		private static Vector3 TownportalMovementVector3 = Vector3.Zero;

		// The distance last loop, so we can compare to current distance to work out if we moved
		private static float iLastDistance;
		// This dictionary stores attempted stash counts on items, to help detect any stash stucks on the same item etc.
		internal static Dictionary<int, int> _dictItemStashAttempted = new Dictionary<int, int>();

		internal static bool bLoggedAnythingThisStash = false;
		private static bool bUpdatedStashMap;
		private static bool bCheckUnidItems;
		private static bool bCheckedItemDurability;

		internal static bool bLoggedJunkThisStash = false;
		internal static string sValueItemStatString = "";
		internal static string sJunkItemStatString = "";
		// Whether to try forcing a vendor-run for custom reasons
		internal static bool bWantToTownRun = false;
		internal static bool bFailedToLootLastItem = false;
		private static bool bLastTownRunCheckResult;
		private static bool bReachedSafety;
		// DateTime check to prevent inventory-check spam when looking for repairs being needed
		private static DateTime TimeLastCheckedForTownRun = DateTime.Today;
		private static bool bCurrentlyMoving;
		private static bool bNeedsEquipmentRepairs;
		// Stash mapper - it's an array representing every slot in your stash, true or false dictating if the slot is free or not
		private static readonly bool[,] GilesStashSlotBlocked = new bool[7, 40];
		internal static bool TownrunStartedInTown = true;

		internal static TownRunCache townRunItemCache = new TownRunCache();

		public class TownRunCache
		{
			private int InventoryRowCombine(int i)
			{
				if ((i & 1) == 0)
					return i;
				else
					return i - 1;
			}

			// These three lists are used to cache item data from the backpack when handling sales, salvaging and stashing
			// It completely minimized D3 <-> DB memory access, to reduce any random bugs/crashes etc.
			public HashSet<CacheACDItem> KeepItems = new HashSet<CacheACDItem>();
			public HashSet<CacheACDItem> SalvageItems = new HashSet<CacheACDItem>();
			public HashSet<CacheACDItem> SellItems = new HashSet<CacheACDItem>();
			public HashSet<CacheACDItem> InteractItems = new HashSet<CacheACDItem>(); 

			public void sortSellList()
			{
				List<CacheACDItem> sortedList = SellItems.OrderBy(o => InventoryRowCombine(o.invRow)).ThenBy(o => o.invCol).ToList();
				var newSortedHashSet = new HashSet<CacheACDItem>();
				foreach (var item in sortedList)
				{
					newSortedHashSet.Add(item);
				}

				SellItems = newSortedHashSet;

			}

			public void sortSalvagelist()
			{
				List<CacheACDItem> sortedList = SalvageItems.OrderBy(o => InventoryRowCombine(o.invRow)).ThenBy(o => o.invCol).ToList();
				var newSortedHashSet = new HashSet<CacheACDItem>();
				foreach (var item in sortedList)
				{
					newSortedHashSet.Add(item);
				}

				SalvageItems = newSortedHashSet;

			}
		}

		// Random variables used during item handling and town-runs
		private static int iItemDelayLoopLimit;
		private static int iCurrentItemLoops;
		internal static bool TownRunItemLoopsTest(double multiplier = 1)
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

		// **********************************************************************************************
		// *****         TownRunCheckOverlord - determine if we should do a town-run or not         *****
		// **********************************************************************************************
		internal static bool GilesTownRunCheckOverlord(object ret)
		{
			bWantToTownRun = false;

			// Check if we should be forcing a town-run
			if (BrainBehavior.IsVendoring)
			{
				bWantToTownRun = true;
			}
			else
			{
				int recheckDelay = Bot.Character.Data.bIsInTown ? 1 : 6;
				if (DateTime.Now.Subtract(TimeLastCheckedForTownRun).TotalSeconds > recheckDelay)
				{
					TimeLastCheckedForTownRun = DateTime.Now;
					if (BrainBehavior.ShouldVendor || Bot.Character.Data.BackPack.ShouldRepairItems())
					{
						bCheckedItemDurability = false;
						bWantToTownRun = true;
					}
				}
			}

			bLastTownRunCheckResult = bWantToTownRun;

			//Precheck prior to casting TP..
			if (bLastTownRunCheckResult)
			{
				if (!ZetaDia.IsInTown)
				{
					bPreStashPauseDone = false;
					Bot.NavigationCache.LOSmovementObject = null;
					return TownPortalBehavior.SafetyCheckForTownRun();
				}

				//Should we check Unidentified Items?
				bCheckUnidItems = Bot.Settings.ItemRules.ItemRulesUnidStashing;
			}

			return bWantToTownRun;
		}

		enum TownRunBehavior
		{
			Stash,
			Sell,
			Salvage,
			Gamble,
			Interaction
		}

		private static Vector3 ReturnMovementVector(TownRunBehavior type, Act act)
		{
			switch (type)
			{
				case TownRunBehavior.Salvage:
					switch (act)
					{
						case Act.A1:
							if (!Bot.Game.AdventureMode)
								return new Vector3(2958.418f, 2823.037f, 24.04533f);
							else
								return new Vector3(375.5075f, 563.1337f, 24.04533f);
						case Act.A2:
							return new Vector3(289.6358f, 232.1146f, 0.1f);
						case Act.A3:
						case Act.A4:
							return new Vector3(379.6096f, 415.6198f, 0.3321424f);
						case Act.A5:
							return new Vector3(560.1434f, 501.5706f, 2.685907f);
					}
					break;
				case TownRunBehavior.Sell:
					switch (act)
					{
						case Act.A1:
							if (!Bot.Game.AdventureMode)
								return new Vector3(2901.399f, 2809.826f, 24.04533f);
							else
								return new Vector3(320.8555f, 524.6776f, 24.04532f);
						case Act.A2:
							return new Vector3(295.2101f, 265.1436f, 0.1000002f);
						case Act.A3:
						case Act.A4:
							return new Vector3(410.6073f, 355.8762f, 0.1000005f);
						case Act.A5:
							return new Vector3(560.1434f, 501.5706f, 2.685907f);
					}
					break;
				case TownRunBehavior.Stash:
					switch (act)
					{
						case Act.A1:
							if (!Bot.Game.AdventureMode)
								return new Vector3(2967.146f, 2799.459f, 24.04533f);
							else
								return new Vector3(386.8494f, 524.2585f, 24.04533f);
						case Act.A2:
							return new Vector3(323.4543f, 228.5806f, 0.1f);
						case Act.A3:
						case Act.A4:
							return new Vector3(389.3798f, 390.7143f, 0.3321428f);
						case Act.A5:
							return new Vector3(510.6552f, 502.1889f, 2.620764f);
					}
					break;
				case TownRunBehavior.Gamble:
					switch (act)
					{
						case Act.A1:
							return new Vector3(376.3878f, 561.3141f, 24.04533f);
						case Act.A2:
							return new Vector3(334.8506f, 267.2392f, 0.1000038f);
						case Act.A3:
						case Act.A4:
							return new Vector3(458.5429f, 416.3311f, 0.2663189f);
						case Act.A5:
							return new Vector3(592.5067f, 535.6719f, 2.74532f);
					}
					break;
				case TownRunBehavior.Interaction:
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
					break;
			}

			return Vector3.Zero;
		}


	}

}