using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Demonhunter
{
	public class ClusterArrow : Skill
	{
		public override double Cooldown { get { return 150; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Location | SkillExecutionFlags.ClusterLocation; } }

		private bool FullMarauderSetBonus = false;
		public override void Initialize()
		{
			FullMarauderSetBonus = Equipment.CheckLegendaryItemCount(LegendaryItemTypes.EmbodimentoftheMarauder, 6);

			Cost = 50;
			Range = 50;

			
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);


			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: Range, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
			ClusterConditions.Add(new SkillClusterConditions(7d, Range, 3, true));

			//Any unit when our energy is greater than 90%!
			SingleUnitCondition.Add(new UnitTargetConditions
			{
				TrueConditionFlags = TargetProperties.None,
				Criteria = () => FunkyGame.Hero.dCurrentEnergyPct > (FullMarauderSetBonus ? 0.5d : 0.9d),
				MaximumDistance = Range,
				FalseConditionFlags = TargetProperties.LowHealth,
			});

			FcriteriaCombat = () => !FunkyGame.Hero.Class.bWaitingForSpecial;
		}

		public override SNOPower Power
		{
			get { return SNOPower.DemonHunter_ClusterArrow; }
		}
	}
}
