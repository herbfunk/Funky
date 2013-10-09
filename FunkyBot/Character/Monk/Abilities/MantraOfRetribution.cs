using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.Monk
{
	 public class MantraOfRetribution : Ability, IAbility
	 {
		  public MantraOfRetribution()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=3300;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=50;
				IsBuff=true;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCastFlags=(AbilityPreCastFlags.CheckEnergy);
				IsSpecialAbility=true;
				FcriteriaBuff=new Func<bool>(() =>
				{
					 return !Bot.Class.HasBuff(Power);
				});

				FcriteriaCombat=new Func<bool>(() =>
				{

					 return
						 !Bot.Class.HasBuff(Power)
						 ||
						 Bot.Settings.Class.bMonkSpamMantra&&Bot.Targeting.CurrentTarget!=null&&
						 (Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_25]>0||
						  Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_20]>=2||
						  (Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_20]>=1&&Bot.Settings.Class.bMonkInnaSet)||
						  (Bot.Targeting.CurrentUnitTarget.IsEliteRareUnique||Bot.Targeting.CurrentTarget.IsBoss)&&
						  Bot.Targeting.CurrentTarget.RadiusDistance<=25f)&&
						  // Check if either we don't have blinding flash, or we do and it's been cast in the last 6000ms
						  //DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.Monk_BlindingFlash]).TotalMilliseconds <= 6000)) &&
						 (!Bot.Class.HotbarPowers.Contains(SNOPower.Monk_BlindingFlash)||
						  (Bot.Class.HotbarPowers.Contains(SNOPower.Monk_BlindingFlash)&&(Bot.Class.HasBuff(SNOPower.Monk_BlindingFlash))));
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
				get { return SNOPower.Monk_MantraOfRetribution; }
		  }
	 }
}
