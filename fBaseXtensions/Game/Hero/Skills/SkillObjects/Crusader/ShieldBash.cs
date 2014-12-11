using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Crusader
{
	public class ShieldBash : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_ShieldBash2; }
		}


		public override double Cooldown { get { return 5; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override bool IsRanged { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target | SkillExecutionFlags.ClusterTarget; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		private bool ReducedShieldBashCost = false;

		public override void Initialize()
		{
			ReducedShieldBashCost = Equipment.CheckLegendaryItemCount(LegendaryItemTypes.PiroMarella);

			Range = 25;
			Cost = 30;
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 25, MinimumHealthPercent: 0.75d, falseConditionalFlags: TargetProperties.Normal));
			SingleUnitCondition.Add(new UnitTargetConditions
			{
				TrueConditionFlags = TargetProperties.None,
                Criteria = (unit) => FunkyGame.Hero.dCurrentEnergyPct > (ReducedShieldBashCost ? 0.25 : 0.90d),
				FalseConditionFlags = TargetProperties.LowHealth,
			});
			ClusterConditions.Add(new SkillClusterConditions(5d, 25f, 2, true));
		}
	}
}
