using System;

using Zeta;
using Zeta.Internals.SNO;
using Zeta.Internals.Actors;
using System.Collections.Generic;
using Zeta.Common;
using Zeta.Internals.Actors.Gizmos;

using FunkyTrinity.Cache;
using FunkyTrinity.Cache.Enums;
using FunkyTrinity.Avoidances;

namespace FunkyTrinity.Cache
{



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

						  if (ObjectCache.dictCanBurrow.ContainsKey(SNOID))
								 return ObjectCache.dictCanBurrow[SNOID];
						  else
								return null;
					 }
					 set
					 {
						  if (!this.IsFinalized)
								 ObjectCache.dictCanBurrow[SNOID]=value;
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

							if (ObjectCache.dictIsBarricade.ContainsKey(SNOID))
								 return ObjectCache.dictIsBarricade[SNOID];
						  else
								return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
								return;

							ObjectCache.dictIsBarricade[SNOID]=value;
					 }
				}

				private readonly bool? _DropsNoLoot;
				public bool? DropsNoLoot
				{
					 get
					 {
						  if (this.IsFinalized)
								return this._DropsNoLoot;

							if (ObjectCache.dictDropsNoLoot.ContainsKey(SNOID))
								 return ObjectCache.dictDropsNoLoot[SNOID];
						  else
								return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
								return;

							ObjectCache.dictDropsNoLoot[SNOID]=value;
					 }
				}

				private readonly bool? _GrantsNoXP;
				public bool? GrantsNoXP
				{
					 get
					 {
						  if (this.IsFinalized)
								return this._GrantsNoXP;

							if (ObjectCache.dictGrantsNoXp.ContainsKey(SNOID))
								 return ObjectCache.dictGrantsNoXp[SNOID];
						  else
								return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
								return;

							ObjectCache.dictGrantsNoXp[SNOID]=value;
					 }
				}

				private readonly GizmoType? _gizmotype;
				public GizmoType? Gizmotype
				{
					 get
					 {
						  if (this.IsFinalized)
								return this._gizmotype;

							if (ObjectCache.dictGizmoType.ContainsKey(SNOID))
								 return ObjectCache.dictGizmoType[SNOID];
						  else
								return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
								return;

							ObjectCache.dictGizmoType[SNOID]=value;
					 }
				}

				private readonly float? _actorsphereradius;
				public float? ActorSphereRadius
				{
					 get
					 {
						  if (this.IsFinalized)
								return this._actorsphereradius;

							if (ObjectCache.dictActorSphereRadius.ContainsKey(SNOID))
								 return ObjectCache.dictActorSphereRadius[SNOID];
						  else
								return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
								return;

							ObjectCache.dictActorSphereRadius[SNOID]=value;
					 }
				}

				private readonly ObstacleType? _obstacletype;
				public ObstacleType? Obstacletype
				{
					 get
					 {
						  if (this.IsFinalized)
								return this._obstacletype;

							if (ObjectCache.dictObstacleType.ContainsKey(SNOID))
								 return ObjectCache.dictObstacleType[SNOID];
						  else
								return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
								return;

							ObjectCache.dictObstacleType[SNOID]=value;
					 }
				}

				private readonly ActorType? _actortype;
				public ActorType? Actortype
				{
					 get
					 {
						  if (this.IsFinalized) return this._actortype;

							if (ObjectCache.dictActorType.ContainsKey(SNOID)) return ObjectCache.dictActorType[SNOID]; else return null;
					 }
					 set
					 {
						  if (this.IsFinalized) return;

							ObjectCache.dictActorType[SNOID]=value;
					 }
				}

				private TargetType? _targettype;
				public TargetType? targetType
				{
					 get
					 {
						  if (this.IsFinalized) return this._targettype;

							if (ObjectCache.dictTargetType.ContainsKey(SNOID)) return ObjectCache.dictTargetType[SNOID]; else return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
						  {
								_targettype=value;
								return;
						  }

							ObjectCache.dictTargetType[SNOID]=value;
					 }
				}

				private readonly float? _collisionradius;
				public float? CollisionRadius
				{
					 get
					 {
						  if (this.IsFinalized) return this._collisionradius;

							if (ObjectCache.dictCollisionRadius.ContainsKey(SNOID)) return ObjectCache.dictCollisionRadius[SNOID]; else return null;
					 }
					 set
					 {
						  if (this.IsFinalized) return;

							ObjectCache.dictCollisionRadius[SNOID]=(float)value;

					 }
				}

				private MonsterType? _monstertype;
				public MonsterType? Monstertype
				{
					 get
					 {
						  if (this.IsFinalized) return this._monstertype;

							if (ObjectCache.dictMonstertype.ContainsKey(SNOID)) return ObjectCache.dictMonstertype[SNOID]; else return null;
					 }
					 set
					 {
						  if (this.IsFinalized)
						  {
								this._monstertype=value;
								return;
						  }

							ObjectCache.dictMonstertype[SNOID]=value;
					 }
				}

				private readonly MonsterSize? _monstersize;
				public MonsterSize? Monstersize
				{
					 get
					 {
						  if (this.IsFinalized) return this._monstersize;

							if (ObjectCache.dictMonstersize.ContainsKey(SNOID)) return ObjectCache.dictMonstersize[SNOID]; else return null;
					 }
					 set
					 {
						  if (this.IsFinalized) return;

							ObjectCache.dictMonstersize[SNOID]=value;

					 }
				}

				private readonly string _internalname;
				public string InternalName
				{
					 get
					 {
						  if (this.IsFinalized) return this._internalname;


							if (ObjectCache.dictInternalName.ContainsKey(SNOID)) return ObjectCache.dictInternalName[SNOID]; else return null;
					 }
					 set
					 {
						  if (this.IsFinalized) return;

							ObjectCache.dictInternalName[SNOID]=value;
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
				public bool IsObstacle { get { return CacheIDLookup.hashSNONavigationObstacles.Contains(SNOID); } }
				public bool IsHealthWell { get { return SNOID==138989; } }
				public bool IsTreasureGoblin { get { return CacheIDLookup.hashActorSNOGoblins.Contains(SNOID); } }
				public bool IsBoss { get { return CacheIDLookup.hashBossSNO.Contains(SNOID); } }
				public bool IsWormBoss { get { return (SNOID==218947||SNOID==144400); } }
				public bool IsResplendantChest { get { return CacheIDLookup.hashSNOContainerResplendant.Contains(SNOID); } }
				public bool IsAvoidance { get { return AvoidanceCache.hashAvoidanceSNOList.Contains(SNOID); } }
				public bool IsSummonedPet { get { return CacheIDLookup.hashSummonedPets.Contains(SNOID); } }
				public bool IsRespawnable { get { return CacheIDLookup.hashActorSNOSummonedUnit.Contains(SNOID); } }
				public bool IsProjectileAvoidance { get { return AvoidanceCache.hashAvoidanceSNOProjectiles.Contains(SNOID); } }
				public bool IsCorpseContainer { get { return (this.internalNameLower.Contains("loottype")||this.internalNameLower.Contains("corpse")); } }
				public bool IsChestContainer { get { return (this.internalNameLower.Contains("chest")); } }
				public bool IgnoresLOSCheck { get { return CacheIDLookup.hashActorSNOIgnoreLOSCheck.Contains(SNOID); } }
				public bool IsMissileReflecting { get { return CacheIDLookup.hashActorSNOReflectiveMissleUnits.Contains(SNOID); } }
				public bool IsStealthableUnit { get { return CacheIDLookup.hashActorSNOStealthUnits.Contains(SNOID); } }
				public bool IsBurrowableUnit { get { return CacheIDLookup.hashActorSNOBurrowableUnits.Contains(SNOID); } }
				public bool IsSucideBomber { get { return CacheIDLookup.hashActorSNOSucideBomberUnits.Contains(SNOID); } }
				public bool IsGrotesqueActor { get { return CacheIDLookup.hashActorSNOCorpulent.Contains(SNOID); } }
				public bool IsFast { get { return CacheIDLookup.hashActorSNOFastMobs.Contains(SNOID); } }
				public bool IsCorruptantGrowth { get { return SNOID==210120||SNOID==210268; } }
				public bool IsSpawnerUnit { get { return CacheIDLookup.hashSpawnerUnitSNOs.Contains(SNOID); } }
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
						  } catch (NullReferenceException) {Logger.Write(LogLevel.Execption, "Failure to get internal name on object, SNO {0}", this.SNOID); return false; }
					 }

					 if (!this.Actortype.HasValue)
					 {
						  #region ActorType
						  try
						  {
								this.Actortype=thisObj.ActorType;
						  } catch (NullReferenceException) { Logger.Write(LogLevel.Execption, "Failure to get actorType for object, SNO: {0}", this.SNOID); return false; }
						  #endregion
					 }

					 //Ignored actor types..
					 if (BlacklistCache.IgnoredActorTypes.Contains(this.Actortype.Value))//||!LootBehaviorEnabled&&this.Actortype.Value==ActorType.Item)
					 {
						  BlacklistCache.IgnoreThisObject(this, raguid, true, true);
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

											AvoidanceType AT=AvoidanceCache.FindAvoidanceUsingSNOID(this.SNOID);

										  //Check if avoidance is enabled or if the avoidance type is set to 0
										  if (!Bot.SettingsFunky.Avoidance.AttemptAvoidanceMovements||AT!=AvoidanceType.None&&AvoidanceCache.IgnoringAvoidanceType(AT))
										  {
												BlacklistCache.AddObjectToBlacklist(raguid, BlacklistType.Temporary);
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
										  CacheIDLookup.hashActorSNOForceTargetUnit.Contains(this.SNOID))
									 {
										  this.targetType=TargetType.Unit;
										  this.Obstacletype=ObstacleType.Monster;

										  if (CacheIDLookup.hashActorSNOForceTargetUnit.Contains(this.SNOID))
										  {
												//Fill in monster data?
												this.Actortype=ActorType.Unit;
										  }
									 }
									 else if (this.Actortype.Value==ActorType.Item||
										  CacheIDLookup.hashForceSNOToItemList.Contains(this.SNOID))
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
										  } catch (NullReferenceException) { Logger.Write(LogLevel.Execption, "Failure to get actor Gizmo Type!"); return false; }


										  if (thisGizmoType==GizmoType.DestructibleLootContainer||thisGizmoType==GizmoType.Destructible)
												this.targetType=TargetType.Destructible;
										  else if (thisGizmoType==GizmoType.Shrine||thisGizmoType==GizmoType.Healthwell)
										  {
												this.targetType=TargetType.Shrine;
										  }
										  else if (thisGizmoType==GizmoType.LootContainer)
												this.targetType=TargetType.Container;
										  else if (thisGizmoType==GizmoType.Barricade)
												this.targetType=TargetType.Barricade;
										  else if (thisGizmoType==GizmoType.Door)
												this.targetType=TargetType.Door;
										  else
										  {//All other gizmos should be ignored!
												BlacklistCache.IgnoreThisObject(this, raguid, true, true);
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
									 else if (CacheIDLookup.hashSNOInteractWhitelist.Contains(this.SNOID))
										  this.targetType=TargetType.Interactable;
									 else if (this.Actortype.Value==ActorType.ServerProp)
									 {
										  string TestString=this.InternalName.ToLower();
										  //Server props with Base in name are the destructibles "remains" which is considered an obstacle!
										  if (TestString.Contains("base")||TestString.Contains("fence"))
										  {
												//Add this to the obstacle navigation cache
												if (!this.IsObstacle)
													 CacheIDLookup.hashSNONavigationObstacles.Add(this.SNOID);

												this.Obstacletype=ObstacleType.ServerObject;

												//Use unknown since we lookup SNO ID for server prop related objects.
												this.targetType=TargetType.None;
										  }
										  else if (TestString.StartsWith("monsteraffix_"))
										  {
												 AvoidanceType T=AvoidanceCache.FindAvoidanceUsingName(TestString);
												if (T==AvoidanceType.Wall)
												{
													 Bot.Combat.bCheckGround=true;
													 //Add this to the obstacle navigation cache
													 if (!this.IsObstacle)
															CacheIDLookup.hashSNONavigationObstacles.Add(this.SNOID);

													 this.Obstacletype=ObstacleType.ServerObject;

													 //Use unknown since we lookup SNO ID for server prop related objects.
													 this.targetType=TargetType.None;
												}
												//else if (Bot.AvoidancesHealth.ContainsKey(T))
												//{
												//	 Logging.WriteVerbose("Found Avoidance not recongized by SNO! Name {0} SNO {1}", TestString, this.SNOID);
												//	 CacheIDLookup.hashAvoidanceSNOList.Add(this.SNOID);
												//	 this.targetType=TargetType.Avoidance;
												//}
												else
												{
													 //Blacklist all other monster affixes
													 BlacklistCache.IgnoreThisObject(this, raguid, true, true);
													 return false;
												}
										  }
										  else
										  {
												BlacklistCache.IgnoreThisObject(this, raguid, true, true);
												return false;
										  }


									 }
									 else
									 {//Misc?? Ignore it!
										  BlacklistCache.IgnoreThisObject(this, raguid, true, true);
										  return false;
									 }
								}
						  } catch (NullReferenceException) { Logger.Write(LogLevel.Execption, "Failure to get actorType for object, SNO: {0}", this.SNOID); return false; }
						  #endregion
					 }


					 if (!this.Obstacletype.HasValue)
						  this.Obstacletype=ObstacleType.None;


					 if (ObjectCache.CheckTargetTypeFlag(this.targetType.Value,TargetType.Unit))
					 {
						  SNORecordMonster monsterInfo=null;
						  try
						  {
								monsterInfo=thisObj.CommonData.MonsterInfo;
						  } catch (Exception)
						  {
								Logger.Write(LogLevel.Execption, "Safely Handled MonsterInfo Exception for Object {0}", this.InternalName);
								return false;
						  }
							

						  if (!this.Monstertype.HasValue||this.ShouldRefreshMonsterType)
						  {
								#region MonsterType
								try
								{
									 this.Monstertype=monsterInfo.MonsterType;
								} catch (NullReferenceException )
								{ Logger.Write(LogLevel.Execption, "Failure to get MonsterType for SNO: {0}", this.SNOID); failureDuringUpdate=true; }
								#endregion
						  }
						  if (!this.Monstersize.HasValue)
						  {
								#region MonsterSize
								try
								{
									 this.Monstersize=monsterInfo.MonsterSize;
								} catch (NullReferenceException )
								{ Logger.Write(LogLevel.Execption, "Failure to get MonsterSize for SNO: {0}", this.SNOID); failureDuringUpdate=true; }
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
								{ Logger.Write(LogLevel.Execption, "Failure to get CollisionRadius for SNO: {0}", this.SNOID); failureDuringUpdate=true; }
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
									 Logger.Write(LogLevel.Execption, "Safely handled getting attribute Sphere radius for gizmo {0}", this.InternalName);
									 failureDuringUpdate=true;
								}
								#endregion
						  }

						  #region GizmoProperties
						  if (ObjectCache.CheckTargetTypeFlag(this.targetType.Value,TargetType.Destructible|TargetType.Interactable))
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
										  Logger.Write(LogLevel.Execption, "Safely handled reading DropsNoLoot for gizmo {0}", this.InternalName);
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
										  Logger.Write(LogLevel.Execption, "Safely handled reading GrantsNoXp for gizmo {0}", this.InternalName);
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
										  Logger.Write(LogLevel.Execption, "Safely handled getting attribute IsBarricade for gizmo {0}", this.InternalName);
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