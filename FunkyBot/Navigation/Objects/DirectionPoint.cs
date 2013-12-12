using Zeta.Common;

namespace FunkyBot.Movement
{


		  public struct DirectionPoint
		  {
				public float DirectionDegrees;
				public GridPoint StartingPoint;
				public GridPoint EndingPoint;
				public GridPoint Center;
				public float Range;

				private void update_(Vector3 startV3)
				{
					 //raycast to test how far we could go.. 
					 Vector3 MaxRangeTestVector3=MathEx.GetPointAt(startV3, Range, MathEx.ToRadians(DirectionDegrees));

					 Vector2 RaycastTestV2;
					 //we use main grid providers raycast to test since we are looking at how far we can travel and not if anything is blocking us.
					 if (Navigation.MGP.Raycast(startV3.ToVector2(), MaxRangeTestVector3.ToVector2(), out RaycastTestV2))
					 {//Set our endpoint at the Hit point
						  MaxRangeTestVector3=RaycastTestV2.ToVector3();
						  MaxRangeTestVector3.Z=Navigation.MGP.GetHeight(MaxRangeTestVector3.ToVector2()); //adjust height acordingly!
					 }
					 Range=Vector3.Distance2D(ref startV3, ref MaxRangeTestVector3);

					 //lets see if we can stand here at all?
					 if (!Navigation.MGP.CanStandAt(MaxRangeTestVector3))
					 {

						  //just because raycast set our max range, we need to see if we can use that cell as a walking point!
						  if (!Navigation.CanRayCast(startV3, MaxRangeTestVector3, Zeta.Internals.SNO.NavCellFlags.AllowWalk))
						  {
								//loop to find a walkable range.
								float currentRange=Range-2.5f;
								float directionRadianFlipped=Navigation.FindDirection(MaxRangeTestVector3, startV3, true);
								int maxTestAttempts=(int)(currentRange/2.5f);

								for (int i=0; i<maxTestAttempts; i++)
								{
									 Vector3 newtestPoint=MathEx.GetPointAt(MaxRangeTestVector3, currentRange, directionRadianFlipped);
									 newtestPoint.Z=Navigation.MGP.GetHeight(newtestPoint.ToVector2());//update Z
									 if (Navigation.CanRayCast(startV3, newtestPoint, Zeta.Internals.SNO.NavCellFlags.AllowWalk))
									 {
										  MaxRangeTestVector3=newtestPoint;
										  break;
									 }

									 if (currentRange-2.5f<=0f) break;
									 currentRange=-2.5f;
								}
								Range=currentRange;
						  }

					 }
					 
					 EndingPoint=MaxRangeTestVector3;
					 StartingPoint=startV3;
					 Range=startV3.Distance2D(MaxRangeTestVector3); //(float)GridPoint.GetDistanceBetweenPoints(StartingPoint, EndingPoint);
					 Center=MathEx.GetPointAt(startV3, Range/2, MathEx.ToRadians(DirectionDegrees));
				}

				public void UpdateRange(Vector3 start)
				{
					 StartingPoint=start;
					 update_(start);
				}

				public DirectionPoint(Vector3 start, float directiondegrees, float MaxRange=25f)
				{
					 StartingPoint=start;
					 DirectionDegrees=directiondegrees;
					 Range=MaxRange;
					 Center=start;
					 EndingPoint=start;
					 update_(start);
				}
		  }

	 
}