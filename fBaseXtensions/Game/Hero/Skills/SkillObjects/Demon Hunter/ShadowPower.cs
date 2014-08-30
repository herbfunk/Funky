using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Demonhunter
{
	 public class ShadowPower : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=5000;
			
				WaitVars=new WaitLoops(1, 1, true);
				Cost=14;
				SecondaryEnergy=true;
				
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckRecastTimer|
				                          SkillPrecastFlags.CheckEnergy));

				FcriteriaCombat=() => (FunkyGame.Hero.dCurrentHealthPct<=0.99d||FunkyGame.Hero.bIsRooted||
				                       FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_25]>=1||
				                       FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3);
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_ShadowPower; }
		  }
	 }
}
