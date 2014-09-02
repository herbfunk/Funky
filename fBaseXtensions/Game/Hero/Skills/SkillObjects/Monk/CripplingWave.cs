using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	public class CripplingWave : Skill
	{
		public override double Cooldown { get { return _cooldown; } set { _cooldown = value; } }
		private double _cooldown = 5;

		public override bool IsDestructiblePower { get { return true; } }
		public override bool IsPrimarySkill { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			//if (FunkyBaseExtension.Settings.Monk.bMonkComboStrike)
				//Cooldown = 250 + (250 * FunkyBaseExtension.Settings.Monk.iMonkComboStrikeAbilities);

			Priority = SkillPriority.Low;
			Range = 14;

			if (FunkyBaseExtension.Settings.Monk.bMonkComboStrike)
			{
				PreCast = new SkillPreCast
				{
					Flags = SkillPrecastFlags.CheckPlayerIncapacitated
				};
				PreCast.Criteria += skill => FunkyGame.Hero.Class.LastUsedAbilities.IndexOf(this) >= FunkyBaseExtension.Settings.Monk.iMonkComboStrikeAbilities-1;
				PreCast.CreatePrecastCriteria();
			}
			else
				PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated);

		}

		public override SNOPower Power
		{
			get { return SNOPower.Monk_CripplingWave; }
		}
	}
}
