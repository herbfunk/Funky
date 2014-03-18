using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class ArchonArcaneBlast : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=1000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(1, 1, true);
				Range=15;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckCanCast));
				IsBuff=true;
				FcriteriaBuff=() => { return false; };
				UnitsWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_6, 2);
				ElitesWithinRangeConditions=new Tuple<RangeIntervals, int>(RangeIntervals.Range_6, 1);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.IsSpecial, 8);


				FcriteriaCombat=() =>
				{
					//We only want to use this if there are nearby units!
					return Bot.Targeting.Cache.Environment.SurroundingUnits>1;
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
				get { return SNOPower.Wizard_Archon_ArcaneBlast; }
		  }
	 }
}
