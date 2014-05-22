using System.Linq;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using FunkyBot.Game.Bounty;
using FunkyBot.Movement;
using Zeta.Common;
using Zeta.Game.Internals;

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
				return Bot.Settings.LOSMovement.EnableLOSMovementBehavior &&
					!Bot.IsInNonCombatBehavior &&
					(Bot.Targeting.Cache.Environment.LoSMovementObjects.Count > 0 ||
					Bot.NavigationCache.LOSmovementObject != null ||
					(Bot.Game.AdventureMode && Bot.Game.Bounty.ShouldNavigatePointsOfInterest && Bot.Game.Bounty.CurrentBountyMapMarkers.Count > 0));
			}
		}

		public override void Initialize()
		{
			base.Test = (ref CacheObject obj) =>
			{

				if (obj == null)
				{
					if (Bot.NavigationCache.LOSmovementObject != null &&
						(Bot.NavigationCache.LOSmovementObject.CentreDistance < (Bot.NavigationCache.LOSmovementObject.IgnoringCacheCheck ? 25f : 45f) &&
						(Bot.NavigationCache.LOSmovementObject.IgnoringCacheCheck || !Bot.NavigationCache.LOSmovementObject.CacheContainsOrginObject())))
					{//Invalidated the Line of sight obj!


						Logger.Write(LogLevel.LineOfSight, "LOS Object is No Longer Valid -- Reseting.");


						if (!Bot.NavigationCache.LOSmovementObject.IgnoringCacheCheck ||
								(Bot.Game.Bounty.CurrentBountyCacheEntry == null || Bot.Game.Bounty.CurrentBountyCacheEntry.Type != BountyQuestTypes.Event))
						{
							Bot.NavigationCache.LOSBlacklistedRAGUIDs.Add(Bot.NavigationCache.LOSmovementObject.OrginCacheObjectRAGUID);
						}

						Bot.NavigationCache.LOSmovementObject = null;

						if (Bot.Targeting.Cache.LastCachedTarget.targetType.Value == TargetType.LineOfSight)
							Navigation.NP.Clear();
					}


					if (Bot.NavigationCache.LOSmovementObject == null)
					{//New LOS Movement Selection.


						Bot.Targeting.Cache.Environment.LoSMovementObjects = Bot.Targeting.Cache.Environment.LoSMovementObjects.OrderBy(o => o.CentreDistance).ToList();

						//Iterate Objects!
						foreach (var cobj in Bot.Targeting.Cache.Environment.LoSMovementObjects)
						{

							if (Bot.NavigationCache.LOSBlacklistedRAGUIDs.Contains(cobj.RAGUID)) continue;

							if (!Navigation.NP.CanFullyClientPathTo(cobj.Position)) continue;

							Logger.Write(LogLevel.LineOfSight, "Line of Sight Started for object {0} -- with {1} vectors", cobj.InternalName, Navigation.NP.CurrentPath.Count);


							Bot.NavigationCache.LOSBlacklistedRAGUIDs.Add(cobj.RAGUID);

							//Set the object
							Bot.NavigationCache.LOSmovementObject = new CacheLineOfSight(cobj, cobj.Position);
							break;
						}

						//Check if we still found nothing and game mode is adventure mode
						if (Bot.NavigationCache.LOSmovementObject == null && Bot.Game.AdventureMode && Bot.Game.Bounty.ShouldNavigatePointsOfInterest
								&&  Bot.Game.Bounty.ActiveBounty != null && Bot.Game.Bounty.CurrentBountyMapMarkers.Count > 0)
						{

							Bot.Game.Bounty.ActiveBounty.Refresh();
							if (Bot.Game.Bounty.ActiveBounty.State == QuestState.InProgress)
							{//Lets make sure the bounty is not completed.

								foreach (var mapmarker in Bot.Game.Bounty.CurrentBountyMapMarkers.Values)
								{
									if (mapmarker.WorldID != Bot.Character.Data.CurrentWorldDynamicID) continue;
									if (Bot.NavigationCache.LOSBlacklistedRAGUIDs.Contains(mapmarker.GetHashCode())) continue;
									if (mapmarker.Distance > 750f || mapmarker.Distance < 25f) continue;

									if (!Navigation.NP.CanFullyClientPathTo(mapmarker.Position)) continue;


									Logger.Write(LogLevel.LineOfSight, "Line of Sight Started for Map Marker with {0} vectors", Navigation.NP.CurrentPath.Count);

									if (Bot.Game.Bounty.CurrentBountyCacheEntry == null || Bot.Game.Bounty.CurrentBountyCacheEntry.Type != BountyQuestTypes.Event)
										Bot.NavigationCache.LOSBlacklistedRAGUIDs.Add(mapmarker.GetHashCode());

									//Set the object
									Bot.NavigationCache.LOSmovementObject = new CacheLineOfSight(mapmarker);
								}

							}
						}
					}

					if (Bot.NavigationCache.LOSmovementObject != null)
					{//Line of Sight unit is valid

						//See if the orgin object is still valid..

						//min Distance for Map Markers is 25f
						if (Bot.NavigationCache.LOSmovementObject.CentreDistance < (Bot.NavigationCache.LOSmovementObject.IgnoringCacheCheck ? 25f : 35f))
						{

							if (!Bot.NavigationCache.LOSmovementObject.CacheContainsOrginObject())
							{
								Logger.Write(LogLevel.LineOfSight, "Line of Sight Ending due to Orgin Object No Longer Available!");
								Bot.NavigationCache.LOSmovementObject = null;
								return false;
							}

							if (!Bot.NavigationCache.LOSmovementObject.IsValidForTargeting())
							{//Valid for Targeting?
								Logger.Write(LogLevel.LineOfSight, "Line of Sight Ending due to Orgin Object Not Valid for Targeting!");
								Bot.NavigationCache.LOSmovementObject = null;
								return false;
							}

							//Minimap Marker Check
							if (Bot.NavigationCache.LOSmovementObject.IgnoringCacheCheck && 
								Bot.Game.Bounty.CurrentActCache!=null && Bot.Game.Bounty.CurrentBountyCacheEntry!=null && Bot.Game.Bounty.CurrentBountyCacheEntry.Type==BountyQuestTypes.Event)
							{
								Bot.Game.Bounty.RefreshActiveQuests();
							}

							//Update Position using Orgin Object?
							Bot.NavigationCache.LOSmovementObject.UpdateOrginObject();
						}

						Navigation.NP.MoveTo(Bot.NavigationCache.LOSmovementObject.Position, "LOS", true);
						if (Navigation.NP.CurrentPath.Count > 0)
						{
							//Setup a temp target that the handler will use
							obj = new CacheObject(Bot.NavigationCache.LOSmovementObject.Position, TargetType.LineOfSight, 1d, "LOS Movement", Navigation.NP.PathPrecision);
							return true;
						}
					}
				}

				return false;
			};
		}
	}
}