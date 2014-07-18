using fBaseXtensions.Game;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Crusader
{
	public class Consecration : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Consecration; }
		}

		public override double Cooldown { get { return 30000; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Self; } }
	
		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Range = 25;
			Cost = 10;
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

			FcriteriaCombat = () => FunkyGame.Hero.dCurrentHealthPct < 0.5d;
		}
	}
}
