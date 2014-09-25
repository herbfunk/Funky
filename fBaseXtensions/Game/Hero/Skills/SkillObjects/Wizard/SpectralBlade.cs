﻿using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Wizard
{
	public class SpectralBlade : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override bool IsDestructiblePower { get { return true; } }
		public override bool IsPrimarySkill { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			Range = RuneIndex == 1 ? 20 : 12;
			Priority = SkillPriority.Low;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast));
		}


		public override SNOPower Power
		{
			get { return SNOPower.Wizard_SpectralBlade; }
		}
	}
}
