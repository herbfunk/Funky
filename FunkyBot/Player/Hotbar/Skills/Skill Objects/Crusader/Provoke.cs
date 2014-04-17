using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class Provoke : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Provoke; }
		}

		public override void Initialize()
		{
			Cooldown = 20000;
			Range = 20;
			Priority = AbilityPriority.High;
			ExecutionType = AbilityExecuteFlags.Buff;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(AbilityPreCastFlags.None);
			UseageType = AbilityUseage.Combat;
		}
	}
}
