using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	public class Electrocute : Skill
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
			Range = (RuneIndex == 2 ? 15 : 40);



			Priority = SkillPriority.Low;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast));

			//Aim for cluster with 2 units very close together.
			ClusterConditions.Add(new SkillClusterConditions(3d, RuneIndex == 2 ? 15 : 40, 2, true));
			//No conditions for a single target.
			SingleUnitCondition.Add(new UnitTargetConditions());
		}

		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Electrocute; }
		}
	}
}
