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

				if (DateTime.Now.Subtract(LastObstacleIntersectionTest).TotalMilliseconds>2000)
				{
					 Bot.Character.UpdateMovementData();

					 if (!Bot.Character.isMoving||Bot.Character.currentMovementState==MovementState.WalkingInPlace)
					 {
						  //Logging.WriteVerbose("Testing for obstacles");

						  LastObstacleIntersectionTest=DateTime.Now;

						  Vector3 CurrentPosition=Bot.Character.Position;
						  CurrentPosition.Z+=0.5f;

						  //Vector3 destination = Bot.CurrentTarget.ObjectData!=null?Bot.CurrentTarget.ObjectData.Position:DestinationVector!=vNullLocation?DestinationVector:vNullLocation;

						  float CurrentDirection=FindDirection(CurrentPosition, DestinationVector, true);
						  Vector3 IntersectionDestinationVector=MathEx.GetPointAt(Bot.Character.Position, range, CurrentDirection);
						  IntersectionDestinationVector.Z=mgp.GetHeight(IntersectionDestinationVector.ToVector2());
						  IntersectionDestinationVector.Z+=0.5f;

						  var intersectingObstacles=ObjectCache.Obstacles.Values.OfType<CacheServerObject>()
																					 .Where(obstacle => !Bot.Combat.PrioritizedRAGUIDs.Contains(obstacle.RAGUID)&&obstacle.Obstacletype.HasValue
																						  &&ObstacleType.Navigation.HasFlag(obstacle.Obstacletype.Value)
																						  &&obstacle.TestIntersection(CurrentPosition, IntersectionDestinationVector));

						  //(ObjectCache.Obstacles.DoesPositionIntersectAny(IntersectionDestinationVector, ObstacleType.ServerObject))
						  if (intersectingObstacles.Any())
						  {
								intersectingObstacles=intersectingObstacles.OrderBy(obstacle => obstacle.CentreDistance);

								CacheObstacle intersectingObstacle=intersectingObstacles.First();


								//Handle Destructable objects by making it a target!
								if ((ObstacleType.Monster|ObstacleType.Destructable).HasFlag(intersectingObstacle.Obstacletype.Value)
									 &&intersectingObstacle.ObjectIsValidForTargeting)
								{
									 Logging.WriteVerbose("Intersecting Object found and added to prioritized list {0}", intersectingObstacle.InternalName);

									 intersectingObstacle.PrioritizedDate=DateTime.Now;
									 Bot.Combat.PrioritizedRAGUIDs.Add(intersectingObstacle.RAGUID);
									 intersectingObstacle.BlacklistLoops=0;

									 //Force Update if we currently have a target..
									 if (Bot.Target.ObjectData!=null)
										  Bot.Combat.bForceTargetUpdate=true;
									 else
										  Bot.Combat.ResetTargetHandling();

									 return false;
								}

								//return false;

								//Logging.WriteVerbose("Intersecting Obstacle found, attempting to find a location to move. {0}", intersectingObstacle.InternalName);

								bool foundSpot=GridPointAreaCache.AttemptFindSafeSpot(out TempMovementVector, Bot.Character.Position);
								return foundSpot;
						  }
					 }
				}
				return false;
		  }
		  private static bool ObstacleCheck(out Vector3 TempMovementVector, CacheObject Target)
		  {
				TempMovementVector=vNullLocation;

				if (DateTime.Now.Subtract(LastObstacleIntersectionTest).TotalMilliseconds>2000)
				{
					 LastObstacleIntersectionTest=DateTime.Now;

					 Vector3 CurrentPosition=Bot.Character.Position;
					 CurrentPosition.Z+=0.5f;

					 //Vector3 destination = Bot.CurrentTarget.ObjectData!=null?Bot.CurrentTarget.ObjectData.Position:DestinationVector!=vNullLocation?DestinationVector:vNullLocation;

					 float CurrentDirection=FindDirection(CurrentPosition, Target.Position, true);
					 Vector3 IntersectionDestinationVector=MathEx.GetPointAt(Bot.Character.Position, Target.RadiusDistance, CurrentDirection);
					 IntersectionDestinationVector.Z=mgp.GetHeight(IntersectionDestinationVector.ToVector2());
					 IntersectionDestinationVector.Z+=0.5f;

					 var intersectingObstacles=ObjectCache.Obstacles.Values.OfType<CacheServerObject>()
						  .Where(obstacle => !Bot.Combat.PrioritizedRAGUIDs.Contains(obstacle.RAGUID)&&obstacle.Obstacletype.HasValue&&ObstacleType.Navigation.HasFlag(obstacle.Obstacletype.Value)&&obstacle.TestIntersection(CurrentPosition, IntersectionDestinationVector));
					 //(ObjectCache.Obstacles.DoesPositionIntersectAny(IntersectionDestinationVector, ObstacleType.ServerObject))
					 if (intersectingObstacles.Any())
					 {
						  intersectingObstacles=intersectingObstacles.OrderBy(obstacle => obstacle.CentreDistance);

						  foreach (var intersectingObstacle in intersectingObstacles.Where(obj => (ObstacleType.Monster|ObstacleType.Destructable).HasFlag(obj.Obstacletype.Value)&&obj.ObjectIsValidForTargeting))
						  {
								//Handle Destructable objects by making it a target!


								Logging.WriteVerbose("Intersecting Object found and added to prioritized list {0}", intersectingObstacle.InternalName);

								intersectingObstacle.PrioritizedDate=DateTime.Now;
								Bot.Combat.PrioritizedRAGUIDs.Add(intersectingObstacle.RAGUID);

								//Force Update if we currently have a target..
								if (Bot.Target.ObjectData!=null)
									 Bot.Combat.bForceTargetUpdate=true;
								else
									 Bot.Combat.ResetTargetHandling();

								return false;

								//Logging.WriteVerbose("Intersecting Obstacle found, attempting to find a location to move. {0}", intersectingObstacle.InternalName);
						  }

						  CacheObject nearestObj=intersectingObstacles.First();
						  if ((ObstacleType.ServerObject).HasFlag(nearestObj.Obstacletype.Value))
						  {
								bool foundSpot=nearestObj.GPRect.TryFindSafeSpot(out TempMovementVector, Target.BotMeleeVector, false);

								if (foundSpot)
								{
									 Vector3 newIntersectionVector=TempMovementVector;

									 //Test if this spot will still intersect
									 bool intersectingSpot=ObjectCache.Obstacles.Values.OfType<CacheServerObject>().Any(obstacle => obstacle.TestIntersection(CurrentPosition, newIntersectionVector));

									 if (intersectingSpot)
									 {
										  foundSpot=nearestObj.GPRect.TryFindSafeSpot(out TempMovementVector, newIntersectionVector, false);
									 }

									 //Logging.WriteVerbose("Found location to move around object");
								}

								return foundSpot;
						  }
					 }
				}
				return false;
		  }


	 }
}