using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class BlessedHammer : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_BlessedHammer; }
		}

		public override void Initialize()
		{
			Cooldown = 5;
			Range = 25;
			Cost = 10;
			Priority = SkillPriority.Low;
			ExecutionType = SkillExecutionFlags.Buff;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckEnergy);
			UseageType = SkillUseage.Combat;
		}
	}
}
