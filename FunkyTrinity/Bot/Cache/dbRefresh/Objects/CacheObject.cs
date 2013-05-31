using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.Internals.Actors.Gizmos;
using Zeta.CommonBot;
using Zeta.Internals.SNO;
using System.Collections.Generic;

namespace FunkyTrinity
{
	 public partial class Funky
	 {

		  public class CacheObject : CachedSNOEntry, IComparable
		  {

				#region Constructors
				public CacheObject(int sno, int raguid, Vector3 pos)
					 : base(sno)
				{
					 this.RAGUID=raguid;
					 this.position_=pos;
					 this.radius_=0f;
				}

				public CacheObject(int sno, int raguid, int acdguid, Vector3 position, string Name=null)
					 : base(sno)
				{
					 this.RAGUID=raguid;
					 this.NeedsUpdate=true;
					 this.removal_=false;
					 this.BlacklistFlag=BlacklistType.None;
					 this.AcdGuid=acdguid;
					 this.radius_=0f;
					 this.position_=position;
					 this.RequiresLOSCheck=!(base.IgnoresLOSCheck); //require a LOS check initally on a new object!
					 this.LastLOSCheck=DateTime.Today;
					 this.PrioritizedDate=DateTime.Today;
                     this.PriorityCounter = 0;
					 this.LOSV3=vNullLocation;

					 //Keep track of each unique RaGuid that is created and uses this SNO during each level.
					 //if (!UsedByRaGuids.Contains(RAGUID)) UsedByRaGuids.Add(RAGUID);
				}
				///<summary>
				///Used to create objects to use as temp targeting
				///</summary>
				public CacheObject(Vector3 thisposition, TargetType thisobjecttype=TargetType.None, double thisweight=0, string name=null, float thisradius=0f, int thisractorguid=-1, int thissno=0)
					 : base(thissno)
				{
					 this.RAGUID=thisractorguid;
					 this.position_=thisposition;
					 this.weight_=thisweight;
					 this.radius_=thisradius;
					 this.BlacklistFlag=BlacklistType.None;
					 this.targetType=thisobjecttype;
					 this.InternalName=name;

				}
				///<summary>
				///Used to recreate from temp into obstacle object.
				///</summary>
				public CacheObject(CacheObject parent)
					 : base(parent)
				{
					 this.AcdGuid=parent.AcdGuid;
					 this.BlacklistFlag=parent.BlacklistFlag;
					 this.BlacklistLoops=parent.BlacklistLoops;
					 this.gprect_=parent.gprect_;
					 this.InteractionAttempts=parent.InteractionAttempts;
					 this.LastLOSCheck=parent.LastLOSCheck;
					 this.LoopsUnseen=parent.LoopsUnseen;
					 this.LOSV3=parent.LOSV3;
					 this.NeedsRemoved=parent.NeedsRemoved;
					 this.NeedsUpdate=parent.NeedsUpdate;
					 this.PrioritizedDate=parent.PrioritizedDate;
                     this.PriorityCounter = parent.PriorityCounter;
					 this.position_=parent.Position;
					 this.radius_=parent.Radius;
					 this.RAGUID=parent.RAGUID;
					 this.ref_DiaObject=parent.ref_DiaObject;
					 this.removal_=parent.removal_;
					 this.RequiresLOSCheck=parent.RequiresLOSCheck;
					 this.SummonerID=parent.SummonerID;
					 this.weight_=parent.Weight;
				}
				#endregion

				public int? AcdGuid { get; set; }
				public int RAGUID { get; set; }

				private double weight_;
				public virtual double Weight { get { return weight_; } set { weight_=value; } }
				private float radius_;
				public virtual float Radius { get { return radius_; } set { radius_=value; } }


				///<summary>
				///Used only if the object is a summonable pet.
				///</summary>
				public int? SummonerID { get; set; }
				///<summary>
				///The number of interaction attempts made.
				///</summary>
				public int InteractionAttempts { get; set; }
				///<summary>
				///References the actual DiaObject
				///</summary>
				public DiaObject ref_DiaObject { get; set; }

                public int PriorityCounter { get; set; }
				public DateTime PrioritizedDate { get; set; }
				public double LastPriortized
				{
					 get
					 {
						  return DateTime.Now.Subtract(PrioritizedDate).TotalMilliseconds;
					 }
				}


				#region Position Properties & Methods
				private Vector3 position_;
				public virtual Vector3 Position { get { return position_; } set { position_=value; } }

				private GridPoint pointposition_;
				public GridPoint PointPosition
				{
					 get
					 {
						  if (positionUpdated)
						  {
								pointposition_=position_;
								positionUpdated=false;
						  }

						  return pointposition_;
					 }
				}

				public float CentreDistance
				{
					 get
					 {
						  return Bot.Character.Position.Distance(this.Position);
					 }
				}

