using System;
using Zeta;
using Zeta.CommonBot;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.Actors;
using FunkyTrinity.Enums;
using FunkyTrinity.Cache;
using FunkyTrinity.Movement;
using System.Threading;
using FunkyTrinity.Targeting;
using FunkyTrinity.Settings;

namespace FunkyTrinity
{

		  //This class is used to hold the data

		  public static partial class Bot
		  {
				public static Settings_Funky SettingsFunky=new Settings_Funky();
				public static Player Class { get; set; }
				public static CharacterCache Character { get; set; }
				public static CombatCache Combat { get; set; }
				public static TargetHandler Target { get; set; }
				private static BotStatistics Stats_=new BotStatistics();
				public static BotStatistics Stats
				{
					 get { return Bot.Stats_; }
					 set { Bot.Stats_=value; }
				}
				public static Navigation NavigationCache { get; set; }



				private static bool shuttingDownBot=false;
				internal static bool ShuttingDownBot
				{
					 get { return Bot.shuttingDownBot; }
					 set { Bot.shuttingDownBot=value; }
				}

				internal static void ShutDownBot()
				{
					 if (Bot.SettingsFunky.StopGameOnBotEnableScreenShot)
					 {
						  //Pause Game
						  Zeta.Internals.UIElements.BackgroundScreenPCButtonMenu.Click();

						  //Copy orginal coords
						  ScreenCapture.RECT OrginRECT=new ScreenCapture.RECT();
						  ScreenCapture.GetWindowRect(Funky.D3Handle,ref OrginRECT);

						  //Resize and Move
						  ScreenCapture.MoveWindow(Funky.D3Handle, 0, 0, Bot.SettingsFunky.StopGameScreenShotWindowWidth, Bot.SettingsFunky.StopGameScreenShotWindowHeight, true);

						  //Click to refresh?
						  ScreenCapture.LeftClick(2, Bot.SettingsFunky.StopGameScreenShotWindowHeight/2);

						  //Bring window to foreground
						  ScreenCapture.SetForegroundWindow(Funky.D3Handle);

						  //Sleep...
						  Thread.Sleep(2500);

						  //UnPause Game
						  Zeta.Internals.UIElements.BackgroundScreenPCButtonMenu.Click();

						  //Capture Screen
						  ScreenCapture SC=new ScreenCapture();
						  SC.CaptureWindowToFile(Funky.D3Handle, Funky.FolderPaths.sTrinityLogScreenShotPath+"LowHealthSS_"+Bot.CurrentAccountName+"_"+DateTime.Now.ToString("MM_dd--hh-mm-ss-tt")+".Jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);

						  //Pause Game
						  Zeta.Internals.UIElements.BackgroundScreenPCButtonMenu.Click();

						  //Return to orginal
						  ScreenCapture.MoveWindow(Funky.D3Handle, OrginRECT.left, OrginRECT.top, OrginRECT.Width(), OrginRECT.Height(), true);
						  
						  //Click to refresh?
						  ScreenCapture.LeftClick(OrginRECT.left+2, OrginRECT.Height()/2);
					 }
					 else
						  Zeta.Internals.UIElements.BackgroundScreenPCButtonMenu.Click();
					
					 BotMain.Stop(true, "Low Health Setting Triggered!");
				}

				internal static Zeta.CommonBot.Profile.ProfileBehavior CurrentProfileBehavior { get; set; }
				private static DateTime LastProfileBehaviorCheck=DateTime.Today;
				///<summary>
				///Tracks Current Profile Behavior and sets IsRunningOOCBehavior depending on the current Type of behavior.
				///</summary>
				internal static void CheckCurrentProfileBehavior()
				{
					 if (DateTime.Now.Subtract(LastProfileBehaviorCheck).TotalMilliseconds>1000)
					 {
						  LastProfileBehaviorCheck=DateTime.Now;

						  if (Bot.CurrentProfileBehavior==null
								||Zeta.CommonBot.ProfileManager.CurrentProfileBehavior!=null
								&&Zeta.CommonBot.ProfileManager.CurrentProfileBehavior.Behavior!=null
								&&Bot.CurrentProfileBehavior.Behavior.Guid!=Zeta.CommonBot.ProfileManager.CurrentProfileBehavior.Behavior.Guid)
						  {
								Bot.CurrentProfileBehavior=Zeta.CommonBot.ProfileManager.CurrentProfileBehavior;

								if (ObjectCache.oocDBTags.Contains(Bot.CurrentProfileBehavior.GetType()))
								{
									 Logging.WriteDiagnostic("Current Profile Behavior has enabled OOC Behavior.");
									 Bot.Character.IsRunningOOCBehavior=true;
								}
								else
									 Bot.Character.IsRunningOOCBehavior=false;


								//Bot.Character.IsRunningInteractiveBehavior=ObjectCache.InteractiveTags.Contains(Bot.CurrentProfileBehavior.GetType());

								//if (Bot.Character.ShouldBackTrack&&!Bot.Character.IsRunningInteractiveBehavior)
								//{
								//    Bot.Character.ShouldBackTrack=false;
								//}
						  }
					 }
				}


				///<summary>
				///Usable Objects -- refresh inside Target.UpdateTarget
				///</summary>
				internal static System.Collections.Generic.List<CacheObject> ValidObjects { get; set; }

				internal static Zeta.Internals.Actors.ActorClass ActorClass=Zeta.Internals.Actors.ActorClass.Invalid;
				internal static string CurrentAccountName;
				internal static string CurrentHeroName;
				internal static int CurrentLevel;

				///<summary>
				///Updates Account Name, Current Hero Name and Class Variables
				///</summary>
				internal static void UpdateCurrentAccountDetails()
				{

					 try
					 {
						  ActorClass=Zeta.ZetaDia.Service.CurrentHero.Class;
						  CurrentAccountName=Zeta.ZetaDia.Service.CurrentHero.BattleTagName;
						  CurrentHeroName=Zeta.ZetaDia.Service.CurrentHero.Name;
						  CurrentLevel=Zeta.ZetaDia.Service.CurrentHero.Level;
					 } catch (Exception)
					 {
						  Logging.WriteDiagnostic("[Funky] Exception Attempting to Update Current Account Details.");
					 }
				}

				///<summary>
				///Checks behavioral flags that are considered OOC/Non-Combat
				///</summary>
				internal static bool IsInNonCombatBehavior
				{
					 get
					 {
						  //OOC IDing, Town Portal Casting, Town Run
							return (Character.IsRunningOOCBehavior||Funky.shouldPreformOOCItemIDing||Funky.FunkyTPBehaviorFlag||Funky.TownRunManager.bWantToTownRun);
					 }
				}

				internal static float iCurrentMaxKillRadius=0f;
				internal static float iCurrentMaxLootRadius=0f;
				internal static void UpdateKillLootRadiusValues()
				{
					 iCurrentMaxKillRadius=Zeta.CommonBot.Settings.CharacterSettings.Instance.KillRadius;
					 iCurrentMaxLootRadius=Zeta.CommonBot.Settings.CharacterSettings.Instance.LootRadius;
					 // Not allowed to kill monsters due to profile/routine/combat targeting settings - just set the kill range to a third
					 if (!ProfileManager.CurrentProfile.KillMonsters) iCurrentMaxKillRadius/=3;
					 
					 // Always have a minimum kill radius, so we're never getting whacked without retaliating
					 if (iCurrentMaxKillRadius<10||Bot.SettingsFunky.Ranges.IgnoreCombatRange) iCurrentMaxKillRadius=10;

					 //Non-Combat Behavior we set minimum kill radius
					 if (IsInNonCombatBehavior) iCurrentMaxKillRadius=50;

					 // Not allowed to loots due to profile/routine/loot targeting settings - just set range to a quarter
					 if (!ProfileManager.CurrentProfile.PickupLoot) iCurrentMaxLootRadius/=4;
					 

					 //Ignore Loot Range Setting
					 if (Bot.SettingsFunky.Ranges.IgnoreLootRange) iCurrentMaxLootRadius=10;
				}

				#region SettingsRangeValues
				internal static int FleeDistance
				{
					 get
					 {
						  int value=Bot.SettingsFunky.Fleeing.FleeMaxMonsterDistance;

						  if (value==0&&Bot.Character.ShouldFlee)
								value=8;
						  

						  return value;
					 }
				}
				internal static int ContainerRange
				{
					 get
					 {
						  return Bot.SettingsFunky.Ranges.ContainerOpenRange;
					 }
				}
				internal static int NonEliteRange
				{
					 get
					 {
						  return Bot.SettingsFunky.Ranges.NonEliteCombatRange;
					 }
				}
				internal static int EliteRange
				{
					 get
					 {
						  return Bot.SettingsFunky.Ranges.EliteCombatRange;
					 }
				}
				internal static int GlobeRange
				{
					 get
					 {
						  return Bot.SettingsFunky.Ranges.GlobeRange;
					 }
				}
				internal static double ItemRange
				{
					 get
					 {
						  return Bot.iCurrentMaxLootRadius+SettingsFunky.Ranges.ItemRange;
					 }
				}
				internal static int GoldRange
				{
					 get
					 {
						  return Bot.SettingsFunky.Ranges.GoldRange;
					 }
				}
				internal static int DestructibleRange
				{
					 get
					 {
						  return Bot.SettingsFunky.Ranges.DestructibleRange;
					 }
				}
				internal static int TreasureGoblinRange
				{
					 get
					 {
						  return Bot.SettingsFunky.Ranges.TreasureGoblinRange;
					 }
				}
				internal static int ShrineRange
				{
					 get
					 {
						  return Bot.SettingsFunky.Ranges.ShrineRange;
					 }
				}
				internal static double EmergencyHealthPotionLimit
				{
					 get
					 {
						  return Bot.SettingsFunky.Combat.PotionHealthPercent;
					 }
				}
				internal static double EmergencyHealthGlobeLimit
				{
					 get
					 {
						  return Bot.SettingsFunky.Combat.GlobeHealthPercent;
					 }
				}
				#endregion

				#region Avoidances

				internal static bool IgnoringAvoidanceType(AvoidanceType thisAvoidance)
				{
					 if (!Bot.SettingsFunky.Avoidance.AttemptAvoidanceMovements)
						  return true;

					 double dThisHealthAvoid=Bot.SettingsFunky.Avoidance.Avoidances[(int)thisAvoidance].Health;
					 if (dThisHealthAvoid==0d)
						  return true;

					 return false;
				}

				///<summary>
				///Tests the given avoidance type to see if it should be ignored either due to a buff or if health is greater than the avoidance HP.
				///</summary>
				internal static bool IgnoreAvoidance(AvoidanceType thisAvoidance)
				{
					 double dThisHealthAvoid=Bot.SettingsFunky.Avoidance.Avoidances[(int)thisAvoidance].Health;

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
								hashActorSNOKitingIgnore_.UnionWith(CacheIDLookup.hashActorSNOBurrowableUnits);
								//grunts
								hashActorSNOKitingIgnore_.UnionWith(CacheIDLookup.hashActorSNOSummonedUnit);
								//LOS exceptions (gyser, heart of sin)
								hashActorSNOKitingIgnore_.UnionWith(CacheIDLookup.hashActorSNOIgnoreLOSCheck);
						  }
						  return Bot.hashActorSNOKitingIgnore_; 
					 }
				}


