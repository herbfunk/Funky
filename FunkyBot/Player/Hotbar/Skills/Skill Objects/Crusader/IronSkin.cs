using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class IronSkin : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_IronSkin; }
		}

		public override void Initialize()
		{
			Cooldown = 30000;
			Range = 0;
			Priority = AbilityPriority.Medium;
			ExecutionType = AbilityExecuteFlags.Buff;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(AbilityPreCastFlags.CheckRecastTimer);
			UseageType = AbilityUseage.Combat;
		}
	}
}
