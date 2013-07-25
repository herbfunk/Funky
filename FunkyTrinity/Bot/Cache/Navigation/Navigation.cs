using System;
using System.Linq;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.SNO;
using Zeta.Navigation;
using System.Globalization;

namespace FunkyTrinity
{
	 public partial class Funky
	 {

		  ///<summary>
		  ///Cache of all values Navigation related
		  ///</summary>
		  public class Navigation
		  {
				public Navigation() { }

				#region Bot (Character) Properties
				private bool isMoving=false;
				///<summary>
				///Cached Value of ZetaDia.Me.IsMoving
				///</summary>
				public bool IsMoving
				{
					 get { return isMoving; }
					 set { isMoving=value; }
				}

				private MovementState curMoveState=MovementState.None;
				public MovementState currentMovementState
				{
					 get
					 {
						  RefreshMovementCache();
						  return curMoveState;
					 }
				}

				private float curRotation=0f;
				public float currentRotation
				{
					 get
					 {
						  return curRotation;
					 }
				}

				private double curSpeedXY=0d;
				public double currentSpeedXY
				{
					 get
					 {
						  RefreshMovementCache();
						  return curSpeedXY;
					 }
				}

				private DateTime LastUpdatedMovementData=DateTime.Today;
				internal void RefreshMovementCache()
				{
					 // If we aren't in the game of a world is loading, don't do anything yet
					 if (!ZetaDia.IsInGame||ZetaDia.IsLoadingWorld)
						  return;

					 if (DateTime.Now.Subtract(LastUpdatedMovementData).TotalMilliseconds>150)
					 {
						  LastUpdatedMovementData=DateTime.Now;

						  //These vars are used to accuratly predict what the bot is doing (Melee Movement/Combat)
						  using (ZetaDia.Memory.AcquireFrame())
						  {
								try
								{
									 ActorMovement botMovement=ZetaDia.Me.Movement;
									 isMoving=botMovement.IsMoving;
									 curSpeedXY=botMovement.SpeedXY;
									 curRotation=botMovement.Rotation;
									 curMoveState=botMovement.MovementState;
								} catch
								{
									 Log("Safely handled exception during RefreshMovementCache()", true);
								}
						  }
					 }
				}
				
				#endregion


				//GPCache

				private GPRectangle CurrentLocationGPrect=null;
				///<summary>
				///Current Bots Position Rectangle
				///</summary>
				public GPRectangle CurrentLocationGPRect
				{
					 get { return CurrentLocationGPrect; }
					 set { CurrentLocationGPrect=value; }
				}

				//Blacklisted points used by movement behaviors
				public HashSet<GridPoint> BlacklistedGridpoints=new HashSet<GridPoint>();
				internal void BlacklistLastSafespot()
				{
					 if (Bot.NavigationCache.LastUsedRect!=null)
					 {
						  //Blacklist the creation vector and nullify the last used..
						  Bot.NavigationCache.BlacklistedGridpoints.Add(Bot.NavigationCache.LastUsedRect.LastFoundSafeSpot);
					 }
					 vlastSafeSpot=Vector3.Zero;
				}







				///<summary>
				///Bots Position is blocked from adjacent movement -- Updated whenever searching for a safe location!
				///</summary>
				internal void RefreshNavigationBlocked()
				{
					 //Check if bot is navigationally blocked..
					 if (DateTime.Now.Subtract(LastNavigationBlockCheck).TotalMilliseconds>500)
					 {
						  LastNavigationBlockCheck=DateTime.Now;

						  if (IsVectorBlocked(Bot.Character.Position))
						  {
								Logging.WriteVerbose("[Funky] Current Position is Navigationally Blocked");
								BotIsNavigationallyBlocked=true;
						  }
						  else
								BotIsNavigationallyBlocked=false;
					 }
				}
				internal bool BotIsNavigationallyBlocked=false;
				private DateTime LastNavigationBlockCheck=DateTime.Today;

