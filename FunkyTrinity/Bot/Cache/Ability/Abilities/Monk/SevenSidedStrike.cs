using System;
using FunkyTrinity.Enums;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Monk
{
	public class SevenSidedStrike : Ability, IAbility
	{
		public SevenSidedStrike() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = PowerExecutionTypes.Location;
			WaitVars = new WaitLoops(2, 3, true);
			Cost = 50;
			Range = 16;
			Priority = AbilityPriority.Low;
			UseFlagsType=AbilityUseFlags.Combat;
			PreCastConditions = (CastingConditionTypes.CheckEnergy | CastingConditionTypes.CheckCanCast |
			                     CastingConditionTypes.CheckRecastTimer | CastingConditionTypes.CheckPlayerIncapacitated);
			ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial, 15);


			Fcriteria = new Func<bool>(() =>
			{
				 return !Bot.Class.bWaitingForSpecial||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount;
			});
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
			get { return SNOPower.Monk_SevenSidedStrike; }
		}
	}
}
