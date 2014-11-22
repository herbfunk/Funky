using System;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Barbarian
{
	public class CalloftheAncients : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_CallOfTheAncients; } }

	
		public override double Cooldown { get { return 120500; } }

		private readonly WaitLoops _waitVars = new WaitLoops(4, 4, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated));

			ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);

			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 25, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
		}


	}
}
