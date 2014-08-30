using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Wizard
{
	 public class DiamondSkin : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=15000;
				
				WaitVars=new WaitLoops(0, 1, true);
				
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckCanCast));

				FcriteriaBuff = () => FunkyGame.Hero.dCurrentHealthPct <= 0.45d && FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_40] > 0;
				FcriteriaCombat=() => FunkyGame.Hero.dCurrentHealthPct<=0.45d && FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_40]>0;
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_DiamondSkin; }
		  }
	 }
}
