using System;
using FunkyTrinity.Enums;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Barb
{
	public class FuriousCharge : Ability, IAbility
	{
		public FuriousCharge() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_FuriousCharge; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		protected override void Initialize()
		{
			ExecutionType = AbilityUseType.Target;
			WaitVars = new WaitLoops(1, 2, true);
			Cost = 20;
			Range = 35;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastConditions = (AbilityConditions.CheckRecastTimer | AbilityConditions.CheckEnergy |
			                     AbilityConditions.CheckCanCast | AbilityConditions.CheckPlayerIncapacitated);
			ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
			UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 3);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial, 15);
		}
		#region IAbility
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
	}
}
