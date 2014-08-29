using System;
using System.Collections.Generic;
using System.Linq;
using fBaseXtensions.Cache.External.Debugging;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Cache.Internal.Avoidance;
using fBaseXtensions.Cache.Internal.Blacklist;
using fBaseXtensions.Cache.Internal.Collections;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;
using ObstacleType = fBaseXtensions.Cache.Internal.Enums.ObstacleType;
using TargetType = fBaseXtensions.Cache.Internal.Enums.TargetType;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.External.Objects;

namespace fBaseXtensions.Cache.Internal
{

	///<summary>
	///Contains Collections for all the cached objects being tracked.
	///</summary>
	public static class ObjectCache
	{
		internal static CacheObject FakeCacheObject;
		//internal static SnoIDCache SNOCache; 

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

		//Used to log non-cached SNOs
		internal static DebugData DebuggingData;

		internal static Dictionary<int, CacheObject> InteractableObjectCache = new Dictionary<int, CacheObject>();
	

		internal static bool CheckFlag(TargetType property, TargetType flag)
		{
			return (property & flag) != 0;
		}
		internal static bool CheckFlag(UnitFlags property, UnitFlags flag)
		{
			return (property & flag) != 0;
		}
		internal static bool CheckFlag(TargetProperties property, TargetProperties flag)
		{
			return (property & flag) != 0;
		}
		internal static bool CheckFlag(ObstacleType property, ObstacleType flag)
		{
			return (property & flag) != 0;
		}
		internal static bool CheckFlag(SkillExecutionFlags property, SkillExecutionFlags flag)
		{
			return (property & flag) != 0;
		}
		internal static bool CheckFlag(GizmoTargetTypes property, GizmoTargetTypes flag)
		{
			return (property & flag) != 0;
		}
		//

		///<summary>
		///Adds/Updates CacheObjects inside collection by Iteration of RactorList
		///This is the method that caches all live data about an object!
		///</summary>
		internal static bool UpdateCacheObjectCollection()
		{
			HashSet<int> hashDoneThisRactor = new HashSet<int>();
			using (ZetaDia.Memory.AcquireFrame(true))
			{
				if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || !ZetaDia.Me.IsValid) return false;

				foreach (Actor thisActor in ZetaDia.Actors.RActorList)
				{
					int tmp_raGUID;
					DiaObject thisObj;

					if (!thisActor.IsValid) continue;
					//Convert to DiaObject
					thisObj = (DiaObject)thisActor;
					tmp_raGUID = thisObj.RActorGuid;

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
							tmp_SNOID = thisObj.ActorSNO;
						}
						catch (Exception ex) 
						{
							Logger.Write(LogLevel.Cache, "Safely handled getting SNO for {0}", tmp_raGUID);
							//Logger.DBLog.InfoFormat("Failure to get SNO from object! RaGUID: {0}", tmp_raGUID); 
							continue;
						}
						#endregion


						//check our SNO blacklist (exclude pets?)
						if (BlacklistCache.IsSNOIDBlacklisted(tmp_SNOID) && !TheCache.ObjectIDCache.UnitPetEntries.ContainsKey(tmp_SNOID)) continue;


						#region Position
						try
						{
							tmp_position = thisObj.Position;
						}
						catch (Exception ex) 
						{
							Logger.Write(LogLevel.Cache, "Safely handled getting Position for {0}", tmp_raGUID);
							continue;
						}

						#endregion

						#region AcdGUID
						try
						{
							tmp_acdguid = thisObj.ACDGuid;
						}
						catch (Exception ex) 
						{
							Logger.Write(LogLevel.Cache, "Safely handled getting ACDGuid for {0}", tmp_raGUID);
							continue;
						}

						#endregion


						tmp_CachedObj = new CacheObject(tmp_SNOID, tmp_raGUID, tmp_acdguid, tmp_position);
					}
					else
						//Reset unseen var
						tmp_CachedObj.LoopsUnseen = 0;


