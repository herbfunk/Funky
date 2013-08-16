using System;
using FunkyTrinity.Enums;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Barb
{
	public class GroundStomp : Ability, IAbility
	{
		public GroundStomp() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_GroundStomp; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		protected override void Initialize()
		{
			ExecutionType = AbilityUseType.Self;
			WaitVars = new WaitLoops(1, 2, true);
			Cost = 20;
			Range = 16;
			UseageType=AbilityUseage.Anywhere;
			Priority = AbilityPriority.Low;

			PreCastConditions = (AbilityConditions.CheckRecastTimer | AbilityConditions.CheckEnergy |
			                     AbilityConditions.CheckCanCast | AbilityConditions.CheckPlayerIncapacitated);
			UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 4);
			ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1);
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
