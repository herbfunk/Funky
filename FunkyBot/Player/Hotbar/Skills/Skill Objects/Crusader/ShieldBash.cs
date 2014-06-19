using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class ShieldBash : Skill
	{
		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_ShieldBash2; }
		}

		public override void Initialize()
		{
			Cooldown = 5;
			Range = 25;
			Cost = 30;
			Priority = SkillPriority.Medium;
			ExecutionType = SkillExecutionFlags.Target|SkillExecutionFlags.ClusterTarget;
			IsRanged = true;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 25, MinimumHealthPercent: 0.75d, falseConditionalFlags: TargetProperties.Normal));
			SingleUnitCondition.Add(new UnitTargetConditions
			{
				TrueConditionFlags = TargetProperties.None,
				Criteria = () => Bot.Character.Data.dCurrentEnergyPct > (Bot.Settings.Crusader.ReducedShieldBashCost?0.25:0.90d),
				FalseConditionFlags = TargetProperties.LowHealth,
			});
			ClusterConditions.Add(new SkillClusterConditions(5d, 25f, 2, true));
			UseageType = SkillUseage.Combat;
		}
	}
}
