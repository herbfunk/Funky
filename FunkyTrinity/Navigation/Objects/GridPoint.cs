using System;
using System.Collections.Generic;
using Zeta;
using Zeta.Common;

namespace FunkyBot.Movement
{
		  ///<summary>
		  ///X,Y of a point. Conversions added for System Points and Vectors.
		  ///</summary>
		  public class GridPoint : IComparer<GridPoint>
		  {
				private float x_;
				public float X
				{
					 get
					 {
						  return x_;
					 }
				}

				private float y_;
				public float Y
				{
					 get
					 {
						  return y_;
					 }
				}
				private float z_;
				public float Z
				{
					 get
					 {
						  return z_;
					 }
				}
				private int ignored_;
				public bool Ignored
				{
					 get
					 {
						  return (ignored_&1)==1;
					 }
					 set
					 {
						  if (value)
								ignored_=1;
						  else
								ignored_=0;
					 }
				}

				#region Windows.Point.Conversions
				public static implicit operator GridPoint(System.Windows.Point P)
				{
					 return new GridPoint((float)P.X, (float)P.Y);
				}
				public static implicit operator System.Windows.Point(GridPoint P)
				{
					 return new System.Windows.Point(P.X, P.Y);
				}
				#endregion

				#region Drawing.Point Conversions
				public static implicit operator GridPoint(System.Drawing.Point P)
				{
					 return new GridPoint((float)P.X, (float)P.Y);
				}
				public static implicit operator System.Drawing.Point(GridPoint P)
				{
					 return new System.Drawing.Point((int)P.X, (int)P.Y);
				}
				#endregion

				#region VectorConversions
				public static implicit operator GridPoint(Vector3 V)
				{
					 //return new GridPoint(V.X, V.Y, V.Z);
					 return Navigation.MGP.WorldToGrid(V.ToVector2());
				}
				public static implicit operator Vector2(GridPoint P)
				{
					 return Navigation.MGP.GridToWorld(P);
				}
				public static explicit operator Vector3(GridPoint P)
				{
					 //return new Vector3(P.X, P.Y, P.Z);
					 Vector2 v_=Navigation.MGP.GridToWorld(P);
					 Vector3 p_=new Vector3(v_.X, v_.Y, Navigation.MGP.GetHeight(v_));
					 return p_;
				}
				#endregion


