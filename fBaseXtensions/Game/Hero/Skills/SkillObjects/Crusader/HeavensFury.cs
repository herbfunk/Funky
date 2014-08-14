using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Crusader
{
	public class HeavensFury : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_HeavensFury3; }
		}

		public override double Cooldown { get { return _cooldown; } set { _cooldown = value; } }
		private double _cooldown = 5;
		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }
		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Location | SkillExecutionFlags.ClusterLocation; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			if (RuneIndex != 4)
				Cooldown = 20000;

			Range = 49;
			Cost = RuneIndex == 4 ? 40 : 0;
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

			//Fires of Heaven (Straight Line Fire)
			ClusterConditions.Add(RuneIndex == 4
				? new SkillClusterConditions(5d, 45f, 3, true)
				: new SkillClusterConditions(8d, 45f, 4, true));

			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 45, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

			//Fire of Heaven (No CD!)
			if (RuneIndex == 4)
			{
				SingleUnitCondition.Add(new UnitTargetConditions
				{
					TrueConditionFlags = TargetProperties.None,
					Criteria = () => FunkyGame.Hero.dCurrentEnergyPct >  0.90d,
					FalseConditionFlags = TargetProperties.LowHealth|TargetProperties.Weak,
				});
			}
		}
	}
}
