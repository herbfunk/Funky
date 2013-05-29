﻿using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using System.Collections.Generic;
using System.Collections;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  public static partial class GridPointAreaCache
		  {
				///<summary>
				///GridPointCircle
				///A collection of GridPoints that is surrounding a given vector3.
				///Allows safespot finding and will iterate thru its sectors to find any safespots contained within.
				///</summary>
				public class GPRectangle : PointCollection
				{
					 ///<summary>
					 ///Creates a new point collection from center point. 
					 ///Area will be sqrt(1 + expansionCount)
					 ///Points are sorted by the quadrant it lays in using center point as the testing point.
					 ///</summary>
					 public GPRectangle(Vector3 center, int expansionCount=2)
						  : base()
					 {
						  CreationVector=center;
						  GridPoint center_=center;
						  searchablepoints_=new List<GridPoint>();
						  searchablepoints_.Add(center_);//this is our first "surrounding" point to expand upon..
						  this.centerpoint=center;

						  //This will expand by finding new surrounding points
						  for (int i=0; i<expansionCount-1; i++)
						  {
								if (!this.CanExpandFurther) break; //no remaining searchable points..
								this.FullyExpand();
						  }

						  this.UpdateQuadrants();

						  // Logging.WriteVerbose(debugstr);
						  UpdateObjectCount();
						  lastRefreshedObjectContents=DateTime.Now;
					 }

					 ///<summary>
					 ///Using a direction point. Center will be established at the DirectionPoint center, with an area covering the start/end points.
					 ///</summary>
					 public GPRectangle(DirectionPoint Direction)
						  : base()
					 {
						  // Logging.WriteVerbose("StartPoint: {0} EndPoint {1} Range: {2}", Direction.StartingPoint.ToString(), Direction.EndingPoint.ToString(), Direction.Range);

						  CreationDirectionPoint=Direction;
						  CreationVector=(Vector3)Direction.Center;
						  GridPoint center_=Direction.Center;
						  searchablepoints_=new List<GridPoint>();
						  searchablepoints_.Add(center_);//this is our first "surrounding" point to expand upon..
						  this.centerpoint=Direction.Center;


						  int expansion=(int)(Math.Round(Math.Sqrt(Direction.Range), MidpointRounding.AwayFromZero)); //Get the radius value and square it to get # of expansions we want..
						  //Logging.WriteVerbose("Expansion count == {0}, Diameter == {1}", expansion, Diameter);

						  //This will expand by finding new surrounding points
						  for (int i=0; i<expansion; i++)
						  {
								if (searchablepoints_.Count==0) break; //no remaining searchable points..

								this.FullyExpand();
						  }

						  this.UpdateQuadrants();

						  UpdateObjectCount();
						  lastRefreshedObjectContents=DateTime.Now;
						  /*
						  //Since we used a DirectionPoint to establish this.. we also want to a quick 90 degree interval direction point search.
						  //These DPs will be used to evaluate against other GPCs for which would be best to use overall.
						  List<DirectionPoint> DPs=new List<DirectionPoint>();
						  float Radians=MathEx.ToRadians(Direction.DirectionDegrees);
						  for (int i=1; i<2; i++)
						  {
								float DegreesPOS=MathEx.ToDegrees(NormalizeRadian(Radians+MathEx.ToRadians(45*i)));
								DPs.Add(new DirectionPoint(CreationVector, DegreesPOS));
								float DegreesNEG=MathEx.ToDegrees(NormalizeRadian(Radians+MathEx.ToRadians(-45*i)));
								DPs.Add(new DirectionPoint(CreationVector, DegreesNEG));
						  }
						  Directions=new List<DirectionPoint>(DPs.OrderByDescending(dp => dp.Range).ToArray());
						  */
					 }

					 private void UpdateQuadrants()
					 {
						  QuadrantLocation[] sectors_=Quadrant.Keys.ToArray();
						  string debugstring="";

						  foreach (var item in sectors_)
						  {
								GridPoint cornerPoint;
								GridPoint[] sectorpoints=this.QuadrantPoints(item, out cornerPoint);
								if (sectorpoints==null||cornerPoint==null)
								{
									 this.Quadrant[item]=null;
									 continue;
								}
								//Logging.WriteVerbose("Sectorpoints Count {0}", sectorpoints.Length);
								Quadrant[item]=new GPQuadrant((int)item, sectorpoints, CreationVector, cornerPoint);

								//item = new Sector(Points.SectorPoints(item.SectorCode, true));
								debugstring+="SectorID: "+item.ToString()+" "+Quadrant[item].ContainedPoints.Count+",";
						  }

						  if (SettingsFunky.LogSafeMovementOutput)
								Logging.WriteDiagnostic(debugstring);
					 }


					 private GPQuadrant lastUsedQuadrant { get; set; }

					 //Represents Sectors that use 90 angle to divide into 4 parts.
					 private Dictionary<QuadrantLocation, GPQuadrant> Quadrant=new Dictionary<QuadrantLocation, GPQuadrant> 
					 {												 //"End Point" of the sector
								{QuadrantLocation.BottomLeft,new GPQuadrant(5)},			 //BottomLeft
								{QuadrantLocation.BottomRight,new GPQuadrant(6)},			 //BottomRight
								{QuadrantLocation.TopLeft, new GPQuadrant(9)},			 //TopLeft
								{QuadrantLocation.TopRight, new GPQuadrant(10)}			 //TopRight
					 };

					 public readonly Vector3 CreationVector;
					 public GridPoint Center { get; set; }
					 public double Radius
					 {
						  get
						  {
								return this.DiagonalLength/2;
						  }
					 }

					 private readonly DirectionPoint CreationDirectionPoint;
					 public readonly List<DirectionPoint> Directions;
					 private Dictionary<GridPoint, GPRectangle> DirectionGPR=new Dictionary<GridPoint, GPRectangle>();

					 public double DirectionPointsTotalWeight
					 {
						  get
						  {
								double weight=(from points in Directions
													select points.Range).Sum();
								return weight;
						  }
					 }
					 public GridPoint LastFoundSafeSpot
					 {
						  get
						  {
								if (lastUsedQuadrant!=null)
									 return lastUsedQuadrant.LastSafeGridPointFound;
								else
									 return null;
						  }
					 }
					 private List<GridPoint> CenterConnectingPoints=new List<GridPoint>();
					 internal List<GridPoint> searchablepoints_; //the "outer" points of the collection in which we use to search and expand.
					 private DateTime lastRefreshedObjectContents=DateTime.Today;
					 internal bool AllQuadrantsFailed=false;

					 //info about this circle related to objects contained
					 public int MonsterCount=0;
					 public int AvoidanceCount=0;
					 public double Weight { get; set; }

					 internal void UpdateObjectCount()
					 {
						  if (DateTime.Now.Subtract(lastRefreshedObjectContents).TotalMilliseconds>900)
						  {
								lastRefreshedObjectContents=DateTime.Now;
								this.AllQuadrantsFailed=false;
								this.Weight=0;
								//MonsterCount=ObjectCache.Objects.OfType<CacheUnit>().Count(monster => CreationVector.Distance2D(monster.Position)<=this.Radius);
								//AvoidanceCount=ObjectCache.Obstacles.Avoidances.Count(avoidance => CreationVector.Distance2D(avoidance.Position)<=this.Radius);
								int monsters, avoids;
								this.MonsterCount=0;
								this.AvoidanceCount=0;

								List<int> OccupiedRAGUIDS=new List<int>();

								foreach (var item in Quadrant.Values)
								{
									 if (item==null) continue;

									 this.Weight+=item.UpdateWeight(out monsters, out avoids, ref OccupiedRAGUIDS);
									 this.MonsterCount+=monsters;
									 this.AvoidanceCount+=avoids;

									 OccupiedRAGUIDS=OccupiedRAGUIDS.Union(item.OccupiedObjects).ToList();
								}

								var SectorOccupiedLists=(from sector in Quadrant.Values
																 select sector.OccupiedObjects);

								if (SettingsFunky.LogSafeMovementOutput)
									 Logging.WriteDiagnostic("Updated GPC with Avoids {0} / Mobs {1} Count and total weight {2}", this.AvoidanceCount, this.MonsterCount, this.Weight);
						  }
					 }

					 public bool TryFindSafeSpot(out Vector3 safespot, Vector3 los, bool kite=false, bool checkAvoidIntersection=false)
					 {
						  this.UpdateObjectCount();

						  lastUsedQuadrant=null;
						  safespot=vNullLocation;

						  //Do not continue search if all sectors failed recently.
						  if (AllQuadrantsFailed)
								return false;


						  foreach (var item in Quadrant.Values)
						  {
								if (item==null) continue;

								if (item.FindSafeSpot(out safespot, los, kite, checkAvoidIntersection))
								{
									 lastUsedQuadrant=item;
									 return true;
								}
						  }


						  AllQuadrantsFailed=true;

						  if (CanExpandFurther)
						  {
								//Logging.WriteVerbose("Expanding GPC due to failure to find a location!");
								this.FullyExpand();
								this.UpdateQuadrants();
								this.UpdateObjectCount();
						  }
						  return false;
					 }

					 private void FullyExpand()
					 {
						  GridPoint[] currentSearchPoints_=searchablepoints_.ToArray();
						  int count=searchablepoints_.Count;
						  foreach (var item in currentSearchPoints_)
						  {
								Expand(item);
						  }
						  searchablepoints_.RemoveRange(0, count); //remove the expanding points
						  searchablepoints_.TrimExcess();
					 }
					 private void Expand(GridPoint point)
					 {
						  foreach (var item in mgp.GetSearchAreaNeighbors(point, true))
						  {
								if (!this.Contains(item))
								{
									 this.Add(new GridPoint(item.X, item.Y, mgp.GetHeight(mgp.GridToWorld(item)), !mgp.CanStandAt(item)));

									 if (!searchablepoints_.Contains(item))
									 {
										  //if (GilesCanRayCast(CreationVector, pointV3, Zeta.Internals.SNO.NavCellFlags.AllowWalk))
										  searchablepoints_.Add(item);
									 }
								}
						  }
					 }
					 private bool CanExpandFurther
					 {
						  get
						  {
								return searchablepoints_.Count>0;
						  }
					 }

					 public string DebugString
					 {
						  get
						  {
								string debugstring=String.Format("GPC - Creation Vector {0} with {1} Total Gridpoints \r\n"+
																					 "Avoid {2} / Monster {3} Counts with total weight {4} \r\n"+
																					 "Last Found Vector {5}",
																					 this.CreationVector.ToString(), this.Count,
																					 this.AvoidanceCount, this.MonsterCount, this.Weight, this.LastFoundSafeSpot.ToString()!=null?this.LastFoundSafeSpot.ToString():"");
								return debugstring;

						  }
					 }

					 public GPRectangle Clone()
					 {
						  return (GPRectangle)this.MemberwiseClone();
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
								GPRectangle p=(GPRectangle)obj;
								return (CreationVector.Equals(p.CreationVector));
						  }
					 }
					 public override int GetHashCode()
					 {
						  return this.CreationVector.GetHashCode();
					 }
				}
		  }
	 }
}