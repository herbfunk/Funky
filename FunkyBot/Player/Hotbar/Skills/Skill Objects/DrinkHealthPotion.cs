using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills
{
	 public class DrinkHealthPotion : Skill
	 {
		 public void AttemptToUseHealthPotion()
		  {
				//Update and find best potion to use.
				//Bot.Character.Data.BackPack.ReturnCurrentPotions();

				ACDItem thisBestPotion = Bot.Character.Data.BackPack.ReturnBestPotionToUse();
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
				
				WaitVars=new WaitLoops(3, 3, true);
				Priority=SkillPriority.High;

				
				PreCast=new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckRecastTimer);

				//Important!! We have to override the default return of true.. we dont want this to fire as a combat Ability.
				FcriteriaCombat=() => { return Bot.Character.Data.dCurrentHealthPct<=Bot.Settings.Combat.PotionHealthPercent; };


		  }

	 }
}
