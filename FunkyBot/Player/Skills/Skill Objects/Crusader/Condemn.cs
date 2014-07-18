using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Crusader
{
	public class Condemn : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Condemn; }
		}

		public override double Cooldown { get { return 15000; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }
		
		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

			ClusterConditions.Add(new SkillClusterConditions(10d, 16f, 7, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 15, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
		
		}
	}
}
