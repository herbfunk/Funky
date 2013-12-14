using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class DiamondSkin : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=15000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=10;
				Counter=1;
				Range=0;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckCanCast|
				                          AbilityPreCastFlags.CheckExisitingBuff));

				FcriteriaCombat=() => (Bot.Targeting.Environment.iElitesWithinRange[(int)RangeIntervals.Range_25]>0||
				                       Bot.Targeting.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_25]>0||
				                       Bot.Character.Data.dCurrentHealthPct<=0.90||Bot.Character.Data.bIsIncapacitated||Bot.Character.Data.bIsRooted||
				                       (Bot.Targeting.CurrentTarget.RadiusDistance<=40f));
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
