using FunkyBot.Movement.Clustering;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	public class DashingStrike : Skill
	{
		public override double Cooldown { get { return 1000; } }

		public override bool IsMovementSkill { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Location; } }

		public override void Initialize()
		{
			Range = 40;
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated);
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Ranged, mindistance: 30));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, mindistance: 30, falseConditionalFlags: TargetProperties.Normal));

			//SingleUnitCondition.Add(
			//	new UnitTargetConditions
			//	{
			//		TrueConditionFlags=TargetProperties.TargetableAndAttackable,
			//		MinimumDistance=30,
			//		Criteria = () => (ObjectCache.Obstacles.ReturnAllIntersectingObjects(Bot.Character.Data.Position, Bot.Targeting.Cache.CurrentTarget.Position, ObstacleType.Monster).Count > 2)
			//	}
			//);

			//Rainments of Thousand Storms Full Set Bonus (Explosion of lightning)
			if (Bot.Settings.Monk.RainmentsOfThousandStormsFiveBonus)
			{
				//Boss!
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, MinimumHealthPercent: 0.95d));
				//Clusters that are at least 15 yards away
				ClusterConditions.Add(new SkillClusterConditions(6d, 40f, 5, true, minDistance: 15));
				//Clusters of Non-Normal Units
				ClusterConditions.Add(new SkillClusterConditions(6d, 40f, 2, true, clusterflags: ClusterProperties.Elites));
			}

			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial;


			FCombatMovement = (v) =>
			{
				float fDistanceFromTarget = Bot.Character.Data.Position.Distance(v);
				if (!Bot.Character.Class.bWaitingForSpecial && Funky.Difference(Bot.Character.Data.Position.Z, v.Z) <= 4)
				{
					if (fDistanceFromTarget > 50f)
						return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 50f);

					return v;
				}

				return Vector3.Zero;
			};
			FOutOfCombatMovement = (v) =>
			{
				float fDistanceFromTarget = Bot.Character.Data.Position.Distance(v);
				if (Funky.Difference(Bot.Character.Data.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
				{
					if (fDistanceFromTarget > 50f)
						return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 50f);
					else
						return v;
				}

				return Vector3.Zero;
			};
		}

		public override SNOPower Power
		{
			get { return SNOPower.X1_Monk_DashingStrike; }
		}
	}
}
