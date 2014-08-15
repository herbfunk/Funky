using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Wizard
{
	public class Hydra : Skill
	{
		public override double Cooldown { get { return 1500; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.Location; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(1, 2, true);
			Counter = Equipment.CheckLegendaryItemCount(LegendaryItemTypes.SerpentSparker) ? 2 : 1;
			Cost = 15;
			Range = 50;

		
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckEnergy |
									  SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckPetCount));
			ClusterConditions.Add(new SkillClusterConditions(7d, 50f, 2, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 45, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal | TargetProperties.Fast));
		}

		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Hydra; }
		}
	}
}
