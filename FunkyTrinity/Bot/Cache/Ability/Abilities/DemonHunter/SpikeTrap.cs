using System;
using FunkyTrinity.Enums;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.DemonHunter
{
	public class SpikeTrap : Ability, IAbility
	{
		public SpikeTrap() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = PowerExecutionTypes.Location;
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 30;
			Range = 40;
			UseFlagsType=AbilityUseFlags.Anywhere;
			Priority = AbilityPriority.Low;

			PreCastConditions = (CastingConditionTypes.CheckPlayerIncapacitated | CastingConditionTypes.CheckRecastTimer |
			                     CastingConditionTypes.CheckEnergy);
			UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 4);
			ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_30, 1);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial, 35);


			Fcriteria = new Func<bool>(() =>
			{
				return Bot.Combat.powerLastSnoPowerUsed != SNOPower.DemonHunter_SpikeTrap;

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
			get { return SNOPower.DemonHunter_SpikeTrap; }
		}
	}
}
