using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	 public class Gargantuan : Skill
	 {
		 public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=25000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(2, 1, true);
				Cost=147;
				Counter=1;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast|
				                          AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckPetCount));
				IsBuff=true;
				FcriteriaBuff=
				  () => Bot.Character.Class.HotBar.RuneIndexCache[SNOPower.Witchdoctor_Gargantuan]!=0&&Bot.Character.Data.PetData.Gargantuan==0;
				FcriteriaCombat=() => (Bot.Character.Class.HotBar.RuneIndexCache[SNOPower.Witchdoctor_Gargantuan]==0&&
				                       (Bot.Targeting.Environment.iElitesWithinRange[(int)RangeIntervals.Range_15]>=1||
				                        (Bot.Targeting.CurrentUnitTarget.IsEliteRareUnique&&Bot.Targeting.CurrentTarget.RadiusDistance<=15f))
				                       ||Bot.Character.Class.HotBar.RuneIndexCache[SNOPower.Witchdoctor_Gargantuan]!=0&&Bot.Character.Data.PetData.Gargantuan==0);
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
				get { return SNOPower.Witchdoctor_Gargantuan; }
		  }
	 }
}
