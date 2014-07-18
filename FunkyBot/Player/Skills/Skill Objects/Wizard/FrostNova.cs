using System;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Wizard
{
	 public class FrostNova : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		 public override void Initialize()
		  {
				Cooldown=9000;
				
				WaitVars=new WaitLoops(0, 0, true);
				Range=14;
				
				Priority=SkillPriority.Medium;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast));

				ClusterConditions.Add(new SkillClusterConditions(7d, 20f, 2, false));
				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_20, 1);
				ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_20, 1);
				//SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 12);
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_FrostNova; }
		  }
	 }
}
