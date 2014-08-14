using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Barbarian
{
	public class ThreateningShout : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_ThreateningShout; } }

		
		public override double Cooldown { get { return 10200; } }

		private readonly WaitLoops _waitVars = new WaitLoops(1, 1, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Self; } }
	
		public override SkillUseage UseageType { get { return SkillUseage.Anywhere; } }

		public override void Initialize()
		{
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated));
			FcriteriaCombat = () => (
				FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_20] > 1 ||
				(FunkyGame.Targeting.Cache.CurrentTarget.IsBoss && FunkyGame.Targeting.Cache.CurrentTarget.RadiusDistance <= 20) ||
				(FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20] > 2 && !FunkyGame.Targeting.Cache.Environment.bAnyBossesInRange &&
				 (FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_50] == 0 ||
				  Hotbar.HasPower(SNOPower.Barbarian_SeismicSlam))) ||
				FunkyGame.Hero.dCurrentHealthPct <= 0.75
				);
		}
	}
}
