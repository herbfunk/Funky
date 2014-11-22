using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Barbarian
{
	public class Overpower : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_Overpower; } }


		public override double Cooldown { get { return 12000; } }

		private readonly WaitLoops _waitVars = new WaitLoops(0, 4, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Self; } }

		public override SkillUseage UseageType { get { return SkillUseage.Anywhere; } }

		public override void Initialize()
		{
		    Range = 9;
			Priority = SkillPriority.Medium;

			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated));

            SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, Range, falseConditionalFlags: TargetProperties.Normal));
            SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, Range, falseConditionalFlags: TargetProperties.Fast|TargetProperties.LowHealth));

			ClusterConditions.Add(new SkillClusterConditions(5d, Range, 2, true, useRadiusDistance: true));

		}

	}
}
