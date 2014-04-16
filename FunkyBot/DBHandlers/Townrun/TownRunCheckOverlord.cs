using System;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.DBHandlers
{

	internal static partial class TownRunManager
	{
		private static bool PotionCheck = false;

		private static bool bPreStashPauseDone;
		private static double iPreStashLoops;

		private static int[] LastStashPoint = { -1, -1 };
		private static int LastStashPage = -1;

		private static bool FoundTownPortal;
		private static DiaGizmo TownPortalObj;
		private static Vector3 TownportalMovementVector3 = Vector3.Zero;

		// The distance last loop, so we can compare to current distance to work out if we moved
		private static float iLastDistance;
		// This dictionary stores attempted stash counts on items, to help detect any stash stucks on the same item etc.
		internal static Dictionary<int, int> _dictItemStashAttempted = new Dictionary<int, int>();
		// Random variables used during item handling and town-runs
		private static int iItemDelayLoopLimit;
		private static int iCurrentItemLoops;
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
			Salvage
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
			}

			return Vector3.Zero;
		}


	}

}