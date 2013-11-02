using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.Monk
{
	 public class SweepingWind : Ability, IAbility
	 {
		  public SweepingWind()
				: base()
		  {
		  }


		  public override void Initialize()
		  {
				Cooldown=6000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=Bot.Settings.Class.bMonkInnaSet?5:75;
				Priority=AbilityPriority.High;
				UseageType=AbilityUseage.Combat;
				IsSpecialAbility=true;

				PreCastFlags=(AbilityPreCastFlags.CheckEnergy);

				ClusterConditions=new ClusterConditions(7d, 35f, 2, false);
				TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 25);
                IsBuff = true;
                FcriteriaBuff = new Func<bool>(() =>
                {
                    //Rune index of 4 increases duration of buff to 20 seconds..
                    int buffDuration = this.RuneIndex == 4 ? 17500 : 4500;

                    if (Bot.Settings.Class.bMonkMaintainSweepingWind &&  //Maintaining Sweeping Wind (Must already have buff.. and has not used combat ability within 2000ms!)
                        DateTime.Now.Subtract(Bot.Class.LastUsedACombatAbility).TotalMilliseconds > 2000 &&
                        this.LastUsedMilliseconds > buffDuration)
                    {
                        return true;
                    }

                    return false;
                });
                FcriteriaCombat = new Func<bool>(() =>
                {
                    if (!Bot.Class.CurrentBuffs.ContainsKey((int)SNOPower.Monk_SweepingWind))
                        return true;

                    return false;
                });
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
				get { return SNOPower.Monk_SweepingWind; }
		  }
	 }
}
