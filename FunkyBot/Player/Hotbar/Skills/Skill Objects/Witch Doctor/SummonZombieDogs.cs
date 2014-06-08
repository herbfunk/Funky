using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	 public class SummonZombieDogs : Skill
	 {
		 public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }


		  public override void Initialize()
		  {
				Cooldown=25000;
				ExecutionType=SkillExecutionFlags.Buff;
				WaitVars=new WaitLoops(0, 0, true);
				UseageType=SkillUseage.Anywhere;
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast(SkillPrecastFlags.CheckCanCast);
				IsBuff=true;
				FcriteriaBuff= () => Bot.Character.Data.PetData.ZombieDogs<GetTotalZombieDogsSummonable();
				FcriteriaCombat=() => Bot.Character.Data.PetData.ZombieDogs<GetTotalZombieDogsSummonable();
		  }

		 private int GetTotalZombieDogsSummonable()
		  {
			 int total=3;

			 if (Bot.Character.Class.HotBar.PassivePowers.Contains(SNOPower.Witchdoctor_Passive_ZombieHandler))
				 total++;
			 if (Bot.Character.Class.HotBar.PassivePowers.Contains(SNOPower.Witchdoctor_Passive_MidnightFeast))
				 total++;
			 if (Bot.Character.Class.HotBar.PassivePowers.Contains(SNOPower.Witchdoctor_Passive_FierceLoyalty))
				 total++;

			 //Tall Man Finger - Only 1
			 if (Bot.Settings.WitchDoctor.TallManFinger)
				 total = 1;

			 return total;
		  }

		  #region IAbility


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
				get { return SNOPower.Witchdoctor_SummonZombieDog; }
		  }
	 }
}
