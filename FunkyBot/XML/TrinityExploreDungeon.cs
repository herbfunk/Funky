using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FunkyBot.Cache;
using FunkyBot.Cache.Objects;
using FunkyBot.Movement;
using Zeta.Bot;
using Zeta.Bot.Dungeons;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Bot.Pathfinding;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;

namespace FunkyBot.XMLTags
{
	/// <summary>
	/// TrinityExploreDungeon is fuly backwards compatible with the built-in Demonbuddy ExploreArea tag. It provides additional features such as:
	/// Moving to investigate MiniMapMarker pings and the current ExitNameHash if provided and visible (mini map marker 0 and the current exitNameHash)
	/// Moving to investigate Priority Scenes if provided (PrioritizeScenes subtags)
	/// Ignoring DungeonExplorer nodes in certain scenes if provided (IgnoreScenes subtags)
	/// Reduced backtracking (via pathPrecision attribute and combat skip ahead cache)
	/// Multiple ActorId's for the ObjectFound end type (AlternateActors sub-tags)
	/// </summary>
	[XmlElement("TrinityExploreDungeon")]
	public class TrinityExploreDungeon : ProfileBehavior
	{
		/// <summary>
		/// The SNOId of the Actor that we're looking for, used with until="ObjectFound"
		/// </summary>
		[XmlAttribute("actorId", true)]
		public int ActorId { get; set; }

		/// <summary>
		/// Sets a custom grid segmentation Box Size (default 15)
		/// </summary>
		[XmlAttribute("boxSize", true)]
		public int BoxSize { get; set; }

		/// <summary>
		/// Sets a custom grid segmentation Box Tolerance (default 0.55)
		/// </summary>
		[XmlAttribute("boxTolerance", true)]
		public float BoxTolerance { get; set; }

		/// <summary>
		/// The nameHash of the exit the bot will move to and finish the tag when found
		/// </summary>
		[XmlAttribute("exitNameHash", true)]
		public int ExitNameHash { get; set; }

		[XmlAttribute("ignoreGridReset", true)]
		public bool IgnoreGridReset { get; set; }

