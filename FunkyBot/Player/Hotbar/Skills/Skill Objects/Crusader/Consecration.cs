using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class Consecration : Skill
	{
		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Consecration; }
		}

		public override void Initialize()
		{
			Cooldown = 30000;
			Range = 25;
			Priority = SkillPriority.Medium;
			ExecutionType = SkillExecutionFlags.Self;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			UseageType = SkillUseage.Combat;

			FcriteriaCombat = () => Bot.Character.Data.dCurrentHealthPct < 0.5d;
		}
	}
}
