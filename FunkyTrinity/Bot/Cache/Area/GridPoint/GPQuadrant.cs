using System;
using System.Linq;
using Zeta;
using System.Collections.Generic;
using Zeta.Common;
using System.Windows;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  internal static partial class GridPointAreaCache
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


					 public Vector3 LastSafespotFound=vNullLocation;
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
						  if (ResetIndex)
								LastIndexUsed=0;

						  OccupiedObjects.Clear();

						  Vector3 sectorCenter=this.Center;
						  //Get the Diagonal Length between start and end, multiply by 2.5 since each point represents an area of 5f than Divide the total by 2 for the radius range.
						  double range=(GridPoint.GetDistanceBetweenPoints(this.StartPoint, this.CornerPoint)*2.5)/2;

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
								GridPoint[] SectorPoints=new GridPoint[this.ContainedPoints.Count];
								this.ContainedPoints.CopyTo(SectorPoints);

								List<GridPoint> NonNavPoints=new List<GridPoint>();

								foreach (CacheObstacle item in obstaclesContained)
								{
									 OccupiedObjects.Add(item.RAGUID);

									 if (item is CacheServerObject)
									 {

										  //Monsters should add 10% of its weight
										  if (item.Obstacletype.HasValue&&item.Obstacletype.Value==ObstacleType.Monster)
										  {

												if (!UsedRAGUIDs.Contains(item.RAGUID))
												{
													 CacheUnit thisUnitObj;
													 if (ObjectCache.Objects.TryGetValue(item.RAGUID, out thisUnitObj))
													 {

														  double thismaxhp=thisUnitObj.MaximumHealth.Value;

														  //Monsters who have high health will give more points.
														  if (thisUnitObj.UnitMaxHitPointAverageWeight>0)
														  {
																double multiplier=(thismaxhp/maxaverage);
																this.ThisWeight+=(5*multiplier);
														  }

														  if (thisUnitObj.IsEliteRareUnique)
																this.ThisWeight+=5;


														  if (thisUnitObj.CurrentHealthPct.Value>0.75d)
																this.ThisWeight+=5;
													 }

													 this.ThisWeight+=5;
													 monstercount++;
													 UsedRAGUIDs.Add(item.RAGUID);


												}
										  }


										  //Since server objects occupy space, we eliminate points
										  foreach (var point in SectorPoints)
										  {
												if (item.PointInside(point))
												{
													 NonNavPoints.Add(point);
												}
										  }
										  GridPoint[] NewSectorPointArray=new GridPoint[SectorPoints.Length];
										  SectorPoints.CopyTo(NewSectorPointArray, 0);
										  SectorPoints=Array.FindAll(NewSectorPointArray, P => !NonNavPoints.Contains(P));
									 }
									 else if (item is CacheAvoidance)
									 {
										  if (!UsedRAGUIDs.Contains(item.RAGUID))
										  {
												avoidcount++;
												float BaseWeight=0f;

												AvoidanceType thisAvoidanceType=((CacheAvoidance)item).AvoidanceType;
												if (thisAvoidanceType==AvoidanceType.ArcaneSentry||thisAvoidanceType==AvoidanceType.Dececrator||thisAvoidanceType==AvoidanceType.MoltenCore)
													 BaseWeight=2.5f;
												else
													 BaseWeight=1f;

												//Multiply the avoidance by health value (Max value possible 2500 -- 1% HP using 25)
												this.ThisWeight+=(BaseWeight/Bot.Character.dCurrentHealthPct);

												UsedRAGUIDs.Add(item.RAGUID);
										  }
									 }
								}

								//Now add a base score for non-nav points. (25 being 100% non-navigable)
								int PointMultiplier=(25/TotalGridPoints);
								int RemainingPoints=SectorPoints.Length;
								this.ThisWeight+=25-(RemainingPoints*PointMultiplier);


								//Logging.WriteVerbose("Weight assigned to this sector {0}. \r\n"
								//+"Total Points {1} with {2} Remaining points Valid!", this.ThisWeight, this.ContainedPoints.Count, SectorPoints.Length);
						  }

						  return this.ThisWeight;

						  //(Total Points / Non-Navigable Points Ratio)
					 }

					 ///<summary>
					 ///Searches through the contained GridPoints and preforms multiple tests to return a successful point for navigation.
					 ///</summary>
					 public bool FindSafeSpot(out Vector3 safespot, Vector3 LoSCheckV3, bool kite=false, bool checkBotAvoidIntersection=false)
					 {
						  bool checkLOS=LoSCheckV3!=vNullLocation;
						  
						  float averageZ=this.AverageAreaVectorZ;
						  Vector3 botcurpos=(Bot.Character.Position);
						  float botCurrentZ=botcurpos.Z;
						  bool ZHeightCheckPass=Difference(this.AverageAreaVectorZ, botCurrentZ)<1f;

						  for (int curIndex=LastIndexUsed; curIndex<ContainedPoints.Count-1; curIndex++)
						  {
								GridPoint point=ContainedPoints[curIndex];
								//Check blacklisted points and ignored
								if (GridPointAreaCache.BlacklistedGridpoints.Contains(point)||point.Ignored) continue;

								//2D Obstacle Navigation Check
								bool ZCheck=false;
								if (ZHeightCheckPass&&this.AreaIsFlat)
								{
									 if (ObjectCache.Obstacles.Values.OfType<CacheServerObject>().Any(obj => ObstacleType.Navigation.HasFlag(obj.Obstacletype.Value)&&obj.PointInside(point))) continue;
									 ZCheck=true;
								}

								//Create Vector3
								Vector3 pointVectorReturn=(Vector3)point;
								Vector3 pointVector=pointVectorReturn;
								pointVector.Z+=Bot.Character.fCharacterRadius/2f;

								//Check if we already within this "point".
								if (botcurpos.Distance(pointVector)<2.5f) continue;

								//3D Obstacle Navigation Check
								if (!ZCheck)
								{
									 if (ObjectCache.Obstacles.Values.OfType<CacheServerObject>().Any(obj => ObstacleType.Navigation.HasFlag(obj.Obstacletype.Value)&&obj.PointInside(pointVector))) continue;

									 //Because Z Variance we need to check if we can raycast walk to the location.
									 if (!GilesCanRayCast(botcurpos, pointVector, Zeta.Internals.SNO.NavCellFlags.AllowWalk)) continue;
								}

								//Avoidance Check (Any Avoidance)
								if (ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(pointVector)) continue;

								//Kiting Check
								if (kite&&ObjectCache.Objects.IsPointNearbyMonsters(pointVector, Bot.Class.KiteDistance)) continue;

								//LOS Check
								//Melee -- Walkable , Range -- Projectile
								if (checkLOS)
								{
									 if (Bot.Class.IsMeleeClass)
									 {
										  if (!GilesCanRayCast(pointVector, LoSCheckV3, Zeta.Internals.SNO.NavCellFlags.AllowWalk)) continue;
									 }
									 else
									 {
										  if (!GilesCanRayCast(pointVector, LoSCheckV3, Zeta.Internals.SNO.NavCellFlags.AllowWalk|Zeta.Internals.SNO.NavCellFlags.AllowProjectile)) continue;
									 }
								}

								//Avoidance Intersection Check
								if (checkBotAvoidIntersection&&ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(botcurpos, pointVector)) continue;


								LastSafespotFound=pointVectorReturn;
								safespot=LastSafespotFound;
								LastSafeGridPointFound=point.Clone();
								LastIndexUsed=curIndex;
								return true;
						  }

						  LastSafespotFound=vNullLocation;
						  safespot=LastSafespotFound;
						  LastIndexUsed=this.ContainedPoints.Count-1;
						  return false;
					 }

				}
		  }
	 }
}