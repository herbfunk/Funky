using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class SevenSidedStrike : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=RuneIndex==3?17200:30200;
				ExecutionType=AbilityExecuteFlags.Location;
				WaitVars=new WaitLoops(2, 3, true);
				Cost=50;
				Range=16;
				Priority=AbilityPriority.Medium;
				UseageType=AbilityUseage.Combat;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckCanCast|AbilityPreCastFlags.CheckPlayerIncapacitated));

				UnitsWithinRangeConditions = new Tuple<RangeIntervals, int>(RangeIntervals.Range_20, 6);
			    ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_20, 3);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.None, 15, falseConditionalFlags: TargetProperties.Normal); //any non-normal unit!


				FcriteriaCombat=() => !Bot.Character.Class.bWaitingForSpecial||Bot.Character.Data.dCurrentEnergy>=Bot.Character.Class.iWaitingReservedAmount;
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
				get { return SNOPower.Monk_SevenSidedStrike; }
		  }
	 }
}
