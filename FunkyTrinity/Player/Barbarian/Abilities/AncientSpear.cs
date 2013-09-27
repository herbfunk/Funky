using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Barb
{
	public class AncientSpear : ability, IAbility
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
			ExecutionType = AbilityExecuteFlags.Target;
			WaitVars = new WaitLoops(2, 2, true);
			Range = 35;
			IsRanged = true;
			IsProjectile=true;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckRecastTimer | AbilityPreCastFlags.CheckCanCast |
			                     AbilityPreCastFlags.CheckPlayerIncapacitated);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.Ranged, 25, 0.50d);
								
								//TestCustomCombatConditionAlways=true,
			FcriteriaCombat = new Func<bool>(() =>
			{
				return Bot.Target.CurrentUnitTarget.Monstersize.Value == Zeta.Internals.SNO.MonsterSize.Ranged ||
				       Bot.Character.dCurrentEnergyPct < 0.5d;
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
				  ability p=(ability)obj;
				  return this.Power==p.Power;
			 }
		}

		#endregion
	}
}
