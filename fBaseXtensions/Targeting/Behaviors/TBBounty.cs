using System;
using System.Linq;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Bounty;
using fBaseXtensions.Helpers;
using Zeta.Game.Internals;

namespace fBaseXtensions.Targeting.Behaviors
{
	public class TBBounty : TargetBehavior
	{
		//Monitor Current Bounty

		public TBBounty() : base() { }

		public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Bounty; } }
		public override bool BehavioralCondition
		{
			get
			{
				return FunkyBaseExtension.Settings.AdventureMode.EnableAdventuringMode && FunkyGame.Bounty.ActiveBounty != null && FunkyGame.Bounty.CurrentBountyCacheEntry != null;
			}
		}
		public override void Initialize()
		{
			base.Test = (ref CacheObject obj) =>
			{
				if (obj == null)
				{
					//	Cursed Events that have had interaction with a cursed object in the last nth time frame we check for completed bounty.
					//  Kill/Clear Events that are on the last area level we check for completed bounty.
					if ((FunkyGame.Bounty.CurrentBountyCacheEntry.Type == BountyQuestTypes.CursedEvent && DateTime.Now.Subtract(FunkyGame.Targeting.Cache.lastSeenCursedShrine).TotalSeconds < 45))
						//|| ((FunkyGame.Bounty.CurrentBountyCacheEntry.Type == BountyQuestTypes.Kill || FunkyGame.Bounty.CurrentBountyCacheEntry.Type == BountyQuestTypes.Clear)
																								//&& FunkyGame.Hero.iCurrentLevelID == FunkyGame.Bounty.CurrentBountyCacheEntry.EndingLevelAreaID))
					{
						FunkyGame.Bounty.ActiveBounty.Refresh();

						//Check Current Quest State!
						if (FunkyGame.Bounty.ActiveBounty.State == QuestState.Completed)
						{
							Logger.DBLog.Info("Bounty Is Finished.. Reloading Profile!!!");
							//Refresh Active Bounty to Verify there is no active bounty still!
							//FunkyGame.Bounty.UpdateActiveBounty();
							//if (FunkyGame.Bounty.ActiveBounty == null)
							//{
								
							//	//No Active Bounty.. lets reload profile!
							//	Zeta.Bot.ProfileManager.Load(Zeta.Bot.ProfileManager.CurrentProfile.Path);
							//}
						}
					}
					else if (FunkyGame.Bounty.CurrentBountyCacheEntry.Type == BountyQuestTypes.Event)
					{
						FunkyGame.Bounty.RefreshBountyQuestStates();
						FunkyGame.Bounty.RefreshActiveQuests();

						if (FunkyGame.Bounty.BountyQuestStates[FunkyGame.Bounty.ActiveBounty.QuestSNO] == QuestState.InProgress)
						{
							//Check interactive object cache..
							if (ObjectCache.InteractableObjectCache.Values.Any(o => o.targetType.HasValue && o.targetType.Value == TargetType.Interaction && o.CentreDistance < 75f))
							{
								Logger.DBLog.Info("Quest Giver Nearby!");

								var interactableObj = ObjectCache.InteractableObjectCache.Values.First(o => o.targetType.HasValue && o.targetType.Value == TargetType.Interaction && o.CentreDistance < 75f);

								if (!interactableObj.LineOfSight.LOSTest(FunkyGame.Hero.Position, interactableObj.BotMeleeVector))
								{
									if (interactableObj.CentreDistance > 25f)
									{
										FunkyGame.Targeting.Cache.Environment.LoSMovementObjects.Add(interactableObj);
										FunkyGame.Navigation.LOSmovementObject = new CacheLineOfSight(interactableObj, interactableObj.Position);
										Navigation.Navigation.NP.MoveTo(FunkyGame.Navigation.LOSmovementObject.Position, "LOS", true);
										obj = new CacheObject(FunkyGame.Navigation.LOSmovementObject.Position, TargetType.LineOfSight, 1d, "LOS Movement", Navigation.Navigation.NP.PathPrecision);
										return true;
									}
								}
								else if (interactableObj.CentreDistance < 25f)
								{
									CacheUnit interactableUnit = (CacheUnit)interactableObj;
									if (interactableUnit.IsQuestGiver)
									{
										obj = interactableObj;
										return true;
									}
								}
							}

							if (FunkyGame.Bounty.CurrentBountyMapMarkers.Count > 0)
							{
								//Check map marker..
								foreach (var mapmarker in FunkyGame.Bounty.CurrentBountyMapMarkers.Values)
								{
									if (mapmarker.WorldID != FunkyGame.Hero.CurrentWorldDynamicID) continue;

									//Check Distance
									var DistanceFromMarker = mapmarker.Position.Distance(FunkyGame.Hero.Position);
									if (DistanceFromMarker < 25f)
									{
										obj = new CacheObject(FunkyGame.Hero.Position, TargetType.NoMovement, 20000, "EventWait", 2f, -1);
										return true;
									}

									if (DistanceFromMarker < 100f && DistanceFromMarker > 25f)
									{
										Logger.DBLog.Info("Event Marker Nearby!");
										FunkyGame.Bounty.RefreshActiveQuests();
										obj = new CacheObject(mapmarker.Position, TargetType.LineOfSight, 20000, "Marker", 2f, -1);
										return true;
									}
								}
							}
						}
						else if (FunkyGame.Bounty.BountyQuestStates[FunkyGame.Bounty.ActiveBounty.QuestSNO]==QuestState.Completed)
						{//Bounty is Completed..

						}
					}
				}

				return false;

			};
		}
	}
}
