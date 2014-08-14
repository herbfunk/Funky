using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Barbarian
{
	public class IgnorePain : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_IgnorePain; } }


		public override double Cooldown { get { return 30200; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override bool IsSpecialAbility { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override SkillUseage UseageType { get { return SkillUseage.Anywhere; } }

		public override void Initialize()
		{
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckCanCast));

			FcriteriaCombat = () => FunkyGame.Hero.dCurrentHealthPct <= 0.45;
		}

	}
}
