using System;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.DemonHunter
{
	public class Strafe : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override bool IsMovementSkill { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ZigZagPathing; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 12;
			Range = 25;


			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast));
			//UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 2);
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 25, -1, 0.95d, TargetProperties.Normal));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, 60, -1, 0.95d));
			ClusterConditions.Add(new SkillClusterConditions(9d, 45f, 5, true));

			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial;
		}


		public override SNOPower Power
		{
			get { return SNOPower.DemonHunter_Strafe; }
		}
	}
}
