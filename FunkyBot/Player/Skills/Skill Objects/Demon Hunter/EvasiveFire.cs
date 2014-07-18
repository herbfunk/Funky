using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.DemonHunter
{
	public class EvasiveFire : Skill
	{
		public override double Cooldown { get { return 1500; } }

		public override bool IsDestructiblePower { get { return true; } }
		public override bool IsPrimarySkill { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			Range = RuneIndex == 4 ? 35 : 45;

			WaitVars = new WaitLoops(0, 0, false);
			Cost = 0;

			Priority = SkillPriority.Low;

			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None));
		}

		public override SNOPower Power
		{
			get { return SNOPower.X1_DemonHunter_EvasiveFire; }
		}
	}
}
