using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.WitchDoctor
{
	 public class Horrify : Ability, IAbility
	 {
		  public Horrify()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=16200;
				ExecutionType=AbilityExecuteFlags.Self;
				WaitVars=new WaitLoops(0, 0, true);
				Cost=37;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.Low;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast|
											AbilityPreCastFlags.CheckEnergy);

				FcriteriaCombat=new Func<bool>(() =>
				{
					 return Bot.Character.dCurrentHealthPct<=0.60;
				});
		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; }
		  }

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
				get { return SNOPower.Witchdoctor_Horrify; }
		  }
	 }
}
