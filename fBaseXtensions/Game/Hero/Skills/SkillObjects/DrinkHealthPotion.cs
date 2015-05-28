using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects
{
	public class DrinkHealthPotion : Skill
	{
		public void AttemptToUseHealthPotion()
		{
			//Update and find best potion to use.
			//Backpack.ReturnCurrentPotions();

			ACDItem thisBestPotion = Backpack.ReturnBestPotionToUse();
			if (thisBestPotion != null)
			{
				FunkyGame.Hero.WaitWhileAnimating(4, true);
				ZetaDia.Me.Inventory.UseItem((thisBestPotion.DynamicId));
			}
			OnSuccessfullyUsed();
			FunkyGame.Hero.WaitWhileAnimating(3, true);
		}

		public override SNOPower Power
		{
			get { return SNOPower.DrinkHealthPotion; }
		}

		public override void Initialize()
		{
			Cooldown = 30000;

			WaitVars = new WaitLoops(3, 3, true);
			Priority = SkillPriority.High;


			PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);

			//Important!! We have to override the default return of true.. we dont want this to fire as a combat Ability.
			FcriteriaCombat = (u) => { return FunkyGame.Hero.dCurrentHealthPct <= FunkyBaseExtension.Settings.Combat.PotionHealthPercent; };


		}

	}
}
