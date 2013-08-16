using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Monk
{
	public class BreathofHeaven : Ability, IAbility
	{
		public BreathofHeaven() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityUseType.Buff;
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 25;
			UseageType = AbilityUseage.Anywhere;
			IsBuff = true;
			Priority = AbilityPriority.High;
			PreCastConditions = (AbilityConditions.CheckEnergy | AbilityConditions.CheckCanCast |
			                     AbilityConditions.CheckRecastTimer);
			Fbuff = new Func<bool>(() => { return !Bot.Class.HasBuff(SNOPower.Monk_BreathOfHeaven); });
			Fcriteria =
				new Func<bool>(() => { return (Bot.Character.dCurrentHealthPct<=0.5||!Bot.Class.HasBuff(SNOPower.Monk_BreathOfHeaven)); });
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
			get { return SNOPower.Monk_BreathOfHeaven; }
		}
	}
}
