using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Monk
{
	public class LashingTailKick : ability, IAbility
	{
		public LashingTailKick() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Target;
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 30;
			Range = 10;
			Priority = AbilityPriority.Low;
			UseageType=AbilityUseage.Combat;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckEnergy | AbilityPreCastFlags.CheckCanCast |
			                     AbilityPreCastFlags.CheckRecastTimer | AbilityPreCastFlags.CheckPlayerIncapacitated);
			ClusterConditions = new ClusterConditions(4d, 18f, 3, true);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial, 10);


			Fcriteria = new Func<bool>(() =>
			{
				return
					// Either doesn't have sweeping wind, or does but the buff is already up
					(!Bot.Class.HotbarPowers.Contains(SNOPower.Monk_SweepingWind) ||
					 (Bot.Class.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)&&Bot.Class.HasBuff(SNOPower.Monk_SweepingWind)))&&
					(!Bot.Class.bWaitingForSpecial||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount);
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
			get { return SNOPower.Monk_LashingTailKick; }
		}
	}
}
