using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class SmokeScreen : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=3000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=28;
				SecondaryEnergy=true;
				Range=0;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				//PreCastFlags=,

				FcriteriaCombat=() => (!Bot.Character.Class.HotBar.HasBuff(SNOPower.DemonHunter_ShadowPower)||Bot.Character.Data.bIsIncapacitated)
				                      &&(Bot.Character.Data.dDiscipline>=28)
				                      &&
				                      (Bot.Character.Data.dCurrentHealthPct<=0.90||Bot.Character.Data.bIsRooted||
				                       Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_20]>=1||
				                       Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3||Bot.Character.Data.bIsIncapacitated);
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
				else
				{
					 Skill p=(Skill)obj;
					 return Power==p.Power;
				}
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_SmokeScreen; }
		  }
	 }
}
