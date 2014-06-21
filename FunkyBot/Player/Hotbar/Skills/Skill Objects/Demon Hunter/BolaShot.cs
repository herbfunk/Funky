using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	public class BolaShot : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override bool IsDestructiblePower { get { return true; } }
		public override bool IsPrimarySkill { get { return true; } }
		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			Range = 50;

			Priority = SkillPriority.Low;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast));

			ClusterConditions.Add(new SkillClusterConditions(5d, 49f, 2, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 49));
		}


		public override SNOPower Power
		{
			get { return SNOPower.DemonHunter_Bolas; }
		}
	}
}
