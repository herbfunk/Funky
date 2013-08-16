using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Barb
{
	public class Battlerage : Ability, IAbility
	{
		public Battlerage() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_BattleRage; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		protected override void Initialize()
		{
			ExecutionType = AbilityUseType.Buff;
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 20;
			UseageType = AbilityUseage.Anywhere;
			Priority = AbilityPriority.High;
			PreCastConditions = (AbilityConditions.CheckEnergy | AbilityConditions.CheckPlayerIncapacitated);
			Fbuff = new Func<bool>(() => { return !Bot.Class.HasBuff(SNOPower.Barbarian_BattleRage); });
			Fcriteria = new Func<bool>(() =>
			{
				 return !Bot.Class.HasBuff(SNOPower.Barbarian_BattleRage)||
				       //Only if we cannot spam sprint..
							 (!Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_Sprint)&&
				        ((Bot.SettingsFunky.Class.bFuryDumpWrath && Bot.Character.dCurrentEnergyPct >= 0.98 &&
									Bot.Class.HasBuff(SNOPower.Barbarian_WrathOfTheBerserker))||
				         (Bot.SettingsFunky.Class.bFuryDumpAlways && Bot.Character.dCurrentEnergyPct >= 0.98)));
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