				public static bool operator<(GridPoint LS, GridPoint RS)
				{
					 return (LS.X<RS.X&&LS.Y<RS.Y);
				}
				public static bool operator<=(GridPoint LS, GridPoint RS)
				{
					 return (LS.X<=RS.X&&LS.Y<=RS.Y);
				}
				public static bool operator>(GridPoint LS, GridPoint RS)
				{
					 return (LS.X>RS.X&&LS.Y>RS.Y);
				}
				public static bool operator>=(GridPoint LS, GridPoint RS)
				{
					 return (LS.X>=RS.X&&LS.Y>=RS.Y);
				}
				public static GridPoint operator+(GridPoint a, GridPoint b)
				{
					 return new GridPoint(a.X+b.X, a.Y+b.Y);
				}
				public static GridPoint operator/(GridPoint a, int Amount)
				{
					 return new GridPoint(a.X/Amount, a.Y/Amount);
				}
				public static GridPoint GetCenteroid(GridPoint A, GridPoint B)
				{
					 return new GridPoint((A.X+B.X)/2f, (A.Y+B.Y)/2f);
				}
				//public static double AngleDegreeBetweenPoints(GridPoint A, GridPoint B)
				//{
				//	 float DeltaY=B.Y-A.Y;
				//	 float DeltaX=B.X-A.X;
				//	 return Math.Atan2(DeltaY, DeltaX)*180/Math.PI;
				//}
				//public static bool LinesIntersect(GridPoint Astart, GridPoint Aend, GridPoint test, float testradiussqrt)
				//{
				//	 System.Drawing.Size offsetSize=new System.Drawing.Size((int)testradiussqrt, (int)testradiussqrt);
				//	 System.Drawing.Point testPoint=new System.Drawing.Point((int)test.X, (int)test.Y);
				//	 return LineIntersectsLine(Astart, Aend, System.Drawing.Point.Add(testPoint, offsetSize), System.Drawing.Point.Subtract(testPoint, offsetSize));
				//}
				public static bool LineIntersectsRect(GridPoint p1, GridPoint p2, System.Windows.Rect r)
				{
					 return LineIntersectsLine(p1, p2, new GridPoint(r.X, r.Y), new GridPoint(r.X+r.Width, r.Y))||
							  LineIntersectsLine(p1, p2, new GridPoint(r.X+r.Width, r.Y), new GridPoint(r.X+r.Width, r.Y+r.Height))||
							  LineIntersectsLine(p1, p2, new GridPoint(r.X+r.Width, r.Y+r.Height), new GridPoint(r.X, r.Y+r.Height))||
							  LineIntersectsLine(p1, p2, new GridPoint(r.X, r.Y+r.Height), new GridPoint(r.X, r.Y))||
							  (r.Contains(p1)&&r.Contains(p2));
				}
				public static double GetDistanceBetweenPoints(GridPoint p, GridPoint q)
				{
					 if (q<p)
					 {
						  GridPoint switchPoint=q.Clone();
						  q=p.Clone();
						  p=switchPoint;
					 }

					 double a=p.X-q.X;
					 double b=p.Y-q.Y;
					 double distance=Math.Sqrt(a*a+b*b);
					 return distance;
				}
				public static bool LineIntersectsLine(GridPoint l1p1, GridPoint l1p2, GridPoint l2p1, GridPoint l2p2)
				{
					 float q=(l1p1.Y-l2p1.Y)*(l2p2.X-l2p1.X)-(l1p1.X-l2p1.X)*(l2p2.Y-l2p1.Y);
					 float d=(l1p2.X-l1p1.X)*(l2p2.Y-l2p1.Y)-(l1p2.Y-l1p1.Y)*(l2p2.X-l2p1.X);

					 if (d==0)
					 {
						  return false;
					 }

					 float r=q/d;

					 q=(l1p1.Y-l2p1.Y)*(l1p2.X-l1p1.X)-(l1p1.X-l2p1.X)*(l1p2.Y-l1p1.Y);
					 float s=q/d;

					 if (r<0||r>1||s<0||s>1)
					 {
						  return false;
					 }

					 return true;
				}
				public static bool IsOnLine(GridPoint endPoint1, GridPoint endPoint2, GridPoint checkPoint)
				{
					 return (checkPoint.Y-endPoint1.Y)/(checkPoint.X-endPoint1.X)
						   ==(endPoint2.Y-endPoint1.Y)/(endPoint2.X-endPoint1.X);
				}
				//public bool IsConnectedWith(GridPoint Test)
				//{
				//	 if (Difference(this.X, Test.X)>1||Difference(this.Y, Test.Y)>1) 
				//		  return false;

				//	 return true;
				//}
				public double Distance(GridPoint p)
				{
					 GridPoint q=this.Clone();
					 if (q<p)
					 {
						  GridPoint switchPoint=q.Clone();
						  q=p.Clone();
						  p=switchPoint;
					 }

					 double a=p.X-q.X;
					 double b=p.Y-q.Y;
					 double distance=Math.Sqrt(a*a+b*b);
					 return distance;
				}


				public GridPoint(float x, float y, float z=0f, bool ignored=false)
				{
					 this.x_=x;
					 this.y_=y;
					 this.z_=z;
					 this.Ignored=ignored;
				}
				public GridPoint(double x, double y, float z=0f, bool ignored=false)
				{
					 this.x_=(float)x;
					 this.y_=(float)y;
					 this.z_=z;
					 this.Ignored=ignored;
				}

				public GridPoint Clone()
				{
					 return new GridPoint(this.X, this.Y, this.Z, this.Ignored);
				}

				public override string ToString()
				{
					 return X.ToString()+","+Y.ToString();
				}

				public override bool Equals(object obj)
				{
					 //Check for null and compare run-time types. 
					 if (obj==null||this.GetType()!=obj.GetType())
					 {
						  return false;
					 }
					 else
					 {
						  GridPoint p=(GridPoint)obj;
						  return (X==p.X)&&(Y==p.Y);
					 }
				}
				public override int GetHashCode()
				{
					 return (int)X^(int)Y;
				}


				public int Compare(GridPoint x, GridPoint y)
				{
					 int l_v;

					 double l_d=GridPoint.GetDistanceBetweenPoints(x, y);

					 if (l_d==0)
						  l_v=0;
					 else if (l_d>0)
						  l_v=1;
					 else
						  l_v=-1;

					 return l_v;
				}
		  }
	 
}