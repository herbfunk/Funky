using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;
using System.Collections.Generic;
using Zeta.Internals.SNO;
using Zeta.CommonBot;
using FunkyTrinity.Enums;
using FunkyTrinity.Cache;
using FunkyTrinity.Movement;

namespace FunkyTrinity
{


	 public static partial class Bot
	 {
		  public static class Refresh
		  {
				// Used to force-refresh dia objects at least once every XX milliseconds 
				internal static DateTime lastRefreshedObjects=DateTime.Today;
				private static int RefreshRateMilliseconds=150;
				public static bool ShouldRefreshObjectList
				{
					 get
					 {
						  return DateTime.Now.Subtract(lastRefreshedObjects).TotalMilliseconds>=RefreshRateMilliseconds;
					 }
				}
				///<summary>
				///Tracks the current Level ID
				///</summary>
				private static int LastLevelID=-1;
				private static DateTime LastCheckedLevelID=DateTime.Today;

				///<summary>
				///Used to flag when Init should iterate and remove the objects
				///</summary>
				internal static bool RemovalCheck=false;



				///<summary>
				///Resets/Updates cache and misc vars
				///</summary>
				private static void InitObjectRefresh()
				{
					 //Cache last target only if current target is not avoidance (Movement).
					 if (!Bot.Target.Equals(null)&&Bot.Target.CurrentTarget.targetType.HasValue&&Bot.Target.CurrentTarget.targetType.Value!=TargetType.Avoidance)
						  Bot.Character.LastCachedTarget=Bot.Target.CurrentTarget!=null?Bot.Target.CurrentTarget.Clone():Funky.FakeCacheObject;

					 if (!Bot.Target.Equals(null)&&Bot.Target.CurrentTarget.targetType.HasValue&&Bot.Target.CurrentTarget.targetType.Value==TargetType.Avoidance
						  &&!String.IsNullOrEmpty(Bot.Target.CurrentTarget.InternalName))
					 {
						  string internalname=Bot.Target.CurrentTarget.InternalName;
						  if (internalname.Contains("FleeSpot"))
						  {
								Bot.Combat.LastFleeAction=DateTime.Now;
								Bot.Combat.FleeingLastTarget=true;
						  }
						  else if (internalname.Contains("AvoidanceIntersection")||internalname.Contains("StayPutPoint")||internalname.Contains("SafeAvoid"))
						  {
								Bot.Combat.LastAvoidanceMovement=DateTime.Now;
								Bot.Combat.AvoidanceLastTarget=true;
						  }
					 }
					 else
					 {
						  Bot.Combat.FleeingLastTarget=false;
						  Bot.Combat.AvoidanceLastTarget=false;
					 }

					 Bot.Target.CurrentTarget=null;
					 Bot.Target.CurrentUnitTarget=null;

					 //Kill Loot Radius Update
					 Bot.UpdateKillLootRadiusValues();

					 // Refresh buffs (so we can check for wrath being up to ignore ice balls and anything else like that)
					 Bot.Class.RefreshCurrentBuffs();
					 Bot.Class.RefreshCurrentDebuffs();

					 // Clear forcing close-range priority on mobs after XX period of time
					 if (Bot.Combat.bForceCloseRangeTarget&&DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>Bot.Combat.iMillisecondsForceCloseRange)
					 {
						  Bot.Combat.bForceCloseRangeTarget=false;
					 }

					 // Bunch of variables used throughout
					 Bot.Character.PetData.Reset();
					 // Reset the counters for monsters at various ranges
					 Bot.Combat.Reset();


					 //Check if we should trim our SNO cache..
					 if (DateTime.Now.Subtract(ObjectCache.cacheSnoCollection.lastTrimming).TotalMinutes>3)
						  ObjectCache.cacheSnoCollection.TrimOldUnusedEntries();


					 //Check Level ID changes and clear cache objects.



					 if (ZetaDia.CurrentLevelAreaId!=LastLevelID&&(!ZetaDia.Me.IsInTown))
					 {
						  //Grace Peroid of 5 Seconds before updating.
						  if (DateTime.Now.Subtract(LastCheckedLevelID).TotalSeconds>5)
						  {
								LastCheckedLevelID=DateTime.Now;
								LastLevelID=ZetaDia.CurrentLevelAreaId;

								//Clear our current collection since we changed levels.
								ObjectCache.Objects.Clear();
								ObjectCache.cacheSnoCollection.ClearDictionaryCacheEntries();
								RemovalCheck=false;

								//Reset Playermover Backtrack Positions
								BackTrackCache.cacheMovementGPRs.Clear();

								//Reset Skip Ahead Cache
								SkipAheadCache.ClearCache();

								//This is the only time we should call this. MGP only needs updated every level change!
								Bot.NavigationCache.UpdateSearchGridProvider(true);
						  }
					 }

					 //Check Cached Object Removal flag
					 if (RemovalCheck)
					 {
						  //Remove flagged objects
						  var RemovalObjs=(from objs in ObjectCache.Objects.Values
												 where objs.NeedsRemoved
												 select objs.RAGUID).ToList();

						  foreach (var item in RemovalObjs)
						  {
								CacheObject thisObj=ObjectCache.Objects[item];

								//remove prioritized raguid
								if (Bot.Combat.PrioritizedRAGUIDs.Contains(item))
									 Bot.Combat.PrioritizedRAGUIDs.Remove(item);

								//Blacklist flag check
								if (thisObj.BlacklistFlag!=BlacklistType.None)
									 ObjectCache.AddObjectToBlacklist(thisObj.RAGUID, thisObj.BlacklistFlag);

								ObjectCache.Objects.Remove(thisObj.RAGUID);
						  }

						  RemovalCheck=false;
					 }


					 //Increase counter, clear entries if overdue.
					 ObjectCache.Obstacles.AttemptToClearEntries();

					 //Non-Combat behavior we reset temp blacklist so we don't get killed by "ignored" units..
					 if (Bot.IsInNonCombatBehavior)
					 {
						  ObjectCache.CheckRefreshBlacklists(10);
					 }
				}

