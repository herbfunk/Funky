using System;
using System.Drawing.Imaging;
using Zeta;
using Zeta.CommonBot;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.CommonBot.Settings;
using Zeta.Internals;
using Zeta.Internals.Actors;

using FunkyTrinity.Cache;
using FunkyTrinity.Movement;
using System.Threading;
using FunkyTrinity.Targeting;
using FunkyTrinity.Settings;
using FunkyTrinity.Avoidances;

namespace FunkyTrinity
{

		  //This class is used to hold the data

		  public static partial class Bot
		  {
				public static Settings_Funky SettingsFunky=new Settings_Funky();
				public static Player Class { get; set; }
				private static CharacterCache character=new CharacterCache();
				public static CharacterCache Character
				{
					 get { return character; }
					 set { character=value; }
				}
				public static CombatCache Combat { get; set; }
				public static TargetHandler Target { get; set; }
				private static BotStatistics Stats_=new BotStatistics();
				public static BotStatistics Stats
				{
					 get { return Stats_; }
					 set { Stats_=value; }
				}
				private static ProfileCache profile=new ProfileCache();
				public static ProfileCache Profile
				{
					 get { return profile; }
					 set { profile=value; }
				}

				public static Navigation NavigationCache { get; set; }



				private static bool shuttingDownBot=false;
				internal static bool ShuttingDownBot
				{
					 get { return shuttingDownBot; }
					 set { shuttingDownBot=value; }
				}

				internal static void ShutDownBot()
				{
					 if (SettingsFunky.StopGameOnBotEnableScreenShot)
					 {
						  //Pause Game
						  UIElements.BackgroundScreenPCButtonMenu.Click();

						  //Copy orginal coords
						  ScreenCapture.RECT OrginRECT=new ScreenCapture.RECT();
						  ScreenCapture.GetWindowRect(Funky.D3Handle,ref OrginRECT);

						  //Resize and Move
						  ScreenCapture.MoveWindow(Funky.D3Handle, 0, 0, SettingsFunky.StopGameScreenShotWindowWidth, SettingsFunky.StopGameScreenShotWindowHeight, true);

						  //Click to refresh?
						  ScreenCapture.LeftClick(2, SettingsFunky.StopGameScreenShotWindowHeight/2);

						  //Bring window to foreground
						  ScreenCapture.SetForegroundWindow(Funky.D3Handle);

						  //Sleep...
						  Thread.Sleep(2500);

						  //UnPause Game
						  UIElements.BackgroundScreenPCButtonMenu.Click();

						  //Capture Screen
						  ScreenCapture SC=new ScreenCapture();
						  SC.CaptureWindowToFile(Funky.D3Handle, Funky.FolderPaths.sTrinityLogScreenShotPath+"LowHealthSS_"+CurrentAccountName+"_"+DateTime.Now.ToString("MM_dd--hh-mm-ss-tt")+".Jpeg", ImageFormat.Jpeg);

						  //Pause Game
						  UIElements.BackgroundScreenPCButtonMenu.Click();

						  //Return to orginal
						  ScreenCapture.MoveWindow(Funky.D3Handle, OrginRECT.left, OrginRECT.top, OrginRECT.Width(), OrginRECT.Height(), true);
						  
						  //Click to refresh?
						  ScreenCapture.LeftClick(OrginRECT.left+2, OrginRECT.Height()/2);
					 }
					 else
						  UIElements.BackgroundScreenPCButtonMenu.Click();
					
					 BotMain.Stop(true, "Low Health Setting Triggered!");
				}

				///<summary>
				///Usable Objects -- refresh inside Target.UpdateTarget
				///</summary>
				internal static List<CacheObject> ValidObjects { get; set; }

