using System;
using fBaseXtensions.Game;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Monk
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

			UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_20, 6);
			ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_20, 3);
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 15, falseConditionalFlags: TargetProperties.Normal)); //any non-normal unit!


			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial || FunkyGame.Hero.dCurrentEnergy >= Bot.Character.Class.iWaitingReservedAmount;
		}


		public override SNOPower Power
		{
			get { return SNOPower.Monk_SevenSidedStrike; }
		}
	}
}
