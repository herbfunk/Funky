using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities
{
	 public class WeaponInstantRanged : Ability, IAbility
	 {
		  public WeaponInstantRanged()
				: base()
		  {
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.Weapon_Ranged_Instant; }
		  }

		  public override void Initialize()
		  {
				Cooldown=5;
				Range=25;
				Priority=AbilityPriority.None;
				ExecutionType=AbilityExecuteFlags.Target;
				IsRanged=true;
				IsProjectile=true;
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