					//Validate
					try
					{
						if (thisObj.CommonData == null || thisObj.CommonData.ACDGuid != thisObj.ACDGuid) continue;
					}
					catch (Exception ex)
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
								tmp_CachedObj.SummonerID = thisObj.CommonData.GetAttribute<int>(ActorAttributeType.SummonedByACDID);
							}
							catch (Exception ex)
							{
								//Logger.DBLog.InfoFormat("[Funky] Safely handled exception getting summoned-by info [" + tmp_CachedObj.SNOID.ToString(CultureInfo.InvariantCulture) + "]");
								//Logger.DBLog.DebugFormat(ex.ToString());
								continue;
							}
						}

						

						//See if this summoned unit was summoned by the bot.
						if (FunkyGame.Hero.iMyDynamicID == tmp_CachedObj.SummonerID.Value)
						{
							PetTypes PetType = TheCache.ObjectIDCache.UnitPetEntries[tmp_CachedObj.SNOID];

							//Now modify the player data pets count..
							if (FunkyGame.CurrentActorClass == ActorClass.Monk)
								FunkyGame.Targeting.Cache.Environment.HeroPets.MysticAlly++;
							else if (FunkyGame.CurrentActorClass == ActorClass.DemonHunter)
							{
								if (PetType== PetTypes.DEMONHUNTER_Pet)
									FunkyGame.Targeting.Cache.Environment.HeroPets.DemonHunterPet++;
								else if (PetType == PetTypes.DEMONHUNTER_SpikeTrap && tmp_CachedObj.CentreDistance <= 50f)
									FunkyGame.Targeting.Cache.Environment.HeroPets.DemonHunterSpikeTraps++;
								else if (PetType == PetTypes.DEMONHUNTER_Sentry && tmp_CachedObj.CentreDistance <= 60f)
									FunkyGame.Targeting.Cache.Environment.HeroPets.DemonHunterSentry++;
							}
							else if (FunkyGame.CurrentActorClass == ActorClass.Witchdoctor)
							{
								if (PetType == PetTypes.WITCHDOCTOR_ZombieDogs)
									FunkyGame.Targeting.Cache.Environment.HeroPets.ZombieDogs++;
								else if (PetType == PetTypes.WITCHDOCTOR_Gargantuan)
									FunkyGame.Targeting.Cache.Environment.HeroPets.Gargantuan++;
								else if (PetType == PetTypes.WITCHDOCTOR_Fetish)
									FunkyGame.Targeting.Cache.Environment.HeroPets.WitchdoctorFetish++;
							}
							else if (FunkyGame.CurrentActorClass == ActorClass.Wizard)
							{
								//only count when range is within 45f (so we can summon a new one)
								if (PetType == PetTypes.WIZARD_Hydra && tmp_CachedObj.CentreDistance <= 50f)
									FunkyGame.Targeting.Cache.Environment.HeroPets.WizardHydra++;
							}
						}

						//We return regardless if it was summoned by us or not since this object is not anything we want to deal with..
						tmp_CachedObj.NeedsRemoved = true;
						continue;
					}
					#endregion

					//Update any SNO Data.
					#region SNO_Cache_Update
					if (tmp_CachedObj.ref_DiaObject == null || tmp_CachedObj.ContainsNullValues())
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
					if (CheckFlag(tmp_CachedObj.targetType.Value, TargetType.ServerInteractable))
					{
						if (!InteractableObjectCache.ContainsKey(tmp_CachedObj.RAGUID))
						{
							InteractableObjectCache.Add(tmp_CachedObj.RAGUID, tmp_CachedObj);

							//Adventure Mode -- Rifting we add Exit to LOS movement!
							//if (FunkyGame.AdventureMode && FunkyGame.Bounty.IsInRiftWorld && FunkyBaseExtension.Settings.AdventureMode.EnableAdventuringMode)
							//{
							//	if (tmp_CachedObj.InternalName.Contains("Exit"))
							//	{
							//		int index = FunkyGame.Bounty.CurrentBountyMapMarkers.Count;
							//		FunkyGame.Bounty.CurrentBountyMapMarkers.Add(index, new BountyCache.BountyMapMarker(tmp_CachedObj.Position, FunkyGame.Hero.CurrentWorldDynamicID, index));
							//	}
							//}
						}

						//Do not add to main cache!
						continue;
					}

					//Objects with static positions already cached don't need to be updated here.
					if (!tmp_CachedObj.NeedsUpdate) continue;

					//Obstacles -- (Not an actual object we add to targeting.)
					if (CheckFlag(tmp_CachedObj.targetType.Value, TargetType.Avoidance) || tmp_CachedObj.IsObstacle || tmp_CachedObj.HandleAsAvoidanceObject)
					{
						#region Obstacles

						CacheObstacle thisObstacle;
						//Do we have this cached?
						if (!Obstacles.TryGetValue(tmp_CachedObj.RAGUID, out thisObstacle))
						{
							AvoidanceType AvoidanceType = AvoidanceType.None;
							if (tmp_CachedObj.IsAvoidance)
							{
								AvoidanceType = AvoidanceCache.FindAvoidanceUsingSNOID(tmp_CachedObj.SNOID);
								if (AvoidanceType == AvoidanceType.None)
								{
									AvoidanceType = AvoidanceCache.FindAvoidanceUsingName(tmp_CachedObj.InternalName);
									if (AvoidanceType == AvoidanceType.None) continue;
								}
							}

							if (tmp_CachedObj.IsAvoidance && tmp_CachedObj.IsProjectileAvoidance)
							{//Ranged Projectiles require more than simple bounding points.. so we create it as avoidance zone to cache it with properties.
								//Check for intersection..
								try
								{
									ActorMovement thisMovement = thisObj.Movement;
									Vector2 Direction = thisMovement.DirectionVector;
									Ray R = new Ray(tmp_CachedObj.Position, Direction.ToVector3());
									double Speed;
									//Lookup Cached Speed, or add new entry.
									if (!dictProjectileSpeed.TryGetValue(tmp_CachedObj.SNOID, out Speed))
									{
										Speed = thisMovement.DesiredSpeed;
										dictProjectileSpeed.Add(tmp_CachedObj.SNOID, Speed);
									}

									thisObstacle = new CacheAvoidance(tmp_CachedObj, AvoidanceType, R, Speed);
									Obstacles.Add(thisObstacle);
								}
								catch
								{
									
										Logger.Write(LogLevel.Cache, "Failed to create projectile avoidance with rotation and speed. {0}", tmp_CachedObj.InternalName);
								}
							}
							else if (tmp_CachedObj.IsAvoidance)
							{
								//Poison Gas Can Be Friendly...
								if (AvoidanceType == AvoidanceType.PoisonGas)
								{
									int TeamID = 0;
									try
									{
										TeamID = thisObj.CommonData.GetAttribute<int>(ActorAttributeType.TeamID);
									}
									catch
									{
										
											Logger.Write(LogLevel.Cache, "Failed to retrieve TeamID attribute for object {0}", thisObstacle.InternalName);
									}

									//ID of 1 means its non-hostile! (-1?) 2??
									//if (TeamID==1||TeamID==-1)
									if (TeamID != 10)
									{
										//Logger.Write(LogLevel.None, "Ignoring Avoidance {0} due to Friendly TeamID match!", tmp_CachedObj.InternalName);
										BlacklistCache.AddObjectToBlacklist(tmp_CachedObj.RAGUID, BlacklistType.Permanent);
										continue;
									}
								}


								thisObstacle = new CacheAvoidance(tmp_CachedObj, AvoidanceType);
								Obstacles.Add(thisObstacle);
							}
							else
							{
								//Obstacles.
								thisObstacle = new CacheServerObject(tmp_CachedObj);
								Obstacles.Add(thisObstacle);
							}
						}

						continue;
						#endregion
					}

					if (tmp_CachedObj.ObjectShouldBeRecreated)
					{//This is where we create the real object after its done with SNO Update.

						//Specific updates
						if (tmp_CachedObj.Actortype.Value == ActorType.Item)
						{
							tmp_CachedObj = new CacheItem(tmp_CachedObj);
						}
						else if (tmp_CachedObj.Actortype.Value == ActorType.Monster)
						{
							tmp_CachedObj = new CacheUnit(tmp_CachedObj);
						}
						else if (tmp_CachedObj.Actortype.Value == ActorType.Gizmo)
						{

							if (CheckFlag(tmp_CachedObj.targetType.Value, TargetType.Interactables))
								tmp_CachedObj = new CacheInteractable(tmp_CachedObj);
							else
								tmp_CachedObj = new CacheDestructable(tmp_CachedObj);
						}

						//Update Properties (currently only for units)

						try
						{
							tmp_CachedObj.UpdateProperties();
						}
						catch 
						{
							Logger.Write(LogLevel.Cache,"Failed to update properties for {0}", tmp_CachedObj.DebugStringSimple);
						}
					}

					if (!tmp_CachedObj.UpdateData())
					{
						//Logger.Write(LogLevel.Cache, "Update Failed for {0}", tmp_CachedObj.DebugStringSimple);

						if (!tmp_CachedObj.IsStillValid())
							tmp_CachedObj.NeedsRemoved = true;

						continue;
					}

					//Obstacle cache
					if (tmp_CachedObj.Obstacletype.Value != ObstacleType.None
						  && (CheckFlag(tmp_CachedObj.targetType.Value, TargetType.ServerObjects)))
					{
						CacheObstacle thisObstacleObj;

						if (!Obstacles.TryGetValue(tmp_CachedObj.RAGUID, out thisObstacleObj))
						{
							CacheServerObject newobj = new CacheServerObject(tmp_CachedObj);
							Obstacles.Add(tmp_CachedObj.RAGUID, newobj);

							//Add nearby objects to our collection (used in navblock/obstaclecheck methods to reduce queries)
							if (CacheIDLookup.hashSNONavigationObstacles.Contains(newobj.SNOID))
								Navigation.Navigation.MGP.AddCellWeightingObstacle(newobj.SNOID, newobj.Radius);
						}
						else
						{
							if (thisObstacleObj.targetType.Value == TargetType.Unit)
							{
								//Since units position requires updating, we update using the CacheObject
								thisObstacleObj.Position = tmp_CachedObj.Position;
								Obstacles[tmp_CachedObj.RAGUID] = thisObstacleObj;
							}
						}
					}

					//cache it
					if (Objects.ContainsKey(tmp_CachedObj.RAGUID))
						Objects[tmp_CachedObj.RAGUID] = tmp_CachedObj;
					else
						Objects.Add(tmp_CachedObj.RAGUID, tmp_CachedObj);



				} //End of Loop

			}// End of Framelock

			//Tally up unseen objects.
			var UnseenObjects = Objects.Keys.Where(O => !hashDoneThisRactor.Contains(O)).ToList();
			if (UnseenObjects.Any())
			{
				for (int i = 0; i < UnseenObjects.Count(); i++)
				{
					Objects[UnseenObjects[i]].LoopsUnseen++;
				}
			}

			//Trim our collection every 5th refresh.
			UpdateLoopCounter++;
			if (UpdateLoopCounter > 4)
			{
				UpdateLoopCounter = 0;
				//Now flag any objects not seen for 5 loops. Gold/Globe only 1 loop.
				foreach (var item in Objects.Values.Where(CO =>
					(CO.LoopsUnseen >= 5 || //5 loops max.. 
					(CO.targetType.HasValue && (CheckFlag(CO.targetType.Value, TargetType.Gold | TargetType.Globe)) && CO.LoopsUnseen > 0)))) //gold/globe only 1 loop!
				{
					item.NeedsRemoved = true;
				}
			}


			return true;
		}
		//Trimming/Update Vars
		private static int UpdateLoopCounter;

		///<summary>
		///Used to flag when Init should iterate and remove the objects
		///</summary>
		internal static bool RemovalCheck = false;
		internal static void CheckForCacheRemoval()
		{
			//Check Cached Object Removal flag
			if (RemovalCheck)
			{
				//Remove flagged objects
				var RemovalObjs = (from objs in ObjectCache.Objects.Values
								   where objs.NeedsRemoved
								   select objs.RAGUID).ToList();

				foreach (var item in RemovalObjs)
				{
					CacheObject thisObj = ObjectCache.Objects[item];

					//remove prioritized raguid
					if (FunkyGame.Navigation.PrioritizedRAGUIDs.Contains(item))
						FunkyGame.Navigation.PrioritizedRAGUIDs.Remove(item);

					//Blacklist flag check
					if (thisObj.BlacklistFlag != BlacklistType.None)
						BlacklistCache.AddObjectToBlacklist(thisObj.RAGUID, thisObj.BlacklistFlag);

					ObjectCache.Objects.Remove(thisObj.RAGUID);
				}

				RemovalCheck = false;
			}
		}


		#region SNO Cache Dictionaries
		internal static Dictionary<int, GizmoTargetTypes?> dictGizmoTargetTypes = new Dictionary<int, GizmoTargetTypes?>();
		internal static Dictionary<int, UnitFlags?> dictUnitFlags = new Dictionary<int, UnitFlags?>();
		internal static Dictionary<int, PluginDroppedItemTypes?> dictBaseItemTypes = new Dictionary<int, PluginDroppedItemTypes?>();
		internal static Dictionary<int, ActorType?> dictActorType = new Dictionary<int, ActorType?>();
		internal static Dictionary<int, TargetType?> dictTargetType = new Dictionary<int, TargetType?>();
		internal static Dictionary<int, MonsterSize?> dictMonstersize = new Dictionary<int, MonsterSize?>();
		internal static Dictionary<int, MonsterType?> dictMonstertype = new Dictionary<int, MonsterType?>();
		internal static Dictionary<int, float?> dictCollisionRadius = new Dictionary<int, float?>();
		internal static Dictionary<int, String> dictInternalName = new Dictionary<int, String>();
		internal static Dictionary<int, ObstacleType?> dictObstacleType = new Dictionary<int, ObstacleType?>();
		internal static Dictionary<int, float?> dictActorSphereRadius = new Dictionary<int, float?>();
		internal static Dictionary<int, bool?> dictCanBurrow = new Dictionary<int, bool?>();
		internal static Dictionary<int, bool?> dictGrantsNoXp = new Dictionary<int, bool?>();
		internal static Dictionary<int, bool?> dictDropsNoLoot = new Dictionary<int, bool?>();
		internal static Dictionary<int, GizmoType?> dictGizmoType = new Dictionary<int, GizmoType?>();
		internal static Dictionary<int, bool?> dictIsBarricade = new Dictionary<int, bool?>();
		internal static Dictionary<int, double> dictProjectileSpeed = new Dictionary<int, double>();
		#endregion



	}

}