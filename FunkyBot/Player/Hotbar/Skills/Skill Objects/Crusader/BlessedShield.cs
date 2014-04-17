using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class BlessedShield : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_BlessedShield; }
		}

		public override void Initialize()
		{
			Cooldown = 5;
			Range = 25;
			Cost = 20;
			Priority = AbilityPriority.Low;
			ExecutionType = AbilityExecuteFlags.Target;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(AbilityPreCastFlags.CheckCanCast);
			UseageType = AbilityUseage.Combat;
		}
	}
}