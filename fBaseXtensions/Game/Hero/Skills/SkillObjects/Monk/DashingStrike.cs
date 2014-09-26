using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using fBaseXtensions.Navigation.Clustering;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	public class DashingStrike : Skill
	{
		public override double Cooldown { get { return _cooldown; } set { _cooldown = value; } }
		private double _cooldown = 750;

		public override bool IsMovementSkill { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return _executiontype; } set { _executiontype = value; } }
		private SkillExecutionFlags _executiontype = SkillExecutionFlags.Location | SkillExecutionFlags.ClusterLocation;

		public override void Initialize()
		{
			Range = 40;
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckRecastTimer);
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Ranged, mindistance: 30));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, mindistance: 30, falseConditionalFlags: TargetProperties.Normal));
			
			if (Equipment.CheckLegendaryItemCount(LegendaryItemTypes.Jawbreaker))
			{
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, mindistance: 35));
				ClusterConditions.Add(new SkillClusterConditions(7d, 50f, 1, true, 0, ClusterProperties.None, 35f, false));
			}
			
			//Rainments of Thousand Storms Full Set Bonus (Explosion of lightning)
			if (Equipment.CheckLegendaryItemCount(LegendaryItemTypes.RaimentofaThousandStorms,6))
			{
				//Boss!
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, MinimumHealthPercent: 0.95d));
				//Clusters that are at least 15 yards away
				ClusterConditions.Add(new SkillClusterConditions(6d, 40f, 7, true));
				//Clusters of Non-Normal Units
				ClusterConditions.Add(new SkillClusterConditions(6d, 40f, 2, true, clusterflags: ClusterProperties.Elites));
			}

			//Sunwoko CDR Build
			if (Equipment.CheckLegendaryItemCount(LegendaryItemTypes.MonkeyKingsGarb, 4) && RuneIndex==4)
			{
				//ExecutionType=SkillExecutionFlags.Target | SkillExecutionFlags.ClusterTargetNearest;
				Cooldown = 4000;

				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None));
			}

			FCombatMovement = (v) =>
			{
				float fDistanceFromTarget = FunkyGame.Hero.Position.Distance(v);
				if (FunkyBaseExtension.Difference(FunkyGame.Hero.Position.Z, v.Z) <= 4)
				{
					if (fDistanceFromTarget > 50f)
						return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, 50f);

					return v;
				}

				return Vector3.Zero;
			};
			FOutOfCombatMovement = (v) =>
			{
				float fDistanceFromTarget = FunkyGame.Hero.Position.Distance(v);
				if (FunkyBaseExtension.Difference(FunkyGame.Hero.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
				{
					if (fDistanceFromTarget > 50f)
						return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, 50f);
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
