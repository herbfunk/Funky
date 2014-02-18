using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills
{
	public class FistOfTheHeavens : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.None; }
		}

		public override void Initialize()
		{
			Cooldown = 5;
			Range = 30;
			Cost = 30;
			Priority = AbilityPriority.None;
			ExecutionType = AbilityExecuteFlags.Location;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(AbilityPreCastFlags.CheckCanCast);
			UseageType = AbilityUseage.Combat;
		}
	}
}
