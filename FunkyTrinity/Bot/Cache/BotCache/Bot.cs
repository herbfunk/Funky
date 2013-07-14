using System;
using Zeta;
using Zeta.CommonBot;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.Actors;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  //This class is used to hold the data

		  public static partial class Bot
		  {
				internal static Settings_Funky SettingsFunky=new Settings_Funky(false, false, false, false, false, false, false, 4, 8, 3, 1.5d, true, 20, false, false, "hard", "Rare", "Rare", true, false, 0, 10, 30, 60, 0.6d, 0.4d, true, 2, 250, false, 60, 30, 40, new int[1], new int[1], new int[1], 1, 100, 300, new bool[3], 60, true, true, true, 59, false, 75000, 25000, 25000, false, false);
				public static Player Class { get; set; }
				public static CharacterCache Character { get; set; }
				public static CombatCache Combat { get; set; }
				public static TargetHandler Target { get; set; }

				///<summary>
				///Usable Objects -- refresh inside Target.UpdateTarget
				///</summary>
				internal static System.Collections.Generic.List<CacheObject> ValidObjects { get; set; }

				internal static Zeta.Internals.Actors.ActorClass ActorClass=Zeta.Internals.Actors.ActorClass.Invalid;
				internal static string CurrentAccountName;
				internal static string CurrentHeroName;
				internal static void UpdateCurrentAccountDetails()
				{
					 ActorClass=Zeta.ZetaDia.Service.CurrentHero.Class;
					 CurrentAccountName=Zeta.ZetaDia.Service.CurrentHero.BattleTagName;
					 CurrentHeroName=Zeta.ZetaDia.Service.CurrentHero.Name;
				}

				///<summary>
				///Checks behavioral flags that are considered OOC/Non-Combat
				///</summary>
				internal static bool IsInNonCombatBehavior
				{
					 get
					 {
						  //OOC IDing, Town Portal Casting, Town Run
						  return (Character.IsRunningOOCBehavior||shouldPreformOOCItemIDing||FunkyTPBehaviorFlag||TownRunManager.bWantToTownRun);
					 }
				}

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


				internal static float iCurrentMaxKillRadius=0f;
				internal static float iCurrentMaxLootRadius=0f;
				internal static void UpdateKillLootRadiusValues()
				{
					 iCurrentMaxKillRadius=Zeta.CommonBot.Settings.CharacterSettings.Instance.KillRadius;
					 iCurrentMaxLootRadius=Zeta.CommonBot.Settings.CharacterSettings.Instance.LootRadius;
					 // Not allowed to kill monsters due to profile/routine/combat targeting settings - just set the kill range to a third
					 if (!ProfileManager.CurrentProfile.KillMonsters)
					 {
						  iCurrentMaxKillRadius/=3;
					 }
					 // Always have a minimum kill radius, so we're never getting whacked without retaliating
					 if (iCurrentMaxKillRadius<10||Bot.SettingsFunky.IgnoreCombatRange)
						  iCurrentMaxKillRadius=10;

					 if (shouldPreformOOCItemIDing||FunkyTPBehaviorFlag||TownRunManager.bWantToTownRun)
						  iCurrentMaxKillRadius=50;

					 // Not allowed to loots due to profile/routine/loot targeting settings - just set range to a quarter
					 if (!ProfileManager.CurrentProfile.PickupLoot)
					 {
						  iCurrentMaxLootRadius/=4;
					 }

					 //Ignore Loot Range Setting
					 if (Bot.SettingsFunky.IgnoreLootRange)
						  iCurrentMaxLootRadius=10;


					 // Counter for how many cycles we extend or reduce our attack/kill radius, and our loot radius, after a last kill
					 if (iKeepKillRadiusExtendedFor>0)
						  iKeepKillRadiusExtendedFor--;
					 if (iKeepLootRadiusExtendedFor>0)
						  iKeepLootRadiusExtendedFor--;
				}

				#region SettingsRangeValues
				internal static int KiteDistance
				{
					 get
					 {
						  return Bot.SettingsFunky.KiteDistance;
					 }
				}
				internal static int ContainerRange
				{
					 get
					 {
						  return Bot.SettingsFunky.ContainerOpenRange;
					 }
				}
				internal static int NonEliteRange
				{
					 get
					 {
						  return Bot.SettingsFunky.NonEliteCombatRange;
					 }
				}
				internal static int EliteRange
				{
					 get
					 {
						  return Bot.SettingsFunky.EliteCombatRange;
					 }
				}
				internal static int GlobeRange
				{
					 get
					 {
						  return Bot.SettingsFunky.GlobeRange;
					 }
				}
				internal static int ItemRange
				{
					 get
					 {
						  return Bot.SettingsFunky.ItemRange;
					 }
				}
				internal static int GoldRange
				{
					 get
					 {
						  return Bot.SettingsFunky.GoldRange;
					 }
				}
				internal static int DestructibleRange
				{
					 get
					 {
						  return Bot.SettingsFunky.DestructibleRange;
					 }
				}
				internal static int TreasureGoblinRange
				{
					 get
					 {
						  return Bot.SettingsFunky.TreasureGoblinRange;
					 }
				}
				internal static int ShrineRange
				{
					 get
					 {
						  return Bot.SettingsFunky.ShrineRange;
					 }
				}
				internal static double EmergencyHealthPotionLimit
				{
					 get
					 {
						  return Bot.SettingsFunky.PotionHealthPercent;
					 }
				}
				internal static double EmergencyHealthGlobeLimit
				{
					 get
					 {
						  return Bot.SettingsFunky.GlobeHealthPercent;
					 }
				}
				#endregion

				#region Avoidances
				///<summary>
				///Returns a specific dictionary according to the bots character flags.
				///</summary>
				internal static Dictionary<AvoidanceType, double> AvoidancesHealth
				{
					 get
					 {
						  if (Combat.CriticalAvoidance||IsInNonCombatBehavior)
								return dictAvoidanceHealthOOCIDBehaviorDefaults;
						  else
								return ReturnDictionaryUsingActorClass(Class.AC);
					 }
				}

				internal static bool IgnoringAvoidanceType(AvoidanceType thisAvoidance)
				{
					 if (!Bot.SettingsFunky.AttemptAvoidanceMovements)
						  return true;

					 double dThisHealthAvoid;
					 if (!AvoidancesHealth.TryGetValue(thisAvoidance, out dThisHealthAvoid))
						  return true;
					 else if (dThisHealthAvoid==0d)
						  return true;

					 return false;
				}

				///<summary>
				///Tests the given avoidance type to see if it should be ignored either due to a buff or if health is greater than the avoidance HP.
				///</summary>
				internal static bool IgnoreAvoidance(AvoidanceType thisAvoidance)
				{
					 double dThisHealthAvoid;
					 if (!AvoidancesHealth.TryGetValue(thisAvoidance, out dThisHealthAvoid))
						  return true;

					 if (!Combat.CriticalAvoidance)
					 {//Not Critical Avoidance, should we be in total ignorance because of a buff?

						  // Monks with Serenity up ignore all AOE's
						  if (Class.AC==ActorClass.Monk&&Class.HotbarPowers.Contains(SNOPower.Monk_Serenity)&&Class.HasBuff(SNOPower.Monk_Serenity))
						  {
								// Monks with serenity are immune
								return true;

						  }// Witch doctors with spirit walk available and not currently Spirit Walking will subtly ignore ice balls, arcane, desecrator & plague cloud
						  else if (Class.AC==ActorClass.WitchDoctor
								&&Class.HotbarPowers.Contains(SNOPower.Witchdoctor_SpiritWalk)
								&&(!Class.HasBuff(SNOPower.Witchdoctor_SpiritWalk)&&Class.AbilityUseTimer(SNOPower.Witchdoctor_SpiritWalk))||Class.HasBuff(SNOPower.Witchdoctor_SpiritWalk))
						  {
								switch (thisAvoidance)
								{
									 case AvoidanceType.Frozen:
									 case AvoidanceType.ArcaneSentry:
									 case AvoidanceType.Dececrator:
									 case AvoidanceType.PlagueCloud:
										  return true;
								}
						  }
						  else if (Class.AC==ActorClass.Barbarian&&Class.HotbarPowers.Contains(SNOPower.Barbarian_WrathOfTheBerserker)&&Class.HasBuff(SNOPower.Barbarian_WrathOfTheBerserker))
						  {
								switch (thisAvoidance)
								{
									 case AvoidanceType.Frozen:
									 case AvoidanceType.ArcaneSentry:
									 case AvoidanceType.Dececrator:
									 case AvoidanceType.PlagueCloud:
										  return true;
								}
						  }
					 }

					 //Only procedee if health percent is necessary for avoidance!
					 return dThisHealthAvoid<Character.dCurrentHealthPct;
				}

				#endregion

				private static HashSet<int> hashActorSNOKitingIgnore_;
				internal static HashSet<int> HashActorSNOKitingIgnore
				{
					 get 
					 {
						  if (hashActorSNOKitingIgnore_==null)
						  {
								hashActorSNOKitingIgnore_=new HashSet<int> { 4095, 144315 };
								//burrowing units
								hashActorSNOKitingIgnore_.UnionWith(SnoCacheLookup.hashActorSNOBurrowableUnits);
								//grunts
								hashActorSNOKitingIgnore_.UnionWith(SnoCacheLookup.hashActorSNOSummonedUnit);
								//LOS exceptions (gyser, heart of sin)
								hashActorSNOKitingIgnore_.UnionWith(SnoCacheLookup.hashActorSNOIgnoreLOSCheck);
						  }
						  return Bot.hashActorSNOKitingIgnore_; 
					 }
				}


				internal static void UpdateAvoidKiteRates(bool Reset=false)
				{
					 if (Reset)
					 {
						  Combat.iMillisecondsCancelledEmergencyMoveFor=0;
						  Combat.iMillisecondsCancelledKiteMoveFor=0;
						  return;
					 }

					 double extraWaitTime=Bot.SettingsFunky.AvoidanceRecheckMaximumRate*Character.dCurrentHealthPct;
					 if (extraWaitTime<Bot.SettingsFunky.AvoidanceRecheckMinimumRate) extraWaitTime=Bot.SettingsFunky.AvoidanceRecheckMinimumRate;
					 Combat.iMillisecondsCancelledEmergencyMoveFor=(int)extraWaitTime;

					 extraWaitTime=MathEx.Random(Bot.SettingsFunky.KitingRecheckMinimumRate, Bot.SettingsFunky.KitingRecheckMaximumRate)*Character.dCurrentHealthPct;
					 Combat.iMillisecondsCancelledKiteMoveFor=(int)extraWaitTime;
				}

				internal static void AttemptToUseHealthPotion()
				{
					 //Update and find best potion to use.
					 Character.BackPack.ReturnCurrentPotions();

					 Zeta.Internals.Actors.ACDItem thisBestPotion=Character.BackPack.BestPotionToUse;
					 if (thisBestPotion!=null)
					 {
						  Bot.Character.WaitWhileAnimating(4, true);
						  ZetaDia.Me.Inventory.UseItem((thisBestPotion.DynamicId));
					 }
					 dictAbilityLastUse[Zeta.Internals.Actors.SNOPower.DrinkHealthPotion]=DateTime.Now;
					 Bot.Character.WaitWhileAnimating(3, true);
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