using System;
using System.Collections.Generic;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class Archon : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=100000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(4, 5, true);
				Cost=25;
				UseageType=AbilityUseage.Combat;
				IsSpecialAbility=true;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast|
				                          AbilityPreCastFlags.CheckEnergy));
				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_30, 1);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.IsSpecial, 30);
				FcriteriaCombat=() =>
				{
					bool missingBuffs=MissingBuffs();
					if (missingBuffs)
						Bot.Character.Class.bWaitingForSpecial=true;

					return !missingBuffs;
				};
		  }

		  private bool MissingBuffs()
		  {
				HashSet<SNOPower> abilities_=Bot.Character.Class.HotBar.HasBuff(SNOPower.Wizard_Archon)?Bot.Character.Class.HotBar.CachedPowers:Bot.Character.Class.HotBar.HotbarPowers;

				if ((abilities_.Contains(SNOPower.Wizard_EnergyArmor)&&!Bot.Character.Class.HotBar.HasBuff(SNOPower.Wizard_EnergyArmor))||(abilities_.Contains(SNOPower.Wizard_IceArmor)&&!Bot.Character.Class.HotBar.HasBuff(SNOPower.Wizard_IceArmor))||(abilities_.Contains(SNOPower.Wizard_StormArmor)&&!Bot.Character.Class.HotBar.HasBuff(SNOPower.Wizard_StormArmor)))
					 return true;

				if (abilities_.Contains(SNOPower.Wizard_MagicWeapon)&&!Bot.Character.Class.HotBar.HasBuff(SNOPower.Wizard_MagicWeapon))
					 return true;

				return false;

		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; }
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

		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_Archon; }
		  }
	 }
}
