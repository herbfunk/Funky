using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Barbarian
{
	public class FuriousCharge : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_FuriousCharge; } }


		public override double Cooldown { get { return 15000; } }

        public override bool IsRanged { get { return true; } }
		public override bool IsMovementSkill { get { return true; } }
        public override bool IsPiercing { get { return true; } }

		private readonly WaitLoops _waitVars = new WaitLoops(1, 2, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Priority = SkillPriority.Medium;
			Range = 60;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated);

            ClusterConditions.Add(new SkillClusterConditions(4d, Range, 3, false, useRadiusDistance: true));

		    var IntersectingUnitTargetConditions =
		        new UnitTargetConditions
		        {
		            TrueConditionFlags = TargetProperties.None,
		            MaximumDistance = Range,
                    Criteria = (unit) => unit.TargetInfo.IntersectingUnits>2
		        };

		    SingleUnitCondition.Add(IntersectingUnitTargetConditions);
            SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, Range, falseConditionalFlags: TargetProperties.Normal));
            SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Summoner, Range, falseConditionalFlags: TargetProperties.LowHealth));
            SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.AvoidanceSummoner, Range, falseConditionalFlags: TargetProperties.LowHealth));
            SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Debuffing, Range, falseConditionalFlags: TargetProperties.LowHealth));

			FCombatMovement = v =>
			{
				float fDistanceFromTarget = FunkyGame.Hero.Position.Distance(v);
				if (FunkyBaseExtension.Settings.General.OutOfCombatMovement && !FunkyGame.Hero.Class.bWaitingForSpecial && FunkyBaseExtension.Difference(FunkyGame.Hero.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
				{
                    if (fDistanceFromTarget > Range)
                        return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, Range);
					return v;
				}

				return Vector3.Zero;
			};
			FOutOfCombatMovement = v =>
			{
				float fDistanceFromTarget = FunkyGame.Hero.Position.Distance(v);
				if (FunkyBaseExtension.Difference(FunkyGame.Hero.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
				{
                    if (fDistanceFromTarget > Range)
                        return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, Range);
					return v;
				}

				return Vector3.Zero;
			};
		}

	}
}
