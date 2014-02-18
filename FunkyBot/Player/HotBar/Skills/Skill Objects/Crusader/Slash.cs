using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills
{
	public class Slash : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.None; }
		}

		public override void Initialize()
		{
			Cooldown = 5;
			Range = 10;
			Priority = AbilityPriority.None;
			ExecutionType = AbilityExecuteFlags.Target;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(AbilityPreCastFlags.None);
			UseageType = AbilityUseage.Combat;
		}
	}
}
