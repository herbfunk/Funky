using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using Zeta.Common;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fBaseXtensions.Cache.Internal.Collections
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

			//Logger.DBLog.InfoFormat(s);
			return s;
		}

		public delegate void ObjectAddedToCollection(CacheObject obj);
		public event ObjectAddedToCollection OnObjectAddedToCollection;
		public void Add(int key, CacheObject value)
		{
			objects.Add(key, value);

			if (OnObjectAddedToCollection != null)
				OnObjectAddedToCollection(value);
		}

	    public class ObjectRemovedArgs
	    {
            public int SNO { get; set; }
            public int RAGUID { get; set; }
            public int ACDGUID { get; set; }
            public RemovalTypes RemovalType { get; set; }

	        public ObjectRemovedArgs(CacheObject obj)
	        {
	            SNO = obj.SNOID;
	            RAGUID = obj.RAGUID;
	            ACDGUID = obj.AcdGuid.HasValue ? obj.AcdGuid.Value : -1;
	            RemovalType = obj.RemovalType;
	        }

	        public ObjectRemovedArgs(int sno, int raguid, int acdguid, RemovalTypes removaltype)
	        {
	            SNO = sno;
	            RAGUID = raguid;
	            ACDGUID = acdguid;
	            RemovalType = removaltype;
	        }
	    }
		public delegate void ObjectRemovedFromCollection(ObjectRemovedArgs args);
		public event ObjectRemovedFromCollection OnObjectRemovedFromCollection;
		public void Remove(int key)
		{
			if (OnObjectRemovedFromCollection != null)
			{
                OnObjectRemovedFromCollection(new ObjectRemovedArgs(objects[key]));
			}
				
			objects.Remove(key);
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


        public IEnumerable<T> OfType<T>() where T : CacheObject
	    {
            foreach (var obj in Values)
            {
                if (obj.NeedsRemoved || obj.BlacklistLoops < 0) continue;

                if (obj is T) yield return (T)obj;
            }
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
			surroundingUnits = (from objs in objects.OfType<T>()
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

	    public int TotalIntersectingUnits(Vector3 TargetDestination, float RayRadius)
	    {
	        Ray playerRay = new Ray(FunkyGame.Hero.Position,
	            Vector3.NormalizedDirection(FunkyGame.Hero.Position, TargetDestination));

	        var distance = FunkyGame.Hero.Position.Distance(TargetDestination);

	        var botFacingUnits = (from objs in objects.OfType<CacheUnit>()
                                    where objs.RadiusDistance <= distance
                                    && objs.BotIsFacing(TargetDestination)
	                                    select objs).ToList();

	        //if (botFacingUnits.Count > 0)
	           // Logger.DBLog.DebugFormat("Total Intersecting Units Test Found {0} Units facing destination!", botFacingUnits.Count);

	        int totalIntersectingCount = 0;
            foreach (var obj in botFacingUnits)
            {
                var objSphere = new Sphere(obj.Position, RayRadius);
                float? nullable = playerRay.Intersects(objSphere);
                if (nullable.HasValue && nullable.Value < distance)
                    totalIntersectingCount++;
            }

            //if (totalIntersectingCount>0)
               // Logger.DBLog.DebugFormat("Total Intersecting Units Found {0}!",totalIntersectingCount);

	        return totalIntersectingCount;
	    }


		public bool IsPointNearbyMonsters(Vector3 Vector, float Range = 1f)
		{
			return OfType<CacheUnit>().Any(monster => monster.ShouldFlee &&
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