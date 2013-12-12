using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class WaveOfForce : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=12000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(1, 2, true);
				Cost=25;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckEnergy|
				                          AbilityPreCastFlags.CheckCanCast|AbilityPreCastFlags.CheckRecastTimer));

				FcriteriaCombat=() => ((UsingCriticalMass()&&
				                        ((!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Wizard_FrostNova)&&
				                          !Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Wizard_ExplosiveBlast))||
				                         (Bot.Character.Data.dCurrentHealthPct<=0.7&&
				                          (Bot.Targeting.Environment.iElitesWithinRange[(int)RangeIntervals.Range_15]>0||
				                           Bot.Targeting.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>0||
				                           (Bot.Targeting.CurrentTarget.ObjectIsSpecial&&Bot.Targeting.CurrentTarget.RadiusDistance<=23f)))))
					// Else normal wizard in which case check standard stuff
				                       ||
				                       (!UsingCriticalMass()&&Bot.Targeting.Environment.iElitesWithinRange[(int)RangeIntervals.Range_15]>0||
				                        Bot.Targeting.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>3||Bot.Character.Data.dCurrentHealthPct<=0.7||
				                        (Bot.Targeting.CurrentTarget.ObjectIsSpecial&&Bot.Targeting.CurrentTarget.RadiusDistance<=23f)));
		  }

		  private bool UsingCriticalMass()
		  {
				return Bot.Character.Class.HotBar.PassivePowers.Contains(SNOPower.Wizard_Passive_CriticalMass); ;
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
				get { return SNOPower.Wizard_WaveOfForce; }
		  }
	 }
}
