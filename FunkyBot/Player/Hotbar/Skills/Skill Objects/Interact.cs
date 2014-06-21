using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills
{
	public class Interact : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.Interact_Normal; }
		}
		public override double Cooldown { get { return 5; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			Range = 7;
			Priority = SkillPriority.None;

			WaitVars = new WaitLoops(2, 2, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.None);

		}
	}
}