				internal static ActorClass ActorClass=ActorClass.Invalid;
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
						  ActorClass=ZetaDia.Service.CurrentHero.Class;
						  CurrentAccountName=ZetaDia.Service.CurrentHero.BattleTagName;
						  CurrentHeroName=ZetaDia.Service.CurrentHero.Name;
						  CurrentLevel=ZetaDia.Service.CurrentHero.Level;
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
							return (Bot.Profile.IsRunningOOCBehavior||Funky.shouldPreformOOCItemIDing||Funky.FunkyTPBehaviorFlag||Funky.TownRunManager.bWantToTownRun);
					 }
				}

				internal static float iCurrentMaxKillRadius=0f;
				internal static float iCurrentMaxLootRadius=0f;
				internal static void UpdateKillLootRadiusValues()
				{
					 iCurrentMaxKillRadius=CharacterSettings.Instance.KillRadius;
					 iCurrentMaxLootRadius=CharacterSettings.Instance.LootRadius;
					 // Not allowed to kill monsters due to profile/routine/combat targeting settings - just set the kill range to a third
					 if (!ProfileManager.CurrentProfile.KillMonsters) iCurrentMaxKillRadius/=3;
					 
					 // Always have a minimum kill radius, so we're never getting whacked without retaliating
					 if (iCurrentMaxKillRadius<10||SettingsFunky.Ranges.IgnoreCombatRange) iCurrentMaxKillRadius=10;

					 //Non-Combat Behavior we set minimum kill radius
					 if (IsInNonCombatBehavior) iCurrentMaxKillRadius=50;

					 // Not allowed to loots due to profile/routine/loot targeting settings - just set range to a quarter
					 if (!ProfileManager.CurrentProfile.PickupLoot) iCurrentMaxLootRadius/=4;
					 

					 //Ignore Loot Range Setting
					 if (SettingsFunky.Ranges.IgnoreLootRange) iCurrentMaxLootRadius=10;
				}

				#region SettingsRangeValues
				internal static int FleeDistance
				{
					 get
					 {
						  int value=SettingsFunky.Fleeing.FleeMaxMonsterDistance;

						  if (value==0&&Character.ShouldFlee)
								value=8;
						  

						  return value;
					 }
				}
				internal static int ContainerRange
				{
					 get
					 {
						  return SettingsFunky.Ranges.ContainerOpenRange;
					 }
				}
				internal static int NonEliteRange
				{
					 get
					 {
						  return SettingsFunky.Ranges.NonEliteCombatRange;
					 }
				}
				internal static int EliteRange
				{
					 get
					 {
						  return SettingsFunky.Ranges.EliteCombatRange;
					 }
				}
				internal static int GlobeRange
				{
					 get
					 {
						  return SettingsFunky.Ranges.GlobeRange;
					 }
				}
				internal static double ItemRange
				{
					 get
					 {
						  return iCurrentMaxLootRadius+SettingsFunky.Ranges.ItemRange;
					 }
				}
				internal static int GoldRange
				{
					 get
					 {
						  return SettingsFunky.Ranges.GoldRange;
					 }
				}
				internal static int DestructibleRange
				{
					 get
					 {
						  return SettingsFunky.Ranges.DestructibleRange;
					 }
				}
				internal static int TreasureGoblinRange
				{
					 get
					 {
						  return SettingsFunky.Ranges.TreasureGoblinRange;
					 }
				}
				internal static int ShrineRange
				{
					 get
					 {
						  return SettingsFunky.Ranges.ShrineRange;
					 }
				}
				internal static double EmergencyHealthPotionLimit
				{
					 get
					 {
						  return SettingsFunky.Combat.PotionHealthPercent;
					 }
				}
				internal static double EmergencyHealthGlobeLimit
				{
					 get
					 {
						  return SettingsFunky.Combat.GlobeHealthPercent;
					 }
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
						  return hashActorSNOKitingIgnore_; 
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

					 double extraWaitTime=SettingsFunky.AvoidanceRecheckMaximumRate*Character.dCurrentHealthPct;
					 if (extraWaitTime<SettingsFunky.AvoidanceRecheckMinimumRate) extraWaitTime=SettingsFunky.AvoidanceRecheckMinimumRate;
					 Combat.iMillisecondsCancelledEmergencyMoveFor=(int)extraWaitTime;

					 extraWaitTime=MathEx.Random(SettingsFunky.KitingRecheckMinimumRate, SettingsFunky.KitingRecheckMaximumRate)*Character.dCurrentHealthPct;
					 Combat.iMillisecondsCancelledFleeMoveFor=(int)extraWaitTime;
				}

				internal static void AttemptToUseHealthPotion()
				{
					 //Update and find best potion to use.
					 Character.BackPack.ReturnCurrentPotions();

					 ACDItem thisBestPotion=Character.BackPack.BestPotionToUse;
					 if (thisBestPotion!=null)
					 {
						  Character.WaitWhileAnimating(4, true);
						  ZetaDia.Me.Inventory.UseItem((thisBestPotion.DynamicId));
					 }
					 PowerCacheLookup.dictAbilityLastUse[SNOPower.DrinkHealthPotion]=DateTime.Now;
					 Character.WaitWhileAnimating(3, true);
				}

				internal static void Reset()
				{
					 Class=null;
					 character=new CharacterCache();
					 Combat=new CombatCache();
					 Target=new TargetHandler();
					 NavigationCache=new Navigation();
					 Stats_=new BotStatistics();
					 shuttingDownBot=false;
					 Funky.LeveledUpEventFired=false;
				}
		  }
	 
}