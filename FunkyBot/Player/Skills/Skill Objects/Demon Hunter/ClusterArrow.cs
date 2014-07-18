using fBaseXtensions.Game;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.DemonHunter
{
	public class ClusterArrow : Skill
	{
		public override double Cooldown { get { return 150; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Location | SkillExecutionFlags.ClusterLocation; } }

		public override void Initialize()
		{
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
				Criteria = () => FunkyGame.Hero.dCurrentEnergyPct > 0.9d,
				MaximumDistance = Range,
				FalseConditionFlags = TargetProperties.LowHealth,
			});

			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial;
		}

		public override SNOPower Power
		{
			get { return SNOPower.DemonHunter_ClusterArrow; }
		}
	}
}
