using System;
using FunkyTrinity.Enums;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;



namespace FunkyTrinity
{
    public partial class Funky
    {
        private static bool SafetyCheckForTownRun()
        {
            //This is called only if we want to townrun... basically a pre-check to if we should proceede.
				Logger.Write(LogLevel.OutOfCombat, "Precheck running for town run");

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

				//check if we recently taken waypoint..
				if (Bot.Profile.LastProfileBehavior!=null&&Bot.Profile.LastProfileBehavior.GetType()==typeof(Zeta.CommonBot.Profile.Common.UseWaypointTag))
				{
					//Only if it was within the last 10 seconds..
					 if (DateTime.Now.Subtract(Bot.Profile.LastProfileBehaviorChanged).TotalSeconds<10)
					 {
						  Logger.Write(LogLevel.OutOfCombat, "Recently Used Waypoint, Updating Navigator with movement.");
						 //To prevent any issues.. we should force movement -- to cache our location.
						  Vector3 movementLoc=MathEx.GetPointAt(Bot.Character.Position, 5f, 0);
						  Zeta.Navigation.Navigator.MoveTo(movementLoc, "NavUpdate", false);
						  //CommonBehaviors.MoveAndStop((ValueRetriever<Vector3>)(ret=>Zeta.Navigation.Navigator.StuckHandler.GetUnstuckPos()),10f, true, "NavUpdate", Zeta.TreeSharp.RunStatus.Failure, false);// Zeta.Navigation.Navigator.StuckHandler.GetUnstuckPos()
					 }
				}

            //All checks passed.. so continue behavior!
            return true;
        }
    }
}