				///<summary>
				///Refreshes Cache and updates current target
				///</summary>
				public static void RefreshDiaObjects()
				{
					 //Update Character (just incase it wasnt called before..)
					 Bot.Character.Update(false, true);

					 //Reset key targeting vars always!
					 InitObjectRefresh();

					 //Update object cache collection
					 UpdateCacheObjectCollection();

					 //Update last Refresh Time
					 lastRefreshedObjects=DateTime.Now;


					 //Check avoidance requirement still valid
					 if (FunkyTrinity.Bot.Combat.RequiresAvoidance)
					 {
						  if (FunkyTrinity.Bot.Combat.TriggeringAvoidances.Count==0)
						  {
								if (!FunkyTrinity.Bot.SettingsFunky.EnableFleeingBehavior||FunkyTrinity.Bot.Character.dCurrentHealthPct>0.25d)
									 FunkyTrinity.Bot.Combat.RequiresAvoidance=false;
						  }
						  else
						  {
								if (Bot.NavigationCache.IsMoving)
									 Bot.Combat.RequiresAvoidance=false;
						  }
					 }

					 FunkyTrinity.Bot.ValidObjects=ObjectCache.Objects.Values.Where(o => o.ObjectIsValidForTargeting).ToList();


					 // Still no target, let's end it all!
					 if (!Bot.Target.UpdateTarget())
					 {
						  //clear all prioritzed objects.
						  Bot.Combat.PrioritizedRAGUIDs.Clear();
						  return;
					 }

					 if (!Bot.Target.CurrentTarget.Equals(Bot.Character.LastCachedTarget))
						  TargetMovement.NewTargetResetVars();

					 if (Bot.SettingsFunky.EnableWaitAfterContainers&&Bot.Target.CurrentTarget.targetType==TargetType.Container)
					 {
						  //Herbfunks delay for container loot.
						  Bot.Combat.lastHadContainerAsTarget=DateTime.Now;

						  if (Bot.Target.CurrentTarget.IsResplendantChest)
								Bot.Combat.lastHadRareChestAsTarget=DateTime.Now;
					 }

					 // We're sticking to the same target, so update the target's health cache to check for stucks
					 if (Bot.Target.CurrentTarget.targetType==TargetType.Unit)
					 {
						  CacheUnit thisUnitObj=(CacheUnit)Bot.Target.CurrentTarget;
						  //Used to pause after no targets found.
						  Bot.Combat.lastHadUnitInSights=DateTime.Now;

						  // And record when we last saw any form of elite
						  if (Bot.Target.CurrentTarget.IsBoss||thisUnitObj.IsEliteRareUnique||Bot.Target.CurrentTarget.IsTreasureGoblin)
								Bot.Combat.lastHadEliteUnitInSights=DateTime.Now;
					 }

					 // Record the last time our target changed etc.
					 if (Bot.Target.CurrentTarget!=Bot.Character.LastCachedTarget)
					 {
						  Bot.Combat.dateSincePickedTarget=DateTime.Now;
						  Bot.Combat.LastHealthChange=DateTime.Now;
					 }
				}

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
						  if (ObjectCache.IsRAGUIDBlacklisted(tmp_raGUID)) continue;
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
									 if (ObjectCache.IsSNOIDBlacklisted(tmp_SNOID)&&!CacheIDLookup.hashSummonedPets.Contains(tmp_SNOID)) continue;


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
												Logging.WriteVerbose("[Funky] Safely handled exception getting summoned-by info ["+tmp_CachedObj.SNOID.ToString()+"]");
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
										  else if (Bot.Class.AC==ActorClass.DemonHunter&&CacheIDLookup.hashDHPets.Contains(tmp_CachedObj.SNOID))
												Bot.Character.PetData.DemonHunterPet++;
										  else if (Bot.Class.AC==ActorClass.WitchDoctor)
										  {
												if (CacheIDLookup.hashZombie.Contains(tmp_CachedObj.SNOID))
													 Bot.Character.PetData.ZombieDogs++;
												else if (CacheIDLookup.hashGargantuan.Contains(tmp_CachedObj.SNOID))
													 Bot.Character.PetData.Gargantuan++;
										  }
										  else if (Bot.Class.AC==Zeta.Internals.Actors.ActorClass.Wizard)
										  {
												//only count when range is within 45f (so we can summon a new one)
												if (CacheIDLookup.hashWizHydras.Contains(tmp_CachedObj.SNOID)&&tmp_CachedObj.CentreDistance<=45f)
													 Bot.Character.PetData.WizardHydra++;
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
								if (tmp_CachedObj.targetType.Value==TargetType.Avoidance||tmp_CachedObj.IsObstacle||tmp_CachedObj.HandleAsAvoidanceObject)
								{
									 #region Obstacles
									 bool bRequireAvoidance=false;
									 bool bTravellingAvoidance=false;

									 CacheObstacle thisObstacle;

									 //Do we have this cached?
									 if (!ObjectCache.Obstacles.TryGetValue(tmp_CachedObj.RAGUID, out thisObstacle))
									 {
										  AvoidanceType AvoidanceType=AvoidanceType.Unknown;
										  if (tmp_CachedObj.IsAvoidance)
										  {
												AvoidanceType=CacheIDLookup.FindAvoidanceUsingSNOID(tmp_CachedObj.SNOID);
												if (AvoidanceType==AvoidanceType.Unknown)
												{
													 AvoidanceType=CacheIDLookup.FindAvoidanceUsingName(tmp_CachedObj.InternalName);
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
												if (!ObjectCache.dictProjectileSpeed.TryGetValue(tmp_CachedObj.SNOID, out Speed))
												{
													 Speed=thisMovement.DesiredSpeed;
													 ObjectCache.dictProjectileSpeed.Add(tmp_CachedObj.SNOID, Speed);
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

										  if (Bot.IgnoreAvoidance(thisAvoidance.AvoidanceType)) continue;

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
										  else
										  {
												if (thisObstacle.CentreDistance<50f)
													 Bot.Combat.NearbyAvoidances.Add(thisObstacle.RAGUID);

												if (thisAvoidance.Position.Distance(Bot.Character.Position)<=thisAvoidance.Radius)
													 bRequireAvoidance=true;
										  }

										  Bot.Combat.RequiresAvoidance=bRequireAvoidance;
										  Bot.Combat.TravellingAvoidance=bTravellingAvoidance;
										  if (bRequireAvoidance)
												Bot.Combat.TriggeringAvoidances.Add((CacheAvoidance)thisObstacle);
									 }
									 else
									 {
										  //Add this server object to cell weighting in MGP
										  //MGP.AddCellWeightingObstacle(thisObstacle.SNOID, thisObstacle.CollisionRadius.Value);

										  //Add nearby objects to our collection (used in navblock/obstaclecheck methods to reduce queries)
										  if (thisObstacle.CentreDistance<25f)
												Bot.Combat.NearbyObstacleObjects.Add((CacheServerObject)thisObstacle);
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
										  CacheServerObject newobj =new CacheServerObject(tmp_CachedObj);
										  ObjectCache.Obstacles.Add(tmp_CachedObj.RAGUID, newobj);

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
												ObjectCache.Obstacles[tmp_CachedObj.RAGUID]=thisObstacleObj;
										  }
										  if (thisObstacleObj.CentreDistance<=25f)
												Bot.Combat.NearbyObstacleObjects.Add((CacheServerObject)thisObstacleObj);
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