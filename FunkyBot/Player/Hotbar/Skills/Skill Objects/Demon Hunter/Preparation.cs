using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class Preparation : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=5000;
				
				WaitVars=new WaitLoops(1, 1, true);
				
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckRecastTimer|
				                          SkillPrecastFlags.CheckCanCast));
				Cost = RuneIndex == 0 ? 25 : 0;
				FcriteriaCombat=() => Bot.Character.Data.dDisciplinePct<0.25d
					//Rune: Punishment (Restores all Hatered for 25 disc)
									  || (RuneIndex == 0 && Bot.Character.Data.dCurrentEnergyPct < 0.20d);

		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_Preparation; }
		  }
	 }
}
