using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using System.Collections.Generic;

namespace FunkyBot.Movement
{

		  public static class SkipAheadCache
		  {
				public static bool bSkipAheadAGo=false;

				private static List<SkipAheadNavigation> SkipAheadAreaCache=new List<SkipAheadNavigation>();
				internal static DateTime lastRecordedSkipAheadCache=DateTime.Today;
			  internal static Vector3 lastRecordedSkipAheadLocation=Vector3.Zero;
				private static List<SkipAheadNavigation> UsedSkipAheadAreaCache=new List<SkipAheadNavigation>();

				internal static void ClearCache()
				{
					if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
						Logger.Write(LogLevel.Movement, "Clearing Skipahead Cache");
					 SkipAheadAreaCache.Clear();
					 UsedSkipAheadAreaCache.Clear();
					 lastRecordedSkipAheadCache=DateTime.Now;
				}
				public static bool CheckPositionForSkipping(Vector3 Position)
				{
					 foreach (var v in UsedSkipAheadAreaCache)
					 {
						  if (Position.Distance(v.Position)<=v.Radius)
								return true;
					 }

					 bool valid=false;
					 if (SkipAheadAreaCache.Count>0)
					 {
						  int validIndex=-1;
						  for (int i=0; i<SkipAheadAreaCache.Count-1; i++)
						  {
								SkipAheadNavigation v=SkipAheadAreaCache[i];
								if (Position.Distance(v.Position)<=v.Radius)
								{
									 validIndex=i;
									 valid=true;
									 break;
								}
						  }
						  if (valid&&validIndex>0)
						  {
								UsedSkipAheadAreaCache.Add(SkipAheadAreaCache[validIndex].Clone());
								SkipAheadAreaCache.RemoveRange(0, validIndex-1);
								SkipAheadAreaCache.TrimExcess();
						  }
					 }
					 return valid;

				}
				public static void RecordSkipAheadCachePoint(float Precision=20f)
				{
					 double millisecondsLastRecord=DateTime.Now.Subtract(lastRecordedSkipAheadCache).TotalMilliseconds;

					 if (millisecondsLastRecord<100)
						  return;
					 // else if (millisecondsLastRecord>10000) //10 seconds.. clear cache!
					 // SkipAheadAreaCache.Clear();

					 if (SkipAheadAreaCache.Any(p => p.Position.Distance(ZetaDia.Me.Position)<=Precision))
						  return;
					 lastRecordedSkipAheadLocation = ZetaDia.Me.Position;
					 SkipAheadAreaCache.Add(new SkipAheadNavigation(lastRecordedSkipAheadLocation, Precision));

					 lastRecordedSkipAheadCache=DateTime.Now;
				}
		  }

		  // A list of small areas covering zones we move through while fighting to help our custom move-handler skip ahead waypoints
		  internal class SkipAheadNavigation
		  {
				public Vector3 Position { get; set; }
				public float Radius { get; set; }

				public SkipAheadNavigation(Vector3 pos, float radius)
				{
					 Position=pos;
					 Radius=radius;
				}

				public SkipAheadNavigation Clone()
				{
					 return (SkipAheadNavigation)MemberwiseClone();
				}
		  }
    
}