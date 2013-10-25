using System;
using System.Linq;
using FunkyBot.Avoidances;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using Zeta;
using System.Collections.Generic;
using Zeta.Common;
using System.Windows;
using FunkyBot.Movement;

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
										  if (item.Obstacletype.HasValue&&item.Obstacletype.Value==ObstacleType.Monster)
										  {
												continue;
										  //	 if (!UsedRAGUIDs.Contains(item.RAGUID))
										  //	 {
										  //		  if (!Bot.Combat.UnitRAGUIDs.Contains(item.RAGUID)) continue;

										  //		  CacheUnit thisUnitObj;
										  //		  if (ObjectCache.Objects.TryGetValue(item.RAGUID, out thisUnitObj))
										  //		  {
										  //				int HealthPointAverageWeight=thisUnitObj.UnitMaxHitPointAverageWeight;
										  //				//Monsters who have high health will give more points.
										  //				if (HealthPointAverageWeight>0)
										  //				{
										  //					 this.ThisWeight+=(5*HealthPointAverageWeight);
										  //				}

										  //				if (thisUnitObj.IsEliteRareUnique)
										  //					 this.ThisWeight+=5;
										  //		  }

										  //		  this.ThisWeight+=5;
										  //		  monstercount++;
										  //		  UsedRAGUIDs.Add(item.RAGUID);
										  //	 }
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

												this.ThisWeight+=(BaseWeight/Bot.Character.dCurrentHealthPct);

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
					 public bool FindSafeSpot(Vector3 CurrentLocation,out Vector3 safespot, Vector3 LoSCheckV3, bool kite=false, bool checkBotAvoidIntersection=false)
					 {
						  bool checkLOS=LoSCheckV3!=Vector3.Zero;
						  
						  float averageZ=this.AverageAreaVectorZ;
							Vector3 botcurpos=(Bot.Character.Position);
						  float botCurrentZ=botcurpos.Z;
							bool ZHeightCheckPass=Funky.Difference(this.AverageAreaVectorZ, botCurrentZ)<1f;

							bool foundLocation=false;
							if (kite)
								 foundLocation=IteratePointsDescending(ZHeightCheckPass, botcurpos, kite, checkBotAvoidIntersection, LoSCheckV3!=Vector3.Zero, LoSCheckV3);
							else
								 foundLocation=IteratePointsAscending(ZHeightCheckPass, botcurpos, kite, checkBotAvoidIntersection, LoSCheckV3!=Vector3.Zero, LoSCheckV3);

							if (foundLocation)
							{
								 safespot=LastSafespotFound;
								 return true;
							}

						  LastSafespotFound=Vector3.Zero;
						  safespot=LastSafespotFound;
						  LastIndexUsed=kite==true?0:this.ContainedPoints.Count-1;
						  return false;
					 }

                     ///<summary>
                     ///Searches through the contained GridPoints and preforms multiple tests to return a successful point for navigation.
                     ///</summary>
                     public bool FindSafeSpot(Vector3 CurrentLocation, out Vector3 safespot, Vector3 LoSCheckV3, PointCheckingFlags Flags)
                     {
                         Func<Vector3, bool> PointChecker = CreatePointChecking(Flags,LoSCheckV3);

                         for (int curIndex = LastIndexUsed; curIndex < ContainedPoints.Count - 1; curIndex++)
                         {
                             Vector3 gpV3 = (Vector3)ContainedPoints[curIndex];
                             if (!CheckPoint(gpV3, PointChecker)) continue;

                             //PointChecker passed!
                             LastIndexUsed = curIndex;
                             LastSafespotFound = gpV3;
                             LastSafeGridPointFound = ContainedPoints[curIndex];
                             safespot = gpV3;
                             return true;
                         }

                         LastSafespotFound = Vector3.Zero;
                         safespot = LastSafespotFound;
                         LastIndexUsed = 0;
                         return false;
                     }

                    private bool CheckPoint(Vector3 point, Func<Vector3,bool> PointChecker)
                     {
                         foreach (Func<Vector3, bool> item in PointChecker.GetInvocationList())
                         {
                             if (!item.Invoke(point)) return false;
                         }

                         return true;
                     }

					 private bool IteratePointsAscending(bool ZHeightCheckPass, Vector3 botcurpos,bool kite, bool checkBotAvoidIntersection, bool checkLOS, Vector3 LoSCheckV3)
					 {
						  for (int curIndex=LastIndexUsed; curIndex<ContainedPoints.Count-1; curIndex++)
						  {
								if (CheckPoint(ContainedPoints[curIndex], ZHeightCheckPass, botcurpos, kite, checkBotAvoidIntersection, checkLOS, LoSCheckV3))
								{
									 LastIndexUsed=curIndex;
									 return true;
								}
						  }

						  return false;
					 }
					 private bool IteratePointsDescending(bool ZHeightCheckPass, Vector3 botcurpos, bool kite, bool checkBotAvoidIntersection, bool checkLOS, Vector3 LoSCheckV3)
					 {
						  for (int curIndex=LastIndexUsed; curIndex>0; curIndex--)
						  {
								if (CheckPoint(ContainedPoints[curIndex], ZHeightCheckPass, botcurpos, kite, checkBotAvoidIntersection, checkLOS, LoSCheckV3))
								{
									 LastIndexUsed=curIndex;
									 return true;
								}
						  }

						  return false;
					 }

                     private Func<Vector3, bool> CreatePointChecking(PointCheckingFlags flags, Vector3 LoSCheckV3)
                     {
                         Func<Vector3, bool> pointChecker = new Func<Vector3, bool>((gp) => 
                         {
                             if (!Navigation.CanRayCast(Bot.Character.Position, gp)) return false;
                             if (!Navigation.MGP.CanStandAt(gp)) return false;
                             return true; 
                         });

                         //add additional delegates
                         if (!flags.Equals(PointCheckingFlags.None))
                         {
                             if (flags.HasFlag(PointCheckingFlags.AvoidanceIntersection))
                             {
                                 pointChecker += new Func<Vector3, bool>((gp) =>
                                 {
                                     return ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(Bot.Character.Position, gp);
                                 });
                             }
                             if (flags.HasFlag(PointCheckingFlags.AvoidanceOverlap))
                             {
                                 pointChecker += new Func<Vector3, bool>((gp) =>
                                 {
                                     return ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(gp);
                                 });
                             }
                             if (flags.HasFlag(PointCheckingFlags.MonsterOverlap))
                             {
                                 pointChecker += new Func<Vector3, bool>((gp) =>
                                 {
                                     return ObjectCache.Objects.OfType<CacheUnit>().Any(m => m.ShouldBeKited && m.IsPositionWithinRange(gp, Bot.Settings.Fleeing.FleeMaxMonsterDistance));
                                 });
                             }
                             if (flags.HasFlag(PointCheckingFlags.ObstacleOverlap))
                             {
                                 pointChecker += new Func<Vector3, bool>((gp) =>
                                 {
                                     return ObjectCache.Obstacles.Values.OfType<CacheServerObject>().Any(obj => ObstacleType.Navigation.HasFlag(obj.Obstacletype.Value) && obj.PointInside(gp));
                                 });
                             }
                             if (flags.HasFlag(PointCheckingFlags.Raycast))
                             {
                                 pointChecker += new Func<Vector3, bool>((gp) =>
                                 {
                                     return true;
                                 });
                             }
                         }

                         if (LoSCheckV3!=Vector3.Zero)
                         {
                             pointChecker += new Func<Vector3, bool>((gp) =>
                             {
                                 return !Navigation.CanRayCast(gp, LoSCheckV3, UseSearchGridProvider: true);
                             });
                         }

                         return pointChecker;
                     }

                     private bool CheckPoint(GridPoint point, bool ZHeightCheckPass, Vector3 botcurpos, bool kite, bool checkBotAvoidIntersection, bool checkLOS, Vector3 LoSCheckV3)
					 {
						  //Check blacklisted points and ignored
						  if (point.Ignored) return false;

						  //2D Obstacle Navigation Check
						  bool ZCheck=false;
						  if (ZHeightCheckPass&&this.AreaIsFlat)
						  {
								if (ObjectCache.Obstacles.Values.OfType<CacheServerObject>().Any(obj => ObstacleType.Navigation.HasFlag(obj.Obstacletype.Value)&&obj.PointInside(point))) return false;
								ZCheck=true;
						  }

						  //Create Vector3
						  Vector3 pointVectorReturn=(Vector3)point;
						  Vector3 pointVector=pointVectorReturn;

						  //Check if we already within this "point".
						  if (botcurpos.Distance(pointVector)<2.5f) return false;

						  //3D Obstacle Navigation Check
						  if (!ZCheck)
						  {
								//Because Z Variance we need to check if we can raycast walk to the location.
								if (!Navigation.CanRayCast(botcurpos, pointVector)) return false;
								if (!Navigation.MGP.CanStandAt(pointVector)) return false;
								if (ObjectCache.Obstacles.Values.OfType<CacheServerObject>().Any(obj => ObstacleType.Navigation.HasFlag(obj.Obstacletype.Value)&&obj.PointInside(pointVector))) return false;
						  }

						  //LOS Check
						  if (checkLOS)
						  {
								if (!Navigation.CanRayCast(pointVector, LoSCheckV3, UseSearchGridProvider: true)) return false;
						  }

						  //Avoidance Check (Any Avoidance)
						  if (ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(pointVector)) return false;

						  //Kiting Check
						  if (kite&&ObjectCache.Objects.OfType<CacheUnit>().Any(m => m.ShouldBeKited&&m.IsPositionWithinRange(pointVector, Bot.Settings.Fleeing.FleeMaxMonsterDistance))) return false;

						  //Avoidance Intersection Check
						  if (checkBotAvoidIntersection&&ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(botcurpos, pointVector)) return false;

						  LastSafespotFound=pointVectorReturn;
						  LastSafeGridPointFound=point.Clone();
						  return true;
					 }
				}
		  
	 
}