				public float RadiusDistance
				{
					 get
					 {
						  return Math.Max(0f, CentreDistance-this.Radius);
					 }
				}

				private DateTime lastUpdatedPosition=DateTime.Today;
				private bool positionUpdated=true;

				public virtual void UpdatePosition(bool force=false)
				{
					 if (!force&&DateTime.Now.Subtract(lastUpdatedPosition).TotalMilliseconds<300)
						  return;

					 using (ZetaDia.Memory.AcquireFrame())
					 {
						  try
						  {
								this.Position=this.ref_DiaObject.Position;
								this.lastUpdatedPosition=DateTime.Now;
								this.positionUpdated=true;
						  } catch (NullReferenceException )
						  {
								Logging.WriteVerbose("Safely Handled Updating Position for Object {0}", this.InternalName);
						  }
					 }
				}
				///<summary>
				///Returns adjusted position using direction of current bot and radius of object to reduce distance.
				///</summary>
				public Vector3 BotMeleeVector
				{
					 get
					 {
						  float distance=this.ActorSphereRadius.HasValue?this.ActorSphereRadius.Value
												:this.CollisionRadius.HasValue?this.CollisionRadius.Value:this.Radius;

                          Vector3 GroundedVector = new Vector3(this.position_.X,this.position_.Y,this.position_.Z-this.radius_/2);
                          return MathEx.GetPointAt(GroundedVector, (distance * 1.15f), FindDirection(GroundedVector, Bot.Character.Position, true));
					 }
				}

				internal GridPointAreaCache.GPRectangle gprect_;
				public virtual GridPointAreaCache.GPRectangle GPRect
				{
					 get
					 {
						  //Create new one..
						  if (gprect_==null)
								gprect_=new GridPointAreaCache.GPRectangle(Position, (int)(Math.Sqrt(this.ActorSphereRadius.Value)));

						  return gprect_;
					 }
				}
				#endregion


				#region Blacklist, Removal, and Valid
				///<summary>
				///Counter which increases when object is not seen during the refresh stage.
				///</summary>
				public int LoopsUnseen=0;
				///<summary>
				///Amount of loops this object is being ignored during the Usable Object iteration. If set to -1 it will be ignored indefinitely.
				///</summary>
				public int BlacklistLoops=0;
				///<summary>
				///Flag that determines if the object will be updated during Refresh (Live Data).
				///</summary>
				public bool NeedsUpdate { get; set; }
				///<summary>
				///Flag that determines if the object should be removed from the collection.
				///</summary>
				public bool NeedsRemoved
				{
					 get
					 {
						  return removal_;
					 }
					 set
					 {
						  removal_=value;
						  //This helps reduce code by flagging this here instead of after everytime we flag removal of an object!
						  if (value==true) dbRefresh.RemovalCheck=true;
					 }
				}
				internal bool removal_;
				///<summary>
				///This is evaluated during removal and when set to something other than none it will be blacklisted.
				///</summary>
				public BlacklistType BlacklistFlag { get; set; }
				///<summary>
				///All derieved objects override this and will return false.
				///</summary>
				public virtual bool ObjectShouldBeRecreated
				{
					 get
					 {
						  return true;
					 }
				}
				public virtual bool IsStillValid()
				{
					 //Check DiaObject first
					 if (ref_DiaObject==null||!ref_DiaObject.IsValid||ref_DiaObject.BaseAddress==IntPtr.Zero)
						  return false;

					 return true;
				}
				#endregion


				#region Line Of Sight Properties & Methods
				///<summary>
				///Line Of Sight Checking Variable
				///</summary>
				public bool RequiresLOSCheck { get; set; }


