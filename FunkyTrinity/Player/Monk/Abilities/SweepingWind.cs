using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Monk
{
	public class SweepingWind : ability, IAbility
	{
		public SweepingWind() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Buff;
			WaitVars = new WaitLoops(0, 1, true);
			Cost = Bot.SettingsFunky.Class.bMonkInnaSet ? 5 : 75;
			Priority = AbilityPriority.High;
			UseageType=AbilityUseage.Combat;
			IsSpecialAbility = true;

			PreCastPreCastFlags = (AbilityPreCastFlags.CheckEnergy | AbilityPreCastFlags.CheckExisitingBuff);

			ClusterConditions = new ClusterConditions(7d, 35f, 2, false);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial, 25);
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
			get { return SNOPower.Monk_SweepingWind; }
		}
	}
}
