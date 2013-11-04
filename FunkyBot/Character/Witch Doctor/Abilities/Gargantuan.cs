using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.WitchDoctor
{
	 public class Gargantuan : Ability, IAbility
	 {
		  public Gargantuan()
				: base()
		  {
		  }



		  public override int RuneIndex { get { return Bot.Class.HotBar.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.HotBar.RuneIndexCache[this.Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=25000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(2, 1, true);
				Cost=147;
				Counter=1;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast|
											AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckPetCount);
				IsBuff=true;
				FcriteriaBuff=
				  new Func<bool>(
					  () =>
					  {
							return Bot.Class.HotBar.RuneIndexCache[SNOPower.Witchdoctor_Gargantuan]!=0&&Bot.Character.PetData.Gargantuan==0;
					  });
				FcriteriaCombat=new Func<bool>(() =>
				{
					 return (Bot.Class.HotBar.RuneIndexCache[SNOPower.Witchdoctor_Gargantuan]==0&&
							  (Bot.Targeting.Environment.iElitesWithinRange[(int)RangeIntervals.Range_15]>=1||
								(Bot.Targeting.CurrentUnitTarget.IsEliteRareUnique&&Bot.Targeting.CurrentTarget.RadiusDistance<=15f))
									||Bot.Class.HotBar.RuneIndexCache[SNOPower.Witchdoctor_Gargantuan]!=0&&Bot.Character.PetData.Gargantuan==0);
				});
		  }

		  #region IAbility



		  public override int GetHashCode()
		  {
				return (int)this.Power;
		  }

		  public override bool Equals(object obj)
		  {
				//Check for null and compare run-time types. 
				if (obj==null||this.GetType()!=obj.GetType())
				{
					 return false;
				}
				else
				{
					 Ability p=(Ability)obj;
					 return this.Power==p.Power;
				}
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.Witchdoctor_Gargantuan; }
		  }
	 }
}
