using System;
using FunkyBot.Movement.Clustering;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class CycloneStrike : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=1000;
				ExecutionType=AbilityExecuteFlags.Buff;
				UseageType=AbilityUseage.Combat;
				WaitVars=new WaitLoops(2, 2, true);
				Cost=50;
				Priority=AbilityPriority.High;

				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckCanCast|
				                          AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckPlayerIncapacitated));

				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 7);
				//ElitesWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_25, 3);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.Fast|TargetProperties.IsSpecial, 23);
				ClusterConditions = new SkillClusterConditions(8d, 20f, 4, false, 0, ClusterProperties.Large, 10f, false);
				FcriteriaCombat = () =>
				{
					if (LastConditionPassed == ConditionCriteraTypes.SingleTarget) return true; //special and fast..

					//Use every 5.5s when 7+ units are within 25f.
					if (LastConditionPassed == ConditionCriteraTypes.UnitsInRange && LastUsedMilliseconds > 5500 && !Bot.Character.Class.bWaitingForSpecial)
						return true;

					if (!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_ExplodingPalm)) return true; //Non Exploding Palm Check

					if ((Bot.Targeting.Cache.CurrentUnitTarget.HasDOTdps.HasValue && Bot.Targeting.Cache.CurrentUnitTarget.HasDOTdps.Value)
						&& Bot.Targeting.Cache.CurrentUnitTarget.CurrentHealthPct<0.10d)
					{
						return true;
					}

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
				get { return SNOPower.Monk_CycloneStrike; }
		  }
	 }
}
