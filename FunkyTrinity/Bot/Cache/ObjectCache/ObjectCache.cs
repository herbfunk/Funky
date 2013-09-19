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
using FunkyTrinity.Enums;
using FunkyTrinity.Cache;

namespace FunkyTrinity.Cache
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



				#region SNO Cache Dictionaries
				internal static Dictionary<int, ActorType?> dictActorType=new Dictionary<int, ActorType?>();
				internal static Dictionary<int, TargetType?> dictTargetType=new Dictionary<int, TargetType?>();
				internal static Dictionary<int, MonsterSize?> dictMonstersize=new Dictionary<int, MonsterSize?>();
				internal static Dictionary<int, MonsterType?> dictMonstertype=new Dictionary<int, MonsterType?>();
				internal static Dictionary<int, float?> dictCollisionRadius=new Dictionary<int, float?>();
				internal static Dictionary<int, String> dictInternalName=new Dictionary<int, String>();
				internal static Dictionary<int, ObstacleType?> dictObstacleType=new Dictionary<int, ObstacleType?>();
				internal static Dictionary<int, float?> dictActorSphereRadius=new Dictionary<int, float?>();
				internal static Dictionary<int, bool?> dictCanBurrow=new Dictionary<int, bool?>();
				internal static Dictionary<int, bool?> dictGrantsNoXp=new Dictionary<int, bool?>();
				internal static Dictionary<int, bool?> dictDropsNoLoot=new Dictionary<int, bool?>();
				internal static Dictionary<int, GizmoType?> dictGizmoType=new Dictionary<int, GizmoType?>();
				internal static Dictionary<int, bool?> dictIsBarricade=new Dictionary<int, bool?>();
				internal static Dictionary<int, double> dictProjectileSpeed=new Dictionary<int, double>();
				#endregion

				//Common Used Profile Tags that should be considered Out-Of-Combat Behavior.
				internal static readonly HashSet<Type> oocDBTags=new HashSet<Type> 
																	{ 
																	  typeof(Zeta.CommonBot.Profile.Common.UseWaypointTag), 
																	  typeof(Zeta.CommonBot.Profile.Common.UseObjectTag),
																	  typeof(Zeta.CommonBot.Profile.Common.UseTownPortalTag),
																	  typeof(Zeta.CommonBot.Profile.Common.WaitTimerTag),
																	};

				//Common Used Profile Tags that requires backtracking during combat sessions.
				internal static readonly HashSet<Type> InteractiveTags=new HashSet<Type> 
																	{ 
																	  typeof(Zeta.CommonBot.Profile.Common.UseWaypointTag), 
																	  typeof(Zeta.CommonBot.Profile.Common.UseObjectTag),
																	  typeof(Zeta.CommonBot.Profile.Common.UseTownPortalTag),
																	  typeof(Zeta.CommonBot.Profile.Common.UsePortalTag),
																	};
		  }
	 
}