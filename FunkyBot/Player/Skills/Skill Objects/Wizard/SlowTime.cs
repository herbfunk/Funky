using System;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Wizard
{
	 public class SlowTime : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=16000;
				
				WaitVars=new WaitLoops(1, 1, true);
				
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast));
				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 2);
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 35, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_SlowTime; }
		  }
	 }
}
