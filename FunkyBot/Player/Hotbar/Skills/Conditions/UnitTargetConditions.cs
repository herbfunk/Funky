

using System;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Conditions
{
	///<summary>
	///Describes a condition for a single unit
	///</summary>
	public class UnitTargetConditions
	{
		public UnitTargetConditions(TargetProperties trueconditionalFlags, int MinimumDistance = -1, double MinimumHealthPercent = 0d, TargetProperties falseConditionalFlags = TargetProperties.None)
		{
			TrueConditionFlags = trueconditionalFlags;
			FalseConditionFlags = falseConditionalFlags;
			Distance = MinimumDistance;
			HealthPercent = MinimumHealthPercent;
			CreateCriteria();
		}

		public UnitTargetConditions(Func<bool> criteria)
		{
			Criteria = criteria;
		}

		//Default Constructor
		public UnitTargetConditions()
		{
			TrueConditionFlags = TargetProperties.None;
			FalseConditionFlags = TargetProperties.None;
			Distance = -1;
			HealthPercent = 0d;
		}


		public TargetProperties TrueConditionFlags { get; set; }
		public TargetProperties FalseConditionFlags { get; set; }
		public int Distance { get; set; }
		public double HealthPercent { get; set; }
		public Func<bool> Criteria { get; set; } 


		private void CreateCriteria()
		{
			//Distance
			if (Distance > -1)
				Criteria += () => Bot.Targeting.CurrentTarget.RadiusDistance <= Distance;
			//Health
			if (HealthPercent > 0d)
				Criteria += () => Bot.Targeting.CurrentUnitTarget.CurrentHealthPct.Value <= HealthPercent;


			//TRUE CONDITIONS
			if (TrueConditionFlags.Equals(TargetProperties.None))
				Criteria += () => true;
			else
			{
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Boss))
					Criteria += () => Bot.Targeting.CurrentTarget.IsBoss;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Burrowing))
					Criteria += () => Bot.Targeting.CurrentTarget.IsBurrowableUnit;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.FullHealth))
					Criteria += () => Bot.Targeting.CurrentUnitTarget.CurrentHealthPct.Value == 1d;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.IsSpecial))
					Criteria += () => Bot.Targeting.CurrentTarget.ObjectIsSpecial;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Weak))
					Criteria += () => Bot.Targeting.CurrentUnitTarget.UnitMaxHitPointAverageWeight < 0;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.MissileDampening))
					Criteria += () => Bot.Targeting.CurrentUnitTarget.MonsterMissileDampening;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.RareElite))
					Criteria += () => Bot.Targeting.CurrentUnitTarget.IsEliteRareUnique;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.MissileReflecting))
					Criteria += () => Bot.Targeting.CurrentTarget.IsMissileReflecting && Bot.Targeting.CurrentTarget.AnimState == AnimationState.Transform;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Shielding))
					Criteria += () => Bot.Targeting.CurrentUnitTarget.MonsterShielding;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Stealthable))
					Criteria += () => Bot.Targeting.CurrentTarget.IsStealthableUnit;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.SucideBomber))
					Criteria += () => Bot.Targeting.CurrentTarget.IsSucideBomber;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.TreasureGoblin))
					Criteria += () => Bot.Targeting.CurrentTarget.IsTreasureGoblin;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Unique))
					Criteria += () => Bot.Targeting.CurrentUnitTarget.MonsterUnique;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Ranged))
					Criteria += () => Bot.Targeting.CurrentUnitTarget.IsRanged;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.TargetableAndAttackable))
					Criteria += () => Bot.Targeting.CurrentUnitTarget.IsTargetableAndAttackable;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Fast))
					Criteria += () => Bot.Targeting.CurrentUnitTarget.IsFast;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.DOTDPS))
					Criteria += () => Bot.Targeting.CurrentUnitTarget.HasDOTdps.HasValue && Bot.Targeting.CurrentUnitTarget.HasDOTdps.Value;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.CloseDistance))
					Criteria += () => Bot.Targeting.CurrentTarget.RadiusDistance < 10f;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.ReflectsDamage))
					Criteria += () => Bot.Targeting.CurrentUnitTarget.MonsterReflectDamage;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Electrified))
					Criteria += () => Bot.Targeting.CurrentUnitTarget.MonsterElectrified;
			}

			//FALSE CONDITIONS
			if (FalseConditionFlags.Equals(TargetProperties.None))
				Criteria += () => true;
			else
			{
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Boss))
					Criteria += () => !Bot.Targeting.CurrentTarget.IsBoss;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Burrowing))
					Criteria += () => !Bot.Targeting.CurrentTarget.IsBurrowableUnit;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.FullHealth))
					Criteria += () => Bot.Targeting.CurrentUnitTarget.CurrentHealthPct.Value != 1d;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.IsSpecial))
					Criteria += () => !Bot.Targeting.CurrentTarget.ObjectIsSpecial;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Weak))
					Criteria += () => Bot.Targeting.CurrentUnitTarget.UnitMaxHitPointAverageWeight > 0;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.MissileDampening))
					Criteria += () => !Bot.Targeting.CurrentUnitTarget.MonsterMissileDampening;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.RareElite))
					Criteria += () => !Bot.Targeting.CurrentUnitTarget.IsEliteRareUnique;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.MissileReflecting))
					Criteria += () => !Bot.Targeting.CurrentTarget.IsMissileReflecting || Bot.Targeting.CurrentTarget.AnimState != AnimationState.Transform;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Shielding))
					Criteria += () => !Bot.Targeting.CurrentUnitTarget.MonsterShielding;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Stealthable))
					Criteria += () => !Bot.Targeting.CurrentTarget.IsStealthableUnit;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.SucideBomber))
					Criteria += () => !Bot.Targeting.CurrentTarget.IsSucideBomber;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.TreasureGoblin))
					Criteria += () => !Bot.Targeting.CurrentTarget.IsTreasureGoblin;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Unique))
					Criteria += () => !Bot.Targeting.CurrentUnitTarget.MonsterUnique;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Ranged))
					Criteria += () => !Bot.Targeting.CurrentUnitTarget.IsRanged;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.TargetableAndAttackable))
					Criteria += () => !Bot.Targeting.CurrentUnitTarget.IsTargetableAndAttackable;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Fast))
					Criteria += () => !Bot.Targeting.CurrentUnitTarget.IsFast;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.DOTDPS))
					Criteria += () => !Bot.Targeting.CurrentUnitTarget.HasDOTdps.HasValue || !Bot.Targeting.CurrentUnitTarget.HasDOTdps.Value;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.CloseDistance))
					Criteria += () => Bot.Targeting.CurrentTarget.RadiusDistance > 10f;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.ReflectsDamage))
					Criteria += () => !Bot.Targeting.CurrentUnitTarget.MonsterReflectDamage;
				if (AbilityLogicConditions.CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Electrified))
					Criteria += () => !Bot.Targeting.CurrentUnitTarget.MonsterElectrified;
			}
		}
	}
}