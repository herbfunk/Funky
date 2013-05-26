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

		  private static List<Cluster> RunKmeans<T>(List<T> unitList, double distance) where T : CacheObject
		  {
				List<Cluster> LC_=new List<Cluster>();

				if (unitList.Count==0)
					 return LC_;

				List<CacheObject> l_ListUnits=new List<CacheObject>(unitList.ToArray());

				if (l_ListUnits.Count==0)
					 return LC_;

				

				// for starters, take a point to create one cluster
				CacheUnit l_P1=(CacheUnit)l_ListUnits[0];

				l_ListUnits.RemoveAt(0);

				// so far, we have a one-point cluster
				LC_.Add(new Cluster(distance, l_P1));

				#region Main Loop
				// the algorithm is inside this loop
				List<Cluster> l_ListAttainableClusters;
				Cluster l_c;
				foreach (CacheUnit p in l_ListUnits)
				{
					 l_ListAttainableClusters=new List<Cluster>();
					 l_ListAttainableClusters=LC_.FindAll(x => x.IsPointReachable(p.PointPosition));
					 LC_.RemoveAll(x => x.IsPointReachable(p.PointPosition));
					 l_c=new Cluster(distance, p);
					 // merge point's "reachable" clusters
					 if (l_ListAttainableClusters.Count>0)
						  l_c.AnnexCluster(l_ListAttainableClusters.Aggregate((c, x) =>
							 c=Cluster.MergeClusters(x, c)));
					 LC_.Add(l_c);
					 //Logging.WriteVerbose("Cluster Found: Total Points {0} with Centeroid {1}", l_c.ListPoints.Count, l_c.Centeroid.ToString());
					 l_ListAttainableClusters=null;
					 l_c=null;
				}  // of loop over candidate points

				LC_=LC_.OrderByDescending(o => o.ListPoints.Count).ToList();
				#endregion

				return LC_;
		  }


		  // **********************************************************************************************
		  // *****                Quick Easy Raycast Function for quick changes                       *****
		  // **********************************************************************************************
		  public static bool GilesCanRayCast(Vector3 vStartLocation, Vector3 vDestination, NavCellFlags NavType=NavCellFlags.None)
		  {
				if (NavType!=NavCellFlags.None)
					 return ZetaDia.Physics.Raycast(new Vector3(vStartLocation.X, vStartLocation.Y, vStartLocation.Z+1f), new Vector3(vDestination.X, vDestination.Y, vDestination.Z+1f), NavType);

				return !Navigator.Raycast(new Vector3(vStartLocation.X, vStartLocation.Y, vStartLocation.Z+1f), new Vector3(vDestination.X, vDestination.Y, vDestination.Z+1f));
		  }

		  public static bool ProjectileIntersects(CacheAvoidance Projectile, Vector3 start, Vector3 end, float ProjectileDistance=50f)
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


		  internal static Vector3 GetCenteroid(Vector3 v1, Vector3 v2)
		  {
				return (v1+(v2+v1)/2);
		  }





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