				///<summary>
				///Last time we preformed LOS Raycasting Test
				///</summary>
				public DateTime LastLOSCheck { get; set; }
				public double LastLOSCheckMS
				{
					 get
					 {
						  return (DateTime.Now.Subtract(LastLOSCheck).TotalMilliseconds);
					 }
				}
				///<summary>
				///Runs raycasting and intersection tests to validate LOS.
				///</summary>
				public bool LOSTest(Vector3 PositionToTestFrom, bool NavRayCast=true, bool ServerObjectIntersection=true, bool NavCellWalkable=false)
				{
					 this.LastLOSCheck=DateTime.Now;

					 if (NavRayCast&&Zeta.Navigation.Navigator.Raycast(PositionToTestFrom, this.BotMeleeVector))
						  return false;

					 if (ServerObjectIntersection&&ObjectCache.Obstacles.Values.OfType<CacheServerObject>().Any(obstacle => obstacle.Obstacletype.HasValue&&obstacle.Obstacletype.Value!=ObstacleType.Monster&&obstacle.TestIntersection(PositionToTestFrom, this.Position)))
						  return false;

					 if (NavCellWalkable&&!GilesCanRayCast(PositionToTestFrom, this.BotMeleeVector, Zeta.Internals.SNO.NavCellFlags.AllowWalk))
						  return false;



					 return true;
				}
				///<summary>
				///Last time we preformed a LOS vector search
				///</summary>
				public DateTime LastLOSSearch { get; set; }
				public double LastLOSSearchMS
				{
					 get
					 {
						  return (DateTime.Now.Subtract(LastLOSSearch).TotalMilliseconds);
					 }
				}
				///<summary>
				///Searches using the GridPointAreaCache and returns bool if LOSV3 has been set.
				///</summary>
				public bool FindLOSLocation
				{
					 get
					 {
						  this.LastLOSSearch=DateTime.Now;

						  Vector3 LOSV3;
						  bool FoundLOSLocation=false;

						  if (Bot.Class.IsMeleeClass)
						  {
								FoundLOSLocation=this.GPRect.TryFindSafeSpot(out LOSV3, this.Position, (Bot.Class.KiteDistance>0));

								if (!FoundLOSLocation)
									 FoundLOSLocation=GridPointAreaCache.AttemptFindTargetSafeLocation(out LOSV3, this, true, (Bot.Class.KiteDistance>0));
						  }
						  else
						  {
								FoundLOSLocation=FoundLOSLocation=GridPointAreaCache.AttemptFindTargetSafeLocation(out LOSV3, this, true, (Bot.Class.KiteDistance>0));
								if (!FoundLOSLocation)
									 this.GPRect.TryFindSafeSpot(out LOSV3, this.Position, (Bot.Class.KiteDistance>0));
						  }


						  if (FoundLOSLocation&&GilesCanRayCast(this.Position, LOSV3, NavCellFlags.AllowWalk))
						  {
								Logging.WriteVerbose("LOS Found new location for target {0}", this.InternalName);
								this.LOSV3=LOSV3;
						  }

						  return FoundLOSLocation;

					 }
				}
                private Vector3 losv3_;
                private DateTime losv3LastChanged = DateTime.Today;
				///<summary>
				///Used during targeting as destination vector
				///</summary>
				public Vector3 LOSV3 
                { 
                    get
                    {
                        //invalidate los vector after 4 seconds
                        if((this.LastLOSCheckMS>1500&&!this.RequiresLOSCheck)||DateTime.Now.Subtract(losv3LastChanged).TotalSeconds > 4)
                            losv3_ = vNullLocation;

                        return losv3_;
                    }

                    set { losv3_ = value; losv3LastChanged = DateTime.Now; }
                }
				///<summary>
				///Validates that the current LOS
				///</summary>
				public bool LastLOSCheckStillValid
				{
					 get
					 {
						  double LastCheckMS=this.LastLOSCheckMS;
                         //priority counter
                          if(this.PriorityCounter > 1 && LastCheckMS > 2500)
								return false;

						  //Recheck the LOS against the target.
						  bool valid=this.LOSTest(this.LOSV3!=vNullLocation?this.LOSV3:this.Position, true, (!Bot.Class.IsMeleeClass), (Bot.Class.IsMeleeClass));
						  
						  if (!valid)
								this.LOSV3=vNullLocation;

						  return valid;
					 }
				}
				#endregion





				public override bool UpdateData(DiaObject thisObj, int RaGuid)
				{
					 //Reference the object (just in case!)
					 if (this.ref_DiaObject==null)
						  this.ref_DiaObject=thisObj;

					 return base.UpdateData(thisObj, this.RAGUID);
				}
				///<summary>
				///Updates the object values (Used by derieved classes only!)
				///</summary>
				public virtual bool UpdateData()
				{
					 return true;
				}

				///<summary>
				///Base Testing Method for object targeting.
				///</summary>
				public virtual bool ObjectIsValidForTargeting
				{
					 get
					 {
						  //Blacklist loop counter checks
						  if (this.BlacklistLoops<0) return false;
						  if (this.BlacklistLoops>0)
						  {
								this.BlacklistLoops--;
								return false;
						  }

						  //Skip objects if not seen during cache refresh
						  if (this.LoopsUnseen>0) return false;

						  //Check if we are doing something important.. if so we only want to check units!
						  if (Bot.Combat.IsInNonCombatBehavior)
								if (!this.targetType.HasValue||this.targetType.Value!=TargetType.Unit) return false;

						  //Validate refrence still remains
						  if (!this.IsStillValid())
						  {
								//Flag for removal, and blacklist
								this.NeedsRemoved=true;
								this.BlacklistFlag=BlacklistType.Temporary;
								return false;
						  }


						  //Far away object?
						  if (this.CentreDistance>300f)
						  {
								this.NeedsRemoved=true;
								return false;
						  }



						  return true;
					 }
				}

