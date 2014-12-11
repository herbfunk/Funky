using System;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Demonhunter
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
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast));

                ClusterConditions.Add(new SkillClusterConditions(10d, 30, 4, true));
                SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 40, falseConditionalFlags: TargetProperties.Normal));

		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_Caltrops; }
		  }
	 }
}
