using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class SweepAttack : Skill
	{
		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_SweepAttack; }
		}

		public override void Initialize()
		{
			Cooldown = 5;
			Range = 18;
			Cost = 20;
			Priority = SkillPriority.Low;
			ExecutionType = SkillExecutionFlags.Target|SkillExecutionFlags.ClusterTargetNearest;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			UseageType = SkillUseage.Combat;

			ClusterConditions.Add(new SkillClusterConditions(10d, 25f, 3, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 20, 0.95d, TargetProperties.Normal));
			SingleUnitCondition.Add(new UnitTargetConditions
			{
				TrueConditionFlags = TargetProperties.None,
				Criteria = () => Bot.Character.Data.dCurrentEnergyPct > 0.5d,
				FalseConditionFlags = TargetProperties.LowHealth,
			});
		}
	}
}