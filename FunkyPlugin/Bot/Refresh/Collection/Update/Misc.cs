using System;
using System.Linq;
using Zeta;
using System.Collections.Generic;
using Zeta.Internals.SNO;
using Zeta.Common;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  public partial class dbRefresh
		  {
				internal static readonly HashSet<ActorType> IgnoredActorTypes=new HashSet<ActorType>
				{
					 ActorType.AxeSymbol,
					 ActorType.ClientEffect,
					 ActorType.Critter,
					 ActorType.CustomBrain,
					 ActorType.Invalid
				};
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
						  hashActorSNOIgnoreBlacklist.Add(sno);

					 if (removal)
					 {
						  //Clear SNO cache entries..
						  ObjectCache.cacheSnoCollection.Remove(snoObj.SNOID);
						  //Clear previous cache entries..
						  ObjectCache.Objects.Remove(RAGUID);
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
						  hashActorSNOIgnoreBlacklist.Add(sno);

					 if (removal)
					 {
						  //Clear SNO cache entries..
						  ObjectCache.cacheSnoCollection.Remove(sno);
						  //Clear previous cache entries..
						  ObjectCache.Objects.Remove(raguid);


					 }
				}
		  }

	 }
}