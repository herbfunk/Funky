using System;
using System.Linq;
using fBaseXtensions.Cache;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Game.Hero.Class;
using fBaseXtensions.Monitor;
using fBaseXtensions.Navigation;
using fBaseXtensions.Settings;
using Zeta.Bot.Dungeons;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fBaseXtensions.Game.Hero
{
	public class ActiveHero
	{
		public ActiveHero()
		{
			OnLevelAreaIDChanged += OnLevelAreaIDChangedHandler;
			FunkyGame.Bounty.ActiveBountyChanged += ActiveBountyChangedHandler;
			Class = null;
			Equipment.OnEquippedItemsChanged += EquippmentChangedHandler;
		}

		#region Properties
		public PlayerClass Class { get; set; }
		private DateTime lastUpdatedPlayer { get; set; }
		internal DateTime lastPreformedNonCombatAction { get; set; }
		public bool bIsIncapacitated { get; set; }
		public bool bIsRooted { get; set; }
		public bool bIsInTown { get; set; }
		public bool bIsInBossEncounter { get; set; }
		private double dcurrentHealthPct;
		public double dCurrentHealthPct
		{
			get { return dcurrentHealthPct; }
			set
			{
				dcurrentHealthPct = value;
			}
		}
		public double dCurrentEnergy { get; set; }
		public double dCurrentEnergyPct { get; set; }
		public double dDiscipline { get; set; }
		public double dDisciplinePct { get; set; }
		public double MaxEnergy = 0;
		public double MaxDiscipline = 0;

		public float fCharacterRadius { get; set; }
		public Sphere CharacterSphere
		{
			get
			{
				return new Sphere(Position, fCharacterRadius);
			}
		}
		internal bool CriticalAvoidance { get; set; }
		public int iMyDynamicID { get; set; }
		public int iMyLevel { get; set; }
		public int iTotalPotions { get; set; }
		public int iSceneID { get; set; }
		public string SceneName { get; set; }
		//internal Pets PetData { get; set; }
		//internal Backpack BackPack { get; set; }

		public float PickupRadius { get; set; }
		public int FreeBackpackSlots { get; set; }
		public int CurrentExp { get; set; }

		private int coinage;
		public int Coinage
		{
			get
			{
				return coinage;
			}
			set
			{
				if (coinage != value)
				{
					//Gold Inactivity
					GoldInactivity.LastCoinageUpdate = DateTime.Now;
				}

				coinage = value;
				UpdateCoinage = false;
			}
		}
		public int CurrentWorldDynamicID { get; set; }
		public int CurrentWorldID { get; set; }
		public int iCurrentLevelID { get; set; }
		public bool UpdateCoinage { get; set; }


		//Returns Live Data
		private DateTime lastPositionUpdate = DateTime.Today;
		private Vector3 currentPosition = Vector3.Zero;
		public Vector3 Position
		{
			get
			{
				//Because we don't want to update this X amount of times in a single loop!
				if (DateTime.Now.Subtract(lastPositionUpdate).TotalMilliseconds < 150)
					return currentPosition;

				lastPositionUpdate = DateTime.Now;
				Vector3 updatedPosition = currentPosition;
				try
				{
					using (ZetaDia.Memory.AcquireFrame())
					{
						updatedPosition = ZetaDia.Me.Position;
					}
				}
				catch (NullReferenceException)
				{
					//lastPosition=Vector3.Zero;
				}

				//Position Change Check.
				if (!updatedPosition.Equals(currentPosition))
				{
					positionChangedHandler(updatedPosition);
				}

				return currentPosition;
			}
		}


		private AnimationState lastAnimationState = AnimationState.Invalid;
		public AnimationState CurrentAnimationState
		{
			get
			{
				try
				{
					using (ZetaDia.Memory.AcquireFrame())
					{
						lastAnimationState = ZetaDia.Me.CommonData.AnimationState;
					}
				}
				catch
				{

				}
				return lastAnimationState;
			}
		}

		private SNOAnim Lastsnoanim = SNOAnim.a1dun_Cath_chest_idle;
		public SNOAnim CurrentSNOAnim
		{
			get
			{

				try
				{
					using (ZetaDia.Memory.AcquireFrame())
					{
						Lastsnoanim = ZetaDia.Me.CommonData.CurrentAnimation;
					}
				}
				catch
				{

				}
				return Lastsnoanim;
			}
		}

		public int ActorSno
		{
			get { return actorSNO; }
			set
			{
				actorSNO = value;
				_snoactor = (SNOActor)value;
			}
		}
		private int actorSNO;

		private SNOActor _snoactor;
		public SNOActor SnoActor
		{
			get { return _snoactor; }
		}

		internal DateTime lastUpdateNonEssentialData = DateTime.Today;
		internal DateTime lastCheckedSceneID = DateTime.Today;

		private bool isMoving = false;
		public bool IsMoving
		{
			get
			{
				if (ShouldRefreshMovementProperty)
					RefreshMovementCache();

				return isMoving;
			}
		}
		private MovementState curMoveState = MovementState.None;
		public MovementState currentMovementState
		{
			get
			{
				if (ShouldRefreshMovementProperty)
					RefreshMovementCache();
				return curMoveState;
			}
		}
		private StuckFlags stuckflags=StuckFlags.WasStuck;
		public StuckFlags Stuckflags
		{
			get
			{
				if (ShouldRefreshMovementProperty)
					RefreshMovementCache();
				return stuckflags;
			}
		}

		private DateTime LastUpdatedMovementData = DateTime.MinValue;
		private bool ShouldRefreshMovementProperty
		{
			get { return DateTime.Now.Subtract(LastUpdatedMovementData).TotalMilliseconds > 25; }
		}
		private void RefreshMovementCache()
		{
			LastUpdatedMovementData = DateTime.Now;

			//These vars are used to accuratly predict what the bot is doing (Melee Movement/Combat)
			using (ZetaDia.Memory.AcquireFrame())
			{
				// If we aren't in the game of a world is loading, don't do anything yet
				if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld)
					return;
				try
				{
					ActorMovement botMovement = ZetaDia.Me.Movement;
					isMoving = botMovement.IsMoving;
					curMoveState=botMovement.MovementState;
					stuckflags = botMovement.StuckFlags;
				}
				catch
				{

				}
			}
		}


		#endregion
		#region Events

		private int LastWorldID = -1;
		private bool LastLevelIDChangeWasTownRun;
		public delegate void LevelAreaIDChanged(int ID);
		public event LevelAreaIDChanged OnLevelAreaIDChanged;
		private void levelareaIDchanged(int ID)
		{
			iCurrentLevelID = ID;
			Logger.Write(Helpers.LogLevel.Event, "Level Area ID has Changed to {0}", ID);
			


			if (!BrainBehavior.IsVendoring)
			{
				//Check for World ID change!
				if (FunkyGame.Hero.CurrentWorldDynamicID != LastWorldID)
				{
					Logger.Write(Helpers.LogLevel.Event, "World ID changed.. clearing Profile Interactable Cache.");
					LastWorldID = FunkyGame.Hero.CurrentWorldDynamicID;
					ObjectCache.InteractableObjectCache.Clear();
					//Gold Inactivity
					GoldInactivity.LastCoinageUpdate = DateTime.Now;
				}

				if (!LastLevelIDChangeWasTownRun)
				{//Do full clear

					BackTrackCache.cacheMovementGPRs.Clear();
					FunkyGame.Navigation.LOSBlacklistedRAGUIDs.Clear();
					FunkyGame.Game.InteractableCachedObject = null;
				}
				else
				{
					//Gold Inactivity
					GoldInactivity.LastCoinageUpdate = DateTime.Now;
				}

				//Clear the object cache!
				ObjectCache.Objects.Clear();
				//ObjectCache.cacheSnoCollection.ClearDictionaryCacheEntries();
				ObjectCache.RemovalCheck = false;

				//Reset Skip Ahead Cache
				SkipAheadCache.ClearCache();

				FunkyGame.Hero.UpdateCoinage = true;

				//Check active bounty
				if (FunkyGame.AdventureMode && SettingAdventureMode.AdventureModeSettingsTag.EnableAdventuringMode)
					CheckActiveBounty();

				Navigator.SearchGridProvider.Update();

				LastLevelIDChangeWasTownRun = false;
			}
			else if (FunkyGame.Hero.bIsInTown)
			{
				LastLevelIDChangeWasTownRun = true;
			}
			if (OnLevelAreaIDChanged != null)
				OnLevelAreaIDChanged(ID);
		}
		private void ActiveBountyChangedHandler()
		{
			CheckActiveBounty();
		}

		private void CheckActiveBounty()
		{
			if (FunkyGame.AdventureMode)
			{
				FunkyGame.Game.ResetCombatModifiers();

				if (!FunkyGame.Bounty.IsInRiftWorld)
				{
					//We could check that active bounty has been completed..
					if (FunkyGame.Bounty.ActiveBounty != null && FunkyGame.Bounty.BountyQuestStates.ContainsKey(FunkyGame.Bounty.ActiveBounty.QuestSNO) && FunkyGame.Bounty.BountyQuestStates[FunkyGame.Bounty.ActiveBounty.QuestSNO] == QuestState.Completed)
					{
						Logger.Write(Helpers.LogLevel.Bounty, "ActiveBounty Quest State is Completed!");
					}

					if (FunkyGame.Bounty.CurrentBountyCacheEntry != null)
					{
						var CurrentBountyCacheEntry = FunkyGame.Bounty.CurrentBountyCacheEntry;
						Logger.Write(Helpers.LogLevel.Bounty, "Checking Bounty Type {0}", CurrentBountyCacheEntry.Type);
						
						if (!SettingAdventureMode.AdventureModeSettingsTag.EnableAdventuringMode) return;

						int curLevelID = FunkyGame.Hero.iCurrentLevelID;

						//Check if We should Modify the Bots Combat Behavior
						switch (FunkyGame.Bounty.CurrentBountyCacheEntry.Type)
						{
							case BountyTypes.Clear:
								if (CurrentBountyCacheEntry.EndingLevelAreaID == curLevelID)
								{
									Logger.Write(Helpers.LogLevel.Bounty, "Bounty Level ID Match (Clear) -- Disabling Cluster Logic!");
									SettingCluster.ClusterSettingsTag = SettingCluster.DisabledClustering;
									FunkyGame.Game.AllowAnyUnitForLOSMovement = true;
								}

								break;
							case BountyTypes.Kill:


								if (CurrentBountyCacheEntry.StartingLevelAreaID == curLevelID || CurrentBountyCacheEntry.EndingLevelAreaID == curLevelID ||
									CurrentBountyCacheEntry.LevelAreaIDs != null && CurrentBountyCacheEntry.LevelAreaIDs.Any(i => i == curLevelID))
								{
									Logger.Write(Helpers.LogLevel.Bounty, "Bounty Level ID Match (Kill) -- Disabling Cluster Logic and Enabling Navigation of Points!");
									SettingCluster.ClusterSettingsTag = SettingCluster.DisabledClustering;

									//only enable when its the ending level.
									FunkyGame.Game.ShouldNavigateMinimapPoints = CurrentBountyCacheEntry.EndingLevelAreaID == curLevelID;
								}
								break;
							case BountyTypes.CursedEvent:
								if (CurrentBountyCacheEntry.EndingLevelAreaID == curLevelID)
								{
									Logger.Write(Helpers.LogLevel.Bounty, "Bounty Level ID Match (CursedEvent) -- Enabling Navigation of Points!");
									FunkyGame.Game.ShouldNavigateMinimapPoints = true;
									FunkyGame.Game.QuestMode = true;
								}
								break;
							case BountyTypes.Boss:
								if (CurrentBountyCacheEntry.StartingLevelAreaID == curLevelID)
								{
									Logger.Write(Helpers.LogLevel.Bounty, "Bounty Level ID Match (Boss) -- Enabling Navigation of Points!");
									FunkyGame.Game.ShouldNavigateMinimapPoints = true;
								}
								break;
							case BountyTypes.Event:
								if (CurrentBountyCacheEntry.EndingLevelAreaID == curLevelID)
								{
									Logger.Write(Helpers.LogLevel.Bounty, "Bounty Level ID Match (Event) -- Enabling Navigation of Points!");
									FunkyGame.Game.ShouldNavigateMinimapPoints = true;
									FunkyGame.Game.QuestMode = true;
								}
								break;
						}
					}
				}
				else if (FunkyGame.Bounty.ActiveBounty != null)
				{
					int curStep = ((BountyCache.QuestInfoCache)FunkyGame.Bounty.ActiveBounty).Step;

					//Killing..
					if (curStep == 1 || curStep == 13)
					{
						FunkyGame.Bounty.RefreshRiftMapMarkers();
						FunkyGame.Game.ShouldNavigateMinimapPoints = true;
						SettingCluster.ClusterSettingsTag = SettingCluster.DisabledClustering;
						if (curStep == 13) MonitorSettings.MonitorSettingsTag.GoldInactivityTimeoutSeconds = 0;
					}
					else if (curStep == 3 || curStep==16)//Boss Spawned
					{
						FunkyGame.Bounty.RefreshRiftMapMarkers();
						SettingCluster.ClusterSettingsTag = FunkyBaseExtension.Settings.Cluster;
						FunkyGame.Game.ShouldNavigateMinimapPoints = false;
						if (curStep == 16) MonitorSettings.MonitorSettingsTag.GoldInactivityTimeoutSeconds = 0;
					}
					else//Boss Killed 10 / 34
					{
						SettingCluster.ClusterSettingsTag = FunkyBaseExtension.Settings.Cluster;
						FunkyGame.Game.ShouldNavigateMinimapPoints = false;
						if (curStep == 10 || curStep == 34) MonitorSettings.MonitorSettingsTag.GoldInactivityTimeoutSeconds = 0;
					}
				}
			}
		}

		public delegate void HealthValueChanged(double oldvalue, double newvalue);
		public event HealthValueChanged OnHealthChanged;
		private void healthvalueChanged(double oldvalue, double newvalue)
		{
			dCurrentHealthPct = newvalue;
			if (OnHealthChanged != null)
				OnHealthChanged(oldvalue, newvalue);
		}

		public delegate void PositionChanged(Vector3 NewPosition);
		public event PositionChanged OnPositionChanged;
		private void positionChangedHandler(Vector3 position)
		{
			//update reference
			currentPosition = position;

			//raise event
			if (OnPositionChanged != null)
				OnPositionChanged(position);
		}
		#endregion

		private void EquippmentChangedHandler()
		{
			Logger.DBLog.InfoFormat("Equippment has changed!");

			if (!PlayerClass.ShouldRecreatePlayerClass)
				PlayerClass.ShouldRecreatePlayerClass = true;
		}
		private void OnLevelAreaIDChangedHandler(int ID)
		{
			if (FunkyGame.AdventureMode)
			{
				FunkyGame.Bounty.UpdateActiveBounty();
				if (FunkyGame.Bounty.ActiveBounty != null)
				{//No Longer Null.. Do Full Refresh!
					FunkyGame.Bounty.RefreshLevelChanged();
				}
			}
		}

		public void Update(bool combat = false, bool force = false)
		{


			double lastUpdate = DateTime.Now.Subtract(lastUpdatedPlayer).TotalMilliseconds;
			//Update only every 100ms, unless in combat than 25ms..
			if (!force &&
				  (combat && lastUpdate < 50 || lastUpdate < 150)) return;

			using (ZetaDia.Memory.AcquireFrame())
			{
				try
				{
					// If we aren't in the game of a world is loading, don't do anything yet
					if (!ZetaDia.IsInGame || !ZetaDia.Me.IsValid || ZetaDia.IsLoadingWorld) return;

					var me = ZetaDia.Me;
					if (me == null) return;

					//update actorSNO
					if (ActorSno == -1)
					{
						ActorSno = me.ActorSNO;
					}

					if (FunkyGame.CurrentActorClass == ActorClass.DemonHunter)
					{
						dDiscipline = me.CurrentSecondaryResource;
						if (dDiscipline > MaxDiscipline) MaxDiscipline = dDiscipline;
						dDisciplinePct = dDiscipline / MaxDiscipline;
					}

					double curhealthpct = me.HitpointsCurrentPct;
					if (!dcurrentHealthPct.Equals(curhealthpct))
						healthvalueChanged(dcurrentHealthPct, curhealthpct);

					dCurrentEnergy = me.CurrentPrimaryResource;
					if (dCurrentEnergy > MaxEnergy) MaxEnergy = dCurrentEnergy;
					dCurrentEnergyPct = dCurrentEnergy / MaxEnergy;

					bIsInBossEncounter = me.IsInBossEncounter;
					bIsInTown = ZetaDia.IsInTown;
					bIsRooted = me.IsRooted || Hotbar.HasDebuff(SNOPower.MonsterAffix_JailerCast);

					var frozen = me.IsFrozen || Hotbar.HasDebuff(SNOPower.CritDebuffCold);
					var feared = me.IsFeared || Hotbar.HasDebuff(SNOPower.DebuffFeared);

					if (feared || me.IsStunned || frozen || me.IsBlind)
						bIsIncapacitated = true;
					else
					{
						var bIsInKnockBack = (me.CommonData.GetAttribute<int>(ActorAttributeType.InKnockback) != 0);
						bIsIncapacitated = bIsInKnockBack; //|| FunkyGame.Hero.Class.KnockbackLandAnims.Contains(CurrentSNOAnim);
					}



					int currentLevelAreaID = ZetaDia.CurrentLevelAreaId;
					int currentWorldID = ZetaDia.CurrentWorldId;
					bool inRift = TheCache.riftWorldIds.Contains(currentWorldID);

					if (iCurrentLevelID != currentLevelAreaID || (inRift && currentWorldID != CurrentWorldID))
					{
						CurrentWorldDynamicID = ZetaDia.CurrentWorldDynamicId;
						CurrentWorldID = ZetaDia.CurrentWorldId;
						levelareaIDchanged(currentLevelAreaID);
					}



					if (FunkyGame.CurrentHeroLevel == 70)
						CurrentExp = me.ParagonCurrentExperience;
					else
						CurrentExp = me.CurrentExperience;

					//Set Character Radius?
					if (fCharacterRadius == 0f)
					{
						fCharacterRadius = me.ActorInfo.Sphere.Radius;

						//Wizards are short -- causing issues (At least Male Wizard is!)
						//if (Bot.Game.ActorClass == ActorClass.Wizard) this.fCharacterRadius += 1f;
					}


					//Update vars that are not essential to combat (survival).
					if (DateTime.Now.Subtract(lastUpdateNonEssentialData).TotalSeconds > 30)
					{
						lastUpdateNonEssentialData = DateTime.Now;

						//update level if not 60 else update paragonlevel
						if (iMyLevel < 70) iMyLevel = me.Level;

						iMyDynamicID = me.CommonData.DynamicId;
						FreeBackpackSlots = me.Inventory.NumFreeBackpackSlots;
						PickupRadius = me.GoldPickupRadius;
						Coinage = me.Inventory.Coinage;
					}

					if (UpdateCoinage)
						Coinage = me.Inventory.Coinage;

					//Check current scence every 1.5 seconds
					if (!bIsInTown && DateTime.Now.Subtract(lastCheckedSceneID).TotalSeconds > 1.50)
					{
						//Get the current guid, compare/update.
						int CurrentSceneID = me.CurrentScene.SceneGuid;
						if (CurrentSceneID != iSceneID)
						{
							iSceneID = CurrentSceneID;
							SceneName = me.CurrentScene.Name;
						}
						lastCheckedSceneID = DateTime.Now;
					}
				}
				catch (Exception)
				{

				}
			}
			lastUpdatedPlayer = DateTime.Now;
		}

		public void WaitWhileAnimating(int iMaxSafetyLoops = 10, bool bWaitForAttacking = false)
		{
			bool bKeepLooping = true;
			int iSafetyLoops = 0;
			while (bKeepLooping)
			{
				iSafetyLoops++;
				if (iSafetyLoops > iMaxSafetyLoops)
					bKeepLooping = false;
				bool bIsAnimating = false;
				try
				{
					AnimationState currentanimstate = CurrentAnimationState;
					if (currentanimstate==(AnimationState.Casting) || currentanimstate==AnimationState.Channeling)
						bIsAnimating = true;
					if (bWaitForAttacking && currentanimstate==AnimationState.Attacking)
						bIsAnimating = true;
				}
				catch (NullReferenceException)
				{
					bIsAnimating = true;
				}
				if (!bIsAnimating)
					bKeepLooping = false;
			}
		}

		public string DebugString()
		{
			return String.Format("Character Info \r\n" +
															   "CurrentLevelID={0} -- WorldID={18} WorldDynamicID={1} -- SceneID={2} \r\n" +
																"IsIncapacitated={16} IsRooted={17}  \r\n" +
															   "SNOAnim={3} AnimState={4} \r\n" +
															   "Incapacitated={5} -- Rooted={6} \r\n" +
															   "Current Health={7} -- Current Energy={8}[{9}%] \r\n" +
															   "Current Coin={10} -- CurrentXP={11} \r\n" +
															   "Is In Boss Encounter={12}  IsInTown={15}\r\n" +
															   "SNOActor ID: {13} -- {14}",
															   iCurrentLevelID, CurrentWorldDynamicID, iSceneID,
															   Lastsnoanim.ToString(), lastAnimationState.ToString(),
															   bIsIncapacitated.ToString(), bIsRooted.ToString(),
															   dCurrentHealthPct, dCurrentEnergy, dCurrentEnergyPct,
															   Coinage, CurrentExp,
															   bIsInBossEncounter,
															   ActorSno,SnoActor,
															   bIsInTown,
															   bIsIncapacitated, bIsRooted, CurrentWorldID);

		}


		public static decimal TotalResourceCostReduction = 0;
		public static decimal TotalSkillCooldownReduction = 0;
		internal static void UpdateStaticProperties()
		{
			if (FunkyGame.GameIsInvalid) return;
			var skillcooldownreduction = ZetaDia.Me.CommonData.GetAttribute<float>(ActorAttributeType.PowerCooldownReductionPercentAll) * 100;
			TotalSkillCooldownReduction = Math.Round(Convert.ToDecimal(skillcooldownreduction),1);

			var resourcecostreduction =ZetaDia.Me.CommonData.GetAttribute<float>(ActorAttributeType.ResourceCostReductionPercentAll) * 100;
			TotalResourceCostReduction = Math.Round(Convert.ToDecimal(resourcecostreduction),1);
			Logger.DBLog.InfoFormat("Skill cooldown reduction {0}", TotalSkillCooldownReduction);
			Logger.DBLog.InfoFormat("Skill resource cost reduction {0}", TotalResourceCostReduction);
		}
	}
}
