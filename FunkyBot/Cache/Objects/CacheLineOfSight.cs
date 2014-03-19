using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FunkyBot.Cache.Enums;
using Zeta.Common;

namespace FunkyBot.Cache.Objects
{
	public class CacheLineOfSight : CacheObject
	{
		public CacheLineOfSight(int sno, int raguid, int acdguid, Vector3 position, string Name = null) : base(sno, raguid, acdguid, position, Name)
		{
		}

		public CacheLineOfSight(Vector3 thisposition, TargetType thisobjecttype = TargetType.None, double thisweight = 0, string name = null, float thisradius = 0, int thisractorguid = -1, int thissno = 0) : base(thisposition, thisobjecttype, thisweight, name, thisradius, thisractorguid, thissno)
		{
		}

		public CacheLineOfSight(CacheObject parent) : base(parent)
		{
		}
	}
}
