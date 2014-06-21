using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{

	 public class Bash : Skill
	 {
		 public override SNOPower Power { get { return SNOPower.Barbarian_Bash; } }

	
		 public override double Cooldown { get { return 5; } }

		 public override bool IsDestructiblePower { get { return true; } }

		 public override bool IsPrimarySkill { get { return true; } }

		 public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		 public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }


		 public override SkillUseage UseageType { get { return SkillUseage.Combat; } }


		  public override void Initialize()
		  {
			  Priority = SkillPriority.Low;

			  //Pulverize Rune increases the range!
			  if (RuneIndex == 4)
				  Range = 20;
			  else
				  Range = 10;
		  }
	 }
}
