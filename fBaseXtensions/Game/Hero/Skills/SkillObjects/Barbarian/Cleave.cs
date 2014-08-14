using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Barbarian
{
	public class Cleave : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_Cleave; } }


		public override double Cooldown { get { return 5; } }

		public override bool IsDestructiblePower { get { return true; } }
		public override bool IsPrimarySkill { get { return true; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target | SkillExecutionFlags.ClusterTargetNearest; } }
	
		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Priority = SkillPriority.Low;
			Range = 10;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast));
			ClusterConditions.Add(new SkillClusterConditions(4d, 10f, 2, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None));
		}
	}
}
