using System;
using System.Collections.Generic;
using System.Linq;
using FunkyBot.Movement;
using Zeta;
using Zeta.Common;
using Zeta.Internals;
using Zeta.Navigation;
using Zeta.TreeSharp;
using Action=Zeta.TreeSharp.Action;

namespace FunkyBot.XMLTags
{
	 /// <summary>
	 /// Class to help track MiniMapMarkers during Dungeon Exploration
	 /// </summary>
	 public class MiniMapMarker : IEquatable<MiniMapMarker>
	 {
		  public int MarkerNameHash { get; set; }
		  public Vector3 Position { get; set; }
		  public bool Visited { get; set; }
		  public bool Failed { get; set; }
		  public MiniMapMarker() { }
		  public static List<MiniMapMarker> KnownMarkers=new List<MiniMapMarker>();

		  public static MoveResult lastMoveResult=MoveResult.Moved;

		  public static bool AnyUnvisitedMarkers()
		  {
				return KnownMarkers.Any(m => !m.Visited&&!m.Failed);
		  }

		  public static void SetNearbyMarkersVisited(Vector3 near, float pathPrecision)
		  {
				MiniMapMarker nearestMarker=GetNearestUnvisitedMarker(near);
				if (nearestMarker!=null)
				{
					 foreach (MiniMapMarker marker in KnownMarkers.Where(m => m.Equals(nearestMarker)&&near.Distance2D(m.Position)<=pathPrecision))
					 {
						  //DbHelper.Logging.Write(TrinityLogLevel.Normal, LogCategory.ProfileTag, "Setting MiniMapMarker {0} as Visited, within PathPrecision {1:0}", marker.MarkerNameHash, pathPrecision);
						  marker.Visited=true;
						  lastMoveResult=MoveResult.Moved;
					 }

					 // Navigator will return "ReacheDestination" when it can't fully move to the specified position
					 if (lastMoveResult==MoveResult.ReachedDestination)
					 {
						  foreach (MiniMapMarker marker in KnownMarkers.Where(m => m.Equals(nearestMarker)))
						  {
								//DbHelper.Logging.Write(TrinityLogLevel.Normal, LogCategory.ProfileTag, "Setting MiniMapMarker {0} as Visited, MoveResult=ReachedDestination", marker.MarkerNameHash);
								marker.Visited=true;
								lastMoveResult=MoveResult.Moved;
						  }
					 }

					 if (lastMoveResult==MoveResult.PathGenerationFailed)
					 {
						  foreach (MiniMapMarker marker in KnownMarkers.Where(m => m.Equals(nearestMarker)))
						  {
								//DbHelper.Logging.Write(TrinityLogLevel.Normal, LogCategory.ProfileTag, "Unable to navigate to marker, setting MiniMapMarker {0} at {1} as failed", marker.MarkerNameHash, marker.Position);
								marker.Failed=true;
								lastMoveResult=MoveResult.Moved;
						  }
					 }
				}

		  }

		  public static MiniMapMarker GetNearestUnvisitedMarker(Vector3 near)
		  {
				return KnownMarkers.OrderBy(m => m.MarkerNameHash!=0).ThenBy(m => Vector3.Distance(near, m.Position)).FirstOrDefault(m => !m.Visited&&!m.Failed);
		  }

		  private static DefaultNavigationProvider NavProvider;

		  public static void UpdateFailedMarkers()
		  {
				if (NavProvider==null)
					 NavProvider=new DefaultNavigationProvider();

				foreach (MiniMapMarker marker in KnownMarkers.Where(m => m.Failed))
				{
					 if (NavProvider.CanPathWithinDistance(marker.Position, 10f))
					 {
						  //DbHelper.LogNormal("Was able to generate full path to failed MiniMapMarker {0} at {1}, marking as good", marker.MarkerNameHash, marker.Position);
						  marker.Failed=false;
						  lastMoveResult=MoveResult.PathGenerated;
					 }
				}
		  }

		  internal static void AddMarkersToList(int includeMarker=0)
		  {
				foreach (MinimapMarker marker in GetMarkerList(includeMarker))
				{
					 MiniMapMarker mmm=new MiniMapMarker()
					 {
						  MarkerNameHash=marker.NameHash,
						  Position=marker.Position,
						  Visited=false
					 };

					 //DbHelper.Logging.Write(TrinityLogLevel.Normal, LogCategory.ProfileTag, "Adding MiniMapMarker {0} at {1} to KnownMarkers", mmm.MarkerNameHash, mmm.Position);

					 KnownMarkers.Add(mmm);
				}
		  }

		  private static IEnumerable<MinimapMarker> GetMarkerList(int includeMarker)
		  {
				return ZetaDia.Minimap.Markers.CurrentWorldMarkers.Where(m => (m.NameHash==0||m.NameHash==includeMarker)&&!KnownMarkers.Any(ml => ml.Position==m.Position&&ml.MarkerNameHash==m.NameHash)).OrderBy(m => m.NameHash!=0);
		  }

		  public static DecoratorContinue DetectMiniMapMarkers(int includeMarker=0)
		  {
				return
				new DecoratorContinue(ret => ZetaDia.Minimap.Markers.CurrentWorldMarkers.Any(m => (m.NameHash==0||m.NameHash==includeMarker)&&!KnownMarkers.Any(m2 => m2.Position!=m.Position&&m2.MarkerNameHash==m.NameHash)),
					 new Sequence(
						  new Action(ret => AddMarkersToList(includeMarker))
					 )
				);
		  }

		  public static Decorator VisitMiniMapMarkers(Vector3 near, float markerDistance)
		  {
				return
				new Decorator(ret => AnyUnvisitedMarkers(),
					 new Sequence(
						  new Action(ret => SetNearbyMarkersVisited(ZetaDia.Me.Position, markerDistance)),
						  new Decorator(ret => GetNearestUnvisitedMarker(ZetaDia.Me.Position)!=null,
								new Action(ret => MoveToNearestMarker(near))
						  )
					 )
				);
		  }

		  public static RunStatus MoveToNearestMarker(Vector3 near)
		  {
				if (SkipAheadCache.bSkipAheadAGo)
					 SkipAheadCache.RecordSkipAheadCachePoint();

			
				lastMoveResult=Navigator.MoveTo(GetNearestUnvisitedMarker(near).Position);

				//DbHelper.Logging.Write(TrinityLogLevel.Normal, LogCategory.ProfileTag, "Moving to inspect nameHash {0} at {1} distance {2:0} mr: {3}",
					 //m.MarkerNameHash, m.Position, ZetaDia.Me.Position.Distance2D(m.Position), lastMoveResult);


				return RunStatus.Success;
		  }


		  public bool Equals(MiniMapMarker other)
		  {
				return other.Position==Position&&other.MarkerNameHash==MarkerNameHash;
		  }
	 }
}