				#region Vetor Blocking
				//Tracks Objects that occupy surrounding navigation grid points
				private Dictionary<int, int> LastObjectblockCounter=new Dictionary<int, int>();
				private Dictionary<int, GridPoint[]> LastObjectOccupiedGridPoints=new Dictionary<int, GridPoint[]>();
				private GPRectangle LastUsedBlockCheckGPRect=null;
				private List<GridPoint> LastNavigationBlockedPoints=new List<GridPoint>();
				///<summary>
				///Checks if the position is total blocked from adjacent movements either by objects or non navigation
				///</summary>
				private bool IsVectorBlocked(Vector3 location)
				{
					 //Reset Navigationally Blocked GPs
					 this.LastNavigationBlockedPoints=new List<GridPoint>();

					 //Create Local GPRect!
					 if (this.LastUsedBlockCheckGPRect==null||this.LastUsedBlockCheckGPRect.centerpoint!=(GridPoint)location)
					 {
						  //Clear lists
						  LastObjectblockCounter.Clear();
						  LastObjectOccupiedGridPoints.Clear();
						  this.LastUsedBlockCheckGPRect=new GPRectangle(location);
					 }

					 if (this.LastUsedBlockCheckGPRect.Count==0)
					 {
						  Logging.WriteDiagnostic("Current Location GP Rect has no valid Grid Points!");
						  return false;
					 }

					 GridPoint[] CurrentLocationGridPoints=this.LastUsedBlockCheckGPRect.Keys.ToArray();
					 List<GridPoint> SurroundingPoints=new List<GridPoint>();
					 int SurroundingMaxCount=this.LastUsedBlockCheckGPRect.Count>=8?8:this.LastUsedBlockCheckGPRect.Count;
					 for (int i=0; i<SurroundingMaxCount; i++)
					 {
						  GridPoint gp=CurrentLocationGridPoints[i];
						  if (!gp.Ignored)
								SurroundingPoints.Add(gp);
						  else
								this.LastNavigationBlockedPoints.Add(gp);
					 }

					 List<int> NearbyObjectRAGUIDs=new List<int>();
					 List<CacheServerObject> NearbyObjects=Bot.Combat.NearbyObstacleObjects.Where(obj => obj.RadiusDistance<=5f).ToList();//ObjectCache.Obstacles.Navigations.Where(obj => obj.RadiusDistance<=5f).ToList();

					 //no nearby objects passed distance check..
					 if (NearbyObjects.Count==0)
					 {
						  //Clear list, and return pure navigational check (Zero means we are completely stuck in a non-navigable location?)
						  LastObjectblockCounter.Clear();
						  LastObjectOccupiedGridPoints.Clear();

						  if (Bot.SettingsFunky.LogSafeMovementOutput)
								Logging.WriteVerbose("Current Location Point has {0} usable points (NoNewObjs)", SurroundingPoints.Count);

						  return (SurroundingPoints.Count==0);
					 }

					 //Update ObjectBlockCounter Collection
					 if (LastObjectblockCounter.Count>0)
					 {
						  //Add current nearby object RAGUIDs to collection
						  NearbyObjectRAGUIDs.AddRange((from objs in NearbyObjects
																  select objs.RAGUID).ToArray());

						  //Generate Removal List for ObjectBlockCounter Collections
						  List<int> RemovalRAGUIDList=(from raguids in LastObjectblockCounter.Keys
																 where !NearbyObjectRAGUIDs.Contains(raguids)
																 select raguids).ToList();

						  //Removal
						  foreach (var item in RemovalRAGUIDList)
						  {
								LastObjectblockCounter.Remove(item);
								LastObjectOccupiedGridPoints.Remove(item);
						  }
					 }

					 //Check any exisiting block entries
					 if (LastObjectblockCounter.Count>0)
					 {
						  foreach (var item in LastObjectOccupiedGridPoints.Values)
						  {
								this.LastNavigationBlockedPoints.AddRange(item);
						  }

						  //Update Surrounding Points
						  SurroundingPoints=SurroundingPoints.Except(this.LastNavigationBlockedPoints).ToList();

						  if (SurroundingPoints.Count==0)
						  {
								if (Bot.SettingsFunky.LogSafeMovementOutput)
									 Logging.WriteVerbose("NavBlocked -- No available surrounding points.");

								return true;
						  }
					 }

					 //Generate new object list that contains objects that are not already accounted for
					 List<CacheServerObject> NewObjects=NearbyObjects.Where(obj => !LastObjectblockCounter.ContainsKey(obj.RAGUID)||LastObjectblockCounter[obj.RAGUID]<4).ToList();

					 //No new objects to test..
					 if (NewObjects.Count==0)
					 {
						  if (Bot.SettingsFunky.LogSafeMovementOutput)
								Logging.WriteVerbose("No new Objects Unaccounted");

						  return (SurroundingPoints.Count==0);
					 }


					 foreach (GridPoint item in SurroundingPoints)
					 {
						  //Find any objects that contain this GP
						  CacheServerObject[] ContainedObjs=NewObjects.Where(Obj => Obj.PointInside(item)			   //only objects that have hit there maximum block count.
																						  &&(!LastObjectblockCounter.ContainsKey(Obj.RAGUID)||Math.Round(Obj.PointRadius)<LastObjectblockCounter[Obj.RAGUID])).ToArray();
						  if (ContainedObjs.Length>0)
						  {
								if (ContainedObjs.Length>1&&Bot.SettingsFunky.LogSafeMovementOutput)
									 Logging.WriteVerbose("Multiple Objects Found Occuping Grid Point!");

								CacheServerObject ThisObjBlocking=ContainedObjs[0];
								int ObjRAGUID=ThisObjBlocking.RAGUID;

								if (LastObjectblockCounter.ContainsKey(ObjRAGUID))
								{
									 int GPCount=LastObjectOccupiedGridPoints[ObjRAGUID].Length;
									 LastObjectblockCounter[ObjRAGUID]++;
									 GridPoint[] newArrayGPs=new GridPoint[GPCount];
									 LastObjectOccupiedGridPoints[ObjRAGUID].CopyTo(newArrayGPs, 0);
									 newArrayGPs[GPCount-1]=item.Clone();
									 LastObjectOccupiedGridPoints[ObjRAGUID]=newArrayGPs;
								}
								else
								{
									 LastObjectblockCounter.Add(ObjRAGUID, 1);
									 GridPoint[] NewArrayGP=new GridPoint[1] { item.Clone() };
									 LastObjectOccupiedGridPoints.Add(ObjRAGUID, NewArrayGP);
								}

								this.LastNavigationBlockedPoints.Add(item);
						  }
					 }

					 //Update Surrounding Points
					 SurroundingPoints=SurroundingPoints.Except(this.LastNavigationBlockedPoints).ToList();

					 if (Bot.SettingsFunky.LogSafeMovementOutput)
						  Logging.WriteVerbose("Current Location Point has {0} usable points", SurroundingPoints.Count);


					 return (SurroundingPoints.Count==0);
				}
				
