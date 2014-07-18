using System;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.WitchDoctor
{
	public class WallOfZombies : Skill
	{
		public override double Cooldown { get { return 25200; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Location; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 103;
			Range = 25;

			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckEnergy |
									  SkillPrecastFlags.CheckCanCast));
			UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 3);
			ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1);
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 25, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial;
		}


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_WallOfZombies; }
		}
	}
}
