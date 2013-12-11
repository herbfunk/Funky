using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using FunkyBot.Movement;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.Internals.SNO;
using FunkyBot.Cache.Collections;

namespace FunkyBot.Cache
{

	 ///<summary>
	 ///Contains Collections for all the cached objects being tracked.
	 ///</summary>
	 public static class ObjectCache
	 {
		  internal static CacheObject FakeCacheObject;

		  ///<summary>
		  ///Cached Objects.
		  ///</summary>
		  public static ObjectCollection Objects = new ObjectCollection();

		  ///<summary>
		  ///Obstacles related to either avoidances or navigational blocks.
		  ///</summary>
		  public static ObstacleCollection Obstacles = new ObstacleCollection();

		  ///<summary>
		  ///Cached Sno Data.
		  ///</summary>
		  public static SnoCollection cacheSnoCollection = new SnoCollection();

		  internal static bool CheckTargetTypeFlag(TargetType property, TargetType flag)
		  {
				return (property&flag)!=0;
		  }

		  ///<summary>
		  ///Adds/Updates CacheObjects inside collection by Iteration of RactorList
		  ///This is the method that caches all live data about an object!
		  ///</summary>
		  internal static bool UpdateCacheObjectCollection()
		  {
				HashSet<int> hashDoneThisRactor=new HashSet<int>();
				using (ZetaDia.Memory.AcquireFrame(true))
				{
					 if (!ZetaDia.IsInGame||ZetaDia.IsLoadingWorld||!ZetaDia.Me.IsValid) return false;

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
						  if (BlacklistCache.IsRAGUIDBlacklisted(tmp_raGUID)) continue;
						  CacheObject tmp_CachedObj;

						  if (!Objects.TryGetValue(tmp_raGUID, out tmp_CachedObj))
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
								if (BlacklistCache.IsSNOIDBlacklisted(tmp_SNOID)&&!CacheIDLookup.hashSummonedPets.Contains(tmp_SNOID)) continue;


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
									 } catch (Exception ex)
									 {
										  Logging.WriteVerbose("[Funky] Safely handled exception getting summoned-by info ["+tmp_CachedObj.SNOID.ToString(CultureInfo.InvariantCulture)+"]");
										  Logging.WriteDiagnostic(ex.ToString());
										  continue;
									 }
								}

