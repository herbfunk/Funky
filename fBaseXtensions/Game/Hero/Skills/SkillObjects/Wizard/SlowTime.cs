using System;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Wizard
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
                //UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 2);
                //ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);

                ClusterConditions.Add(new SkillClusterConditions(7d, 25f, 2, false));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 35, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_SlowTime; }
		  }
	 }
}
