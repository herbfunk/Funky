using System;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Navigation;
using fBaseXtensions.Settings;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;

namespace fBaseXtensions.Targeting.Behaviors
{
	public class TBEnd : TargetBehavior
	{
		public TBEnd() : base() { }
		public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Target; } }


		public override void Initialize()
		{
			base.Test = (ref CacheObject obj) =>
			{
				if (obj == null)
				{

					// See if we should wait for milliseconds for possible loot drops before continuing run
					if (DateTime.Now.Subtract(FunkyGame.Targeting.Cache.lastHadUnitInSights).TotalMilliseconds <= FunkyBaseExtension.Settings.General.AfterCombatDelay && DateTime.Now.Subtract(FunkyGame.Targeting.Cache.lastHadEliteUnitInSights).TotalMilliseconds <= 10000 ||
						//Cut the delay time in half for non-elite monsters!
						DateTime.Now.Subtract(FunkyGame.Targeting.Cache.lastHadUnitInSights).TotalMilliseconds <= FunkyBaseExtension.Settings.General.AfterCombatDelay)
					{
						obj = new CacheObject(FunkyGame.Hero.Position, TargetType.NoMovement, 20000, "WaitForLootDrops", 2f, -1);
						return true;

					}
					//Herbfunks wait after loot containers are opened. 3s for rare chests, half the settings delay for everything else.
					if ((DateTime.Now.Subtract(FunkyGame.Targeting.Cache.lastHadRareChestAsTarget).TotalMilliseconds <= 3750) ||
						(DateTime.Now.Subtract(FunkyGame.Targeting.Cache.lastHadContainerAsTarget).TotalMilliseconds <= (FunkyBaseExtension.Settings.General.AfterCombatDelay * 1.25)))
					{
						obj = new CacheObject(FunkyGame.Hero.Position, TargetType.NoMovement, 20000, "ContainerLootDropsWait", 2f, -1);
						return true;
					}

					if (DateTime.Now.Subtract(FunkyGame.Targeting.Cache.lastSeenCursedShrine).TotalMilliseconds <= (1000))
					{
						if (FunkyGame.AdventureMode && SettingAdventureMode.AdventureModeSettingsTag.AllowCombatModifications && FunkyGame.Bounty.CurrentBountyCacheEntry != null && FunkyGame.Bounty.CurrentBountyCacheEntry.Type == BountyTypes.CursedEvent)
						{
							Logger.DBLog.Info("[Funky] Cursed Object Found During Cursed Bounty -- Enabling LOS movement for all Units!");
							SettingLOSMovement.LOSSettingsTag.MiniumRangeObjects = 10;
							SettingLOSMovement.LOSSettingsTag.MaximumRange = 125;
							FunkyGame.Game.AllowAnyUnitForLOSMovement = true;
							SettingCluster.ClusterSettingsTag = SettingCluster.DisabledClustering;
						}

						obj = new CacheObject(FunkyGame.Hero.Position, TargetType.NoMovement, 20000, "CursedShrineWait", 2f, -1);
						return true;
					}

				    if (DateTime.Now.Subtract(FunkyGame.Targeting.Cache.lastHadSwitchAsTarget).TotalMilliseconds <= (4000))
				    {
                        obj = new CacheObject(FunkyGame.Hero.Position, TargetType.NoMovement, 20000, "SwitchWait", 2f, -1);
                        return true;
				    }

					// Finally, a special check for waiting for wrath of the berserker cooldown before engaging Azmodan
					if (Hotbar.HasPower(SNOPower.Barbarian_WrathOfTheBerserker) && FunkyBaseExtension.Settings.Barbarian.bWaitForWrath && !FunkyGame.Hero.Class.Abilities[SNOPower.Barbarian_WrathOfTheBerserker].AbilityUseTimer() &&
						FunkyGame.Hero.CurrentWorldDynamicID == 121214 &&
						(Vector3.Distance(FunkyGame.Hero.Position, new Vector3(711.25f, 716.25f, 80.13903f)) <= 40f || Vector3.Distance(FunkyGame.Hero.Position, new Vector3(546.8467f, 551.7733f, 1.576313f)) <= 40f))
					{
						Logger.DBLog.InfoFormat("[Funky] Waiting for Wrath Of The Berserker cooldown before continuing to Azmodan.");
						obj = new CacheObject(FunkyGame.Hero.Position, TargetType.NoMovement, 20000, "GilesWaitForWrath", 0f, -1);
						InactivityDetector.Reset();
						return true;
					}
					// And a special check for wizard archon
					if (Hotbar.HasPower(SNOPower.Wizard_Archon) && !FunkyGame.Hero.Class.Abilities[SNOPower.Wizard_Archon].AbilityUseTimer() && FunkyBaseExtension.Settings.Wizard.bWaitForArchon && ZetaDia.CurrentWorldId == 121214 &&
						(Vector3.Distance(FunkyGame.Hero.Position, new Vector3(711.25f, 716.25f, 80.13903f)) <= 40f || Vector3.Distance(FunkyGame.Hero.Position, new Vector3(546.8467f, 551.7733f, 1.576313f)) <= 40f))
					{
						Logger.DBLog.InfoFormat("[Funky] Waiting for Wizard Archon cooldown before continuing to Azmodan.");
						obj = new CacheObject(FunkyGame.Hero.Position, TargetType.NoMovement, 20000, "GilesWaitForArchon", 0f, -1);
						InactivityDetector.Reset();
						return true;
					}
					// And a very sexy special check for WD BigBadVoodoo
					if (Hotbar.HasPower(SNOPower.Witchdoctor_BigBadVoodoo) && !PowerManager.CanCast(SNOPower.Witchdoctor_BigBadVoodoo) && ZetaDia.CurrentWorldId == 121214 &&
						(Vector3.Distance(FunkyGame.Hero.Position, new Vector3(711.25f, 716.25f, 80.13903f)) <= 40f || Vector3.Distance(FunkyGame.Hero.Position, new Vector3(546.8467f, 551.7733f, 1.576313f)) <= 40f))
					{
						Logger.DBLog.InfoFormat("[Funky] Waiting for WD BigBadVoodoo cooldown before continuing to Azmodan.");
						obj = new CacheObject(FunkyGame.Hero.Position, TargetType.NoMovement, 20000, "GilesWaitForVoodooo", 0f, -1);
						InactivityDetector.Reset();
						return true;
					}

					//Currently preforming an interactive profile behavior (check if in town and not vendoring)
					if (FunkyGame.Game.InteractableCachedObject != null && (!FunkyGame.Hero.bIsInTown || !BrainBehavior.IsVendoring))
					{
						if (FunkyGame.Game.InteractableCachedObject.Position.Distance(FunkyGame.Hero.Position) > 50f)
						{
							//if (FunkyGame.Targeting.Cache.LastCachedTarget.Position != Bot.Game.Profile.InteractableCachedObject.Position)
							//	Navigator.Clear();

							//Generate the path here so we can start moving..
							Navigation.Navigation.NP.MoveTo(FunkyGame.Game.InteractableCachedObject.Position, "ReturnToOOCLoc", true);

							//Setup a temp target that the handler will use
							obj = new CacheObject(FunkyGame.Game.InteractableCachedObject.Position, TargetType.LineOfSight, 1d, "ReturnToOOCLoc", 10f, FunkyGame.Game.InteractableCachedObject.RAGUID);
							return true;
						}
					}

					//Check if we engaged in combat..
					bool EngagedInCombat = false;
					float distanceFromStart = 0f;
					if (!FunkyGame.Targeting.Cache.LastCachedTarget.Equals(ObjectCache.FakeCacheObject) && !FunkyGame.Targeting.Cache.Backtracking && FunkyGame.Targeting.Cache.StartingLocation != Vector3.Zero)
					{
						EngagedInCombat = true;
						distanceFromStart = FunkyGame.Hero.Position.Distance(FunkyGame.Targeting.Cache.StartingLocation);
						//lets see how far we are from our starting location.
						if (distanceFromStart > 20f &&
							  !Navigation.Navigation.CanRayCast(FunkyGame.Hero.Position, PlayerMover.vLastMoveTo, UseSearchGridProvider: true))
						{
							Logger.Write(LogLevel.Movement, "Updating Navigator in Target Refresh");

							SkipAheadCache.ClearCache();
							Navigator.Clear();
							//Navigator.MoveTo(Funky.PlayerMover.vLastMoveTo, "original destination", true);
						}
					}

					//Check if our current path intersects avoidances. (When not in town, and not currently inside avoidance)
					if (!FunkyGame.Hero.bIsInTown && (FunkyBaseExtension.Settings.Avoidance.AttemptAvoidanceMovements) //|| FunkyGame.Hero.CriticalAvoidance)
							&& Navigation.Navigation.NP.CurrentPath.Count > 0
							&& FunkyGame.Targeting.Cache.Environment.TriggeringAvoidances.Count == 0)
					{
						if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(FunkyGame.Hero.Position, Navigation.Navigation.NP.CurrentPath.Current))
						{
							obj = new CacheObject(FunkyGame.Hero.Position, TargetType.NoMovement, 20000, "AvoidanceIntersection", 2.5f, -1);
							return true;
						}
					}

					//Backtracking Check..
					if (EngagedInCombat && FunkyBaseExtension.Settings.Backtracking.EnableBacktracking && distanceFromStart >= FunkyBaseExtension.Settings.Backtracking.MinimumDistanceFromStart)
					{
						FunkyGame.Targeting.Cache.Backtracking = true;
						obj = new CacheObject(FunkyGame.Targeting.Cache.StartingLocation, TargetType.Backtrack, 20000, "Backtracking", 2.5f);
						return true;
					}


				}

				return obj != null;
			};
		}
	}
}
