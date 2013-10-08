using System;
using System.Collections.Generic;
using FunkyBot.Cache;
using Zeta;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities
{
	 public class DrinkHealthPotion : Ability, IAbility
	 {
		  public DrinkHealthPotion()
				: base()
		  {
		  }

		  public void AttemptToUseHealthPotion()
		  {
				//Update and find best potion to use.
				Bot.Character.BackPack.ReturnCurrentPotions();

				Zeta.Internals.Actors.ACDItem thisBestPotion=Bot.Character.BackPack.BestPotionToUse;
				if (thisBestPotion!=null)
				{
					 Bot.Character.WaitWhileAnimating(4, true);
					 ZetaDia.Me.Inventory.UseItem((thisBestPotion.DynamicId));
				}
				this.OnSuccessfullyUsed();
				Bot.Character.WaitWhileAnimating(3, true);
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.DrinkHealthPotion; }
		  }

		  public override void Initialize()
		  {
				Cooldown=30000;
				ExecutionType=AbilityExecuteFlags.None;
				WaitVars=new WaitLoops(3, 3, true);
				Priority=AbilityPriority.High;

				UseageType=AbilityUseage.Anywhere;
				PreCastFlags=AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckRecastTimer;

				//Important!! We have to override the default return of true.. we dont want this to fire as a combat Ability.
				FcriteriaCombat=new Func<bool>(() => { return Bot.Character.dCurrentHealthPct<=Bot.Settings.Combat.PotionHealthPercent; });


		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return -1; }
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


	 }
}
