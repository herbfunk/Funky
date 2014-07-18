using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Wizard
{
	public class MagicMissile : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override bool IsDestructiblePower { get { return true; } }
		public override bool IsPrimarySkill { get { return true; } }
		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			Range = 40;


		
			Priority = SkillPriority.Low;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast));
		}


		public override SNOPower Power
		{
			get { return SNOPower.Wizard_MagicMissile; }
		}
	}
}
