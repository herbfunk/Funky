using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.DemonHunter
{
	public class Impale : ability, IAbility
	{
		public Impale() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Target;
			WaitVars = new WaitLoops(0, 1, true);
			Cost = 25;
			Range = 12;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.None, 12);


			Fcriteria = new Func<bool>(() =>
			{
				return ((Bot.Character.dCurrentEnergy >= 25 && !Bot.Character.bWaitingForReserveEnergy) ||
				        Bot.Character.dCurrentEnergy >= Bot.Class.iWaitingReservedAmount);
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
			get { return SNOPower.DemonHunter_Impale; }
		}
	}
}
