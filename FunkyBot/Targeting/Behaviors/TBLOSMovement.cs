using System;
using System.Linq;
using System.Collections.Generic;
using FunkyBot.Cache;
using FunkyBot.Cache.Enums;
using FunkyBot.Movement;
using FunkyBot.Movement.Clustering;
using Zeta.Common;
using Zeta.Navigation;
namespace FunkyBot.Targeting.Behaviors
{
	 public class TBLOSMovement : TargetBehavior
	 {
		  /*
			Line of Sight Movement Behavior
		   --Units that are "special" that fail the Line of Sight Check during ObjectIsValidForTargeting will be added to a special list of units that we check here.
		   --We use the list of units to compute clusters
 		   --We iterate the units searching for any that has <=75% HP and CanFullyClientPathTo. 
		   --Once we find a valid unit, we generate the path and let the target handler begin the movement.
		   

		   Note: This behavior only activates when no targets are found during refresh.
				(Excluding Non-Movement targets)


		  */

		  public TBLOSMovement() : base() { }
		  public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.LineOfSight; } }
		  public override bool BehavioralCondition
		  {
				get
				{
					 //Check objects added for LOS movement
					 return Bot.Settings.Plugin.EnableLineOfSightBehavior&&!Bot.IsInNonCombatBehavior&&(Bot.Combat.LoSMovementObjects.Count>0||Bot.NavigationCache.LOSmovementObject!=null);
				}
		  }

		  public override void Initialize()
		  {
				base.Test=(ref CacheObject obj) =>
				{
					 if (obj==null)
					 {
						  if (Bot.NavigationCache.LOSmovementObject!=null&&
                              (Bot.NavigationCache.LOSmovementObject.CentreDistance<50f&&!Bot.NavigationCache.LOSmovementObject.IsStillValid()))
						  {//Invalidated the Line of sight Unit!

								if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
								{
									 Logger.Write(LogLevel.Movement, "LOS Unit is No Longer Valid -- Reseting.");
								}

								Bot.NavigationCache.LOSBlacklistedRAGUIDs.Add(Bot.NavigationCache.LOSmovementObject.RAGUID);
								Bot.NavigationCache.LOSVector=Vector3.Zero;
								Bot.NavigationCache.LOSmovementObject=null;
						  }
							

						  if (Bot.NavigationCache.LOSmovementObject==null)
						  {//New LOS Movement Selection.

								Bot.Combat.LoSMovementObjects=Bot.Combat.LoSMovementObjects.OrderBy(o => o.CentreDistance).ToList();
								foreach (var cobj in Bot.Combat.LoSMovementObjects)
								{//Iterate Units

									 if (Bot.NavigationCache.LOSBlacklistedRAGUIDs.Contains(cobj.RAGUID)) continue;

									 if (!Navigation.NP.CanFullyClientPathTo(cobj.Position)) continue;

									 if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Movement))
									 {
										  Logger.Write(LogLevel.Movement, "Line of Sight Started for object {0} -- with {1} vectors", cobj.InternalName, Navigation.NP.CurrentPath.Count);
									 }

									 Bot.NavigationCache.LOSBlacklistedRAGUIDs.Add(cobj.RAGUID);

									 //Set the unit as our Line of Sight Unit
									 Bot.NavigationCache.LOSmovementObject=cobj;
									 break;
								}

						  }

						  if (Bot.NavigationCache.LOSmovementObject!=null)
						  {//Line of Sight unit is valid

								if (Bot.NavigationCache.LOSVector==Vector3.Zero)
								{//Set the Vector we will be using.
									 Bot.NavigationCache.LOSVector=Bot.NavigationCache.LOSmovementObject.Position;
								}

								//Generate the path here so we can start moving..
								Navigation.NP.MoveTo(Bot.NavigationCache.LOSVector, "LineOfSightMoveTo", true);

								//Setup a temp target that the handler will use
								obj=new CacheObject(Bot.NavigationCache.LOSVector, TargetType.LineOfSight, 1d, "Line Of Sight", 5f, Bot.NavigationCache.LOSmovementObject.RAGUID);
								return true;
						  }
					 }

					 return false;
				};
		  }
	 }
}