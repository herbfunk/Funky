using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.Internals.SNO;
using Zeta.Navigation;
using System.Collections.Generic;
using System.Drawing;

namespace FunkyTrinity
{
    public partial class Funky
    {



		  // **********************************************************************************************
		  // *****                Quick Easy Raycast Function for quick changes                       *****
		  // **********************************************************************************************
		  public static bool GilesCanRayCast(Vector3 vStartLocation, Vector3 vDestination, NavCellFlags NavType=NavCellFlags.None)
		  {
				if (NavType!=NavCellFlags.None)
					 return ZetaDia.Physics.Raycast(vStartLocation, vDestination, NavType);

				return !Navigator.Raycast(vStartLocation,vDestination);
		  }

		  internal static bool ProjectileIntersects(CacheAvoidance Projectile, Vector3 start, Vector3 end, float ProjectileDistance=50f)
		  {
				Vector3 ProjectileEndPoint=MathEx.GetPointAt(Projectile.Position, ProjectileDistance, Projectile.Rotation);
				return GridPoint.LineIntersectsLine(start, end, Projectile.PointPosition, ProjectileEndPoint);
		  }
		  internal static float Difference(float A, float B)
		  {
				return Math.Abs(A-B);
		  }

		  //Since my code uses System.Windows.Point to test against Rect intersections.. Conversion between System.Drawing.Point and System.Windows.Point is necessary :(




		  // **********************************************************************************************
		  // *****                            Calculate direction of A to B                           *****
		  // **********************************************************************************************
		  // Quickly calculate the direction a vector is from ourselves, and return it as a float
		  public static float FindDirection(Vector3 vStartLocation, Vector3 vTargetLocation, bool inRadian=false)
		  {
				Vector3 startNormalized=vStartLocation;
				startNormalized.Normalize();
				Vector3 targetNormalized=vTargetLocation;
				targetNormalized.Normalize();

				float angle=NormalizeRadian((float)Math.Atan2((vTargetLocation.Y-vStartLocation.Y),(vTargetLocation.X-vStartLocation.X)));
					 //MathEx.ToDegrees(NormalizeRadian((float)Math.Atan2(vTargetLocation.Y-vStartLocation.Y, vTargetLocation.X-vStartLocation.X)));

				

				if (inRadian)
					 return angle;
				else
					 return MathEx.ToDegrees(angle);
		  }

		  // **********************************************************************************************
		  // *****                       Check if an obstacle is blocking our path                    *****
		  // **********************************************************************************************
		  public static bool GilesIntersectsPath(Vector3 obstacle, float radius, Vector3 start, Vector3 destination)
		  {
				return MathEx.IntersectsPath(obstacle, radius, start, destination);
		  }
		  public static float NormalizeRadian(float radian)
		  {
				if (radian<0)
				{
					 double mod=-radian;
					 mod%=Math.PI*2d;
					 mod=-mod+Math.PI*2d;
					 return (float)mod;
				}
				return (float)(radian%(Math.PI*2d));
		  }


		  //internal static Vector3 GetCenteroid(Vector3 v1, Vector3 v2)
		  //{
		  //	 return (v1+(v2+v1)/2);
		  //}





		  public static string GetHeadingToPoint(Vector3 TargetPoint)
		  {
				return GetHeading(FindDirection(Bot.Character.Position, TargetPoint));
		  }
		  public static string GetHeading(float heading)
		  {
				var directions=new string[] {
              //"n", "ne", "e", "se", "s", "sw", "w", "nw", "n"
                "s", "se", "e", "ne", "n", "nw", "w", "sw", "s"
            };

				var index=(((int)heading)+23)/45;
				return directions[index].ToUpper();
		  }

    }
}