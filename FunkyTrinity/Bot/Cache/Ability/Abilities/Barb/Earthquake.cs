using System;
using FunkyTrinity.Enums;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Barb
{
	public class Earthquake : Ability, IAbility
	{
		public Earthquake() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_Earthquake; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		public override void Initialize()
		{

			ExecutionType = PowerExecutionTypes.Buff;
			WaitVars = new WaitLoops(4, 4, true);
			Cost = 0;
			UseFlagsType=AbilityUseFlags.Combat;
			IsSpecialAbility = true;
			Priority = AbilityPriority.High;
			PreCastConditions = (CastingConditionTypes.CheckRecastTimer | CastingConditionTypes.CheckEnergy |
			                     CastingConditionTypes.CheckExisitingBuff | CastingConditionTypes.CheckCanCast |
			                     CastingConditionTypes.CheckPlayerIncapacitated);
			ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial, 13);
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
