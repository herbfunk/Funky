using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Crusader
{
	public class BlessedHammer : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_BlessedHammer; }
		}

		
		public override double Cooldown { get { return 5; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Location; } }
		
		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Range = 8;
			Cost = 10;
			Priority = SkillPriority.Low;
			ClusterConditions.Add(new SkillClusterConditions(10d, 15f, 3, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

			if (!Bot.Character.Class.ContainsAnyPrimarySkill)
			{
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None));
			}
		}
	}
}
