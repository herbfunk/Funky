using System;
using Zeta.Common;
using Zeta;
using Zeta.Navigation;

namespace FunkyTrinity
{
	 public partial class Funky
	 {

		  /// <summary>
		  /// MainGridProvider
		  /// </summary>
		  internal static MainGridProvider MGP
		  {
				get
				{
					 return (Navigator.SearchGridProvider as MainGridProvider);
				}
		  }
		  /// <summary>
		  /// DungeonExplorer
		  /// </summary>
		  internal static Zeta.CommonBot.Dungeons.DungeonExplorer DE
		  {
				get
				{
					 return (Zeta.CommonBot.Logic.BrainBehavior.DungeonExplorer);
				}
		  }

		  ///<summary>
		  ///Returns GetNavigationProviderAs as DefaultNavigationProvider (Pathing)
		  ///</summary>
		  internal static DefaultNavigationProvider NP
		  {
				get
				{
					 return Navigator.GetNavigationProviderAs<DefaultNavigationProvider>();
				}
		  }

		  private static IndexedList<Vector3> CurrentPath=null;
		  


		  private static Vector3 currentpathvector_=vNullLocation;
		  ///<summary>
		  ///Trys to find current path by checking NavigationProvider and DungeonExplorer for any valid pathing.
		  ///</summary>
		  internal static Vector3 CurrentPathVector
		  {
				get
				{
					 if (NP.CurrentPath.Count>0&&currentpathvector_!=NP.CurrentPath.Current)
						  currentpathvector_=NP.CurrentPath.Current;
					 else if (DE.CurrentRoute!=null&&DE.CurrentRoute.Count>0&&currentpathvector_!=DE.CurrentNode.NavigableCenter)
						  currentpathvector_=DE.CurrentNode.NavigableCenter;
					 else
						  currentpathvector_=vNullLocation;

					 return currentpathvector_;
				}
		  }


		  private static DateTime LastMGPUpdate=DateTime.MinValue;
		  private static int LastScenceUpdateOccured=0;

		  internal static void UpdateSearchGridProvider(bool force=false)
		  {
				if (!ZetaDia.IsInGame||ZetaDia.IsLoadingWorld)
					 return;

				//Enforce a time update rule and a position check
				if (!force&&LastScenceUpdateOccured==Bot.Character.iSceneID)
					 return;
				
				if (LastScenceUpdateOccured!=Bot.Character.iSceneID)
					 LastScenceUpdateOccured=Bot.Character.iSceneID;


				if (!force&&DateTime.Now.Subtract(LastMGPUpdate).TotalMilliseconds<1000)
					 return;


				Log("[Funky] Updating Main Grid Provider....", true);
				
				try
				{
					 MGP.Update();
				} catch
				{
					 Log("MGP Update Exception Safely Handled!", true);
					 return;
				}

				LastMGPUpdate=DateTime.Now;
		  }
	 }
}