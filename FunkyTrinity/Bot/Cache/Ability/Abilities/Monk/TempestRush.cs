using System;
using FunkyTrinity.Enums;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Monk
{
	public class TempestRush : Ability, IAbility
	{
		public TempestRush() : base()
		{
		}


		private bool IsHobbling
		{
			 get
			 {
					Bot.Character.UpdateAnimationState(false);
					return Bot.Character.CurrentSNOAnim.HasFlag(SNOAnim.Monk_Female_Hobble_Run|SNOAnim.Monk_Male_HTH_Hobble_Run);
			 }
		}

		public override void Initialize()
		{
			ExecutionType = AbilityUseType.ZigZagPathing;
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 15;
			Range = 23;
			Priority = AbilityPriority.Low;
			PreCastConditions = (AbilityConditions.CheckPlayerIncapacitated);
			UseageType=AbilityUseage.Anywhere;

			UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 2);
			ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
			TargetUnitConditionFlags = new UnitTargetConditions
			{
				TrueConditionFlags = TargetProperties.RareElite | TargetProperties.Unique,
				Distance = 15,
			};

			Fcriteria = new Func<bool>(() =>
			{
				bool isChanneling = (this.IsHobbling || Bot.Class.AbilityLastUseMS(SNOPower.Monk_TempestRush) < 350);
				int channelingCost = Bot.Class.RuneIndexCache[Power] == 3 ? 8 : 10;

				//If channeling, check if energy is greater then 10.. else only start when energy is at least -40-
				return (isChanneling && Bot.Character.dCurrentEnergy > channelingCost) || (Bot.Character.dCurrentEnergy > 40)
							 &&(!Bot.Class.bWaitingForSpecial||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount);
			});
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
			get { return SNOPower.Monk_TempestRush; }
		}
	}
}
