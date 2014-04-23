using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class Punish : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Punish; }
		}

		public override void Initialize()
		{
			Cooldown = 5;
			Range = 8;
			Priority = SkillPriority.None;
			ExecutionType = SkillExecutionFlags.Target;

			WaitVars = new WaitLoops(1, 1, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.None);
			UseageType = SkillUseage.Combat;
		}
	}
}
