using System.Collections.Generic;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills
{
	 public class CancelArchonBuff : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Wizard_Archon; }
		  }

		  private bool MissingBuffs()
		  {
				HashSet<SNOPower> abilities_=Bot.Character.Class.HotBar.CachedPowers;

				if ((abilities_.Contains(SNOPower.Wizard_EnergyArmor)&&!Bot.Character.Class.HotBar.HasBuff(SNOPower.Wizard_EnergyArmor))||
					 (abilities_.Contains(SNOPower.Wizard_IceArmor)&&!Bot.Character.Class.HotBar.HasBuff(SNOPower.Wizard_IceArmor))||
					 (abilities_.Contains(SNOPower.Wizard_StormArmor)&&!Bot.Character.Class.HotBar.HasBuff(SNOPower.Wizard_StormArmor)))
					 return true;

				if (abilities_.Contains(SNOPower.Wizard_MagicWeapon)&&!Bot.Character.Class.HotBar.HasBuff(SNOPower.Wizard_MagicWeapon))
					 return true;

				return false;
		  }

		  public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=AbilityExecuteFlags.RemoveBuff;
				WaitVars=new WaitLoops(3, 3, true);
				IsBuff=true;
				Priority = AbilityPriority.None;
				UseageType=AbilityUseage.OutOfCombat;
				PreCast=new SkillPreCast(AbilityPreCastFlags.None);
			  
				FcriteriaBuff=() => Bot.Character.Class.HotBar.HasBuff(SNOPower.Wizard_Archon)&&MissingBuffs();

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
