using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Barb
{
	public class WrathOfTheBerserker : Ability, IAbility
	{
		public WrathOfTheBerserker() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_WrathOfTheBerserker; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		public override void Initialize()
		{
			ExecutionType = PowerExecutionTypes.Buff;
			WaitVars = new WaitLoops(4, 4, true);
			Cost = 50;
			UseFlagsType=AbilityUseFlags.Anywhere;
			IsSpecialAbility = Bot.SettingsFunky.Class.bWaitForWrath;
			Priority = AbilityPriority.High;
			PreCastConditions = (CastingConditionTypes.CheckEnergy | CastingConditionTypes.CheckExisitingBuff |
			                     CastingConditionTypes.CheckCanCast);
			Fcriteria = new Func<bool>(() =>
			{
				return Bot.Combat.bAnyChampionsPresent
				       || (Bot.SettingsFunky.Class.bBarbUseWOTBAlways && Bot.Combat.bAnyMobsInCloseRange)
				       || (Bot.SettingsFunky.Class.bGoblinWrath && Bot.Target.CurrentTarget.IsTreasureGoblin);
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
