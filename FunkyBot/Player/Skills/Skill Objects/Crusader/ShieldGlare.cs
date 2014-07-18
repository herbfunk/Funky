using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Crusader
{
	public class ShieldGlare : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_ShieldGlare; }
		}


		public override double Cooldown { get { return 12000; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }


		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Location | SkillExecutionFlags.ClusterLocation; } }
	
		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Range = 30;
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckRecastTimer);

			//Clusters with 3 units within 30f
			ClusterConditions.Add(new SkillClusterConditions(10d, 30f, 3, true));
			
			//Any Non-Normal Mob thats within 20f and is below 95% HP.
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 20, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
		}
	}
}
