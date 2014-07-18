using System;
using System.Linq;
using fBaseXtensions.Game;
using FunkyBot.Cache.Enums;
using FunkyBot.Movement;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game.Internals.SNO;

namespace FunkyBot.Cache.Objects
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
		 public bool? SearchGridRayCast { get; set; }
		 public bool? ObjectIntersection { get; set; }
		 public bool? NavCellWalk { get; set; }
		 public bool? NavCellProjectile { get; set; }


		public string DebugString
		{
			get
			{
				 return String.Format("RayCast=={0} , SearchGridRayCast={4} , ObjIntersect=={1} , Walk=={2} , Projectile=={3}",
												RayCast.HasValue?RayCast.Value.ToString():"null",
												ObjectIntersection.HasValue?ObjectIntersection.Value.ToString():"null",
												NavCellWalk.HasValue?NavCellWalk.Value.ToString():"null",
												NavCellProjectile.HasValue?NavCellProjectile.Value.ToString():"null",
												SearchGridRayCast.HasValue?SearchGridRayCast.Value.ToString():"null");
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
		public bool LOSTest(Vector3 PositionToTestFrom, bool NavRayCast = true, bool searchGridRayCast = false, bool ServerObjectIntersection = true, NavCellFlags Flags = NavCellFlags.None, bool ContinueOnFailures = true)
		{
			return LOSTest(PositionToTestFrom, Obj.Position, NavRayCast, searchGridRayCast, ServerObjectIntersection, Flags, ContinueOnFailures);
		}

		///<summary>
		 ///Runs raycasting and intersection tests to validate LOS.
		 ///</summary>
		 public bool LOSTest(Vector3 PositionToTestFrom, Vector3 objposition, bool NavRayCast = true, bool searchGridRayCast = false, bool ServerObjectIntersection = true, NavCellFlags Flags = NavCellFlags.None, bool ContinueOnFailures = true)
		 {
			  LastLOSCheck=DateTime.Now;
			  bool Failed=false;


			  if (NavRayCast)
			  {//This is a basic raycast test to see if we have clear view of the object.

					if (PositionToTestFrom.Z > objposition.Z)
						RayCast = Navigation.CanRayCast(PositionToTestFrom, objposition);
					else
						RayCast = Navigation.CanRayCast(objposition, PositionToTestFrom);

					if (!RayCast.Value)
					{
						 Failed=true;
						 if (!ContinueOnFailures) return false;
					}
						 
			  }

			 if (searchGridRayCast)
			 {
				 Vector2 hitpos;

				 //Must check the Z difference.. (so we don't get false-positive result)
				 if (PositionToTestFrom.Z > objposition.Z)
					 SearchGridRayCast = !Navigator.SearchGridProvider.Raycast(PositionToTestFrom.ToVector2(), objposition.ToVector2(), out hitpos);
				 else
					 SearchGridRayCast = !Navigator.SearchGridProvider.Raycast(objposition.ToVector2(), PositionToTestFrom.ToVector2(), out hitpos);
					
				 if (!SearchGridRayCast.Value)
				 {
					 Failed = true;
					 if (!ContinueOnFailures) return false;
				 }
			 }

			  if (ServerObjectIntersection)
			  {//This test uses the obstacle cache to check objects for intersection

					if (Funky.Difference(FunkyGame.Hero.Position.Z, objposition.Z) > 1f)
					{
						//Get actual height using MGP
						objposition.Z = Navigation.MGP.GetHeight(objposition.ToVector2());
					}

					ObjectIntersection=ObjectCache.Obstacles.Values.OfType<CacheServerObject>()
						 .Any(obstacle =>
							  obstacle.RAGUID!=Obj.RAGUID&&
							  obstacle.Obstacletype.HasValue&&
							  obstacle.Obstacletype.Value==ObstacleType.ServerObject &&
							  obstacle.TestIntersection(PositionToTestFrom, objposition,false));

					if (!Failed&&ObjectIntersection.Value)
					{
						 Failed=true;
						 if (!ContinueOnFailures) return false;
					}
			  }


			  if (!Flags.Equals(NavCellFlags.None))
			  {//Raycast test to validate it can preform the path -- walk/projectile
				  bool NavTest = Navigation.CanRayCast(PositionToTestFrom, objposition, Flags);

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
