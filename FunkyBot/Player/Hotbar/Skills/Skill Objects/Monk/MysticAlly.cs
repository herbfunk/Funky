using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class MysticAlly : Skill
	 {
		 public override void Initialize()
		  {
			 //RuneIndex 2, 1, 0 == Damaging Attack
			 //RuneIndex 3 == Restore Spirit
			 //RuneIndex 4 == Sacrifice for 100% Heal

				Cooldown=RuneIndex==4?50000:30000; //Restore Health increases cooldown to 50s
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(2, 2, true);
				UseageType=AbilityUseage.Anywhere;
				IsBuff=true;
				Priority=AbilityPriority.High;
				IsSpecialAbility=true;
				PreCast=new SkillPreCast(AbilityPreCastFlags.CheckCanCast);
				FcriteriaBuff = () => Bot.Character.Data.PetData.MysticAlly == 0;
				FcriteriaCombat = () => ((this.RuneIndex == 1 || this.RuneIndex==0 || this.RuneIndex==2) && //Damaging Attack
									   (Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_15] > 0 ||
				                       Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>2)) ||
										//Restore Spirit
									   (this.RuneIndex == 3 && Bot.Character.Data.dCurrentEnergy < 30) || 
									   //Restore Health
									   this.RuneIndex == 4 && Bot.Character.Data.dCurrentHealthPct < 0.4d;
		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; }
		  }

		  public override int GetHashCode()
		  {
				return (int)Power;
		  }

		  public override bool Equals(object obj)
		  {
				//Check for null and compare run-time types. 
				if (obj==null||GetType()!=obj.GetType())
				{
					 return false;
				}
			  Skill p=(Skill)obj;
			  return Power==p.Power;
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.X1_Monk_MysticAlly_v2; }
		  }
	 }
}
