using fBaseXtensions.Game;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Wizard
{
	 public class MirrorImage : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		 public override void Initialize()
		  {
				Cooldown=5000;
				
				WaitVars=new WaitLoops(1, 1, true);
				Cost=10;
				Range=48;
			
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckCanCast));

				FcriteriaCombat=() => (FunkyGame.Hero.dCurrentHealthPct<=0.50||
				                       Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_30]>=5||FunkyGame.Hero.bIsIncapacitated||
				                       FunkyGame.Hero.bIsRooted||Bot.Targeting.Cache.CurrentTarget.ObjectIsSpecial);
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_MirrorImage; }
		  }
	 }
}
