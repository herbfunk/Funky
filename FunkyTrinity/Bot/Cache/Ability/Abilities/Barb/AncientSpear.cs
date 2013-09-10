using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Barb
{
	public class AncientSpear : Ability, IAbility
	{
		public AncientSpear() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_AncientSpear; }
		}

		public override void Initialize()
		{
			ExecutionType = AbilityUseType.Target;
			WaitVars = new WaitLoops(2, 2, true);
			Range = 35;
			IsRanged = true;
			IsProjectile=true;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastConditions = (AbilityConditions.CheckRecastTimer | AbilityConditions.CheckCanCast |
			                     AbilityConditions.CheckPlayerIncapacitated);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.Ranged, 25, 0.50d);
								
								//TestCustomCombatConditionAlways=true,
			Fcriteria = new Func<bool>(() =>
			{
				return Bot.Target.CurrentUnitTarget.Monstersize.Value == Zeta.Internals.SNO.MonsterSize.Ranged ||
				       Bot.Character.dCurrentEnergyPct < 0.5d;
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
