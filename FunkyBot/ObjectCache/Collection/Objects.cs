using System;
using Zeta;
using System.Collections.Generic;
using Zeta.Common;
using System.Linq;
using System.Collections;
using Zeta.Internals.SNO;

namespace FunkyBot.Cache
{

		  public partial class ObjectCache
		  {
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
                    public void ClearHealthAverageStats()
                     {
                         HealthEntriesForAverageValue.Clear();
                         MaximumHealthAverage = 0d;
                     }

					 public string DumpDebugInfo()
					 {
						  string s="[Object Cache] Total: ["+Count+"] - Units ["+Values.OfType<CacheUnit>().Count()+"] Items ["+Values.OfType<CacheItem>().Count()+"]"+" Gizmos ["+Values.OfType<CacheGizmo>().Count()+"]\r\n";
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



					 public bool IsPointNearbyMonsters(Vector3 Vector, float Range=1f)
					 {
						  return this.Values.OfType<CacheUnit>().Any(monster => monster.ShouldFlee&&
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