using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class FistOfTheHeavens : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_FistOfTheHeavens; }
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
