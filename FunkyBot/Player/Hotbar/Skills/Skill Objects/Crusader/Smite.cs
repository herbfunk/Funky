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

		public override double Cooldown { get { return 5; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override bool IsDestructiblePower { get { return true; } }
		public override bool IsPrimarySkill { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target | SkillExecutionFlags.ClusterTargetNearest; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Range = 15;
			Priority = SkillPriority.Low;
			PreCast = new SkillPreCast(SkillPrecastFlags.None);

			ClusterConditions.Add(new SkillClusterConditions(8d, 20f, 3, true));
			SingleUnitCondition.Add(new UnitTargetConditions());
		}
	}
}
