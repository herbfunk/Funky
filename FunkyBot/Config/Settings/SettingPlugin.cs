using FunkyBot.Movement;
namespace FunkyBot.Settings
{
	 //To hold all plugin internal related variables (for advance tweaking!)
	public class SettingPlugin
	{
		 public int GoldInactivityTimeoutMilliseconds { get; set; }
		 public int CacheObjectRefreshRate { get; set; }
		 public int UnusedSNORemovalRate { get; set; }
		 public int MovementNonMovementCount { get; set; }
		 public int OutofCombatMaxDistance { get; set; }
		 public bool CreateMuleOnStashFull { get; set; }

         public PointCheckingFlags AvoidanceFlags { get; set; }
         public PointCheckingFlags FleeingFlags { get; set; }

		 public SettingPlugin()
		 {
			  GoldInactivityTimeoutMilliseconds = 180000; //default of 3 minutes
			  CacheObjectRefreshRate=150;
			  UnusedSNORemovalRate=180000;
			  MovementNonMovementCount=50;
			  OutofCombatMaxDistance=50;
			  CreateMuleOnStashFull=true;
              AvoidanceFlags = PointCheckingFlags.AvoidanceIntersection | PointCheckingFlags.AvoidanceOverlap | PointCheckingFlags.ObstacleOverlap | PointCheckingFlags.BlockedDirection;
              FleeingFlags = PointCheckingFlags.AvoidanceIntersection | PointCheckingFlags.AvoidanceOverlap | PointCheckingFlags.ObstacleOverlap | PointCheckingFlags.Raycast | PointCheckingFlags.MonsterOverlap | PointCheckingFlags.BlockedDirection;

         }
	}
}
