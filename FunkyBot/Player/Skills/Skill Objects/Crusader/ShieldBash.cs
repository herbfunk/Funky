using fBaseXtensions.Game;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Crusader
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

		public override void Initialize()
		{
			Range = 25;
			Cost = 30;
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 25, MinimumHealthPercent: 0.75d, falseConditionalFlags: TargetProperties.Normal));
			SingleUnitCondition.Add(new UnitTargetConditions
			{
				TrueConditionFlags = TargetProperties.None,
				Criteria = () => FunkyGame.Hero.dCurrentEnergyPct > (Bot.Settings.Crusader.ReducedShieldBashCost?0.25:0.90d),
				FalseConditionFlags = TargetProperties.LowHealth,
			});
			ClusterConditions.Add(new SkillClusterConditions(5d, 25f, 2, true));
		}
	}
}
