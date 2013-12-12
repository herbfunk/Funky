using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	 public class GroundStomp : Skill
	 {
		 public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_GroundStomp; }
		  }

		  public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=12200;
				ExecutionType=AbilityExecuteFlags.Self;
				WaitVars=new WaitLoops(1, 2, true);
				Cost=20;
				Range=16;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.Low;

				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckEnergy|
				                          AbilityPreCastFlags.CheckCanCast|AbilityPreCastFlags.CheckPlayerIncapacitated));
				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 4);
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_15, 1);
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
