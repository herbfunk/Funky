using System;
using fBaseXtensions.Cache.Internal.Objects;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.Conditions
{
	///<summary>
	///Describes a condition for a single unit
	///</summary>
	public class UnitTargetConditions
	{
		public UnitTargetConditions(TargetProperties trueconditionalFlags, int maxdistance = -1, int mindistance = -1, double MinimumHealthPercent = 0d, TargetProperties falseConditionalFlags = TargetProperties.None)
		{
			TrueConditionFlags = trueconditionalFlags;
			FalseConditionFlags = falseConditionalFlags;
			MaximumDistance = maxdistance;
			MinimumDistance = mindistance;
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
			MaximumDistance = -1;
			MinimumDistance = -1;
			HealthPercent = 0d;
			CreateCriteria();
		}


		public TargetProperties TrueConditionFlags { get; set; }
		public TargetProperties FalseConditionFlags { get; set; }
		public int MaximumDistance { get; set; }
		public int MinimumDistance { get; set; }
		public double HealthPercent { get; set; }
		public Func<bool> Criteria { get; set; } 


		private void CreateCriteria()
		{
			//Distance
			if (MaximumDistance > -1)
				Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.RadiusDistance <= MaximumDistance;
			if (MinimumDistance > -1)
				Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.RadiusDistance >= MinimumDistance;

			//Health
			if (HealthPercent > 0d)
				Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.CurrentHealthPct.Value <= HealthPercent;


			//TRUE CONDITIONS
			if (TrueConditionFlags.Equals(TargetProperties.None))
				Criteria += () => true;
			else
			{
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Boss))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.IsBoss;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Burrowing))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.IsBurrowableUnit;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.FullHealth))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.CurrentHealthPct.Value == 1d;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Weak))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.UnitMaxHitPointAverageWeight < 0;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.MissileDampening))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterMissileDampening;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.RareElite))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.IsEliteRareUnique;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.MissileReflecting))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.IsMissileReflecting && FunkyGame.Targeting.Cache.CurrentTarget.AnimState == AnimationState.Transform;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Shielding))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterShielding;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Stealthable))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.IsStealthableUnit;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.SucideBomber))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.IsSucideBomber;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.TreasureGoblin))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.IsTreasureGoblin;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Unique))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterUnique;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Ranged))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.IsRanged;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.TargetableAndAttackable))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.IsTargetableAndAttackable;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Fast))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.IsFast;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.DOTDPS))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.HasDOTdps.HasValue && FunkyGame.Targeting.Cache.CurrentUnitTarget.HasDOTdps.Value;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.CloseDistance))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.RadiusDistance < 10f;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.ReflectsDamage))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterReflectDamage;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Electrified))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterElectrified;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.Normal))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterNormal;
				if (CheckTargetPropertyFlag(TrueConditionFlags, TargetProperties.LowHealth))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.CurrentHealthPct.HasValue && FunkyGame.Targeting.Cache.CurrentUnitTarget.CurrentHealthPct.Value < 0.25d;
			}

			//FALSE CONDITIONS
			if (FalseConditionFlags.Equals(TargetProperties.None))
				Criteria += () => true;
			else
			{
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Boss))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentTarget.IsBoss;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Burrowing))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentTarget.IsBurrowableUnit;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.FullHealth))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.CurrentHealthPct.Value != 1d;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Weak))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.UnitMaxHitPointAverageWeight > 0;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.MissileDampening))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterMissileDampening;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.RareElite))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.IsEliteRareUnique;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.MissileReflecting))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentTarget.IsMissileReflecting || FunkyGame.Targeting.Cache.CurrentTarget.AnimState != AnimationState.Transform;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Shielding))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterShielding;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Stealthable))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentTarget.IsStealthableUnit;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.SucideBomber))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentTarget.IsSucideBomber;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.TreasureGoblin))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentTarget.IsTreasureGoblin;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Unique))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterUnique;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Ranged))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.IsRanged;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.TargetableAndAttackable))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.IsTargetableAndAttackable;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Fast))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.IsFast;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.DOTDPS))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.HasDOTdps.HasValue || !FunkyGame.Targeting.Cache.CurrentUnitTarget.HasDOTdps.Value;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.CloseDistance))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.RadiusDistance > 10f;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.ReflectsDamage))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterReflectDamage;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Electrified))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterElectrified;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.Normal))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterNormal;
				if (CheckTargetPropertyFlag(FalseConditionFlags, TargetProperties.LowHealth))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.CurrentHealthPct.HasValue || FunkyGame.Targeting.Cache.CurrentUnitTarget.CurrentHealthPct.Value >= 0.25d;
			}
		}

		internal static bool CheckTargetPropertyFlag(TargetProperties property, TargetProperties flag)
		{
			return (property & flag) != 0;
		}

		internal static TargetProperties EvaluateUnitProperties(CacheUnit unit)
		{
			TargetProperties properties = TargetProperties.None;

			if (unit.IsBoss)
				properties |= TargetProperties.Boss;

			if (unit.IsBurrowableUnit)
				properties |= TargetProperties.Burrowing;

			if (unit.MonsterMissileDampening)
				properties |= TargetProperties.MissileDampening;

			if (unit.IsMissileReflecting)
				properties |= TargetProperties.MissileReflecting;

			if (unit.MonsterShielding)
				properties |= TargetProperties.Shielding;

			if (unit.IsStealthableUnit)
				properties |= TargetProperties.Stealthable;

			if (unit.IsSucideBomber)
				properties |= TargetProperties.SucideBomber;

			if (unit.IsTreasureGoblin)
				properties |= TargetProperties.TreasureGoblin;

			if (unit.IsFast)
				properties |= TargetProperties.Fast;



			if (unit.IsEliteRareUnique)
				properties |= TargetProperties.RareElite;

			if (unit.MonsterUnique)
				properties |= TargetProperties.Unique;


			if (unit.CurrentHealthPct.HasValue && unit.CurrentHealthPct.Value == 1d)
				properties |= TargetProperties.FullHealth;

			if (unit.UnitMaxHitPointAverageWeight < 0)
				properties |= TargetProperties.Weak;


			if (unit.IsRanged)
				properties |= TargetProperties.Ranged;


			if (unit.IsTargetableAndAttackable)
				properties |= TargetProperties.TargetableAndAttackable;


			if (unit.HasDOTdps.HasValue && unit.HasDOTdps.Value)
				properties |= TargetProperties.DOTDPS;

			if (unit.RadiusDistance < 10f)
				properties |= TargetProperties.CloseDistance;

			if (unit.MonsterReflectDamage)
				properties |= TargetProperties.ReflectsDamage;

			if (unit.MonsterElectrified)
				properties |= TargetProperties.Electrified;

			if (unit.MonsterNormal)
				properties |= TargetProperties.Normal;

			if (unit.CurrentHealthPct.HasValue && unit.CurrentHealthPct.Value < 0.25d)
				properties |= TargetProperties.LowHealth;

			return properties;
		}
	}
}