using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Crusader
{
	public class Punish : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Punish; }
		}

		public override double Cooldown { get { return 5; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override bool IsDestructiblePower { get { return true; } }
		public override bool IsPrimarySkill { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }
	
		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Range = 8;
			Priority = SkillPriority.Low;
			PreCast = new SkillPreCast(SkillPrecastFlags.None);
			
		}
	}
}
