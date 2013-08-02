using System;
using System.Linq;
using System.Collections.Generic;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.Internals;
using Zeta.Internals.SNO;
using Zeta.CommonBot;
using Zeta.Internals.Actors.Gizmos;
using System.Windows;
using System.Collections;
using System.Collections.ObjectModel;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  ///<summary>
		  ///Contains Collections for all the cached objects being tracked.
		  ///</summary>
		  public static partial class ObjectCache
		  {
				///<summary>
				///Cached Objects.
				///</summary>
				public static ObjectCollection Objects=new ObjectCollection();

				///<summary>
				///Obstacles related to either avoidances or navigational blocks.
				///</summary>
				public static ObstacleCollection Obstacles=new ObstacleCollection();

				///<summary>
				///Cached Sno Data.
				///</summary>
				public static SnoCollection cacheSnoCollection=new SnoCollection();

		  }
	 }
}