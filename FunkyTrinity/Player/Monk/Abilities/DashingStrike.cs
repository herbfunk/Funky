using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Monk
{
	public class DashingStrike : ability, IAbility
	{
		public DashingStrike() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Target;
			UseageType=AbilityUseage.Combat;
			WaitVars = new WaitLoops(0, 1, true);
			Cost = 25;
			Range = 30;
			Priority = AbilityPriority.Low;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckEnergy | AbilityPreCastFlags.CheckCanCast |
			                     AbilityPreCastFlags.CheckRecastTimer | AbilityPreCastFlags.CheckPlayerIncapacitated);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.Ranged, 20);


			Fcriteria = new Func<bool>(() =>
			{
				 return (!Bot.Class.bWaitingForSpecial||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount);
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
			get { return SNOPower.Monk_DashingStrike; }
		}
	}
}
