using System;
using FunkyTrinity.Enums;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.WitchDoctor
{
	public class Gargantuan : Ability, IAbility
	{
		public Gargantuan() : base()
		{
		}



		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		public override void Initialize()
		{
			ExecutionType = AbilityUseType.Buff;
			WaitVars = new WaitLoops(2, 1, true);
			Cost = 147;
			Counter = 1;
			UseageType=AbilityUseage.Anywhere;
			Priority = AbilityPriority.High;
			PreCastConditions = (AbilityConditions.CheckPlayerIncapacitated | AbilityConditions.CheckCanCast |
			                     AbilityConditions.CheckEnergy | AbilityConditions.CheckPetCount);
			IsBuff=true;
			 Fbuff =
				new Func<bool>(
					() =>
					{
						return Bot.Class.RuneIndexCache[SNOPower.Witchdoctor_Gargantuan] != 0 && Bot.Character.PetData.Gargantuan == 0;
					});
			Fcriteria = new Func<bool>(() =>
			{
				 return (Bot.Class.RuneIndexCache[SNOPower.Witchdoctor_Gargantuan]==0&&
				        (Bot.Combat.iElitesWithinRange[(int) RangeIntervals.Range_15] >= 1 ||
				         (Bot.Target.CurrentUnitTarget.IsEliteRareUnique && Bot.Target.CurrentTarget.RadiusDistance <= 15f))
								||Bot.Class.RuneIndexCache[SNOPower.Witchdoctor_Gargantuan]!=0&&Bot.Character.PetData.Gargantuan==0);
			});
		}

		public override void InitCriteria()
		{
			base.AbilityTestConditions = new AbilityUsablityTests(this);
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
			get { return SNOPower.Witchdoctor_Gargantuan; }
		}
	}
}
