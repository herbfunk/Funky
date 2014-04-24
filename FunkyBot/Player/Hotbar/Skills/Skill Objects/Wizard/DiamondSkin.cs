using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class DiamondSkin : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=15000;
				ExecutionType=SkillExecutionFlags.Buff;
				WaitVars=new WaitLoops(0, 1, true);
				UseageType=SkillUseage.Anywhere;
				Priority=SkillPriority.High;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckCanCast));

				FcriteriaBuff = () => Bot.Character.Data.dCurrentHealthPct <= 0.45d && Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_40] > 0;
				FcriteriaCombat=() => Bot.Character.Data.dCurrentHealthPct<=0.45d && Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_40]>0;
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
				get { return SNOPower.Wizard_DiamondSkin; }
		  }
	 }
}
