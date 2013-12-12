using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills
{
	 public class DrinkHealthPotion : Skill
	 {
		 public void AttemptToUseHealthPotion()
		  {
				//Update and find best potion to use.
				Bot.Character.Data.BackPack.ReturnCurrentPotions();

				ACDItem thisBestPotion=Bot.Character.Data.BackPack.BestPotionToUse;
				if (thisBestPotion!=null)
				{
					 Bot.Character.Data.WaitWhileAnimating(4, true);
					 ZetaDia.Me.Inventory.UseItem((thisBestPotion.DynamicId));
				}
				OnSuccessfullyUsed();
				Bot.Character.Data.WaitWhileAnimating(3, true);
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
				PreCast=new SkillPreCast(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckRecastTimer);

				//Important!! We have to override the default return of true.. we dont want this to fire as a combat Ability.
				FcriteriaCombat=() => { return Bot.Character.Data.dCurrentHealthPct<=Bot.Settings.Combat.PotionHealthPercent; };


		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return -1; }
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


	 }
}
