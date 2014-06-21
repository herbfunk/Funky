using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class Slash : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Slash; }
		}

	
		public override double Cooldown { get { return 100; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override bool IsDestructiblePower { get { return true; } }
		public override bool IsPrimarySkill { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Range = 8;
			Cost = 10;

			Priority = SkillPriority.Low;
			PreCast = new SkillPreCast(SkillPrecastFlags.None);
			SingleUnitCondition.Add(new UnitTargetConditions());
		}
	}
}
