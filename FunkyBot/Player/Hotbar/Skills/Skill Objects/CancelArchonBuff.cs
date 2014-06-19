using System.Collections.Generic;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills
{
	 public class CancelArchonBuff : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Wizard_Archon; }
		  }

		  public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=SkillExecutionFlags.RemoveBuff;
				WaitVars=new WaitLoops(3, 3, true);
				Priority = SkillPriority.None;
				UseageType=SkillUseage.OutOfCombat;
				PreCast=new SkillPreCast(SkillPrecastFlags.None);
			  
				//Important!! We have to override the default return of true.. we dont want this to fire as a combat Ability.
				FcriteriaCombat=() => false;
		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return -1; }
		  }

		  public override int GetHashCode()
		  {
				return (int)Power;
		  }

		  public override bool Equals(object obj)
		  {
				//Check for null and compare run-time types. 
				if (obj==null||GetType()!=obj.GetType())
				{
					 return false;
				}
			  Skill p=(Skill)obj;
			  return Power==p.Power;
		  }

		  #endregion


	 }
}
