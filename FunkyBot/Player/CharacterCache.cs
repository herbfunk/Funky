using System;
using FunkyBot.DBHandlers;
using FunkyBot.Movement;
using Zeta.Bot;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player
{
	///<summary>
	///Cache of all values Character related and variable.
	///</summary>
	public class CharacterCache
	{
		public CharacterCache()
		{
			lastUpdatedPlayer = DateTime.Today;
			lastPreformedNonCombatAction = DateTime.Today;
			bIsIncapacitated = false;
			bIsRooted = false;
			bIsInTown = false;
			bIsInBossEncounter = false;
			dcurrentHealthPct = 0d;
			dCurrentEnergy = 0d;
			dCurrentEnergyPct = 0d;
			dDiscipline = 0d;
			dDisciplinePct = 0d;
			iMyDynamicID = 0;
			iMyLevel = 1;
			iSceneID = -1;
			iCurrentWorldID = -1;
			iCurrentLevelID = -1;
			BackPack = new Backpack();
			PetData = new Pets();
			PickupRadius = 1;
			coinage = 0;
			fCharacterRadius = 0f;
			CurrentExp = 0;
		}

		#region Events

		public delegate void LevelAreaIDChanged(int ID);
		public event LevelAreaIDChanged OnLevelAreaIDChanged;
		private void levelareaIDchanged(int ID)
		{
			iCurrentLevelID = ID;
			if (OnLevelAreaIDChanged != null)
				OnLevelAreaIDChanged(ID);
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
		#region Properties

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
		internal float fCharacterRadius { get; set; }
		internal Sphere CharacterSphere
		{
			get
			{
				return new Sphere(Position, fCharacterRadius);
			}
		}
		internal bool CriticalAvoidance { get; set; }
		internal int iMyDynamicID { get; set; }
		internal int iMyLevel { get; set; }
		internal int iTotalPotions { get; set; }
		internal int iSceneID { get; set; }
		internal Pets PetData { get; set; }
		internal Backpack BackPack { get; set; }
		internal float PickupRadius { get; set; }
		internal int FreeBackpackSlots { get; set; }
		internal int CurrentExp { get; set; }

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
					Bot.Game.GoldTimeoutChecker.LastCoinageUpdate = DateTime.Now;
				}

				coinage = value;
				UpdateCoinage = false;
			}
		}
		public int iCurrentWorldID { get; set; }
		public int iCurrentLevelID { get; set; }
		internal bool UpdateCoinage { get; set; }
		public bool ShouldFlee
		{
			get
			{
				bool flee = Bot.Settings.Fleeing.EnableFleeingBehavior && Bot.Character.Data.dCurrentHealthPct <= Bot.Settings.Fleeing.FleeBotMinimumHealthPercent;
				return flee;
			}
		}

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
		internal GridPoint PointPosition
		{
			get
			{
				return Position;
			}
		}

		private AnimationState lastAnimationState = AnimationState.Invalid;
		internal AnimationState CurrentAnimationState
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
		internal SNOAnim CurrentSNOAnim
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

		internal DateTime lastUpdateNonEssentialData = DateTime.Today;
		internal DateTime lastCheckedSceneID = DateTime.Today;


		#endregion
		#region Methods

		internal void Update(bool combat = false, bool force = false)
		{


			double lastUpdate = DateTime.Now.Subtract(lastUpdatedPlayer).TotalMilliseconds;
			//Update only every 100ms, unless in combat than 25ms..
			if (!force &&
				  (combat && lastUpdate < 50 || lastUpdate < 150)) return;

			using (ZetaDia.Memory.AcquireFrame())
			{
				// If we aren't in the game of a world is loading, don't do anything yet
				if (!ZetaDia.IsInGame || !ZetaDia.Me.IsValid || ZetaDia.IsLoadingWorld) return;

				var me = ZetaDia.Me;
				if (me == null) return;

				try
				{
					if (Bot.Character.Class.AC == ActorClass.DemonHunter)
					{
						dDiscipline = me.CurrentSecondaryResource;
						dDisciplinePct = Bot.Character.Data.dDiscipline / me.MaxSecondaryResource;
					}

					double curhealthpct = me.HitpointsCurrentPct;
					if (!dcurrentHealthPct.Equals(curhealthpct))
						healthvalueChanged(dcurrentHealthPct, curhealthpct);
					
					dCurrentEnergy = me.CurrentPrimaryResource;
					dCurrentEnergyPct = dCurrentEnergy / me.MaxPrimaryResource;

					//Critical Avoidance (when no avoidance is set!)
					if (dCurrentHealthPct < 0.50d && !Bot.Settings.Avoidance.AttemptAvoidanceMovements &&
						 !PowerManager.CanCast(SNOPower.DrinkHealthPotion))
						CriticalAvoidance = true;
					else if (CriticalAvoidance && !ItemIdentifyBehavior.shouldPreformOOCItemIDing && !TownPortalBehavior.FunkyTPBehaviorFlag && dCurrentHealthPct > 0.5)
						//Disable it when not OOC/TP/Low health still..
						CriticalAvoidance = false;

					bIsInBossEncounter = me.IsInBossEncounter;
					bIsInTown = ZetaDia.IsInTown;
					bIsRooted = me.IsRooted;

					if (me.IsFeared || me.IsStunned || me.IsFrozen || me.IsBlind)
						bIsIncapacitated = true;
					else
					{
						var bIsInKnockBack = (me.CommonData.GetAttribute<int>(ActorAttributeType.InKnockback) != 0);
						bIsIncapacitated = bIsInKnockBack || Bot.Character.Class.KnockbackLandAnims.Contains(CurrentSNOAnim);
					}

					int currentLevelAreaID = ZetaDia.CurrentLevelAreaId;
					if (iCurrentLevelID != currentLevelAreaID)
					{
						levelareaIDchanged(currentLevelAreaID);
						iCurrentWorldID = ZetaDia.CurrentWorldDynamicId;
					}

					if (Bot.Character.Account.CurrentLevel == 60)
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
						if (iMyLevel < 60) iMyLevel = me.Level;

						iMyDynamicID = me.CommonData.DynamicId;
						FreeBackpackSlots = me.Inventory.NumFreeBackpackSlots;
						PickupRadius = me.GoldPickupRadius;
						Coinage = me.Inventory.Coinage;
						//Clear our BPItems list..
						BackPack.BPItems.Clear();
					}

					if (UpdateCoinage)
						Coinage = me.Inventory.Coinage;

					//Check current scence every 1.5 seconds
					if (!bIsInTown && DateTime.Now.Subtract(lastCheckedSceneID).TotalSeconds > 1.50)
					{
						//Get the current guid, compare/update.
						int CurrentSceneID = me.CurrentScene.SceneGuid;
						if (CurrentSceneID != Bot.Character.Data.iSceneID)
						{
							Bot.Character.Data.iSceneID = CurrentSceneID;
						}
						lastCheckedSceneID = DateTime.Now;
					}
				}
				catch (AccessViolationException)
				{

				}
			}
			lastUpdatedPlayer = DateTime.Now;
		}
		// **********************************************************************************************
		// *****      Quick and Dirty routine just to force a wait until the character is "free"    *****
		// **********************************************************************************************
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
					if (currentanimstate.HasFlag(AnimationState.Casting | AnimationState.Channeling))
						bIsAnimating = true;
					if (bWaitForAttacking && (currentanimstate.HasFlag(AnimationState.Attacking)))
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
															   "CurrentLevelID={0} -- WorldID={1} -- SceneID={2} \r\n" +
															   "SNOAnim={3} AnimState={4} \r\n" +
															   "Incapacitated={5} -- Rooted={6} \r\n" +
															   "Current Health={7} -- Current Energy={8}[{9}%] \r\n" +
															   "Current Coin={10} -- CurrentXP={11} \r\n" +
															   "Is In Boss Encounter={12}",
															   iCurrentLevelID, iCurrentWorldID, iSceneID,
															   Lastsnoanim.ToString(), lastAnimationState.ToString(),
															   Bot.Character.Data.bIsIncapacitated.ToString(), Bot.Character.Data.bIsRooted.ToString(),
															   dCurrentHealthPct, dCurrentEnergy, dCurrentEnergyPct,
															   Coinage, CurrentExp,
															   bIsInBossEncounter);

		}


		#endregion

		internal class Pets
		{
			public enum PetTypes
			{
				MONK_MysticAlly = 1,
				WITCHDOCTOR_Gargantuan = 2,
				WITCHDOCTOR_ZombieDogs = 4,
				DEMONHUNTER_Pet = 8,
				WIZARD_Hydra = 16,
				DEMONHUNTER_SpikeTrap = 32,
			}

			public Dictionary<PetTypes, int> dictPetCounter = new Dictionary<PetTypes, int>();

			public Pets()
			{
				Reset();
			}

			// A count for player mystic ally, gargantuans, and zombie dogs
			public int MysticAlly
			{
				get
				{
					if (!dictPetCounter.ContainsKey(PetTypes.MONK_MysticAlly))
						dictPetCounter.Add(PetTypes.MONK_MysticAlly, 0);

					return dictPetCounter[PetTypes.MONK_MysticAlly];
				}
				set
				{
					if (!dictPetCounter.ContainsKey(PetTypes.MONK_MysticAlly))
						dictPetCounter.Add(PetTypes.MONK_MysticAlly, value);
					else
						dictPetCounter[PetTypes.MONK_MysticAlly] = value;
				}
			}
			public int Gargantuan
			{
				get
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WITCHDOCTOR_Gargantuan))
						dictPetCounter.Add(PetTypes.WITCHDOCTOR_Gargantuan, 0);

					return dictPetCounter[PetTypes.WITCHDOCTOR_Gargantuan];
				}
				set
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WITCHDOCTOR_Gargantuan))
						dictPetCounter.Add(PetTypes.WITCHDOCTOR_Gargantuan, value);
					else
						dictPetCounter[PetTypes.WITCHDOCTOR_Gargantuan] = value;
				}
			}
			public int ZombieDogs
			{
				get
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WITCHDOCTOR_ZombieDogs))
						dictPetCounter.Add(PetTypes.WITCHDOCTOR_ZombieDogs, 0);

					return dictPetCounter[PetTypes.WITCHDOCTOR_ZombieDogs];
				}
				set
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WITCHDOCTOR_ZombieDogs))
						dictPetCounter.Add(PetTypes.WITCHDOCTOR_ZombieDogs, value);
					else
						dictPetCounter[PetTypes.WITCHDOCTOR_ZombieDogs] = value;
				}
			}
			public int DemonHunterPet
			{
				get
				{
					if (!dictPetCounter.ContainsKey(PetTypes.DEMONHUNTER_Pet))
						dictPetCounter.Add(PetTypes.DEMONHUNTER_Pet, 0);

					return dictPetCounter[PetTypes.DEMONHUNTER_Pet];
				}
				set
				{
					if (!dictPetCounter.ContainsKey(PetTypes.DEMONHUNTER_Pet))
						dictPetCounter.Add(PetTypes.DEMONHUNTER_Pet, value);
					else
						dictPetCounter[PetTypes.DEMONHUNTER_Pet] = value;
				}
			}
			public int DemonHunterSpikeTraps
			{
				get
				{
					if (!dictPetCounter.ContainsKey(PetTypes.DEMONHUNTER_SpikeTrap))
						dictPetCounter.Add(PetTypes.DEMONHUNTER_SpikeTrap, 0);

					return dictPetCounter[PetTypes.DEMONHUNTER_SpikeTrap];
				}
				set
				{
					if (!dictPetCounter.ContainsKey(PetTypes.DEMONHUNTER_SpikeTrap))
						dictPetCounter.Add(PetTypes.DEMONHUNTER_SpikeTrap, value);
					else
						dictPetCounter[PetTypes.DEMONHUNTER_SpikeTrap] = value;
				}
			}
			public int WizardHydra
			{
				get
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WIZARD_Hydra))
						dictPetCounter.Add(PetTypes.WIZARD_Hydra, 0);

					return dictPetCounter[PetTypes.WIZARD_Hydra];
				}
				set
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WIZARD_Hydra))
						dictPetCounter.Add(PetTypes.WIZARD_Hydra, value);
					else
						dictPetCounter[PetTypes.WIZARD_Hydra] = value;
				}
			}

			public void Reset()
			{
				MysticAlly = 0;
				Gargantuan = 0;
				ZombieDogs = 0;
				DemonHunterPet = 0;
				WizardHydra = 0;
				DemonHunterSpikeTraps = 0;
			}

		}
	}



}