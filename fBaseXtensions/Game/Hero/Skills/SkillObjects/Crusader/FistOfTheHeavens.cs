using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Crusader
{
	public class FistOfTheHeavens : Skill
	{

		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_FistOfTheHeavens; }
		}


		public override double Cooldown { get { return 5; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Location | SkillExecutionFlags.ClusterLocation; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Range = 49;
			Cost = 30;
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

			ClusterConditions.Add(new SkillClusterConditions(8d, 45f, 4, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 45, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
			SingleUnitCondition.Add(new UnitTargetConditions
			{
				TrueConditionFlags = TargetProperties.None,
				Criteria = () => FunkyGame.Hero.dCurrentEnergyPct > 0.5d,
				FalseConditionFlags = TargetProperties.LowHealth,
			});
		}
	}
}
