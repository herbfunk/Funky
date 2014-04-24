using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class Condemn : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Condemn; }
		}

		public override void Initialize()
		{
			Cooldown = 15000;
			Priority = SkillPriority.High;
			ExecutionType = SkillExecutionFlags.Buff;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			UseageType = SkillUseage.Combat;

			ClusterConditions.Add(new SkillClusterConditions(10d, 16f, 7, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.IsSpecial, 15, 0.95d, TargetProperties.Normal));
		}
	}
}
