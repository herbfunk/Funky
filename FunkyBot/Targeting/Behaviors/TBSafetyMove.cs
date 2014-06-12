using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using Zeta.Common;

namespace FunkyBot.Targeting.Behaviors
{
	public class TBSafetyMove : TargetBehavior
	{
		private DateTime AvoidRetryDate = DateTime.Today;
		private int iSecondsAvoidMoveFor = 0;
		private Vector3 LastSafeSpotFound = Vector3.Zero;
		private CacheUnit LastUnitAvoided = null;

		public TBSafetyMove() : base() { }
		public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.SafetyMove; } }
		public override bool BehavioralCondition
		{
			get
			{
				return !Bot.Targeting.Cache.RequiresAvoidance && Bot.Targeting.Cache.Environment.UnitAnimationWatchList.Count > 0;
			}
		}

		public override void Initialize()
		{
			base.Test = (ref CacheObject obj) =>
			{
				//Determine if any of the Watch List should attempt to find a movement location!
				if (LastSafeSpotFound != Vector3.Zero && LastUnitAvoided != null && Bot.Targeting.Cache.Environment.UnitAnimationWatchList.Contains(LastUnitAvoided))
				{
					if (DateTime.Now.Subtract(Bot.Targeting.Cache.LastAvoidanceMovement).TotalSeconds < this.iSecondsAvoidMoveFor)
					{
						LastSafeSpotFound = Vector3.Zero;
					}
					else
					{
						obj = new CacheObject(LastSafeSpotFound, TargetType.Avoidance, 20000f, "SafetyMovement", 2.5f, LastSafeSpotFound.GetHashCode());
						return true;
					}
				}
				else if(LastUnitAvoided != null && !Bot.Targeting.Cache.Environment.UnitAnimationWatchList.Contains(LastUnitAvoided))
				{
					LastUnitAvoided = null;
				}

				CacheUnit Cobj = Bot.Targeting.Cache.Environment.UnitAnimationWatchList.First();
				Vector3 safespot = Bot.NavigationCache.FindLocationBehindObject(Cobj);

				if (safespot!=Vector3.Zero)
				{
					float distance = safespot.Distance(Bot.Character.Data.Position);
					LastUnitAvoided = Cobj;
					LastSafeSpotFound = safespot;
					iSecondsAvoidMoveFor = 1 + (int)(distance / 2.5f);

					obj = new CacheObject(safespot, TargetType.Avoidance, 20000f, "SafetyMovement", 2.5f, safespot.GetHashCode());
				
					return true;
				}

				return false;
			};

		}
	}
}
