using System;
using System.Collections.Generic;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities
{
	 public class CancelArchonBuff : Ability, IAbility
	 {
		  public CancelArchonBuff()
				: base()
		  {
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_Archon; }
		  }

		  private bool MissingBuffs()
		  {
				HashSet<SNOPower> abilities_=Bot.Class.HotBar.CachedPowers;

				if ((abilities_.Contains(SNOPower.Wizard_EnergyArmor)&&!Bot.Class.HotBar.HasBuff(SNOPower.Wizard_EnergyArmor))||
					 (abilities_.Contains(SNOPower.Wizard_IceArmor)&&!Bot.Class.HotBar.HasBuff(SNOPower.Wizard_IceArmor))||
					 (abilities_.Contains(SNOPower.Wizard_StormArmor)&&!Bot.Class.HotBar.HasBuff(SNOPower.Wizard_StormArmor)))
					 return true;

				if (abilities_.Contains(SNOPower.Wizard_MagicWeapon)&&!Bot.Class.HotBar.HasBuff(SNOPower.Wizard_MagicWeapon))
					 return true;

				return false;
		  }

		  public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=AbilityExecuteFlags.RemoveBuff;
				WaitVars=new WaitLoops(3, 3, true);
				IsBuff=true;
				Priority=AbilityPriority.None;
				UseageType=AbilityUseage.OutOfCombat;
				PreCastFlags=AbilityPreCastFlags.None;

				FcriteriaBuff=new Func<bool>(() =>
				{
					 return Bot.Class.HotBar.HasBuff(SNOPower.Wizard_Archon)&&this.MissingBuffs();
				});

				//Important!! We have to override the default return of true.. we dont want this to fire as a combat Ability.
				FcriteriaCombat=new Func<bool>(() => { return false; });
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
