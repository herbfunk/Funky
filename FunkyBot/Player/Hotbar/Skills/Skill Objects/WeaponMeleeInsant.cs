using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills
{
	 public class WeaponMeleeInsant : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Weapon_Melee_Instant; }
		  }

		  public override void Initialize()
		  {
				Cooldown=5;
				Range=8;
				Priority = AbilityPriority.None;
				ExecutionType=AbilityExecuteFlags.Target;

				WaitVars=new WaitLoops(0, 0, true);
				PreCast=new SkillPreCast(AbilityPreCastFlags.None);
				UseageType=AbilityUseage.Combat;
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
