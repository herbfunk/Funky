using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects
{
	public class Interact : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.Interact_Normal; }
		}
		public override double Cooldown { get { return 5; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override void Initialize()
		{
			Range = 7;
			Priority = SkillPriority.None;

			WaitVars = new WaitLoops(0, 2, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.None);
		}
	}
}
