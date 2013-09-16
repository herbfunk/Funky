using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Monk
{
	public class Serenity : Ability, IAbility
	{
		public Serenity() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = PowerExecutionTypes.Buff;
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 10;
			UseFlagsType=AbilityUseFlags.Anywhere;
			Priority = AbilityPriority.High;
			PreCastConditions = (CastingConditionTypes.CheckEnergy | CastingConditionTypes.CheckCanCast |
			                     CastingConditionTypes.CheckRecastTimer);

			Fcriteria = new Func<bool>(() => { return Bot.Character.dCurrentHealthPct <= 0.30d
																	&& Bot.Combat.bAnyMobsInCloseRange; });
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
			get { return SNOPower.Monk_Serenity; }
		}
	}
}
