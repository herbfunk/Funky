using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	 public class MysticAlly : Skill
	 {
		 public override bool IsBuff { get { return true; } }
		 public override bool IsSpecialAbility { get { return true; } }
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override void Initialize()
		  {
			 //RuneIndex 2, 1, 0 == Damaging Attack
			 //RuneIndex 3 == Restore Spirit
			 //RuneIndex 4 == Sacrifice for 100% Heal

				Cooldown=RuneIndex==4?50000:30000; //Restore Health increases cooldown to 50s
			
				WaitVars=new WaitLoops(2, 2, true);
				
				
				Priority=SkillPriority.High;
				
				PreCast=new SkillPreCast(SkillPrecastFlags.CheckCanCast);
				FcriteriaBuff = () => FunkyGame.Targeting.Cache.Environment.HeroPets.MysticAlly == 0;
				FcriteriaCombat = (u) => ((this.RuneIndex == 1 || this.RuneIndex==0 || this.RuneIndex==2) && //Damaging Attack
									   (FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_15] > 0 ||
				                       FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>2)) ||
										//Restore Spirit
									   (this.RuneIndex == 3 && FunkyGame.Hero.dCurrentEnergy < 30) || 
									   //Restore Health
									   this.RuneIndex == 4 && FunkyGame.Hero.dCurrentHealthPct < 0.4d;
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.X1_Monk_MysticAlly_v2; }
		  }
	 }
}
