using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Barb
{
	public class Leap : Ability, IAbility
	{
		public Leap() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_Leap; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(2, 2, true);
			ExecutionType = AbilityUseType.ClusterLocation | AbilityUseType.Location;
			Range = 35;
			Priority = AbilityPriority.Low;
			UseageType=AbilityUseage.Combat;
			IsNavigationSpecial = true;
			PreCastConditions = (AbilityConditions.CheckPlayerIncapacitated | AbilityConditions.CheckRecastTimer |
			                     AbilityConditions.CheckCanCast);
			ClusterConditions = new ClusterConditions(5d, 30, 2, true);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial,
				falseConditionalFlags: TargetProperties.Fast, MinimumDistance: 30);

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
