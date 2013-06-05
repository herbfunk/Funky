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
		  public static partial class GridPointAreaCache
		  {
				///<summary>
				///The maximum number of GPCs that will be added to cache during routine movement.
				///</summary>
				internal static int MovementGPRectMaxCount=3;
				///<summary>
				///This sets whether or not to cache GPCs during routine movement.
				///</summary>
				internal static bool EnableBacktrackGPRcache=true;
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

				private static DateTime LastNavigationBlockCheck=DateTime.Today;
				private static bool BotIsNavigationallyBlocked=false;
				private static List<GridPoint> NavigationBlockedPoints=new List<GridPoint>();
				internal static bool IsPointNavigationallyBlocked(GridPoint GP)
				{
					 Vector3 GPV3=(Vector3)GP;
					 var SurroundingPoints=mgp.GetSearchAreaNeighbors(GP, true);
					 int TotalSurroundingPoints=SurroundingPoints.Count();
					 int TotalBlockedCount=0;
					 List<CacheServerObject> NearbyObjects=ObjectCache.Obstacles.Values.OfType<CacheServerObject>()
						  .Where(obj => ObstacleType.Navigation.HasFlag(obj.Obstacletype.Value)&&GridPoint.GetDistanceBetweenPoints(GP, obj.PointPosition)*2.5f<=10f).ToList();

					 Dictionary<int, int> ObjectblockCounter=new Dictionary<int, int>();
					 NavigationBlockedPoints=new List<GridPoint>();

					 foreach (GridPoint item in mgp.GetSearchAreaNeighbors(GP, true))
					 {
						  if (!mgp.CanStandAt(item))
						  {
								TotalBlockedCount++;
								NavigationBlockedPoints.Add(item);
						  }
						  else if (NearbyObjects.Any())
						  {

								//Monsters can only occupy two surround points (if diagonal to the center) and any other object can occupy upto 4, so we ignore any objects that go beyond this logic.
								var NonIgnoredObjs=NearbyObjects.Where(obj => obj.GPRect.Contains(item)&&(!ObjectblockCounter.ContainsKey(obj.RAGUID)||ObjectblockCounter[obj.RAGUID]<2||obj.Obstacletype.Value!=ObstacleType.Monster&&ObjectblockCounter[obj.RAGUID]<4));

								if (NonIgnoredObjs!=null&&NonIgnoredObjs.Any())
								{
									 NonIgnoredObjs=NonIgnoredObjs.OrderBy(obj => GridPoint.GetDistanceBetweenPoints(item, obj.PointPosition));
									 //Logging.WriteVerbose("Found {0} potential objects and total contained {1}", NearbyObjects.Count, NonIgnoredObjs.Count());

									 var ThisObjBlocking=NonIgnoredObjs.First();
									 if (ObjectblockCounter.ContainsKey(ThisObjBlocking.RAGUID))
										  ObjectblockCounter[ThisObjBlocking.RAGUID]++;
									 else
										  ObjectblockCounter.Add(ThisObjBlocking.RAGUID, 1);

									 TotalBlockedCount++;
									 NavigationBlockedPoints.Add(item);
								}
								else
								{
									 //Logging.WriteVerbose("Found {0} potential objects for nav surrounding test, all failed", NearbyObjects.Count);
								}
						  }
						  else
						  {
								//Validate that this point is navigable by checking Z difference and raycasting
								Vector3 itemV3=(Vector3)item;
								if (Difference(GPV3.Z, itemV3.Z)>1f)
								{
									 if (!GilesCanRayCast(GPV3, itemV3, Zeta.Internals.SNO.NavCellFlags.AllowWalk))
									 {
										  TotalBlockedCount++;
										  NavigationBlockedPoints.Add(item);
									 }
								}

						  }
					 }
					 if (SettingsFunky.LogSafeMovementOutput)
						  Logging.WriteVerbose("Current Location Point has surrounding points {0} out of {1} blocked", TotalBlockedCount, TotalSurroundingPoints);
					 return (TotalSurroundingPoints==TotalBlockedCount);
				}

				//These vars are used here internally to search and track our actions.
				private static Vector3 LastSearchVector=vNullLocation;
				private static List<DirectionPoint> DirectionPoints=new List<DirectionPoint>();
				private static GPRectangle CurrentLocationGPRect=null;
				private static bool UpdatedLocalMovementTree=false;
				internal static List<GPRectangle> CacheSafeGPR=new List<GPRectangle>();
				private static GPRectangle LastUsedRect=null;
				private static bool AllGPRectsFailed=false;

				internal static bool AllDirectionsFailed=false;
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
				///
				///</summary>
				public static bool AttemptFindSafeSpot(out Vector3 safespot, Vector3 LOS, bool kiting=false)
				{
					 if (!Bot.Combat.TravellingAvoidance&&DateTime.Now.Subtract(lastFoundSafeSpot).TotalMilliseconds<=600
						&&vlastSafeSpot!=vNullLocation
						&&(!ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(vlastSafeSpot))
						  &&(!kiting||!ObjectCache.Objects.IsPointNearbyMonsters(vlastSafeSpot, Bot.Class.KiteDistance)))
					 {	 //Already found a safe spot in the last 800ms
						  safespot=vlastSafeSpot;
						  return true;
					 }

					 //Check if we should refresh..
					 if (LastSearchVector==vNullLocation||LastSearchVector.Distance(Bot.Character.Position)>MinimumChangeofDistanceBeforeRefresh)
						  Refresh(kiting);
					 else if (AllGPRectsFailed&&BlacklistedGridpoints.Count>0)
					 {
						  //Reset all blacklist to retry again.
						  AllGPRectsFailed=false;
						  //Clear Blacklisted
						  BlacklistedGridpoints.Clear();
					 }

					 //Set default safespot to null.
					 safespot=vNullLocation;

					 //Check Bot Navigationally blocked
					 UpdateBotNavBlocked();
					 if (BotIsNavigationallyBlocked)
						  return false;

					 Vector3 BotPosition=Bot.Character.Position;

					 //Check if we need to recreate
					 if (!UpdatedLocalMovementTree)
						  CreateNewSurroundingGPRs();

					 #region IterationOfGPCs
					 //Check if we actually created any surrounding GPCs..
					 if (cacheMovementGPRs.Count>0)
					 {
						  foreach (var item in cacheMovementGPRs)
						  {
								if (item.Weight>CurrentLocationWeight)
								{
									 item.UpdateObjectCount();
									 continue;
								}

								if (item.TryFindSafeSpot(out safespot, LOS, kiting))
								{//Found a spot..
									 LastUsedRect=item;
									 break;
								}
						  }
					 }


					 //Try to use the routine movement GPCs if we have any!
					 if (LastUsedRect==null&&CacheSafeGPR.Count>0)
					 {
						  for (int i=CacheSafeGPR.Count-1; i>0; i--)
						  {
								//if (BlacklistedSafespots.Contains(CachedMovementGPCs[i].CreationVector)) continue;

								if (CacheSafeGPR[i].CreationVector.Distance(BotPosition)>75f) continue;

								if (CacheSafeGPR[i].Weight>CurrentLocationWeight)
								{
									 CacheSafeGPR[i].UpdateObjectCount();
									 continue;
								}
								if (CacheSafeGPR[i].TryFindSafeSpot(out safespot, LOS, kiting))
								{
									 LastUsedRect=CacheSafeGPR[i];
									 break;
								}
						  }
					 }

					 if (LastUsedRect==null)
						  CurrentLocationGPRect.TryFindSafeSpot(out safespot, LOS, kiting);


					 //If still failed to find a safe spot.. set the timer before we try again.
					 if (safespot==vNullLocation)
					 {
						  if (SettingsFunky.LogSafeMovementOutput)
								Logging.WriteVerbose("All GPCs failed to find a valid location to move!");

						  AllGPRectsFailed=true;
						  CurrentLocationGPRect.UpdateObjectCount();
						  CurrentLocationWeight=CurrentLocationGPRect.Weight;

						  if (SettingsFunky.LogSafeMovementOutput)
								Logging.WriteVerbose("Current Location GPC Weight is {0}", CurrentLocationWeight);

						  MinimumChangeofDistanceBeforeRefresh-=5f;
						  if (MinimumChangeofDistanceBeforeRefresh<0f)
						  {
								MinimumChangeofDistanceBeforeRefresh=25f;
						  }


						  if (!kiting)
						  {
								//Set timer here until next we try... since we've already attempted at least 9 GPCs!
								Bot.Combat.iMillisecondsCancelledEmergencyMoveFor=(int)(Bot.Character.dCurrentHealthPct*SettingsFunky.AvoidanceRecheckMinimumRate)+1000;
								Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
						  }
						  else
						  {
								Bot.Combat.iMillisecondsCancelledKiteMoveFor=(int)(Bot.Character.dCurrentHealthPct*SettingsFunky.KitingRecheckMinimumRate)+1000;
								Bot.Combat.timeCancelledKiteMove=DateTime.Now;
						  }

						  vlastSafeSpot=vNullLocation;
						  return false;
					 }
					 else
						  LastUsedRect=CurrentLocationGPRect;
					 #endregion

					 vlastSafeSpot=safespot;
					 lastFoundSafeSpot=DateTime.Now;
					 //Logging.WriteVerbose("Safespot location {0} distance from {1} is {2}", safespot.ToString(), LastSearchVector.ToString(), safespot.Distance2D(LastSearchVector));
					 return (safespot!=vNullLocation);
				}

				private static DateTime LastClearedSearchLocationRAGUIDsCollection=DateTime.Today;
				private static List<int> RecentSearchLocationUsedTargetRAGUIDs=new List<int>();

				internal static void CheckClearedSearchRefresh()
				{
					 if (RecentSearchLocationUsedTargetRAGUIDs.Count>0&&DateTime.Now.Subtract(LastClearedSearchLocationRAGUIDsCollection).TotalMilliseconds>1000)
					 {
						  RecentSearchLocationUsedTargetRAGUIDs.Clear();
						  LastClearedSearchLocationRAGUIDsCollection=DateTime.Now;
					 }
				}

				public static bool AttemptFindTargetSafeLocation(out Vector3 safespot, CacheObject Target, bool CheckAvoidance=true, bool CheckKiting=true)
				{
					 //Set default safespot to null.
					 safespot=vNullLocation;

					 //Check if this target has already searched recently.
					 if (!RecentSearchLocationUsedTargetRAGUIDs.Contains(Target.RAGUID))
						  RecentSearchLocationUsedTargetRAGUIDs.Add(Target.RAGUID);
					 else
						  return false;

					 //Check if we should refresh..
					 if (LastSearchVector==vNullLocation||LastSearchVector.Distance(Bot.Character.Position)>MinimumChangeofDistanceBeforeRefresh)
						  Refresh(CheckKiting);
					 else if (AllGPRectsFailed&&BlacklistedGridpoints.Count>0)
					 {
						  //Reset all blacklist to retry again.
						  AllGPRectsFailed=false;
						  //Clear Blacklisted
						  BlacklistedGridpoints.Clear();
					 }



					 //Check Bot Navigationally blocked
					 UpdateBotNavBlocked();
					 if (BotIsNavigationallyBlocked)
						  return false;

					 //Check if we need to recreate
					 if (!UpdatedLocalMovementTree)
						  CreateNewSurroundingGPRs();


					 Logging.WriteVerbose("Searching for a valid location!");


					 #region IterationOfGPCs
					 //Check if we actually created any surrounding GPCs..
					 if (cacheMovementGPRs.Count>0)
					 {
						  foreach (var item in cacheMovementGPRs)
						  {
								if (item.TryFindSafeSpot(out safespot, Target.BotMeleeVector, CheckKiting, CheckAvoidance))
								{//Found a spot..
									 LastUsedRect=item;
									 break;
								}
						  }
					 }


					 //Try to use the routine movement GPCs if we have any!
					 if (LastUsedRect==null&&CacheSafeGPR.Count>0)
					 {
						  for (int i=CacheSafeGPR.Count-1; i>0; i--)
						  {
								//if (BlacklistedSafespots.Contains(CachedMovementGPCs[i].CreationVector)) continue;

								if (CacheSafeGPR[i].CreationVector.Distance(Bot.Character.Position)>75f) continue;

								if (CacheSafeGPR[i].TryFindSafeSpot(out safespot, Target.BotMeleeVector, CheckKiting, CheckAvoidance))
								{
									 LastUsedRect=CacheSafeGPR[i];
									 break;
								}
						  }
					 }

					 if (LastUsedRect==null)
						  if (CurrentLocationGPRect.TryFindSafeSpot(out safespot, Target.BotMeleeVector, CheckKiting, CheckAvoidance))
								LastUsedRect=CurrentLocationGPRect;




					 //If still failed to find a safe spot.. set the timer before we try again.
					 if (safespot==vNullLocation)
					 {
						  if (SettingsFunky.LogSafeMovementOutput)
								Logging.WriteVerbose("All GPCs failed to find a valid location to move!");

						  AllGPRectsFailed=true;
						  CurrentLocationGPRect.UpdateObjectCount();
						  CurrentLocationWeight=CurrentLocationGPRect.Weight;

						  if (SettingsFunky.LogSafeMovementOutput)
								Logging.WriteVerbose("Current Location GPC Weight is {0}", CurrentLocationWeight);


						  vlastSafeSpot=vNullLocation;
						  return false;
					 }
					 #endregion

					 vlastSafeSpot=safespot;
					 lastFoundSafeSpot=DateTime.Now;
					 //Logging.WriteVerbose("Safespot location {0} distance from {1} is {2}", safespot.ToString(), LastSearchVector.ToString(), safespot.Distance2D(LastSearchVector));
					 return (safespot!=vNullLocation);
				}

				private static void UpdateBotNavBlocked()
				{
					 //Check if bot is navigationally blocked..
					 if (DateTime.Now.Subtract(LastNavigationBlockCheck).TotalMilliseconds>600)
					 {
						  LastNavigationBlockCheck=DateTime.Now;

						  if (IsPointNavigationallyBlocked(Bot.Character.PointPosition))
						  {
								Logging.WriteVerbose("Bots Current Position is navigationally blocked!");
								BotIsNavigationallyBlocked=true;
						  }
						  else
								BotIsNavigationallyBlocked=false;
					 }
				}

				private static void Refresh(bool kiting)
				{
					 //Recreation of our current position and directions(maximum range)
					 MinimumChangeofDistanceBeforeRefresh=25f;
					 LastSearchVector=Bot.Character.Position;
					 LastUsedRect=null;
					 AllGPRectsFailed=false;
					 LastNavigationBlockCheck=DateTime.Today;
					 float MaximumRangeAllowed=(kiting?75f:100f);

					 //Clear Blacklisted
					 BlacklistedGridpoints.Clear();
					 DirectionPoints.Clear();

					 //Create a 4x4 rect to start our direction search
					 //Create a new GPC under the bot!
					 CurrentLocationGPRect=new GPRectangle(LastSearchVector, 3);
					 

					 //Update our base weight which we compare others with to see if its a better placement.
					 CurrentLocationWeight=CurrentLocationGPRect.Weight;
					 if (SettingsFunky.LogSafeMovementOutput)
						  Logging.WriteVerbose("Current Location GPC Weight is {0}", CurrentLocationWeight);


					 //Get upto 15 points for "Point of View" directions, we create rectangles with them.
					 var SearchPoints=CurrentLocationGPRect.Points.Keys.Where(gp => !gp.Equals(CurrentLocationGPRect.centerpoint));


					 //we should check our surrounding points to see if we can even move into any of them first!
					 foreach (var item in SearchPoints)
					 {
						  //We only want points we can stand at.. since we will travel inside one of the 8 surrounding points!
						  if (!item.Ignored&&!NavigationBlockedPoints.Contains(item))
						  {
								//Vector2 thisV2=mgp.GridToWorld(item);
								Vector3 thisV3=(Vector3)item;

								//Its a valid point for direction testing!
								float DirectionDegrees=FindDirection(LastSearchVector, thisV3, false);
								DirectionPoint P=new DirectionPoint(LastSearchVector, DirectionDegrees, MaximumRangeAllowed);
								DirectionPoints.Add(P);
						  }
					 }

					 if (SettingsFunky.LogSafeMovementOutput)
						  Logging.WriteVerbose("Total Direction Points Successfully Created {0}", DirectionPoints.Count);

					 //Check if we swapped into our "Routine" cache GPCs..
					 if (UpdatedLocalMovementTree)
					 {
						  UpdatedLocalMovementTree=false;

						  if (CacheSafeGPR.Count>0)
						  {
								//Swap back to our orginal GPCs
								cacheMovementGPRs.Clear();
								cacheMovementGPRs.AddRange(CacheSafeGPR.ToArray());
								CacheSafeGPR.Clear();
						  }
					 }

					 //Update avoidance objects so they are sorted by distance from bot..
					 ObjectCache.Obstacles.SortAvoidanceZones();
					 if (SettingsFunky.LogSafeMovementOutput)
						  Logging.WriteDiagnostic("Finished Creating Direction Points and Local GPC");
				}

				private static void CreateNewSurroundingGPRs(bool WeightSorted=true)
				{
					 List<GPRectangle> newMovementGPCs=new List<GPRectangle>();

					 //Update to see if all directions will be ignored..
					 AllDirectionsFailed=!DirectionPoints.Any(dp => dp.Range>5f);

					 float maxrangeFound=0f;
					 //Create GPCs based on DirectionPoints that have range > 5f
					 foreach (var direction in DirectionPoints)
					 {
						  //Skip directions that have very low range possible.
						  if (direction.Range<2.5f) continue;

						  if (maxrangeFound<direction.Range)
								maxrangeFound=direction.Range;

						  GPRectangle newEntry=new GPRectangle(direction);
						  newMovementGPCs.Add(newEntry);
					 }
					 //Cache our movement GPCs to switch back after we are done.
					 CacheSafeGPR=new List<GPRectangle>();
					 CacheSafeGPR.AddRange(cacheMovementGPRs.ToArray());


					 GridPoint botPoint=Bot.Character.PointPosition;
					 if (WeightSorted)
						  cacheMovementGPRs=new List<GPRectangle>(newMovementGPCs.OrderBy(gpc => gpc.Weight).ToArray());
					 else
						  cacheMovementGPRs=new List<GPRectangle>(newMovementGPCs.OrderByDescending(gpc => gpc.centerpoint.Distance(botPoint)).ToArray());


					 //update refresh range
					 MinimumChangeofDistanceBeforeRefresh=maxrangeFound;



					 UpdatedLocalMovementTree=true;
					 if (SettingsFunky.LogSafeMovementOutput)
						  Logging.WriteDiagnostic("Updated Local GPCs");

				}
		  }
	 }
}