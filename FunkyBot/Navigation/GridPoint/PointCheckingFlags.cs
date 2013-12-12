using System;

namespace FunkyBot.Movement
{
    /*
 * Obstacle Overlap
 * Avoidance Overlap
 * Monster Overlap
 * Raycast
 * Avoidance Intersection
 */

    [Flags]
    public enum PointCheckingFlags
    {
        None=0,
        AvoidanceOverlap=1,
        ObstacleOverlap=2,
        MonsterOverlap=4,
        AvoidanceIntersection=8,
        Raycast=16,
        RaycastWalkable=32,
        RaycastNavProvider=64,
        BlockedDirection=128,
    }

}
