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

		  private static void ObstacleCheck(Vector3 DestinationVector, float range=20f)
		  {

				if (DateTime.Now.Subtract(LastObstacleIntersectionTest).TotalMilliseconds>1500)
				{
					 Bot.Character.UpdateMovementData();

					 if (!Bot.Character.isMoving||Bot.Character.currentMovementState==MovementState.WalkingInPlace)
					 {
						  LastObstacleIntersectionTest=DateTime.Now;

						  Vector3 CurrentPosition=Bot.Character.Position;
						  Vector3 IntersectionDestinationVector=MathEx.GetPointAt(CurrentPosition, range, Bot.Character.currentRotation);
						  //GridPoint IntersectionDestinationPoint=(GridPoint)IntersectionDestinationVector;
						  //GridPoint BotGridPoint=Bot.Character.PointPosition;

						  //get collection of objects that pass the tests.
						  var intersectingObstacles=Bot.Combat.NearbyObstacleObjects //ObjectCache.Obstacles.Values.OfType<CacheServerObject>()
																					 .Where(obstacle =>
																						  !Bot.Combat.PrioritizedRAGUIDs.Contains(obstacle.RAGUID)//Only objects not already prioritized
																						  &&obstacle.Obstacletype.HasValue
																						  &&ObstacleType.Navigation.HasFlag(obstacle.Obstacletype.Value)//only navigation/intersection blocking objects!
																						  &&obstacle.CentreDistance<=range //Only within range..
																						  &&obstacle.BotIsFacing());
																						  //&&obstacle.TestIntersection(BotGridPoint, IntersectionDestinationPoint));



						  if (intersectingObstacles.Any())
						  {
								var intersectingObjectRAGUIDs=(from objs in intersectingObstacles
																		 select objs.RAGUID);

								Bot.Combat.PrioritizedRAGUIDs.AddRange(intersectingObjectRAGUIDs);
						  }
					 }
				}
		  }

	 }
}