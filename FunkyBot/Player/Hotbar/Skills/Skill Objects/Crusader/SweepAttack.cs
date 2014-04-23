using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class SweepAttack : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_SweepAttack; }
		}

		public override void Initialize()
		{
			Cooldown = 5;
			Range = 18;
			Cost = 20;
			Priority = SkillPriority.Low;
			ExecutionType = SkillExecutionFlags.Target|SkillExecutionFlags.Location;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckEnergy);
			UseageType = SkillUseage.Combat;
		}
	}
}