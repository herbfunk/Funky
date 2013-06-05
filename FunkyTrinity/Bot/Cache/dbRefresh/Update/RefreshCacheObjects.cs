using System;
using System.Linq;
using Zeta;
using System.Collections.Generic;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.CommonBot;
using Zeta.Internals.SNO;
using Zeta.Internals.Actors.Gizmos;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  public static readonly CacheObject FakeCacheObject=new CacheObject(vNullLocation, TargetType.None, 0d, "Fake Target", 1f, -1);
		  public partial class dbRefresh
		  {
				//Trimming/Update Vars
				private static int UpdateLoopCounter=0;


				///<summary>
				///Adds/Updates CacheObjects inside collection by Iteration of RactorList
				///This is the method that caches all live data about an object!
				///</summary>
				public static bool UpdateCacheObjectCollection()
				{
					 if (!ZetaDia.IsInGame) return false;

					 HashSet<int> hashDoneThisRactor=new HashSet<int>();
					 foreach (Actor thisActor in ZetaDia.Actors.RActorList)
					 {
						  int tmp_raGUID;
						  DiaObject thisObj;

						  if (!thisActor.IsValid) continue;
						  //Convert to DiaObject
						  thisObj=(DiaObject)thisActor;
						  tmp_raGUID=thisObj.RActorGuid;

						  // See if we've already checked this ractor, this loop
						  if (hashDoneThisRactor.Contains(tmp_raGUID)) continue;
						  hashDoneThisRactor.Add(tmp_raGUID);

						  //Update RactorGUID and check blacklisting..
						  if (!IsObjectBlacklisted(tmp_raGUID)) continue;
						  CacheObject tmp_CachedObj;
						  using (ZetaDia.Memory.AcquireFrame())
						  {
								if (!ObjectCache.Objects.TryGetValue(tmp_raGUID, out tmp_CachedObj))
								{
									 Vector3 tmp_position;
									 int tmp_acdguid;
									 int tmp_SNOID;

									 #region SNO
									 //Lookup SNO
									 try
									 {
										  tmp_SNOID=thisObj.ActorSNO;
									 } catch (NullReferenceException) { Logging.WriteVerbose("Failure to get SNO from object! RaGUID: {0}", tmp_raGUID); continue; }
									 #endregion


									 //check our SNO blacklist
									 if (hashActorSNOIgnoreBlacklist.Contains(tmp_SNOID)&&!SnoCacheLookup.hashSummonedPets.Contains(tmp_SNOID)) continue;


									 #region Position
									 try
									 {
										  tmp_position=thisObj.Position;
									 } catch (NullReferenceException) { Logging.WriteVerbose("Failure to get position vector for RAGUID {0}", tmp_raGUID); continue; }

									 #endregion

									 #region AcdGUID
									 try
									 {
										  tmp_acdguid=thisObj.ACDGuid;
									 } catch (NullReferenceException) { Logging.WriteVerbose("Failure to get ACDGUID for RAGUID {0}", tmp_raGUID); continue; }

									 #endregion



									 tmp_CachedObj=new CacheObject(tmp_SNOID, tmp_raGUID, tmp_acdguid, tmp_position);
								}
								else
									 //Reset unseen var
									 tmp_CachedObj.LoopsUnseen=0;


								//Validate
								try
								{
									 if (thisObj.CommonData==null||thisObj.CommonData.ACDGuid!=thisObj.ACDGuid) continue;
								} catch (NullReferenceException)
								{
									 continue;
								}
								


								//Check if this object is a summoned unit by a player...
								#region SummonedUnits
								if (tmp_CachedObj.IsSummonedPet)
								{
									 // Get the summoned-by info, cached if possible
									 if (!tmp_CachedObj.SummonerID.HasValue)
									 {
										  try
										  {
												tmp_CachedObj.SummonerID=thisObj.CommonData.GetAttribute<int>(ActorAttributeType.SummonedByACDID);
										  } catch (NullReferenceException ex)
										  {
												Logging.WriteVerbose("[Funky] Safely handled exception getting summoned-by info for Mystic Ally ["+tmp_CachedObj.SNOID.ToString()+"]");
												Logging.WriteDiagnostic(ex.ToString());
												continue;
										  }
									 }

									 //See if this summoned unit was summoned by the bot.
									 if (Bot.Character.iMyDynamicID==tmp_CachedObj.SummonerID.Value)
									 {
										  //Now modify the player data pets count..
										  if (Bot.Class.AC==ActorClass.Monk)
												Bot.Character.PetData.MysticAlly++;
										  else if (Bot.Class.AC==ActorClass.DemonHunter&&SnoCacheLookup.hashDHPets.Contains(tmp_CachedObj.SNOID))
												Bot.Character.PetData.DemonHunterPet++;
										  else if (Bot.Class.AC==ActorClass.WitchDoctor)
										  {
												if (SnoCacheLookup.hashZombie.Contains(tmp_CachedObj.SNOID))
													 Bot.Character.PetData.ZombieDogs++;
												else if (SnoCacheLookup.hashGargantuan.Contains(tmp_CachedObj.SNOID))
													 Bot.Character.PetData.Gargantuan++;
										  }
									 }

									 //We return regardless if it was summoned by us or not since this object is not anything we want to deal with..
									 tmp_CachedObj.NeedsRemoved=true;
									 continue;
								}
								#endregion

								//Update any SNO Data.
								#region SNO_Cache_Update
								if (tmp_CachedObj.ref_DiaObject==null||tmp_CachedObj.ContainsNullValues())
								{
									 if (!tmp_CachedObj.UpdateData(thisObj, tmp_CachedObj.RAGUID))
										  continue;
								}
								else if (!tmp_CachedObj.IsFinalized)
								{//Finalize this data by recreating it and updating the Sno cache with a new finalized entry, this also clears our all Sno cache dictionaries since we no longer need them!
									 ObjectCache.cacheSnoCollection.FinalizeEntry(tmp_CachedObj.SNOID);
								}
								#endregion

								//Objects with static positions already cached don't need to be updated here.
								if (!tmp_CachedObj.NeedsUpdate) continue;

								//Obstacles -- (Not an actual object we add to targeting.)
								if (tmp_CachedObj.targetType.Value==TargetType.Avoidance||tmp_CachedObj.IsObstacle)
								{
									 #region Obstacles
									 bool bRequireAvoidance=false;
									 bool bTravellingAvoidance=false;

									 CacheObstacle thisObstacle;

									 //Do we have this cached?
									 if (!ObjectCache.Obstacles.TryGetValue(tmp_CachedObj.RAGUID, out thisObstacle))
									 {
										  AvoidanceType AvoidanceType=Funky.AvoidanceType.Unknown;
										  if (tmp_CachedObj.IsAvoidance)
										  {
												AvoidanceType=FindAvoidanceUsingSNOID(tmp_CachedObj.SNOID);
												if (AvoidanceType==AvoidanceType.Unknown)
												{
													 AvoidanceType=FindAvoidanceUsingName(tmp_CachedObj.InternalName);
													 if (AvoidanceType==AvoidanceType.Unknown) continue;
												}
										  }

										  if (tmp_CachedObj.IsAvoidance&&tmp_CachedObj.IsProjectileAvoidance)
										  {//Ranged Projectiles require more than simple bounding points.. so we create it as avoidance zone to cache it with properties.
												//Check for intersection..
												ActorMovement thisMovement=thisObj.Movement;

												Vector2 Direction=thisMovement.DirectionVector;
												Ray R=new Ray(tmp_CachedObj.Position, Direction.ToVector3());
												double Speed;
												//Lookup Cached Speed, or add new entry.
												if (!dictProjectileSpeed.TryGetValue(tmp_CachedObj.SNOID, out Speed))
												{
													 Speed=thisMovement.DesiredSpeed;
													 dictProjectileSpeed.Add(tmp_CachedObj.SNOID, Speed);
												}

												thisObstacle=new CacheAvoidance(tmp_CachedObj, AvoidanceType, R, Speed);
												ObjectCache.Obstacles.Add(thisObstacle);
										  }
										  else if (tmp_CachedObj.IsAvoidance)
										  {
												thisObstacle=new CacheAvoidance(tmp_CachedObj, AvoidanceType);
												ObjectCache.Obstacles.Add(thisObstacle);
										  }
										  else
										  {
												//Obstacles.
												thisObstacle=new CacheServerObject(tmp_CachedObj);
												ObjectCache.Obstacles.Add(thisObstacle);
												continue;
										  }
									 }

									 //Test if this avoidance requires movement now.
									 if (thisObstacle is CacheAvoidance)
									 {
										  //Check last time we attempted avoidance movement (Only if its been at least a second since last time we required it..)
										  //if (DateTime.Now.Subtract(Bot.Combat.LastAvoidanceMovement).TotalMilliseconds<1000)
												//continue;

										  CacheAvoidance thisAvoidance=thisObstacle as CacheAvoidance;

										  if (Bot.Class.IgnoreAvoidance(thisAvoidance.AvoidanceType)) continue;

										  //Only update position of Movement Avoidances!
										  if (thisAvoidance.Obstacletype.Value==ObstacleType.MovingAvoidance)
										  {
												//Blacklisted updates
												if (thisAvoidance.BlacklistRefreshCounter>0&&
													 !thisAvoidance.CheckUpdateForProjectile)
												{
													 thisAvoidance.BlacklistRefreshCounter--;
												}

												bRequireAvoidance=thisAvoidance.UpdateProjectileRayTest(tmp_CachedObj.Position);
												//If we need to avoid, than enable travel avoidance flag also.
												if (bRequireAvoidance) bTravellingAvoidance=true;
										  }
										  else if (thisAvoidance.Position.Distance(Bot.Character.Position)<=thisAvoidance.Radius)
												bRequireAvoidance=true;


										  Bot.Combat.RequiresAvoidance=bRequireAvoidance;
										  Bot.Combat.TravellingAvoidance=bTravellingAvoidance;
										  if (bRequireAvoidance)
												Bot.Combat.TriggeringAvoidances.Add((CacheAvoidance)thisObstacle);
									 }


									 continue;
									 #endregion
								}

								if (tmp_CachedObj.ObjectShouldBeRecreated)
								{
									 //Specific updates
									 if (tmp_CachedObj.Actortype.Value==ActorType.Item)
									 {
										  tmp_CachedObj=new CacheItem(tmp_CachedObj);
									 }
									 else if (tmp_CachedObj.Actortype.Value==ActorType.Unit)
									 {
										  tmp_CachedObj=new CacheUnit(tmp_CachedObj);
									 }
									 else if (tmp_CachedObj.Actortype.Value==ActorType.Gizmo)
									 {

										  if (TargetType.Interactables.HasFlag(tmp_CachedObj.targetType.Value))
												tmp_CachedObj=new CacheInteractable(tmp_CachedObj);
										  else
												tmp_CachedObj=new CacheDestructable(tmp_CachedObj);
									 }
								}

								if (!tmp_CachedObj.UpdateData())
									 continue;

								//Obstacle cache
								if (tmp_CachedObj.Obstacletype.Value!=ObstacleType.None
									 &&(TargetType.ServerObjects.HasFlag(tmp_CachedObj.targetType.Value)))
								{
									 CacheObstacle thisObstacleObj;
									 if (!ObjectCache.Obstacles.TryGetValue(tmp_CachedObj.RAGUID, out thisObstacleObj))
									 {
										  ObjectCache.Obstacles.Add(tmp_CachedObj.RAGUID, new CacheServerObject(tmp_CachedObj));
									 }
									 else if (thisObstacleObj.targetType.Value==TargetType.Unit)
									 {
										  //Since units position requires updating, we update using the CacheObject
										  thisObstacleObj.Position=tmp_CachedObj.Position;
										  ObjectCache.Obstacles[tmp_CachedObj.RAGUID]=thisObstacleObj;
									 }
								}

								//cache it
								if (ObjectCache.Objects.ContainsKey(tmp_CachedObj.RAGUID))
									 ObjectCache.Objects[tmp_CachedObj.RAGUID]=tmp_CachedObj;
								else
									 ObjectCache.Objects.Add(tmp_CachedObj.RAGUID, tmp_CachedObj);


						  }
					 } //End of Loop


					 //Tally up unseen objects.
					 var UnseenObjects=ObjectCache.Objects.Keys.Where<int>(O => !hashDoneThisRactor.Contains(O)).ToList();
					 if (UnseenObjects.Count()>0)
					 {
						  for (int i=0; i<UnseenObjects.Count(); i++)
						  {
								ObjectCache.Objects[UnseenObjects[i]].LoopsUnseen++;
						  }
					 }

					 //Trim our collection every 5th refresh.
					 UpdateLoopCounter++;
					 if (UpdateLoopCounter>4)
					 {
						  UpdateLoopCounter=0;
						  //Now flag any objects not seen for 5 loops. Gold/Globe only 1 loop.
						  foreach (var item in ObjectCache.Objects.Values.Where<CacheObject>(CO => CO.LoopsUnseen>=5||
								(CO.targetType.HasValue&&(CO.targetType.Value==TargetType.Gold||CO.targetType.Value==TargetType.Globe)&&CO.LoopsUnseen>0)))
						  {
								item.NeedsRemoved=true;
						  }
					 }


					 return true;

				}
		  }
	 }
}