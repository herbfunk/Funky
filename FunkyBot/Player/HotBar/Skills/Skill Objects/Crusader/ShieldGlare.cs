using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills
{
	public class ShieldGlare : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.None; }
		}

		public override void Initialize()
		{
			Cooldown = 12000;
			Range = 30;
			Priority = AbilityPriority.Medium;
			ExecutionType = AbilityExecuteFlags.Buff;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(AbilityPreCastFlags.CheckRecastTimer);
			UseageType = AbilityUseage.Combat;
		}
	}
}
