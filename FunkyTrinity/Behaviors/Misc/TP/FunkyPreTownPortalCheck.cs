using System;
using FunkyBot.AbilityFunky;
using Zeta;
using Zeta.Common;



namespace FunkyBot
{
    public partial class Funky
    {
        private static bool SafetyCheckForTownRun()
        {
            //This is called only if we want to townrun... basically a pre-check to if we should proceede.
				if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.OutOfCombat))
					 Logger.Write(LogLevel.OutOfCombat, "Precheck running for town run");

            //Avoidance Flag
				Bot.Character.CriticalAvoidance=true;

				if (Bot.Targeting.ShouldRefreshObjectList)
            {
					 Bot.Targeting.RefreshDiaObjects();
					 // Check for death / player being dead
					 if (Bot.Character.dCurrentHealthPct<=0)
					 {
						  return false;
					 }
            }

            //Checks
            if (Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_40] > 0 || Bot.Targeting.CurrentTarget != null)
            {
                return false;
            }

            //All checks passed.. so continue behavior!
            return true;
        }
    }
}