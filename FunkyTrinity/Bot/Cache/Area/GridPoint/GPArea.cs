using System;
using System.Collections.Generic;
using System.Linq;
using Zeta;
using Zeta.Common;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  //Redesign how we use the GPCache by creating it into a instance which contains the entrie area we conduct a search in.

		  internal static partial class GridPointAreaCache
		  {
				internal static GPArea CurrentGPAREA=null;
				
				internal class GPArea
				{
					 //ToDo: Track object IDs so we can add them to the appropriate GPRect to use for updating.

					 //GPArea -- a collection of GPRects which are connected, describes the entire location, holds each point indexed for easier access
					 private List<GPRectangle> gridpointrectangles_;
					 internal bool AllGPRectsFailed=false;
					 internal GPRectangle centerGPRect;

					 public GPArea(Vector3 startingLocation)
					 {
						  //Creation and Cache base
						  centerGPRect=new GPRectangle(startingLocation, 3);
						  //Get all valid points (besides current point) from our current location GPR
						  GridPoint[] SearchPoints=centerGPRect.Points.Keys.Where(gp => !gp.Ignored).ToArray();
						  gridpointrectangles_=new List<GPRectangle>();
						  if (SearchPoints.Length>1)
						  {								//we should check our surrounding points to see if we can even move into any of them first!
								for (int i=1; i<SearchPoints.Length-1; i++)
								{
									 GridPoint curGP=SearchPoints[i];
									 Vector3 thisV3=(Vector3)curGP;
									 thisV3.Z+=(Bot.Character.fCharacterRadius/2f);

									 //Its a valid point for direction testing!
									 float DirectionDegrees=FindDirection(LastSearchVector, thisV3, false);
									 DirectionPoint P=new DirectionPoint((Vector3)curGP, DirectionDegrees, 125f);

									 if (P.Range>5f)
									 {
										  gridpointrectangles_.Add(new GPRectangle(P, centerGPRect));
									 }
								}
						  }
					 }

					 public bool GridPointContained(GridPoint point)
					 {
						  return centerGPRect.Contains(point);
					 }

					 ///<summary>
					 ///Searches for a safespot!
					 ///</summary>
					 public Vector3 AttemptFindSafeSpot(Vector3 CurrentPosition, Vector3 LOS, bool kiting=false)
					 {
						  if (AllGPRectsFailed&&BlacklistedGridpoints.Count>0)
						  {
								//Reset all blacklist to retry again.
								AllGPRectsFailed=false;
								//Clear Blacklisted
								BlacklistedGridpoints.Clear();
						  }

						  Vector3 safespot=vNullLocation;
						  //Check if we actually created any surrounding GPCs..
						  if (gridpointrectangles_.Count>0)
						  {
								iterateGPRectsSafeSpot(out safespot, LOS, kiting);
								//If still failed to find a safe spot.. set the timer before we try again.
								if (safespot==vNullLocation)
								{
									 if (Bot.SettingsFunky.LogSafeMovementOutput) 
										  Logging.WriteVerbose("All GPCs failed to find a valid location to move!");

									 AllGPRectsFailed=true;

									 //Set timer here
									 if (!kiting)
									 {
										  Bot.Combat.iMillisecondsCancelledEmergencyMoveFor=(int)(Bot.Character.dCurrentHealthPct*Bot.SettingsFunky.AvoidanceRecheckMinimumRate)+1000;
										  Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
									 }
									 else
									 {
										  Bot.Combat.iMillisecondsCancelledKiteMoveFor=(int)(Bot.Character.dCurrentHealthPct*Bot.SettingsFunky.KitingRecheckMinimumRate)+1000;
										  Bot.Combat.timeCancelledKiteMove=DateTime.Now;
									 }
									 return safespot;
								}
								else
								{
									 //Cache it and set timer
									 lastFoundSafeSpot=DateTime.Now;
									 vlastSafeSpot=safespot;
								}
						  }
						  //Logging.WriteVerbose("Safespot location {0} distance from {1} is {2}", safespot.ToString(), LastSearchVector.ToString(), safespot.Distance2D(LastSearchVector));
						  return vlastSafeSpot;
					 }

					 private int lastGPRectIndexUsed=0;
					 private void iterateGPRectsSafeSpot(out Vector3 safespot, Vector3 LOS, bool kiting=false)
					 {
						  safespot=vNullLocation;
						  for (int i=lastGPRectIndexUsed; i<gridpointrectangles_.Count-1; i++)
						  {
								GPRectangle item=gridpointrectangles_[i];
								item.UpdateObjectCount(AllGPRectsFailed);
								if (item.Weight>CurrentLocationGPRect.Weight) continue;
								
								if (item.TryFindSafeSpot(out safespot, LOS, kiting, false, AllGPRectsFailed))  return;									
						  }
						  lastGPRectIndexUsed=0;
					 }
				}

				private static GPRectangle CurrentLocationGPRect=null;
				///<summary>
				///Current Grid Points that are Blocked.
				///</summary>
				private static List<GridPoint> LastNavigationBlockedPoints=new List<GridPoint>();
				//Tracks Objects that occupy surrounding navigation grid points
				private static Dictionary<int, int> LastObjectblockCounter=new Dictionary<int, int>();
				private static Dictionary<int, GridPoint[]> LastObjectOccupiedGridPoints=new Dictionary<int, GridPoint[]>();
				///<summary>
				///Checks if the position is total blocked from adjacent movements either by objects or non navigation
				///</summary>
				private static bool IsVectorBlocked(Vector3 location)
				{
					 //Reset Navigationally Blocked GPs
					 LastNavigationBlockedPoints=new List<GridPoint>();

					 //Create Local GPRect!
					 if (CurrentLocationGPRect==null||CurrentLocationGPRect.centerpoint!=(GridPoint)location)
					 {
						  //Clear lists
						  LastObjectblockCounter.Clear();
						  LastObjectOccupiedGridPoints.Clear();
						  CurrentLocationGPRect=new GPRectangle(location);
					 }

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
								LastNavigationBlockedPoints.Add(gp);
					 }

					 List<int> NearbyObjectRAGUIDs=new List<int>();
					 List<CacheServerObject> NearbyObjects=Bot.Combat.NearbyObstacleObjects.Where(obj => obj.RadiusDistance<=5f).ToList();//ObjectCache.Obstacles.Navigations.Where(obj => obj.RadiusDistance<=5f).ToList();

					 //no nearby objects passed distance check..
					 if (NearbyObjects.Count==0)
					 {
						  //Clear list, and return pure navigational check (Zero means we are completely stuck in a non-navigable location?)
						  LastObjectblockCounter.Clear();
						  LastObjectOccupiedGridPoints.Clear();

						  if (Bot.SettingsFunky.LogSafeMovementOutput)
								Logging.WriteVerbose("Current Location Point has {0} usable points (NoNewObjs)", SurroundingPoints.Count);

						  return (SurroundingPoints.Count==0);
					 }

					 //Update ObjectBlockCounter Collection
					 if (LastObjectblockCounter.Count>0)
					 {
						  //Add current nearby object RAGUIDs to collection
						  NearbyObjectRAGUIDs.AddRange((from objs in NearbyObjects
																  select objs.RAGUID).ToArray());

						  //Generate Removal List for ObjectBlockCounter Collections
						  List<int> RemovalRAGUIDList=(from raguids in LastObjectblockCounter.Keys
																 where !NearbyObjectRAGUIDs.Contains(raguids)
																 select raguids).ToList();

						  //Removal
						  foreach (var item in RemovalRAGUIDList)
						  {
								LastObjectblockCounter.Remove(item);
								LastObjectOccupiedGridPoints.Remove(item);
						  }
					 }

					 //Check any exisiting block entries
					 if (LastObjectblockCounter.Count>0)
					 {
						  foreach (var item in LastObjectOccupiedGridPoints.Values)
						  {
								LastNavigationBlockedPoints.AddRange(item);
						  }

						  //Update Surrounding Points
						  SurroundingPoints=SurroundingPoints.Except(LastNavigationBlockedPoints).ToList();

						  if (SurroundingPoints.Count==0)
						  {
								if (Bot.SettingsFunky.LogSafeMovementOutput)
									 Logging.WriteVerbose("NavBlocked -- No available surrounding points.");

								return true;
						  }
					 }

					 //Generate new object list that contains objects that are not already accounted for
					 List<CacheServerObject> NewObjects=NearbyObjects.Where(obj => !LastObjectblockCounter.ContainsKey(obj.RAGUID)||LastObjectblockCounter[obj.RAGUID]<4).ToList();

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
																						  &&(!LastObjectblockCounter.ContainsKey(Obj.RAGUID)||Math.Round(Obj.PointRadius)<LastObjectblockCounter[Obj.RAGUID])).ToArray();
						  if (ContainedObjs.Length>0)
						  {
								if (ContainedObjs.Length>1&&Bot.SettingsFunky.LogSafeMovementOutput)
									 Logging.WriteVerbose("Multiple Objects Found Occuping Grid Point!");

								CacheServerObject ThisObjBlocking=ContainedObjs[0];
								int ObjRAGUID=ThisObjBlocking.RAGUID;

								if (LastObjectblockCounter.ContainsKey(ObjRAGUID))
								{
									 int GPCount=LastObjectOccupiedGridPoints[ObjRAGUID].Length;
									 LastObjectblockCounter[ObjRAGUID]++;
									 GridPoint[] newArrayGPs=new GridPoint[GPCount];
									 LastObjectOccupiedGridPoints[ObjRAGUID].CopyTo(newArrayGPs, 0);
									 newArrayGPs[GPCount-1]=item.Clone();
									 LastObjectOccupiedGridPoints[ObjRAGUID]=newArrayGPs;
								}
								else
								{
									 LastObjectblockCounter.Add(ObjRAGUID, 1);
									 GridPoint[] NewArrayGP=new GridPoint[1] { item.Clone() };
									 LastObjectOccupiedGridPoints.Add(ObjRAGUID, NewArrayGP);
								}

								LastNavigationBlockedPoints.Add(item);
						  }
					 }

					 //Update Surrounding Points
					 SurroundingPoints=SurroundingPoints.Except(LastNavigationBlockedPoints).ToList();

					 if (Bot.SettingsFunky.LogSafeMovementOutput)
						  Logging.WriteVerbose("Current Location Point has {0} usable points", SurroundingPoints.Count);


					 return (SurroundingPoints.Count==0);
				}
		  }
    }
}