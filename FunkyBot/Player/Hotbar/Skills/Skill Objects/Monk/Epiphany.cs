using FunkyBot.Movement.Clustering;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	public class Epiphany : Skill
	{
		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		 public override void Initialize()
		  {
				//Only check for buff when correct rune is set! rune==2
				Cooldown=60000;
				
				WaitVars=new WaitLoops(1, 1, true);
				

				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckCanCast|SkillPrecastFlags.CheckRecastTimer));
				
				ClusterConditions.Add(new SkillClusterConditions(10d, 50f, 13, false));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, maxdistance: 45, MinimumHealthPercent: 0.75d));

		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.X1_Monk_Epiphany; }
		  }
	 }
}
