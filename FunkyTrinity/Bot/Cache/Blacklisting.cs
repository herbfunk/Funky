using System;
using System.Collections.Generic;
using FunkyTrinity.Enums;
using Zeta.Common;
using Zeta.Internals.SNO;

namespace FunkyTrinity.Cache
{
	 public static partial class ObjectCache
	{
		 // When did we last clear the temporary blacklist?
		 private static DateTime dateSinceTemporaryBlacklistClear=DateTime.Today;
		 // And the full blacklist?
		 // These are blacklists used to blacklist objects/monsters/items either for the rest of the current game, or for a shorter period
		 private static HashSet<int> hashRGUIDTemporaryIgnoreBlacklist=new HashSet<int>();
		 private static HashSet<int> hashRGUIDIgnoreBlacklist=new HashSet<int>();
		 internal static HashSet<int> hashSNOTargetBlacklist=new HashSet<int>();
		 internal static HashSet<int> hashProfileSNOTargetBlacklist=new HashSet<int>();

		 internal static void ClearBlacklistCollections()
		 {
				hashRGUIDIgnoreBlacklist=new HashSet<int>();
				hashRGUIDTemporaryIgnoreBlacklist=new HashSet<int>();
		 }

		 internal static readonly HashSet<ActorType> IgnoredActorTypes=new HashSet<ActorType>
			  {
				  ActorType.AxeSymbol,
				  ActorType.ClientEffect,
				  ActorType.Critter,
				 // ActorType.CustomBrain,
				  ActorType.Invalid
			  };

		 internal static void CheckRefreshBlacklists(int minimumSeconds=90)
		 {
				// Clear the temporary blacklist every 90 seconds
				if (DateTime.Now.Subtract(dateSinceTemporaryBlacklistClear).TotalSeconds>minimumSeconds)
				{
					 dateSinceTemporaryBlacklistClear=DateTime.Now;
					 hashRGUIDTemporaryIgnoreBlacklist=new HashSet<int>();
				}
		 }

		 
		 internal static void AddObjectToBlacklist(int RAGUID, BlacklistType blacklist)
		 {
				if (blacklist==BlacklistType.Permanent)
				{
					 if (!hashRGUIDIgnoreBlacklist.Contains(RAGUID))
							hashRGUIDIgnoreBlacklist.Add(RAGUID);

				}
				else if (blacklist==BlacklistType.Temporary)
				{
					 if (!hashRGUIDTemporaryIgnoreBlacklist.Contains(RAGUID))
					 {
							hashRGUIDTemporaryIgnoreBlacklist.Add(RAGUID);
							dateSinceTemporaryBlacklistClear=DateTime.Now;
					 }
				}
		 }

		 //Updates RActorGUID and returns if we should continue Cache Process by checking blacklists.
		 internal static bool IsRAGUIDBlacklisted(int ractorGUID)
		 {
				// See if it's on our temporary blacklist (from being stuck targeting it), as long as it's distance is not extremely close
				if (hashRGUIDTemporaryIgnoreBlacklist.Contains(ractorGUID))
				{
					 return true;
				}
				// Or on our more permanent "per-game" blacklist
				if (hashRGUIDIgnoreBlacklist.Contains(ractorGUID))
				{
					 return true;
				}

				return false;
		 }
		 internal static bool IsSNOIDBlacklisted(int snoID)
		 {
				if (CacheIDLookup.hashActorSNOIgnoreBlacklist.Contains(snoID))
					 return true;

				if (CacheIDLookup.hashSNOIgnoreBlacklist.Contains(snoID))
					 return true;

				if (hashSNOTargetBlacklist.Contains(snoID))
					 return true;

				return false;
		 }

		 internal static void IgnoreThisObject(CachedSNOEntry snoObj, int RAGUID, bool removal=true, bool blacklistSNOID=true)
		 {
				Logging.WriteVerbose("[Blacklist] -- RAGUID {0} SNOID {1} ({2})", snoObj.SNOID, RAGUID, snoObj.InternalName);

				int sno, raguid;
				sno=snoObj.SNOID;
				raguid=RAGUID;

				//Add to our blacklist so we don't create it again..
				hashRGUIDIgnoreBlacklist.Add(raguid);

				if (blacklistSNOID)
					 //Blacklist SNO so we don't create it ever again!
					 CacheIDLookup.hashActorSNOIgnoreBlacklist.Add(sno);

				if (removal)
				{
					 //Clear SNO cache entries..
					 cacheSnoCollection.Remove(snoObj.SNOID);
					 //Clear previous cache entries..
					 Objects.Remove(RAGUID);
				}


		 }
		 internal static void IgnoreThisObject(CacheObject thisobj, bool removal=true, bool blacklistSNOID=true)
		 {
				Logging.WriteVerbose("[Blacklist] Obj RAGUID {0} SNOID {1} ({2})", thisobj.RAGUID, thisobj.SNOID, thisobj.InternalName);

				int sno, raguid;
				sno=thisobj.SNOID;
				raguid=thisobj.RAGUID;

				//Add to our blacklist so we don't create it again..
				hashRGUIDIgnoreBlacklist.Add(raguid);

				if (blacklistSNOID)
					 //Blacklist SNO so we don't create it ever again!
					 CacheIDLookup.hashActorSNOIgnoreBlacklist.Add(sno);

				if (removal)
				{
					 //Clear SNO cache entries..
					 cacheSnoCollection.Remove(sno);
					 //Clear previous cache entries..
					 Objects.Remove(raguid);


				}
		 }
	}
}
