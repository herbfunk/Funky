using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Demonhunter
{
	 public class SmokeScreen : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=3000;
			
				WaitVars=new WaitLoops(0, 1, true);
				Cost=28;
				SecondaryEnergy=true;
				Range=0;
				
				Priority=SkillPriority.High;
				//PreCastFlags=,

				FcriteriaCombat=() => (!Hotbar.HasBuff(SNOPower.DemonHunter_ShadowPower)||FunkyGame.Hero.bIsIncapacitated)
				                      &&(FunkyGame.Hero.dDiscipline>=28)
				                      &&
				                      (FunkyGame.Hero.dCurrentHealthPct<=0.90||FunkyGame.Hero.bIsRooted||
				                       FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_20]>=1||
				                       FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3||FunkyGame.Hero.bIsIncapacitated);
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_SmokeScreen; }
		  }
	 }
}
