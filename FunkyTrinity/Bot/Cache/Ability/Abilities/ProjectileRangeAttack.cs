using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities
{
	public class ProjectileRangeAttack : Ability, IAbility
	{
		public ProjectileRangeAttack() : base()
		{
		}

		public override SNOPower Power
		{
			 get { return SNOPower.Weapon_Ranged_Projectile; }
		}

		protected override void Initialize()
		{
			Range = 25;
			Priority = AbilityPriority.None;
			ExecutionType = AbilityUseType.Target;
			WaitVars = new WaitLoops(0, 0, true);
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
