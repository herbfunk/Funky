using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	 public class Earthquake : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_Earthquake; }
		  }

		  public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=120500;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(4, 4, true);
				Cost=0;
				UseageType=AbilityUseage.Combat;
				IsSpecialAbility=true;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckEnergy|
				                          AbilityPreCastFlags.CheckExisitingBuff|AbilityPreCastFlags.CheckCanCast|
				                          AbilityPreCastFlags.CheckPlayerIncapacitated));
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.IsSpecial, 13);
		  }

		  #region IAbility
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
				else
				{
					 Skill p=(Skill)obj;
					 return Power==p.Power;
				}
		  }


		  #endregion
	 }
}
