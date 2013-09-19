using System;
using FunkyTrinity.Targeting;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Common;
using Zeta.Internals.SNO;
using Zeta.CommonBot;

using FunkyTrinity.Cache;
using FunkyTrinity.Movement;
using FunkyTrinity.Cache.Enums;

namespace FunkyTrinity.Cache
{
	 public enum PetTypes
	 {
		  MONK_MysticAlly=1,
		  WITCHDOCTOR_Gargantuan=2,
		  WITCHDOCTOR_ZombieDogs=4,
		  DEMONHUNTER_Pet=8,
		  WIZARD_Hydra=16,
	 }

				///<summary>
				///Cache of all values Character related and variable.
				///</summary>
				public class CharacterCache
				{
					 public CharacterCache()
					 {
						  lastUpdatedPlayer=DateTime.Today;
						  lastPreformedNonCombatAction=DateTime.Today;
						  bIsIncapacitated=false;
						  bIsRooted=false;
						  bIsInTown=false;
						  dcurrentHealthPct=0d;
						  dCurrentEnergy=0d;
						  dCurrentEnergyPct=0d;
						  dDiscipline=0d;
						  dDisciplinePct=0d;
						  //EnergyRegenerationRate=0;
						  bWaitingForReserveEnergy=false;
						  iMyDynamicID=0;
						  iMyLevel=1;
						  iMyParagonLevel=0;
						  iSceneID=-1;
						  iCurrentWorldID=-1;
						  BackPack=new Backpack();
						  PetData=new Pets();
						  PickupRadius=1;
						  coinage=0;
						  fCharacterRadius=0f;
						  ShouldBackTrack=false;
						  BackTrackVector=Vector3.Zero;
					 }

					 private DateTime lastUpdatedPlayer { get; set; }
					 internal DateTime lastPreformedNonCombatAction { get; set; }

					 public bool bIsIncapacitated { get; set; }
					 public bool bIsRooted { get; set; }
					 public bool bIsInTown { get; set; }

					 private double dcurrentHealthPct;
					 public double dCurrentHealthPct
					 {
						  get { return dcurrentHealthPct; }
						  set 
						  { 
								dcurrentHealthPct=value;
								if (Bot.SettingsFunky.StopGameOnBotLowHealth&&value<=Bot.SettingsFunky.StopGameOnBotHealthPercent)
								{
									 Bot.ShuttingDownBot=true;
								}
						  }
					 }
					 public double dCurrentEnergy { get; set; }
					 public double dCurrentEnergyPct { get; set; }
					 public double dDiscipline { get; set; }
					 public double dDisciplinePct { get; set; }

					 private int coinage;
					 public int Coinage 
					 { 
						  get
						  {
								return coinage;
						  }
						  set
						  {
								coinage=value;
								UpdateCoinage=false;
						  }
					 }
					 internal bool UpdateCoinage { get; set; }

					 public bool ShouldFlee
					 {
						  get
						  {
								bool flee=Bot.SettingsFunky.Fleeing.EnableFleeingBehavior&&Bot.Character.dCurrentHealthPct<=Bot.SettingsFunky.Fleeing.FleeBotMinimumHealthPercent;
								return flee;
						  }
					 }

					 //internal int EnergyRegenerationRate { get; set; }
					 
					 public int iCurrentWorldID { get; set; }
					 //public GameDifficulty iCurrentGameDifficulty { get; set; }

					internal bool ShouldBackTrack { get; set; }
					 internal Vector3 BackTrackVector { get; set; }

