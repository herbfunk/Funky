using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FunkyBot.Cache.Objects;
using Zeta.Common;

namespace FunkyBot.Cache.Collections
{


	///<summary>
	///Collection which is used to hold cached objects.
	///</summary>
	public class ObjectCollection : IDictionary<int, CacheObject>
	{
		private Dictionary<int, CacheObject> objects = new Dictionary<int, CacheObject>();

		internal Dictionary<int, double> HealthEntriesForAverageValue = new Dictionary<int, double>();
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
			if (HealthEntriesForAverageValue.Count == 0)
				MaximumHealthAverage = 0d;
			else
			{
				int totalEntries = HealthEntriesForAverageValue.Keys.Count;
				var TotalHealth = HealthEntriesForAverageValue.Values.Sum();
				MaximumHealthAverage = TotalHealth / totalEntries;
			}
		}
		public void ClearHealthAverageStats()
		{
			HealthEntriesForAverageValue.Clear();
			MaximumHealthAverage = 0d;
		}

		public string DumpDebugInfo()
		{
			string s = "[Object Cache] Total: [" + Count + "] - Units [" + Values.OfType<CacheUnit>().Count() + "] Items [" + Values.OfType<CacheItem>().Count() + "]" + " Gizmos [" + Values.OfType<CacheGizmo>().Count() + "]\r\n";
			s += "Maximum Health Average: " + MaximumHitPointAverage + "  using " + HealthEntriesForAverageValue.Count + " entries";

			Logging.WriteVerbose(s);
			return s;
		}

		public void Add(int key, CacheObject value)
		{
			objects.Add(key, value);
		}

		public bool ContainsKey(int key)
		{
			return objects.ContainsKey(key);
		}

		public ICollection<int> Keys
		{
			get
			{
				return objects.Keys;
			}
		}

		public void Remove(int key)
		{
			if (Bot.NavigationCache.LOSmovementObject != null && Bot.NavigationCache.LOSmovementObject.RAGUID.Equals(key))
				Bot.NavigationCache.LOSmovementObject = null;

			objects.Remove(key);
		}

		public bool TryGetValue(int key, out CacheObject value)
		{
			return objects.TryGetValue(key, out value);
		}
		public bool TryGetValue<T>(int key, out T value) where T : CacheObject
		{
			if (objects.ContainsKey(key))
			{
				value = (T)objects[key];
				return true;
			}
			value = null;
			return false;
		}

		public ICollection<CacheObject> Values
		{
			get
			{
				return objects.Values;
			}
		}

		public CacheObject this[int key]
		{
			get
			{
				return objects[key];
			}
			set
			{
				objects[key] = value;
			}
		}

		public void Clear()
		{
			objects.Clear();

		}

		public int Count
		{
			get { return objects.Values.Count; }
		}

		///<summary>
		///Attempts to check for surrounding units using rect with area equal to range, successful attempt will return occupied list.
		///Uses Vector3 instead of cacheobject
		///</summary>
		public void FindSurroundingObjects<T>(Vector3 objPosition, float range, out List<T> surroundingUnits) where T : CacheObject
		{
			surroundingUnits = (from objs in objects.Values.OfType<T>()
								where Vector3.Distance(objPosition, objs.Position) <= range
								select objs).ToList();
		}

		///<summary>
		///Blacklists all objects surrounding given object within given range.
		///</summary>
		public void BlacklistObjectsSurroundingObject<T>(CacheObject obj, float range, int blacklistloops = 15) where T : CacheObject
		{
			//Create Destructible List
			List<T> sObjs;
			FindSurroundingObjects(obj.Position, range, out sObjs);

			//Blacklist 'em!
			foreach (var item in sObjs)
			{
				item.BlacklistLoops = blacklistloops;
			}
		}



		public bool IsPointNearbyMonsters(Vector3 Vector, float Range = 1f)
		{
			return Values.OfType<CacheUnit>().Any(monster => monster.ShouldFlee &&
				  Math.Max(0f, monster.Position.Distance(Vector) - monster.Radius) <= Range);
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
			return objects.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return objects.GetEnumerator();
		}

		#endregion
	}


}