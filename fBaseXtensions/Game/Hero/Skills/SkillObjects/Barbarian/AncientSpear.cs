using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Barbarian
{
	public class AncientSpear : Skill
	{
		public override SNOPower Power { get { return SNOPower.X1_Barbarian_AncientSpear; } }

		public override double Cooldown { get { return 300; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		private readonly WaitLoops _waitVars = new WaitLoops(2, 2, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Priority = SkillPriority.Medium;
			Range = 60;
			PreCast=new SkillPreCast((SkillPrecastFlags.CheckCanCast |SkillPrecastFlags.CheckPlayerIncapacitated));

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
			//SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Ranged, Range, MinimumHealthPercent: 0.50d));
            SingleUnitCondition.Add(new UnitTargetConditions
            {
                Criteria = (unit) => FunkyGame.Hero.dCurrentEnergyPct > 0.80d,
                MaximumDistance = Range,
                FalseConditionFlags = TargetProperties.LowHealth,
            });
		}

	}
}
