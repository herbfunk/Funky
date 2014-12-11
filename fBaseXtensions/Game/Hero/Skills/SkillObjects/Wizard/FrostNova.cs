using System;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Wizard
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
                SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 20, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_FrostNova; }
		  }
	 }
}