				#endregion



				private GPArea CurrentGParea=null;
				///<summary>
				///Area used to search for movement locations.
				///</summary>
				public GPArea CurrentGPArea
				{
					 get { return CurrentGParea; }
					 set { CurrentGParea=value; }
				}
				///<summary>
				///Last successful GP Rectangle used during search method.
				///</summary>
				public GPRectangle LastUsedRect
				{
					 get { return CurrentGParea!=null?CurrentGParea.lastUsedGPRect:null; }
				}
				private Vector3 lastsearchvector=Vector3.Zero;
				public Vector3 LastSearchVector
				{
					 get { return lastsearchvector; }
					 set { lastsearchvector=value; }
				}
				///<summary>
				///Searches for a safespot!
				///</summary>
				public bool AttemptFindSafeSpot(out Vector3 safespot, Vector3 LOS, bool kiting=false)
				{
					 safespot=vlastSafeSpot;
					 if (!Bot.Combat.TravellingAvoidance&&DateTime.Now.Subtract(lastFoundSafeSpot).TotalMilliseconds<=800
						&&vlastSafeSpot!=Vector3.Zero
						&&(!ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(vlastSafeSpot)))
					 //&&(!kiting||!ObjectCache.Objects.IsPointNearbyMonsters(vlastSafeSpot,Bot.KiteDistance)))
					 {	 //Already found a safe spot in the last 800ms
						  return true;
					 }


					 Vector3 BotPosition=Bot.Character.Position;

					 //Check if we should refresh..
					 if (Bot.NavigationCache.CurrentGPArea==null||Bot.NavigationCache.CurrentGPArea.AllGPRectsFailed&&!Bot.NavigationCache.CurrentGPArea.centerGPRect.Contains(BotPosition)||!Bot.NavigationCache.CurrentGPArea.GridPointContained(BotPosition))
						  Bot.NavigationCache.CurrentGPArea=new GPArea(BotPosition);

					 //Check Bot Navigationally blocked
					 RefreshNavigationBlocked();
					 if (BotIsNavigationallyBlocked) return false;

					 Bot.NavigationCache.CurrentLocationGPRect.UpdateObjectCount();

					 safespot=Bot.NavigationCache.CurrentGPArea.AttemptFindSafeSpot(BotPosition, LOS, kiting);
					 return (safespot!=Vector3.Zero);
				}




