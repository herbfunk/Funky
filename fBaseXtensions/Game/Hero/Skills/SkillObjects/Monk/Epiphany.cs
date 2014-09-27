using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	public class Epiphany : Skill
	{
		public override double Cooldown { get { return 60000; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		 public override void Initialize()
		  {				
				WaitVars=new WaitLoops(1, 1, true);
				

				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckCanCast));
				
				ClusterConditions.Add(new SkillClusterConditions(10d, 50f, 13, false));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, 45, MinimumHealthPercent: 0.95d));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 45, -1, 0.95d, TargetProperties.Normal));

		  }


		  public override SNOPower Power
		  {
				get { return SNOPower.X1_Monk_Epiphany; }
		  }
	 }
}
