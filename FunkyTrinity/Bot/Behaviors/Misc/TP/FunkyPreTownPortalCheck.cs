using System;
using FunkyTrinity.Enums;
using Zeta;
using Zeta.Common;



namespace FunkyTrinity
{
    public partial class Funky
    {
        private static bool SafetyCheckForTownRun()
        {
            //This is called only if we want to townrun... basically a pre-check to if we should proceede.
            Logging.WriteDiagnostic("[Funky] Precheck running for town run");

            //Avoidance Flag
				Bot.Combat.CriticalAvoidance=true;

				if (Bot.Refresh.ShouldRefreshObjectList)
            {
					 Bot.Refresh.RefreshDiaObjects();
					 // Check for death / player being dead
					 if (Bot.Character.dCurrentHealthPct<=0)
					 {
						  return false;
					 }
            }

            //Checks
            if (Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_40] > 0 || Bot.Target.CurrentTarget != null)
            {
                return false;
            }

            //All checks passed.. so continue behavior!
            return true;
        }
    }
}