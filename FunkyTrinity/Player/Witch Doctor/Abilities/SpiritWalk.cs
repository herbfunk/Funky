using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.WitchDoctor
{
	 public class SpiritWalk : Ability, IAbility
	 {
		  public SpiritWalk()
				: base()
		  {
		  }


		  public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }


		  public override void Initialize()
		  {
				Cooldown=15200;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(0, 0, true);
				Cost=49;
				UseageType=AbilityUseage.Anywhere;
				IsSpecialAbility=true;
				Priority=AbilityPriority.High;
				PreCastFlags=(AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckCanCast);

				IsBuff=true;
				FcriteriaBuff=new Func<bool>(() =>
				{
					 return Bot.Settings.OutOfCombatMovement;
				});

				FcriteriaCombat=new Func<bool>(() =>
				{
					 return (Bot.Character.dCurrentHealthPct<=0.65
								||(Bot.Combat.FleeTriggeringUnits.Count>0)
								||(Bot.Combat.TriggeringAvoidances.Count>0)
								||Bot.Character.bIsIncapacitated
								||Bot.Character.bIsRooted);
				});
		  }

		  #region IAbility


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
				get { return SNOPower.Witchdoctor_SpiritWalk; }
		  }
	 }
}
