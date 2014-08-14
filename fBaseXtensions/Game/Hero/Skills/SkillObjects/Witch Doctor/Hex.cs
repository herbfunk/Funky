using System;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Witchdoctor
{
	public class Hex : Skill
	{
		public override double Cooldown { get { return 15200; } }

		public override bool IsBuff { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Self; } }

		public override void Initialize()
		{
		
			
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 49;

			Priority = SkillPriority.Medium;

			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckEnergy |
									  SkillPrecastFlags.CheckCanCast));
			UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_12, 1);
			ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_12, 1);
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 18, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
		}


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_Hex; }
		}
	}
}
