using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Crusader
{
	public class Bombardment : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Bombardment; }
		}

		
		public override double Cooldown { get { return 60000; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		private SkillExecutionFlags _executiontype = SkillExecutionFlags.Location | SkillExecutionFlags.ClusterLocation;
		public override SkillExecutionFlags ExecutionType { get { return _executiontype; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			if (RuneIndex==4)
				_executiontype=SkillExecutionFlags.ClusterTarget|SkillExecutionFlags.Target;

			Range = 45;
			Cost = 10;
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

			ClusterConditions.Add(new SkillClusterConditions(7d, 45f, 9, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 45, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
		}
	}
}
