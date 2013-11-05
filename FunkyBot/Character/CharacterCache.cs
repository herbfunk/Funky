using System;
using FunkyBot.Movement;
using FunkyBot.Targeting;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Common;
using Zeta.Internals.SNO;
using Zeta.CommonBot;

using FunkyBot.Cache;
using FunkyBot.Cache.Enums;

namespace FunkyBot.Character
{


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
						  iMyDynamicID=0;
						  iMyLevel=1;
						  iSceneID=-1;
						  iCurrentWorldID=-1;
						  iCurrentLevelID=-1;
						  BackPack=new Backpack();
						  PetData=new Pets();
						  PickupRadius=1;
						  coinage=0;
						  fCharacterRadius=0f;
						  CurrentExp = 0;
					 }

					 #region Events

					 public delegate void LevelAreaIDChanged(int ID);
					 public event LevelAreaIDChanged OnLevelAreaIDChanged;
					 private void levelareaIDchanged(int ID)
					 {
						 this.iCurrentLevelID = ID;
						 if (OnLevelAreaIDChanged != null)
							 this.OnLevelAreaIDChanged(ID);
					 }

					 public delegate void HealthValueChanged(double oldvalue, double newvalue);
					 public event HealthValueChanged OnHealthChanged;
					 private void healthvalueChanged(double oldvalue, double newvalue)
					 {
						 dCurrentHealthPct = newvalue;
						 if (OnHealthChanged != null)
							 this.OnHealthChanged(oldvalue, newvalue);
					 }
					 
					 #endregion

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
								coinage=value;
								UpdateCoinage=false;
						  }
					 }
					 internal bool UpdateCoinage { get; set; }

					 public bool ShouldFlee
					 {
						  get
						  {
								bool flee=Bot.Settings.Fleeing.EnableFleeingBehavior&&Bot.Character.dCurrentHealthPct<=Bot.Settings.Fleeing.FleeBotMinimumHealthPercent;
								return flee;
						  }
					 }

					 public int iCurrentWorldID { get; set; }
					 public int iCurrentLevelID { get; set; }


					 //Returns Live Data
					 private DateTime lastPositionUpdate=DateTime.Today;
					 private Vector3 lastPosition=Vector3.Zero;
					 public Vector3 Position
					 {
						  get
						  {
								//Because we don't want to update this X amount of times in a single loop!
								if (DateTime.Now.Subtract(lastPositionUpdate).TotalMilliseconds<150)
									 return lastPosition;

								lastPositionUpdate=DateTime.Now;
								try
								{
									 using (ZetaDia.Memory.AcquireFrame())
									 {
										  lastPosition=ZetaDia.Me.Position;
									 }
								} catch (NullReferenceException)
								{
									 lastPosition=Vector3.Zero;
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



					private AnimationState lastAnimationState=AnimationState.Invalid;
					internal AnimationState CurrentAnimationState
					{
						get
						{
							 try
							 {
								  using (ZetaDia.Memory.AcquireFrame())
								  {
										lastAnimationState=ZetaDia.Me.CommonData.AnimationState;
								  }
							 } catch (Exception)
							 {

							 }
							 return lastAnimationState;
						}
					}

					private SNOAnim Lastsnoanim=SNOAnim.a1dun_Cath_chest_idle;
					internal SNOAnim CurrentSNOAnim
					{
						get
						{

							try
							{
								using (ZetaDia.Memory.AcquireFrame())
								{
									Lastsnoanim=ZetaDia.Me.CommonData.CurrentAnimation;
								}
							}
							catch (Exception)
							{

							}
							return Lastsnoanim;
						}
					}



					 internal DateTime lastUpdateNonEssentialData=DateTime.Today;
					 internal DateTime lastCheckedSceneID=DateTime.Today;


					 internal void Update(bool combat=false, bool force=false)
					 {


						  double lastUpdate=DateTime.Now.Subtract(lastUpdatedPlayer).TotalMilliseconds;
						  //Update only every 100ms, unless in combat than 25ms..
						  if (!force&&
								(combat&&lastUpdate<50||lastUpdate<150)) return;

						  using (ZetaDia.Memory.AcquireFrame())
						  {
								// If we aren't in the game of a world is loading, don't do anything yet
								if (!ZetaDia.IsInGame||!ZetaDia.Me.IsValid||ZetaDia.IsLoadingWorld) return;

								var me=ZetaDia.Me;
								if (me==null) return;

								try
								{
									 if (Bot.Class.AC==ActorClass.DemonHunter)
									 {
										  dDiscipline=me.CurrentSecondaryResource;
										  dDisciplinePct=Bot.Character.dDiscipline/me.MaxSecondaryResource;
									 }

									 double curhealthpct=me.HitpointsCurrentPct;
									 if (!this.dcurrentHealthPct.Equals(curhealthpct))
										  healthvalueChanged(dcurrentHealthPct, curhealthpct);

									 dCurrentEnergy=me.CurrentPrimaryResource;
									 dCurrentEnergyPct=dCurrentEnergy/me.MaxPrimaryResource;

									 //Critical Avoidance (when no avoidance is set!)
									 if (dCurrentHealthPct<0.50d&&!Bot.Settings.Avoidance.AttemptAvoidanceMovements&&
										  !Zeta.CommonBot.PowerManager.CanCast(SNOPower.DrinkHealthPotion))
										  this.CriticalAvoidance=true;
									 else if (this.CriticalAvoidance&&!Funky.shouldPreformOOCItemIDing&&!Funky.FunkyTPBehaviorFlag&&dCurrentHealthPct>0.5)
										  //Disable it when not OOC/TP/Low health still..
										  this.CriticalAvoidance=false;

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
										  {
												if (Bot.Class.KnockbackLandAnims.Contains(this.CurrentSNOAnim))
													 bIsIncapacitated=true;
												else
													 bIsIncapacitated=false;
										  }
									 }

									 int currentLevelAreaID=ZetaDia.CurrentLevelAreaId;
									 if (this.iCurrentLevelID!=currentLevelAreaID)
									 {
										  levelareaIDchanged(currentLevelAreaID);
										  iCurrentWorldID=ZetaDia.CurrentWorldDynamicId;
									 }

									 if (Bot.Game.CurrentLevel == 60)
										CurrentExp = me.ParagonCurrentExperience;
									else
										CurrentExp = me.CurrentExperience;

									 //Set Character Radius?
									 if (this.fCharacterRadius == 0f)
									 {
										 this.fCharacterRadius = me.ActorInfo.Sphere.Radius;

										 //Wizards are short -- causing issues (At least Male Wizard is!)
										 //if (Bot.Game.ActorClass == ActorClass.Wizard) this.fCharacterRadius += 1f;
									 }
									 

									 //Update vars that are not essential to combat (survival).
									 if (DateTime.Now.Subtract(lastUpdateNonEssentialData).TotalSeconds>30)
									 {
										  lastUpdateNonEssentialData=DateTime.Now;

										  //update level if not 60 else update paragonlevel
										  if (iMyLevel<60) iMyLevel=me.Level;

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
										  }
										  lastCheckedSceneID=DateTime.Now;
									 }
								} catch (AccessViolationException)
								{

								}
						  }
						  lastUpdatedPlayer=DateTime.Now;
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
									 AnimationState currentanimstate=CurrentAnimationState;
									 if (currentanimstate.HasFlag(AnimationState.Casting|AnimationState.Channeling))
										  bIsAnimating=true;
									 if (bWaitForAttacking&&(currentanimstate.HasFlag(AnimationState.Attacking)))
										  bIsAnimating=true;
								} catch (NullReferenceException)
								{
									 bIsAnimating=true;
								}
								if (!bIsAnimating)
									 bKeepLooping=false;
						  }
					 }
					/*
					 * 						  lastUpdatedPlayer=DateTime.Today;
						  lastPreformedNonCombatAction=DateTime.Today;
						  bIsIncapacitated=false;
						  bIsRooted=false;
						  bIsInTown=false;
						  dcurrentHealthPct=0d;
						  dCurrentEnergy=0d;
						  dCurrentEnergyPct=0d;
						  dDiscipline=0d;
						  dDisciplinePct=0d;
						  iMyDynamicID=0;
						  iMyLevel=1;
						  BackPack=new Backpack();
						  PetData=new Pets();
						  PickupRadius=1;
						  fCharacterRadius=0f;
					 */
					 public string DebugString()
					 {
						  return String.Format("Character Info \r\n"+
																			 "CurrentLevelID={0} -- WorldID={1} -- SceneID={2} \r\n" +
																			 "SNOAnim={3} AnimState={4} \r\n" +
																			 "Incapacitated={5} -- Rooted={6} \r\n" +
																			 "Current Health={7} -- Current Energy={8}[{9}%] \r\n"+
																		     "Current Coin={10} -- CurrentXP={11}",
																			 this.iCurrentLevelID, this.iCurrentWorldID, this.iSceneID,
																			 this.Lastsnoanim.ToString(), this.lastAnimationState.ToString(),
																			 Bot.Character.bIsIncapacitated.ToString(), Bot.Character.bIsRooted.ToString(),
																			 this.dCurrentHealthPct,this.dCurrentEnergy,this.dCurrentEnergyPct,
																			 this.Coinage,this.CurrentExp);

					 }
				}
}