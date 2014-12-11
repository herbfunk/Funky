using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Witchdoctor
{
	public class Piranhas : Skill
	{
		public override double Cooldown { get { return 8000; } }

		public override bool IsRanged { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.Location; } }

		public override void Initialize()
		{

			WaitVars = new WaitLoops(0, 3, true);
			Cost = 250;
			Range = 45;

		
			Priority = SkillPriority.High;

			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast));

			ClusterConditions.Add(new SkillClusterConditions(4d, 45f, 5, true));
			//SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, 45, 0.95d));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: Range, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
			SingleUnitCondition.Add(new UnitTargetConditions
			{
				TrueConditionFlags = TargetProperties.None,
                Criteria = (unit) => FunkyGame.Hero.dCurrentEnergyPct > 0.9d,
				MaximumDistance = Range,
				FalseConditionFlags = TargetProperties.LowHealth,
			});

			FcriteriaCombat = (u) => !FunkyGame.Hero.Class.bWaitingForSpecial;
		}


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_Piranhas; }
		}
	}
}
