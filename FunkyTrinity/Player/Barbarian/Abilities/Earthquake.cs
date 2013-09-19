using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Barb
{
	public class Earthquake : ability, IAbility
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

			ExecutionType = AbilityExecuteFlags.Buff;
			WaitVars = new WaitLoops(4, 4, true);
			Cost = 0;
			UseageType=AbilityUseage.Combat;
			IsSpecialAbility = true;
			Priority = AbilityPriority.High;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckRecastTimer | AbilityPreCastFlags.CheckEnergy |
			                     AbilityPreCastFlags.CheckExisitingBuff | AbilityPreCastFlags.CheckCanCast |
			                     AbilityPreCastFlags.CheckPlayerIncapacitated);
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
				  ability p=(ability)obj;
				  return this.Power==p.Power;
			 }
		}

	
		#endregion
	}
}
