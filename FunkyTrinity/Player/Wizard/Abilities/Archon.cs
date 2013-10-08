using System;
using System.Collections.Generic;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.Wizard
{
	 public class Archon : Ability, IAbility
	 {
		  public Archon()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=100000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(4, 5, true);
				Cost=25;
				UseageType=AbilityUseage.Combat;
				IsSpecialAbility=true;
				Priority=AbilityPriority.High;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast|
											AbilityPreCastFlags.CheckEnergy);
				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 1);
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_30, 1);
				TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 30);
				FcriteriaCombat=new Func<bool>(() =>
				{
					 bool missingBuffs=this.MissingBuffs();
					 if (missingBuffs)
						  Bot.Class.bWaitingForSpecial=true;

					 return !missingBuffs;
				});
		  }

		  private bool MissingBuffs()
		  {
				HashSet<SNOPower> abilities_=Bot.Class.HasBuff(SNOPower.Wizard_Archon)?Bot.Class.CachedPowers:Bot.Class.HotbarPowers;

				if ((abilities_.Contains(SNOPower.Wizard_EnergyArmor)&&!Bot.Class.HasBuff(SNOPower.Wizard_EnergyArmor))||(abilities_.Contains(SNOPower.Wizard_IceArmor)&&!Bot.Class.HasBuff(SNOPower.Wizard_IceArmor))||(abilities_.Contains(SNOPower.Wizard_StormArmor)&&!Bot.Class.HasBuff(SNOPower.Wizard_StormArmor)))
					 return true;

				if (abilities_.Contains(SNOPower.Wizard_MagicWeapon)&&!Bot.Class.HasBuff(SNOPower.Wizard_MagicWeapon))
					 return true;

				return false;

		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; }
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

		  public override SNOPower Power
		  {
				get { return SNOPower.Wizard_Archon; }
		  }
	 }
}
