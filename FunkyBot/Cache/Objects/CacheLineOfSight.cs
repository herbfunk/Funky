using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FunkyBot.Cache.Enums;
using Zeta.Common;

namespace FunkyBot.Cache.Objects
{
	public class CacheLineOfSight
	{
		public CacheObject OrginCacheObject { get; set; }
		public readonly int OrginCacheObjectRAGUID;
		public bool CacheContainsOrginObject()
		{
			return ObjectCache.Objects.ContainsKey(OrginCacheObjectRAGUID);
		}
		public void UpdateOrginObject()
		{
			if (CacheContainsOrginObject())
			{
				OrginCacheObject = ObjectCache.Objects[OrginCacheObjectRAGUID];
				Position = OrginCacheObject.Position;
			}
		}

		public Vector3 Position { get; set; }
		public float CentreDistance
		{
			get
			{
				return Bot.Character.Data.Position.Distance(Position);
			}
		}

		public CacheLineOfSight(CacheObject obj, Vector3 pos)
		{
			OrginCacheObject = obj;
			OrginCacheObjectRAGUID=obj.RAGUID;
			Position = pos;
		}

	}
}
