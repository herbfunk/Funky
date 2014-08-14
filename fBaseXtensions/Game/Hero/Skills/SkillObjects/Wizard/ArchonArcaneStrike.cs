using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Wizard
{
	 public class ArchonArcaneStrike : Skill
	 {
		 public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target | SkillExecutionFlags.ClusterTargetNearest; } }

		 public override void Initialize()
		  {
				Cooldown=200;
				
				
				Range=15;
				
				Priority=SkillPriority.Low;
				IsDestructiblePower = true;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated));
				ClusterConditions.Add(new SkillClusterConditions(6d, 10f, 2, true));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None));
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_Archon_ArcaneStrike; }
		  }
	 }

	 public class ArchonArcaneStrikeFire:ArchonArcaneStrike
	 {
		 public override SNOPower Power
		 {
			 get { return SNOPower.Wizard_Archon_ArcaneStrike_Fire; }
		 }
	 }

	 public class ArchonArcaneStrikeCold : ArchonArcaneStrike
	 {
		 public override SNOPower Power
		 {
			 get { return SNOPower.Wizard_Archon_ArcaneStrike_Cold; }
		 }
	 }

	 public class ArchonArcaneStrikeLightning : ArchonArcaneStrike
	 {
		 public override SNOPower Power
		 {
			 get { return SNOPower.Wizard_Archon_ArcaneStrike_Lightning; }
		 }
	 }
}
