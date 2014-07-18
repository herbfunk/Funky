using System;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.DemonHunter
{
	 public class RainOfVengeance : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=10000;
			
				WaitVars=new WaitLoops(1, 1, true);
				
				
				Priority=SkillPriority.Medium;

				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckRecastTimer|
				                          SkillPrecastFlags.CheckCanCast));
				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 7);
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_RainOfVengeance; }
		  }
	 }
}
