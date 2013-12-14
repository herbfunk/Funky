using System;
using System.Linq;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using System.Collections.Generic;
using FunkyBot.Cache.Objects;
using Zeta.Common;

namespace FunkyBot.Movement
{


				///<summary>
				///GridPoint Rectangle 
				///</summary>
				public class GPQuadrant
				{
					 internal readonly List<GridPoint> ContainedPoints=new List<GridPoint>();
					 private int LastIndexUsed=0;

					 public float AverageAreaVectorZ { get; set; }
					 ///<summary>
					 ///Logic: The average Z difference between all points is very *minimal
					 ///</summary>
					 public readonly bool AreaIsFlat;


					 public Vector3 LastSafespotFound=Vector3.Zero;
					 public GridPoint LastSafeGridPointFound { get; set; }


					 public GridPoint CornerPoint { get; set; }
					 public GridPoint StartPoint { get; set; }

					 public Vector3 Center
					 {
						  get
						  {
								return (Vector3)(GridPoint.GetCenteroid(StartPoint, CornerPoint));
						  }
					 }

					 public double ThisWeight { get; set; }

					 public List<int> OccupiedObjects=new List<int>();




					 public GPQuadrant()
						  : base()
					 {
						  
					 }
					 public GPQuadrant(GridPoint[] points, Vector3 GPCenteringVector, GridPoint endpoint)
					 {
						  this.ContainedPoints.AddRange(points);
						  this.StartPoint=GPCenteringVector;
						  this.CornerPoint=endpoint.Clone();
						 

						  if (points.Length>0)
						  {
								this.AverageAreaVectorZ=this.ContainedPoints.Sum(gp => gp.Z)/this.ContainedPoints.Count;
								this.AreaIsFlat=(this.AverageAreaVectorZ==this.ContainedPoints.First().Z);
						  }
					 }




					 public double UpdateWeight(out int monstercount, out int avoidcount, ref List<int> UsedRAGUIDs, bool ResetIndex=false)
					 {
						  monstercount=0;
						  avoidcount=0;
						  if (ResetIndex) LastIndexUsed=0;

						  OccupiedObjects.Clear();

						  Vector3 sectorCenter=this.Center;
						  //Get the Diagonal Length between start and end, multiply by 2.5 since each point represents an area of 5f than Divide the total by 2 for the radius range.
						  double range=GridPoint.GetDistanceBetweenPoints(this.StartPoint, this.CornerPoint);

						  int TotalGridPoints=this.ContainedPoints.Count;
						  this.ThisWeight=0d;


						  //We use 2D Distance and subtract the obstacles radius
						  IEnumerable<CacheObstacle> obstaclesContained=ObjectCache.Obstacles.Values
								.Where(obs => Math.Max(0f,sectorCenter.Distance2D(obs.Position)-obs.Radius)<=range);

						  double maxaverage=ObjectCache.Objects.MaximumHitPointAverage;
						  if (obstaclesContained.Any())
						  {
								//reset weight
								this.ThisWeight=0;

								//copy SectorPoints
								//GridPoint[] SectorPoints=new GridPoint[this.ContainedPoints.Count];
								//this.ContainedPoints.CopyTo(SectorPoints);

								List<GridPoint> NonNavPoints=new List<GridPoint>();

								foreach (CacheObstacle item in obstaclesContained)
								{
									 OccupiedObjects.Add(item.RAGUID);

									 if (item is CacheServerObject)
									 {

										  //Monsters should add 10% of its weight
										  //if (item.Obstacletype.Value==ObstacleType.Monster)
										  //{
										  //	if (Bot.Settings.Fleeing.EnableFleeingBehavior&& Bot.Targeting.Environment.FleeTrigeringRAGUIDs.Contains(item.RAGUID))
										  //	{

										  //	}
												 
										  //}
										  if(item.Obstacletype.Value == ObstacleType.ServerObject)
										  {
											  //give +1 to weight
											  this.ThisWeight++;
										  }
									 }
									 else if (item is CacheAvoidance)
									 {
										  if (!UsedRAGUIDs.Contains(item.RAGUID))
										  {
												AvoidanceType thisAvoidanceType=((CacheAvoidance)item).AvoidanceType;
												if (AvoidanceCache.IgnoringAvoidanceType(thisAvoidanceType)) continue;
											   AvoidanceValue AV= Bot.Settings.Avoidance.Avoidances[(int)thisAvoidanceType];

												avoidcount++;
												float BaseWeight=AV.Weight;
												
												//if ((AvoidanceType.ArcaneSentry|AvoidanceType.Dececrator|AvoidanceType.MoltenCore|AvoidanceType.TreeSpore).HasFlag(thisAvoidanceType))
												//	 BaseWeight=1f;
												//else
												//	 BaseWeight=0.5f;

												this.ThisWeight+=(BaseWeight/Bot.Character.Data.dCurrentHealthPct);

												UsedRAGUIDs.Add(item.RAGUID);
										  }
									 }
								}

								////Now add a base score for non-nav points. (25 being 100% non-navigable)
								//int PointMultiplier=(25/TotalGridPoints);
								//int RemainingPoints=SectorPoints.Length;
								//this.ThisWeight+=25-(RemainingPoints*PointMultiplier);


								//Logging.WriteVerbose("Weight assigned to this sector {0}. \r\n"
								//+"Total Points {1} with {2} Remaining points Valid!", this.ThisWeight, this.ContainedPoints.Count, SectorPoints.Length);
						  }

						  return this.ThisWeight;

						  //(Total Points / Non-Navigable Points Ratio)
					 }

