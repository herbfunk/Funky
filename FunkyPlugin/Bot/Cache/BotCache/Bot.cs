using System;
using Zeta;

namespace FunkyTrinity
{
	public partial class Funky
	{
		//This class is used to hold the data

		public partial class Bot
		{
			 internal static CharacterInfo Class { get; set; }
			internal static CharacterCache Character { get; set; }
			internal static CombatCache Combat { get; set; }
			internal static TargetHandler Target { get; set; }


			// Death counts
			internal static int iMaxDeathsAllowed=0;
			internal static int iDeathsThisRun=0;
			// On death, clear the timers for all abilities
			internal static DateTime lastDied=DateTime.Today;
			internal static int iTotalDeaths=0;
			// How many total leave games, for stat-tracking?
			internal static int iTotalJoinGames=0;
			// How many total leave games, for stat-tracking?
			internal static int iTotalLeaveGames=0;
			internal static int iTotalProfileRecycles=0;

			internal static void AttemptToUseHealthPotion()
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

			internal static void Reset()
			{
				 Class=null;
				 Character=new CharacterCache();
				 Combat=new CombatCache();
				 Target=new TargetHandler();
			}

		}
	}
}