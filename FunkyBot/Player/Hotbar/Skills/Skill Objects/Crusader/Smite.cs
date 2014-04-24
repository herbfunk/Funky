using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class Smite : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Smite; }
		}

		public override void Initialize()
		{
			Cooldown = 5;
			Range = 15;
			Priority = SkillPriority.None;
			ExecutionType = SkillExecutionFlags.Target|SkillExecutionFlags.ClusterTargetNearest;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.None);
			UseageType = SkillUseage.Combat;

			ClusterConditions.Add(new SkillClusterConditions(8d, 20f, 3, true));
			SingleUnitCondition.Add(new UnitTargetConditions());
		}
	}
}
