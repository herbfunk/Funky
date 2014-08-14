using System;
using fBaseXtensions.Cache.Internal;
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
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Boss))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.IsBoss;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Burrowing))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.IsBurrowableUnit;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.FullHealth))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.CurrentHealthPct.Value == 1d;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Weak))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.UnitMaxHitPointAverageWeight < 0;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.MissileDampening))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterMissileDampening;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.RareElite))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.IsEliteRareUnique;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.MissileReflecting))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.IsMissileReflecting && FunkyGame.Targeting.Cache.CurrentTarget.AnimState == AnimationState.Transform;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Shielding))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterShielding;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Stealthable))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.IsStealthableUnit;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.SucideBomber))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.IsSucideBomber;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.TreasureGoblin))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.IsTreasureGoblin;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Unique))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterUnique;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Ranged))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.IsRanged;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.TargetableAndAttackable))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.IsTargetableAndAttackable;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Fast))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.IsFast;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.DOTDPS))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.HasDOTdps.HasValue && FunkyGame.Targeting.Cache.CurrentUnitTarget.HasDOTdps.Value;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.CloseDistance))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.RadiusDistance < 10f;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.ReflectsDamage))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterReflectDamage;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Electrified))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterElectrified;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Normal))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterNormal;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.LowHealth))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.CurrentHealthPct.HasValue && FunkyGame.Targeting.Cache.CurrentUnitTarget.CurrentHealthPct.Value < 0.25d;
			}

			//FALSE CONDITIONS
			if (FalseConditionFlags.Equals(TargetProperties.None))
				Criteria += () => true;
			else
			{
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Boss))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentTarget.IsBoss;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Burrowing))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentTarget.IsBurrowableUnit;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.FullHealth))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.CurrentHealthPct.Value != 1d;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Weak))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentUnitTarget.UnitMaxHitPointAverageWeight > 0;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.MissileDampening))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterMissileDampening;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.RareElite))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.IsEliteRareUnique;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.MissileReflecting))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentTarget.IsMissileReflecting || FunkyGame.Targeting.Cache.CurrentTarget.AnimState != AnimationState.Transform;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Shielding))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterShielding;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Stealthable))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentTarget.IsStealthableUnit;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.SucideBomber))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentTarget.IsSucideBomber;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.TreasureGoblin))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentTarget.IsTreasureGoblin;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Unique))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterUnique;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Ranged))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.IsRanged;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.TargetableAndAttackable))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.IsTargetableAndAttackable;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Fast))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.IsFast;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.DOTDPS))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.HasDOTdps.HasValue || !FunkyGame.Targeting.Cache.CurrentUnitTarget.HasDOTdps.Value;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.CloseDistance))
					Criteria += () => FunkyGame.Targeting.Cache.CurrentTarget.RadiusDistance > 10f;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.ReflectsDamage))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterReflectDamage;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Electrified))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterElectrified;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Normal))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterNormal;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.LowHealth))
					Criteria += () => !FunkyGame.Targeting.Cache.CurrentUnitTarget.CurrentHealthPct.HasValue || FunkyGame.Targeting.Cache.CurrentUnitTarget.CurrentHealthPct.Value >= 0.25d;
			}
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