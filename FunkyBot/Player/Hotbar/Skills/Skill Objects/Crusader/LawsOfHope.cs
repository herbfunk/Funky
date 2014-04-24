using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class LawsOfHope : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_LawsOfHope2; }
		}

		public override void Initialize()
		{
			Cooldown = 45000;
			Range = 8;
			Priority = SkillPriority.High;
			ExecutionType = SkillExecutionFlags.Buff;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			UseageType = SkillUseage.Combat;
		}
	}
}
