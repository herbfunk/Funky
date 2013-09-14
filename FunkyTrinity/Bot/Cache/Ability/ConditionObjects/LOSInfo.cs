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

		 private Vector3 LastVectorTested=Vector3.Zero;

		public string DebugString
		{
			get
			{
				 return String.Format("RayCast=={0} , ObjIntersect=={1} , Walk=={2} , Projectile=={3}",
												RayCast.HasValue?RayCast.Value.ToString():"null",
												ObjectIntersection.HasValue?ObjectIntersection.Value.ToString():"null",
												NavCellWalk.HasValue?NavCellWalk.Value.ToString():"null",
												NavCellProjectile.HasValue?NavCellProjectile.Value.ToString():"null");
			}
		}

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
		 public bool LOSTest(Vector3 PositionToTestFrom, bool NavRayCast=true, bool ServerObjectIntersection=true, NavCellFlags Flags=NavCellFlags.None, bool ContinueOnFailures=true)
		 {
			  this.LastLOSCheck=DateTime.Now;
			  bool Failed=false;


			  if (NavRayCast)
			  {//This is a basic raycast test to see if we have clear view of the object.
					RayCast=!Zeta.Navigation.Navigator.Raycast(PositionToTestFrom, Obj.Position);
					if (!RayCast.Value)
					{
						 Failed=true;
						 if (!ContinueOnFailures) return false;
					}
						 
			  }

			  if (ServerObjectIntersection)
			  {//This test uses the obstacle cache to check objects for intersection

					Vector3 TargetTestLocation=Obj.Position;
					if (Funky.Difference(Bot.Character.Position.Z,TargetTestLocation.Z)>1f)
					{
						//Get actual height using MGP
						 TargetTestLocation.Z=Navigation.MGP.GetHeight(TargetTestLocation.ToVector2());
					}

					ObjectIntersection=ObjectCache.Obstacles.Values.OfType<CacheServerObject>()
						 .Any(obstacle =>
							  obstacle.RAGUID!=Obj.RAGUID&&
							  obstacle.Obstacletype.HasValue&&
							  obstacle.Obstacletype.Value!=ObstacleType.Monster&&
							  obstacle.TestIntersection(PositionToTestFrom, TargetTestLocation));

					if (!Failed&&ObjectIntersection.Value)
					{
						 Failed=true;
						 if (!ContinueOnFailures) return false;
					}
			  }


			  if (!Flags.Equals(NavCellFlags.None))
			  {//Raycast test to validate it can preform the path -- walk/projectile
					bool NavTest=Navigation.CanRayCast(PositionToTestFrom, Obj.Position, Flags);

					if (Flags.HasFlag(NavCellFlags.AllowWalk)) NavCellWalk=NavTest;
					if (Flags.HasFlag(NavCellFlags.AllowProjectile)) NavCellProjectile=NavTest;
					if (!Failed&&!NavTest)
					{
						 Failed=true;
					}
			  }

			  //this.LastVectorTested=PositionToTestFrom;
			  return !Failed;
		 }
	}
}
