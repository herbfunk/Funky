using fBaseXtensions.Game;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Monk
{
	 public class Serenity : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
				Cooldown=20200;
				
				WaitVars=new WaitLoops(1, 1, true);
				
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckRecastTimer));

				//TODO:: Give better conditions
				FcriteriaCombat = () => FunkyGame.Hero.dCurrentHealthPct <= 0.45d
				                      &&Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20]>0;
		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.Monk_Serenity; }
		  }
	 }
}
