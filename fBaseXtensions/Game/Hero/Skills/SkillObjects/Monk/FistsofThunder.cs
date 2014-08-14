using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	public class FistsofThunder : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override bool IsDestructiblePower { get { return true; } }
		public override bool IsPrimarySkill { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.Target; } }

		public override void Initialize()
		{

			Priority = SkillPriority.Low;
			Range = RuneIndex == 0 ? 25 : 12;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast));

			ClusterConditions.Add(new SkillClusterConditions(5d, 20f, 1, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None));

		}

		public override SNOPower Power
		{
			get { return SNOPower.Monk_FistsofThunder; }
		}
	}
}
