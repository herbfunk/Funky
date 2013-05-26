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
		  public static partial class GridPointAreaCache
		  {
				///<summary>
				///GridPoint Rectangle 
				///</summary>
				public class GPQuadrant
				{
					 internal readonly List<GridPoint> ContainedPoints=new List<GridPoint>();
					 private int LastIndexUsed=0;

					 public float AverageAreaVectorZ { get; set; }
					 public readonly bool AreaIsFlat;
					 //public DirectionPoint Direction { get; set; }
					 public readonly int SectorCode;

					 public Vector3 LastSafespotFound=vNullLocation;
					 public GridPoint LastSafeGridPointFound { get; set; }

					 public readonly Vector3 CenterVector;
					 private GridPoint CenterPoint
					 {
						  get
						  {
								return CenterVector;
						  }
					 }

					 public GridPoint CornerPoint { get; set; }
					 public GridPoint StartPoint { get; set; }

					 public Rect RectangleArea { get; set; }
					 public Vector3 Center
					 {
						  get
						  {
								return (Vector3)(GridPoint.GetCenteroid(StartPoint, CornerPoint));
						  }
					 }

					 public double ThisWeight { get; set; }
					 public bool CheckedAllPoints
					 {
						  get
						  {
								return (this.LastIndexUsed==ContainedPoints.Count-1);
						  }
					 }

					 public List<int> OccupiedObjects=new List<int>();




					 public GPQuadrant(int Sector)
						  : base()
					 {
						  SectorCode=Sector;

					 }
					 public GPQuadrant(int Sector, GridPoint[] points, Vector3 GPCenteringVector, GridPoint endpoint)
					 {
						  this.CenterVector=GPCenteringVector;
						  this.SectorCode=Sector;
						  this.ContainedPoints.AddRange(points);
						  this.StartPoint=this.CenterPoint.Clone();
						  this.CornerPoint=endpoint.Clone();
						  this.RectangleArea=new Rect(this.StartPoint, endpoint);

						  //Check for a boundry?
						  if (this.CornerPoint.Ignored)
						  {

						  }

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
						  IEnumerable<CacheObstacle> obstaclesContained=ObjectCache.Obstacles.Values.Where(obs => Math.Abs(sectorCenter.Distance2D(obs.Position)-obs.Radius)<=range);

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
														  if (thismaxhp>maxaverage)
														  {
																double multiplier=(thismaxhp/maxaverage);
																this.ThisWeight+=(10*multiplier);
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

												AvoidanceType thisAvoidanceType=((CacheAvoidance)item).AvoidanceType;
												if (thisAvoidanceType==AvoidanceType.ArcaneSentry||thisAvoidanceType==AvoidanceType.Dececrator||thisAvoidanceType==AvoidanceType.MoltenCore)
													 this.ThisWeight+=15;
												else
													 this.ThisWeight+=10;

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

					 public bool FindSafeSpot(out Vector3 safespot, Vector3 LoSCheckV3, bool kite=false, bool checkBotAvoidIntersection=false)
					 {
						  bool checkLOS=LoSCheckV3!=vNullLocation;
						  
						  float averageZ=this.AverageAreaVectorZ;
						  Vector3 botcurpos=(Bot.Character.Position);
						  float botCurrentZ=botcurpos.Z;
						  botcurpos.Z+=Bot.Character.PickupRadius/2f;
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
								Vector3 pointVector=(Vector3)point;

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
								if (checkLOS&&!GilesCanRayCast(pointVector, LoSCheckV3, Zeta.Internals.SNO.NavCellFlags.AllowWalk)) continue;

								//Avoidance Intersection Check
								if (checkBotAvoidIntersection&&ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(botcurpos, pointVector)) continue;


								LastSafespotFound=pointVector;
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