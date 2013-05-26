using System;
using Zeta;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.Navigation;
using Zeta.Internals.Actors;
using Zeta.CommonBot;
using System.Linq;
using System.IO;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  // **********************************************************************************************
		  // *****         TownRunCheckOverlord - determine if we should do a town-run or not         *****
		  // **********************************************************************************************
		  private static bool GilesTownRunCheckOverlord(object ret)
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
				else if (OverrideTownportalBehavior)
					 bWantToTownRun=true;

				bLastTownRunCheckResult=bWantToTownRun;

				//Precheck prior to casting TP..
				if (bLastTownRunCheckResult&&!ZetaDia.Me.IsInTown)
					 return SafetyCheckForTownRun();

				return bWantToTownRun;
		  }

	 }
}