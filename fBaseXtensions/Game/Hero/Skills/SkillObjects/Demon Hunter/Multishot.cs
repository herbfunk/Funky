using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Demonhunter
{
	public class Multishot : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTarget|SkillExecutionFlags.Target; } }

		private bool FullMarauderSetBonus = false;
		public override void Initialize()
		{
			FullMarauderSetBonus = Equipment.CheckLegendaryItemCount(LegendaryItemTypes.EmbodimentoftheMarauder, 6);

			WaitVars = new WaitLoops(1, 1, true);
			Cost = 30;
			Range = 50;

		
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckEnergy));
			ClusterConditions.Add(new SkillClusterConditions(10d, 40, 3, true));
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
			get { return SNOPower.DemonHunter_Multishot; }
		}
	}
}
