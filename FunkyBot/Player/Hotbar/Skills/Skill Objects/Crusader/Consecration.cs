using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class Consecration : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Consecration; }
		}

		public override void Initialize()
		{
			Cooldown = 30000;
			Range = 25;
			Priority = AbilityPriority.Medium;
			ExecutionType = AbilityExecuteFlags.Self;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(AbilityPreCastFlags.None);
			UseageType = AbilityUseage.Combat;
		}
	}
}
