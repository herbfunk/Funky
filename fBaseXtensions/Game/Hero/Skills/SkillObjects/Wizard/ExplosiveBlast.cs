using System;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Wizard
{
	 public class ExplosiveBlast : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		 public override void Initialize()
		  {
				Cooldown=6000;
				WaitVars=new WaitLoops(0, 0, true);
				Cost=20;
				Range=10;
				
				Priority=SkillPriority.Medium;
				PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);
				
             
             // UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
             //   ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);

                SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 20, MinimumHealthPercent: 0.95d));

				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 12, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_ExplosiveBlast; }
		  }
	 }
}
