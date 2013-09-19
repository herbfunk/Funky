using System;
using System.Collections.Generic;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities
{
	public class CancelArchonBuff : Ability, IAbility
	{
		public CancelArchonBuff() : base()
		{
		}

		public override SNOPower Power
		{
			 get { return SNOPower.Wizard_Archon; }
		}

		private bool MissingBuffs()
		{
			 HashSet<SNOPower> abilities_=Bot.Class.CachedPowers;

			 if ((abilities_.Contains(SNOPower.Wizard_EnergyArmor)&&!Bot.Class.HasBuff(SNOPower.Wizard_EnergyArmor))||
				  (abilities_.Contains(SNOPower.Wizard_IceArmor)&&!Bot.Class.HasBuff(SNOPower.Wizard_IceArmor))||
				  (abilities_.Contains(SNOPower.Wizard_StormArmor)&&!Bot.Class.HasBuff(SNOPower.Wizard_StormArmor)))
				  return true;

			 if (abilities_.Contains(SNOPower.Wizard_MagicWeapon)&&!Bot.Class.HasBuff(SNOPower.Wizard_MagicWeapon))
				  return true;

			 return false;
		}

		public override void Initialize()
		{
			ExecutionType = AbilityUseType.RemoveBuff;
			WaitVars = new WaitLoops(3, 3, true);
			IsBuff=true;
			Priority=AbilityPriority.High;
			UseageType=AbilityUseage.OutOfCombat;
			PreCastConditions=AbilityConditions.None;

			Fbuff=new Func<bool>(() =>
			{
				 return Bot.Class.HasBuff(SNOPower.Wizard_Archon)&&this.MissingBuffs();
			});

		   //Important!! We have to override the default return of true.. we dont want this to fire as a combat ability.
			Fcriteria=new Func<bool>(() => { return false; });
		}

		public override void InitCriteria()
		{
			 base.AbilityTestConditions=new AbilityUsablityTests(this);
		}

		#region IAbility

		public override int RuneIndex
		{
			 get { return -1; }
		}

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
