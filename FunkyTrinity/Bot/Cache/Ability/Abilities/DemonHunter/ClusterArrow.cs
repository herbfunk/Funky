using System;
using FunkyTrinity.Enums;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.DemonHunter
{
	public class ClusterArrow : Ability, IAbility
	{
		public ClusterArrow() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityUseType.Location | AbilityUseType.ClusterLocation;
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 50;
			Range = 50;
			IsRanged = true;
			IsProjectile=true;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastConditions = (AbilityConditions.CheckPlayerIncapacitated | AbilityConditions.CheckEnergy |
			                     AbilityConditions.CheckRecastTimer);

			UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_50, 3);
			ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_50, 1);
								//TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial,69),
			ClusterConditions = new ClusterConditions(5d, 50, 1, true);
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
			get { return SNOPower.DemonHunter_ClusterArrow; }
		}
	}
}
