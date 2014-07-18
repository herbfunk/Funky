using System;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.DemonHunter
{
	 public class Caltrops : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		 public override void Initialize()
		  {
				Cooldown=6000;
				
				WaitVars=new WaitLoops(1, 1, true);
				Cost=6;
				SecondaryEnergy=true;
				Range=0;
			
				Priority=SkillPriority.Medium;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckEnergy|
				                          SkillPrecastFlags.CheckRecastTimer));
				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_30, 2);
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_40, 1);
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_Caltrops; }
		  }
	 }
}
