using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Barbarian
{
	public class Leap : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_Leap; } }

		public override double Cooldown { get { return 15000; } }

		public override bool IsMovementSkill { get { return true; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.Location; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Range = 45;
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);

            ClusterConditions.Add(new SkillClusterConditions(6d, Range, 2, true, useRadiusDistance: true));
            SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, falseConditionalFlags: TargetProperties.Normal));
            SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, falseConditionalFlags: TargetProperties.LowHealth));

			FCombatMovement = (v) =>
			{
				float fDistanceFromTarget = FunkyGame.Hero.Position.Distance(v);
				if (!FunkyGame.Hero.Class.bWaitingForSpecial && FunkyBaseExtension.Difference(FunkyGame.Hero.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
				{
                    if (fDistanceFromTarget > Range)
                        return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, Range);
					else
						return v;
				}

				return Vector3.Zero;
			};
			FOutOfCombatMovement = (v) =>
			{
				float fDistanceFromTarget = FunkyGame.Hero.Position.Distance(v);
				if (FunkyBaseExtension.Difference(FunkyGame.Hero.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
				{
                    if (fDistanceFromTarget > Range)
                        return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, Range);
					else
						return v;
				}

				return Vector3.Zero;
			};

		}

	}
}
