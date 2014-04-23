using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class ShieldGlare : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_ShieldGlare; }
		}

		public override void Initialize()
		{
			Cooldown = 12000;
			Range = 30;
			Priority = SkillPriority.Medium;
			ExecutionType = SkillExecutionFlags.Buff;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckRecastTimer);
			UseageType = SkillUseage.Combat;
		}
	}
}
