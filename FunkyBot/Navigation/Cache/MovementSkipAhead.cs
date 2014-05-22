using System;
using System.Linq;
using Zeta.Bot;
using Zeta.Bot.Dungeons;
using Zeta.Bot.Logic;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Game;

namespace FunkyBot.Movement
{

	public static class SkipAheadCache
	{
		public static bool bSkipAheadAGo = false;

		private static List<SkipAheadNavigation> SkipAheadAreaCache = new List<SkipAheadNavigation>();
		internal static DateTime lastRecordedSkipAheadCache = DateTime.Today;
		internal static Vector3 lastRecordedSkipAheadLocation = Vector3.Zero;
		private static List<SkipAheadNavigation> UsedSkipAheadAreaCache = new List<SkipAheadNavigation>();

		internal static void ClearCache()
		{
			Logger.Write(LogLevel.Movement, "Clearing Skipahead Cache");
			SkipAheadAreaCache.Clear();
			UsedSkipAheadAreaCache.Clear();
			lastRecordedSkipAheadCache = DateTime.Now;
		}
		public static bool CheckPositionForSkipping(Vector3 Position)
		{
			foreach (var v in UsedSkipAheadAreaCache)
			{
				if (Position.Distance(v.Position) <= v.Radius)
					return true;
			}

			bool valid = false;
			if (SkipAheadAreaCache.Count > 0)
			{
				int validIndex = -1;
				for (int i = 0; i < SkipAheadAreaCache.Count - 1; i++)
				{
					SkipAheadNavigation v = SkipAheadAreaCache[i];
					if (Position.Distance(v.Position) <= v.Radius)
					{
						validIndex = i;
						valid = true;
						break;
					}
				}
				if (valid && validIndex > 0)
				{
					UsedSkipAheadAreaCache.Add(SkipAheadAreaCache[validIndex].Clone());
					SkipAheadAreaCache.RemoveRange(0, validIndex - 1);
					SkipAheadAreaCache.TrimExcess();
				}
			}
			return valid;

		}
		public static void RecordSkipAheadCachePoint(float Precision = 20f)
		{
			double millisecondsLastRecord = DateTime.Now.Subtract(lastRecordedSkipAheadCache).TotalMilliseconds;

			if (millisecondsLastRecord < 100)
				return;
			// else if (millisecondsLastRecord>10000) //10 seconds.. clear cache!
			// SkipAheadAreaCache.Clear();

			if (SkipAheadAreaCache.Any(p => p.Position.Distance(ZetaDia.Me.Position) <= Precision))
				return;

			lastRecordedSkipAheadLocation = ZetaDia.Me.Position;

			SkipAheadAreaCache.Add(new SkipAheadNavigation(lastRecordedSkipAheadLocation, Precision));

			//Dungeon Explorer?
			if (Navigation.CurrentDungeonExplorer != null)
			{
				if (Navigation.DungeonExplorerCurrentNode != null)
				{
					Vector2 lastrecordedV2 = lastRecordedSkipAheadLocation.ToVector2();

					if (lastrecordedV2.Distance(Navigation.DungeonExplorerCurrentNode.Center) <= Precision)
					{
						Navigation.DungeonExplorerCurrentNode.Visited = true;
						Logger.DBLog.Info("[Funky] Marking Node as Visited!");
					}
				}
				//if (Navigation.CurrentDungeonExplorer.CurrentRoute != null && Navigation.CurrentDungeonExplorer.CurrentRoute.Count > 0)
				//{
				//	List<DungeonNode> RouteNodes = Navigation.CurrentDungeonExplorer.CurrentRoute.ToList();

				//	Vector2 lastrecordedV2 = lastRecordedSkipAheadLocation.ToVector2();
				//	foreach (var node in Navigation.CurrentDungeonExplorer.CurrentRoute)
				//	{
				//		if (lastrecordedV2.Distance(node.Center) <= Precision)
				//		{
				//			node.Visited = true;
				//		}
				//	}
				//}
			}

			lastRecordedSkipAheadCache = DateTime.Now;
		}

		public static HashSet<int> LevelAreaIDsIgnoreSkipping = new HashSet<int>
 		{
			276226, //Keep Depths
		};
	}

	// A list of small areas covering zones we move through while fighting to help our custom move-handler skip ahead waypoints
	internal class SkipAheadNavigation
	{
		public Vector3 Position { get; set; }
		public float Radius { get; set; }

		public SkipAheadNavigation(Vector3 pos, float radius)
		{
			Position = pos;
			Radius = radius;
		}

		public SkipAheadNavigation Clone()
		{
			return (SkipAheadNavigation)MemberwiseClone();
		}
	}

}
