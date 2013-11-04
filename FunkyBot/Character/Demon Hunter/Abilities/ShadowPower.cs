using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.DemonHunter
{
	 public class ShadowPower : Ability, IAbility
	 {
		  public ShadowPower()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=5000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(1, 1, true);
				Cost=14;
				SecondaryEnergy=true;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckRecastTimer|
											AbilityPreCastFlags.CheckEnergy);

				FcriteriaCombat=new Func<bool>(() =>
				{
					 return (Bot.Character.dCurrentHealthPct<=0.99d||Bot.Character.bIsRooted||
								Bot.Targeting.Environment.iElitesWithinRange[(int)RangeIntervals.Range_25]>=1||
								Bot.Targeting.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3);

				});
		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Class.HotBar.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.HotBar.RuneIndexCache[this.Power]:-1; }
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
				get { return SNOPower.DemonHunter_ShadowPower; }
		  }
	 }
}
