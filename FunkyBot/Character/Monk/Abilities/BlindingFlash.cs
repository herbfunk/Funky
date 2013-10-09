using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.Monk
{
	 public class BlindingFlash : Ability, IAbility
	 {
		  public BlindingFlash()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=15200;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=10;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCastFlags=(AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckCanCast|
											AbilityPreCastFlags.CheckRecastTimer);

				FcriteriaCombat=new Func<bool>(() =>
				{
					 return
						 Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_15]>=1||Bot.Character.dCurrentHealthPct<=0.4||
						 (Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_20]>=5&&
						  Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_50]==0)||
						 (Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=3&&Bot.Character.dCurrentEnergyPct<=0.5)||
						 (Bot.Targeting.CurrentTarget.IsBoss&&Bot.Targeting.CurrentTarget.RadiusDistance<=15f)||
						 (Bot.Settings.Class.bMonkInnaSet&&Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_15]>=1&&
						  Bot.Class.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)&&!Bot.Class.HasBuff(SNOPower.Monk_SweepingWind))
						 &&
						  // Check if we don't have breath of heaven
						 (!Bot.Class.HotbarPowers.Contains(SNOPower.Monk_BreathOfHeaven)||
						  (Bot.Class.HotbarPowers.Contains(SNOPower.Monk_BreathOfHeaven)&&(!Bot.Settings.Class.bMonkInnaSet||
																																					  Bot.Class.HasBuff(SNOPower.Monk_BreathOfHeaven))))&&
						  // Check if either we don't have sweeping winds, or we do and it's ready to cast in a moment
						 (!Bot.Class.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)||
						  (Bot.Class.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)&&(Bot.Character.dCurrentEnergy>=95||
																										  (Bot.Settings.Class.bMonkInnaSet&&
																											Bot.Character.dCurrentEnergy>=25)||
																																				  Bot.Class.HasBuff(SNOPower.Monk_SweepingWind)))||
						  Bot.Character.dCurrentHealthPct<=0.4);
				});
		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; }
		  }

		  public override int GetHashCode()
		  {
				return (int)this.Power;
		  }

		  public override bool Equals(object obj)
		  {
				//Check for null and compare run-time types. 
				if (obj==null||this.GetType()!=obj.GetType())
				{
					 return false;
				}
				else
				{
					 Ability p=(Ability)obj;
					 return this.Power==p.Power;
				}
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.Monk_BlindingFlash; }
		  }
	 }
}