								//See if this summoned unit was summoned by the bot.
								if (Bot.Character.Data.iMyDynamicID==tmp_CachedObj.SummonerID.Value)
								{
									 //Now modify the player data pets count..
									 if (Bot.Character.Class.AC==ActorClass.Monk)
										  Bot.Character.Data.PetData.MysticAlly++;
									 else if (Bot.Character.Class.AC==ActorClass.DemonHunter)
									 {
										  if (CacheIDLookup.hashDHPets.Contains(tmp_CachedObj.SNOID))
												Bot.Character.Data.PetData.DemonHunterPet++;
										  else if (CacheIDLookup.hashDHSpikeTraps.Contains(tmp_CachedObj.SNOID)&&tmp_CachedObj.CentreDistance<=50f)
												Bot.Character.Data.PetData.DemonHunterSpikeTraps++;
									 }
									 else if (Bot.Character.Class.AC==ActorClass.WitchDoctor)
									 {
										  if (CacheIDLookup.hashZombie.Contains(tmp_CachedObj.SNOID))
												Bot.Character.Data.PetData.ZombieDogs++;
										  else if (CacheIDLookup.hashGargantuan.Contains(tmp_CachedObj.SNOID))
												Bot.Character.Data.PetData.Gargantuan++;
									 }
									 else if (Bot.Character.Class.AC==ActorClass.Wizard)
									 {
										  //only count when range is within 45f (so we can summon a new one)
										  if (CacheIDLookup.hashWizHydras.Contains(tmp_CachedObj.SNOID)&&tmp_CachedObj.CentreDistance<=45f)
												Bot.Character.Data.PetData.WizardHydra++;
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
								cacheSnoCollection.FinalizeEntry(tmp_CachedObj.SNOID);
						  }
						  #endregion

						 //Special Cache for Interactable Server Objects
						 if (CheckTargetTypeFlag(tmp_CachedObj.targetType.Value, TargetType.ServerInteractable))
						 {
							 if (!Bot.Game.Profile.InteractableObjectCache.ContainsKey(tmp_CachedObj.RAGUID))
							 {
								 Bot.Game.Profile.InteractableObjectCache.Add(tmp_CachedObj.RAGUID, tmp_CachedObj);
							 }

							 //Do not add to main cache!
							 continue;
						 }

						  //Objects with static positions already cached don't need to be updated here.
						  if (!tmp_CachedObj.NeedsUpdate) continue;

						  //Obstacles -- (Not an actual object we add to targeting.)
						  if (CheckTargetTypeFlag(tmp_CachedObj.targetType.Value, TargetType.Avoidance)||tmp_CachedObj.IsObstacle||tmp_CachedObj.HandleAsAvoidanceObject)
						  {
								#region Obstacles

								CacheObstacle thisObstacle;
								//Do we have this cached?
								if (!Obstacles.TryGetValue(tmp_CachedObj.RAGUID, out thisObstacle))
								{
									 AvoidanceType AvoidanceType=AvoidanceType.None;
									 if (tmp_CachedObj.IsAvoidance)
									 {
										  AvoidanceType=AvoidanceCache.FindAvoidanceUsingSNOID(tmp_CachedObj.SNOID);
										  if (AvoidanceType==AvoidanceType.None)
										  {
												AvoidanceType=AvoidanceCache.FindAvoidanceUsingName(tmp_CachedObj.InternalName);
												if (AvoidanceType==AvoidanceType.None) continue;
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
										  Obstacles.Add(thisObstacle);
									 }
									 else if (tmp_CachedObj.IsAvoidance)
									 {

										  //Poison Gas Can Be Friendly...
										  if (AvoidanceType==AvoidanceType.PoisonGas)
										  {
												int TeamID=0;
												try
												{
													 TeamID=thisObj.CommonData.GetAttribute<int>(ActorAttributeType.TeamID);
												} catch
												{
													 if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Execption))
														  Logger.Write(LogLevel.Execption, "Failed to retrieve TeamID attribute for object {0}", thisObstacle.InternalName);
												}

												//ID of 1 means its non-hostile! (-1?) 2??
												//if (TeamID==1||TeamID==-1)
												if (TeamID!=10)
												{
													 //Logger.Write(LogLevel.None, "Ignoring Avoidance {0} due to Friendly TeamID match!", tmp_CachedObj.InternalName);
													 BlacklistCache.AddObjectToBlacklist(tmp_CachedObj.RAGUID, BlacklistType.Permanent);
													 continue;
												}
										  }


										  thisObstacle=new CacheAvoidance(tmp_CachedObj, AvoidanceType);
										  Obstacles.Add(thisObstacle);
									 }
									 else
									 {
										  //Obstacles.
										  thisObstacle=new CacheServerObject(tmp_CachedObj);
										  Obstacles.Add(thisObstacle);
									 }
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

									 if (CheckTargetTypeFlag(tmp_CachedObj.targetType.Value, TargetType.Interactables))
										  tmp_CachedObj=new CacheInteractable(tmp_CachedObj);
									 else
										  tmp_CachedObj=new CacheDestructable(tmp_CachedObj);
								}

								//Update Properties
								tmp_CachedObj.UpdateProperties();
						  }

						  if (!tmp_CachedObj.UpdateData())
								continue;

