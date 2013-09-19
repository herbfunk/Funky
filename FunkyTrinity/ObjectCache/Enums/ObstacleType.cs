using System;

namespace FunkyTrinity.Cache.Enums
{
	 ///<summary>
	 ///Used to describe the objects obstacle type.
	 ///</summary>
	 [Flags]
	 public enum ObstacleType
	 {
			Monster=1,
			StaticAvoidance=2,
			MovingAvoidance=4,
			ServerObject=8,
			Destructable=16,
			None=32,

			All=Monster|MovingAvoidance|StaticAvoidance|ServerObject,
			Avoidance=StaticAvoidance|MovingAvoidance,
			Navigation=Monster|ServerObject|Destructable,
	 }
}