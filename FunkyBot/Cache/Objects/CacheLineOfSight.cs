using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FunkyBot.Cache.Enums;
using FunkyBot.Game;
using FunkyBot.Game.Bounty;
using Zeta.Common;

namespace FunkyBot.Cache.Objects
{
	public class CacheLineOfSight
	{
		public CacheLineOfSight(CacheObject obj, Vector3 pos)
		{
			OrginCacheObject = obj;
			OrginCacheObjectRAGUID = obj.RAGUID;
			Position = pos;
		}
		public CacheLineOfSight(BountyCache.BountyMapMarker bmm)
		{
			Position=bmm.Position;
			OrginCacheObjectRAGUID = bmm.GetHashCode();
			OrginMapMarker = bmm;
			IgnoringCacheCheck = true;
		}

		internal readonly bool IgnoringCacheCheck;
		private CacheObject OrginCacheObject;
		private BountyCache.BountyMapMarker OrginMapMarker;

		public readonly int OrginCacheObjectRAGUID;

		public Vector3 Position { get; set; }

		public float CentreDistance
		{
			get
			{
				return Bot.Character.Data.Position.Distance(Position);
			}
		}



		public bool CacheContainsOrginObject()
		{
			if (IgnoringCacheCheck)
			{
				Bot.Game.Bounty.RefreshBountyMapMarkers();
				return Bot.Game.Bounty.CurrentBountyMapMarkers.ContainsKey(OrginCacheObjectRAGUID);
			}

			return ObjectCache.Objects.ContainsKey(OrginCacheObjectRAGUID);
		}

		public void UpdateOrginObject()
		{
			if (CacheContainsOrginObject())
			{
				if (IgnoringCacheCheck)
				{
					Position=Bot.Game.Bounty.CurrentBountyMapMarkers[OrginCacheObjectRAGUID].Position;
				}
				else
				{
					OrginCacheObject = ObjectCache.Objects[OrginCacheObjectRAGUID];
					Position = OrginCacheObject.Position;
				}
			}
		}

		public bool IsValidForTargeting()
		{
			if (IgnoringCacheCheck)
			{
				return true;
			}
			return OrginCacheObject.ObjectIsValidForTargeting;
		}



	}
}
