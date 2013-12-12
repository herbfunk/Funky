using System;
using System.Globalization;
using FunkyBot.Cache.Enums;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.Internals.SNO;

namespace FunkyBot.Cache.Objects
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
					 SNOID=sno;

					 CachedSNOEntry thisEntry=ObjectCache.cacheSnoCollection[sno];

					 if (sno>0)
					 {
						  SNOID=thisEntry.SNOID;
						  _actortype=thisEntry.Actortype;
						  _targettype=thisEntry.targetType;
						  _monstersize=thisEntry.Monstersize;
						  _monstertype=thisEntry.Monstertype;
						  _collisionradius=thisEntry.CollisionRadius;
						  _actorsphereradius=thisEntry.ActorSphereRadius;
						  _CanBurrow=thisEntry.CanBurrow;
						  _GrantsNoXP=thisEntry.GrantsNoXP;
						  _DropsNoLoot=thisEntry.DropsNoLoot;
						  _IsBarricade=thisEntry.IsBarricade;
						  _internalname=thisEntry.InternalName;
						  _obstacletype=thisEntry.Obstacletype;
						  _gizmotype=thisEntry.Gizmotype;
						  //this._RunningRate=thisEntry.RunningRate;
						  IsFinalized=thisEntry.IsFinalized;
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
					 SNOID=sno.SNOID;
					 _actortype=sno.Actortype;
					 _targettype=sno.targetType;
					 _monstersize=sno.Monstersize;
					 _monstertype=sno.Monstertype;
					 _collisionradius=sno.CollisionRadius;
					 _actorsphereradius=sno.ActorSphereRadius;
					 _CanBurrow=sno.CanBurrow;
					 _GrantsNoXP=sno.GrantsNoXP;
					 _DropsNoLoot=sno.DropsNoLoot;
					 _IsBarricade=sno.IsBarricade;
					 _internalname=sno.InternalName;
					 _obstacletype=sno.Obstacletype;
					 _gizmotype=sno.Gizmotype;
					 //this._RunningRate=sno.RunningRate;
					 IsFinalized=sno.IsFinalized;
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
						  if (IsFinalized)
								return _CanBurrow;

						  if (ObjectCache.dictCanBurrow.ContainsKey(SNOID))
								 return ObjectCache.dictCanBurrow[SNOID];
						 return null;
					 }
					 set
					 {
						  if (!IsFinalized)
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
						  if (IsFinalized)
								return _IsBarricade;

							if (ObjectCache.dictIsBarricade.ContainsKey(SNOID))
								 return ObjectCache.dictIsBarricade[SNOID];
						 return null;
					 }
					 set
					 {
						  if (IsFinalized)
								return;

							ObjectCache.dictIsBarricade[SNOID]=value;
					 }
				}

				private readonly bool? _DropsNoLoot;
				public bool? DropsNoLoot
				{
					 get
					 {
						  if (IsFinalized)
								return _DropsNoLoot;

							if (ObjectCache.dictDropsNoLoot.ContainsKey(SNOID))
								 return ObjectCache.dictDropsNoLoot[SNOID];
						 return null;
					 }
					 set
					 {
						  if (IsFinalized)
								return;

							ObjectCache.dictDropsNoLoot[SNOID]=value;
					 }
				}

				private readonly bool? _GrantsNoXP;
				public bool? GrantsNoXP
				{
					 get
					 {
						  if (IsFinalized)
								return _GrantsNoXP;

							if (ObjectCache.dictGrantsNoXp.ContainsKey(SNOID))
								 return ObjectCache.dictGrantsNoXp[SNOID];
						 return null;
					 }
					 set
					 {
						  if (IsFinalized)
								return;

							ObjectCache.dictGrantsNoXp[SNOID]=value;
					 }
				}

				private readonly GizmoType? _gizmotype;
				public GizmoType? Gizmotype
				{
					 get
					 {
						  if (IsFinalized)
								return _gizmotype;

							if (ObjectCache.dictGizmoType.ContainsKey(SNOID))
								 return ObjectCache.dictGizmoType[SNOID];
						 return null;
					 }
					 set
					 {
						  if (IsFinalized)
								return;

							ObjectCache.dictGizmoType[SNOID]=value;
					 }
				}

				private readonly float? _actorsphereradius;
				public float? ActorSphereRadius
				{
					 get
					 {
						  if (IsFinalized)
								return _actorsphereradius;

							if (ObjectCache.dictActorSphereRadius.ContainsKey(SNOID))
								 return ObjectCache.dictActorSphereRadius[SNOID];
						 return null;
					 }
					 set
					 {
						  if (IsFinalized)
								return;

							ObjectCache.dictActorSphereRadius[SNOID]=value;
					 }
				}

				private readonly ObstacleType? _obstacletype;
				public ObstacleType? Obstacletype
				{
					 get
					 {
						  if (IsFinalized)
								return _obstacletype;

							if (ObjectCache.dictObstacleType.ContainsKey(SNOID))
								 return ObjectCache.dictObstacleType[SNOID];
						 return null;
					 }
					 set
					 {
						  if (IsFinalized)
								return;

							ObjectCache.dictObstacleType[SNOID]=value;
					 }
				}

				private readonly ActorType? _actortype;
				public ActorType? Actortype
				{
					 get
					 {
						  if (IsFinalized) return _actortype;

							if (ObjectCache.dictActorType.ContainsKey(SNOID)) return ObjectCache.dictActorType[SNOID];
						 return null;
					 }
					 set
					 {
						  if (IsFinalized) return;

							ObjectCache.dictActorType[SNOID]=value;
					 }
				}

				private TargetType? _targettype;
				public TargetType? targetType
				{
					 get
					 {
						  if (IsFinalized) return _targettype;

							if (ObjectCache.dictTargetType.ContainsKey(SNOID)) return ObjectCache.dictTargetType[SNOID];
						 return null;
					 }
					 set
					 {
						  if (IsFinalized)
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
						  if (IsFinalized) return _collisionradius;

							if (ObjectCache.dictCollisionRadius.ContainsKey(SNOID)) return ObjectCache.dictCollisionRadius[SNOID];
						 return null;
					 }
					 set
					 {
						  if (IsFinalized) return;

							ObjectCache.dictCollisionRadius[SNOID]=value;

					 }
				}

				private MonsterType? _monstertype;
				public MonsterType? Monstertype
				{
					 get
					 {
						  if (IsFinalized) return _monstertype;

							if (ObjectCache.dictMonstertype.ContainsKey(SNOID)) return ObjectCache.dictMonstertype[SNOID];
						 return null;
					 }
					 set
					 {
						  if (IsFinalized)
						  {
								_monstertype=value;
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
						  if (IsFinalized) return _monstersize;

							if (ObjectCache.dictMonstersize.ContainsKey(SNOID)) return ObjectCache.dictMonstersize[SNOID];
						 return null;
					 }
					 set
					 {
						  if (IsFinalized) return;

							ObjectCache.dictMonstersize[SNOID]=value;

					 }
				}

				private readonly string _internalname;
				public string InternalName
				{
					 get
					 {
						  if (IsFinalized) return _internalname;


							if (ObjectCache.dictInternalName.ContainsKey(SNOID)) return ObjectCache.dictInternalName[SNOID];
						 return null;
					 }
					 set
					 {
						  if (IsFinalized) return;

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
						  string debugstring="SNO: "+SNOID+"("+InternalName+")\r\n";
						  debugstring+=Actortype.HasValue?"ActorType: "+Actortype.Value.ToString()+" ":"";
						  debugstring+=targetType.HasValue?"TargetType: "+targetType.Value.ToString()+" "+"\r\n":""+"\r\n";

						  debugstring+=CollisionRadius.HasValue?"CollisionRadius: "+CollisionRadius.Value.ToString(CultureInfo.InvariantCulture)+" ":"";
						  debugstring+=ActorSphereRadius.HasValue?"ActorSphereRadius: "+ActorSphereRadius.Value.ToString(CultureInfo.InvariantCulture)+" "+"\r\n":""+"\r\n";

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
				public bool IsCorpseContainer { get { return (internalNameLower.Contains("loottype")||internalNameLower.Contains("corpse")); } }
				public bool IsChestContainer { get { return (internalNameLower.Contains("chest")); } }
				public bool IgnoresLOSCheck { get { return CacheIDLookup.hashActorSNOIgnoreLOSCheck.Contains(SNOID); } }
				public bool IsMissileReflecting { get { return CacheIDLookup.hashActorSNOReflectiveMissleUnits.Contains(SNOID); } }
				public bool IsStealthableUnit { get { return CacheIDLookup.hashActorSNOStealthUnits.Contains(SNOID); } }
				public bool IsBurrowableUnit { get { return CacheIDLookup.hashActorSNOBurrowableUnits.Contains(SNOID); } }
				public bool IsSucideBomber { get { return CacheIDLookup.hashActorSNOSucideBomberUnits.Contains(SNOID); } }
				public bool IsGrotesqueActor { get { return CacheIDLookup.hashActorSNOCorpulent.Contains(SNOID); } }
				public bool IsCorruptantGrowth { get { return SNOID==210120||SNOID==210268; } }
				public bool IsSpawnerUnit { get { return CacheIDLookup.hashSpawnerUnitSNOs.Contains(SNOID); } }
				public bool IsTransformUnit { get { return CacheIDLookup.hashActorSNOTransforms.Contains(SNOID);} }
				public bool IsFlyingHoverUnit { get { return CacheIDLookup.hashActorSNOFlying.Contains(SNOID); } }
				#endregion

				public bool ContainsNullValues()
				{
					 if (IsFinalized||SNOID==0) return false;

					 if (!targetType.HasValue||!Actortype.HasValue||InternalName==null||!Obstacletype.HasValue)
						  return true;

					 if (targetType.Value==TargetType.Unit)
					 {
						  if (!Monstertype.HasValue||!Monstersize.HasValue) return true; //||!this.RunningRate.HasValue
					 }

					 if (targetType.Value!=TargetType.Item&&targetType.Value!=TargetType.Avoidance)
					 {
						  if (!CollisionRadius.HasValue||!ActorSphereRadius.HasValue) return true;

						  if (targetType.Value==TargetType.Destructible||targetType.Value==TargetType.Barricade||targetType.Value==TargetType.Interactable)
						  {
								if (!DropsNoLoot.HasValue||!GrantsNoXP.HasValue||!IsBarricade.HasValue||!Gizmotype.HasValue) return true;
						  }
					 }
					 return false;
				}

				public virtual object Clone()
				{
					 return MemberwiseClone();
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
						 if (!Monstertype.HasValue)
								return true;
						 return ((Monstertype==MonsterType.Ally||Monstertype==MonsterType.Scenery||
						          Monstertype==MonsterType.Helper||Monstertype==MonsterType.Team));
					 }
				}
				public bool MonsterTypeIsHostile()
				{
					 switch (Monstertype.Value)
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

					 if (InternalName==null)
					 {
						  try
						  {
								InternalName=thisObj.Name;
						  } catch
						  {
								if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Execption))
									 Logger.Write(LogLevel.Execption, "Failure to get internal name on object, SNO {0}", SNOID); 
							  
							  return false;
						  }
					 }

					 if (!Actortype.HasValue)
					 {
						  #region ActorType
						  try
						  {
								Actortype=thisObj.ActorType;
						  } catch
						  {
								if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Execption))
									 Logger.Write(LogLevel.Execption, "Failure to get actorType for object, SNO: {0}", SNOID); 
							  
							  return false;
						  }
						  #endregion
					 }

					 //Ignored actor types..
					 if (BlacklistCache.IgnoredActorTypes.Contains(Actortype.Value))//||!LootBehaviorEnabled&&this.Actortype.Value==ActorType.Item)
					 {
						  BlacklistCache.IgnoreThisObject(this, raguid);
						  return false;
					 }

					 if (!targetType.HasValue)
					 {
						  #region EvaluateTargetType
						  try
						  {
								//Evaluate Target Type..
								// See if it's an avoidance first from the SNO
								if (IsAvoidance||IsObstacle)
								{
									 targetType=TargetType.None;

									 if (IsAvoidance)
									 {
										  if (IsProjectileAvoidance)
												Obstacletype=ObstacleType.MovingAvoidance;
										  else
												Obstacletype=ObstacleType.StaticAvoidance;

											AvoidanceType AT=AvoidanceCache.FindAvoidanceUsingSNOID(SNOID);

										  //Check if avoidance is enabled or if the avoidance type is set to 0
										  if (!Bot.Settings.Avoidance.AttemptAvoidanceMovements||AT!=AvoidanceType.None&&AvoidanceCache.IgnoringAvoidanceType(AT))
										  {
												BlacklistCache.AddObjectToBlacklist(raguid, BlacklistType.Temporary);
												return false;
										  }

										  // Avoidance isn't disabled, so set this object type to avoidance
										  targetType=TargetType.Avoidance;
									 }
									 else
										  Obstacletype=ObstacleType.ServerObject;
								}
								else
								{
									 // Calculate the object type of this object
									 if (Actortype.Value==ActorType.Unit||
										  CacheIDLookup.hashActorSNOForceTargetUnit.Contains(SNOID))
									 {
										  targetType=TargetType.Unit;
										  Obstacletype=ObstacleType.Monster;

										  if (CacheIDLookup.hashActorSNOForceTargetUnit.Contains(SNOID))
										  {
												//Fill in monster data?
												Actortype=ActorType.Unit;
										  }
									 }
									 else if (Actortype.Value==ActorType.Item||
										  CacheIDLookup.hashForceSNOToItemList.Contains(SNOID))
									 {
										  string testname=InternalName.ToLower();
										  //Check if this item is gold/globe..
										  if (testname.StartsWith("gold"))
												targetType=TargetType.Gold;
										  else if (testname.StartsWith("healthglobe"))
												targetType=TargetType.Globe;
										  else
												targetType=TargetType.Item;
										  //Gold/Globe?
									 }
									 else if (Actortype.Value==ActorType.Gizmo)
									 {

										  GizmoType thisGizmoType;
										  try
										  {
												thisGizmoType=thisObj.ActorInfo.GizmoType;
										  } catch
										  {
											  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Execption))
												Logger.Write(LogLevel.Execption, "Failure to get actor Gizmo Type!");
											  return false; 
										  }


										  if (thisGizmoType==GizmoType.DestructibleLootContainer||thisGizmoType==GizmoType.Destructible)
												targetType=TargetType.Destructible;
										  else if (thisGizmoType==GizmoType.Shrine||thisGizmoType==GizmoType.Healthwell)
										  {
												targetType=TargetType.Shrine;
										  }
										  else if (thisGizmoType==GizmoType.LootContainer)
												targetType=TargetType.Container;
										  else if (thisGizmoType==GizmoType.Barricade)
												targetType=TargetType.Barricade;
										  else if (thisGizmoType==GizmoType.Door)
												targetType=TargetType.Door;
										  else if(thisGizmoType== GizmoType.Waypoint||thisGizmoType== GizmoType.Portal || thisGizmoType== GizmoType.DungeonStonePortal ||thisGizmoType== GizmoType.BossPortal)
										  {//Special Interactive Object -- Add to special cache!
											  targetType = TargetType.ServerInteractable;
										  }
										  else
										  {//All other gizmos should be ignored!
												BlacklistCache.IgnoreThisObject(this, raguid);
												return false;
										  }

										  if (targetType.HasValue)
										  {
												if (targetType.Value==TargetType.Destructible||targetType.Value==TargetType.Barricade||targetType.Value==TargetType.Door)
													 Obstacletype=ObstacleType.Destructable;
												else if (targetType.Value==TargetType.Shrine||IsChestContainer)
													 Obstacletype=ObstacleType.ServerObject;
										  }

										  if (!Gizmotype.HasValue)
												Gizmotype=thisGizmoType;
									 }
									 else if (CacheIDLookup.hashSNOInteractWhitelist.Contains(SNOID))
										  targetType=TargetType.Interactable;
									 else if (Actortype.Value==ActorType.ServerProp)
									 {
										  string TestString=InternalName.ToLower();
										  //Server props with Base in name are the destructibles "remains" which is considered an obstacle!
										  if (TestString.Contains("base")||TestString.Contains("fence"))
										  {
												//Add this to the obstacle navigation cache
												if (!IsObstacle)
													 CacheIDLookup.hashSNONavigationObstacles.Add(SNOID);

												Obstacletype=ObstacleType.ServerObject;

												//Use unknown since we lookup SNO ID for server prop related objects.
												targetType=TargetType.None;
										  }
										  else if (TestString.StartsWith("monsteraffix_"))
										  {
												 AvoidanceType T=AvoidanceCache.FindAvoidanceUsingName(TestString);
												if (T==AvoidanceType.Wall)
												{
													 //Add this to the obstacle navigation cache
													 if (!IsObstacle)
															CacheIDLookup.hashSNONavigationObstacles.Add(SNOID);

													 Obstacletype=ObstacleType.ServerObject;

													 //Use unknown since we lookup SNO ID for server prop related objects.
													 targetType=TargetType.None;
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
													 BlacklistCache.IgnoreThisObject(this, raguid);
													 return false;
												}
										  }
										  else
										  {
												BlacklistCache.IgnoreThisObject(this, raguid);
												return false;
										  }


									 }
									 else
									 {//Misc?? Ignore it!
										  BlacklistCache.IgnoreThisObject(this, raguid);
										  return false;
									 }
								}
						  } catch
						  {
								if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Execption))
									 Logger.Write(LogLevel.Execption, "Failure to get actorType for object, SNO: {0}", SNOID); 
							  return false;
						  }
						  #endregion
					 }


					 if (!Obstacletype.HasValue)
						  Obstacletype=ObstacleType.None;


					 if (ObjectCache.CheckTargetTypeFlag(targetType.Value,TargetType.Unit))
					 {
						  SNORecordMonster monsterInfo;
						  try
						  {
								monsterInfo=thisObj.CommonData.MonsterInfo;
						  } catch
						  {
								if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Execption))
									 Logger.Write(LogLevel.Execption, "Safely Handled MonsterInfo Exception for Object {0}", InternalName);
								return false;
						  }
							

						  if (!Monstertype.HasValue||ShouldRefreshMonsterType)
						  {
								#region MonsterType
								try
								{
									 Monstertype=monsterInfo.MonsterType;
								} catch
								{
									 if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Execption))
										  Logger.Write(LogLevel.Execption, "Failure to get MonsterType for SNO: {0}", SNOID); 
									
									failureDuringUpdate=true;
								}
								#endregion
						  }
						  if (!Monstersize.HasValue)
						  {
								#region MonsterSize
								try
								{
									 Monstersize=monsterInfo.MonsterSize;
								} catch
								{
									 if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Execption))
										  Logger.Write(LogLevel.Execption, "Failure to get MonsterSize for SNO: {0}", SNOID); 
									
									failureDuringUpdate=true;
								}
								#endregion
						  }
					 }


					 if (Actortype.HasValue&&targetType.HasValue&&
						  (Actortype.Value != ActorType.Item && targetType.Value != TargetType.Avoidance && targetType.Value != TargetType.ServerInteractable))
					 {
						  //Validate sphere info
						  Sphere sphereInfo=thisObj.CollisionSphere;

						 if (!CollisionRadius.HasValue)
						  {
								#region CollisionRadius
								try
								{
									 CollisionRadius=sphereInfo.Radius;
								} catch
								{
									 if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Execption))
										  Logger.Write(LogLevel.Execption, "Failure to get CollisionRadius for SNO: {0}", SNOID); 
									
									failureDuringUpdate=true;
								}
								#endregion

                                if(CacheIDLookup.dictFixedCollisionRadius.ContainsKey(SNOID))
								{//Override The Default Collision Sphere Value
									 CollisionRadius=CacheIDLookup.dictFixedCollisionRadius[SNOID];
								}
						  }

						  if (!ActorSphereRadius.HasValue)
						  {
								#region ActorSphereRadius
								try
								{
									 ActorSphereRadius=thisObj.ActorInfo.Sphere.Radius;
								} catch
								{
									 if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Execption))
										  Logger.Write(LogLevel.Execption, "Safely handled getting attribute Sphere radius for gizmo {0}", InternalName);
									 failureDuringUpdate=true;
								}
								#endregion
						  }

						  #region GizmoProperties
						  if (ObjectCache.CheckTargetTypeFlag(targetType.Value,TargetType.Destructible|TargetType.Interactable))
						  {
								//No Loot
								if (!DropsNoLoot.HasValue)
								{
									 #region DropsNoLoot
									 try
									 {
										  DropsNoLoot=thisObj.CommonData.GetAttribute<float>(ActorAttributeType.DropsNoLoot)<=0;
									 } catch
									 {
										  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Execption))
												Logger.Write(LogLevel.Execption, "Safely handled reading DropsNoLoot for gizmo {0}", InternalName);
										  failureDuringUpdate=true;
									 }
									 #endregion
								}
								//No XP
								if (!GrantsNoXP.HasValue)
								{
									 #region GrantsNoXP
									 try
									 {

										  GrantsNoXP=thisObj.CommonData.GetAttribute<float>(ActorAttributeType.GrantsNoXP)<=0;
									 } catch
									 {
										  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Execption))
												Logger.Write(LogLevel.Execption, "Safely handled reading GrantsNoXp for gizmo {0}", InternalName);
										  failureDuringUpdate=true;
									 }
									 #endregion
								}
								//Barricade flag
								if (!IsBarricade.HasValue)
								{
									 #region Barricade
									 try
									 {
										  IsBarricade=((DiaGizmo)thisObj).IsBarricade;
									 } catch
									 {
										  if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Execption))
												Logger.Write(LogLevel.Execption, "Safely handled getting attribute IsBarricade for gizmo {0}", InternalName);
										  failureDuringUpdate=true;
									 }
									 #endregion
								}
						  }
						  #endregion
					 }


					 return !failureDuringUpdate;
				}

				public override int GetHashCode()
				{
					 return SNOID;
				}

				public override bool Equals(object obj)
				{
					 //Check for null and compare run-time types. 
					 if (obj==null||GetType()!=obj.GetType())
					 {
						  return false;
					 }
					CachedSNOEntry p=(CachedSNOEntry)obj;
					return SNOID==p.SNOID;
				}


		  }

	 
}