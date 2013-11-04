using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.Wizard
{
	 public class WaveOfForce : Ability, IAbility
	 {
		  public WaveOfForce()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=12000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(1, 2, true);
				Cost=25;
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckEnergy|
											AbilityPreCastFlags.CheckCanCast|AbilityPreCastFlags.CheckRecastTimer);

				FcriteriaCombat=new Func<bool>(() =>
				{
					 return
						  // Check this isn't a critical mass wizard, cos they won't want to use this except for low health unless they don't have nova/blast in which case go for it
						 ((UsingCriticalMass()&&
							((!Bot.Class.HotBar.HotbarPowers.Contains(SNOPower.Wizard_FrostNova)&&
							  !Bot.Class.HotBar.HotbarPowers.Contains(SNOPower.Wizard_ExplosiveBlast))||
							 (Bot.Character.dCurrentHealthPct<=0.7&&
							  (Bot.Targeting.Environment.iElitesWithinRange[(int)RangeIntervals.Range_15]>0||
								Bot.Targeting.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>0||
								(Bot.Targeting.CurrentTarget.ObjectIsSpecial&&Bot.Targeting.CurrentTarget.RadiusDistance<=23f)))))
						  // Else normal wizard in which case check standard stuff
						  ||
						  (!UsingCriticalMass()&&Bot.Targeting.Environment.iElitesWithinRange[(int)RangeIntervals.Range_15]>0||
							Bot.Targeting.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]>3||Bot.Character.dCurrentHealthPct<=0.7||
							(Bot.Targeting.CurrentTarget.ObjectIsSpecial&&Bot.Targeting.CurrentTarget.RadiusDistance<=23f)));
				});
		  }

		  private bool UsingCriticalMass()
		  {
				return Bot.Class.HotBar.PassivePowers.Contains(SNOPower.Wizard_Passive_CriticalMass); ;
		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Class.HotBar.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.HotBar.RuneIndexCache[this.Power]:-1; }
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
				get { return SNOPower.Wizard_WaveOfForce; }
		  }
	 }
}
