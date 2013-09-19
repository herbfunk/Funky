using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Barb
{
	public class Revenge : ability, IAbility
	{
		public Revenge() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_Revenge; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Self;
			WaitVars = new WaitLoops(4, 4, true);
			Cost = 0;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckRecastTimer | AbilityPreCastFlags.CheckEnergy |
			                     AbilityPreCastFlags.CheckCanCast | AbilityPreCastFlags.CheckPlayerIncapacitated);

			UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_6, 1);
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
