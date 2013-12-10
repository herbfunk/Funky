using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class BlindingFlash : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=15200;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=10;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckCanCast|
				                          AbilityPreCastFlags.CheckRecastTimer));

				FcriteriaCombat=() => Bot.Targeting.Environment.iElitesWithinRange[(int)RangeIntervals.Range_15]>=1||Bot.Character.Data.dCurrentHealthPct<=0.4||
				                      (Bot.Targeting.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20]>=5&&
				                       Bot.Targeting.Environment.iElitesWithinRange[(int)RangeIntervals.Range_50]==0)||
				                      (Bot.Targeting.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3&&Bot.Character.Data.dCurrentEnergyPct<=0.5)||
				                      (Bot.Targeting.CurrentTarget.IsBoss&&Bot.Targeting.CurrentTarget.RadiusDistance<=15f)||
				                      (Bot.Settings.Class.bMonkInnaSet&&Bot.Targeting.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=1&&
				                       Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)&&!Bot.Character.Class.HotBar.HasBuff(SNOPower.Monk_SweepingWind))
				                      &&
				                      // Check if we don't have breath of heaven
				                      (!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_BreathOfHeaven)||
				                       (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_BreathOfHeaven)&&(!Bot.Settings.Class.bMonkInnaSet||
				                                                                                                         Bot.Character.Class.HotBar.HasBuff(SNOPower.Monk_BreathOfHeaven))))&&
				                      // Check if either we don't have sweeping winds, or we do and it's ready to cast in a moment
				                      (!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)||
				                       (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)&&(Bot.Character.Data.dCurrentEnergy>=95||
				                                                                                                       (Bot.Settings.Class.bMonkInnaSet&&
				                                                                                                        Bot.Character.Data.dCurrentEnergy>=25)||
				                                                                                                       Bot.Character.Class.HotBar.HasBuff(SNOPower.Monk_SweepingWind)))||
				                       Bot.Character.Data.dCurrentHealthPct<=0.4);
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
				get { return SNOPower.Monk_BlindingFlash; }
		  }
	 }
}
