using FunkyBot.Movement.Clustering;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	public class Vengeance : Skill
	{
		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		 public override void Initialize()
		  {
				Cooldown=90000;
				
				WaitVars=new WaitLoops(0, 1, true);
			
				Priority=SkillPriority.High;
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, maxdistance: 50, MinimumHealthPercent: 0.75d));
				ClusterConditions.Add(new SkillClusterConditions(10d, 50f, 4, true, clusterflags: ClusterProperties.Elites));
				PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.X1_DemonHunter_Vengeance; }
		  }
	 }
}
