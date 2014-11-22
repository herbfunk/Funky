using System;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Barbarian
{
	public class Revenge : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_Revenge; } }


		public override double Cooldown { get { return 600; } }

		private readonly WaitLoops _waitVars = new WaitLoops(4, 4, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Self; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Priority = SkillPriority.Medium;

            PreCast = new SkillPreCast
            {
                Flags = SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast,
            };
		    PreCast.Criteria += skill => Hotbar.HasBuff(SNOPower.Barbarian_Revenge_Buff);
            PreCast.CreatePrecastCriteria();

            SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 10, falseConditionalFlags: TargetProperties.Normal));
            ClusterConditions.Add(new SkillClusterConditions(5d, 7, 2, false, useRadiusDistance: true));
		}

	}
}
