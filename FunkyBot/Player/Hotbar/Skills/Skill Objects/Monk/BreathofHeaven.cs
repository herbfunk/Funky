using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class BreathofHeaven : Skill
	 {
		 public override void Initialize()
		  {
				//Only check for buff when correct rune is set! rune==2
				Cooldown=15000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(1, 1, true);
				UseageType=AbilityUseage.Anywhere;
				IsBuff=true;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckCanCast|AbilityPreCastFlags.CheckRecastTimer));
				FcriteriaBuff=() => Bot.Character.Data.dCurrentHealthPct<=0.5d;

				FcriteriaCombat=() => Bot.Character.Data.dCurrentHealthPct<=0.5d ||
									   Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_25]>0|| //with elites nearby..
				                       Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_25]>3;

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
				get { return SNOPower.Monk_BreathOfHeaven; }
		  }
	 }
}
