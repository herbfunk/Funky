using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Monk
{
	public class MysticAlly : Ability, IAbility
	{
		public MysticAlly() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = PowerExecutionTypes.Buff;
			WaitVars = new WaitLoops(2, 2, true);
			Cost = 25;
			UseFlagsType=AbilityUseFlags.Anywhere;
			IsBuff=true;
			Priority = AbilityPriority.High;
			IsSpecialAbility = true;
			Counter = 1;
			PreCastConditions = (CastingConditionTypes.CheckEnergy | CastingConditionTypes.CheckCanCast | CastingConditionTypes.CheckPetCount);
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
			get { return SNOPower.Monk_MysticAlly; }
		}
	}
}
