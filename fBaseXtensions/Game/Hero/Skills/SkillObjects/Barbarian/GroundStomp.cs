using System;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Navigation.Clustering;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Barbarian
{
	public class GroundStomp : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_GroundStomp; } }


		public override double Cooldown { get { return 12200; } }


		private readonly WaitLoops _waitVars = new WaitLoops(1, 2, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Self; } }

		public override SkillUseage UseageType { get { return SkillUseage.Anywhere; } }

		public override void Initialize()
		{
			Priority = SkillPriority.Medium;
			Range = 16;
			Cost = 20;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated));

		    ClusterConditions.Add(new SkillClusterConditions(5d, Range, 4, true, useRadiusDistance: true));

			//UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 4);
			//ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1);

            //Include goblins, bosses, and uniques too!
		    SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, Range, falseConditionalFlags: TargetProperties.Normal));
		}
	}
}
