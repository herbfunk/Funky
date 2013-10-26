using System;
using FunkyBot.Avoidances;
using FunkyBot.Cache.Enums;
using Zeta;
using System.Collections.Generic;
using Zeta.Common;
using System.Linq;
using System.Collections;
using System.Windows;

namespace FunkyBot.Cache
{

		  public partial class ObjectCache
		  {
				///<summary>
				///Holds all "obstacle" related objects including Navigational and Avoidance related.
				///</summary>
				public class ObstacleCollection : IDictionary<int, CacheObstacle>
				{
					 private Dictionary<int, CacheObstacle> obstacles=new Dictionary<int, CacheObstacle>();

					 public string DumpDebugInfo()
					 {
						  string s="[Obstacle Cache] Total: ["+Count+"] - Avoidances ["+this.Values.OfType<CacheAvoidance>().Count()+"] Monster ["+Monsters.Count+"]"+" Navigational ["+Navigations.Count+"]";
						  Logging.WriteVerbose(s);
						  return s;
					 }

					 #region IDictionary<int,CachedObstacle> Members


					 public bool ContainsKey(int key)
					 {
						  return this.obstacles.ContainsKey(key);
					 }

					 public ICollection<int> Keys
					 {
						  get { return this.obstacles.Keys; }
					 }

					 bool IDictionary<int, CacheObstacle>.Remove(int key)
					 {
						  throw new NotImplementedException();
					 }

					 #endregion

					 #region ICollection<KeyValuePair<int,CachedObstacle>> Members

					 public void Add(KeyValuePair<int, CacheObstacle> item)
					 {
						  throw new NotImplementedException();
					 }

					 public bool Contains(KeyValuePair<int, CacheObstacle> item)
					 {
						  throw new NotImplementedException();
					 }

					 public void CopyTo(KeyValuePair<int, CacheObstacle>[] array, int arrayIndex)
					 {
						  throw new NotImplementedException();
					 }

					 public bool IsReadOnly
					 {
						  get { return false; }
					 }

					 public bool Remove(KeyValuePair<int, CacheObstacle> item)
					 {
						  throw new NotImplementedException();
					 }

					 #endregion

					 #region IEnumerable<KeyValuePair<int,CachedObstacle>> Members

					 public IEnumerator<KeyValuePair<int, CacheObstacle>> GetEnumerator()
					 {
						  return this.obstacles.GetEnumerator();
					 }

					 #endregion

					 #region IEnumerable Members

					 IEnumerator IEnumerable.GetEnumerator()
					 {
						  return this.obstacles.GetEnumerator();
					 }

					 #endregion


					 #region IDictionary<int,CacheObstacle> Members

					 public void Add(int Key, CacheObstacle obstacle)
					 {
						  this.obstacles.Add(Key, obstacle);
					 }
					 public void Add(CacheObject obstacle)
					 {
						  if (obstacle is CacheObstacle)
								this.obstacles.Add(obstacle.RAGUID, (CacheObstacle)obstacle);

					 }

					 public void Remove(int Key)
					 {
						  this.obstacles.Remove(Key);
					 }

					 public bool TryGetValue(int key, out CacheObstacle value)
					 {
						  bool found=this.obstacles.TryGetValue(key, out value);

						  if (found)
						  {
								RemovalList.Remove(key);//we exclude this from being removed..
						  }

						  return found;

					 }

					 public ICollection<CacheObstacle> Values
					 {
						  get
						  {
								return this.obstacles.Values;
						  }
					 }

					 public CacheObstacle this[int key]
					 {
						  get
						  {
								return this.obstacles[key];
						  }
						  set
						  {
								this.obstacles[key]=value;
						  }
					 }

					 #endregion

					 #region ICollection<KeyValuePair<int,CacheObstacle>> Members


					 public void Clear()
					 {
						  this.obstacles=new Dictionary<int, CacheObstacle>();
					 }

					 public int Count
					 {
						  get { return this.obstacles.Count; }
					 }

					 #endregion




					 #region SpecificLists

					 public List<CacheServerObject> Monsters
					 {
						  get
						  {
								return this.Values.OfType<CacheServerObject>().Where(obj => obj.Obstacletype.HasValue&&obj.Obstacletype.Value==ObstacleType.Monster).ToList();
						  }
					 }
					 public List<CacheServerObject> Navigations
					 {
						  get
						  {
								return this.Values.OfType<CacheServerObject>().Where(obj => obj.Obstacletype.HasValue&&ObstacleType.Navigation.HasFlag(obj.Obstacletype.Value)).ToList();
						  }
					 }
					 public List<CacheAvoidance> Avoidances
					 {
						  get
						  {
								return this.Values.OfType<CacheAvoidance>().ToList();
						  }
					 }

					 ///<summary>
					 ///Returns any objects found with matching TargetTypes.
					 ///</summary>
					 public IEnumerable<CacheObstacle> OfType(ObstacleType var)
					 {
						  foreach (CacheObstacle item in this.obstacles.Values)
						  {
								if (!item.Obstacletype.HasValue)
									 continue;

								if (var.HasFlag(item.Obstacletype.Value))
									 yield return item;
						  }
					 }

					 #endregion

