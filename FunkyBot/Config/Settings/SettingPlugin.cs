using FunkyBot.Movement;
namespace FunkyBot.Settings
{
	 //To hold all plugin internal related variables (for advance tweaking!)
	public class SettingPlugin
	{
		 public int CacheObjectRefreshRate { get; set; }
		 public int UnusedSNORemovalRate { get; set; }
		 public int MovementNonMovementCount { get; set; }
		 public int OutofCombatMaxDistance { get; set; }
		 public bool CreateMuleOnStashFull { get; set; }
         public bool EnableLineOfSightBehavior { get; set; }
         public PointCheckingFlags AvoidanceFlags { get; set; }

		 public SettingPlugin()
		 {
			  CacheObjectRefreshRate=150;
			  UnusedSNORemovalRate=180000;
			  MovementNonMovementCount=50;
			  OutofCombatMaxDistance=50;
			  CreateMuleOnStashFull=true;
              EnableLineOfSightBehavior = false;
              AvoidanceFlags = PointCheckingFlags.AvoidanceIntersection | PointCheckingFlags.AvoidanceOverlap | PointCheckingFlags.ObstacleOverlap | PointCheckingFlags.Raycast;
		 }
	}
}
