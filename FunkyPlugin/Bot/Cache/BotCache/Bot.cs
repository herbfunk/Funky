using System;
using Zeta;

namespace FunkyTrinity
{
	public partial class Funky
	{
		//This class is used to hold the data

		public partial class Bot
		{
			public static CharacterInfo Class { get; set; }
			public static CharacterCache Character { get; set; }
			public static CombatCache Combat { get; set; }
			public static TargetHandler Target { get; set; }

			public static void AttemptToUseHealthPotion()
			{
				 //Update and find best potion to use.
				 Character.BackPack.ReturnCurrentPotions();

				 Zeta.Internals.Actors.ACDItem thisBestPotion=Character.BackPack.BestPotionToUse;
				 if (thisBestPotion!=null)
				 {
					  WaitWhileAnimating(4, true);
					  ZetaDia.Me.Inventory.UseItem((thisBestPotion.DynamicId));
				 }
				 dictAbilityLastUse[Zeta.Internals.Actors.SNOPower.DrinkHealthPotion]=DateTime.Now;
				 WaitWhileAnimating(3, true);
			}

			public static void Reset()
			{
				 Class=null;
				 Character=new CharacterCache();
				 Combat=new CombatCache();
				 Target=new TargetHandler();
			}

		}
	}
}