					 //public IEnumerable<CacheObstacle> returnFacingNavigationObstacles(float maxRange=30f)
					 //{
					 //	 return this.obstacles.Values.Where(obs =>
					 //		  obs.Obstacletype.Value==ObstacleType.ServerObject //navigational only!
					 //		  &&(obs.CentreDistance-Bot.Character.fCharacterRadius)<=maxRange //within maximum range!
					 //		  &&ZetaDia.Me.IsFacing(obs.Position)); //and the bot is facing..
					 //}


					 #region Methods
					 private List<int> RemovalList=new List<int>();

					 ///<summary>
					 ///Simple counter/reset method to clear entries every 5 loops.
					 ///</summary>
					 public void AttemptToClearEntries()
					 {
						  if (RemovalList.Count>0)
						  {
								foreach (int item in RemovalList)
								{
									 if (this.ContainsKey(item))
									 {
										  if (this.obstacles[item].RefreshRemovalCounter>0)
										  {
												this.obstacles[item].RefreshRemovalCounter--;
												if (this.obstacles[item].IsAvoidance)
												{
													 CacheAvoidance avoidance=(CacheAvoidance)this.obstacles[item];
													 AvoidanceCache.CheckAvoidanceObject(ref avoidance);
												}
										  }	
										  else
												this.Remove(item);
									 }
								}
						  }

						  //update removal list with what remains..
						  RemovalList=new List<int>(this.Keys.ToList());
					 }

					 ///<summary>
					 ///Tests obstacles for intersections, return value of true means no intersections found.
					 ///</summary>
					 public bool DoesPositionIntersectAny(Vector3 Pos, ObstacleType par=ObstacleType.All)
					 {
						  Vector3 BotPosition=Bot.Character.Position;
						  return OfType(par).Any(O => O.TestIntersection(BotPosition,Pos));
					 }
					 ///<summary>
					 ///Tests obstacles against vector to see if it is contained within any. return of false means no obstacles are under the vector.
					 ///</summary>
					 public bool IsPositionWithinAny(Vector3 Pos, ObstacleType par=ObstacleType.All)
					 {
						  return OfType(par).Any(O => O.PointInside(Pos));
					 }

					 ///<summary>
					 ///Tests for avoidances that should be currently avoided due to health!
					 ///</summary>
					 public bool IsPositionWithinAvoidanceArea(Vector3 Pos)
					 {
						  return this.Avoidances.Any(O => O.ShouldAvoid&&O.PointInside(Pos));
					 }
					 /////<summary>
					 /////Tests for avoidances that are active but not necessarly needing to be avoided.
					 /////</summary>
					 //public bool IsPositionWithinActiveAvoidanceArea(Vector3 Pos)
					 //{
					 //	 return this.Avoidances.Any(O => O.IsActiveAvoidance&&O.PointInside(Pos));
					 //}
					 ///<summary>
					 ///Tests Two Vectors for intersecting avoidances.
					 ///</summary>
					 public bool TestVectorAgainstAvoidanceZones(Vector3 startingPoint, Vector3 destinationPoint, bool IgnoreTriggeringAvoidances=true)
					 {
                         return this.Avoidances.Any(A => A.ShouldAvoid &&
                               (!IgnoreTriggeringAvoidances || !Bot.Combat.TriggeringAvoidanceRAGUIDs.Contains(A.RAGUID)) &&
								MathEx.IntersectsPath(A.Position, A.Radius, startingPoint, destinationPoint));
					 }

					 //public bool TestVectorAgainstActiveAvoidanceZones(Vector3 destinationPoint)
					 //{
					 //	 Vector3 currentPosition=Bot.Character.Position;
					 //	 return this.Avoidances.Any(A => A.IsActiveAvoidance&&!A.PointInside(currentPosition)
					 //		  &&MathEx.IntersectsPath(A.Position, A.Radius, currentPosition, destinationPoint));
					 //}
					 public bool TestVectorAgainstAvoidanceZones(Vector3 destinationPoint)
					 {
						  Vector3 currentPosition=Bot.Character.Position;
						  return this.Avoidances.Any(A => A.ShouldAvoid
								&&MathEx.IntersectsPath(A.Position, A.Radius, currentPosition, destinationPoint));
					 }
					 ///<summary>
					 ///Sorts avoidance zone objects based upon current bots position.
					 ///</summary>
					 public void SortAvoidanceZones()
					 {
						  this.Avoidances.Sort();
					 }
					 /////<summary>
					 /////Fills List with any obstacles found within given range surrounding the object.
					 /////</summary>
					 //public void FindObstaclesSurroundingObject<T>(CacheObstacle obj, float Range, out List<T> obstacles) where T : CacheObstacle
					 //{
					 //	 Rect objRect=new Rect(obj.PointPosition, new Size(0, 0));
					 //	 float Radius=obj.Radius!=0f?obj.Radius:obj.ActorSphereRadius.HasValue?obj.ActorSphereRadius.Value:4f;
					 //	 Radius+=Range;
					 //	 Radius=(float)Math.Sqrt(Radius);
					 //	 objRect.Inflate(Radius, Radius);
					 //	 obstacles=this.Values.OfType<T>().Where(O => objRect.Contains(O.PointPosition)).ToList();
					 //}
					 #endregion


				}


		  }
    
}