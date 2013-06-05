using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  // For "position-shifting" to navigate around obstacle SNO's
		  private static DateTime LastObstacleIntersectionTest=DateTime.Today;
		  private static Vector3 vShiftedPosition=Vector3.Zero;
		  private static DateTime lastShiftedPosition=DateTime.Today;
		  private static int iShiftPositionFor=0;

		  private static bool ObstacleCheck(out Vector3 TempMovementVector, Vector3 DestinationVector, float range=20f)
		  {
				TempMovementVector=vNullLocation;

				if (DateTime.Now.Subtract(LastObstacleIntersectionTest).TotalMilliseconds>1500)
				{
					 Bot.Character.UpdateMovementData();

					 if (!Bot.Character.isMoving||Bot.Character.currentMovementState==MovementState.WalkingInPlace)
					 {
						  LastObstacleIntersectionTest=DateTime.Now;

						  Vector3 CurrentPosition=Bot.Character.Position;
						  Vector3 IntersectionDestinationVector=MathEx.GetPointAt(Bot.Character.Position, range, Bot.Character.currentRotation);

						  //get collection of objects that pass the tests.
						  GridPoint BotPoint=Bot.Character.PointPosition;
						  var intersectingObstacles=ObjectCache.Obstacles.Values.OfType<CacheServerObject>()
																					 .Where(obstacle =>
																						  !Bot.Combat.PrioritizedRAGUIDs.Contains(obstacle.RAGUID)//Only objects not already prioritized
																						  &&obstacle.Obstacletype.HasValue
																						  &&ObstacleType.Navigation.HasFlag(obstacle.Obstacletype.Value)//only navigation/intersection blocking objects!
																						  &&obstacle.CentreDistance<30f //Only within range..
																						  &&obstacle.TestIntersection(BotPoint, (GridPoint)IntersectionDestinationVector));



						  if (intersectingObstacles.Any())
						  {
								intersectingObstacles=intersectingObstacles.OrderBy(obstacle => obstacle.CentreDistance);

								//get monsters and prioritize them
								var intersectingUnits=intersectingObstacles.Where(obstacle => obstacle.Obstacletype.Value==ObstacleType.Monster).ToList();
								if (intersectingUnits.Any())
								{
									 int counter=0;
									 foreach (var item in intersectingUnits)
									 {
										  Bot.Combat.PrioritizedRAGUIDs.Add(item.RAGUID);
										  item.PriorityCounter++;
										  counter++;
									 }
									 Logging.WriteVerbose("A total of {0} units were prioritized!", counter.ToString());
								}

								////find any non-monsters within 20f and find a location around it..
								//foreach (var intersectingObstacle in intersectingObstacles.Except(intersectingUnits).Where(obj => obj.CentreDistance<20f))
								//{

								//    if (intersectingObstacle.GPRect.TryFindSafeSpot(out TempMovementVector, DestinationVector, false, true))
								//    {
								//        Logging.WriteVerbose("Found intersecting obstacle object {0} -- using LOS Vector to move to destination", intersectingObstacle.InternalName);
								//        return true;
								//    }
								//}
								//return true;
						  }
					 }
				}
				//nothing?
				return false;
		  }

	 }
}