				///<summary>
				///Base Weighting Method
				///</summary>
				public virtual void UpdateWeight()
				{
					 // Just to make sure each one starts at 0 weight...
					 this.Weight=0d;


					 //Unit && Melee Or Gizmo/Item AND Distance > xf.. than we check against avoidance zones!
					 bool ShouldTestMeleeAvoidance=((this.targetType.Value==TargetType.Unit&&Bot.Class.IsMeleeClass)||
															 (this.Actortype.Value==ActorType.Gizmo||this.targetType.Value==TargetType.Item)
															 &&this.CentreDistance>=7f);

					 if (ShouldTestMeleeAvoidance)
					 {
						  //Test if this object is within any avoidances.
						  if (ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(this.Position)
								&&(this.targetType.Value!=TargetType.Unit||
									 this.targetType.Value==TargetType.Unit&&Bot.Combat.RequiresAvoidance))
						  {
								this.Weight=1;
						  }

						  //intersecting avoidances..
						  if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(this.Position))
						  {
								if (this.Weight!=1&&(this.ObjectIsSpecial&&Bot.Class.IsMeleeClass))
								{//Only add this to the avoided list when its not currently inside avoidance area
									 ObjectCache.Objects.objectsIgnoredDueToAvoidance.Add(this);
								}
								else
									 this.Weight=1;
						  }
					 }
				}

				///<summary>
				///Validate the object is still capable of interaction. (Used by derieved classes only!)
				///</summary>
				public virtual bool CanInteract()
				{
					 return true;
				}

				public virtual bool IsZDifferenceValid
				{
					 get
					 {
						  return true;
					 }

				}
				///<summary>
				///Used to quickly check if object is considered special. (Used by derieved classes only!)
				///</summary>
				public virtual bool ObjectIsSpecial
				{
					 get
					 {
						  return false;
					 }
				}
				///<summary>
				///Value is set when object calls WithinInteractionRange.
				///</summary>
				public float DistanceFromTarget { get; set; }
				///<summary>
				///Called during target handling movement check stage. (Derieved classes override this)
				///</summary>
				public virtual bool WithinInteractionRange()
				{
					 this.DistanceFromTarget=Vector3.Distance(Bot.Character.Position, this.Position);
					 return (this.Radius<=0f||this.DistanceFromTarget<=this.Radius);
				}
				///<summary>
				///Called during target handling interaction stage. (Derieved classes override this)
				///</summary>
				public virtual RunStatus Interact()
				{
					 this.InteractionAttempts++;

					 // If we've tried interacting too many times, blacklist this for a while
					 if (this.InteractionAttempts>3)
					 {
						  this.NeedsRemoved=true;
						  this.BlacklistFlag=BlacklistType.Temporary;
					 }
					 this.BlacklistLoops=10;

					 return RunStatus.Success;
				}

