using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class HeavensFury : Skill
	{
		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_HeavensFury3; }
		}

		public override void Initialize()
		{
			Cooldown = 20000;
			Range = 49;
			Priority = SkillPriority.High;
			ExecutionType = SkillExecutionFlags.Location | SkillExecutionFlags.ClusterLocation;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			UseageType = SkillUseage.Combat;

			ClusterConditions.Add(new SkillClusterConditions(8d, 45f, 4, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 45, 0.95d, TargetProperties.Normal));
		}
	}
}
