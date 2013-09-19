using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.WitchDoctor
{
	public class SummonZombieDogs : ability, IAbility
	{
		public SummonZombieDogs() : base()
		{
		}


		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }


		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Buff;
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 49;
			UseageType=AbilityUseage.Anywhere;
			Priority = AbilityPriority.High;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckCanCast | AbilityPreCastFlags.CheckEnergy);
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
				ability p = (ability) obj;
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
