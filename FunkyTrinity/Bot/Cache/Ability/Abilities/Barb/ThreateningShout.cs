using System;
using FunkyTrinity.Enums;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Barb
{
	public class ThreateningShout : Ability, IAbility
	{
		public ThreateningShout() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_ThreateningShout; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		public override void Initialize()
		{
			ExecutionType = AbilityUseType.Self;
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 20;
			UseageType=AbilityUseage.Anywhere;
			Priority = AbilityPriority.Low;
			PreCastConditions = (AbilityConditions.CheckRecastTimer | AbilityConditions.CheckEnergy |
			                     AbilityConditions.CheckCanCast | AbilityConditions.CheckPlayerIncapacitated);
			Fcriteria = new Func<bool>(() =>
			{
				return (
					Bot.Combat.iElitesWithinRange[(int) RangeIntervals.Range_20] > 1 ||
					(Bot.Target.CurrentTarget.IsBoss && Bot.Target.CurrentTarget.RadiusDistance <= 20) ||
					(Bot.Combat.iAnythingWithinRange[(int) RangeIntervals.Range_20] > 2 && !Bot.Combat.bAnyBossesInRange &&
					 (Bot.Combat.iElitesWithinRange[(int) RangeIntervals.Range_50] == 0 ||
					  Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_SeismicSlam))) ||
					Bot.Character.dCurrentHealthPct <= 0.75
					);
			});
		}
		public override void InitCriteria()
		{
			 base.AbilityTestConditions=new AbilityUsablityTests(this);
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