		/// <summary>
		/// Not currently implimented
		/// </summary>
		[XmlAttribute("leaveWhenFinished", true)]
		public bool LeaveWhenExplored { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[XmlAttribute("ignoreExploredAreas", true)]
		public bool IgnoreExploredAreas { get; set; }

		/// <summary>
		/// The distance the bot must be from an actor before marking the tag as complete, when used with until="ObjectFound"
		/// </summary>
		[XmlAttribute("objectDistance", true)]
		public float ObjectDistance { get; set; }

		/// <summary>
		/// The until="" atribute must match one of these
		/// </summary>
		public enum TrinityExploreEndType
		{
			FullyExplored = 0,
			ObjectFound,
			ExitFound,
			SceneFound
		}

		[XmlAttribute("endType", true)]
		[XmlAttribute("until", true)]
		public TrinityExploreEndType EndType { get; set; }

		/// <summary>
		/// The list of Scene SNOId's or Scene Names that the bot will ignore dungeon nodes in
		/// </summary>
		[XmlElement("IgnoreScenes")]
		public List<IgnoreScene> IgnoreScenes { get; set; }

		/// <summary>
		/// The list of Scene SNOId's or Scene Names that the bot will prioritize (only works when the scene is "loaded")
		/// </summary>
		[XmlElement("PriorityScenes")]
		[XmlElement("PrioritizeScenes")]
		public List<PrioritizeScene> PriorityScenes { get; set; }

		/// <summary>
		/// The Ignore Scene class, used as IgnoreScenes child elements
		/// </summary>
		[XmlElement("IgnoreScene")]
		public class IgnoreScene : IEquatable<Scene>
		{
			[XmlAttribute("sceneName")]
			public string SceneName { get; set; }
			[XmlAttribute("sceneId")]
			public int SceneId { get; set; }

			public IgnoreScene()
			{
				SceneId = -1;
				SceneName = String.Empty;
			}

			public IgnoreScene(string name)
			{
				this.SceneName = name;
			}
			public IgnoreScene(int id)
			{
				this.SceneId = id;
			}

			public bool Equals(Scene other)
			{
				return (!string.IsNullOrWhiteSpace(SceneName) && other.Name.ToLowerInvariant().Contains(SceneName.ToLowerInvariant())) || other.SceneInfo.SNOId == SceneId;
			}
		}

		private CachedValue<List<Area>> m_IgnoredAreas;
		private List<Area> IgnoredAreas
		{
			get
			{
				if (m_IgnoredAreas == null)
					m_IgnoredAreas = new CachedValue<List<Area>>(() => { return GetIgnoredAreas(); }, TimeSpan.FromSeconds(1));
				return m_IgnoredAreas.Value;
			}
		}

		private List<Area> GetIgnoredAreas()
		{
			var ignoredScenes = ZetaDia.Scenes.GetScenes()
				.Where(scn => scn.IsValid && IgnoreScenes.Any(igns => igns.Equals(scn)) && !PriorityScenes.Any(psc => psc.Equals(scn)))
				.Select(scn =>
					scn.Mesh.Zone == null
					? new Area(new Vector2(float.MinValue, float.MinValue), new Vector2(float.MaxValue, float.MaxValue))
					: new Area(scn.Mesh.Zone.ZoneMin, scn.Mesh.Zone.ZoneMax))
					.ToList();
			Logger.DBLog.DebugFormat("Returning {0} ignored areas", ignoredScenes.Count());
			return ignoredScenes;
		}

		private class Area
		{
			public Vector2 Min { get; set; }
			public Vector2 Max { get; set; }

			/// <summary>
			/// Initializes a new instance of the Area class.
			/// </summary>
			public Area(Vector2 min, Vector2 max)
			{
				Min = min;
				Max = max;
			}

			public bool IsPositionInside(Vector2 position)
			{
				return position.X >= Min.X && position.X <= Max.X && position.Y >= Min.Y && position.Y <= Max.Y;
			}

			public bool IsPositionInside(Vector3 position)
			{
				return IsPositionInside(position.ToVector2());
			}
		}

		/// <summary>
		/// The Priority Scene class, used as PrioritizeScenes child elements
		/// </summary>
		[XmlElement("PriorityScene")]
		[XmlElement("PrioritizeScene")]
		public class PrioritizeScene : IEquatable<Scene>
		{
			[XmlAttribute("sceneName")]
			public string SceneName { get; set; }
			[XmlAttribute("sceneId")]
			public int SceneId { get; set; }
			[XmlAttribute("pathPrecision")]
			public float PathPrecision { get; set; }

			public PrioritizeScene()
			{
				PathPrecision = 15f;
				SceneName = String.Empty;
				SceneId = -1;
			}

			public PrioritizeScene(string name)
			{
				this.SceneName = name;
			}
			public PrioritizeScene(int id)
			{
				this.SceneId = id;
			}
			public bool Equals(Scene other)
			{
				return (SceneName != String.Empty && other.Name.ToLowerInvariant().Contains(SceneName.ToLowerInvariant())) || other.SceneInfo.SNOId == SceneId;
			}
		}

		[XmlElement("AlternateActors")]
		public List<AlternateActor> AlternateActors { get; set; }

		[XmlElement("AlternateActor")]
		public class AlternateActor
		{
			[XmlAttribute("actorId")]
			public int ActorId { get; set; }
			[XmlAttribute("objectDistance")]
			public float ObjectDistance { get; set; }

			public AlternateActor()
			{
				ActorId = -1;
				ObjectDistance = 60f;
			}
		}

		/// <summary>
		/// The Scene SNOId, used with ExploreUntil="SceneFound"
		/// </summary>
		[XmlAttribute("sceneId")]
		public int SceneId { get; set; }

		/// <summary>
		/// The Scene Name, used with ExploreUntil="SceneFound", a sub-string match will work
		/// </summary>
		[XmlAttribute("sceneName")]
		public string SceneName { get; set; }

		/// <summary>
		/// The distance the bot will mark dungeon nodes as "visited" (default is 1/2 of box size, minimum 10)
		/// </summary>
		[XmlAttribute("pathPrecision")]
		public float PathPrecision { get; set; }

		/// <summary>
		/// The distance before reaching a MiniMapMarker before marking it as visited
		/// </summary>
		[XmlAttribute("markerDistance")]
		public float MarkerDistance { get; set; }

		/// <summary>
		/// Disable Mini Map Marker Scouting
		/// </summary>
		[XmlAttribute("ignoreMarkers")]
		public bool IgnoreMarkers { get; set; }

		public enum TimeoutType
		{
			Timer,
			GoldInactivity,
			None,
		}

		/// <summary>
		/// The TimeoutType to use (default None, no timeout)
		/// </summary>
		[XmlAttribute("timeoutType")]
		public TimeoutType ExploreTimeoutType { get; set; }

		/// <summary>
		/// Value in Seconds. 
		/// The timeout value to use, when used with Timer will force-end the tag after a certain time. When used with GoldInactivity will end the tag after coinages doesn't change for the given period
		/// </summary>
		[XmlAttribute("timeoutValue")]
		public int TimeoutValue { get; set; }

		/// <summary>
		/// If we want to use a townportal before ending the tag when a timeout happens
		/// </summary>
		[XmlAttribute("townPortalOnTimeout")]
		public bool TownPortalOnTimeout { get; set; }

		/// <summary>
		/// Ignore last N nodes of dungeon explorer, when using endType=FullyExplored
		/// </summary>
		[XmlAttribute("ignoreLastNodes")]
		public int IgnoreLastNodes { get; set; }

		/// <summary>
		/// Used with IgnoreLastNodes, minimum visited node count before tag can end. 
		/// The minVisistedNodes is purely, and only for use with ignoreLastNodes - it does not serve any other function like you expect. 
		/// The reason this attribute exists, is to prevent prematurely exiting the dungeon exploration when used with ignoreLastNodes. 
		/// For example, when the bot first starts exploring an area, it needs to navigate a few dungeon nodes first before other dungeon nodes even appear - otherwise with ignoreLastNodes > 2, 
		/// the bot would immediately exit from navigation without exploring anything at all.
		/// </summary>
		[XmlAttribute("minVisitedNodes")]
		public int MinVisistedNodes { get; set; }

		[XmlAttribute("SetNodesExploredAutomatically")]
		public bool SetNodesExploredAutomatically { get; set; }

		/// <summary>
		/// The Position of the CurrentNode NavigableCenter
		/// </summary>
		private Vector3 CurrentNavTarget
		{
			get
			{
				if (PrioritySceneTarget != Vector3.Zero)
				{
					return PrioritySceneTarget;
				}

				if (GetRouteUnvisitedNodeCount() > 0)
				{
					return BrainBehavior.DungeonExplorer.CurrentNode.NavigableCenter;
				}
				else
				{
					return Vector3.Zero;
				}
			}
		}

		// Adding these for SimpleFollow compatability
		public float X { get { return CurrentNavTarget.X; } }
		public float Y { get { return CurrentNavTarget.Y; } }
		public float Z { get { return CurrentNavTarget.Z; } }

		private bool InitDone = false;
		private DungeonNode NextNode;

		/// <summary>
		/// The current player position
		/// </summary>
		private Vector3 myPos { get { return Bot.Character.Data.Position; } }
		private static ISearchAreaProvider MainGridProvider
		{
			get
			{
				return Navigation.MGP;
			}
		}

		/// <summary>
		/// The last scene SNOId we entered
		/// </summary>
		private int mySceneId = -1;
		/// <summary>
		/// The last position we updated the ISearchGridProvider at
		/// </summary>
		private Vector3 GPUpdatePosition = Vector3.Zero;

		/// <summary>
		/// Called when the profile behavior starts
		/// </summary>
		public override void OnStart()
		{
			Logger.DBLog.DebugFormat("TrinityExploreDungeon OnStart() called");

			if (SetNodesExploredAutomatically)
			{
				Logger.DBLog.DebugFormat("Minimap Explored Nodes Enabled");
				BrainBehavior.DungeonExplorer.SetNodesExploredAutomatically = true;
			}
			else
			{
				Logger.DBLog.DebugFormat("Minimap Explored Nodes Disabled");
				BrainBehavior.DungeonExplorer.SetNodesExploredAutomatically = false;
			}

			UpdateSearchGridProvider();

			CheckResetDungeonExplorer();

			if (!InitDone)
			{
				Init();
			}
			TagTimer.Reset();
			timesForcedReset = 0;

			PrintNodeCounts("PostInit");

		}

		/// <summary>
		/// Re-sets the DungeonExplorer, BoxSize, BoxTolerance, and Updates the current route
		/// </summary>
		private void CheckResetDungeonExplorer()
		{
			if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || !ZetaDia.WorldInfo.IsValid || !ZetaDia.Scenes.IsValid || !ZetaDia.Service.IsValid)
				return;

			// I added this because GridSegmentation may (rarely) reset itself without us doing it to 15/.55.
			if ((BoxSize != 0 && BoxTolerance != 0) && (GridSegmentation.BoxSize != BoxSize || GridSegmentation.BoxTolerance != BoxTolerance) || (GetGridSegmentationNodeCount() == 0))
			{
				Logger.DBLog.DebugFormat("Box Size or Tolerance has been changed! {0}/{1}", GridSegmentation.BoxSize, GridSegmentation.BoxTolerance);

				BrainBehavior.DungeonExplorer.Reset();
				PrintNodeCounts("BrainBehavior.DungeonExplorer.Reset");

				GridSegmentation.BoxSize = BoxSize;
				GridSegmentation.BoxTolerance = BoxTolerance;
				PrintNodeCounts("SetBoxSize+Tolerance");

				BrainBehavior.DungeonExplorer.Update();
				PrintNodeCounts("BrainBehavior.DungeonExplorer.Update");
			}
		}

