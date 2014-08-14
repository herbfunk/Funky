using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
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
								   FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_25] > 0 || //with elites nearby..
								   FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_25] > 3;

		}

		public override SNOPower Power
		{
			get { return SNOPower.Monk_BreathOfHeaven; }
		}
	}
}
