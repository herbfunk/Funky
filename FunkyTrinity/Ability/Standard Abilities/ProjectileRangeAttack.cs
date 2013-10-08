using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities
{
	 public class ProjectileRangeAttack : Ability, IAbility
	 {
		  public ProjectileRangeAttack()
				: base()
		  {
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.Weapon_Ranged_Projectile; }
		  }

		  public override void Initialize()
		  {
				Cooldown=5;
				Range=25;
				Priority=AbilityPriority.None;
				ExecutionType=AbilityExecuteFlags.Target;
				IsProjectile=true;
				IsRanged=true;
				WaitVars=new WaitLoops(0, 0, true);
				PreCastFlags=AbilityPreCastFlags.None;
				UseageType=AbilityUseage.Combat;
		  }

		  #region IAbility
		  public override int GetHashCode()
		  {
				return (int)this.Power;
		  }
		  public override bool Equals(object obj)
		  {
				//Check for null and compare run-time types. 
				if (obj==null||this.GetType()!=obj.GetType())
				{
					 return false;
				}
				else
				{
					 Ability p=(Ability)obj;
					 return this.Power==p.Power;
				}
		  }

		  #endregion
	 }
}