                     ///<summary>
                     ///Searches through the contained GridPoints and preforms multiple tests to return a successful point for navigation.
                     ///</summary>
                     public bool FindSafeSpot(Vector3 CurrentLocation, out Vector3 safespot, Vector3 LoSCheckV3, PointCheckingFlags Flags, List<GridPoint> BlacklistedPoints)
                     {
                         bool PassedPointNonLOS = false;
                         bool CheckingLOSV3 = LoSCheckV3 != Vector3.Zero;
                         for (int curIndex = LastIndexUsed; curIndex < ContainedPoints.Count - 1; curIndex++)
                         {
                             GridPoint gp=ContainedPoints[curIndex];
                             if (BlacklistedPoints.Contains(gp)) continue;
                             if (!CheckPoint(gp, LoSCheckV3, Flags)) continue;

                             //LOS Check
                             if (CheckingLOSV3)
                             {
                                 if (!Navigation.CanRayCast(LastSafespotFound, LoSCheckV3, UseSearchGridProvider: true))
                                 {
                                     LastIndexUsed = curIndex;
                                     PassedPointNonLOS = true;
                                     continue;
                                 }
                             }
                          

                             //PointChecker passed!
                             LastIndexUsed = curIndex;
                             safespot = LastSafespotFound;
                             return true;
                         }

                         //Did we find a valid point that didn't pass LOS?
                         if (PassedPointNonLOS)
                         {
                             safespot = LastSafespotFound;
                             return true;
                         }

                         LastSafespotFound = Vector3.Zero;
                         safespot = LastSafespotFound;
                         LastIndexUsed = 0;
                         return false;
                     }

                    
                     private bool CheckPoint(GridPoint point, Vector3 LoSCheckV3, PointCheckingFlags flags)
                     {
                         //Check blacklisted points and ignored
                         if (point.Ignored) return false;

                         //Check if this point is in a blocked direction
                         if (flags.HasFlag(PointCheckingFlags.BlockedDirection))
                         {
                             if (Bot.NavigationCache.CheckPointAgainstBlockedDirection(point)) return false;
                         }

                         //2D Obstacle Navigation Check
                         bool ZCheck = false;
                         if (this.AreaIsFlat)
                         {
                             if (flags.HasFlag(PointCheckingFlags.ObstacleOverlap))
                             {
                                 if (ObjectCache.Obstacles.Values.OfType<CacheServerObject>().Any(obj => ObstacleType.Navigation.HasFlag(obj.Obstacletype.Value) && obj.PointInside(point))) return false;
                             }
                            
                             ZCheck = true;
                         }

                         //Create Vector3
                         Vector3 pointVectorReturn = (Vector3)point;
                         Vector3 pointVector = pointVectorReturn;
                         Vector3 botcurpos = Bot.Character.Data.Position;

                         //Check if we already within this "point".
                         if (botcurpos.Distance2D(pointVector) < 2.5f) return false;

                         //3D Obstacle Navigation Check
                         if (!ZCheck)
                         {
                             //Because Z Variance we need to check if we can raycast walk to the location.
                             if (!Navigation.CanRayCast(botcurpos, pointVector)) return false;
                             if (!Navigation.MGP.CanStandAt(pointVector)) return false;

                             if (flags.HasFlag(PointCheckingFlags.ObstacleOverlap))
                             {
                                 if (ObjectCache.Obstacles.Values.OfType<CacheServerObject>().Any(obj => ObstacleType.Navigation.HasFlag(obj.Obstacletype.Value) && obj.PointInside(pointVector))) return false;
                             }
                            
                         }


                         //Avoidance Check (Any Avoidance)
                         if (flags.HasFlag(PointCheckingFlags.AvoidanceOverlap))
                         {
                             if (ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(pointVector)) return false;
                         }
                       

                         //Kiting Check
                         if (flags.HasFlag(PointCheckingFlags.MonsterOverlap))
                         {
                             if (ObjectCache.Objects.OfType<CacheUnit>().Any(m => m.ShouldFlee && m.IsPositionWithinRange(pointVector, Bot.Settings.Fleeing.FleeMaxMonsterDistance))) return false;
                         }
                      

                         //Avoidance Intersection Check
                         if (flags.HasFlag(PointCheckingFlags.AvoidanceIntersection))
                         {
                             if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(botcurpos, pointVector)) return false;
                         }

                         if (flags.HasFlag(PointCheckingFlags.Raycast))
                         {
                             if (!Navigation.CanRayCast(botcurpos, pointVector)) return false;
                         }
                         if (flags.HasFlag(PointCheckingFlags.RaycastWalkable))
                         {
                             if (!Navigation.CanRayCast(botcurpos, pointVector, Zeta.Internals.SNO.NavCellFlags.AllowWalk)) return false;
                         }
                         if (flags.HasFlag(PointCheckingFlags.RaycastNavProvider))
                         {
                             if (!Navigation.CanRayCast(botcurpos, pointVector, UseSearchGridProvider: true )) return false;
                         }

                         LastSafespotFound = pointVectorReturn;
                         LastSafeGridPointFound = point.Clone();
                         return true;
                     }
				}
		  
	 
}