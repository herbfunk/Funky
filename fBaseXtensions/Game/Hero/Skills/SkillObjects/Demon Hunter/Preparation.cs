using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Demonhunter
{
	 public class Preparation : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=5000;
				
				WaitVars=new WaitLoops(1, 1, true);
				
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast(SkillPrecastFlags.CheckCanCast);
				Cost = RuneIndex == 0 ? 25 : 0;
										//Rune: Punishment (Restores all Hatered for 25 disc)
				FcriteriaCombat = (u) => (RuneIndex == 0 && FunkyGame.Hero.dCurrentEnergyPct < 0.20d)
											|| (FunkyGame.Hero.dDisciplinePct < 0.25d);


		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_Preparation; }
		  }
	 }
}
