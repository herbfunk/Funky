using System;
using Zeta;
using System.Collections.Generic;
using Zeta.Common;
using System.Linq;
using System.Collections;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  internal partial class ObjectCache
		  {
				///<summary>
				///Collection which is used to hold cached objects.
				///</summary>
				internal class ObjectCollection : IDictionary<int, CacheObject>
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
					 //Cache last filtered list generated
					 private List<Cluster> LastClusterList=new List<Cluster>();
					 public List<Cluster> Clusters(double Distance=12d, float MaxDistanceFromBot=50f, int MinUnitCount=1, bool ignoreFullyNonAttackable=true)
					 {
						  //only run Kmeans every 200ms or when cache is empty!
						  if (DateTime.Now.Subtract(lastClusterComputed).TotalMilliseconds>150)
						  {
								LastClusterList=Bot.Combat.RunKMeans(MinUnitCount, Distance);
								LastClusterList=LastClusterList.OrderBy(o => o.NearestMonsterDistance).ToList();

								//Sort by distance -- reverse to get nearest unit First
								if (LastClusterList.Count>0)
								{
									 LastClusterList.First().ListUnits.Sort();
									 LastClusterList.First().ListUnits.Reverse();
								}
						  }
						  return LastClusterList;
					 }

					 internal void RunKmeans(double distance, float maximumDistanceFromBot=50f)
					 {
						  CacheUnit[] Units=this.Values.OfType<CacheUnit>().Where(o => Bot.Combat.UnitRAGUIDs.Contains(o.RAGUID)&&o.RadiusDistance<=maximumDistanceFromBot).ToArray();

						  if (Units.Length==0)
								return;

						  List<CacheUnit> l_ListUnits=new List<CacheUnit>(Units);

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
						  return this.Values.OfType<CacheUnit>().Any(monster => monster.ShouldBeKited&&
								Math.Max(0f, monster.Position.Distance(Vector)-monster.Radius)<=Range);
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
						  get { return false; }
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
		  }
	 }
}