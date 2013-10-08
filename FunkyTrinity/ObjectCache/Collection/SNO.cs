using System;
using Zeta;
using System.Collections.Generic;
using Zeta.Common;
using System.Linq;
using System.Collections;

namespace FunkyBot.Cache
{

		  public partial class ObjectCache
		  {
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