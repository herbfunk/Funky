using System;
using FunkyTrinity.Cache;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Barb
{
	public class Warcry : Ability, IAbility
	{
		public Warcry() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_WarCry; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		public override void Initialize()
		{
			ExecutionType = PowerExecutionTypes.Buff;
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 0;
			Range = 0;
			IsBuff=true;
			UseFlagsType = AbilityUseFlags.Anywhere;
			Priority = AbilityPriority.High;
			PreCastConditions = (CastingConditionTypes.CheckCanCast | CastingConditionTypes.CheckPlayerIncapacitated);
			Fbuff = new Func<bool>(() => { return !Bot.Class.HasBuff(SNOPower.Barbarian_WarCry); });
			Fcriteria = new Func<bool>(() =>
			{
				return (!Bot.Class.HasBuff(SNOPower.Barbarian_WarCry)
				        ||
				        (Bot.Class.PassivePowers.Contains(SNOPower.Barbarian_Passive_InspiringPresence) &&
				         DateTime.Now.Subtract(this.LastUsed).TotalSeconds > 59
				         || Bot.Character.dCurrentEnergyPct < 0.10));
			});

		}

		#region IAbility
		public override int GetHashCode()
		{
			 return (int)this.Power;
		}
		public override bool Equals(object obj)
		{
			 //Check for null and compare run-time types. 
			 if (obj==null||this.GetType()!=obj.GetType())
			 {
					return false;
			 }
			 else
			 {
					Ability p=(Ability)obj;
					return this.Power==p.Power;
			 }
		}
	

		#endregion
	}
}
