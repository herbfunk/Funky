using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Crusader
{
	public class Condemn : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Condemn; }
		}

		public override double Cooldown { get { return 15000; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }
		
		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
		    Range = 20;
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

			ClusterConditions.Add(new SkillClusterConditions(10d, 20f, 5, false, useRadiusDistance: true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 20, MinimumHealthPercent: 0.99d, falseConditionalFlags: TargetProperties.Normal));
		
		}
	}
}
