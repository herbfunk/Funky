using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class CycloneStrike : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=10000;
				ExecutionType=AbilityExecuteFlags.Buff;
				UseageType=AbilityUseage.Combat;
				WaitVars=new WaitLoops(2, 2, true);
				Cost=50;
				Priority=AbilityPriority.Medium;

				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckCanCast|
				                          AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckPlayerIncapacitated));

				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_20, 2);
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_20, 1);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.IsSpecial, 18);


				FcriteriaCombat=() => (!Bot.Character.Class.bWaitingForSpecial||Bot.Character.Data.dCurrentEnergy>=Bot.Character.Class.iWaitingReservedAmount);
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
