using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Witchdoctor
{
	public class SoulHarvest : Skill
	{
		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Cooldown = 15000;

			WaitVars = new WaitLoops(0, 1, true);
			Cost = 59;
			Counter = 5;

			Priority = SkillPriority.Medium;
			Range = 10;

			PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);
			ClusterConditions.Add(new SkillClusterConditions(6d, 10f, 4, false, useRadiusDistance: true));
			


			if (!Equipment.CheckLegendaryItemCount(LegendaryItemTypes.RaimentoftheJadeHarvester, 6))
			{
				SingleUnitCondition.Add(new UnitTargetConditions
				{
                    Criteria = (unit) => Hotbar.GetBuffStacks(SNOPower.Witchdoctor_SoulHarvest) == 0,
					MaximumDistance = 9,
					FalseConditionFlags = TargetProperties.Normal,
					HealthPercent = 0.95d,
					TrueConditionFlags = TargetProperties.None,
				});

				FcriteriaCombat = (u) =>
				{

					double lastCast = LastUsedMilliseconds;
					int RecastMS = RuneIndex == 1 ? 45000 : 20000;
					bool recast = lastCast > RecastMS; //if using soul to waste -- 45ms, else 20ms.
					int stackCount = Hotbar.GetBuffStacks(SNOPower.Witchdoctor_SoulHarvest);
					if (stackCount < 5)
						return true;
					if (recast)
						return true;
					return false;
				};
			}
			else
			{
				SingleUnitCondition.Add(new UnitTargetConditions
				{
					MaximumDistance = 9,
					FalseConditionFlags = TargetProperties.Normal,
					HealthPercent = 0.95d,
					TrueConditionFlags = TargetProperties.None,
				});
			}

		}

		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_SoulHarvest; }
		}
	}
}
