using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.WitchDoctor
{
	 public class BigBadVoodoo : Ability, IAbility
	 {
		  public BigBadVoodoo()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=120000;
				ExecutionType=AbilityExecuteFlags.Self;
				WaitVars=new WaitLoops(0, 0, true);
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.Low;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast);

				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_6, 1);
				TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 12);
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
				get { return SNOPower.Witchdoctor_BigBadVoodoo; }
		  }
	 }
}
