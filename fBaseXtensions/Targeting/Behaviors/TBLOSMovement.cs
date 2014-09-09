using System.Linq;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using fBaseXtensions.Settings;
using Zeta.Game.Internals;

namespace fBaseXtensions.Targeting.Behaviors
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
		private float MinimumDistance = SettingLOSMovement.LOSSettingsTag.MiniumRangeObjects;

		public TBLOSMovement() : base() { }
		public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.LineOfSight; } }
		public override bool BehavioralCondition
		{
			get
			{
				//Check objects added for LOS movement
				return SettingLOSMovement.LOSSettingsTag.EnableLOSMovementBehavior &&
					!FunkyGame.IsInNonCombatBehavior &&
					(FunkyGame.Targeting.Cache.Environment.LoSMovementObjects.Count > 0 ||
					FunkyGame.Navigation.LOSmovementObject != null ||
					(FunkyGame.AdventureMode && FunkyGame.Game.ShouldNavigatePointsOfInterest && FunkyGame.Bounty.CurrentBountyMapMarkers.Count > 0));
			}
		}

		public override void Initialize()
		{
			base.Test = (ref CacheObject obj) =>
			{

				if (obj == null)
				{
					if (FunkyGame.Navigation.LOSmovementObject != null &&
						(FunkyGame.Navigation.LOSmovementObject.CentreDistance < (FunkyGame.Navigation.LOSmovementObject.IgnoringCacheCheck ? SettingLOSMovement.LOSSettingsTag.MinimumRangeMarkers : SettingLOSMovement.LOSSettingsTag.MiniumRangeObjects) &&
						(FunkyGame.Navigation.LOSmovementObject.IgnoringCacheCheck || !FunkyGame.Navigation.LOSmovementObject.CacheContainsOrginObject())))
					{//Invalidated the Line of sight obj!


						Logger.Write(LogLevel.LineOfSight, "LOS Object is No Longer Valid -- Reseting.");


						if (!FunkyGame.Navigation.LOSmovementObject.IgnoringCacheCheck ||
								(FunkyGame.Bounty.CurrentBountyCacheEntry == null || FunkyGame.Bounty.CurrentBountyCacheEntry.Type != BountyTypes.Event))
						{
							FunkyGame.Navigation.LOSBlacklistedRAGUIDs.Add(FunkyGame.Navigation.LOSmovementObject.OrginCacheObjectRAGUID);
						}

						FunkyGame.Navigation.LOSmovementObject = null;

						if (FunkyGame.Targeting.Cache.LastCachedTarget.targetType.HasValue && FunkyGame.Targeting.Cache.LastCachedTarget.targetType.Value == TargetType.LineOfSight)
							Navigation.Navigation.NP.Clear();
					}


					if (FunkyGame.Navigation.LOSmovementObject == null)
					{//New LOS Movement Selection.


						FunkyGame.Targeting.Cache.Environment.LoSMovementObjects = FunkyGame.Targeting.Cache.Environment.LoSMovementObjects.OrderBy(o => o.CentreDistance).ToList();

						//Iterate Objects!
						foreach (var cobj in FunkyGame.Targeting.Cache.Environment.LoSMovementObjects)
						{

							if (FunkyGame.Navigation.LOSBlacklistedRAGUIDs.Contains(cobj.RAGUID)) continue;

							if (!Navigation.Navigation.NP.CanFullyClientPathTo(cobj.Position) &&
										!Navigation.Navigation.NP.CanPathWithinDistance(cobj.Position, SettingLOSMovement.LOSSettingsTag.MiniumRangeObjects)) continue;

							Logger.Write(LogLevel.LineOfSight, "Line of Sight Started for object {0} -- with {1} vectors\r\n{2}", cobj.InternalName, Navigation.Navigation.NP.CurrentPath.Count, cobj.DebugString);


							FunkyGame.Navigation.LOSBlacklistedRAGUIDs.Add(cobj.RAGUID);

							//Set the object
							FunkyGame.Navigation.LOSmovementObject = new CacheLineOfSight(cobj, cobj.Position);
							if (cobj.IsBurrowableUnit || cobj.IsStealthableUnit || cobj.IsWormBoss)
								MinimumDistance = 10;
							else
								MinimumDistance = FunkyBaseExtension.Settings.LOSMovement.MiniumRangeObjects;

							break;
						}

						//Check if we still found nothing and game mode is adventure mode
						if (FunkyGame.Navigation.LOSmovementObject == null && FunkyGame.AdventureMode && FunkyGame.Game.ShouldNavigatePointsOfInterest
								&&  FunkyGame.Bounty.ActiveBounty != null && FunkyGame.Bounty.CurrentBountyMapMarkers.Count > 0)
						{

							FunkyGame.Bounty.ActiveBounty.Refresh();
							if (FunkyGame.Bounty.ActiveBounty.State == QuestState.InProgress)
							{//Lets make sure the bounty is not completed.

								foreach (var mapmarker in FunkyGame.Bounty.CurrentBountyMapMarkers.Values)
								{
									if (mapmarker.WorldID != FunkyGame.Hero.CurrentWorldDynamicID) continue;
									if (FunkyGame.Navigation.LOSBlacklistedRAGUIDs.Contains(mapmarker.GetHashCode())) continue;
									if (mapmarker.Distance > 750f || mapmarker.Distance < 25f) continue;

									if (!Navigation.Navigation.NP.CanFullyClientPathTo(mapmarker.Position) &&
										!Navigation.Navigation.NP.CanPathWithinDistance(mapmarker.Position, SettingLOSMovement.LOSSettingsTag.MinimumRangeMarkers)) continue;


									Logger.Write(LogLevel.LineOfSight, "Line of Sight Started for Map Marker with {0} vectors", Navigation.Navigation.NP.CurrentPath.Count);

									if (FunkyGame.Bounty.CurrentBountyCacheEntry == null || FunkyGame.Bounty.CurrentBountyCacheEntry.Type != BountyTypes.Event)
										FunkyGame.Navigation.LOSBlacklistedRAGUIDs.Add(mapmarker.GetHashCode());

									//Set the object
									FunkyGame.Navigation.LOSmovementObject = new CacheLineOfSight(mapmarker);
									MinimumDistance = FunkyBaseExtension.Settings.LOSMovement.MinimumRangeMarkers;
									
								}

							}
						}
					}

					if (FunkyGame.Navigation.LOSmovementObject != null)
					{//Line of Sight unit is valid

						//See if the orgin object is still valid..

						//min Distance for Map Markers is 25f
						if (FunkyGame.Navigation.LOSmovementObject.CentreDistance < (MinimumDistance))
						{

							if (!FunkyGame.Navigation.LOSmovementObject.CacheContainsOrginObject())
							{
								Logger.Write(LogLevel.LineOfSight, "Line of Sight Ending due to Orgin Object No Longer Available!");
								FunkyGame.Navigation.LOSmovementObject = null;
								return false;
							}

							if (!FunkyGame.Navigation.LOSmovementObject.IsValidForTargeting())
							{//Valid for Targeting?
								Logger.Write(LogLevel.LineOfSight, "Line of Sight Ending due to Orgin Object Not Valid for Targeting!");
								FunkyGame.Navigation.LOSmovementObject = null;
								return false;
							}

							//Minimap Marker Check
							if (FunkyGame.Navigation.LOSmovementObject.IgnoringCacheCheck &&
								FunkyGame.Bounty.CurrentBountyCacheEntry != null && FunkyGame.Bounty.CurrentBountyCacheEntry.Type == BountyTypes.Event) //FunkyGame.Bounty.CurrentActCache!=null &&
							{
								FunkyGame.Bounty.RefreshActiveQuests();
							}

							//Update Position using Orgin Object?
							FunkyGame.Navigation.LOSmovementObject.UpdateOrginObject();
						}

						//If we had a different target.. chances are we moved, so lets reset the path.
						if (FunkyGame.Targeting.Cache.LastCachedTarget.targetType.HasValue && FunkyGame.Targeting.Cache.LastCachedTarget.targetType.Value != TargetType.LineOfSight)
						{
							Navigation.Navigation.NP.Clear();
						}

						Navigation.Navigation.NP.MoveTo(FunkyGame.Navigation.LOSmovementObject.Position, "LOS", true);
						
						if (Navigation.Navigation.NP.CurrentPath.Count > 0)
						{
							//Setup a temp target that the handler will use
							obj = new CacheObject(FunkyGame.Navigation.LOSmovementObject.Position, TargetType.LineOfSight, 1d, "LOS Movement", Navigation.Navigation.NP.PathPrecision);
							return true;
						}
					}
				}

				return false;
			};
		}
	}
}