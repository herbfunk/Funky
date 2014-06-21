using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class InnerSanctuary : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=20200;
				
				WaitVars=new WaitLoops(1, 1, true);
				Cost=30;
				
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckEnergy|SkillPrecastFlags.CheckCanCast|
				                          SkillPrecastFlags.CheckRecastTimer));

				FcriteriaCombat=() => Bot.Character.Data.dCurrentHealthPct<=0.45;
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.X1_Monk_InnerSanctuary; }
		  }
	 }
}