				public virtual RunStatus MoveTowards()
				{

					 #region DebugInfo
					 if (SettingsFunky.DebugStatusBar)
					 {
						  string Action="[Move-";
						  switch (this.targetType.Value)
						  {
								case TargetType.Avoidance:
									 Action+="Avoid] ";
									 break;
								case TargetType.Unit:
									 if (this.LOSV3!=vNullLocation)
										  Action+="LOS] ";
									 else
										  Action+="Combat] ";

									 break;
								case TargetType.Item:
								case TargetType.Gold:
								case TargetType.Globe:
									 Action+="Pickup] ";
									 break;
								case TargetType.Interactable:
									 Action+="Interact] ";
									 break;
								case TargetType.Container:
									 Action+="Open] ";
									 break;
								case TargetType.Destructible:
								case TargetType.Barricade:
									 Action+="Destroy] ";
									 break;
								case TargetType.Shrine:
									 Action+="Click] ";
									 break;
						  }
						  Bot.Target.UpdateStatusText(Action);
					 }
					 #endregion

					 // Are we currently incapacitated? If so then wait...
					 if (Bot.Character.bIsIncapacitated||Bot.Character.bIsRooted)
						  return RunStatus.Running;

					 if (CacheMovementTracking.bSkipAheadAGo)
						  CacheMovementTracking.RecordSkipAheadCachePoint();

					 // Some stuff to avoid spamming usepower EVERY loop, and also to detect stucks/staying in one place for too long
					 bool bForceNewMovement=false;

					 //Herbfunk: Added this to prevent stucks attempting to move to a target blocked. (Case: 3 champs behind a wall, within range but could not engage due to being on the other side.)
					 if (Bot.Combat.totalNonMovementCount>50)
					 {
						  DbHelper.Log(DbHelper.TrinityLogLevel.Debug, DbHelper.LogCategory.Behavior, "{0}: Ignoring mob {1} due to no movement counter reaching {2}", "[Funky]", this.InternalName+" _ SNO:"+this.SNOID, Bot.Combat.totalNonMovementCount);
						  Logging.WriteDiagnostic("totalNonMovementCount == "+Bot.Combat.totalNonMovementCount);
						  this.BlacklistLoops=50;
						  Bot.Combat.bForceTargetUpdate=true;
						  Bot.Combat.totalNonMovementCount=0;

						  // Reset the emergency loop counter and return success
						  return RunStatus.Running;
					 }

					 #region DistanceChecks
					 // Count how long we have failed to move - body block stuff etc.
					 if (this.DistanceFromTarget==Bot.Combat.fLastDistanceFromTarget)
					 {
						  bForceNewMovement=true;
						  if (DateTime.Now.Subtract(Bot.Combat.lastMovedDuringCombat).TotalMilliseconds>=250)
						  {
								//If we are moving to a LOS location.. nullify it!
								if (this.LOSV3!=vNullLocation)
									 this.losv3_=vNullLocation;

								Bot.Combat.lastMovedDuringCombat=DateTime.Now;
								// We've been stuck at least 250 ms, let's go and pick new targets etc.
								Bot.Combat.iTimesBlockedMoving++;
								Bot.Combat.bForceCloseRangeTarget=true;
								Bot.Combat.lastForcedKeepCloseRange=DateTime.Now;

								// And tell Trinity to get a new target
								Bot.Combat.bForceTargetUpdate=true;


								// Tell target finder to prioritize close-combat targets incase we were bodyblocked
								#region TargetingPriortize
								switch (Bot.Combat.iTimesBlockedMoving)
								{
									 case 2:
									 case 3:
										  if (this.targetType.Value!=TargetType.Avoidance)
										  {
												// Check for raycastability against static objects

												//Units blocking us..
												//Destuctibles blocking us..
												//Door not opened..
												//Walls/Props blocking us from our target..
												//Navigation impossible or is stuck running in place?
												//Eliminate any potential objects first..
												//	-Units by # surrounding us, that we are facing!
												//Bot.Character.UpdateMovementData();

												List<CacheUnit> monsterobstacles_;

												monsterobstacles_=ObjectCache.Objects.Values.OfType<CacheUnit>().Where(unit => ((PlayerMover.iTimesReachedStuckPoint==0&&ZetaDia.Me.IsFacing(unit.Position))||PlayerMover.iTimesReachedStuckPoint>0)&&unit.CentreDistance<=10f).ToList();
												//ObjectCache.Obstacles.FindObstaclesSurroundingObject<CacheUnit>(ObjectData, 20f, out monsterobstacles_);
												if (monsterobstacles_.Count>0&&monsterobstacles_.Any())
												{
													 Bot.Combat.PrioritizedRAGUIDs.AddRange(from monsters in monsterobstacles_
																										 where !Bot.Combat.PrioritizedRAGUIDs.Contains(monsters.RAGUID)
																										 select monsters.RAGUID);

													 foreach (var item in monsterobstacles_)
													 {
                                                         item.PriorityCounter++;
													 }

													 Logging.WriteVerbose("Nearby Monsters being prioritzed!");
												}
												else
												{
													 IEnumerable<CacheDestructable> GizmoObjects=ObjectCache.Objects.OfType<CacheDestructable>();

													 //Destuctibles.. bot is facing that are less than 12f away.
													 CacheObject[] Destructibles=GizmoObjects.Where(obj => ((PlayerMover.iTimesReachedStuckPoint==0&&ZetaDia.Me.IsFacing(obj.Position))||PlayerMover.iTimesReachedStuckPoint>0)
																																&&obj.CentreDistance<=10f).ToArray();


													 if (Destructibles!=null&&Destructibles.Length>0)
													 {
														  Bot.Combat.PrioritizedRAGUIDs.AddRange(from objs in Destructibles
																											  where !Bot.Combat.PrioritizedRAGUIDs.Contains(objs.RAGUID)
																											  select objs.RAGUID);

														  foreach (var item in Destructibles)
														  {
                                                              item.PriorityCounter++;
														  }

														  Logging.WriteVerbose("Nearby Destructibles being prioritzed!");
													 }
													 else
													 {//Interactables...
														  IEnumerable<CacheInteractable> GizmoInteractables=ObjectCache.Objects.OfType<CacheInteractable>();

														  CacheObject[] Interactables=GizmoInteractables.Where(obj => ((PlayerMover.iTimesReachedStuckPoint==0&&ZetaDia.Me.IsFacing(obj.Position))||PlayerMover.iTimesReachedStuckPoint>0)
																																&&obj.CentreDistance<=10f).ToArray();
														  if (Interactables!=null&&Interactables.Length>0)
														  {
																Bot.Combat.PrioritizedRAGUIDs.AddRange(from objs in Interactables
																													where !Bot.Combat.PrioritizedRAGUIDs.Contains(objs.RAGUID)
																													select objs.RAGUID);
																foreach (var item in Interactables)
																{
                                                                    item.PriorityCounter++;
																}
																Logging.WriteVerbose("Nearby Interactables being prioritzed!");
														  }
														  else
														  {//Movement State
																Bot.Character.UpdateMovementData();
																if (Bot.Character.currentMovementState.HasFlag(MovementState.WalkingInPlace))
																{
																	 Logging.WriteVerbose("Stuck running in place!");
																}
														  }
													 }
												}
										  }
										  else
										  {//Avoidance Movement..
												GridPointAreaCache.BlacklistLastSafespot();
												Bot.Combat.bForceTargetUpdate=true;
												return RunStatus.Running;
										  }

										  //Than check our movement state
										  break;
									 default:
										  if (this.targetType.Value!=TargetType.Avoidance)
										  {
												//Finally try raycasting to see if navigation is possible..
												if (this.Actortype.HasValue&&
													 (this.Actortype.Value==ActorType.Gizmo||this.Actortype.Value==ActorType.Unit))
												{
													 Vector3 hitTest;
													 // No raycast available, try and force-ignore this for a little while, and blacklist for a few seconds
													 if (Zeta.Navigation.Navigator.Raycast(Bot.Character.Position, this.Position, out hitTest))
													 {
														  if (hitTest!=Vector3.Zero)
														  {
																Bot.Combat.iTimesBlockedMoving=0;
																this.RequiresLOSCheck=true;
																Log("Ignoring object "+this.InternalName+" due to not moving and raycast failure!", true);
																Bot.Combat.bForceTargetUpdate=true;
																return RunStatus.Running;
														  }
													 }
												}
										  }
										  else
										  {
												if (!GilesCanRayCast(Bot.Character.Position, Bot.Combat.vCurrentDestination, NavCellFlags.AllowWalk))
												{
													 Logging.WriteVerbose("Cannot continue with avoidance movement due to raycast failure!");
													 Bot.Combat.iTimesBlockedMoving=0;

													 Bot.Combat.iMillisecondsCancelledEmergencyMoveFor/=2;
													 Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
													 //Ignore avoidance movements.
													 Bot.Combat.iMillisecondsCancelledKiteMoveFor/=2;
													 Bot.Combat.timeCancelledKiteMove=DateTime.Now;

													 GridPointAreaCache.BlacklistLastSafespot();
													 Bot.Combat.bForceTargetUpdate=true;
													 return RunStatus.Running;
												}
										  }
										  break;
								}
								#endregion


								return RunStatus.Running;
						  } // Been 250 milliseconds of non-movement?
					 }
					 else
					 {
						  // Movement has been made, so count the time last moved!
						  Bot.Combat.lastMovedDuringCombat=DateTime.Now;
					 }
					 #endregion

					 // Update the last distance stored
					 Bot.Combat.fLastDistanceFromTarget=this.DistanceFromTarget;


					 // See if there's an obstacle in our way, if so try to navigate around it
					 #region ObstacleCheck
					 if (vShiftedPosition==vNullLocation)
					 {
						  Vector3 obstacleV3;
						  // See if there's an obstacle in our way, if so try to navigate around it
						  if (Bot.Combat.vCurrentDestination!=vNullLocation
								&&ObstacleCheck(out obstacleV3, Bot.Combat.vCurrentDestination))
						  {

								lastShiftedPosition=DateTime.Now;
								iShiftPositionFor=1000;
								vShiftedPosition=obstacleV3;

								if (vShiftedPosition!=vNullLocation)
								{
									 Logging.WriteDiagnostic("Using altered navigation vector {0} to bypass obstacle", vShiftedPosition.ToString());
									 Bot.Combat.vCurrentDestination=vShiftedPosition;
								}
						  }
					 }
					 else
					 {
						  if (Bot.Character.Position.Distance2D(vShiftedPosition)<=2.5f||DateTime.Now.Subtract(lastShiftedPosition).TotalMilliseconds>iShiftPositionFor)
								vShiftedPosition=vNullLocation;
						  else
								Bot.Combat.vCurrentDestination=vShiftedPosition;
					 }
					 #endregion

					 // See if we want to ACTUALLY move, or are just waiting for the last move command...
					 if (!bForceNewMovement&&Bot.Combat.bAlreadyMoving&&Bot.Combat.vCurrentDestination==Bot.Combat.vLastMoveToTarget&&DateTime.Now.Subtract(Bot.Combat.lastMovementCommand).TotalMilliseconds<=100)
					 {
						  return RunStatus.Running;
					 }

					 float currentDistance=Vector3.Distance(Bot.Combat.vLastMoveToTarget, Bot.Combat.vCurrentDestination);

					 // If we're doing avoidance, globes or backtracking, try to use special abilities to move quicker
					 #region SpecialMovementChecks
					 if ((TargetType.Avoidance|TargetType.Gold|TargetType.Globe|TargetType.Destructible|TargetType.Unit).HasFlag(this.targetType.Value))
					 {

						  bool bTooMuchZChange=((Bot.Character.Position.Z-Bot.Combat.vCurrentDestination.Z)>=4f);

						  SNOPower MovementPower;
						  if (Bot.Class.FindSpecialMovementPower(out MovementPower))
						  {
								double lastUsedAbilityMS=DateTime.Now.Subtract(Funky.dictAbilityLastUse[MovementPower]).TotalMilliseconds;
								bool foundMovementPower=false;
								float pointDistance=0f;
								Vector3 vTargetAimPoint=Bot.Combat.vCurrentDestination;

								switch (MovementPower)
								{
									 case SNOPower.Monk_TempestRush:
										  vTargetAimPoint=MathEx.CalculatePointFrom(Bot.Combat.vCurrentDestination, Bot.Character.Position, 10f);
										  Bot.Character.UpdateAnimationState(false, true);
										  bool isHobbling=Bot.Character.CurrentSNOAnim.HasFlag(SNOAnim.Monk_Female_Hobble_Run|SNOAnim.Monk_Male_HTH_Hobble_Run);
										  foundMovementPower=(!bTooMuchZChange&&(((!isHobbling||lastUsedAbilityMS>200)&&Bot.Character.dCurrentEnergy>=50)||((isHobbling||lastUsedAbilityMS<400)&&Bot.Character.dCurrentEnergy>15))
												&&!ObjectCache.Obstacles.DoesPositionIntersectAny(vTargetAimPoint, ObstacleType.ServerObject));
										  break;
									 case SNOPower.DemonHunter_Vault:
										  foundMovementPower=(this.targetType.Value!=TargetType.Destructible&&!bTooMuchZChange&&currentDistance>=18f&&
																	 (lastUsedAbilityMS>=Funky.SettingsFunky.Class.iDHVaultMovementDelay));
										  pointDistance=35f;
										  if (currentDistance>pointDistance)
												vTargetAimPoint=MathEx.CalculatePointFrom(Bot.Combat.vCurrentDestination, Bot.Character.Position, pointDistance);
										  break;
									 case SNOPower.Barbarian_FuriousCharge:
									 case SNOPower.Barbarian_Leap:
									 case SNOPower.Wizard_Archon_Teleport:
									 case SNOPower.Wizard_Teleport:
										  foundMovementPower=(this.targetType.Value!=TargetType.Destructible&&!bTooMuchZChange&&currentDistance>20f);
										  pointDistance=35f;
										  if (currentDistance>pointDistance)
												vTargetAimPoint=MathEx.CalculatePointFrom(Bot.Combat.vCurrentDestination, Bot.Character.Position, pointDistance);

										  break;
									 case SNOPower.Barbarian_Whirlwind:
										  break;
									 default:
										  WaitWhileAnimating(3, true);
										  ZetaDia.Me.UsePower(MovementPower, Bot.Combat.vCurrentDestination, Bot.Character.iCurrentWorldID, -1);
										  dictAbilityLastUse[MovementPower]=DateTime.Now;

										  WaitWhileAnimating(6, true);
										  // Store the current destination for comparison incase of changes next loop
										  Bot.Combat.vLastMoveToTarget=Bot.Combat.vCurrentDestination;
										  // Reset total body-block count, since we should have moved
										  if (DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>=2000)
												Bot.Combat.iTimesBlockedMoving=0;
										  return RunStatus.Running;
								}

								if (foundMovementPower)
								{

									 if ((MovementPower==SNOPower.Monk_TempestRush&&lastUsedAbilityMS>500)||
									  ZetaDia.Physics.Raycast(Bot.Character.Position, vTargetAimPoint, NavCellFlags.AllowWalk))
									 {
										  ZetaDia.Me.UsePower(MovementPower, vTargetAimPoint, Funky.Bot.Character.iCurrentWorldID, -1);
										  Funky.dictAbilityLastUse[MovementPower]=DateTime.Now;

										  // Store the current destination for comparison incase of changes next loop
										  Bot.Combat.vLastMoveToTarget=Bot.Combat.vCurrentDestination;

										  // Reset total body-block count, since we should have moved
										  if (DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>=2000)
												Bot.Combat.iTimesBlockedMoving=0;
										  return RunStatus.Running;
									 }
								}
						  }

						  //Special Whirlwind Code
						  if (Bot.Class.AC==Zeta.Internals.Actors.ActorClass.Barbarian&&HotbarAbilitiesContainsPower(SNOPower.Barbarian_Whirlwind))
						  {
								// Whirlwind against everything within range (except backtrack points)
								if (Bot.Character.dCurrentEnergy>=10
									 &&Bot.Combat.iAnythingWithinRange[RANGE_20]>=1
									 &&this.DistanceFromTarget<=12f
									 &&(!HotbarAbilitiesContainsPower(SNOPower.Barbarian_Sprint)||HasBuff(SNOPower.Barbarian_Sprint))
									 &&(!(TargetType.Avoidance|TargetType.Gold|TargetType.Globe).HasFlag(this.targetType.Value)&this.DistanceFromTarget>=6f)
									 &&(this.targetType.Value!=TargetType.Unit
									 ||(this.targetType.Value==TargetType.Unit&&!this.IsTreasureGoblin
										  &&(!SettingsFunky.Class.bSelectiveWhirlwind
												||Bot.Combat.bAnyNonWWIgnoreMobsInRange
												||!SnoCacheLookup.hashActorSNOWhirlwindIgnore.Contains(this.SNOID)))))
								{
									 // Special code to prevent whirlwind double-spam, this helps save fury
									 bool bUseThisLoop=SNOPower.Barbarian_Whirlwind!=Bot.Combat.powerLastSnoPowerUsed;
									 if (!bUseThisLoop)
									 {
										  Bot.Combat.powerLastSnoPowerUsed=SNOPower.None;
										  if (DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.Barbarian_Whirlwind]).TotalMilliseconds>=200)
												bUseThisLoop=true;
									 }
									 if (bUseThisLoop)
									 {
										  ZetaDia.Me.UsePower(SNOPower.Barbarian_Whirlwind, Bot.Combat.vCurrentDestination, Bot.Character.iCurrentWorldID, -1);
										  Bot.Combat.powerLastSnoPowerUsed=SNOPower.Barbarian_Whirlwind;
										  dictAbilityLastUse[SNOPower.Barbarian_Whirlwind]=DateTime.Now;
									 }
									 // Store the current destination for comparison incase of changes next loop
									 Bot.Combat.vLastMoveToTarget=Bot.Combat.vCurrentDestination;
									 // Reset total body-block count
									 if ((!Bot.Combat.bForceCloseRangeTarget||DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>Bot.Combat.iMillisecondsForceCloseRange)&&
										 DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>=2000)
										  Bot.Combat.iTimesBlockedMoving=0;
									 return RunStatus.Running;
								}
						  }
					 }
					 #endregion


