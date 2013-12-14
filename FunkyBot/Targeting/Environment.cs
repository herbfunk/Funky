using FunkyBot.Cache;
using System.Collections.Generic;
using FunkyBot.Cache.Objects;

namespace FunkyBot.Targeting
{
	class Environment
	{

		public Environment()
		  {
				bAnyLootableItemsNearby=false;
				iElitesWithinRange=new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
				iAnythingWithinRange=new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
				iNonRendedTargets_6=0;
				bAnyBossesInRange=false;
				bAnyChampionsPresent=false;
				bAnyTreasureGoblinsPresent=false;
				bAnyNonWWIgnoreMobsInRange=false;
				SurroundingUnits=0;
				UsesDOTDPSAbility=false;
		  }






		  internal List<CacheServerObject> NearbyObstacleObjects = new List<CacheServerObject>();
		  internal List<int> NearbyAvoidances = new List<int>();
		  internal List<CacheUnit> FleeTriggeringUnits = new List<CacheUnit>();
		  internal List<CacheAvoidance> TriggeringAvoidances = new List<CacheAvoidance>();
		  internal List<int> TriggeringAvoidanceRAGUIDs = new List<int>();

		
		  internal List<int> UnitRAGUIDs = new List<int>();
		  internal List<CacheUnit> DistantUnits = new List<CacheUnit>();
		  internal List<CacheObject> LoSMovementObjects = new List<CacheObject>();

		  // Variables relating to quick-reference of monsters within sepcific ranges (if anyone has suggestion for similar functionality with reduced CPU use, lemme know, but this is fast atm!)
		  internal int[] iElitesWithinRange;
		  internal int[] iAnythingWithinRange;

		  internal int iNonRendedTargets_6 { get; set; }
		  internal bool UsesDOTDPSAbility { get; set; }
		  internal int SurroundingUnits { get; set; }

		  internal bool bAnyLootableItemsNearby { get; set; }
		  internal bool bAnyChampionsPresent { get; set; }
		  internal bool bAnyTreasureGoblinsPresent { get; set; }
		  internal bool bAnyBossesInRange { get; set; }
		  // A flag to say whether any NONE-hashActorSNOWhirlwindIgnore things are around
		  internal bool bAnyNonWWIgnoreMobsInRange { get; set; }








		  internal void Reset()
		  {
				iElitesWithinRange=new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
				iAnythingWithinRange=new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
				iNonRendedTargets_6=0;

				bAnyBossesInRange=false;
				bAnyChampionsPresent=false;
				bAnyTreasureGoblinsPresent=false;

				bAnyNonWWIgnoreMobsInRange=false;
				bAnyLootableItemsNearby=false;
				
				UsesDOTDPSAbility=false;

				UnitRAGUIDs.Clear();
				SurroundingUnits=0;
				TriggeringAvoidances.Clear();
                TriggeringAvoidanceRAGUIDs.Clear();
				NearbyAvoidances.Clear();
				NearbyObstacleObjects.Clear();
				FleeTriggeringUnits.Clear();
				DistantUnits.Clear();
				LoSMovementObjects.Clear();
		  }
	}
}
