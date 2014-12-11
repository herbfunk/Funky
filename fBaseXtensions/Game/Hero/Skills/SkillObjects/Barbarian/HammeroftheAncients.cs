using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Barbarian
{
	public class HammeroftheAncients : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_HammerOfTheAncients; } }


		public override double Cooldown { get { return 0; } }


		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTargetNearest | SkillExecutionFlags.Target; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Range = RuneIndex == 1 ? 20 : 15;
			Cost = 20;
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated);
			ClusterConditions.Add(new SkillClusterConditions(5d, Range, 3, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, Range, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
			SingleUnitCondition.Add(new UnitTargetConditions
			{
                Criteria = (unit) => FunkyGame.Hero.dCurrentEnergyPct > 0.80d,
				MaximumDistance = Range,
				FalseConditionFlags = TargetProperties.LowHealth,
			});

            //No actual primary skill.. lets treat this as a primary (as a inital targeting skill)
		    if (!FunkyGame.Hero.Class.ContainsAnyPrimarySkill)
		    {
                SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None));
		    }
			
			FcriteriaCombat = (u) => !FunkyGame.Hero.Class.bWaitingForSpecial;
		}

	}
}
