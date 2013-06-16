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

				private static DateTime LastNavigationBlockCheck=DateTime.Today;

				///<summary>
				///Bots Position is blocked from adjacent movement -- Updated whenever searching for a safe location!
				///</summary>
				internal static bool BotIsNavigationallyBlocked=false;

				///<summary>
				///Current Grid Points that are Blocked.
				///</summary>
				private static List<GridPoint> NavigationBlockedPoints=new List<GridPoint>();

				//Tracks Objects that occupy surrounding navigation grid points
				private static Dictionary<int, int> ObjectblockCounter=new Dictionary<int, int>();
				private static Dictionary<int, GridPoint[]> ObjectOccupiedGridPoints=new Dictionary<int, GridPoint[]>();


				private static void RefreshNavigationBlocked()
				{
					 //Check if bot is navigationally blocked..
					 if (DateTime.Now.Subtract(LastNavigationBlockCheck).TotalMilliseconds>500)
					 {
						  LastNavigationBlockCheck=DateTime.Now;

						  if (UpdateNavigationalBlocking())
						  {
								Logging.WriteVerbose("Bots Current Position is navigationally blocked!");
								BotIsNavigationallyBlocked=true;
						  }
						  else
								BotIsNavigationallyBlocked=false;
					 }
				}
				///<summary>
				///Checks if the Bots current position is total blocked from adjacent movement
				///</summary>
				private static bool UpdateNavigationalBlocking()
				{
					 //Reset Navigationally Blocked GPs
					 NavigationBlockedPoints=new List<GridPoint>();
					 if (CurrentLocationGPRect.Count==0)
					 {
						  Logging.WriteDiagnostic("Current Location GP Rect has no valid Grid Points!");
						  return false;
					 }

					 GridPoint[] CurrentLocationGridPoints=CurrentLocationGPRect.Keys.ToArray();
					 List<GridPoint> SurroundingPoints=new List<GridPoint>();
					 int SurroundingMaxCount=CurrentLocationGPRect.Count>=8?8:CurrentLocationGPRect.Count;
					 for (int i=0; i<SurroundingMaxCount; i++)
					 {
						  GridPoint gp=CurrentLocationGridPoints[i];
						  if (!gp.Ignored)
								SurroundingPoints.Add(gp);
						  else
								NavigationBlockedPoints.Add(gp);
					 }

					 List<int> NearbyObjectRAGUIDs=new List<int>();
					 List<CacheServerObject> NearbyObjects=Bot.Combat.NearbyObstacleObjects.Where(obj => obj.RadiusDistance<=5f).ToList();//ObjectCache.Obstacles.Navigations.Where(obj => obj.RadiusDistance<=5f).ToList();

					 //no nearby objects passed distance check..
					 if (NearbyObjects.Count==0)
					 {
						  //Clear list, and return pure navigational check (Zero means we are completely stuck in a non-navigable location?)
						  ObjectblockCounter.Clear();
						  ObjectOccupiedGridPoints.Clear();

						  if (Bot.SettingsFunky.LogSafeMovementOutput)
								Logging.WriteVerbose("Current Location Point has {0} usable points (NoNewObjs)", SurroundingPoints.Count);

						  return (SurroundingPoints.Count==0);
					 }

					 //Update ObjectBlockCounter Collection
					 if (ObjectblockCounter.Count>0)
					 {
						  //Add current nearby object RAGUIDs to collection
						  NearbyObjectRAGUIDs.AddRange((from objs in NearbyObjects
																  select objs.RAGUID).ToArray());

						  //Generate Removal List for ObjectBlockCounter Collections
						  List<int> RemovalRAGUIDList=(from raguids in ObjectblockCounter.Keys
																 where !NearbyObjectRAGUIDs.Contains(raguids)
																 select raguids).ToList();

						  //Removal
						  foreach (var item in RemovalRAGUIDList)
						  {
								ObjectblockCounter.Remove(item);
								ObjectOccupiedGridPoints.Remove(item);
						  }
					 }

					 //Check any exisiting block entries
					 if (ObjectblockCounter.Count>0)
					 {
						  foreach (var item in ObjectOccupiedGridPoints.Values)
						  {
								NavigationBlockedPoints.AddRange(item);
						  }

						  //Update Surrounding Points
						  SurroundingPoints=SurroundingPoints.Except(NavigationBlockedPoints).ToList();

						  if (SurroundingPoints.Count==0)
						  {
								if (Bot.SettingsFunky.LogSafeMovementOutput)
									 Logging.WriteVerbose("NavBlocked -- No available surrounding points.");

								return true;
						  }
					 }

					 //Generate new object list that contains objects that are not already accounted for
					 List<CacheServerObject> NewObjects=NearbyObjects.Where(obj => !ObjectblockCounter.ContainsKey(obj.RAGUID)||ObjectblockCounter[obj.RAGUID]<4).ToList();

					 //No new objects to test..
					 if (NewObjects.Count==0)
					 {
						  if (Bot.SettingsFunky.LogSafeMovementOutput)
								Logging.WriteVerbose("No new Objects Unaccounted");

						  return (SurroundingPoints.Count==0);
					 }


					 foreach (GridPoint item in SurroundingPoints)
					 {
						  //Find any objects that contain this GP
						  CacheServerObject[] ContainedObjs=NewObjects.Where(Obj => Obj.PointInside(item)			   //only objects that have hit there maximum block count.
																						  &&(!ObjectblockCounter.ContainsKey(Obj.RAGUID)||Math.Round(Obj.PointRadius)<ObjectblockCounter[Obj.RAGUID])).ToArray();
						  if (ContainedObjs.Length>0)
						  {
								if (ContainedObjs.Length>1&&Bot.SettingsFunky.LogSafeMovementOutput)
									 Logging.WriteVerbose("Multiple Objects Found Occuping Grid Point!");

								CacheServerObject ThisObjBlocking=ContainedObjs[0];
								int ObjRAGUID=ThisObjBlocking.RAGUID;

								if (ObjectblockCounter.ContainsKey(ObjRAGUID))
								{
									 int GPCount=ObjectOccupiedGridPoints[ObjRAGUID].Length;
									 ObjectblockCounter[ObjRAGUID]++;
									 GridPoint[] newArrayGPs=new GridPoint[GPCount];
									 ObjectOccupiedGridPoints[ObjRAGUID].CopyTo(newArrayGPs, 0);
									 newArrayGPs[GPCount-1]=item.Clone();
									 ObjectOccupiedGridPoints[ObjRAGUID]=newArrayGPs;
								}
								else
								{
									 ObjectblockCounter.Add(ObjRAGUID, 1);
									 GridPoint[] NewArrayGP=new GridPoint[1] { item.Clone() };
									 ObjectOccupiedGridPoints.Add(ObjRAGUID, NewArrayGP);
								}

								NavigationBlockedPoints.Add(item);
						  }
					 }

					 //Update Surrounding Points
					 SurroundingPoints=SurroundingPoints.Except(NavigationBlockedPoints).ToList();

					 if (Bot.SettingsFunky.LogSafeMovementOutput)
						  Logging.WriteVerbose("Current Location Point has {0} usable points", SurroundingPoints.Count);


					 return (SurroundingPoints.Count==0);
				}

				//These vars are used here internally to search and track our actions.

				///<summary>
				///Location of the Bot when we searched for a safe location.
				///</summary>
				private static Vector3 LastSearchVector=vNullLocation;


				private static List<DirectionPoint> DirectionPoints=new List<DirectionPoint>();
				private static GPRectangle CurrentLocationGPRect=null;
				private static bool UpdatedLocalMovementTree=false;
				internal static List<GPRectangle> CacheSafeGPR=new List<GPRectangle>();

				///<summary>
				///Last successful GP Rectangle used during search method.
				///</summary>
				private static GPRectangle LastUsedRect=null;

				private static bool AllGPRectsFailed=false;
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
					 if (!Bot.Combat.TravellingAvoidance&&DateTime.Now.Subtract(lastFoundSafeSpot).TotalMilliseconds<=600
						&&vlastSafeSpot!=vNullLocation
						&&(!ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(vlastSafeSpot))
						  &&(!kiting||!ObjectCache.Objects.IsPointNearbyMonsters(vlastSafeSpot,Bot.KiteDistance)))
					 {	 //Already found a safe spot in the last 800ms
						  safespot=vlastSafeSpot;
						  return true;
					 }

					 //Check if we should refresh..
					 if (LastSearchVector==vNullLocation||!CurrentLocationGPRect.centerpoint.Equals(Bot.Character.PointPosition))
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
					 RefreshNavigationBlocked();
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
						  if (Bot.SettingsFunky.LogSafeMovementOutput)
								Logging.WriteVerbose("All GPCs failed to find a valid location to move!");

						  AllGPRectsFailed=true;
						  CurrentLocationGPRect.UpdateObjectCount();
						  CurrentLocationWeight=CurrentLocationGPRect.Weight;

						  if (Bot.SettingsFunky.LogSafeMovementOutput)
								Logging.WriteVerbose("Current Location GPC Weight is {0}", CurrentLocationWeight);

						  MinimumChangeofDistanceBeforeRefresh-=5f;
						  if (MinimumChangeofDistanceBeforeRefresh<0f)
						  {
								MinimumChangeofDistanceBeforeRefresh=25f;
						  }


						  if (!kiting)
						  {
								//Set timer here until next we try... since we've already attempted at least 9 GPCs!
								Bot.Combat.iMillisecondsCancelledEmergencyMoveFor=(int)(Bot.Character.dCurrentHealthPct*Bot.SettingsFunky.AvoidanceRecheckMinimumRate)+1000;
								Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
						  }
						  else
						  {
								Bot.Combat.iMillisecondsCancelledKiteMoveFor=(int)(Bot.Character.dCurrentHealthPct*Bot.SettingsFunky.KitingRecheckMinimumRate)+1000;
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
					 RefreshNavigationBlocked();
					 if (BotIsNavigationallyBlocked)
						  return false;

					 //Check if we need to recreate
					 if (!UpdatedLocalMovementTree)
						  CreateNewSurroundingGPRs();


					 Logging.WriteVerbose("Searching for a valid location for target {0}", Target.InternalName);

					 //Use Target GPRect First!
					 if (Target.GPRect.TryFindSafeSpot(out safespot, Bot.Character.Position, false, false))
					 {
						  LastUsedRect=Target.GPRect;
					 }
					 else
					 {
						  //Check if we actually created any surrounding GPCs..
						  if (cacheMovementGPRs.Count>0)
						  {
								foreach (var item in cacheMovementGPRs)
								{
									 if (item.TryFindSafeSpot(out safespot, Target.BotMeleeVector, CheckKiting, CheckAvoidance))
									 {//Found a spot, but we need to validate its connected with our current Position!
										  if (GilesCanRayCast(Bot.Character.Position, safespot, Zeta.Internals.SNO.NavCellFlags.AllowWalk))
										  {
												LastUsedRect=item;
												break;
										  }
									 }
								}
						  }
					 }

					 #region IterationOfGPCs

					 //Try to use the routine movement GPCs if we have any!
					 if (LastUsedRect==null&&CacheSafeGPR.Count>0)
					 {
						  for (int i=CacheSafeGPR.Count-1; i>0; i--)
						  {
								if (CacheSafeGPR[i].TryFindSafeSpot(out safespot, Target.BotMeleeVector, CheckKiting, CheckAvoidance))
								{
									 if (GilesCanRayCast(Bot.Character.Position, safespot, Zeta.Internals.SNO.NavCellFlags.AllowWalk))
									 {
										  LastUsedRect=CacheSafeGPR[i];
										  break;
									 }
								}
						  }
					 }


					 //If still failed to find a safe spot.. set the timer before we try again.
					 if (safespot==vNullLocation)
					 {
						  if (Bot.SettingsFunky.LogSafeMovementOutput)
								Logging.WriteVerbose("All GPCs failed to find a valid location to move!");

						  AllGPRectsFailed=true;
						  vlastSafeSpot=vNullLocation;
						  return false;
					 }
					 #endregion

					 vlastSafeSpot=safespot;
					 lastFoundSafeSpot=DateTime.Now;
					 //Logging.WriteVerbose("Safespot location {0} distance from {1} is {2}", safespot.ToString(), LastSearchVector.ToString(), safespot.Distance2D(LastSearchVector));
					 return (safespot!=vNullLocation);
				}



				///<summary>
				///Resets direction vars and recreates local GPR (benethe the bot) and direction points
				///</summary>
				private static void Refresh(bool kiting)
				{
					 //Recreation of our current position and directions(maximum range)
					 MinimumChangeofDistanceBeforeRefresh=25f;
					 LastSearchVector=Bot.Character.Position;
					 LastUsedRect=null;
					 AllGPRectsFailed=false;
					 float MaximumRangeAllowed=(kiting?125f:100f);

					 //Clear Blacklisted
					 BlacklistedGridpoints.Clear();
					 DirectionPoints.Clear();

					 //Create simple 3x3 to start with (This represents the 8 navigational points that the bot could take)
					 LastNavigationBlockCheck=DateTime.Today;
					 CurrentLocationGPRect=new GPRectangle(LastSearchVector);
					 RefreshNavigationBlocked();

					 //Now we expand our local GPC so we can get 24 direction points
					 CurrentLocationGPRect.FullyExpand();

					 //Update our base weight which we compare others with to see if its a better placement.
					 CurrentLocationWeight=CurrentLocationGPRect.Weight;
					 if (Bot.SettingsFunky.LogSafeMovementOutput)
						  Logging.WriteVerbose("Current Location GPC Weight is {0}", CurrentLocationWeight);


					 //Get upto 15 points for "Point of View" directions, we create rectangles with them.
					 var SearchPoints=CurrentLocationGPRect.Points.Keys.Where(gp => !gp.Equals(CurrentLocationGPRect.centerpoint));


					 //we should check our surrounding points to see if we can even move into any of them first!
					 foreach (var item in SearchPoints)
					 {
						  //We only want points we can stand at.. since we will travel inside one of the 8 surrounding points!
						  if (!NavigationBlockedPoints.Contains(item))
						  {
								//Vector2 thisV2=mgp.GridToWorld(item);
								Vector3 thisV3=(Vector3)item;
								thisV3.Z+=(Bot.Character.fCharacterRadius/2f);

								//Its a valid point for direction testing!
								float DirectionDegrees=FindDirection(LastSearchVector, thisV3, false);
								DirectionPoint P=new DirectionPoint(LastSearchVector, DirectionDegrees, MaximumRangeAllowed);
								DirectionPoints.Add(P);
						  }
					 }

					 if (Bot.SettingsFunky.LogSafeMovementOutput)
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
					 if (Bot.SettingsFunky.LogSafeMovementOutput)
						  Logging.WriteDiagnostic("Finished Creating Direction Points and Local GPC");
				}

				private static void CreateNewSurroundingGPRs(bool WeightSorted=true)
				{
					 List<GPRectangle> newMovementGPCs=new List<GPRectangle>();

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
					 if (Bot.SettingsFunky.LogSafeMovementOutput) Logging.WriteDiagnostic("Updated Local GPCs");
				}
		  }
	 }
}