					 // Now for the actual movement request stuff
					 Bot.Combat.bAlreadyMoving=true;
					 Bot.Combat.lastMovementCommand=DateTime.Now;


					 if (DateTime.Now.Subtract(Bot.Combat.lastSentMovePower).TotalMilliseconds>=250||currentDistance>=2f||bForceNewMovement)
					 {

						  if (this.LOSV3==vNullLocation)
						  {
								ZetaDia.Me.UsePower(SNOPower.Walk, Bot.Combat.vCurrentDestination, Bot.Character.iCurrentWorldID, -1);
						  }
						  else
								ZetaDia.Me.Movement.MoveActor(Bot.Combat.vCurrentDestination);


						  Bot.Combat.lastSentMovePower=DateTime.Now;
						  // Store the current destination for comparison incase of changes next loop
						  Bot.Combat.vLastMoveToTarget=Bot.Combat.vCurrentDestination;
						  // Reset total body-block count, since we should have moved
						  if (DateTime.Now.Subtract(Bot.Combat.lastForcedKeepCloseRange).TotalMilliseconds>=2000)
								Bot.Combat.iTimesBlockedMoving=0;

						  //Herbfunk: Quick fix for stuck occuring on above average mob who is damaged..
						  if (Bot.Combat.lastPlayerPosDuringTargetMovement==Bot.Character.Position)
								Bot.Combat.totalNonMovementCount++;
						  else
								Bot.Combat.totalNonMovementCount=0;

						  Bot.Combat.lastPlayerPosDuringTargetMovement=Bot.Character.Position;
					 }

					 return RunStatus.Running;
				}









				public new CacheObject Clone()
				{
					 return (CacheObject)this.MemberwiseClone();
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
						  CacheObject p=(CacheObject)obj;
						  return this.RAGUID==p.RAGUID;
					 }
				}

				public override int GetHashCode()
				{
					 return this.RAGUID;
				}

				public int CompareTo(object obj)
				{
					 if (obj==null) return 1;
					 CacheObject other=obj as CacheObject;

					 if (other!=null)
						  return other.CentreDistance.CompareTo(this.CentreDistance);
					 else
						  return 1;
				}

				public override string DebugString
				{
					 get
					 {
						  return String.Format("RAGUID {0}: \r\n {1} \r\n Distance (Centre{2} / Radius{3})",
								this.RAGUID.ToString(), base.DebugString, this.CentreDistance.ToString(), this.RadiusDistance.ToString());
					 }
				}

		  }
	 }
}