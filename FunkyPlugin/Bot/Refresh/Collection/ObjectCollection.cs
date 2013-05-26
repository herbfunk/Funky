using System;
using System.Linq;
using System.Collections.Generic;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.Internals;
using Zeta.Internals.SNO;
using Zeta.CommonBot;
using Zeta.Internals.Actors.Gizmos;
using System.Windows;
using System.Collections;
using System.Collections.ObjectModel;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  ///<summary>
		  ///Contains Collections for all the cached objects being tracked.
		  ///</summary>
		  public partial class ObjectCache
		  {
				///<summary>
				///Cached Objects.
				///</summary>
				public static ObjectCollection Objects=new ObjectCollection();

				///<summary>
				///Collection which is used to hold cached objects.
				///</summary>
				public class ObjectCollection : IDictionary<int, CacheObject>
				{
					 private Dictionary<int, CacheObject> objects=new Dictionary<int, CacheObject>();

					 ///<summary>
					 ///Adds the specific object to a list due to the object being avoided due to avoidance.
					 ///</summary>
					 public List<CacheObject> objectsIgnoredDueToAvoidance=new List<CacheObject>();

					 internal Dictionary<int, double> HealthEntriesForAverageValue=new Dictionary<int, double>();
					 private double MaximumHealthAverage { get; set; }
					 internal double MaximumHitPointAverage
					 {
						  get
						  {
								return MaximumHealthAverage;
						  }
					 }
					 internal void UpdateMaximumHealthAverage()
					 {
						  if (HealthEntriesForAverageValue.Count==0)
								MaximumHealthAverage=0d;
						  else
						  {
								int totalEntries=HealthEntriesForAverageValue.Keys.Count;
								var TotalHealth=HealthEntriesForAverageValue.Values.Sum();
								MaximumHealthAverage=TotalHealth/totalEntries;
						  }
					 }

					 public string DumpDebugInfo()
					 {
						  string s="[Object Cache] Total: ["+Count+"] - Units ("+Bot.Combat.UnitRAGUIDs.Count+") ["+Values.OfType<CacheUnit>().Count()+"] Items ["+Values.OfType<CacheItem>().Count()+"]"+" Gizmos ["+Values.OfType<CacheGizmo>().Count()+"]\r\n";
						  s+="Maximum Health Average: "+this.MaximumHitPointAverage.ToString()+"  using "+HealthEntriesForAverageValue.Count.ToString()+" entries";

						  Logging.WriteVerbose(s);
						  return s;
					 }

					 public void Add(int key, CacheObject value)
					 {
						  this.objects.Add(key, value);
					 }

					 public bool ContainsKey(int key)
					 {
						  return this.objects.ContainsKey(key);
					 }

					 public ICollection<int> Keys
					 {
						  get
						  {
								return this.objects.Keys;
						  }
					 }

					 public void Remove(int key)
					 {
						  this.objects.Remove(key);
					 }

					 public bool TryGetValue(int key, out CacheObject value)
					 {
						  return this.objects.TryGetValue(key, out value);
					 }
					 public bool TryGetValue<T>(int key, out T value) where T : CacheObject
					 {
						  if (this.objects.ContainsKey(key))
						  {
								value=(T)this.objects[key];
								return true;
						  }
						  value=null;
						  return false;
					 }

					 public ICollection<CacheObject> Values
					 {
						  get
						  {
								return this.objects.Values;
						  }
					 }

					 public CacheObject this[int key]
					 {
						  get
						  {
								return this.objects[key];
						  }
						  set
						  {
								this.objects[key]=value;
						  }
					 }

					 public void Clear()
					 {
						  this.objects.Clear();
						  this.HealthEntriesForAverageValue.Clear();
						  this.MaximumHealthAverage=0d;
						  cacheSnoCollection.ClearDictionaryCacheEntries();
					 }

					 public int Count
					 {
						  get { return this.objects.Values.Count; }
					 }

					 ///<summary>
					 ///Attempts to check for surrounding units using rect with area equal to range, successful attempt will return occupied list.
					 ///Uses Vector3 instead of cacheobject
					 ///</summary>
					 public void FindSurroundingObjects<T>(Vector3 objPosition, float range, out List<T> surroundingUnits) where T : CacheObject
					 {
						  surroundingUnits=(from objs in objects.Values.OfType<T>()
												  where Vector3.Distance(objPosition, objs.Position)<=range
												  select objs).ToList();
					 }

					 ///<summary>
					 ///Blacklists all objects surrounding given object within given range.
					 ///</summary>
					 public void BlacklistObjectsSurroundingObject<T>(CacheObject obj, float range, int blacklistloops=15) where T : CacheObject
					 {
						  //Create Destructible List
						  List<T> sObjs;
						  FindSurroundingObjects(obj.Position, range, out sObjs);

						  //Blacklist 'em!
						  foreach (var item in sObjs)
						  {
								item.BlacklistLoops=blacklistloops;
						  }
					 }

					 private DateTime lastClusterComputed=DateTime.Today;
					 private List<Cluster> LC_=new List<Cluster>();

					 public List<Cluster> Clusters(double Distance=12d, float MaxDistanceFromBot=50f, int MinUnitCount=1)
					 {

						  if (DateTime.Now.Subtract(lastClusterComputed).TotalMilliseconds>200)
						  {
								RunKmeans(Distance, MaxDistanceFromBot);
						  }
						  return LC_.Where(c => c.ListPoints.Count>=MinUnitCount).ToList();

					 }

					 internal void RunKmeans(double distance, float maximumDistanceFromBot=50f)
					 {
						  CacheUnit[] Units=this.Values.OfType<CacheUnit>().Where(o => Bot.Combat.UnitRAGUIDs.Contains(o.RAGUID)&&o.RadiusDistance<=maximumDistanceFromBot).ToArray();

						  if (Units.Length==0)
								return;

						  List<CacheUnit> l_ListUnits=Units.ToList();

						  if (l_ListUnits.Count==0)
								return;

						  LC_=new List<Cluster>();

						  // for starters, take a point to create one cluster
						  CacheUnit l_P1=l_ListUnits[0];

						  l_ListUnits.Remove(l_P1);

						  // so far, we have a one-point cluster
						  LC_.Add(new Cluster(distance, l_P1));

						  #region Main Loop
						  // the algorithm is inside this loop
						  List<Cluster> l_ListAttainableClusters;
						  Cluster l_c;
						  foreach (CacheUnit p in l_ListUnits)
						  {
								l_ListAttainableClusters=new List<Cluster>();
								l_ListAttainableClusters=LC_.FindAll(x => x.IsPointReachable(p.PointPosition));
								LC_.RemoveAll(x => x.IsPointReachable(p.PointPosition));
								l_c=new Cluster(distance, p);
								// merge point's "reachable" clusters
								if (l_ListAttainableClusters.Count>0)
									 l_c.AnnexCluster(l_ListAttainableClusters.Aggregate((c, x) =>
										c=Cluster.MergeClusters(x, c)));
								LC_.Add(l_c);
								//Logging.WriteVerbose("Cluster Found: Total Points {0} with Centeroid {1}", l_c.ListPoints.Count, l_c.Centeroid.ToString());
								l_ListAttainableClusters=null;
								l_c=null;
						  }  // of loop over candidate points

						  LC_=LC_.OrderBy(o => o.NearestMonsterDistance).ToList();
						  #endregion

						  lastClusterComputed=DateTime.Now;
					 }

					 public bool IsPointNearbyMonsters(Vector3 Vector, float Range=1f)
					 {
						  return this.Values.OfType<CacheUnit>().Any(monster => monster.ShouldBeKited&&monster.Position.Distance(Vector)-monster.Radius<=Range);
					 }

					 #region IDictionary<int,CachedObject> Members


					 bool IDictionary<int, CacheObject>.Remove(int key)
					 {
						  throw new NotImplementedException();
					 }

					 #endregion

					 #region ICollection<KeyValuePair<int,CachedObject>> Members

					 public void Add(KeyValuePair<int, CacheObject> item)
					 {
						  throw new NotImplementedException();
					 }

					 public bool Contains(KeyValuePair<int, CacheObject> item)
					 {
						  throw new NotImplementedException();
					 }

					 public void CopyTo(KeyValuePair<int, CacheObject>[] array, int arrayIndex)
					 {
						  throw new NotImplementedException();
					 }

					 public bool IsReadOnly
					 {
						  get { throw new NotImplementedException(); }
					 }

					 public bool Remove(KeyValuePair<int, CacheObject> item)
					 {
						  throw new NotImplementedException();
					 }

					 #endregion

					 #region IEnumerable<KeyValuePair<int,CachedObject>> Members

					 public IEnumerator<KeyValuePair<int, CacheObject>> GetEnumerator()
					 {
						  return this.objects.GetEnumerator();
					 }

					 #endregion

					 #region IEnumerable Members

					 IEnumerator IEnumerable.GetEnumerator()
					 {
						  return this.objects.GetEnumerator();
					 }

					 #endregion
				}



				///<summary>
				///Obstacles related to either avoidances or navigational blocks.
				///</summary>
				public static ObstacleCollection Obstacles=new ObstacleCollection();

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
						  get { throw new NotImplementedException(); }
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
						  if (found)//we exclude this from being removed..
								RemovalList.Remove(key);

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

					 public IEnumerable<CacheObstacle> returnFacingNavigationObstacles(float maxRange=30f)
					 {
						  return this.obstacles.Values.Where(obs =>
								obs.Obstacletype.Value==ObstacleType.ServerObject //navigational only!
								&&(obs.CentreDistance-Bot.Character.fCharacterRadius)<=maxRange //within maximum range!
								&&ZetaDia.Me.IsFacing(obs.Position)); //and the bot is facing..
					 }


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
										  this.Remove(item);

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
						  return OfType(par).Any(O => O.TestIntersection(Pos));
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
					 ///<summary>
					 ///Tests for avoidances that are active but not necessarly needing to be avoided.
					 ///</summary>
					 public bool IsPositionWithinActiveAvoidanceArea(Vector3 Pos)
					 {
						  return this.Avoidances.Any(O => O.IsActiveAvoidance&&O.PointInside(Pos));
					 }
					 ///<summary>
					 ///Tests Two Vectors for intersecting avoidances.
					 ///</summary>
					 public bool TestVectorAgainstAvoidanceZones(Vector3 startingPoint, Vector3 destinationPoint)
					 {
						  return this.Avoidances.Any(A => A.ShouldAvoid
								&&MathEx.IntersectsPath(A.Position, A.Radius, startingPoint, destinationPoint));
					 }

					 public bool TestVectorAgainstActiveAvoidanceZones(Vector3 destinationPoint)
					 {
						  Vector3 currentPosition=Bot.Character.Position;
						  return this.Avoidances.Any(A => A.IsActiveAvoidance&&!A.PointInside(currentPosition)
								&&MathEx.IntersectsPath(A.Position, A.Radius, currentPosition, destinationPoint));
					 }
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
					 ///<summary>
					 ///Fills List with any obstacles found within given range surrounding the object.
					 ///</summary>
					 public void FindObstaclesSurroundingObject<T>(CacheObstacle obj, float Range, out List<T> obstacles) where T : CacheObstacle
					 {
						  Rect objRect=new Rect(obj.PointPosition, new Size(0, 0));
						  float Radius=obj.Radius!=0f?obj.Radius:obj.ActorSphereRadius.HasValue?obj.ActorSphereRadius.Value:4f;
						  Radius+=Range;
						  Radius=(float)Math.Sqrt(Radius);
						  objRect.Inflate(Radius, Radius);
						  obstacles=this.Values.OfType<T>().Where(O => objRect.Contains(O.PointPosition)).ToList();
					 }
					 #endregion


				}



				///<summary>
				///Cached Sno Data.
				///</summary>
				public static SnoCollection cacheSnoCollection=new SnoCollection();

				///<summary>
				///Used to contain the cached sno entries. 
				///</summary>
				public class SnoCollection : IDictionary<int, CachedSNOEntry>
				{
					 public string DumpDebugInfo()
					 {
						  string s="[SNO Cache] Total: ["+this.snoEntries.Count+"]";
						  Logging.WriteVerbose(s);
						  return s;
					 }

					 private Dictionary<int, CachedSNOEntry> snoEntries=new Dictionary<int, CachedSNOEntry>();

					 private void CreateDictionaryEntries(int key)
					 {
						  //create cached ID in collections
						  if (!dictActorType.ContainsKey(key)) dictActorType.Add(key, null);
						  if (!dictTargetType.ContainsKey(key)) dictTargetType.Add(key, null);
						  if (!dictMonstersize.ContainsKey(key)) dictMonstersize.Add(key, null);
						  if (!dictMonstertype.ContainsKey(key)) dictMonstertype.Add(key, null);
						  if (!dictCollisionRadius.ContainsKey(key)) dictCollisionRadius.Add(key, null);
						  if (!dictInternalName.ContainsKey(key)) dictInternalName.Add(key, null);
						  if (!dictCanBurrow.ContainsKey(key)) dictCanBurrow.Add(key, null);
						  if (!dictDropsNoLoot.ContainsKey(key)) dictDropsNoLoot.Add(key, null);
						  if (!dictGrantsNoXp.ContainsKey(key)) dictGrantsNoXp.Add(key, null);
						  if (!dictIsBarricade.ContainsKey(key)) dictIsBarricade.Add(key, null);
						  if (!dictObstacleType.ContainsKey(key)) dictObstacleType.Add(key, null);
						  if (!dictActorSphereRadius.ContainsKey(key)) dictActorSphereRadius.Add(key, null);
						  if (!dictGizmoType.ContainsKey(key)) dictGizmoType.Add(key, null);
					 }
					 private void RemoveDictionaryEntries(int key)
					 {
						  //Clear out cache
						  dictActorType.Remove(key);
						  dictTargetType.Remove(key);
						  dictMonstersize.Remove(key);
						  dictMonstertype.Remove(key);
						  dictCollisionRadius.Remove(key);
						  dictInternalName.Remove(key);
						  dictCanBurrow.Remove(key);
						  dictDropsNoLoot.Remove(key);
						  dictGrantsNoXp.Remove(key);
						  dictIsBarricade.Remove(key);
						  dictObstacleType.Remove(key);
						  dictActorSphereRadius.Remove(key);
						  dictGizmoType.Remove(key);
					 }

					 #region IDictionary<int,CacheSNO> Members

					 public void Add(int key, CachedSNOEntry value)
					 {
						  if (!this.snoEntries.ContainsKey(key))
						  {
								CreateDictionaryEntries(key);
								this.snoEntries.Add(key, value);
						  }
						  else
								this[key]=value;
					 }

					 public void Add(int key, out CachedSNOEntry sno)
					 {
						  sno=null;
						  if (!this.snoEntries.ContainsKey(key))
						  {
								//create cached ID in collections
								CreateDictionaryEntries(key);
								//create the sno object
								sno=new CachedSNOEntry(key, true);
								//add to static cache
								this.snoEntries.Add(key, sno);
						  }
					 }

					 public void Add(int key)
					 {

						  if (!this.snoEntries.ContainsKey(key))
						  {
								//create cached ID in collections
								CreateDictionaryEntries(key);
								//add to static cache
								this.snoEntries.Add(key, new CachedSNOEntry(key, true));
						  }
					 }

					 public bool ContainsKey(int key)
					 {
						  return this.snoEntries.ContainsKey(key);
					 }


					 public bool Remove(int key)
					 {
						  //Clear out cache
						  RemoveDictionaryEntries(key);
						  this.snoEntries.Remove(key);
						  return true;
					 }

					 public ICollection<CachedSNOEntry> Values
					 {
						  get
						  {
								return this.snoEntries.Values;
						  }
					 }

					 public CachedSNOEntry this[int key]
					 {
						  get
						  {
								if (this.ContainsKey(key))
								{
									 this.snoEntries[key].LastUsed=DateTime.Now;
									 return this.snoEntries[key]; //return reference
								}
								else
								{
									 //create
									 CachedSNOEntry thisnewSno;
									 this.Add(key, out thisnewSno);
									 return this.snoEntries[key]; //return reference
								}
						  }
						  set
						  {
								if (!this.ContainsKey(key))
									 //create with data given
									 this.snoEntries.Add(key, value);
								else
								{
									 //copy value
									 this.snoEntries[key]=value;
									 this.snoEntries[key].LastUsed=DateTime.Now; //update
								}
						  }
					 }

					 #endregion

					 public DateTime lastTrimming=DateTime.Now;
					 public void ResetTrimTimer()
					 {
						  lastTrimming=DateTime.Now;
					 }

					 public void TrimOldUnusedEntries()
					 {
						  var UnusedEntries=(from CachedSNOEntry entries in this.snoEntries.Values
													where DateTime.Now.Subtract(entries.LastUsed).TotalMinutes>5
													select entries.SNOID).ToList();

						  if (UnusedEntries.Count>0)
						  {
								Logging.WriteVerbose("Removing Old Unused SNO Entries that have not been used in over 5 mins. Total {0}", UnusedEntries.Count);
								for (int i=0; i<UnusedEntries.Count; i++)
								{
									 this.Remove(UnusedEntries[i]);
								}
						  }
						  lastTrimming=DateTime.Now;
					 }

					 private List<int> DictionaryEntriesNeedingCleared=new List<int>();
					 public void FinalizeEntry(int sno)
					 {
						  if (!this.ContainsKey(sno)) return;

						  CachedSNOEntry thisSNOdata=this[sno];

						  //Take the value, recreate it using finalized constructor
						  CachedSNOEntry thisNewData=new CachedSNOEntry(thisSNOdata.SNOID, thisSNOdata.InternalName, thisSNOdata.Actortype, thisSNOdata.targetType, thisSNOdata.Monstertype, thisSNOdata.Monstersize, thisSNOdata.CollisionRadius, thisSNOdata.CanBurrow, thisSNOdata.GrantsNoXP, thisSNOdata.DropsNoLoot, thisSNOdata.IsBarricade, thisSNOdata.Obstacletype, thisSNOdata.ActorSphereRadius, thisSNOdata.Gizmotype);

						  //Now clone the data and set it as the value
						  this.snoEntries[sno]=(CachedSNOEntry)thisNewData.Clone();

						  if (!this.DictionaryEntriesNeedingCleared.Contains(sno))
								this.DictionaryEntriesNeedingCleared.Add(sno);
						  //Clear out cache
						  //RemoveDictionaryEntries(sno);

						  //Now all future references to this SNO data will no longer require lookups!
					 }

					 public void ClearDictionaryCacheEntries()
					 {
						  if (this.DictionaryEntriesNeedingCleared.Count>0)
						  {
								Logging.WriteVerbose("Deleting SNO Dictionary Entries, Total Count {0}", DictionaryEntriesNeedingCleared.Count);
								foreach (var item in this.DictionaryEntriesNeedingCleared)
								{
									 RemoveDictionaryEntries(item);
								}
								this.DictionaryEntriesNeedingCleared.Clear();
						  }
					 }

					 #region IDictionary<int,CachedSNOEntry> Members


					 public ICollection<int> Keys
					 {
						  get { return this.snoEntries.Keys; }
					 }

					 public bool TryGetValue(int key, out CachedSNOEntry value)
					 {
						  return this.snoEntries.TryGetValue(key, out value);
					 }

					 #endregion

					 #region ICollection<KeyValuePair<int,CachedSNOEntry>> Members

					 public void Add(KeyValuePair<int, CachedSNOEntry> item)
					 {
						  throw new NotImplementedException();
					 }

					 public void Clear()
					 {
						  this.snoEntries.Clear();
					 }

					 public bool Contains(KeyValuePair<int, CachedSNOEntry> item)
					 {
						  throw new NotImplementedException();
					 }

					 public void CopyTo(KeyValuePair<int, CachedSNOEntry>[] array, int arrayIndex)
					 {
						  throw new NotImplementedException();
					 }

					 public int Count
					 {
						  get { return this.snoEntries.Count; }
					 }

					 public bool IsReadOnly
					 {
						  get { throw new NotImplementedException(); }
					 }

					 public bool Remove(KeyValuePair<int, CachedSNOEntry> item)
					 {
						  throw new NotImplementedException();
					 }

					 #endregion

					 #region IEnumerable<KeyValuePair<int,CachedSNOEntry>> Members

					 public IEnumerator<KeyValuePair<int, CachedSNOEntry>> GetEnumerator()
					 {
						  return this.snoEntries.GetEnumerator();
					 }

					 #endregion

					 #region IEnumerable Members

					 IEnumerator IEnumerable.GetEnumerator()
					 {
						  return this.snoEntries.GetEnumerator();
					 }

					 #endregion
				}
		  }
	 }
}