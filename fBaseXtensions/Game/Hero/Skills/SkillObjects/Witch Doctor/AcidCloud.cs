using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Witchdoctor
{
	public class AcidCloud : Skill
	{
		public override double Cooldown { get { return 1500; } }

		public override bool IsRanged { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 250;
			Range = RuneIndex == 4 ? 20 : 40;

			
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckEnergy |
									  SkillPrecastFlags.CheckCanCast));

			PreCast.Criteria += (s) => !Hotbar.HasDebuff(SNOPower.Succubus_BloodStar);

			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: Range, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal | TargetProperties.Fast));
			//Any unit when our energy is greater than 90%!
			SingleUnitCondition.Add(new UnitTargetConditions
			{
				TrueConditionFlags = TargetProperties.None,
                Criteria = (unit) => FunkyGame.Hero.dCurrentEnergyPct > 0.9d,
				MaximumDistance = Range,
				FalseConditionFlags = TargetProperties.LowHealth,
			});

			ClusterConditions.Add(new SkillClusterConditions(4d, RuneIndex == 4 ? 20f : 40f, 2, true));

			FcriteriaCombat = (u) => !FunkyGame.Hero.Class.bWaitingForSpecial;
		}


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_AcidCloud; }
		}
	}
}
