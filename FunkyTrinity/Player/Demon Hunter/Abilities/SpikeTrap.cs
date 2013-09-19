using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.DemonHunter
{
	public class SpikeTrap : ability, IAbility
	{
		public SpikeTrap() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Location;
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 30;
			Range = 40;
			UseageType=AbilityUseage.Anywhere;
			Priority = AbilityPriority.Low;

			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated | AbilityPreCastFlags.CheckRecastTimer |
			                     AbilityPreCastFlags.CheckEnergy);
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
				ability p = (ability) obj;
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
