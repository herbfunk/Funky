using System;
using System.Linq;
using Zeta;
using System.Collections.Generic;
using Zeta.Common;
using System.Collections;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  internal static partial class GridPointAreaCache
		  {
				///<summary>
				///The maximum number of GPCs that will be added to cache during routine movement.
				///</summary>
				internal static int MovementGPRectMaxCount=3;
				///<summary>
				///This sets whether or not to cache GPCs during routine movement.
				///</summary>
				internal static bool EnableBacktrackGPRcache=false;
				///<summary>
				///The minimum range required to cache a new GPC during routine movement.
				///</summary>
				internal static double MinimumRangeBetweenMovementGPRs=20f;

				///<summary>
				///Cache of GPCs created during routine movement.
				///</summary>
				internal static List<GPRectangle> cacheMovementGPRs=new List<GPRectangle>();

				///<summary>
				///Creates new GPC at given Vector3 with expansion of 3 and adds it to the MovementGPCs list with any links found.
				///</summary>
				public static void StartNewGPR(Vector3 center)
				{//Creates a sample we could backtrack to if needed!

					 cacheMovementGPRs.Add(new GPRectangle(center, 3));
					 if (cacheMovementGPRs.Count>MovementGPRectMaxCount)
					 {
						  cacheMovementGPRs.RemoveAt(0);
						  cacheMovementGPRs.TrimExcess();
					 }
				}





				///<summary>
				///Bots Position is blocked from adjacent movement -- Updated whenever searching for a safe location!
				///</summary>
				internal static void RefreshNavigationBlocked()
				{
					 //Check if bot is navigationally blocked..
					 if (DateTime.Now.Subtract(LastNavigationBlockCheck).TotalMilliseconds>500)
					 {
						  LastNavigationBlockCheck=DateTime.Now;

						  if (IsVectorBlocked(Bot.Character.Position))
						  {
								Logging.WriteVerbose("Bots Current Position is navigationally blocked!");
								BotIsNavigationallyBlocked=true;
						  }
						  else
								BotIsNavigationallyBlocked=false;
					 }
				}
				internal static bool BotIsNavigationallyBlocked=false;
				private static DateTime LastNavigationBlockCheck=DateTime.Today;

				///<summary>
				///Location of the Bot when we searched for a safe location.
				///</summary>
				private static Vector3 LastSearchVector=vNullLocation;
				private static List<DirectionPoint> DirectionPoints=new List<DirectionPoint>();
				private static bool UpdatedLocalMovementTree=false;
				internal static List<GPRectangle> CacheSafeGPR=new List<GPRectangle>();

				///<summary>
				///Last successful GP Rectangle used during search method.
				///</summary>
				private static GPRectangle LastUsedRect=null;

				internal static HashSet<GridPoint> BlacklistedGridpoints=new HashSet<GridPoint>();
				private static double CurrentLocationWeight=0d;
				private static float MinimumChangeofDistanceBeforeRefresh=15f;


				internal static void BlacklistLastSafespot()
				{
					 if (LastUsedRect!=null)
					 {
						  //Blacklist the creation vector and nullify the last used..
						  BlacklistedGridpoints.Add(LastUsedRect.LastFoundSafeSpot);
					 }
					 vlastSafeSpot=vNullLocation;
				}

				///<summary>
				///Searches for a safespot!
				///</summary>
				public static bool AttemptFindSafeSpot(out Vector3 safespot, Vector3 LOS, bool kiting=false)
				{
					 safespot=vlastSafeSpot;
					 if (!Bot.Combat.TravellingAvoidance&&DateTime.Now.Subtract(lastFoundSafeSpot).TotalMilliseconds<=800
						&&vlastSafeSpot!=vNullLocation
						&&(!ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(vlastSafeSpot)))
					 //&&(!kiting||!ObjectCache.Objects.IsPointNearbyMonsters(vlastSafeSpot,Bot.KiteDistance)))
					 {	 //Already found a safe spot in the last 800ms
						  return true;
					 }


					 Vector3 BotPosition=Bot.Character.Position;

					 //Check if we should refresh..
					 if (CurrentGPAREA==null||CurrentGPAREA.AllGPRectsFailed&&!CurrentGPAREA.centerGPRect.Contains(BotPosition)||!CurrentGPAREA.GridPointContained(BotPosition))
						  CurrentGPAREA=new GPArea(BotPosition);

					 //Check Bot Navigationally blocked
					 RefreshNavigationBlocked();
					 if (BotIsNavigationallyBlocked) return false;

					 CurrentLocationGPRect.UpdateObjectCount();

					 safespot=CurrentGPAREA.AttemptFindSafeSpot(BotPosition, LOS, kiting);
					 return (safespot!=vNullLocation);
				}

				//private static DateTime LastClearedSearchLocationRAGUIDsCollection=DateTime.Today;
				//private static List<int> RecentSearchLocationUsedTargetRAGUIDs=new List<int>();

				//internal static void CheckClearedSearchRefresh()
				//{
				//    if (RecentSearchLocationUsedTargetRAGUIDs.Count>0&&DateTime.Now.Subtract(LastClearedSearchLocationRAGUIDsCollection).TotalMilliseconds>1000)
				//    {
				//        RecentSearchLocationUsedTargetRAGUIDs.Clear();
				//        LastClearedSearchLocationRAGUIDsCollection=DateTime.Now;
				//    }
				//}



				/////<summary>
				/////Resets direction vars and recreates local GPR (benethe the bot) and direction points
				/////</summary>
				//private static void Refresh(bool kiting)
				//{
				//    //Recreation of our current position and directions(maximum range)
				//    MinimumChangeofDistanceBeforeRefresh=25f;
				//    LastSearchVector=Bot.Character.Position;
				//    LastUsedRect=null;
				//    AllGPRectsFailed=false;
				//    float MaximumRangeAllowed=(kiting?125f:100f);

				//    //Clear Blacklisted
				//    BlacklistedGridpoints.Clear();
				//    DirectionPoints.Clear();

				//    LastNavigationBlockCheck=DateTime.Today;
				//    RefreshNavigationBlocked();

				//    //Now we expand our local GPC so we can get 24 direction points
				//    CurrentLocationGPRect.FullyExpand();

				//    //Update our base weight which we compare others with to see if its a better placement.
				//    CurrentLocationWeight=CurrentLocationGPRect.Weight;
				//    if (Bot.SettingsFunky.LogSafeMovementOutput)
				//        Logging.WriteVerbose("Current Location GPC Weight is {0}", CurrentLocationWeight);


				//    //Get all valid points (besides current point) from our current location GPR
				//    var SearchPoints=CurrentLocationGPRect.Points.Keys.Where(gp => !gp.Equals(CurrentLocationGPRect.centerpoint));


				//    //we should check our surrounding points to see if we can even move into any of them first!
				//    foreach (var item in SearchPoints)
				//    {
				//        //We only want points we can stand at.. since we will travel inside one of the 8 surrounding points!
				//        if (!NavigationBlockedPoints.Contains(item))
				//        {
				//            //Vector2 thisV2=mgp.GridToWorld(item);
				//            Vector3 thisV3=(Vector3)item;
				//            thisV3.Z+=(Bot.Character.fCharacterRadius/2f);

				//            //Its a valid point for direction testing!
				//            float DirectionDegrees=FindDirection(LastSearchVector, thisV3, false);
				//            DirectionPoint P=new DirectionPoint((Vector3)item, DirectionDegrees, MaximumRangeAllowed);
				//            DirectionPoints.Add(P);
				//        }
				//    }

				//    if (Bot.SettingsFunky.LogSafeMovementOutput)
				//        Logging.WriteVerbose("Total Direction Points Successfully Created {0}", DirectionPoints.Count);

				//    //Check if we swapped into our "Routine" cache GPCs..
				//    if (UpdatedLocalMovementTree)
				//    {
				//        UpdatedLocalMovementTree=false;

				//        if (CacheSafeGPR.Count>0)
				//        {
				//            //Swap back to our orginal GPCs
				//            cacheMovementGPRs.Clear();
				//            cacheMovementGPRs.AddRange(CacheSafeGPR.ToArray());
				//            CacheSafeGPR.Clear();
				//        }
				//    }

				//    //Update avoidance objects so they are sorted by distance from bot..
				//    ObjectCache.Obstacles.SortAvoidanceZones();
				//    if (Bot.SettingsFunky.LogSafeMovementOutput)
				//        Logging.WriteDiagnostic("Finished Creating Direction Points and Local GPC");
				//}

				//private static void CreateNewSurroundingGPRs(bool WeightSorted=true)
				//{
				//    List<GPRectangle> newMovementGPCs=new List<GPRectangle>();

				//    //Add local GPR first
				//    newMovementGPCs.Add(CurrentLocationGPRect);

				//    float maxrangeFound=0f;
				//    //Create GPCs based on DirectionPoints that have range > 5f
				//    foreach (var direction in DirectionPoints)
				//    {
				//        //Skip directions that have very low range possible.
				//        if (direction.Range<2.5f) continue;

				//        if (maxrangeFound<direction.Range)
				//            maxrangeFound=direction.Range;

				//        GPRectangle newEntry=new GPRectangle(direction);
				//        newMovementGPCs.Add(newEntry);
				//    }

				//    //Cache our movement GPCs to switch back after we are done.
				//    CacheSafeGPR=new List<GPRectangle>();
				//    CacheSafeGPR.AddRange(cacheMovementGPRs.ToArray());

				//    GridPoint botPoint=Bot.Character.PointPosition;
				//    if (WeightSorted)
				//        cacheMovementGPRs=new List<GPRectangle>(newMovementGPCs.OrderBy(gpc => gpc.Weight).ToArray());
				//    else
				//        cacheMovementGPRs=new List<GPRectangle>(newMovementGPCs.OrderByDescending(gpc => gpc.centerpoint.Distance(botPoint)).ToArray());


				//    MinimumChangeofDistanceBeforeRefresh=maxrangeFound;
				//    UpdatedLocalMovementTree=true;
				//    if (Bot.SettingsFunky.LogSafeMovementOutput) Logging.WriteDiagnostic("Updated Local GPCs");
				//}
		  }
	 }
}