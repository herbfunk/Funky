using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.DemonHunter
{
	 public class Sentry : Ability, IAbility
	 {
		  public Sentry()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=12000;
				ExecutionType=AbilityExecuteFlags.Self;
				WaitVars=new WaitLoops(0, 0, true);
				Cost=30;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCastFlags=(AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckRecastTimer|
											AbilityPreCastFlags.CheckPlayerIncapacitated);

				FcriteriaCombat=new Func<bool>(() =>
				{
					 return Bot.Class.LastUsedAbility.Power!=SNOPower.DemonHunter_Sentry&&
							 (Bot.Targeting.FleeingLastTarget||DateTime.Now.Subtract(Bot.Targeting.LastFleeAction).TotalMilliseconds<1000)||
							 (Bot.Targeting.Environment.iElitesWithinRange[(int)RangeIntervals.Range_40]>=1||
							  Bot.Targeting.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_40]>=2);
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
				get { return SNOPower.DemonHunter_Sentry; }
		  }
	 }
}
