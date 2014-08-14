using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Witchdoctor
{
	public class Gargantuan : Skill
	{
		public override double Cooldown { get { return 25000; } }

		public override bool IsBuff { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(2, 1, true);
			Counter = 1;

			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

			FcriteriaBuff = () => RuneIndex != 3 && FunkyGame.Targeting.Cache.Environment.HeroPets.Gargantuan == 0;

			FcriteriaCombat = () => (RuneIndex == 3 &&
								   (FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_15] >= 1 ||
									(FunkyGame.Targeting.Cache.CurrentUnitTarget.IsEliteRareUnique && FunkyGame.Targeting.Cache.CurrentTarget.RadiusDistance <= 15f))
								   || RuneIndex != 3 && FunkyGame.Targeting.Cache.Environment.HeroPets.Gargantuan == 0);
		}


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_Gargantuan; }
		}
	}
}
