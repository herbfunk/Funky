using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class ShieldGlare : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_ShieldGlare; }
		}

		public override void Initialize()
		{
			Cooldown = 12000;
			Range = 30;
			Priority = SkillPriority.Medium;
			ExecutionType = SkillExecutionFlags.Location|SkillExecutionFlags.ClusterLocation;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckRecastTimer);
			UseageType = SkillUseage.Combat;

			//Clusters with 3 units within 30f
			ClusterConditions.Add(new SkillClusterConditions(10d, 30f, 3, true));
			
			//Any Non-Normal Mob thats within 20f and is below 95% HP.
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 20, 0.95d, TargetProperties.Normal));
		}
	}
}
