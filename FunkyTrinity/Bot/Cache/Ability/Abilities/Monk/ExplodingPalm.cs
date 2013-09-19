using System;
using FunkyTrinity.Enums;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Monk
{
	public class ExplodingPalm : Ability, IAbility
	{
		public ExplodingPalm() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityUseType.Target;
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 40;
			Range = 14;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastConditions = (AbilityConditions.CheckEnergy | AbilityConditions.CheckCanCast |
			                     AbilityConditions.CheckRecastTimer | AbilityConditions.CheckPlayerIncapacitated);

			UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 3);
			ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial, 14);


			Fcriteria = new Func<bool>(() =>
			{
				 return (!Bot.Class.bWaitingForSpecial||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount);
			});
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
			get { return SNOPower.Monk_ExplodingPalm; }
		}
	}
}
