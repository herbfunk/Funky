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
		  internal static partial class Bot
		  {
				///<summary>
				///Cache of current combat variables
				///</summary>
				internal class CombatCache
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
						  powerPrime=new Ability(SNOPower.None, 0f, vNullLocation, -1, -1, 0, 0, false);
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

					 internal CacheObject LastCachedTarget { get; set; }
					 internal List<int> PrioritizedRAGUIDs=new List<int>();
					 internal List<CacheAvoidance> TriggeringAvoidances=new List<CacheAvoidance>();
					 internal List<int> UnitRAGUIDs=new List<int>();
					 internal List<int> ValidClusterUnits=new List<int>();
					 internal List<Cluster> CurrentTargetClusters=new List<Cluster>();
					 internal DateTime LastClusterTargetLogicRefresh=DateTime.Today;

					 internal List<Cluster> RunKMeans(int MinUnitCount=1, double Distance=5d)
					 {
						  List<CacheUnit> objects=new List<CacheUnit>();

						  //If ClusterTargetLogic is disabled.. we need to generate clusters!
						  if (!SettingsFunky.EnableClusteringTargetLogic&&DateTime.Now.Subtract(LastClusterTargetLogicRefresh).TotalMilliseconds>200)
						  {
								LastClusterTargetLogicRefresh=DateTime.Now;
								List<CacheObject> listObjectUnits=ObjectCache.Objects.Values.Where(u => UnitRAGUIDs.Contains(u.RAGUID)).ToList();
								
								if (listObjectUnits.Count>0)
									 CurrentTargetClusters=RunKmeans(listObjectUnits, Distance);
						  }

						  //populate with units
						  (from clusters in CurrentTargetClusters		 //Only Clusters with a valid targetable unit 
							where clusters.RAGUIDS.Count>=MinUnitCount&&(!clusters.ContainsNoTagetableUnits&&clusters.CurrentValidUnit!=null)
							select clusters.ListUnits)
								.ForEach(units => objects.AddRange(units));



						  return RunKmeans<CacheUnit>(objects, Distance).Where(clusters => clusters.RAGUIDS.Count>=MinUnitCount).ToList();
					 }

					 #region Movement
					 // Timestamp of when our position was last measured as changed
					 internal DateTime lastMovedDuringCombat { get; set; }
					 //Used to check movement during target
					 internal Vector3 lastPlayerPosDuringTargetMovement { get; set; }
					 internal Vector3 vLastMoveToTarget { get; set; }
					 internal float fLastDistanceFromTarget { get; set; }
					 internal Vector3 vCurrentDestination { get; set; }
					 internal bool UsedAutoMovementCommand { get; set; }
					 internal int totalNonMovementCount { get; set; }
					 internal bool bAlreadyMoving { get; set; }
					 internal DateTime lastMovementCommand { get; set; }
					 // How many times a movement fails because of being "blocked"
					 internal int iTimesBlockedMoving { get; set; }
					 #endregion


					 internal DateTime LastHealthChange { get; set; }
					 internal double LastHealthDropPct { get; set; }

					 #region Kite & Avoid

					 ///<summary>
					 ///Tracks if kiting was used last loop.
					 ///</summary>
					 internal bool KitedLastTarget { get; set; }
					 //Kiting
					 internal bool IsKiting { get; set; }
					 // Prevent spam-kiting too much - allow fighting between each kite movement
					 internal DateTime timeCancelledKiteMove=DateTime.Today;
					 internal int iMillisecondsCancelledKiteMoveFor=0;
					 internal DateTime LastKiteAction=DateTime.Today;
					 //Avoidance Related
					 internal bool RequiresAvoidance { get; set; }
					 internal bool TravellingAvoidance { get; set; }
					 internal bool DontMove { get; set; }
					 internal bool CriticalAvoidance { get; set; }
					 internal DateTime LastAvoidanceMovement { get; set; }
					 // When did we last send a move-power command?
					 internal DateTime lastSentMovePower=DateTime.Today;
					 // This force-prevents avoidance for XX loops incase we get stuck trying to avoid stuff
					 internal DateTime timeCancelledEmergencyMove=DateTime.Today;
					 internal int iMillisecondsCancelledEmergencyMoveFor=0;
					 // This lets us know if there is a target but it's in avoidance so we can just "stay put" until avoidance goes
					 internal bool bStayPutDuringAvoidance=false;

					 internal void UpdateAvoidKiteRates()
					 {
						  double extraWaitTime=SettingsFunky.AvoidanceRecheckMaximumRate*Bot.Character.dCurrentHealthPct;
						  if (extraWaitTime<SettingsFunky.AvoidanceRecheckMinimumRate) extraWaitTime=SettingsFunky.AvoidanceRecheckMinimumRate;
						  Bot.Combat.iMillisecondsCancelledEmergencyMoveFor=(int)extraWaitTime;

						  extraWaitTime=MathEx.Random(SettingsFunky.KitingRecheckMinimumRate, SettingsFunky.KitingRecheckMaximumRate)*Bot.Character.dCurrentHealthPct;
						  Bot.Combat.iMillisecondsCancelledKiteMoveFor=(int)extraWaitTime;
					 }
					 #endregion


					 // Variables used to actually hold powers the power-selector has picked to use, for buffing and main power use
					 internal Ability powerBuff { get; set; }
					 internal Ability powerPrime;
					 internal SNOPower powerLastSnoPowerUsed { get; set; }

					 //Loot Check
					 internal bool ShouldCheckItemLooted { get; set; }
					 internal int recheckCount { get; set; }
					 internal bool reCheckedFinished { get; set; }


					 #region HandlerFlags

					 // A flag to indicate whether we have a new target from the overlord (decorator) or not, in which case don't refresh targets again this first loop
					 internal bool bWholeNewTarget { get; set; }
					 // A flag to indicate if we should pick a new power/ability to use or not
					 internal bool bPickNewAbilities { get; set; }
					 // Flag used to indicate if we are simply waiting for a power to go off - so don't do any new target checking or anything
					 internal bool bWaitingForPower { get; set; }
					 // And a special post-use pause
					 internal bool bWaitingAfterPower { get; set; }
					 // If we are waiting before popping a potion
					 internal bool bWaitingForPotion { get; set; }
					 // Force a target update after certain interactions
					 internal bool bForceTargetUpdate { get; set; }
					 // Variable to let us force new target creations immediately after a root
					 internal bool bWasRootedLastTick { get; set; }
					 // This holds whether or not we want to prioritize a close-target, used when we might be body-blocked by monsters
					 internal bool bForceCloseRangeTarget { get; set; }
					 #endregion

					 // how long to force close-range targets for
					 internal int iMillisecondsForceCloseRange=0;
					 // Date time we were last told to stick to close range targets
					 internal DateTime lastForcedKeepCloseRange=DateTime.Today;


					 // Variables relating to quick-reference of monsters within sepcific ranges (if anyone has suggestion for similar functionality with reduced CPU use, lemme know, but this is fast atm!)
					 internal int[] iElitesWithinRange;

					 internal int[] iAnythingWithinRange { get; set; }
					 internal int iNonRendedTargets_6 { get; set; }
					 internal bool UsesDOTDPSAbility { get; set; }
					 internal int SurroundingUnits { get; set; }


					 #region Last seen objects
					 // Last had any mob in range, for loot-waiting
					 internal DateTime lastHadUnitInSights { get; set; }
					 // When we last saw a boss/elite etc.
					 internal DateTime lastHadEliteUnitInSights { get; set; }
					 //Last time we had a container, for loot-waiting
					 internal DateTime lastHadContainerAsTarget { get; set; }
					 //When we last saw a "rare" chest
					 internal DateTime lastHadRareChestAsTarget { get; set; }
					 // Store the date-time when we *FIRST* picked this target, so we can blacklist after X period of time targeting
					 internal DateTime dateSincePickedTarget { get; set; }
					 internal int iTotalNumberGoblins=0;
					 internal DateTime lastGoblinTime=DateTime.Today;
					 #endregion


					 internal int iACDGUIDLastWhirlwind=0;
					 internal Vector3 vSideToSideTarget=vNullLocation;
					 internal DateTime lastChangedZigZag=DateTime.Today;
					 internal Vector3 vPositionLastZigZagCheck=Vector3.Zero;

					 internal bool bAnyChampionsPresent { get; set; }
					 internal bool bAnyTreasureGoblinsPresent { get; set; }
					 internal bool bAnyMobsInCloseRange { get; set; }
					 internal bool bAnyBossesInRange { get; set; }
					 // A flag to say whether any NONE-hashActorSNOWhirlwindIgnore things are around
					 internal bool bAnyNonWWIgnoreMobsInRange { get; set; }
					 /// <summary>
					 /// Check LoS if waller avoidance detected
					 /// </summary>
					 internal bool bCheckGround=false;


					 internal float iCurrentMaxKillRadius=0f;
					 internal float iCurrentMaxLootRadius=0f;
					 internal void UpdateKillLootRadiusValues()
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
					 internal bool IsInNonCombatBehavior
					 {
						  get
						  {
								//OOC IDing, Town Portal Casting, Town Run
								return (Bot.Character.IsRunningTownPortalBehavior||shouldPreformOOCItemIDing||FunkyTPBehaviorFlag||TownRunManager.bWantToTownRun);
						  }
					 }


					 internal void Reset()
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
					 internal void ResetTargetHandling()
					 {
						  Bot.Target.CurrentTarget=null;
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
						  fLastDistanceFromTarget=-1f;
					 }
				}

		  }
	 }
}