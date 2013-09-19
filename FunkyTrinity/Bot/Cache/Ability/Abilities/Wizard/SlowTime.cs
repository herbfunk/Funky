using System;
using FunkyTrinity.Enums;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Wizard
{
	public class SlowTime : Ability, IAbility
	{
		public SlowTime() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityUseType.Buff;
			WaitVars = new WaitLoops(1, 1, true);
			UseageType=AbilityUseage.Anywhere;
			Priority = AbilityPriority.High;
			PreCastConditions = (AbilityConditions.CheckPlayerIncapacitated | AbilityConditions.CheckCanCast);
			UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 2);
			ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial, 35);
		}

		public override void InitCriteria()
		{
			base.AbilityTestConditions = new AbilityUsablityTests(this);
		}

		#region IAbility

		public override int RuneIndex
		{
			get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power) ? Bot.Class.RuneIndexCache[this.Power] : -1; }
		}

		public override int GetHashCode()
		{
			return (int) this.Power;
		}

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj == null || this.GetType() != obj.GetType())
			{
				return false;
			}
			else
			{
				Ability p = (Ability) obj;
				return this.Power == p.Power;
			}
		}

		#endregion

		public override SNOPower Power
		{
			get { return SNOPower.Wizard_SlowTime; }
		}
	}
}
