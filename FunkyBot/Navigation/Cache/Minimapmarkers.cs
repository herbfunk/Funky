using System;
using System.Collections.Generic;
using System.Linq;
using FunkyBot.Cache;
using FunkyBot.Movement;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Action = Zeta.TreeSharp.Action;

namespace FunkyBot.XMLTags
{
	/// <summary>
	/// Class to help track MiniMapMarkers during Dungeon Exploration
	/// </summary>
	public class MiniMapMarker : IEquatable<MiniMapMarker>
	{
		private const int WAYPOINT_MARKER = -1751517829;

		internal static HashSet<int> TownHubMarkers = new HashSet<int>()
        {
            1877684886 // A5 Hub
        };

		public int MarkerNameHash { get; set; }
		public Vector3 Position { get; set; }
		public bool Visited { get; set; }
		public bool Failed { get; set; }
		public MiniMapMarker() { }
		internal static List<MiniMapMarker> KnownMarkers = new List<MiniMapMarker>();

		internal static MoveResult lastMoveResult = MoveResult.Moved;

		internal static bool AnyUnvisitedMarkers()
		{
			return MiniMapMarker.KnownMarkers.Any(m => !m.Visited && !m.Failed);
		}

		internal static void SetNearbyMarkersVisited(Vector3 near, float pathPrecision)
		{
			MiniMapMarker nearestMarker = GetNearestUnvisitedMarker(near);
			if (nearestMarker != null)
			{
				foreach (MiniMapMarker marker in KnownMarkers.Where(m => m.Equals(nearestMarker) && near.Distance2D(m.Position) <= pathPrecision))
				{
					Logger.DBLog.DebugFormat("Setting MiniMapMarker {0} as Visited, within PathPrecision {1:0}", marker.MarkerNameHash, pathPrecision);
					marker.Visited = true;
					lastMoveResult = MoveResult.Moved;
				}

				// Navigator will return "ReacheDestination" when it can't fully move to the specified position
				if (lastMoveResult == MoveResult.ReachedDestination)
				{
					foreach (MiniMapMarker marker in KnownMarkers.Where(m => m.Equals(nearestMarker)))
					{
						Logger.DBLog.DebugFormat("Setting MiniMapMarker {0} as Visited, MoveResult=ReachedDestination", marker.MarkerNameHash);
						marker.Visited = true;
						lastMoveResult = MoveResult.Moved;
					}
				}

				if (lastMoveResult == MoveResult.PathGenerationFailed)
				{
					foreach (MiniMapMarker marker in KnownMarkers.Where(m => m.Equals(nearestMarker)))
					{
						Logger.DBLog.DebugFormat("Unable to navigate to marker, setting MiniMapMarker {0} at {1} as failed", marker.MarkerNameHash, marker.Position);
						marker.Failed = true;
						lastMoveResult = MoveResult.Moved;
					}
				}
			}

		}

		internal static MiniMapMarker GetNearestUnvisitedMarker(Vector3 near)
		{
			return KnownMarkers.OrderBy(m => m.MarkerNameHash != 0).ThenBy(m => Vector3.Distance(near, m.Position)).FirstOrDefault(m => !m.Visited && !m.Failed);
		}

		private static DefaultNavigationProvider NavProvider;

		internal static void UpdateFailedMarkers()
		{
			if (NavProvider == null)
				NavProvider = new DefaultNavigationProvider();

			foreach (MiniMapMarker marker in KnownMarkers.Where(m => m.Failed))
			{
				if (NavProvider.CanPathWithinDistance(marker.Position, 10f))
				{
					Logger.DBLog.DebugFormat("Was able to generate full path to failed MiniMapMarker {0} at {1}, marking as good", marker.MarkerNameHash, marker.Position);
					marker.Failed = false;
					lastMoveResult = MoveResult.PathGenerated;
				}
			}
		}

		internal static void AddMarkersToList(int includeMarker = 0)
		{
			foreach (Zeta.Game.Internals.MinimapMarker marker in GetMarkerList(includeMarker))
			{
				MiniMapMarker mmm = new MiniMapMarker()
				{
					MarkerNameHash = marker.NameHash,
					Position = marker.Position,
					Visited = false
				};

				Logger.DBLog.DebugFormat("Adding MiniMapMarker {0} at {1} to KnownMarkers", mmm.MarkerNameHash, mmm.Position);

				KnownMarkers.Add(mmm);
			}
		}

