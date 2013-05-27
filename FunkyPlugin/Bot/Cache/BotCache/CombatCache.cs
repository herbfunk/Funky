using System;
using System.Linq;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Internals;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.SNO;
using Zeta.CommonBot;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  public partial class Bot
		  {
				///<summary>
				///Cache of current combat variables
				///</summary>
				public class CombatCache
				{
					 public CombatCache()
					 {
						  bWholeNewTarget=false;
						  bPickNewAbilities=false;
						  bWaitingAfterPower=false;
						  bWaitingForPower=false;
						  bWaitingForPotion=false;
						  bForceTargetUpdate=false;
						  bWasRootedLastTick=false;
						  lastMovementCommand=DateTime.Today;
						  iTimesBlockedMoving=0;
						  bAlreadyMoving=false;
						  ShouldCheckItemLooted=false;
						  reCheckedFinished=false;
						  recheckCount=0;
						  totalNonMovementCount=0;
						  UsedAutoMovementCommand=false;
						  lastMovedDuringCombat=DateTime.Today;
						  lastPlayerPosDuringTargetMovement=Vector3.Zero;
						  LastHealthDropPct=0d;
						  LastHealthChange=DateTime.Today;
						  lastSentMovePower=DateTime.Today;
						  powerPrime=new cacheSNOPower(SNOPower.None, 0f, vNullLocation, -1, -1, 0, 0, false);
						  iElitesWithinRange=new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
						  iAnythingWithinRange=new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
						  iNonRendedTargets_6=0;
						  bAnyBossesInRange=false;
						  bAnyChampionsPresent=false;
						  bAnyTreasureGoblinsPresent=false;
						  bAnyMobsInCloseRange=false;
						  bAnyNonWWIgnoreMobsInRange=false;
						  RequiresAvoidance=false;
						  TravellingAvoidance=false;
						  bForceCloseRangeTarget=false;
						  lastHadUnitInSights=DateTime.Today;
						  lastHadEliteUnitInSights=DateTime.Today;
						  lastHadContainerAsTarget=DateTime.Today;
						  lastHadRareChestAsTarget=DateTime.Today;
						  dateSincePickedTarget=DateTime.Today;
						  LastAvoidanceMovement=DateTime.Today;
						  SurroundingUnits=0;
						  DontMove=false;
						  CriticalAvoidance=false;
						  KitedLastTarget=false;
						  UsesDOTDPSAbility=false;
					 }

					 public CacheObject LastCachedTarget { get; set; }
					 public List<int> PrioritizedRAGUIDs=new List<int>();
					 public List<CacheAvoidance> TriggeringAvoidances=new List<CacheAvoidance>();
					 public List<int> UnitRAGUIDs=new List<int>();
					 public List<int> ValidClusterUnits=new List<int>();
					 public DateTime LastClusterTargetLogicRefresh=DateTime.Today;

					 #region Movement
					 // Timestamp of when our position was last measured as changed
					 public DateTime lastMovedDuringCombat { get; set; }
					 //Used to check movement during target
					 public Vector3 lastPlayerPosDuringTargetMovement { get; set; }
					 public Vector3 vLastMoveToTarget { get; set; }
					 public float fLastDistanceFromTarget { get; set; }
					 public Vector3 vCurrentDestination { get; set; }
					 public bool UsedAutoMovementCommand { get; set; }
					 public int totalNonMovementCount { get; set; }
					 public bool bAlreadyMoving { get; set; }
					 public DateTime lastMovementCommand { get; set; }
					 // How many times a movement fails because of being "blocked"
					 public int iTimesBlockedMoving { get; set; }
					 #endregion


					 public DateTime LastHealthChange { get; set; }
					 public double LastHealthDropPct { get; set; }

					 #region Kite & Avoid

					 ///<summary>
					 ///Tracks if kiting was used last loop.
					 ///</summary>
					 public bool KitedLastTarget { get; set; }
					 //Kiting
					 public bool IsKiting { get; set; }
					 // Prevent spam-kiting too much - allow fighting between each kite movement
					 public DateTime timeCancelledKiteMove=DateTime.Today;
					 public int iMillisecondsCancelledKiteMoveFor=0;
					 public DateTime LastKiteAction=DateTime.Today;
					 //Avoidance Related
					 public bool RequiresAvoidance { get; set; }
					 public bool TravellingAvoidance { get; set; }
					 public bool DontMove { get; set; }
					 public bool CriticalAvoidance { get; set; }
					 public DateTime LastAvoidanceMovement { get; set; }
					 // When did we last send a move-power command?
					 public DateTime lastSentMovePower=DateTime.Today;
					 // This force-prevents avoidance for XX loops incase we get stuck trying to avoid stuff
					 public DateTime timeCancelledEmergencyMove=DateTime.Today;
					 public int iMillisecondsCancelledEmergencyMoveFor=0;
					 // This lets us know if there is a target but it's in avoidance so we can just "stay put" until avoidance goes
					 public bool bStayPutDuringAvoidance=false;

					 public void UpdateAvoidKiteRates()
					 {
						  double extraWaitTime=SettingsFunky.AvoidanceRecheckMaximumRate*Bot.Character.dCurrentHealthPct;
						  if (extraWaitTime<SettingsFunky.AvoidanceRecheckMinimumRate) extraWaitTime=SettingsFunky.AvoidanceRecheckMinimumRate;
						  Bot.Combat.iMillisecondsCancelledEmergencyMoveFor=(int)extraWaitTime;

						  extraWaitTime=SettingsFunky.KitingRecheckMaximumRate*Bot.Character.dCurrentHealthPct;
						  if (extraWaitTime<SettingsFunky.KitingRecheckMinimumRate) extraWaitTime=SettingsFunky.KitingRecheckMinimumRate;
						  Bot.Combat.iMillisecondsCancelledKiteMoveFor=(int)extraWaitTime;
					 }
					 #endregion


					 // Variables used to actually hold powers the power-selector has picked to use, for buffing and main power use
					 public cacheSNOPower powerBuff { get; set; }
					 public cacheSNOPower powerPrime;
					 public SNOPower powerLastSnoPowerUsed { get; set; }

					 //Loot Check
					 public bool ShouldCheckItemLooted { get; set; }
					 public int recheckCount { get; set; }
					 public bool reCheckedFinished { get; set; }


					 #region HandlerFlags

					 // A flag to indicate whether we have a new target from the overlord (decorator) or not, in which case don't refresh targets again this first loop
					 public bool bWholeNewTarget { get; set; }
					 // A flag to indicate if we should pick a new power/ability to use or not
					 public bool bPickNewAbilities { get; set; }
					 // Flag used to indicate if we are simply waiting for a power to go off - so don't do any new target checking or anything
					 public bool bWaitingForPower { get; set; }
					 // And a special post-use pause
					 public bool bWaitingAfterPower { get; set; }
					 // If we are waiting before popping a potion
					 public bool bWaitingForPotion { get; set; }
					 // Force a target update after certain interactions
					 public bool bForceTargetUpdate { get; set; }
					 // Variable to let us force new target creations immediately after a root
					 public bool bWasRootedLastTick { get; set; }
					 // This holds whether or not we want to prioritize a close-target, used when we might be body-blocked by monsters
					 public bool bForceCloseRangeTarget { get; set; }
					 #endregion

					 // how long to force close-range targets for
					 public int iMillisecondsForceCloseRange=0;
					 // Date time we were last told to stick to close range targets
					 public DateTime lastForcedKeepCloseRange=DateTime.Today;


					 // Variables relating to quick-reference of monsters within sepcific ranges (if anyone has suggestion for similar functionality with reduced CPU use, lemme know, but this is fast atm!)
					 public int[] iElitesWithinRange;

					 public int[] iAnythingWithinRange { get; set; }
					 public int iNonRendedTargets_6 { get; set; }
					 public bool UsesDOTDPSAbility { get; set; }
					 public int SurroundingUnits { get; set; }
					 

					 #region Last seen objects
					 // Last had any mob in range, for loot-waiting
					 public DateTime lastHadUnitInSights { get; set; }
					 // When we last saw a boss/elite etc.
					 public DateTime lastHadEliteUnitInSights { get; set; }
					 //Last time we had a container, for loot-waiting
					 public DateTime lastHadContainerAsTarget { get; set; }
					 //When we last saw a "rare" chest
					 public DateTime lastHadRareChestAsTarget { get; set; }
					 // Store the date-time when we *FIRST* picked this target, so we can blacklist after X period of time targeting
					 public DateTime dateSincePickedTarget { get; set; }
					 public int iTotalNumberGoblins=0;
					 public DateTime lastGoblinTime=DateTime.Today;
					 #endregion


					 public int iACDGUIDLastWhirlwind=0;
					 public Vector3 vSideToSideTarget=vNullLocation;
					 public DateTime lastChangedZigZag=DateTime.Today;
					 public Vector3 vPositionLastZigZagCheck=Vector3.Zero;

					 public bool bAnyChampionsPresent { get; set; }
					 public bool bAnyTreasureGoblinsPresent { get; set; }
					 public bool bAnyMobsInCloseRange { get; set; }
					 public bool bAnyBossesInRange { get; set; }
					 // A flag to say whether any NONE-hashActorSNOWhirlwindIgnore things are around
					 public bool bAnyNonWWIgnoreMobsInRange { get; set; }
					 /// <summary>
					 /// Check LoS if waller avoidance detected
					 /// </summary>
					 public bool bCheckGround=false;


					 public float iCurrentMaxKillRadius=0f;
					 public float iCurrentMaxLootRadius=0f;
					 public void UpdateKillLootRadiusValues()
					 {
						  iCurrentMaxKillRadius=Zeta.CommonBot.Settings.CharacterSettings.Instance.KillRadius;
						  iCurrentMaxLootRadius=Zeta.CommonBot.Settings.CharacterSettings.Instance.LootRadius;
						  // Not allowed to kill monsters due to profile/routine/combat targeting settings - just set the kill range to a third
						  if (!ProfileManager.CurrentProfile.KillMonsters)
						  {
								iCurrentMaxKillRadius/=3;
						  }
						  // Always have a minimum kill radius, so we're never getting whacked without retaliating
						  if (iCurrentMaxKillRadius<10||SettingsFunky.IgnoreCombatRange)
								iCurrentMaxKillRadius=10;

						  if (shouldPreformOOCItemIDing||FunkyTPBehaviorFlag||TownRunManager.bWantToTownRun)
								iCurrentMaxKillRadius=50;

						  // Not allowed to loots due to profile/routine/loot targeting settings - just set range to a quarter
						  if (!ProfileManager.CurrentProfile.PickupLoot)
						  {
								iCurrentMaxLootRadius/=4;
						  }

						  //Ignore Loot Range Setting
						  if (SettingsFunky.IgnoreLootRange)
								iCurrentMaxLootRadius=10;


						  // Counter for how many cycles we extend or reduce our attack/kill radius, and our loot radius, after a last kill
						  if (iKeepKillRadiusExtendedFor>0)
								iKeepKillRadiusExtendedFor--;
						  if (iKeepLootRadiusExtendedFor>0)
								iKeepLootRadiusExtendedFor--;
					 }


					 ///<summary>
					 ///Checks behavioral flags that are considered OOC/Non-Combat
					 ///</summary>
					 public bool IsInNonCombatBehavior
					 {
						  get
						  {
								//OOC IDing, Town Portal Casting, Town Run
								return (Bot.Character.IsRunningTownPortalBehavior||shouldPreformOOCItemIDing||FunkyTPBehaviorFlag||TownRunManager.bWantToTownRun);
						  }
					 }


					 public void Reset()
					 {
						  iElitesWithinRange=new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
						  iAnythingWithinRange=new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
						  iNonRendedTargets_6=0;
						  bAnyBossesInRange=false;
						  bAnyChampionsPresent=false;
						  bAnyTreasureGoblinsPresent=false;
						  bAnyMobsInCloseRange=false;
						  bAnyNonWWIgnoreMobsInRange=false;
						  TravellingAvoidance=false;
						  UnitRAGUIDs=new List<int>();
						  SurroundingUnits=0;
						  TriggeringAvoidances.Clear();
						  IsKiting=false;
						  UsesDOTDPSAbility=false;
						  bCheckGround=false;
					 }
					 public void ResetTargetHandling()
					 {
						  Bot.Target.ObjectData=null;
						  iTimesBlockedMoving=0;
						  totalNonMovementCount=0;
						  bAlreadyMoving=false;
						  lastMovementCommand=DateTime.Today;
						  bWaitingForPower=false;
						  bWaitingAfterPower=false;
						  bWaitingForPotion=false;
						  bWasRootedLastTick=false;
						  recheckCount=0;
						  reCheckedFinished=false;
						  UsedAutoMovementCommand=false;
						  LastHealthChange=DateTime.MinValue;
						  LastHealthDropPct=0d;
					 }
				}

		  }
	 }
}