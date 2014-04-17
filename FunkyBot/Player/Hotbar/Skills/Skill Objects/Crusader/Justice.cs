using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class Justice : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Justice; }
		}

		public override void Initialize()
		{
			Cooldown = 5;
			Range = 40;
			Priority = AbilityPriority.None;
			ExecutionType = AbilityExecuteFlags.Target;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(AbilityPreCastFlags.None);
			UseageType = AbilityUseage.Combat;
		}
	}
}
