using System;
using System.Linq;
using FunkyBot.Cache.Enums;
using FunkyBot.Cache.Objects;
using FunkyBot.Game.Bounty;
using FunkyBot.Movement;
using Zeta.Game.Internals;

namespace FunkyBot.Targeting.Behaviors
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
				return Bot.Settings.AdventureMode.EnableAdventuringMode && Bot.Game.Bounty.ActiveBounty != null && Bot.Game.Bounty.CurrentBountyCacheEntry != null;
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
					if ((Bot.Game.Bounty.CurrentBountyCacheEntry.Type == BountyQuestTypes.CursedEvent && DateTime.Now.Subtract(Bot.Targeting.Cache.lastSeenCursedShrine).TotalSeconds < 45) ||
						((Bot.Game.Bounty.CurrentBountyCacheEntry.Type == BountyQuestTypes.Kill || Bot.Game.Bounty.CurrentBountyCacheEntry.Type == BountyQuestTypes.Clear)
																								&& Bot.Character.Data.iCurrentLevelID == Bot.Game.Bounty.CurrentBountyCacheEntry.EndingLevelAreaID))
					{
						//Refresh Quest States.
						Bot.Game.Bounty.RefreshBountyQuestStates();

						//Check Current Quest State!
						if (Bot.Game.Bounty.BountyQuestStates[Bot.Game.Bounty.ActiveBounty.QuestSNO] == QuestState.Completed)
						{
							//Refresh Active Bounty to Verify there is no active bounty still!
							Bot.Game.Bounty.UpdateActiveBounty();
							if (Bot.Game.Bounty.ActiveBounty == null)
							{
								Logger.DBLog.Info("Bounty Is Finished.. Reloading Profile!!!");
								//No Active Bounty.. lets reload profile!
								Zeta.Bot.ProfileManager.Load(Zeta.Bot.ProfileManager.CurrentProfile.Path);
							}
						}
					}
					else if (Bot.Game.Bounty.CurrentBountyCacheEntry.Type == BountyQuestTypes.Event)
					{
						Bot.Game.Bounty.RefreshBountyQuestStates();
						Bot.Game.Bounty.RefreshActiveQuests();

						if (Bot.Game.Bounty.BountyQuestStates[Bot.Game.Bounty.ActiveBounty.QuestSNO] == QuestState.InProgress)
						{
							//Check interactive object cache..
							if (Bot.Game.Profile.InteractableObjectCache.Values.Any(o => o.targetType.HasValue && o.targetType.Value == TargetType.Interaction && o.CentreDistance < 75f))
							{
								Logger.DBLog.Info("Quest Giver Nearby!");

								var interactableObj = Bot.Game.Profile.InteractableObjectCache.Values.First(o => o.targetType.HasValue && o.targetType.Value == TargetType.Interaction && o.CentreDistance < 75f);

								if (!interactableObj.LineOfSight.LOSTest(Bot.Character.Data.Position, interactableObj.BotMeleeVector))
								{
									if (interactableObj.CentreDistance > 25f)
									{
										Bot.Targeting.Cache.Environment.LoSMovementObjects.Add(interactableObj);
										Bot.NavigationCache.LOSmovementObject = new CacheLineOfSight(interactableObj, interactableObj.Position);
										Navigation.NP.MoveTo(Bot.NavigationCache.LOSmovementObject.Position, "LOS", true);
										obj = new CacheObject(Bot.NavigationCache.LOSmovementObject.Position, TargetType.LineOfSight, 1d, "LOS Movement", Navigation.NP.PathPrecision);
										return true;
									}
								}
								else if (interactableObj.CentreDistance < 25f)
								{
									CacheUnit interactableUnit = (CacheUnit)interactableObj;
									if (interactableUnit.IsQuestGiver.HasValue && interactableUnit.IsQuestGiver.Value)
									{
										obj = interactableObj;
										return true;
									}
								}
							}

							if (Bot.Game.Bounty.CurrentBountyMapMarkers.Count > 0)
							{
								//Check map marker..
								foreach (var mapmarker in Bot.Game.Bounty.CurrentBountyMapMarkers.Values)
								{
									if (mapmarker.WorldID != Bot.Character.Data.CurrentWorldDynamicID) continue;

									//Check Distance
									var DistanceFromMarker = mapmarker.Position.Distance(Bot.Character.Data.Position);
									if (DistanceFromMarker < 25f)
									{
										obj = new CacheObject(Bot.Character.Data.Position, TargetType.NoMovement, 20000, "EventWait", 2f, -1);
										return true;
									}

									if (DistanceFromMarker < 100f && DistanceFromMarker > 25f)
									{
										Logger.DBLog.Info("Event Marker Nearby!");
										Bot.Game.Bounty.RefreshActiveQuests();
										obj = new CacheObject(mapmarker.Position, TargetType.LineOfSight, 20000, "Marker", 2f, -1);
										return true;
									}
								}
							}
						}
					}
				}

				return false;

			};
		}
	}
}