				//Special Movements
				public Vector3 FindZigZagTargetLocation(Vector3 vTargetLocation, float fDistanceOutreach, bool bRandomizeDistance=false, bool bRandomizeStart=false, bool bCheckGround=false)
				{
					 Vector3 vThisZigZag=Vector3.Zero;
					 bool useTargetBasedZigZag=false;
					 float minDistance=6f;
					 float maxDistance=16f;
					 int minTargets=2;

					 if (useTargetBasedZigZag&&!Bot.Combat.bAnyTreasureGoblinsPresent&&Bot.Combat.UnitRAGUIDs.Count>=minTargets)
					 {
						  List<CacheServerObject> units_=Bot.Combat.NearbyObstacleObjects.Where(obj => Bot.Combat.UnitRAGUIDs.Contains(obj.RAGUID)).ToList();

						  IEnumerable<CacheObject> zigZagTargets=
								from u in units_
								where u.CentreDistance>minDistance&&u.CentreDistance<maxDistance&&u.RAGUID!=Bot.Target.CurrentTarget.RAGUID&&
								!ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(u.Position)&&MGP.CanStandAt(u.BotMeleeVector)
								select u;

						  if (zigZagTargets.Count()>=minTargets)
						  {
								CacheObject unit=zigZagTargets.OrderByDescending(u => u.Weight).FirstOrDefault();
								vThisZigZag=unit.Position;
								return vThisZigZag;
						  }
					 }

					 Random rndNum=new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), NumberStyles.HexNumber));
					 bool bCanRayCast;
					 float iFakeStart=0;
					 //K: bRandomizeStart is for boss and elite, they usually jump around, make obstacles, let you Incapacitated. 
					 //   you usually have to move back and forth to hit them
					 if (bRandomizeStart)
						  iFakeStart=rndNum.Next(18)*5;
					 if (bRandomizeDistance)
						  fDistanceOutreach+=rndNum.Next(18);
					 float fDirectionToTarget=Navigation.FindDirection(Bot.Character.Position, vTargetLocation);

					 float fPointToTarget;

					 float fHighestWeight=float.NegativeInfinity;
					 Vector3 vBestLocation=Vector3.Zero;

					 bool bFoundSafeSpotsFirstLoop=false;
					 float fAdditionalRange=0f;
					 //K: Direction is more important than distance

