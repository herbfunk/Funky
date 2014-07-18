using System;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using FunkyBot.Config.Settings;
using FunkyBot.Game;
using FunkyBot.Game.Bounty;
using FunkyBot.Movement;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;

namespace FunkyBot.Targeting.Behaviors
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
					if (DateTime.Now.Subtract(Bot.Targeting.Cache.lastHadUnitInSights).TotalMilliseconds <= Bot.Settings.General.AfterCombatDelay && DateTime.Now.Subtract(Bot.Targeting.Cache.lastHadEliteUnitInSights).TotalMilliseconds <= 10000 ||
						//Cut the delay time in half for non-elite monsters!
						DateTime.Now.Subtract(Bot.Targeting.Cache.lastHadUnitInSights).TotalMilliseconds <= Bot.Settings.General.AfterCombatDelay)
					{
						obj = new CacheObject(FunkyGame.Hero.Position, TargetType.NoMovement, 20000, "WaitForLootDrops", 2f, -1);
						return true;

					}
					//Herbfunks wait after loot containers are opened. 3s for rare chests, half the settings delay for everything else.
					if ((DateTime.Now.Subtract(Bot.Targeting.Cache.lastHadRareChestAsTarget).TotalMilliseconds <= 3750) ||
						(DateTime.Now.Subtract(Bot.Targeting.Cache.lastHadContainerAsTarget).TotalMilliseconds <= (Bot.Settings.General.AfterCombatDelay * 1.25)))
					{
						obj = new CacheObject(FunkyGame.Hero.Position, TargetType.NoMovement, 20000, "ContainerLootDropsWait", 2f, -1);
						return true;
					}

					if (DateTime.Now.Subtract(Bot.Targeting.Cache.lastSeenCursedShrine).TotalMilliseconds <= (1000))
					{
						if (Bot.Settings.AdventureMode.EnableAdventuringMode && FunkyGame.AdventureMode && Bot.Game.Bounty.CurrentBountyCacheEntry != null && Bot.Game.Bounty.CurrentBountyCacheEntry.Type == BountyQuestTypes.CursedEvent)
						{
							Logger.DBLog.Info("[Funky] Cursed Object Found During Cursed Bounty -- Enabling LOS movement for all Units!");
							SettingLOSMovement.LOSSettingsTag.MiniumRangeObjects = 10f;
							SettingLOSMovement.LOSSettingsTag.MaximumRange = 125;
							Bot.Game.Bounty.AllowAnyUnitForLOSMovement = true;
							SettingCluster.ClusterSettingsTag = SettingCluster.DisabledClustering;
						}

						Bot.Targeting.Cache.UpdateQuestMonsterProperty = true;
						obj = new CacheObject(FunkyGame.Hero.Position, TargetType.NoMovement, 20000, "CursedShrineWait", 2f, -1);
						return true;
					}

					Bot.Targeting.Cache.UpdateQuestMonsterProperty = false;

					// Finally, a special check for waiting for wrath of the berserker cooldown before engaging Azmodan
					if (Hotbar.HasPower(SNOPower.Barbarian_WrathOfTheBerserker) && Bot.Settings.Barbarian.bWaitForWrath && !Bot.Character.Class.Abilities[SNOPower.Barbarian_WrathOfTheBerserker].AbilityUseTimer() &&
						FunkyGame.Hero.CurrentWorldDynamicID == 121214 &&
						(Vector3.Distance(FunkyGame.Hero.Position, new Vector3(711.25f, 716.25f, 80.13903f)) <= 40f || Vector3.Distance(FunkyGame.Hero.Position, new Vector3(546.8467f, 551.7733f, 1.576313f)) <= 40f))
					{
						Logger.DBLog.InfoFormat("[Funky] Waiting for Wrath Of The Berserker cooldown before continuing to Azmodan.");
						obj = new CacheObject(FunkyGame.Hero.Position, TargetType.NoMovement, 20000, "GilesWaitForWrath", 0f, -1);
						InactivityDetector.Reset();
						return true;
					}
					// And a special check for wizard archon
					if (Hotbar.HasPower(SNOPower.Wizard_Archon) && !Bot.Character.Class.Abilities[SNOPower.Wizard_Archon].AbilityUseTimer() && Bot.Settings.Wizard.bWaitForArchon && ZetaDia.CurrentWorldId == 121214 &&
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
					if (Bot.Game.InteractableCachedObject!=null && (!FunkyGame.Hero.bIsInTown || !BrainBehavior.IsVendoring))
					{
						if (Bot.Game.InteractableCachedObject.Position.Distance(FunkyGame.Hero.Position) > 50f)
						{
							//if (Bot.Targeting.Cache.LastCachedTarget.Position != Bot.Game.Profile.InteractableCachedObject.Position)
							//	Navigator.Clear();

							//Generate the path here so we can start moving..
							Navigation.NP.MoveTo(Bot.Game.InteractableCachedObject.Position, "ReturnToOOCLoc", true);

							//Setup a temp target that the handler will use
							obj = new CacheObject(Bot.Game.InteractableCachedObject.Position, TargetType.LineOfSight, 1d, "ReturnToOOCLoc", 10f, Bot.Game.InteractableCachedObject.RAGUID);
							return true;
						}
					}

					//Check if we engaged in combat..
					bool EngagedInCombat = false;
					float distanceFromStart = 0f;
					if (Bot.Targeting.Cache.LastCachedTarget != ObjectCache.FakeCacheObject && !Bot.Targeting.Cache.Backtracking && Bot.Targeting.Cache.StartingLocation != Vector3.Zero)
					{
						EngagedInCombat = true;
						distanceFromStart = FunkyGame.Hero.Position.Distance(Bot.Targeting.Cache.StartingLocation);
						//lets see how far we are from our starting location.
						if (distanceFromStart > 20f &&
							  !Navigation.CanRayCast(FunkyGame.Hero.Position, PlayerMover.vLastMoveTo, UseSearchGridProvider: true))
						{
							Logger.Write(LogLevel.Movement, "Updating Navigator in Target Refresh");

							SkipAheadCache.ClearCache();
							Navigator.Clear();
							//Navigator.MoveTo(Funky.PlayerMover.vLastMoveTo, "original destination", true);
						}
					}

					//Check if our current path intersects avoidances. (When not in town, and not currently inside avoidance)
					if (!FunkyGame.Hero.bIsInTown && (Bot.Settings.Avoidance.AttemptAvoidanceMovements) //|| FunkyGame.Hero.CriticalAvoidance)
							&& Navigation.NP.CurrentPath.Count > 0
							&& Bot.Targeting.Cache.Environment.TriggeringAvoidances.Count == 0)
					{
						if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(FunkyGame.Hero.Position, Navigation.NP.CurrentPath.Current))
						{
							obj = new CacheObject(FunkyGame.Hero.Position, TargetType.NoMovement, 20000, "AvoidanceIntersection", 2.5f, -1);
							return true;
						}
					}

					//Backtracking Check..
					if (EngagedInCombat && Bot.Settings.Backtracking.EnableBacktracking && distanceFromStart >= Bot.Settings.Backtracking.MinimumDistanceFromStart)
					{
						Bot.Targeting.Cache.Backtracking = true;
						obj = new CacheObject(Bot.Targeting.Cache.StartingLocation, TargetType.Backtrack, 20000, "Backtracking", 2.5f);
						return true;
					}
					
					
				}

				return obj != null;
			};
		}
	}
}
