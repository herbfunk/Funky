using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class IceArmor : Skill
	 {
		 public override bool IsBuff { get { return true; } }
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=115000;
				
				WaitVars=new WaitLoops(1, 2, true);
				Cost=25;
				Counter=1;
				
				
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckEnergy|
				                          SkillPrecastFlags.CheckExisitingBuff));
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_IceArmor; }
		  }
	 }
}