					 for (int iMultiplier=1; iMultiplier<=2; iMultiplier++)
					 {
						  if (iMultiplier==2)
						  {
								if (bFoundSafeSpotsFirstLoop)
									 break;
								fAdditionalRange=150f;
								if (bRandomizeStart)
									 iFakeStart=30f+(rndNum.Next(16)*5);
								else
									 iFakeStart=(rndNum.Next(17)*5);
						  }
						  float fRunDistance=fDistanceOutreach;
						  for (float iDegreeChange=iFakeStart; iDegreeChange<=30f+fAdditionalRange; iDegreeChange+=5)
						  {
								float iPosition=iDegreeChange;
								//point to target is better, otherwise we have to avoid obstacle first 
								if (iPosition>105f)
									 iPosition=90f-iPosition;
								else if (iPosition>30f)
									 iPosition-=15f;
								else
									 iPosition=15f-iPosition;
								fPointToTarget=iPosition;

								iPosition+=fDirectionToTarget;
								if (iPosition<0)
									 iPosition=360f+iPosition;
								if (iPosition>=360f)
									 iPosition=iPosition-360f;

								vThisZigZag=MathEx.GetPointAt(Bot.Character.Position, fRunDistance, MathEx.ToRadians(iPosition));

								if (fPointToTarget<=30f||fPointToTarget>=330f)
								{
									 vThisZigZag.Z=vTargetLocation.Z;
								}
								else if (fPointToTarget<=60f||fPointToTarget>=300f)
								{
									 //K: we are trying to find position that we can circle around the target
									 //   but we shouldn't run too far away from target
									 vThisZigZag.Z=(vTargetLocation.Z+Bot.Character.Position.Z)/2;
									 fRunDistance=fDistanceOutreach-5f;
								}
								else
								{
									 //K: don't move too far if we are not point to target, we just try to move
									 //   this can help a lot when we are near stairs
									 fRunDistance=8f;
								}

								bCanRayCast=MGP.CanStandAt(vThisZigZag);



								// Give weight to each zigzag point, so we can find the best one to aim for
								if (bCanRayCast)
								{
									 bool bAnyAvoidance=false;

									 // Starting weight is 1000f
									 float fThisWeight=1000f;
									 if (iMultiplier==2)
										  fThisWeight-=80f;

									 if (Bot.KiteDistance>0&&ObjectCache.Objects.IsPointNearbyMonsters(vThisZigZag, Bot.KiteDistance))
										  continue;

									 if (ObjectCache.Obstacles.Navigations.Any(obj => obj.Obstacletype.Value!=ObstacleType.Monster&&obj.TestIntersection(Bot.Character.Position, vThisZigZag, false)))
										  continue;

									 float distanceToPoint=vThisZigZag.Distance2D(Bot.Character.Position);
									 float distanceToTarget=vTargetLocation.Distance2D(Bot.Character.Position);

									 fThisWeight+=(distanceToTarget*10f);

									 // Use this one if it's more weight, or we haven't even found one yet, or if same weight as another with a random chance
									 if (fThisWeight>fHighestWeight)
									 {
										  fHighestWeight=fThisWeight;


										  vBestLocation=new Vector3(vThisZigZag.X, vThisZigZag.Y, MGP.GetHeight(vThisZigZag.ToVector2()));


										  if (!bAnyAvoidance)
												bFoundSafeSpotsFirstLoop=true;
									 }
								}
								// Can we raycast to the point at minimum?
						  }
						  // Loop through degrees
					 }
					 // Loop through multiplier
					 return vBestLocation;
				}


				// For "position-shifting" to navigate around obstacle SNO's
				private DateTime LastObstacleIntersectionTest=DateTime.Today;
				///<summary>
				///Checks bots movement flags then prioritizes all objects that are considered to be blocking.
				///</summary>
				public void ObstaclePrioritizeCheck(float range=20f)
				{

					 if (DateTime.Now.Subtract(LastObstacleIntersectionTest).TotalMilliseconds>1500)
					 {
						  this.RefreshMovementCache();

						  if (!this.IsMoving||this.currentMovementState.HasFlag(MovementState.WalkingInPlace|MovementState.None)||TargetMovement.BlockedMovementCounter>0)
						  {
								LastObstacleIntersectionTest=DateTime.Now;

								Vector3 CurrentPosition=Bot.Character.Position;

								//We want to get a new vector that is towards the direction of our destination
								//Vector3 IntersectionDestinationVector=MathEx.CalculatePointFrom(CurrentPosition, DestinationVector, range);

								//get collection of objects that pass the tests.
								var intersectingObstacles=Bot.Combat.NearbyObstacleObjects //ObjectCache.Obstacles.Values.OfType<CacheServerObject>()
																						  .Where(obstacle =>
																								!Bot.Combat.PrioritizedRAGUIDs.Contains(obstacle.RAGUID)//Only objects not already prioritized
																								&&obstacle.Obstacletype.HasValue
																								&&ObstacleType.Navigation.HasFlag(obstacle.Obstacletype.Value)//only navigation/intersection blocking objects!
																								&&obstacle.CentreDistance<=range //Only within range..
																								&&obstacle.BotIsFacing());
								//&&obstacle.TestIntersection(BotGridPoint, IntersectionDestinationPoint));



								if (intersectingObstacles.Any())
								{
									 var intersectingObjectRAGUIDs=(from objs in intersectingObstacles
																			  select objs.RAGUID);

									 Bot.Combat.PrioritizedRAGUIDs.AddRange(intersectingObjectRAGUIDs);
								}
						  }
					 }
				}



				private Vector3 currentpathvector_=Vector3.Zero;
				///<summary>
				///Trys to find current path by checking NavigationProvider and DungeonExplorer for any valid pathing.
				///</summary>
				public Vector3 CurrentPathVector
				{
					 get
					 {
						  if (NP.CurrentPath.Count>0&&currentpathvector_!=NP.CurrentPath.Current)
								currentpathvector_=NP.CurrentPath.Current;
						  else if (DE.CurrentRoute!=null&&DE.CurrentRoute.Count>0&&currentpathvector_!=DE.CurrentNode.NavigableCenter)
								currentpathvector_=DE.CurrentNode.NavigableCenter;
						  else
								currentpathvector_=Vector3.Zero;

						  return currentpathvector_;
					 }
				}


				private DateTime LastMGPUpdate=DateTime.MinValue;
				private int LastScenceUpdateOccured=0;
				public void UpdateSearchGridProvider(bool force=false)
				{
					 if (!ZetaDia.IsInGame||ZetaDia.IsLoadingWorld)
						  return;

					 //Enforce a time update rule and a position check
					 if (!force&&LastScenceUpdateOccured==Bot.Character.iSceneID)
						  return;

					 if (LastScenceUpdateOccured!=Bot.Character.iSceneID)
						  LastScenceUpdateOccured=Bot.Character.iSceneID;


					 if (!force&&DateTime.Now.Subtract(LastMGPUpdate).TotalMilliseconds<1000)
						  return;


					 Log("Updating Main Grid Provider", true);

					 try
					 {
						  Navigation.MGP.Update();
					 } catch
					 {
						  Log("MGP Update Exception Safely Handled!", true);
						  return;
					 }

					 LastMGPUpdate=DateTime.Now;
				}


				//Static Methods
				#region Static Movement Methods
				///<summary>
				///Ray Cast -- if no navcellflags parameter is given then it will use Navigator.Raycast -- else it uses ZetaDia.Physics.Raycast to test navcellflags
				///</summary>
				public static bool CanRayCast(Vector3 vStartLocation, Vector3 vDestination, NavCellFlags NavType=NavCellFlags.None)
				{
					 if (!NavType.Equals(NavCellFlags.None))
						  return ZetaDia.Physics.Raycast(vStartLocation, vDestination, NavType); //False means nothing hit
					 else
						  return !Navigator.Raycast(vStartLocation, vDestination); //True means nothing hit
				}

				// Quickly calculate the direction a vector is from ourselves, and return it as a float
				public static float FindDirection(Vector3 vStartLocation, Vector3 vTargetLocation, bool inRadian=false)
				{
					 Vector3 startNormalized=vStartLocation;
					 startNormalized.Normalize();
					 Vector3 targetNormalized=vTargetLocation;
					 targetNormalized.Normalize();

					 float angle=NormalizeRadian((float)Math.Atan2((vTargetLocation.Y-vStartLocation.Y), (vTargetLocation.X-vStartLocation.X)));
					 //MathEx.ToDegrees(NormalizeRadian((float)Math.Atan2(vTargetLocation.Y-vStartLocation.Y, vTargetLocation.X-vStartLocation.X)));



					 if (inRadian)
						  return angle;
					 else
						  return MathEx.ToDegrees(angle);
				}
				public static float NormalizeRadian(float radian)
				{
					 if (radian<0)
					 {
						  double mod=-radian;
						  mod%=Math.PI*2d;
						  mod=-mod+Math.PI*2d;
						  return (float)mod;
					 }
					 return (float)(radian%(Math.PI*2d));
				}
				#endregion


				//Static Encapsulated Navigator Properties
				#region Static Navigator Properties

				/// <summary>
				/// PathFinder
				/// </summary>
				public static Zeta.Pathfinding.PathFinder PathFinder
				{
					 get
					 {
						  return (Navigator.SearchGridProvider as Zeta.Pathfinding.PathFinder);
					 }
				}
				/// <summary>
				/// MainGridProvider
				/// </summary>
				public static MainGridProvider MGP
				{
					 get
					 {
						  return (Navigator.SearchGridProvider as MainGridProvider);
					 }
				}
				/// <summary>
				/// DungeonExplorer
				/// </summary>
				public static Zeta.CommonBot.Dungeons.DungeonExplorer DE
				{
					 get
					 {
						  return (Zeta.CommonBot.Logic.BrainBehavior.DungeonExplorer);
					 }
				}
				///<summary>
				///Returns GetNavigationProviderAs as DefaultNavigationProvider (Pathing)
				///</summary>
				public static DefaultNavigationProvider NP
				{
					 get
					 {
						  return Navigator.GetNavigationProviderAs<DefaultNavigationProvider>();
					 }
				}
				
				#endregion
		  }

	 }
}