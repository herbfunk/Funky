using System;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Demonhunter
{
	 public class RainOfVengeance : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=10000;
			
				WaitVars=new WaitLoops(1, 1, true);
				
				
				Priority=SkillPriority.Medium;

				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast));


                ClusterConditions.Add(new SkillClusterConditions(10d, 30, 7, true));
                SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 30, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_RainOfVengeance; }
		  }
	 }
}
