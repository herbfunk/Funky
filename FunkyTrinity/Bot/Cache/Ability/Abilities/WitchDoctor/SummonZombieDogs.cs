using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.WitchDoctor
{
	public class SummonZombieDogs : Ability, IAbility
	{
		public SummonZombieDogs() : base()
		{
		}


		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }


		public override void Initialize()
		{
			ExecutionType = PowerExecutionTypes.Buff;
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 49;
			UseFlagsType=AbilityUseFlags.Anywhere;
			Priority = AbilityPriority.High;
			PreCastConditions = (CastingConditionTypes.CheckCanCast | CastingConditionTypes.CheckEnergy);
			IsBuff=true;
			 Fbuff =
				new Func<bool>(
					() =>
					{
						return Bot.Character.PetData.ZombieDogs <
						       (Bot.Class.PassivePowers.Contains(SNOPower.Witchdoctor_Passive_ZombieHandler) ? 4 : 3);
					});
			Fcriteria = new Func<bool>(() =>
			{
				return Bot.Character.PetData.ZombieDogs <
				       (Bot.Class.PassivePowers.Contains(SNOPower.Witchdoctor_Passive_ZombieHandler) ? 4 : 3);
			});
		}

		#region IAbility


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
			get { return SNOPower.Witchdoctor_SummonZombieDog; }
		}
	}
}
