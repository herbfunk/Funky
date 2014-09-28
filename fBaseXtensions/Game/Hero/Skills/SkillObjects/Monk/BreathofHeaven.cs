using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
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
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast));

			if (RuneIndex==3)
			{
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 20));
				FcriteriaCombat = () => !Hotbar.HasBuff(SNOPower.Monk_BreathOfHeaven);
			}
			else
			{
				FcriteriaCombat = () => FunkyGame.Hero.dCurrentHealthPct <= 0.5d ||
									   FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_25] > 0 || //with elites nearby..
									   FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_25] > 3;
			}

			FcriteriaBuff = () => FunkyGame.Hero.dCurrentHealthPct <= 0.5d;
		}

		public override SNOPower Power
		{
			get { return SNOPower.Monk_BreathOfHeaven; }
		}
	}
}
