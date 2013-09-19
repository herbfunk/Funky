using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.DemonHunter
{
	public class Strafe : ability, IAbility
	{
		public Strafe() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.ZigZagPathing;
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 15;
			Range = 25;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated | AbilityPreCastFlags.CheckEnergy);
			UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 2);
			 //TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.None, 15);

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
			get { return SNOPower.DemonHunter_Strafe; }
		}
	}
}
