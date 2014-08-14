using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Navigation.Clustering;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Wizard
{
	public class Teleport : Skill
	{
		public override double Cooldown { get { return 16000; } }

		public override bool IsMovementSkill { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		private readonly ClusterConditions combatClusterCondition = new ClusterConditions(5d, 48f, 2, false);

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.ZigZagPathing; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 1, true);
			Cost = 15;
			Range = 50;
		

			Priority = SkillPriority.High;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast |
									  SkillPrecastFlags.CheckEnergy));
			ClusterConditions.Add(new SkillClusterConditions(5d, 48f, 2, false));
			//TestCustomCombatConditionAlways=true,
			FcriteriaCombat = () => ((FunkyBaseExtension.Settings.Wizard.bTeleportFleeWhenLowHP && FunkyGame.Hero.dCurrentHealthPct < 0.5d)
								   ||
								   (FunkyBaseExtension.Settings.Wizard.bTeleportIntoGrouping &&
									FunkyGame.Targeting.Cache.Clusters.AbilityClusterCache(combatClusterCondition).Count > 0 &&
									FunkyGame.Targeting.Cache.Clusters.AbilityClusterCache(combatClusterCondition)[0].Midpoint.Distance(FunkyGame.Hero.Position) > 15f)
								   || (!FunkyBaseExtension.Settings.Wizard.bTeleportFleeWhenLowHP && !FunkyBaseExtension.Settings.Wizard.bTeleportIntoGrouping));
			FCombatMovement = v =>
			{
				float fDistanceFromTarget = FunkyGame.Hero.Position.Distance(v);
				if (!FunkyGame.Hero.Class.bWaitingForSpecial && FunkyBaseExtension.Difference(FunkyGame.Hero.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 25f)
				{
					if (fDistanceFromTarget > 50f)
						return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, 50f);
					return v;
				}

				return Vector3.Zero;
			};
			FOutOfCombatMovement = v =>
			{
				float fDistanceFromTarget = FunkyGame.Hero.Position.Distance(v);
				if (FunkyBaseExtension.Difference(FunkyGame.Hero.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 10f)
				{
					if (fDistanceFromTarget > 50f)
						return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, 50f);
					return v;
				}

				return Vector3.Zero;
			};
		}

		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Teleport; }
		}
	}
}
