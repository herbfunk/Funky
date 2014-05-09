using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FunkyBot.Cache.Enums;
using Zeta.Common;

namespace FunkyBot.Cache.Objects
{
	public class CacheNavPoint : CacheObject
	{
		public CacheNavPoint(Vector3 thisposition, TargetType thisobjecttype = TargetType.None, double thisweight = 0, string name = null, float thisradius = 0, int thisractorguid = -1, int thissno = 0) : base(thisposition, thisobjecttype, thisweight, name, thisradius, thisractorguid, thissno)
		{

		}

		public override bool ObjectIsValidForTargeting
		{
			get
			{ 
				return base.ObjectIsValidForTargeting; 
			}
		}
	}
}
