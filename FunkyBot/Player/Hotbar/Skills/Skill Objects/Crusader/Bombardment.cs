using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class Bombardment : Skill
	{
		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Bombardment; }
		}

		public override void Initialize()
		{
			Cooldown = 60000;
			Range = 45;
			Priority = SkillPriority.High;
			ExecutionType = SkillExecutionFlags.Location|SkillExecutionFlags.ClusterLocation;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			UseageType = SkillUseage.Combat;

			ClusterConditions.Add(new SkillClusterConditions(7d, 45f, 5, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 45, 0.95d, TargetProperties.Normal));
		}
	}
}
