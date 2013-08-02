using System;
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

            if (Bot.ShouldRefreshObjectList)
            {
					 Bot.RefreshDiaObjects();
					 // Check for death / player being dead
					 if (Bot.Character.dCurrentHealthPct<=0)
					 {
						  return false;
					 }
            }

            //Checks
            if (Bot.Combat.iAnythingWithinRange[RANGE_40] > 0 || Bot.Target.CurrentTarget != null)
            {
                return false;
            }

            //All checks passed.. so continue behavior!
            return true;
        }
    }
}