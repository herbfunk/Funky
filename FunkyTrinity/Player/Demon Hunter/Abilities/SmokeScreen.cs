using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.DemonHunter
{
	 public class SmokeScreen : Ability, IAbility
	 {
		  public SmokeScreen()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=3000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=28;
				SecondaryEnergy=true;
				Range=0;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				//PreCastFlags=,

				FcriteriaCombat=new Func<bool>(() =>
				{
					 return (!Bot.Class.HasBuff(SNOPower.DemonHunter_ShadowPower)||Bot.Character.bIsIncapacitated)
							  &&(Bot.Character.dDiscipline>=28||(Bot.Character.dDiscipline>=14&&Bot.Combat.IsFleeing))
							  &&
							  (Bot.Character.dCurrentHealthPct<=0.90||Bot.Character.bIsRooted||
								Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_20]>=1||
								Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3||Bot.Character.bIsIncapacitated);
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
				get { return SNOPower.DemonHunter_SmokeScreen; }
		  }
	 }
}
