using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class ShadowPower : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=5000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(1, 1, true);
				Cost=14;
				SecondaryEnergy=true;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckRecastTimer|
				                          AbilityPreCastFlags.CheckEnergy));

				FcriteriaCombat=() => (Bot.Character.Data.dCurrentHealthPct<=0.99d||Bot.Character.Data.bIsRooted||
				                       Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_25]>=1||
				                       Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3);
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
				get { return SNOPower.DemonHunter_ShadowPower; }
		  }
	 }
}