		/// <summary>
		/// The main profile behavior
		/// </summary>
		/// <returns></returns>
		protected override Composite CreateBehavior()
		{
			return
			new Sequence(
				new DecoratorContinue(ret => !IgnoreMarkers,
					MiniMapMarker.DetectMiniMapMarkers(ExitNameHash)
				),
				UpdateSearchGridProvider(),
				new Action(ret => CheckResetDungeonExplorer()),
				new PrioritySelector(
					CheckIsObjectiveFinished(),
					PrioritySceneCheck(),
					new Decorator(ret => !IgnoreMarkers,
						MiniMapMarker.VisitMiniMapMarkers(myPos, MarkerDistance)
					),
					new Decorator(ret => ShouldInvestigateActor(),
						InvestigatActor()
					),
					new Sequence(
						new DecoratorContinue(ret => DungeonRouteIsEmpty(),
							new Action(ret => UpdateRoute())
						),
						CheckIsExplorerFinished()
					),
					new DecoratorContinue(ret => DungeonRouteIsValid(),
						new PrioritySelector(
							CheckNodeFinished(),
							new Sequence(
								new Action(ret => PrintNodeCounts("MainBehavior")),
								new Action(ret => MoveToNextNode())
							)
						)
					),
					new Action(ret => Logger.DBLog.DebugFormat("Error 1: Unknown error occured!"))
				)
			);
		}

		private static bool DungeonRouteIsValid()
		{
			return BrainBehavior.DungeonExplorer != null && BrainBehavior.DungeonExplorer.CurrentRoute != null && BrainBehavior.DungeonExplorer.CurrentRoute.Any();
		}

		private static bool DungeonRouteIsEmpty()
		{
			return BrainBehavior.DungeonExplorer != null && BrainBehavior.DungeonExplorer.CurrentRoute != null && !BrainBehavior.DungeonExplorer.CurrentRoute.Any();
		}

		private Action InvestigatActor()
		{
			return new Action(ret =>
			{
				SkipAheadCache.RecordSkipAheadCachePoint(PathPrecision);

				var actor = ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false).FirstOrDefault(a => a.ActorSNO == ActorId);

				if (actor != null && actor.IsValid && actor.Position.Distance2D(myPos) >= ObjectDistance)
					Navigator.MoveTo(actor.Position);
			});
		}

		private bool ShouldInvestigateActor()
		{
			if (ActorId == 0)
				return false;

			if (ObjectDistance == 0)
				return false;

			var actors = ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false)
				.Where(a => !SkipAheadCache.CheckPositionForSkipping(a.Position));

			if (actors == null)
				return false;

			if (!actors.Any())
				return false;

			var actor = actors.OrderBy(a => a.Distance).FirstOrDefault();

			if (actor.Distance <= ObjectDistance)
				return false;

