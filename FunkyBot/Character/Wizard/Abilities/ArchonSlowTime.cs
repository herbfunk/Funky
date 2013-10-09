using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.Wizard
{
	 public class ArchonSlowTime : Ability, IAbility
	 {
		  public ArchonSlowTime()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=16000;
				ExecutionType=AbilityExecuteFlags.Target;
				WaitVars=new WaitLoops(1, 1, true);
				Range=48;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.Low;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckRecastTimer|
											AbilityPreCastFlags.CheckCanCast);
				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 2);
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
				TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 35);
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
				get { return SNOPower.Wizard_Archon_SlowTime; }
		  }
	 }
}
