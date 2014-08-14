using System;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Wizard
{
	 public class ArchonSlowTime : Skill
	 {
		 public override bool IsBuff { get { return true; } }
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=16000;
			
				
				Priority = SkillPriority.High;
				WaitVars=new WaitLoops(1, 1, true);
				//Range=48;
				
				PreCast = new SkillPreCast
				{
					Criteria = skill => skill.LastUsed == DateTime.MinValue
				};
				
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_Archon_SlowTime; }
		  }
	 }
}
