using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.DemonHunter
{
	 public class EntanglingShot : Skill
	 {
		 public override bool IsDestructiblePower { get { return true; } }

		 public override bool IsPrimarySkill { get { return true; } }
		 public override bool IsRanged { get { return true; } }
		 public override bool IsProjectile { get { return true; } }

		 public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		 public override void Initialize()
		  {
				Cooldown=5;
				
				
				
				Range=50;

			
				Priority=SkillPriority.Low;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckCanCast));
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.X1_DemonHunter_EntanglingShot; }
		  }
	 }
}
