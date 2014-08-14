using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Wizard
{
	public class ArchonDisintegrationWave : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override bool IsDestructiblePower { get { return true; } }
		public override bool IsPrimarySkill { get { return true; } }
		public override bool IsRanged { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }


		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			Range = 48;

			IsChanneling = true;

		
			Priority = SkillPriority.Low;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, -1, 10));
		}


		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Archon_DisintegrationWave; }
		}
	}

	public class ArchonDisintegrationWaveFire : ArchonDisintegrationWave
	{
		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Archon_DisintegrationWave_Fire; }
		}
	}
	public class ArchonDisintegrationWaveCold : ArchonDisintegrationWave
	{
		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Archon_DisintegrationWave_Cold; }
		}
	}
	public class ArchonDisintegrationWaveLightning : ArchonDisintegrationWave
	{
		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Archon_DisintegrationWave_Lightning; }
		}
	}
}