		private static Composite CreateAddRiftMarkers()
		{
			return
			new DecoratorContinue(ret => ZetaDia.CurrentAct == Act.OpenWorld && CacheIDLookup.riftWorldIds.Contains(Bot.Character.Data.iCurrentWorldID),
				new Action(ret =>
				{
					foreach (var nameHash in CacheIDLookup.riftPortalHashes)
					{
						AddMarkersToList(nameHash);
					}

					foreach (var marker in ZetaDia.Minimap.Markers.CurrentWorldMarkers.Where(m => (m.IsPortalExit || m.IsPointOfInterest) && !TownHubMarkers.Contains(m.NameHash)))
					{
						AddMarkersToList(marker.NameHash);
					}
				})
			);
		}

		internal static void AddMarkersToList(List<TrinityExploreDungeon.Objective> objectives)
		{
			if (objectives == null)
				return;

			foreach (var objective in objectives.Where(o => o.MarkerNameHash != 0))
			{
				if (ZetaDia.Minimap.Markers.CurrentWorldMarkers.Any(m => m.NameHash == objective.MarkerNameHash))
				{
					AddMarkersToList(objective.MarkerNameHash);
				}
			}
		}

		private static IEnumerable<Zeta.Game.Internals.MinimapMarker> GetMarkerList(int includeMarker)
		{
			return ZetaDia.Minimap.Markers.CurrentWorldMarkers
				.Where(m => (m.NameHash == 0 || m.NameHash == includeMarker || m.IsPointOfInterest || m.IsPortalExit) &&
					!KnownMarkers.Any(ml => ml.Position == m.Position && ml.MarkerNameHash == m.NameHash))
					.OrderBy(m => m.NameHash != 0);
		}

		internal static Composite DetectMiniMapMarkers(int includeMarker = 0)
		{
			return
			new Sequence(
				CreateAddRiftMarkers(),
				new DecoratorContinue(ret => ZetaDia.Minimap.Markers.CurrentWorldMarkers
					.Any(m => (m.NameHash == 0 || m.NameHash == includeMarker) && !KnownMarkers.Any(m2 => m2.Position != m.Position && m2.MarkerNameHash == m.NameHash)),
					new Sequence(
					new Action(ret => MiniMapMarker.AddMarkersToList(includeMarker))
					)
				)
			);
		}

		internal static Composite DetectMiniMapMarkers(List<TrinityExploreDungeon.Objective> objectives)
		{
			return
			new Sequence(
				new Action(ret => MiniMapMarker.AddMarkersToList(objectives))
			);
		}

		internal static Composite VisitMiniMapMarkers(Vector3 near, float markerDistance)
		{
			return
			new Decorator(ret => MiniMapMarker.AnyUnvisitedMarkers(),
				new Sequence(
					new Action(ret => MiniMapMarker.SetNearbyMarkersVisited(Bot.Character.Data.Position, markerDistance)),
					new Decorator(ret => MiniMapMarker.GetNearestUnvisitedMarker(Bot.Character.Data.Position) != null,
						new Action(ret => MoveToNearestMarker(near))
					)
				)
			);
		}

		internal static RunStatus MoveToNearestMarker(Vector3 near)
		{
			MiniMapMarker m = MiniMapMarker.GetNearestUnvisitedMarker(near);
			SkipAheadCache.RecordSkipAheadCachePoint();

			lastMoveResult = Navigator.MoveTo(MiniMapMarker.GetNearestUnvisitedMarker(near).Position);

			Logger.DBLog.DebugFormat("Moving to inspect nameHash {0} at {1} distance {2:0} mr: {3}",
				m.MarkerNameHash, m.Position, Bot.Character.Data.Position.Distance2D(m.Position), lastMoveResult);


			return RunStatus.Success;
		}


		public bool Equals(MiniMapMarker other)
		{
			return other.Position == this.Position && other.MarkerNameHash == this.MarkerNameHash;
		}
	}
}