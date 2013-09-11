using System;
using System.Linq;
using FunkyTrinity.Cache;
using FunkyTrinity.Enums;
using FunkyTrinity.Movement;
using Zeta.Common;
using Zeta.Internals.SNO;

namespace FunkyTrinity.ability
{
	public class LOSInfo
	{
		 public LOSInfo(CacheObject obj)
		 {
			  Obj=obj;
		 }

		 //The Object Being Tested
		 private CacheObject Obj;

		 //Each Test Results
		 public bool? RayCast { get; set; }
		 public bool? ObjectIntersection { get; set; }
		 public bool? NavCellWalk { get; set; }
		 public bool? NavCellProjectile { get; set; }

		///<summary>
		///Last time we preformed tests.
		///</summary>
		public DateTime LastLOSCheck
		{
			 get { return lastloscheck; }
			 set { lastloscheck=value; }
		}
		private DateTime lastloscheck=DateTime.Today;

		public double LastLOSCheckMS
		{
			 get
			 {
				  return (DateTime.Now.Subtract(LastLOSCheck).TotalMilliseconds);
			 }
		}

		 ///<summary>
		 ///Runs raycasting and intersection tests to validate LOS.
		 ///</summary>
		 public bool LOSTest(Vector3 PositionToTestFrom, bool NavRayCast=true, bool ServerObjectIntersection=true, NavCellFlags Flags=NavCellFlags.None)
		 {
			  this.LastLOSCheck=DateTime.Now;

			  bool Failed=false;

			  if (NavRayCast)
			  {
					RayCast=!Zeta.Navigation.Navigator.Raycast(PositionToTestFrom, Obj.Position);
					if (!RayCast.Value) Failed=true;
			  }

			  if (ServerObjectIntersection)
			  {
					ObjectIntersection=ObjectCache.Obstacles.Values.OfType<CacheServerObject>()
						 .Any(obstacle =>
							  obstacle.RAGUID!=Obj.RAGUID&&
							  obstacle.Obstacletype.HasValue&&
							  obstacle.Obstacletype.Value!=ObstacleType.Monster&&
							  obstacle.TestIntersection(PositionToTestFrom, Obj.Position));

					if (!Failed&&ObjectIntersection.Value) Failed=true;
			  }
					

			  if (!Flags.Equals(NavCellFlags.None))
			  {
					bool NavTest=Navigation.CanRayCast(PositionToTestFrom, Obj.Position, Flags);

					if (Flags.HasFlag(NavCellFlags.AllowWalk)) NavCellWalk=NavTest;
					if (Flags.HasFlag(NavCellFlags.AllowProjectile)) NavCellProjectile=NavTest;
					if (!Failed&&!NavTest) Failed=true;
			  }

			  return !Failed;
		 }
	}
}