				internal static void UpdateAvoidKiteRates(bool Reset=false)
				{
					 if (Reset)
					 {
						  Combat.iMillisecondsCancelledEmergencyMoveFor=0;
						  Combat.iMillisecondsCancelledFleeMoveFor=0;
						  return;
					 }

					 double extraWaitTime=Bot.SettingsFunky.AvoidanceRecheckMaximumRate*Character.dCurrentHealthPct;
					 if (extraWaitTime<Bot.SettingsFunky.AvoidanceRecheckMinimumRate) extraWaitTime=Bot.SettingsFunky.AvoidanceRecheckMinimumRate;
					 Combat.iMillisecondsCancelledEmergencyMoveFor=(int)extraWaitTime;

					 extraWaitTime=MathEx.Random(Bot.SettingsFunky.KitingRecheckMinimumRate, Bot.SettingsFunky.KitingRecheckMaximumRate)*Character.dCurrentHealthPct;
					 Combat.iMillisecondsCancelledFleeMoveFor=(int)extraWaitTime;
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
					 PowerCacheLookup.dictAbilityLastUse[Zeta.Internals.Actors.SNOPower.DrinkHealthPotion]=DateTime.Now;
					 Bot.Character.WaitWhileAnimating(3, true);
				}

				internal static void Reset()
				{
					 Class=null;
					 Character=new CharacterCache();
					 Combat=new CombatCache();
					 Target=new TargetHandler();
					 NavigationCache=new Navigation();
					 Stats=new BotStatistics();
					 shuttingDownBot=false;
					 Funky.LeveledUpEventFired=false;
				}

		  }
	 
}