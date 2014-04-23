using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class Judgement : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Judgment; }
		}

		public override void Initialize()
		{
			Cooldown = 20000;
			Range = 20;
			Priority = SkillPriority.Medium;
			ExecutionType = SkillExecutionFlags.Location;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.None);
			UseageType = SkillUseage.Combat;
		}
	}
}
