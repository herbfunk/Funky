using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Barb
{
	public class HammeroftheAncients : Ability, IAbility
	{
		public HammeroftheAncients() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_HammerOfTheAncients; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		public override void Initialize()
		{
			ExecutionType = AbilityUseType.ClusterTarget | AbilityUseType.Target;
			WaitVars = new WaitLoops(1, 2, true);
			Cost = 20;
			Range=Bot.Class.RuneIndexCache[Power]==0?13:Bot.Class.RuneIndexCache[Power]==1?20:16;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastConditions = (AbilityConditions.CheckRecastTimer | AbilityConditions.CheckEnergy |
			                     AbilityConditions.CheckCanCast | AbilityConditions.CheckPlayerIncapacitated);
			ClusterConditions = new ClusterConditions(6d, 20f, 2, true);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial, 20);
			Fcriteria=new Func<bool>(() => { return !Bot.Class.bWaitingForSpecial; });
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
