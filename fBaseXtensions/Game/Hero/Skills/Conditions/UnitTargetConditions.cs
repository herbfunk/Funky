using System;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Objects;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Logger = fBaseXtensions.Helpers.Logger;

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

		public UnitTargetConditions(Func<CacheUnit,bool> criteria)
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
		public Func<CacheUnit, bool> Criteria { get; set; } 


		private void CreateCriteria()
		{
		    Criteria += unit =>
		    {
		        if (unit == null)
		        {
		            Logger.DBLog.DebugFormat("[Funky] Recieved null CacheUnit value in skill criteria evaluation");
		            return true;
		        }

		        return true;
		    };
			//Distance
			if (MaximumDistance > -1)
                Criteria += (unit) => unit.TargetInfo.RadiusDistance <= MaximumDistance;
			if (MinimumDistance > -1)
                Criteria += (unit) => unit.TargetInfo.RadiusDistance >= MinimumDistance;

			//Health
			if (HealthPercent > 0d)
                Criteria += (unit) => unit.CurrentHealthPct.HasValue && unit.CurrentHealthPct.Value <= HealthPercent;


			//TRUE CONDITIONS
			if (TrueConditionFlags.Equals(TargetProperties.None))
				Criteria += (unit) => true;
			else
			{
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Boss))
					Criteria += (unit) => unit.IsBoss;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Burrowing))
					Criteria += (unit) => unit.IsBurrowableUnit;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.FullHealth))
                    Criteria += (unit) => unit.CurrentHealthPct.HasValue &&
                                      unit.CurrentHealthPct.Value == 1d;

				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Weak))
					Criteria += (unit) => unit.UnitMaxHitPointAverageWeight < 0;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.MissileDampening))
					Criteria += (unit) => unit.MonsterMissileDampening;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.RareElite))
					Criteria += (unit) => unit.IsEliteRareUnique;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.MissileReflecting))
					Criteria += (unit) => unit.IsMissileReflecting && unit.AnimState == AnimationState.Transform;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Shielding))
					Criteria += (unit) => unit.MonsterShielding;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Stealthable))
					Criteria += (unit) => unit.IsStealthableUnit;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.SucideBomber))
					Criteria += (unit) => unit.IsSucideBomber;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.TreasureGoblin))
					Criteria += (unit) => unit.IsTreasureGoblin;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Unique))
					Criteria += (unit) => unit.MonsterUnique;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Ranged))
					Criteria += (unit) => unit.IsRanged;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.TargetableAndAttackable))
					Criteria += (unit) => unit.IsTargetableAndAttackable;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Fast))
					Criteria += (unit) => unit.IsFast;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.DOTDPS))
					Criteria += (unit) => unit.HasDOTdps.HasValue && unit.HasDOTdps.Value;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.CloseDistance))
					Criteria += (unit) => unit.TargetInfo.RadiusDistance < 10f;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.ReflectsDamage))
					Criteria += (unit) => unit.MonsterReflectDamage;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Electrified))
					Criteria += (unit) => unit.MonsterElectrified;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Normal))
					Criteria += (unit) => unit.MonsterNormal;
				if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.LowHealth))
					Criteria += (unit) => unit.CurrentHealthPct.HasValue && unit.CurrentHealthPct.Value < 0.25d;


			    if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Flying))
			        Criteria +=
                        (unit) =>
			                unit.UnitPropertyFlags.HasValue &&
                            ObjectCache.CheckFlag(unit.UnitPropertyFlags.Value,UnitFlags.Flying);

                if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Summoner))
                    Criteria +=
                        (unit) =>
                            unit.UnitPropertyFlags.HasValue &&
                            ObjectCache.CheckFlag(unit.UnitPropertyFlags.Value, UnitFlags.Summoner);

                if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.AvoidanceSummoner))
                    Criteria +=
                        (unit) =>
                            unit.UnitPropertyFlags.HasValue &&
                            ObjectCache.CheckFlag(unit.UnitPropertyFlags.Value, UnitFlags.AvoidanceSummoner);

                if (ObjectCache.CheckFlag(TrueConditionFlags, TargetProperties.Debuffing))
                    Criteria +=
                        (unit) =>
                            unit.UnitPropertyFlags.HasValue &&
                            ObjectCache.CheckFlag(unit.UnitPropertyFlags.Value, UnitFlags.Debuffing);
			}

			//FALSE CONDITIONS
			if (FalseConditionFlags.Equals(TargetProperties.None))
				Criteria += (unit) => true;
			else
			{
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Boss))
					Criteria += (unit) => !unit.IsBoss;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Burrowing))
					Criteria += (unit) => !unit.IsBurrowableUnit;

				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.FullHealth))
                    Criteria += (unit) => unit.CurrentHealthPct.HasValue &&
                                      unit.CurrentHealthPct.Value < 1d;

				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Weak))
					Criteria += (unit) => unit.UnitMaxHitPointAverageWeight > 0;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.MissileDampening))
					Criteria += (unit) => !unit.MonsterMissileDampening;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.RareElite))
					Criteria += (unit) => !unit.IsEliteRareUnique;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.MissileReflecting))
					Criteria += (unit) => !unit.IsMissileReflecting || unit.AnimState != AnimationState.Transform;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Shielding))
					Criteria += (unit) => !unit.MonsterShielding;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Stealthable))
					Criteria += (unit) => !unit.IsStealthableUnit;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.SucideBomber))
					Criteria += (unit) => !unit.IsSucideBomber;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.TreasureGoblin))
					Criteria += (unit) => !unit.IsTreasureGoblin;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Unique))
					Criteria += (unit) => !unit.MonsterUnique;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Ranged))
					Criteria += (unit) => !unit.IsRanged;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.TargetableAndAttackable))
					Criteria += (unit) => !unit.IsTargetableAndAttackable;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Fast))
					Criteria += (unit) => !unit.IsFast;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.DOTDPS))
					Criteria += (unit) => !unit.HasDOTdps.HasValue || !unit.HasDOTdps.Value;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.CloseDistance))
                    Criteria += (unit) => unit.TargetInfo.RadiusDistance > 10f;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.ReflectsDamage))
					Criteria += (unit) => !unit.MonsterReflectDamage;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Electrified))
					Criteria += (unit) => !unit.MonsterElectrified;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Normal))
					Criteria += (unit) => !unit.MonsterNormal;
				if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.LowHealth))
					Criteria += (unit) => !unit.CurrentHealthPct.HasValue || unit.CurrentHealthPct.Value >= 0.25d;


                if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Flying))
                    Criteria +=
                        (unit) =>
                            unit.UnitPropertyFlags.HasValue &&
                            !ObjectCache.CheckFlag(unit.UnitPropertyFlags.Value, UnitFlags.Flying);

                if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Summoner))
                    Criteria +=
                        (unit) =>
                            unit.UnitPropertyFlags.HasValue &&
                            !ObjectCache.CheckFlag(unit.UnitPropertyFlags.Value, UnitFlags.Summoner);

                if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.AvoidanceSummoner))
                    Criteria +=
                        (unit) =>
                            unit.UnitPropertyFlags.HasValue &&
                            !ObjectCache.CheckFlag(unit.UnitPropertyFlags.Value, UnitFlags.AvoidanceSummoner);

                if (ObjectCache.CheckFlag(FalseConditionFlags, TargetProperties.Debuffing))
                    Criteria +=
                        (unit) =>
                            unit.UnitPropertyFlags.HasValue &&
                            !ObjectCache.CheckFlag(unit.UnitPropertyFlags.Value, UnitFlags.Debuffing);
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