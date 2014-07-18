using fBaseXtensions.Game;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Monk
{
	public class BreathofHeaven : Skill
	{
		public override double Cooldown { get { return 15000; } }

		public override bool IsBuff { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(1, 1, true);


			Priority = SkillPriority.High;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckRecastTimer));
			FcriteriaBuff = () => FunkyGame.Hero.dCurrentHealthPct <= 0.5d;

			FcriteriaCombat = () => FunkyGame.Hero.dCurrentHealthPct <= 0.5d ||
								   Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_25] > 0 || //with elites nearby..
								   Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_25] > 3;

		}

		public override SNOPower Power
		{
			get { return SNOPower.Monk_BreathOfHeaven; }
		}
	}
}
