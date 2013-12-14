using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class SweepingWind : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=6000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=Bot.Settings.Class.bMonkInnaSet?5:75;
				Priority=AbilityPriority.High;
				UseageType=AbilityUseage.Combat;
				IsSpecialAbility=true;

				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckEnergy));

				ClusterConditions=new SkillClusterConditions(7d, 35f, 2, false);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.IsSpecial, 25);
                IsBuff = true;
                FcriteriaBuff = () =>
                {
	                //Rune index of 4 increases duration of buff to 20 seconds..
	                int buffDuration = RuneIndex == 4 ? 17500 : 4500;

	                if (Bot.Settings.Class.bMonkMaintainSweepingWind &&  //Maintaining Sweeping Wind (Must already have buff.. and has not used combat ability within 2000ms!)
	                    DateTime.Now.Subtract(Bot.Character.Class.LastUsedACombatAbility).TotalMilliseconds > 2000 &&
	                    LastUsedMilliseconds > buffDuration)
	                {
		                return true;
	                }

	                return false;
                };
                FcriteriaCombat = () =>
                {
	                if (!Bot.Character.Class.HotBar.CurrentBuffs.ContainsKey((int)SNOPower.Monk_SweepingWind))
		                return true;

	                return false;
                };
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
				get { return SNOPower.Monk_SweepingWind; }
		  }
	 }
}