			return true;
		}


		/// <summary>
		/// Updates the search grid provider as needed
		/// </summary>
		/// <returns></returns>
		private Composite UpdateSearchGridProvider()
		{
			return
			new DecoratorContinue(ret => mySceneId != Bot.Character.Data.iSceneID || Vector3.Distance(myPos, GPUpdatePosition) > 150,
				new Sequence(
					new Action(ret => mySceneId = Bot.Character.Data.iSceneID),
					new Action(ret => Navigator.SearchGridProvider.Update()),
					new Action(ret => GPUpdatePosition = myPos),
					new Action(ret => MiniMapMarker.UpdateFailedMarkers())
				)
			);
		}

		/// <summary>
		/// Checks if we are using a timeout and will end the tag if the timer has breached the given value
		/// </summary>
		/// <returns></returns>
		private Composite TimeoutCheck()
		{
			return
			new PrioritySelector(
				new Decorator(ret => timeoutBreached,
					new Sequence(
						new DecoratorContinue(ret => TownPortalOnTimeout && !Bot.Character.Data.bIsInTown,
							new Sequence(
								new Action(ret => Logger.DBLog.DebugFormat(
									"TrinityExploreDungeon inactivity timer tripped ({0}), tag Using Town Portal!", TimeoutValue)),
								CommonBehaviors.CreateUseTownPortal(),
								new Action(ret => isDone = true)
							)
						),
						new DecoratorContinue(ret => !TownPortalOnTimeout,
							new Action(ret => isDone = true)
						)
					)
				),
				new Decorator(ret => ExploreTimeoutType == TimeoutType.Timer,
					new Action(ret => CheckSetTimer(ret))
				),
				new Decorator(ret => ExploreTimeoutType == TimeoutType.GoldInactivity,
					new Action(ret => CheckSetGoldInactive(ret))
				)
			);
		}

		bool timeoutBreached = false;
		Stopwatch TagTimer = new Stopwatch();
		/// <summary>
		/// Will start the timer if needed, and end the tag if the timer has exceeded the TimeoutValue
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		private RunStatus CheckSetTimer(object ctx)
		{
			if (!TagTimer.IsRunning)
			{
				TagTimer.Start();
				return RunStatus.Failure;
			}
			if (ExploreTimeoutType == TimeoutType.Timer && TagTimer.Elapsed.TotalSeconds > TimeoutValue)
			{
				Logger.DBLog.DebugFormat("TrinityExploreDungeon timer ended ({0}), tag finished!", TimeoutValue);
				timeoutBreached = true;
				return RunStatus.Success;
			}
			return RunStatus.Failure;
		}

		private int lastCoinage = -1;
		/// <summary>
		/// Will check if the bot has not picked up any gold within the allocated TimeoutValue
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		private RunStatus CheckSetGoldInactive(object ctx)
		{
			CheckSetTimer(ctx);
			if (lastCoinage == -1)
			{
				lastCoinage = Bot.Character.Data.Coinage;
				return RunStatus.Failure;
			}
			else if (lastCoinage != Bot.Character.Data.Coinage)
			{
				TagTimer.Restart();
				return RunStatus.Failure;
			}
			else if (lastCoinage == Bot.Character.Data.Coinage && TagTimer.Elapsed.TotalSeconds > TimeoutValue)
			{
				Logger.DBLog.DebugFormat("TrinityExploreDungeon gold inactivity timer tripped ({0}), tag finished!", TimeoutValue);
				timeoutBreached = true;
				return RunStatus.Success;
			}

			return RunStatus.Failure;
		}

		private int timesForcedReset = 0;
		private int timesForceResetMax = 5;

		/// <summary>
		/// Checks to see if the tag is finished as needed
		/// </summary>
		/// <returns></returns>
		private Composite CheckIsExplorerFinished()
		{
			return
			new PrioritySelector(
				CheckIsObjectiveFinished(),
				new Decorator(ret => GetRouteUnvisitedNodeCount() == 0 && timesForcedReset > timesForceResetMax,
					new Sequence(
						new Action(ret => Logger.DBLog.DebugFormat(
							"Visited all nodes but objective not complete, forced reset more than {0} times, finished!", timesForceResetMax)),
						new Action(ret => isDone = true)
					)
				),
				new Decorator(ret => GetRouteUnvisitedNodeCount() == 0,
					new Sequence(
						new Action(ret => Logger.DBLog.DebugFormat("Visited all nodes but objective not complete, forcing grid reset!")),
						new Action(ret => timesForcedReset++),
						new Action(ret => SkipAheadCache.ClearCache()),
						new Action(ret => MiniMapMarker.KnownMarkers.Clear()),
						new Action(ret => ForceUpdateScenes()),
						new Action(ret => GridSegmentation.Reset()),
						new Action(ret => GridSegmentation.Update()),
						new Action(ret => BrainBehavior.DungeonExplorer.Reset()),
						new Action(ret => PriorityScenesInvestigated.Clear()),
						new Action(ret => UpdateRoute())
					)
				)
		   );
		}

		private void ForceUpdateScenes()
		{
			foreach (Scene scene in ZetaDia.Scenes.GetScenes().ToList())
			{
				scene.UpdatePointer(scene.BaseAddress);
			}
		}

		/// <summary>
		/// Checks to see if the tag is finished as needed
		/// </summary>
		/// <returns></returns>
		private Composite CheckIsObjectiveFinished()
		{
			return
			new PrioritySelector(
				TimeoutCheck(),
				new Decorator(ret => EndType == TrinityExploreEndType.FullyExplored && IgnoreLastNodes > 0 && GetRouteUnvisitedNodeCount() <= IgnoreLastNodes && GetGridSegmentationVisistedNodeCount() >= MinVisistedNodes,
					new Sequence(
						new Action(ret => Logger.DBLog.DebugFormat("Fully explored area! Ignoring {0} nodes. Tag Finished.", IgnoreLastNodes)),
						new Action(ret => isDone = true)
					)
				),
				new Decorator(ret => EndType == TrinityExploreEndType.FullyExplored && GetRouteUnvisitedNodeCount() == 0,
					new Sequence(
						new Action(ret => Logger.DBLog.DebugFormat("Fully explored area! Tag Finished.", 0)),
						new Action(ret => isDone = true)
					)
				),
				new Decorator(ret => EndType == TrinityExploreEndType.ExitFound && ExitNameHash != 0 && IsExitNameHashVisible(),
					new Sequence(
						new Action(ret => Logger.DBLog.DebugFormat("Found exitNameHash {0}!", ExitNameHash)),
						new Action(ret => isDone = true)
					)
				),
				new Decorator(ret => EndType == TrinityExploreEndType.ObjectFound && ActorId != 0 && ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false)
					.Any(a => a.ActorSNO == ActorId && a.Distance <= ObjectDistance),
					new Sequence(
						new Action(ret => Logger.DBLog.DebugFormat("Found Object {0}!", ActorId)),
						new Action(ret => isDone = true)
					)
				),
				new Decorator(ret => EndType == TrinityExploreEndType.ObjectFound && AlternateActorsFound(),
					new Sequence(
						new Action(ret => Logger.DBLog.DebugFormat("Found Alternate Object {0}!", GetAlternateActor().ActorSNO)),
						new Action(ret => isDone = true)
					)
				),
				new Decorator(ret => EndType == TrinityExploreEndType.SceneFound && Bot.Character.Data.iSceneID == SceneId,
					new Sequence(
						new Action(ret => Logger.DBLog.DebugFormat("Found SceneId {0}!", SceneId)),
						new Action(ret => isDone = true)
					)
				),
				new Decorator(ret => EndType == TrinityExploreEndType.SceneFound && !string.IsNullOrWhiteSpace(SceneName) && ZetaDia.Me.CurrentScene.Name.ToLower().Contains(SceneName.ToLower()),
					new Sequence(
						new Action(ret => Logger.DBLog.DebugFormat("Found SceneName {0}!", SceneName)),
						new Action(ret => isDone = true)
					)
				),
				new Decorator(ret => Bot.Character.Data.bIsInTown,
					new Sequence(
						new Action(ret => Logger.DBLog.DebugFormat("Cannot use TrinityExploreDungeon in town - tag finished!", SceneName)),
						new Action(ret => isDone = true)
					)
				)
			);
		}

		private bool AlternateActorsFound()
		{
			return AlternateActors.Any() && ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false)
					.Where(o => AlternateActors.Any(a => a.ActorId == o.ActorSNO && o.Distance <= a.ObjectDistance)).Any();
		}

		private DiaObject GetAlternateActor()
		{
			return ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false)
					.Where(o => AlternateActors.Any(a => a.ActorId == o.ActorSNO && o.Distance <= a.ObjectDistance)).OrderBy(o => o.Distance).FirstOrDefault();
		}

		/// <summary>
		/// Determine if the tag ExitNameHash is visible in the list of Current World Markers
		/// </summary>
		/// <returns></returns>
		private bool IsExitNameHashVisible()
		{
			return ZetaDia.Minimap.Markers.CurrentWorldMarkers.Any(m => m.NameHash == ExitNameHash && m.Position.Distance2D(myPos) <= MarkerDistance + 10f);
		}

		private Vector3 PrioritySceneTarget = Vector3.Zero;
		private int PrioritySceneSNOId = -1;
		private Scene CurrentPriorityScene = null;
		private float PriorityScenePathPrecision = -1f;
		/// <summary>
		/// A list of Scene SNOId's that have already been investigated
		/// </summary>
		private List<int> PriorityScenesInvestigated = new List<int>();

		private DateTime lastCheckedScenes = DateTime.MinValue;
		/// <summary>
		/// Will find and move to Prioritized Scene's based on Scene SNOId or Name
		/// </summary>
		/// <returns></returns>
		private Composite PrioritySceneCheck()
		{
			return
			new Decorator(ret => PriorityScenes != null && PriorityScenes.Any(),
				new Sequence(
					new DecoratorContinue(ret => DateTime.Now.Subtract(lastCheckedScenes).TotalMilliseconds > 1000,
						new Sequence(
							new Action(ret => lastCheckedScenes = DateTime.Now),
							new Action(ret => FindPrioritySceneTarget())
						)
					),
					new Decorator(ret => PrioritySceneTarget != Vector3.Zero,
						new PrioritySelector(
							new Decorator(ret => PrioritySceneTarget.Distance2D(myPos) <= PriorityScenePathPrecision,
								new Sequence(
									new Action(ret => Logger.DBLog.DebugFormat("Successfully navigated to priority scene {0} {1} center {2} Distance {3:0}",
										CurrentPriorityScene.Name, CurrentPriorityScene.SceneInfo.SNOId, PrioritySceneTarget, PrioritySceneTarget.Distance2D(myPos))),
									new Action(ret => PrioritySceneMoveToFinished())
								)
							),
							new Action(ret => MoveToPriorityScene())
						)
					)
				)
			);
		}

		/// <summary>
		/// Handles actual movement to the Priority Scene
		/// </summary>
		private void MoveToPriorityScene()
		{
			Logger.DBLog.DebugFormat("Moving to Priority Scene {0} - {1} Center {2} Distance {3:0}",
				CurrentPriorityScene.Name, CurrentPriorityScene.SceneInfo.SNOId, PrioritySceneTarget, PrioritySceneTarget.Distance2D(myPos));

			MoveResult moveResult = Funky.PlayerMover.NavigateTo(PrioritySceneTarget);

			if (moveResult == MoveResult.PathGenerationFailed || moveResult == MoveResult.ReachedDestination)
			{
				Logger.DBLog.DebugFormat("Unable to navigate to Scene {0} - {1} Center {2} Distance {3:0}, cancelling!",
					CurrentPriorityScene.Name, CurrentPriorityScene.SceneInfo.SNOId, PrioritySceneTarget, PrioritySceneTarget.Distance2D(myPos));
				PrioritySceneMoveToFinished();
			}
		}

		/// <summary>
		/// Sets a priority scene as finished
		/// </summary>
		private void PrioritySceneMoveToFinished()
		{
			PriorityScenesInvestigated.Add(PrioritySceneSNOId);
			PrioritySceneSNOId = -1;
			PrioritySceneTarget = Vector3.Zero;
			UpdateRoute();
		}

		/// <summary>
		/// Finds a navigable point in a priority scene
		/// </summary>
		private void FindPrioritySceneTarget()
		{
			if (!PriorityScenes.Any())
				return;

			//gp.Update();

			if (PrioritySceneTarget != Vector3.Zero)
				return;

			bool foundPriorityScene = false;

			// find any matching priority scenes in scene manager - match by name or SNOId

			List<Scene> PScenes = ZetaDia.Scenes.GetScenes()
				.Where(s => PriorityScenes.Any(ps => ps.SceneId != -1 && s.SceneInfo.SNOId == ps.SceneId)).ToList();

			PScenes.AddRange(ZetaDia.Scenes.GetScenes()
				 .Where(s => PriorityScenes.Any(ps => ps.SceneName.Trim() != String.Empty && ps.SceneId == -1 && s.Name.ToLower().Contains(ps.SceneName.ToLower()))).ToList());

			List<Scene> foundPriorityScenes = new List<Scene>();
			Dictionary<int, Vector3> foundPrioritySceneIndex = new Dictionary<int, Vector3>();

			foreach (Scene scene in PScenes)
			{
				if (!scene.IsValid)
					continue;
				if (!scene.SceneInfo.IsValid)
					continue;
				if (!scene.Mesh.Zone.IsValid)
					continue;
				if (!scene.Mesh.Zone.NavZoneDef.IsValid)
					continue;

				if (PriorityScenesInvestigated.Contains(scene.SceneInfo.SNOId))
					continue;

				foundPriorityScene = true;

				NavZone navZone = scene.Mesh.Zone;
				NavZoneDef zoneDef = navZone.NavZoneDef;

				Vector2 zoneMin = navZone.ZoneMin;
				Vector2 zoneMax = navZone.ZoneMax;

				Vector3 zoneCenter = GetNavZoneCenter(navZone);

				List<NavCell> NavCells = zoneDef.NavCells.Where(c => c.IsValid && c.Flags.HasFlag(NavCellFlags.AllowWalk)).ToList();

				if (!NavCells.Any())
					continue;

				NavCell bestCell = NavCells.OrderBy(c => GetNavCellCenter(c.Min, c.Max, navZone).Distance2D(zoneCenter)).FirstOrDefault();

				if (bestCell != null && !foundPrioritySceneIndex.ContainsKey(scene.SceneInfo.SNOId))
				{
					foundPrioritySceneIndex.Add(scene.SceneInfo.SNOId, GetNavCellCenter(bestCell, navZone));
					foundPriorityScenes.Add(scene);
				}
				else
				{
					Logger.DBLog.DebugFormat("Found Priority Scene but could not find a navigable point!", true);
				}
			}

			if (foundPrioritySceneIndex.Any())
			{
				KeyValuePair<int, Vector3> nearestPriorityScene = foundPrioritySceneIndex.OrderBy(s => s.Value.Distance2D(myPos)).FirstOrDefault();

				PrioritySceneSNOId = nearestPriorityScene.Key;
				PrioritySceneTarget = nearestPriorityScene.Value;
				CurrentPriorityScene = foundPriorityScenes.FirstOrDefault(s => s.SceneInfo.SNOId == PrioritySceneSNOId);
				PriorityScenePathPrecision = GetPriorityScenePathPrecision(PScenes.FirstOrDefault(s => s.SceneInfo.SNOId == nearestPriorityScene.Key));

				Logger.DBLog.DebugFormat("Found Priority Scene {0} - {1} Center {2} Distance {3:0}",
					CurrentPriorityScene.Name, CurrentPriorityScene.SceneInfo.SNOId, PrioritySceneTarget, PrioritySceneTarget.Distance2D(myPos));
			}

			if (!foundPriorityScene)
			{
				PrioritySceneTarget = Vector3.Zero;
			}
		}

		private float GetPriorityScenePathPrecision(Scene scene)
		{
			return PriorityScenes.FirstOrDefault(ps => ps.SceneId != 0 && ps.SceneId == scene.SceneInfo.SNOId || scene.Name.ToLower().Contains(ps.SceneName.ToLower())).PathPrecision;
		}

		/// <summary>
		/// Gets the center of a given Navigation Zone
		/// </summary>
		/// <param name="zone"></param>
		/// <returns></returns>
		private Vector3 GetNavZoneCenter(NavZone zone)
		{
			float X = zone.ZoneMin.X + ((zone.ZoneMax.X - zone.ZoneMin.X) / 2);
			float Y = zone.ZoneMin.Y + ((zone.ZoneMax.Y - zone.ZoneMin.Y) / 2);

			return new Vector3(X, Y, 0);
		}

		/// <summary>
		/// Gets the center of a given Navigation Cell
		/// </summary>
		/// <param name="cell"></param>
		/// <param name="zone"></param>
		/// <returns></returns>
		private Vector3 GetNavCellCenter(NavCell cell, NavZone zone)
		{
			return GetNavCellCenter(cell.Min, cell.Max, zone);
		}

		/// <summary>
		/// Gets the center of a given box with min/max, adjusted for the Navigation Zone
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <param name="zone"></param>
		/// <returns></returns>
		private Vector3 GetNavCellCenter(Vector3 min, Vector3 max, NavZone zone)
		{
			float X = zone.ZoneMin.X + min.X + ((max.X - min.X) / 2);
			float Y = zone.ZoneMin.Y + min.Y + ((max.Y - min.Y) / 2);
			float Z = min.Z + ((max.Z - min.Z) / 2);

			return new Vector3(X, Y, Z);
		}

		/// <summary>
		/// Checks to see if the current DungeonExplorer node is in an Ignored scene, and marks the node immediately visited if so
		/// </summary>
		/// <returns></returns>
		private Composite CheckIgnoredScenes()
		{
			return
			new Decorator(ret => IgnoreScenes != null && IgnoreScenes.Any(),
				new PrioritySelector(
					new Decorator(ret => IsPositionInsideIgnoredScene(CurrentNavTarget),
						new Sequence(
							new Action(ret => SetNodeVisited("Node is in Ignored Scene"))
						)
					)
				)
			);
		}


		private bool IsPositionInsideIgnoredScene(Vector3 position)
		{
			return IgnoredAreas.Any(a => a.IsPositionInside(position));
		}

		/// <summary>
		/// Determines if a given Vector3 is in a provided IgnoreScene (if the scene is loaded)
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		private bool PositionInsideIgnoredScene(Vector3 position)
		{
			List<Scene> ignoredScenes = ZetaDia.Scenes.GetScenes()
				.Where(scn => scn.IsValid && (IgnoreScenes.Any(igscn => !string.IsNullOrWhiteSpace(igscn.SceneName) && scn.Name.ToLower().Contains(igscn.SceneName.ToLower())) ||
					IgnoreScenes.Any(igscn => scn.SceneInfo.SNOId == igscn.SceneId) &&
					!PriorityScenes.Any(psc => !string.IsNullOrWhiteSpace(psc.SceneName) && scn.Name.ToLower().Contains(psc.SceneName)) &&
					!PriorityScenes.Any(psc => psc.SceneId != -1 && scn.SceneInfo.SNOId != psc.SceneId))).ToList();

			foreach (Scene scene in ignoredScenes)
			{
				if (scene.Mesh.Zone == null)
					return true;

				Vector2 pos = position.ToVector2();
				Vector2 min = scene.Mesh.Zone.ZoneMin;
				Vector2 max = scene.Mesh.Zone.ZoneMax;

				if (pos.X >= min.X && pos.X <= max.X && pos.Y >= min.Y && pos.Y <= max.Y)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Determines if the current node can be marked as Visited, and does so if needed
		/// </summary>
		/// <returns></returns>
		private Composite CheckNodeFinished()
		{
			return
			new PrioritySelector(
				new Decorator(ret => LastMoveResult == MoveResult.ReachedDestination,
					new Sequence(
						new Action(ret => SetNodeVisited("Reached Destination")),
						new Action(ret => LastMoveResult = MoveResult.Moved),
						new Action(ret => UpdateRoute())
					)
				),
				new Decorator(ret => BrainBehavior.DungeonExplorer.CurrentNode.Visited,
					new Sequence(
						new Action(ret => Logger.DBLog.DebugFormat("Current node was already marked as visited!")),
						new Action(ret => BrainBehavior.DungeonExplorer.CurrentRoute.Dequeue()),
						new Action(ret => UpdateRoute())
					)
				),
				new Decorator(ret => GetRouteUnvisitedNodeCount() == 0 || !BrainBehavior.DungeonExplorer.CurrentRoute.Any(),
					new Sequence(
						new Action(ret => Logger.DBLog.DebugFormat("Error - CheckIsNodeFinished() called while Route is empty!")),
						new Action(ret => UpdateRoute())
					)
				),
				new Decorator(ret => CurrentNavTarget.Distance2D(myPos) <= PathPrecision,
					new Sequence(
						new Action(ret => SetNodeVisited(String.Format("Node {0} is within PathPrecision ({1:0}/{2:0})",
							CurrentNavTarget, CurrentNavTarget.Distance2D(myPos), PathPrecision))),
						new Action(ret => UpdateRoute())
					)
				),
				new Decorator(ret => CurrentNavTarget.Distance2D(myPos) <= 90f && !MainGridProvider.CanStandAt(MainGridProvider.WorldToGrid(CurrentNavTarget.ToVector2())),
					new Sequence(
						new Action(ret => SetNodeVisited("Center Not Navigable")),
						new Action(ret => UpdateRoute())
					)
				),
				new Decorator(ret => ObjectCache.Obstacles.OfType<CacheServerObject>().Any(o => o.Position.Distance2D(CurrentNavTarget) <= o.Radius * 2),
					new Sequence(
						new Action(ret => SetNodeVisited("Navigation obstacle detected at node point")),
						new Action(ret => UpdateRoute())
					)
				),
				 new Decorator(ret => Bot.NavigationCache.currentMovementState == MovementState.WalkingInPlace && myPos.Distance2D(CurrentNavTarget) <= 50f && !Navigation.CanRayCast(myPos, CurrentNavTarget),
					new Sequence(
						new Action(ret => SetNodeVisited("Stuck moving to node point, marking done (in LoS and nearby!)")),
						new Action(ret => UpdateRoute())
					)
				),
				new Decorator(ret => SkipAheadCache.CheckPositionForSkipping(CurrentNavTarget),
					new Sequence(
						new Action(ret => SetNodeVisited("Found node to be in skip ahead cache, marking done")),
						new Action(ret => UpdateRoute())
					)
				),
				new Decorator(ret => !IgnoreExploredAreas && CheckNodeInMiniMap(CurrentNavTarget),
					new Sequence(
						new Action(ret => SetNodeVisited("Found node in Explored Area using Minimap")),
						new Action(ret => UpdateRoute())
					)
				),
				CheckIgnoredScenes()
			);
		}

		/// <summary>
		/// Updates the DungeonExplorer Route
		/// </summary>
		private void UpdateRoute()
		{
			CheckResetDungeonExplorer();

			BrainBehavior.DungeonExplorer.Update();
			PrintNodeCounts("BrainBehavior.DungeonExplorer.Update");

			// Throw an exception if this shiz don't work
			ValidateCurrentRoute();
		}

		/// <summary>
		/// Marks the current dungeon Explorer as Visited and dequeues it from the route
		/// </summary>
		/// <param name="reason"></param>
		private void SetNodeVisited(string reason = "")
		{
			Logger.DBLog.DebugFormat("Dequeueing current node {0} - {1}", BrainBehavior.DungeonExplorer.CurrentNode.NavigableCenter, reason);
			BrainBehavior.DungeonExplorer.CurrentNode.Visited = true;
			BrainBehavior.DungeonExplorer.CurrentRoute.Dequeue();

			MarkNearbyNodesVisited();

			PrintNodeCounts("SetNodeVisited");
		}

		public void MarkNearbyNodesVisited()
		{
			foreach (DungeonNode node in GridSegmentation.Nodes.Where(n => !n.Visited))
			{
				float distance = node.NavigableCenter.Distance2D(myPos);
				if (distance <= PathPrecision)
				{
					node.Visited = true;
					string reason2 = String.Format("Node {0} is within path precision {1:0}/{2:0}", node.NavigableCenter, distance, PathPrecision);
					Logger.DBLog.DebugFormat("Marking unvisited nearby node as visited - {0}", reason2);
				}
			}
		}

		/// <summary>
		/// Makes sure the current route is not null! Bad stuff happens if it's null...
		/// </summary>
		private static void ValidateCurrentRoute()
		{
			if (BrainBehavior.DungeonExplorer.CurrentRoute == null)
			{
				throw new ApplicationException("DungeonExplorer CurrentRoute is null");
			}
		}

		private bool CheckNodeInMiniMap(Vector3 position)
		{
			bool returnValue = false;
			try
			{
				if (ZetaDia.Minimap.IsValid)
				{
					returnValue = ZetaDia.Minimap.IsExplored(position, iWorldID);
				}
			}
			catch (Exception)
			{

			}
			return returnValue;
		}

		/// <summary>
		/// Prints a plethora of useful information about the Dungeon Exploration process
		/// </summary>
		/// <param name="step"></param>
		private void PrintNodeCounts(string step = "")
		{
			//if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.ProfileTag))
			//{
			//	string nodeDistance = String.Empty;
			//	if (GetRouteUnvisitedNodeCount() > 0)
			//	{
			//		try
			//		{
			//			float distance = BrainBehavior.DungeonExplorer.CurrentNode.NavigableCenter.Distance(myPos);

			//			if (distance > 0)
			//				nodeDistance = String.Format("Dist:{0:0}", Math.Round(distance / 10f, 2) * 10f);
			//		}
			//		catch { }
			//	}

			//	var log = String.Format("Nodes [Unvisited: Route:{1} Grid:{3} | Grid-Visited: {2}] Box:{4}/{5} Step:{6} {7} Nav:{8} RayCast:{9} PP:{10:0} Dir: {11} ZDiff:{12:0}",
			//		GetRouteVisistedNodeCount(),                                 // 0
			//		GetRouteUnvisitedNodeCount(),                                // 1
			//		GetGridSegmentationVisistedNodeCount(),                      // 2
			//		GetGridSegmentationUnvisitedNodeCount(),                     // 3
			//		GridSegmentation.BoxSize,                                    // 4
			//		GridSegmentation.BoxTolerance,                               // 5
			//		step,                                                        // 6
			//		nodeDistance,                                                // 7
			//		MainGridProvider.CanStandAt(MainGridProvider.WorldToGrid(CurrentNavTarget.ToVector2())), // 8
			//		!Navigator.Raycast(myPos, CurrentNavTarget),
			//		PathPrecision,
			//		MathUtil.GetHeadingToPoint(CurrentNavTarget),
			//		Math.Abs(myPos.Z - CurrentNavTarget.Z)
			//		);

			//	Logger.DBLog.DebugFormat(log);
			//}
		}

		/*
		 * Dungeon Explorer Nodes
		 */
		/// <summary>
		/// Gets the number of unvisited nodes in the DungeonExplorer Route
		/// </summary>
		/// <returns></returns>
		private int GetRouteUnvisitedNodeCount()
		{
			if (GetCurrentRouteNodeCount() > 0)
				return BrainBehavior.DungeonExplorer.CurrentRoute.Count(n => !n.Visited);
			else
				return 0;
		}

		/// <summary>
		/// Gets the number of visisted nodes in the DungeonExplorer Route
		/// </summary>
		/// <returns></returns>
		private int GetRouteVisistedNodeCount()
		{
			if (GetCurrentRouteNodeCount() > 0)
				return BrainBehavior.DungeonExplorer.CurrentRoute.Count(n => n.Visited);
			else
				return 0;
		}

		/// <summary>
		/// Gets the number of nodes in the DungeonExplorer Route
		/// </summary>
		/// <returns></returns>
		private int GetCurrentRouteNodeCount()
		{
			if (BrainBehavior.DungeonExplorer.CurrentRoute != null)
				return BrainBehavior.DungeonExplorer.CurrentRoute.Count();
			else
				return 0;
		}
		/*
		 *  Grid Segmentation Nodes
		 */
		/// <summary>
		/// Gets the number of Unvisited nodes as reported by the Grid Segmentation provider
		/// </summary>
		/// <returns></returns>
		private int GetGridSegmentationUnvisitedNodeCount()
		{
			if (GetGridSegmentationNodeCount() > 0)
				return GridSegmentation.Nodes.Count(n => !n.Visited);
			else
				return 0;
		}

		/// <summary>
		/// Gets the number of Visited nodes as reported by the Grid Segmentation provider
		/// </summary>
		/// <returns></returns>
		private int GetGridSegmentationVisistedNodeCount()
		{
			if (GetCurrentRouteNodeCount() > 0)
				return GridSegmentation.Nodes.Count(n => n.Visited);
			else
				return 0;
		}

		/// <summary>
		/// Gets the total number of nodes with the current BoxSize/Tolerance as reported by the Grid Segmentation Provider
		/// </summary>
		/// <returns></returns>
		private int GetGridSegmentationNodeCount()
		{
			if (GridSegmentation.Nodes != null)
				return GridSegmentation.Nodes.Count();
			else
				return 0;
		}

		private MoveResult LastMoveResult = MoveResult.Moved;
		private DateTime lastGeneratedPath = DateTime.MinValue;
		/// <summary>
		/// Moves the bot to the next DungeonExplorer node
		/// </summary>
		private void MoveToNextNode()
		{
			SkipAheadCache.RecordSkipAheadCachePoint(PathPrecision);

			NextNode = BrainBehavior.DungeonExplorer.CurrentNode;
			Vector3 moveTarget = NextNode.NavigableCenter;

			//string nodeName = String.Format("{0} Distance: {1:0} Direction: {2}",
			//	NextNode.NavigableCenter, NextNode.NavigableCenter.Distance(Bot.Character.Data.Position), MathUtil.GetHeadingToPoint(NextNode.NavigableCenter));

			SkipAheadCache.RecordSkipAheadCachePoint(PathPrecision);

			LastMoveResult = Navigator.MoveTo(CurrentNavTarget);
		}

		private int iWorldID;
		/// <summary>
		/// Initizializes the profile tag and sets defaults as needed
		/// </summary>
		private void Init(bool forced = false)
		{
			iWorldID = ZetaDia.Me.WorldDynamicId;

			if (BoxSize == 0)
				BoxSize = 15;

			if (BoxTolerance == 0)
				BoxTolerance = 0.55f;

			if (PathPrecision == 0)
				PathPrecision = BoxSize / 2f;

			float minPathPrecision = 15f;

			if (PathPrecision < minPathPrecision)
				PathPrecision = minPathPrecision;

			if (ObjectDistance == 0)
				ObjectDistance = 40f;

			if (MarkerDistance == 0)
				MarkerDistance = 25f;

			if (TimeoutValue == 0)
				TimeoutValue = 900;

			SkipAheadCache.ClearCache();
			PriorityScenesInvestigated.Clear();
			MiniMapMarker.KnownMarkers.Clear();
			if (PriorityScenes == null)
				PriorityScenes = new List<PrioritizeScene>();

			if (IgnoreScenes == null)
				IgnoreScenes = new List<IgnoreScene>();

			if (AlternateActors == null)
				AlternateActors = new List<AlternateActor>();

			if (!forced)
			{
				Logger.DBLog.DebugFormat(
					"Initialized TrinityExploreDungeon: boxSize={0} boxTolerance={1:0.00} endType={2} timeoutType={3} timeoutValue={4} pathPrecision={5:0} sceneId={6} actorId={7} objectDistance={8} markerDistance={9} exitNameHash={10}",
					GridSegmentation.BoxSize, GridSegmentation.BoxTolerance, EndType, ExploreTimeoutType, TimeoutValue, PathPrecision, SceneId, ActorId, ObjectDistance, MarkerDistance, ExitNameHash);
			}
			InitDone = true;
		}


		private bool isDone = false;
		/// <summary>
		/// When true, the next profile tag is used
		/// </summary>
		public override bool IsDone
		{
			get { return !IsActiveQuestStep || isDone; }
		}

		/// <summary>
		/// Resets this profile tag to defaults
		/// </summary>
		public override void ResetCachedDone()
		{
			isDone = false;
			InitDone = false;
			GridSegmentation.Reset();
			BrainBehavior.DungeonExplorer.Reset();
			MiniMapMarker.KnownMarkers.Clear();
		}
	}
}

/*
 * Never need to call GridSegmentation.Update()
 * GridSegmentation.Reset() is automatically called on world change
 * DungeonExplorer.Reset() will reset the current route and revisit nodes
 * DungeonExplorer.Update() will update the current route to include new scenes
 */