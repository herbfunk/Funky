using System;
using Zeta;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.Navigation;
using Zeta.Internals.Actors;
using Zeta.CommonBot;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  internal static partial class TownRunManager
		  {
				// The distance last loop, so we can compare to current distance to work out if we moved
				private static float iLastDistance=0f;
				// This dictionary stores attempted stash counts on items, to help detect any stash stucks on the same item etc.
				internal static Dictionary<int, int> _dictItemStashAttempted=new Dictionary<int, int>();
				// Random variables used during item handling and town-runs
				private static int iItemDelayLoopLimit=0;
				private static int iCurrentItemLoops=0;
				internal static bool bLoggedAnythingThisStash=false;
				private static bool bUpdatedStashMap=false;
				internal static bool bLoggedJunkThisStash=false;
				internal static string sValueItemStatString="";
				internal static string sJunkItemStatString="";
				// Whether to try forcing a vendor-run for custom reasons
				internal static bool bWantToTownRun=false;
				internal static bool bFailedToLootLastItem=false;
				private static bool bLastTownRunCheckResult=false;
				private static bool bReachedSafety=false;
				// DateTime check to prevent inventory-check spam when looking for repairs being needed
				private static DateTime TimeLastCheckedForTownRun=DateTime.Today;
				private static bool bCurrentlyMoving=false;
				private static bool bNeedsEquipmentRepairs=false;
				// Stash mapper - it's an array representing every slot in your stash, true or false dictating if the slot is free or not
				private static bool[,] GilesStashSlotBlocked=new bool[7, 30];

				internal static List<Cache.CacheACDItem> StashedItems=new List<Cache.CacheACDItem>();

				// **********************************************************************************************
				// *****         TownRunCheckOverlord - determine if we should do a town-run or not         *****
				// **********************************************************************************************
				internal static bool GilesTownRunCheckOverlord(object ret)
				{
					 bWantToTownRun=false;

					 // Check if we should be forcing a town-run
					 if (Zeta.CommonBot.Logic.BrainBehavior.IsVendoring)
					 {
						  if (!bLastTownRunCheckResult)
								bPreStashPauseDone=false;

						  bWantToTownRun=true;
					 }
					 else if (DateTime.Now.Subtract(TimeLastCheckedForTownRun).TotalSeconds>6)
					 {
						  TimeLastCheckedForTownRun=DateTime.Now;
						  if (Zeta.CommonBot.Logic.BrainBehavior.ShouldVendor||Bot.Character.BackPack.ShouldRepairItems())
						  {
								if (!bLastTownRunCheckResult)
									 bPreStashPauseDone=false;

								bWantToTownRun=true;
						  }
					 }

					 bLastTownRunCheckResult=bWantToTownRun;

					 //Precheck prior to casting TP..
					 if (bLastTownRunCheckResult&&!ZetaDia.Me.IsInTown)
						  return SafetyCheckForTownRun();

					 return bWantToTownRun;
				}

		  }
	 }
}