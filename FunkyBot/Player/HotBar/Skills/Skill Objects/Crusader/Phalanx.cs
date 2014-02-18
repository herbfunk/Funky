using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills
{
	public class Phalanx : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.None; }
		}

		public override void Initialize()
		{
			Cost = 30;
			Cooldown = 5;
			Range = 20;
			Priority = AbilityPriority.Medium;
			ExecutionType = AbilityExecuteFlags.Location;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(AbilityPreCastFlags.None);
			UseageType = AbilityUseage.Combat;
		}
	}
}
