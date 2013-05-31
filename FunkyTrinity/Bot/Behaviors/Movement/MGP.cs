using System;
using Zeta.Common;
using Zeta;
using Zeta.Navigation;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  // For path finding
		  /// <summary>
		  /// The Grid Provider for Navigation checks
		  /// </summary>
		  internal static MainGridProvider mgp
		  {
				get
				{
					 return (Navigator.SearchGridProvider as MainGridProvider);
				}
		  }
		  /// <summary>
		  /// Bot Dungeon Explorer -- Used to find current path.
		  /// </summary>
		  internal static Zeta.CommonBot.Dungeons.DungeonExplorer de
		  {
				get
				{
					 return (Zeta.CommonBot.Logic.BrainBehavior.DungeonExplorer);
				}
		  }

		  ///<summary>
		  ///Returns Navigator as DefaultNavigationProvider (Pathing)
		  ///</summary>
		  internal static DefaultNavigationProvider navigation
		  {
				get
				{
					 return Navigator.GetNavigationProviderAs<DefaultNavigationProvider>();
				}
		  }
		  private static Vector3 currentpathvector_=vNullLocation;
		  internal static Vector3 CurrentPathVector
		  {
				get
				{
					 if (navigation.CurrentPath.Count>0&&currentpathvector_!=navigation.CurrentPath.Current)
						  currentpathvector_=navigation.CurrentPath.Current;
					 else if (de.CurrentRoute!=null&&de.CurrentRoute.Count>0&&currentpathvector_!=de.CurrentNode.NavigableCenter)
						  currentpathvector_=de.CurrentNode.NavigableCenter;
					 else
						  currentpathvector_=vNullLocation;

					 return currentpathvector_;
				}
		  }

		  internal static DateTime LastMGPUpdate=DateTime.MinValue;
		  internal static Vector3 LastPositionUpdated=vNullLocation;
		  internal static void UpdateSearchGridProvider(bool force=false)
		  {
				if (!ZetaDia.IsInGame||ZetaDia.IsLoadingWorld)
					 return;

				//Enforce a time update rule and a position check
				if (!force) return;


				DbHelper.Log(FunkyTrinity.Funky.DbHelper.TrinityLogLevel.Verbose, FunkyTrinity.Funky.DbHelper.LogCategory.CacheManagement, "Updating Grid Provider", true);
				try
				{
					 mgp.Update();
					 // Log(mgp.BoundsMin.ToString()+", Max: "+mgp.BoundsMax.ToString()+", Width"+mgp.Width+", Height: "+mgp.Height+", Requires Pathing: "+mgp.WorldRequiresPathfinding.ToString());
				} catch
				{
					 Log("MGP Update Exception Safely Handled!", true);
					 return;
				}

				LastMGPUpdate=DateTime.Now;
				LastPositionUpdated=Bot.Character.Position;
		  }
	 }
}