using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	public class WayOfTheHundredFists : Skill
	{
		public override double Cooldown { get { return _cooldown; } set { _cooldown = value; } }
		private double _cooldown = 5;

		public override bool IsDestructiblePower { get { return true; } }
		public override bool IsPrimarySkill { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			if (FunkyBaseExtension.Settings.Monk.bMonkComboStrike)
				Cooldown = 250 + (250 * FunkyBaseExtension.Settings.Monk.iMonkComboStrikeAbilities);



			Priority = FunkyBaseExtension.Settings.Monk.bMonkComboStrike ? SkillPriority.Medium : SkillPriority.Low;
			Range = 14;


			var precastflags = SkillPrecastFlags.CheckPlayerIncapacitated;
			//Combot Strike? lets enforce recast timer and cast check
			if (FunkyBaseExtension.Settings.Monk.bMonkComboStrike)
				precastflags |= SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckCanCast;

			PreCast = new SkillPreCast(precastflags);
		}



		public override SNOPower Power
		{
			get { return SNOPower.Monk_WayOfTheHundredFists; }
		}
	}
}