						  //Obstacle cache
						  if (tmp_CachedObj.Obstacletype.Value!=ObstacleType.None
								&&(CheckTargetTypeFlag(tmp_CachedObj.targetType.Value, TargetType.ServerObjects)))
						  {
								CacheObstacle thisObstacleObj;

								if (!Obstacles.TryGetValue(tmp_CachedObj.RAGUID, out thisObstacleObj))
								{
									 CacheServerObject newobj=new CacheServerObject(tmp_CachedObj);
									 Obstacles.Add(tmp_CachedObj.RAGUID, newobj);

									 //Add nearby objects to our collection (used in navblock/obstaclecheck methods to reduce queries)
									 if (CacheIDLookup.hashSNONavigationObstacles.Contains(newobj.SNOID))
										  Navigation.MGP.AddCellWeightingObstacle(newobj.SNOID, newobj.Radius);
								}
								else
								{
									 if (thisObstacleObj.targetType.Value==TargetType.Unit)
									 {
										  //Since units position requires updating, we update using the CacheObject
										  thisObstacleObj.Position=tmp_CachedObj.Position;
										  Obstacles[tmp_CachedObj.RAGUID]=thisObstacleObj;
									 }
								}
						  }

						  //cache it
						  if (Objects.ContainsKey(tmp_CachedObj.RAGUID))
								Objects[tmp_CachedObj.RAGUID]=tmp_CachedObj;
						  else
								Objects.Add(tmp_CachedObj.RAGUID, tmp_CachedObj);



					 } //End of Loop

				}// End of Framelock

				//Tally up unseen objects.
				var UnseenObjects=Objects.Keys.Where(O => !hashDoneThisRactor.Contains(O)).ToList();
				if (UnseenObjects.Any())
				{
					 for (int i=0; i<UnseenObjects.Count(); i++)
					 {
						  Objects[UnseenObjects[i]].LoopsUnseen++;
					 }
				}

				//Trim our collection every 5th refresh.
				UpdateLoopCounter++;
				if (UpdateLoopCounter>4)
				{
					 UpdateLoopCounter=0;
					 //Now flag any objects not seen for 5 loops. Gold/Globe only 1 loop.
					 foreach (var item in Objects.Values.Where(CO => 
						 (CO.LoopsUnseen >= 5 || //5 loops max.. 
						 (CO.targetType.HasValue && (CheckTargetTypeFlag(CO.targetType.Value, TargetType.Gold | TargetType.Globe)) && CO.LoopsUnseen > 0)))) //gold/globe only 1 loop!
					 {
						  item.NeedsRemoved=true;
					 }
				}


				return true;
		  }
		  //Trimming/Update Vars
		  private static int UpdateLoopCounter;


		  #region SNO Cache Dictionaries
		  internal static Dictionary<int, ActorType?> dictActorType=new Dictionary<int, ActorType?>();
		  internal static Dictionary<int, TargetType?> dictTargetType=new Dictionary<int, TargetType?>();
		  internal static Dictionary<int, MonsterSize?> dictMonstersize=new Dictionary<int, MonsterSize?>();
		  internal static Dictionary<int, MonsterType?> dictMonstertype=new Dictionary<int, MonsterType?>();
		  internal static Dictionary<int, float?> dictCollisionRadius=new Dictionary<int, float?>();
		  internal static Dictionary<int, String> dictInternalName=new Dictionary<int, String>();
		  internal static Dictionary<int, ObstacleType?> dictObstacleType=new Dictionary<int, ObstacleType?>();
		  internal static Dictionary<int, float?> dictActorSphereRadius=new Dictionary<int, float?>();
		  internal static Dictionary<int, bool?> dictCanBurrow=new Dictionary<int, bool?>();
		  internal static Dictionary<int, bool?> dictGrantsNoXp=new Dictionary<int, bool?>();
		  internal static Dictionary<int, bool?> dictDropsNoLoot=new Dictionary<int, bool?>();
		  internal static Dictionary<int, GizmoType?> dictGizmoType=new Dictionary<int, GizmoType?>();
		  internal static Dictionary<int, bool?> dictIsBarricade=new Dictionary<int, bool?>();
		  internal static Dictionary<int, double> dictProjectileSpeed=new Dictionary<int, double>();
		  #endregion



	 }

}