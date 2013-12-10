using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class BreathofHeaven : Skill
	 {
		 public override void Initialize()
		  {
				//Only check for buff when correct rune is set! rune==2
				Cooldown=15200;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(1, 1, true);
				Cost=25;
				UseageType=AbilityUseage.Anywhere;
				IsBuff=true;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckCanCast|AbilityPreCastFlags.CheckRecastTimer));
				FcriteriaBuff=() => (RuneIndex==2&&!Bot.Character.Class.HotBar.HasBuff(SNOPower.Monk_BreathOfHeaven))||
				                    Bot.Character.Data.dCurrentHealthPct<=0.5d;
				FcriteriaCombat=
					() => (RuneIndex==2&&!Bot.Character.Class.HotBar.HasBuff(SNOPower.Monk_BreathOfHeaven))||
					      Bot.Character.Data.dCurrentHealthPct<=0.5d;
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
