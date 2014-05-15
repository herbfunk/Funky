using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills
{
	 public class Interact : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Interact_Normal; }
		  }

		  public override void Initialize()
		  {
				Cooldown=5;
				Range = 7;
				Priority=SkillPriority.None;
				ExecutionType=SkillExecutionFlags.Target;
				WaitVars=new WaitLoops(2, 2, true);
				PreCast=new SkillPreCast(SkillPrecastFlags.None);
				UseageType=SkillUseage.Combat;
		  }

		  #region IAbility
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
