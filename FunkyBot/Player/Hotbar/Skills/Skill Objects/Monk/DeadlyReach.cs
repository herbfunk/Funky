using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	public class DeadlyReach : Skill
	{
		public override double Cooldown { get { return _cooldown; } set { _cooldown = value; } }
		private double _cooldown = 5;

		public override bool IsDestructiblePower { get { return true; } }
		public override bool IsPrimarySkill { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			if (Bot.Settings.Monk.bMonkComboStrike)
				Cooldown = 250 + (250 * Bot.Settings.Monk.iMonkComboStrikeAbilities);




			Priority = Bot.Settings.Monk.bMonkComboStrike ? SkillPriority.Medium : SkillPriority.Low;
			Range = 16;
			var precastflags = SkillPrecastFlags.CheckPlayerIncapacitated;
			//Combot Strike? lets enforce recast timer and cast check
			if (Bot.Settings.Monk.bMonkComboStrike)
				precastflags |= SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckCanCast;

			PreCast = new SkillPreCast(precastflags);
		}


		public override SNOPower Power
		{
			get { return SNOPower.Monk_DeadlyReach; }
		}
	}
}
