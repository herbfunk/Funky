using System;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	public class SevenSidedStrike : Skill
	{
		public override double Cooldown { get { return _cooldown; } set { _cooldown = value; } }
		private double _cooldown = 30200;

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Location; } }

		public override void Initialize()
		{
			if (RuneIndex==3)
				Cooldown = 17200;

			WaitVars = new WaitLoops(2, 3, true);
			Cost = 50;
			Range = 16;
			Priority = SkillPriority.Medium;

			PreCast = new SkillPreCast((SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated));

			//UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_20, 6);
			//ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_20, 3);

            ClusterConditions.Add(new SkillClusterConditions(10d, Range, 6, true));
            SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, Range, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));


			FcriteriaCombat = (u) => !FunkyGame.Hero.Class.bWaitingForSpecial || FunkyGame.Hero.dCurrentEnergy >= FunkyGame.Hero.Class.iWaitingReservedAmount;
		}


		public override SNOPower Power
		{
			get { return SNOPower.Monk_SevenSidedStrike; }
		}
	}
}
