using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities
{
	public class WeaponMeleeInsant : ability, IAbility
	{
		public WeaponMeleeInsant() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Weapon_Melee_Instant; }
		}

		public override void Initialize()
		{
			Range = 8;
			Priority = AbilityPriority.None;
			ExecutionType = AbilityExecuteFlags.Target;
		
			WaitVars = new WaitLoops(0, 0, true);
			PreCastPreCastFlags=AbilityPreCastFlags.None;
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
				  ability p=(ability)obj;
				  return this.Power==p.Power;
			 }
		}
	

		#endregion
	}
}