					 //Returns Live Data
					 private DateTime lastPositionUpdate=DateTime.Today;
					 private Vector3 lastPosition=Vector3.Zero;
					 public Vector3 Position
					 {
						  get
						  {
								//Because we don't want to update this X amount of times in a single loop!
								if (DateTime.Now.Subtract(lastPositionUpdate).TotalMilliseconds>250)
								{
									 try
									 {
										  lastPosition=ZetaDia.Me.Position;
										  lastPositionUpdate=DateTime.Now;
									 } catch (NullReferenceException)
									 {
										  lastPosition=Vector3.Zero;
									 }


								}

								return lastPosition;
						  }
					 }
					 internal GridPoint PointPosition
					 {
						  get
						  {
								return Position;
						  }
					 }

					 //Male Wizard: Radius 5.633342
					 //Female Demonhunter: Radius 6.437767
					 internal float fCharacterRadius { get; set; }


					 internal Sphere CharacterSphere
					 {
						  get
						  {
								return new Sphere(Position, fCharacterRadius);
						  }
					 }


					 internal bool bWaitingForReserveEnergy { get; set; }
					 internal int iMyDynamicID { get; set; }
					 internal int iMyLevel { get; set; }
					 internal int iMyParagonLevel { get; set; }
					 internal int iTotalPotions { get; set; }
					 internal int iSceneID { get; set; }
					 internal Pets PetData { get; set; }
					 internal Backpack BackPack { get; set; }
					 internal float PickupRadius { get; set; }
					 internal int FreeBackpackSlots { get; set; }

					internal AnimationState CurrentAnimationState { get; set; }
					 internal SNOAnim CurrentSNOAnim { get; set; }



					 internal DateTime lastUpdateNonEssentialData=DateTime.Today;
					 internal DateTime lastCheckedSceneID=DateTime.Today;


					 internal void Update(bool combat=false, bool force=false)
					 {
						  // If we aren't in the game of a world is loading, don't do anything yet
						  if (!ZetaDia.IsInGame||ZetaDia.IsLoadingWorld)
								return;

						  var me=ZetaDia.Me;
						  if (me==null)
								return;

						  double lastUpdate=DateTime.Now.Subtract(lastUpdatedPlayer).TotalMilliseconds;
						  //Update only every 100ms, unless in combat than 25ms..
						  if (!force&&
								(combat&&lastUpdate<50||lastUpdate<150)) return;

						  using (ZetaDia.Memory.AcquireFrame())
						  {
								try
								{
									 if (Bot.Class.AC==ActorClass.DemonHunter)
									 {
										  dDiscipline=me.CurrentSecondaryResource;
										  dDisciplinePct=Bot.Character.dDiscipline/me.MaxSecondaryResource;
									 }

									 //vCurrentPosition=me.Position;
									 dCurrentHealthPct=me.HitpointsCurrentPct;
									 if (this.ShouldFlee) Bot.Combat.RequiresAvoidance=true;

									 dCurrentEnergy=me.CurrentPrimaryResource;
									 dCurrentEnergyPct=dCurrentEnergy/me.MaxPrimaryResource;
									 //EnergyRegenerationRate=me.CommonData.GetAttribute<int>(ActorAttributeType.ResourceRegenPerSecond);

									 if (dCurrentEnergy>=Bot.Class.iWaitingReservedAmount)
										  bWaitingForReserveEnergy=false;
									 if (dCurrentEnergy<20)
										  bWaitingForReserveEnergy=true;

									 //Critical Avoidance (when no avoidance is set!)
									 if (dCurrentHealthPct<0.50d&&!Bot.SettingsFunky.Avoidance.AttemptAvoidanceMovements&&
										  !Zeta.CommonBot.PowerManager.CanCast(SNOPower.DrinkHealthPotion))
										  Bot.Combat.CriticalAvoidance=true;
									 else if (Bot.Combat.CriticalAvoidance&&!Funky.shouldPreformOOCItemIDing&&!Funky.FunkyTPBehaviorFlag&&dCurrentHealthPct>0.5)
										  //Disable it when not OOC/TP/Low health still..
										  Bot.Combat.CriticalAvoidance=false;

									 bIsInTown=me.IsInTown;
									 bIsRooted=me.IsRooted;
									
									 if (me.IsFeared||me.IsStunned||me.IsFrozen||me.IsBlind)
										  bIsIncapacitated=true;
									 else
									 {
										  bool bIsInKnockBack=(me.CommonData.GetAttribute<int>(ActorAttributeType.InKnockback)!=0);
										  if (bIsInKnockBack)
												bIsIncapacitated=true;
										  else
												bIsIncapacitated=false;
									 }
									
									 //Update vars that are not essential to combat (survival).
									 if (DateTime.Now.Subtract(lastUpdateNonEssentialData).TotalSeconds>30)
									 {
										  lastUpdateNonEssentialData=DateTime.Now;

										  //update level if not 60 else update paragonlevel
										  if (iMyLevel<60) iMyLevel=me.Level;
										  else iMyParagonLevel=me.ParagonLevel;

										  iMyDynamicID=me.CommonData.DynamicId;
										  FreeBackpackSlots=me.Inventory.NumFreeBackpackSlots;
										  PickupRadius=me.GoldPickupRadius;
										  Coinage=me.Inventory.Coinage;
										  //Clear our BPItems list..
										  BackPack.BPItems.Clear();
									 }

									 if (UpdateCoinage)
										  Coinage=me.Inventory.Coinage;

									 //Check current scence every 1.5 seconds
									 if (!bIsInTown&&DateTime.Now.Subtract(lastCheckedSceneID).TotalSeconds>1.50)
									 {
										  //Get the current guid, compare/update.
										  int CurrentSceneID=me.CurrentScene.SceneGuid;
										  if (CurrentSceneID!=Bot.Character.iSceneID)
										  {
												Bot.Character.iSceneID=CurrentSceneID;
												//UpdateSearchGridProvider(true);
										  }
										  lastCheckedSceneID=DateTime.Now;
									 }
								} catch (AccessViolationException)
								{

								}
						  }
						  lastUpdatedPlayer=DateTime.Now;
					 }



