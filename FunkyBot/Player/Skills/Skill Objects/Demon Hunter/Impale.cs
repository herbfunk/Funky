using fBaseXtensions.Game;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.DemonHunter
{
	 public class Impale : Skill
	 {
		 public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		 public override void Initialize()
		  {
				Cooldown=5;
				
				WaitVars=new WaitLoops(0, 1, true);
				Cost=25;
				Range=12;
			
				Priority=SkillPriority.Medium;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 12));


				FcriteriaCombat=() => (!Bot.Character.Class.bWaitingForSpecial && FunkyGame.Hero.dCurrentEnergy >= Bot.Character.Class.iWaitingReservedAmount);
		  }

	
		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_Impale; }
		  }
	 }
}
