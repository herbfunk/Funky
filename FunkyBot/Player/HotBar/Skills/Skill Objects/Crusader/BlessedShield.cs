using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills
{
	public class BlessedShield : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.None; }
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