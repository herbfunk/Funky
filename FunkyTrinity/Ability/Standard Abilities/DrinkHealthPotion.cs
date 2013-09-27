using System;
using System.Collections.Generic;
using FunkyTrinity.Cache;
using Zeta;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities
{
	 public class DrinkHealthPotion : ability, IAbility
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
				this.SuccessfullyUsed();
				Bot.Character.WaitWhileAnimating(3, true);
		  }

		  public override SNOPower Power
		  {
				get { return SNOPower.DrinkHealthPotion; }
		  }

		  public override void Initialize()
		  {

				ExecutionType=AbilityExecuteFlags.None;
				WaitVars=new WaitLoops(3, 3, true);
				Priority=AbilityPriority.High;
			
				UseageType=AbilityUseage.Anywhere;
				PreCastPreCastFlags=AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckRecastTimer;

				//Important!! We have to override the default return of true.. we dont want this to fire as a combat ability.
				FcriteriaCombat=new Func<bool>(() => { return Bot.Character.dCurrentHealthPct<=Bot.EmergencyHealthPotionLimit; });

				
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
					 ability p=(ability)obj;
					 return this.Power==p.Power;
				}
		  }

		  #endregion


	 }
}
