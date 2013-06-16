using System;
using Zeta;
using Zeta.Internals.SNO;
using Zeta.Internals.Actors;
using System.Collections.Generic;
using Zeta.Common;
using Zeta.Internals.Actors.Gizmos;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  #region SNO Cache Dictionaries
		  private static Dictionary<int, ActorType?> dictActorType=new Dictionary<int, ActorType?>();
		  private static Dictionary<int, TargetType?> dictTargetType=new Dictionary<int, TargetType?>();
		  private static Dictionary<int, MonsterSize?> dictMonstersize=new Dictionary<int, MonsterSize?>();
		  private static Dictionary<int, MonsterType?> dictMonstertype=new Dictionary<int, MonsterType?>();
		  private static Dictionary<int, float?> dictCollisionRadius=new Dictionary<int, float?>();
		  private static Dictionary<int, String> dictInternalName=new Dictionary<int, String>();
		  private static Dictionary<int, ObstacleType?> dictObstacleType=new Dictionary<int, ObstacleType?>();
		  private static Dictionary<int, float?> dictActorSphereRadius=new Dictionary<int, float?>();
		  private static Dictionary<int, bool?> dictCanBurrow=new Dictionary<int, bool?>();
		  private static Dictionary<int, bool?> dictGrantsNoXp=new Dictionary<int, bool?>();
		  private static Dictionary<int, bool?> dictDropsNoLoot=new Dictionary<int, bool?>();
		  private static Dictionary<int, GizmoType?> dictGizmoType=new Dictionary<int, GizmoType?>();
		  private static Dictionary<int, bool?> dictIsBarricade=new Dictionary<int, bool?>();
		  private static Dictionary<int, double> dictProjectileSpeed=new Dictionary<int, double>();
		  #endregion


		  public abstract class SNO
		  {
				#region Constructors
				public SNO(int sno, bool Null=false)
				{
					 SNOID=sno;

					 //create cache?
					 if (!Null)
						  ObjectCache.cacheSnoCollection.Add(sno);
				}
				public SNO(int sno)
				{
					 this.SNOID=sno;

					 CachedSNOEntry thisEntry=ObjectCache.cacheSnoCollection[sno];

					 if (sno>0)
					 {
						  this.SNOID=thisEntry.SNOID;
						  this._actortype=thisEntry.Actortype;
						  this._targettype=thisEntry.targetType;
						  this._monstersize=thisEntry.Monstersize;
						  this._monstertype=thisEntry.Monstertype;
						  this._collisionradius=thisEntry.CollisionRadius;
						  this._actorsphereradius=thisEntry.ActorSphereRadius;
						  this._CanBurrow=thisEntry.CanBurrow;
						  this._GrantsNoXP=thisEntry.GrantsNoXP;
						  this._DropsNoLoot=thisEntry.DropsNoLoot;
						  this._IsBarricade=thisEntry.IsBarricade;
						  this._internalname=thisEntry.InternalName;
						  this._obstacletype=thisEntry.Obstacletype;
						  this._gizmotype=thisEntry.Gizmotype;
						  //this._RunningRate=thisEntry.RunningRate;
						  this.IsFinalized=thisEntry.IsFinalized;
					 }
				}
				public SNO(int sno, String internalname, ActorType? actortype=null, TargetType? targettype=null, MonsterType? monstertype=null, MonsterSize? monstersize=null, float? collisionradius=null, bool? canburrow=null, bool? grantsnoxp=null, bool? dropsnoloot=null, bool? isbarricade=null, ObstacleType? obstacletype=null, float? actorsphereradius=null, GizmoType? gimzotype=null)
				{
					 //Creates the perm data
					 SNOID=sno;
					 _actortype=actortype;
					 _targettype=targettype;
					 _collisionradius=collisionradius;
					 _monstersize=monstersize;
					 _monstertype=monstertype;
					 _internalname=internalname;
					 _CanBurrow=canburrow;
					 _DropsNoLoot=dropsnoloot;
					 _GrantsNoXP=grantsnoxp;
					 _IsBarricade=isbarricade;
					 _obstacletype=obstacletype;
					 _actorsphereradius=actorsphereradius;
					 _gizmotype=gimzotype;
					 //_RunningRate=runningrate;
					 IsFinalized=true;
				}
				public SNO(SNO sno)
				{
					 this.SNOID=sno.SNOID;
					 this._actortype=sno.Actortype;
					 this._targettype=sno.targetType;
					 this._monstersize=sno.Monstersize;
					 this._monstertype=sno.Monstertype;
					 this._collisionradius=sno.CollisionRadius;
					 this._actorsphereradius=sno.ActorSphereRadius;
					 this._CanBurrow=sno.CanBurrow;
					 this._GrantsNoXP=sno.GrantsNoXP;
					 this._DropsNoLoot=sno.DropsNoLoot;
					 this._IsBarricade=sno.IsBarricade;
					 this._internalname=sno.InternalName;
					 this._obstacletype=sno.Obstacletype;
					 this._gizmotype=sno.Gizmotype;
					 //this._RunningRate=sno.RunningRate;
					 this.IsFinalized=sno.IsFinalized;
				}
				#endregion


				public readonly bool IsFinalized=false;
				public readonly int SNOID;
				public DateTime LastUsed=DateTime.Now;

				#region SNO Properties
				private bool? _CanBurrow;
				public bool? CanBurrow
				{
					 get
					 {
						  if (this.IsFinalized)
								return this._CanBurrow;

						  if (dictCanBurrow.ContainsKey(SNOID))
								return dictCanBurrow[SNOID];
						  else
								return null;
					 }
					 set
					 {
						  if (!this.IsFinalized)
								dictCanBurrow[SNOID]=value;
						  else
								_CanBurrow=value;
					 }
				}

				private readonly bool? _IsBarricade;
				public bool? IsBarricade
				{
					 get
					 {
						  if (this.IsFinalized)
								return this._IsBarricade;

						  if (dictIsBarricade.ContainsKey(SNOID))
								return dictIsBarricade[SNOID];
						  else
								return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
								return;

						  dictIsBarricade[SNOID]=value;
					 }
				}

				private readonly bool? _DropsNoLoot;
				public bool? DropsNoLoot
				{
					 get
					 {
						  if (this.IsFinalized)
								return this._DropsNoLoot;

						  if (dictDropsNoLoot.ContainsKey(SNOID))
								return dictDropsNoLoot[SNOID];
						  else
								return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
								return;

						  dictDropsNoLoot[SNOID]=value;
					 }
				}

				private readonly bool? _GrantsNoXP;
				public bool? GrantsNoXP
				{
					 get
					 {
						  if (this.IsFinalized)
								return this._GrantsNoXP;

						  if (dictGrantsNoXp.ContainsKey(SNOID))
								return dictGrantsNoXp[SNOID];
						  else
								return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
								return;

						  dictGrantsNoXp[SNOID]=value;
					 }
				}

				private readonly GizmoType? _gizmotype;
				public GizmoType? Gizmotype
				{
					 get
					 {
						  if (this.IsFinalized)
								return this._gizmotype;

						  if (dictGizmoType.ContainsKey(SNOID))
								return dictGizmoType[SNOID];
						  else
								return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
								return;

						  dictGizmoType[SNOID]=value;
					 }
				}

				private readonly float? _actorsphereradius;
				public float? ActorSphereRadius
				{
					 get
					 {
						  if (this.IsFinalized)
								return this._actorsphereradius;

						  if (dictActorSphereRadius.ContainsKey(SNOID))
								return dictActorSphereRadius[SNOID];
						  else
								return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
								return;

						  dictActorSphereRadius[SNOID]=value;
					 }
				}

				private readonly ObstacleType? _obstacletype;
				public ObstacleType? Obstacletype
				{
					 get
					 {
						  if (this.IsFinalized)
								return this._obstacletype;

						  if (dictObstacleType.ContainsKey(SNOID))
								return dictObstacleType[SNOID];
						  else
								return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
								return;

						  dictObstacleType[SNOID]=value;
					 }
				}

				private readonly ActorType? _actortype;
				public ActorType? Actortype
				{
					 get
					 {
						  if (this.IsFinalized) return this._actortype;

						  if (dictActorType.ContainsKey(SNOID)) return dictActorType[SNOID]; else return null;
					 }
					 set
					 {
						  if (this.IsFinalized) return;

						  dictActorType[SNOID]=value;
					 }
				}

				private TargetType? _targettype;
				public TargetType? targetType
				{
					 get
					 {
						  if (this.IsFinalized) return this._targettype;

						  if (dictTargetType.ContainsKey(SNOID)) return dictTargetType[SNOID]; else return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
						  {
								_targettype=value;
								return;
						  }

						  dictTargetType[SNOID]=value;
					 }
				}

				private readonly float? _collisionradius;
				public float? CollisionRadius
				{
					 get
					 {
						  if (this.IsFinalized) return this._collisionradius;

						  if (dictCollisionRadius.ContainsKey(SNOID)) return dictCollisionRadius[SNOID]; else return null;
					 }
					 set
					 {
						  if (this.IsFinalized) return;

						  dictCollisionRadius[SNOID]=(float)value;

					 }
				}

				private MonsterType? _monstertype;
				public MonsterType? Monstertype
				{
					 get
					 {
						  if (this.IsFinalized) return this._monstertype;

						  if (dictMonstertype.ContainsKey(SNOID)) return dictMonstertype[SNOID]; else return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
						  {
								this._monstertype=value;
								return;
						  }

						  dictMonstertype[SNOID]=value;
					 }
				}

				private readonly MonsterSize? _monstersize;
				public MonsterSize? Monstersize
				{
					 get
					 {
						  if (this.IsFinalized) return this._monstersize;

						  if (dictMonstersize.ContainsKey(SNOID)) return dictMonstersize[SNOID]; else return null;
					 }
					 set
					 {
						  if (this.IsFinalized) return;

						  dictMonstersize[SNOID]=value;

					 }
				}

				private readonly string _internalname;
				public string InternalName
				{
					 get
					 {
						  if (this.IsFinalized) return this._internalname;


						  if (dictInternalName.ContainsKey(SNOID)) return dictInternalName[SNOID]; else return null;
					 }
					 set
					 {
						  if (this.IsFinalized) return;

						  dictInternalName[SNOID]=value;
					 }
				}
				#endregion

				private string internalnamelower;
				private string internalNameLower
				{
					 get
					 {
						  if (internalnamelower==null)
								internalnamelower=InternalName.ToLower();

						  return internalnamelower;
					 }
				}




				public virtual string DebugString
				{
					 get
					 {
						  string debugstring="SNO: "+this.SNOID+"("+InternalName+")\r\n";
						  debugstring+=Actortype.HasValue?"ActorType: "+Actortype.Value.ToString()+" ":"";
						  debugstring+=targetType.HasValue?"TargetType: "+targetType.Value.ToString()+" "+"\r\n":""+"\r\n";

						  debugstring+=CollisionRadius.HasValue?"CollisionRadius: "+CollisionRadius.Value.ToString()+" ":"";
						  debugstring+=ActorSphereRadius.HasValue?"ActorSphereRadius: "+ActorSphereRadius.Value.ToString()+" "+"\r\n":""+"\r\n";

						  debugstring+=Monstertype.HasValue?"Monstertype: "+Monstertype.Value.ToString()+" ":"";
						  debugstring+=Monstersize.HasValue?"Monstersize: "+Monstersize.Value.ToString()+" "+"\r\n":"";
						  //debugstring+=RunningRate.HasValue?"RunningRate: "+RunningRate.Value.ToString()+" "+"\r\n":"";

						  debugstring+=GrantsNoXP.HasValue?"GrantsNoXP: "+GrantsNoXP.Value.ToString()+" ":"";
						  debugstring+=DropsNoLoot.HasValue?"DropsNoLoot: "+DropsNoLoot.Value.ToString()+" ":"";
						  debugstring+=IsBarricade.HasValue?"IsBarricade: "+IsBarricade.Value.ToString()+" "+"\r\n":"";

						  return debugstring;

					 }
				}


				#region Cache Lookup Properties
				public bool IsObstacle { get { return hashSNONavigationObstacles.Contains(SNOID); } }
				public bool IsHealthWell { get { return SNOID==138989; } }
				public bool IsTreasureGoblin { get { return SnoCacheLookup.hashActorSNOGoblins.Contains(SNOID); } }
				public bool IsBoss { get { return SnoCacheLookup.hashBossSNO.Contains(SNOID); } }
				public bool IsWormBoss { get { return (SNOID==218947||SNOID==144400); } }
				public bool IsResplendantChest { get { return SnoCacheLookup.hashSNOContainerResplendant.Contains(SNOID); } }
				public bool IsAvoidance { get { return hashAvoidanceSNOList.Contains(SNOID); } }
				public bool IsSummonedPet { get { return SnoCacheLookup.hashSummonedPets.Contains(SNOID); } }
				public bool IsRespawnable { get { return SnoCacheLookup.hashActorSNOSummonedUnit.Contains(SNOID); } }
				public bool IsProjectileAvoidance { get { return hashAvoidanceSNOProjectiles.Contains(SNOID); } }
				//public bool IsCorpseContainer { get { return (this.internalNameLower.Contains("loottype")||this.internalNameLower.Contains("corpse")); } }
				public bool IsChestContainer { get { return (this.internalNameLower.Contains("chest")); } }
				public bool IgnoresLOSCheck { get { return SnoCacheLookup.hashActorSNOIgnoreLOSCheck.Contains(SNOID); } }
				public bool IsMissileReflecting { get { return SnoCacheLookup.hashActorSNOReflectiveMissleUnits.Contains(SNOID); } }
				public bool IsStealthableUnit { get { return SnoCacheLookup.hashActorSNOStealthUnits.Contains(SNOID); } }
				public bool IsBurrowableUnit { get { return SnoCacheLookup.hashActorSNOBurrowableUnits.Contains(SNOID); } }
				public bool IsSucideBomber { get { return SnoCacheLookup.hashActorSNOSucideBomberUnits.Contains(SNOID); } }
				public bool IsGrotesqueActor { get { return SnoCacheLookup.hashActorSNOCorpulent.Contains(SNOID); } }
				#endregion

				public bool ContainsNullValues()
				{
					 if (this.IsFinalized||this.SNOID==0) return false;

					 if (!this.targetType.HasValue||!this.Actortype.HasValue||this.InternalName==null||!this.Obstacletype.HasValue)
						  return true;

					 if (this.targetType.Value==TargetType.Unit)
					 {
						  if (!this.Monstertype.HasValue||!this.Monstersize.HasValue) return true; //||!this.RunningRate.HasValue
					 }

					 if (this.targetType.Value!=TargetType.Item&&this.targetType.Value!=TargetType.Avoidance)
					 {
						  if (!this.CollisionRadius.HasValue||!this.ActorSphereRadius.HasValue) return true;

						  if (this.targetType.Value==TargetType.Destructible||this.targetType.Value==TargetType.Barricade||this.targetType.Value==TargetType.Interactable)
						  {
								if (!this.DropsNoLoot.HasValue||!this.GrantsNoXP.HasValue||!this.IsBarricade.HasValue||!this.Gizmotype.HasValue) return true;
						  }
					 }
					 return false;
				}

				public virtual object Clone()
				{
					 return this.MemberwiseClone();
				}
		  }

		  ///<summary>
		  ///Caches all SNO related data.
		  ///</summary>
		  public class CachedSNOEntry : SNO
		  {

				public CachedSNOEntry(int sno, String internalname, ActorType? actortype=null, TargetType? targettype=null, MonsterType? monstertype=null, MonsterSize? monstersize=null, float? collisionradius=null, bool? canburrow=null, bool? grantsnoxp=null, bool? dropsnoloot=null, bool? isbarricade=null, ObstacleType? obstacletype=null, float? actorsphereradius=null, GizmoType? gizmotype=null)
					 : base(sno, internalname, actortype, targettype, monstertype, monstersize, collisionradius, canburrow, grantsnoxp, dropsnoloot, isbarricade, obstacletype, actorsphereradius, gizmotype)
				{
				}

				public CachedSNOEntry(int sno, bool Null=false)
					 : base(sno, Null)
				{
				}

				public CachedSNOEntry(CachedSNOEntry parent)
					 : base(parent)
				{
				}

				public bool ShouldRefreshMonsterType
				{
					 get
					 {
						  if (!this.Monstertype.HasValue)
								return true;
						  else
								return ((this.Monstertype==MonsterType.Ally||this.Monstertype==MonsterType.Scenery||
									 this.Monstertype==MonsterType.Helper||this.Monstertype==MonsterType.Team));
					 }
				}
				public bool MonsterTypeIsHostile()
				{
					 switch (this.Monstertype.Value)
					 {
						  case MonsterType.Ally:
						  case MonsterType.Scenery:
						  case MonsterType.Helper:
						  case MonsterType.Team:
								return false;
					 }

					 return true;
				}


				///<summary>
				///Updates SNO Cache Values
				///</summary>
				public virtual bool UpdateData(DiaObject thisObj, int raguid)
				{
					 bool failureDuringUpdate=false;

					 if (this.InternalName==null)
					 {
						  try
						  {
								this.InternalName=thisObj.Name;
						  } catch (NullReferenceException) { Logging.WriteVerbose("Failure to get internal name on object, SNO {0}", this.SNOID); return false; }
					 }

					 if (!this.Actortype.HasValue)
					 {
						  #region ActorType
						  try
						  {
								this.Actortype=thisObj.ActorType;
						  } catch (NullReferenceException ) { Logging.WriteVerbose("Failure to get actorType for object, SNO: {0}", this.SNOID); return false; }
						  #endregion
					 }

					 //Ignored actor types..
					 if (IgnoredActorTypes.Contains(this.Actortype.Value))//||!LootBehaviorEnabled&&this.Actortype.Value==ActorType.Item)
					 {
						  IgnoreThisObject(this, raguid, true, true);
						  return false;
					 }

					 if (!this.targetType.HasValue)
					 {
						  #region EvaluateTargetType
						  try
						  {
								//Evaluate Target Type..
								// See if it's an avoidance first from the SNO
								if (this.IsAvoidance||this.IsObstacle)
								{
									 this.targetType=TargetType.None;

									 if (this.IsAvoidance)
									 {
										  if (this.IsProjectileAvoidance)
												this.Obstacletype=ObstacleType.MovingAvoidance;
										  else
												this.Obstacletype=ObstacleType.StaticAvoidance;

										  AvoidanceType AT=FindAvoidanceUsingSNOID(this.SNOID);

										  //Check if avoidance is enabled or if the avoidance type is set to 0
										  if (!Bot.SettingsFunky.AttemptAvoidanceMovements||AT!=AvoidanceType.Unknown&&Bot.IgnoringAvoidanceType(AT))
										  {
												AddObjectToBlacklist(raguid, BlacklistType.Temporary);
												return false;
										  }

										  // Avoidance isn't disabled, so set this object type to avoidance
										  this.targetType=TargetType.Avoidance;
									 }
									 else
										  this.Obstacletype=ObstacleType.ServerObject;
								}
								else
								{
									 // Calculate the object type of this object
									 if (this.Actortype.Value==ActorType.Unit||
										  SnoCacheLookup.hashActorSNOForceTargetUnit.Contains(this.SNOID))
									 {
										  this.targetType=TargetType.Unit;
										  this.Obstacletype=ObstacleType.Monster;

										  if (SnoCacheLookup.hashActorSNOForceTargetUnit.Contains(this.SNOID))
										  {
												//Fill in monster data?
												this.Actortype=ActorType.Unit;
										  }
									 }
									 else if (this.Actortype.Value==ActorType.Item||
										  SnoCacheLookup.hashForceSNOToItemList.Contains(this.SNOID))
									 {
										  string testname=this.InternalName.ToLower();
										  //Check if this item is gold/globe..
										  if (testname.StartsWith("gold"))
												this.targetType=TargetType.Gold;
										  else if (testname.StartsWith("healthglobe"))
												this.targetType=TargetType.Globe;
										  else
												this.targetType=TargetType.Item;
										  //Gold/Globe?
									 }
									 else if (this.Actortype.Value==ActorType.Gizmo)
									 {

										  GizmoType thisGizmoType=GizmoType.None;
										  try
										  {
												thisGizmoType=thisObj.ActorInfo.GizmoType;
										  } catch (NullReferenceException ) { Logging.WriteVerbose("Failure to get actor Gizmo Type!"); return false; }


										  if (thisGizmoType==GizmoType.DestructibleLootContainer)
												this.targetType=TargetType.Destructible;
										  else if (thisGizmoType==GizmoType.Shrine||thisGizmoType==GizmoType.Healthwell)
										  {
												this.targetType=TargetType.Shrine;
										  }
										  else if (thisGizmoType==GizmoType.LootContainer)
												this.targetType=TargetType.Container;
										  else if (thisGizmoType==GizmoType.Destructible||thisGizmoType==GizmoType.Barricade)
												this.targetType=TargetType.Barricade;
										  else if (thisGizmoType==GizmoType.Door)
												this.targetType=TargetType.Door;
										  else
										  {//All other gizmos should be ignored!
												IgnoreThisObject(this, raguid, true, true);
												return false;
										  }

										  if (this.targetType.HasValue)
										  {
												if (this.targetType.Value==TargetType.Destructible||this.targetType.Value==TargetType.Barricade||this.targetType.Value==TargetType.Door)
													 this.Obstacletype=ObstacleType.Destructable;
												else if (this.targetType.Value==TargetType.Shrine||this.IsChestContainer)
													 this.Obstacletype=ObstacleType.ServerObject;
										  }

										  if (!this.Gizmotype.HasValue)
												this.Gizmotype=thisGizmoType;
									 }
									 else if (SnoCacheLookup.hashSNOInteractWhitelist.Contains(this.SNOID))
										  this.targetType=TargetType.Interactable;
									 else if (this.Actortype.Value==ActorType.ServerProp)
									 {
										  string TestString=this.InternalName.ToLower();
										  //Server props with Base in name are the destructibles "remains" which is considered an obstacle!
										  if (TestString.Contains("base")||TestString.Contains("fence"))
										  {
												//Add this to the obstacle navigation cache
												if (!this.IsObstacle)
													 hashSNONavigationObstacles.Add(this.SNOID);

												this.Obstacletype=ObstacleType.ServerObject;

												//Use unknown since we lookup SNO ID for server prop related objects.
												this.targetType=TargetType.None;
										  }
										  else if (TestString.StartsWith("monsteraffix_"))
										  {
												AvoidanceType T=FindAvoidanceUsingName(TestString);
												if (T==AvoidanceType.Wall)
												{
													 Bot.Combat.bCheckGround=true;
													 //Add this to the obstacle navigation cache
													 if (!this.IsObstacle)
														  hashSNONavigationObstacles.Add(this.SNOID);

													 this.Obstacletype=ObstacleType.ServerObject;

													 //Use unknown since we lookup SNO ID for server prop related objects.
													 this.targetType=TargetType.None;
												}
												else if (Bot.AvoidancesHealth.ContainsKey(T))
												{
													 Logging.WriteVerbose("Found Avoidance not recongized by SNO! Name {0} SNO {1}", TestString, this.SNOID);
													 hashAvoidanceSNOList.Add(this.SNOID);
													 this.targetType=TargetType.Avoidance;
												}
												else
												{
													 //Blacklist all other monster affixes
													 IgnoreThisObject(this, raguid, true, true);
													 return false;
												}
										  }
										  else
										  {
												IgnoreThisObject(this, raguid, true, true);
												return false;
										  }


									 }
									 else
									 {//Misc?? Ignore it!
										  IgnoreThisObject(this, raguid, true, true);
										  return false;
									 }
								}
						  } catch (NullReferenceException ) { Logging.WriteVerbose("Failure to get actorType for object, SNO: {0}", this.SNOID); return false; }
						  #endregion
					 }


					 if (!this.Obstacletype.HasValue)
						  this.Obstacletype=ObstacleType.None;


					 if (this.targetType.Value==TargetType.Unit)
					 {
						  SNORecordMonster monsterInfo=null;
						  try
						  {
								monsterInfo=thisObj.CommonData.MonsterInfo;
						  } catch (Exception)
						  {
								Logging.WriteDiagnostic("Safely Handled MonsterInfo Exception for Object {0}", this.InternalName);
								return false;
						  }
							

						  if (!this.Monstertype.HasValue||this.ShouldRefreshMonsterType)
						  {
								#region MonsterType
								try
								{
									 this.Monstertype=monsterInfo.MonsterType;
								} catch (NullReferenceException )
								{ Logging.WriteVerbose("Failure to get MonsterType for SNO: {0}", this.SNOID); failureDuringUpdate=true; }
								#endregion
						  }
						  if (!this.Monstersize.HasValue)
						  {
								#region MonsterSize
								try
								{
									 this.Monstersize=monsterInfo.MonsterSize;
								} catch (NullReferenceException )
								{ Logging.WriteVerbose("Failure to get MonsterSize for SNO: {0}", this.SNOID); failureDuringUpdate=true; }
								#endregion
						  }

						  /*
						  if (!this.RunningRate.HasValue)
						  {
								#region RunningRate
								try
								{
									 this.RunningRate=thisObj.CommonData.GetAttribute<double>(ActorAttributeType.);
								} catch
								{ Logging.WriteVerbose("Failure to get RunningRate for SNO: {0}", this.SNOID); return false; }
								#endregion
						  }
						  */
					 }


					 if (this.Actortype.HasValue&&this.targetType.HasValue&&
						  (this.Actortype.Value!=ActorType.Item&&this.targetType.Value!=TargetType.Avoidance))
					 {
						  //Validate sphere info
						  Sphere sphereInfo=thisObj.CollisionSphere;

						  if (sphereInfo.Center==null) return false;

						  if (!this.CollisionRadius.HasValue)
						  {
								#region CollisionRadius
								try
								{
									 this.CollisionRadius=sphereInfo.Radius;
								} catch (NullReferenceException )
								{ Logging.WriteVerbose("Failure to get CollisionRadius for SNO: {0}", this.SNOID); failureDuringUpdate=true; }
								#endregion

								if (this.InternalName=="monsterAffix_waller_model")
									 this.CollisionRadius/=2.5f;
						  }

						  if (!this.ActorSphereRadius.HasValue)
						  {
								#region ActorSphereRadius
								try
								{
									 this.ActorSphereRadius=thisObj.ActorInfo.Sphere.Radius;
								} catch (NullReferenceException )
								{
									 Logging.WriteVerbose("Safely handled getting attribute Sphere radius for gizmo {0}", this.InternalName);
									 failureDuringUpdate=true;
								}
								#endregion
						  }

						  #region GizmoProperties
						  if (this.targetType.Value==TargetType.Destructible||this.targetType.Value==TargetType.Barricade||this.targetType.Value==TargetType.Interactable)
						  {
								//No Loot
								if (!this.DropsNoLoot.HasValue)
								{
									 #region DropsNoLoot
									 try
									 {
										  this.DropsNoLoot=thisObj.CommonData.GetAttribute<float>(ActorAttributeType.DropsNoLoot)<=0;
									 } catch (NullReferenceException )
									 {
										  Logging.WriteVerbose("Safely handled reading DropsNoLoot for gizmo {0}", this.InternalName);
										  failureDuringUpdate=true;
									 }
									 #endregion
								}
								//No XP
								if (!this.GrantsNoXP.HasValue)
								{
									 #region GrantsNoXP
									 try
									 {

										  this.GrantsNoXP=thisObj.CommonData.GetAttribute<float>(ActorAttributeType.GrantsNoXP)<=0;
									 } catch (NullReferenceException )
									 {
										  Logging.WriteVerbose("Safely handled reading GrantsNoXp for gizmo {0}", this.InternalName);
										  failureDuringUpdate=true;
									 }
									 #endregion
								}
								//Barricade flag
								if (!this.IsBarricade.HasValue)
								{
									 #region Barricade
									 try
									 {
										  this.IsBarricade=((DiaGizmo)thisObj).IsBarricade;
									 } catch (NullReferenceException )
									 {
										  Logging.WriteVerbose("Safely handled getting attribute IsBarricade for gizmo {0}", this.InternalName);
										  failureDuringUpdate=true;
									 }
									 #endregion
								}
						  }
						  #endregion
					 }


					 if (failureDuringUpdate)
						  return false;


					 return true;
				}

				public override int GetHashCode()
				{
					 return base.SNOID;
				}

				public override bool Equals(object obj)
				{
					 //Check for null and compare run-time types. 
					 if (obj==null||this.GetType()!=obj.GetType())
					 {
						  return false;
					 }
					 else
					 {
						  CachedSNOEntry p=(CachedSNOEntry)obj;
						  return this.SNOID==p.SNOID;
					 }
				}


		  }

	 }
}