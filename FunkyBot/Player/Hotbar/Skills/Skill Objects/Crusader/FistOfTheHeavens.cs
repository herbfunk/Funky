using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class FistOfTheHeavens : Skill
	{
		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_FistOfTheHeavens; }
		}

		public override void Initialize()
		{
			Cooldown = 5;
			Range = 49;
			Cost = 30;
			Priority = SkillPriority.Medium;
			ExecutionType = SkillExecutionFlags.Location|SkillExecutionFlags.ClusterLocation;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			UseageType = SkillUseage.Combat;

			ClusterConditions.Add(new SkillClusterConditions(8d, 45f, 4, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 45, 0.95d, TargetProperties.Normal));
			SingleUnitCondition.Add(new UnitTargetConditions
			{
				TrueConditionFlags = TargetProperties.None,
				Criteria = () => Bot.Character.Data.dCurrentEnergyPct > 0.5d,
				FalseConditionFlags = TargetProperties.LowHealth,
			});
		}
	}
}
