using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class FanOfKnives : Skill
	 {
		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		 public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		 public override void Initialize()
		  {
				Cooldown=10000;
				
				WaitVars=new WaitLoops(1, 1, true);
				Cost=20;
				Range=0;
			
				Priority=SkillPriority.Medium;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckRecastTimer|
				                          SkillPrecastFlags.CheckEnergy));
				ClusterConditions.Add(new SkillClusterConditions(4d, 10f, 2, false));
				//SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.CloseDistance);
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_FanOfKnives; }
		  }
	 }
}
