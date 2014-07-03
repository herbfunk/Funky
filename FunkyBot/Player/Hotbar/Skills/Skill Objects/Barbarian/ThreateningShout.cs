using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
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
				Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_20] > 1 ||
				(Bot.Targeting.Cache.CurrentTarget.IsBoss && Bot.Targeting.Cache.CurrentTarget.RadiusDistance <= 20) ||
				(Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20] > 2 && !Bot.Targeting.Cache.Environment.bAnyBossesInRange &&
				 (Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_50] == 0 ||
				  Bot.Character.Class.HotBar.HasPower(SNOPower.Barbarian_SeismicSlam))) ||
				Bot.Character.Data.dCurrentHealthPct <= 0.75
				);
		}
	}
}