					 //private DateTime LastUpdatedAnimationData=DateTime.Today;
					 internal void UpdateAnimationState(bool animState=true, bool snoAnim=true)
					 {
						  // If we aren't in the game of a world is loading, don't do anything yet
						  if (!ZetaDia.IsInGame||ZetaDia.IsLoadingWorld)//||DateTime.Now.Subtract(LastUpdatedAnimationData).TotalMilliseconds<150)
								return;

						  //LastUpdatedAnimationData=DateTime.Now;
						  using (ZetaDia.Memory.AcquireFrame())
						  {
								try
								{
									 if (animState)
										  CurrentAnimationState=ZetaDia.Me.CommonData.AnimationState;

									 if (snoAnim)
										  CurrentSNOAnim=ZetaDia.Me.CommonData.CurrentAnimation;
								} catch (Exception)
								{
								}

						  }
					 }

					 // **********************************************************************************************
					 // *****      Quick and Dirty routine just to force a wait until the character is "free"    *****
					 // **********************************************************************************************
					 public void WaitWhileAnimating(int iMaxSafetyLoops=10, bool bWaitForAttacking=false)
					 {
						  bool bKeepLooping=true;
						  int iSafetyLoops=0;
						  while (bKeepLooping)
						  {
								iSafetyLoops++;
								if (iSafetyLoops>iMaxSafetyLoops)
									 bKeepLooping=false;
								bool bIsAnimating=false;
								try
								{
									 UpdateAnimationState();
									 if (CurrentAnimationState.HasFlag(AnimationState.Casting|AnimationState.Channeling))
										  bIsAnimating=true;
									 if (bWaitForAttacking&&(CurrentAnimationState.HasFlag(AnimationState.Attacking)))
										  bIsAnimating=true;
								} catch (NullReferenceException)
								{
									 bIsAnimating=true;
								}
								if (!bIsAnimating)
									 bKeepLooping=false;
						  }
					 }
				}
}