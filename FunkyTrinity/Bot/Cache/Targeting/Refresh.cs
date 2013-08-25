using System;
using System.Linq;
using FunkyTrinity.ability;
using FunkyTrinity.Cache;
using FunkyTrinity.Enums;
using FunkyTrinity.Movement;
using FunkyTrinity.Movement.Clustering;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;
using System.Collections.Generic;

namespace FunkyTrinity
{
	 public partial class TargetHandler
	 {
		 ///<summary>
		  ///Update our current object data ("Current Target")
		  ///</summary>
		  public bool UpdateTarget()
		  {
				TargetLogicAction[] tests=new TargetLogicAction[] {new TLA_Refresh(), new TLA_GroupingResume(), new TLA_Avoidance(), new TLA_UpdateTarget(), new TLA_Fleeing(), new TLA_Finalize(), new TLA_Grouping() };
				bool conditionTest=false;
				TargetActions lastAction=TargetActions.None;
				foreach (var TLA in tests)
				{
					 conditionTest=TLA.Test.Invoke(ref CurrentTarget);
					 if (conditionTest)
					 {
						  lastAction=TLA.TargetActionType;
						  break;
					 }
				}

				return conditionTest;
		  }
		  ///<summary>
		  ///Iterates through Usable objects and sets the Bot.CurrentTarget to the highest weighted object found inside the given list.
		  ///</summary>
		  internal void WeightEvaluationObjList(ref CacheObject CurrentTarget)
		  {
				// Store if we are ignoring all units this cycle or not
				bool bIgnoreAllUnits=!FunkyTrinity.Bot.Combat.bAnyChampionsPresent
											&&!FunkyTrinity.Bot.Combat.bAnyMobsInCloseRange
											&&((!FunkyTrinity.Bot.Combat.bAnyTreasureGoblinsPresent&&FunkyTrinity.Bot.SettingsFunky.GoblinPriority>=2)||FunkyTrinity.Bot.SettingsFunky.GoblinPriority<2)
											&&FunkyTrinity.Bot.Character.dCurrentHealthPct>=0.85d;


				//clear our last "avoid" list..
				ObjectCache.Objects.objectsIgnoredDueToAvoidance.Clear();

				double iHighestWeightFound=0;

				foreach (CacheObject thisobj in FunkyTrinity.Bot.ValidObjects)
				{
					 thisobj.UpdateWeight();

					 if (thisobj.Weight==1)
					 {
						  // Force the character to stay where it is if there is nothing available that is out of avoidance stuff and we aren't already in avoidance stuff
						  thisobj.Weight=0;
						  if (!FunkyTrinity.Bot.Combat.RequiresAvoidance) FunkyTrinity.Bot.Combat.bStayPutDuringAvoidance=true;
						  continue;
					 }

					 // Is the weight of this one higher than the current-highest weight? Then make this the new primary target!
					 if (thisobj.Weight>iHighestWeightFound&&thisobj.Weight>0)
					 {
						  //Check combat looting (Demonbuddy Setting)
						  if (iHighestWeightFound>0
												&&thisobj.targetType.Value==TargetType.Item
												&&!Zeta.CommonBot.Settings.CharacterSettings.Instance.CombatLooting
												&&CurrentTarget.targetType.Value==TargetType.Unit) continue;


						  //cache RAGUID so we can switch back if we need to
						  int CurrentTargetRAGUID=CurrentTarget!=null?CurrentTarget.RAGUID:-1;

						  //Set our current target to this object!
						  CurrentTarget=ObjectCache.Objects[thisobj.RAGUID];

						  bool resetTarget=false;
						  //Check for Range Classes and Unit Targets
						  if (!FunkyTrinity.Bot.Class.IsMeleeClass&&CurrentTarget.targetType.Value==TargetType.Unit&&FunkyTrinity.Bot.Combat.NearbyAvoidances.Count>0)
						  {
								//set unit target (for Ability selector).
								CurrentUnitTarget=(CacheUnit)CurrentTarget;

								//Generate next Ability..
								Ability nextAbility=FunkyTrinity.Bot.Class.AbilitySelector();

								//reset unit target
								CurrentUnitTarget=null;

								//Check if we are already within interaction range.
								if (!thisobj.WithinInteractionRange())
								{
									 Vector3 destinationV3=nextAbility.DestinationVector;
									 //Check if the estimated destination will also be inside avoidance zone..
									 if (ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(destinationV3)
										 ||ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(destinationV3))
									 {
										  //Only wait if the object is special and we are not avoiding..
										  if (thisobj.ObjectIsSpecial)
										  {
												if (!FunkyTrinity.Bot.Combat.RequiresAvoidance)
												{
													 FunkyTrinity.Bot.Combat.bStayPutDuringAvoidance=true;
													 resetTarget=true;
												}
												else if (!nextAbility.IsRanged&&nextAbility.Range>0)
												{
													 //Non-Ranged Ability.. act like melee..
													 //Try to find a spot
													 ObjectCache.Objects.objectsIgnoredDueToAvoidance.Add(thisobj);
												}
										  }
										  else
												resetTarget=true;
									 }
								}
						  }

						  //Avoidance Attempt to find a location where we can attack!
						  if (ObjectCache.Objects.objectsIgnoredDueToAvoidance.Contains(thisobj))
						  {
								//Wait if no valid target found yet.. and no avoidance movement required.
								if (!FunkyTrinity.Bot.Combat.RequiresAvoidance)
									 FunkyTrinity.Bot.Combat.bStayPutDuringAvoidance=true;

								//Check Bot Navigationally blocked
								FunkyTrinity.Bot.NavigationCache.RefreshNavigationBlocked();
								if (!FunkyTrinity.Bot.NavigationCache.BotIsNavigationallyBlocked)
								{
									 Vector3 SafeLOSMovement;
									 if (thisobj.GPRect.TryFindSafeSpot(FunkyTrinity.Bot.Character.Position, out SafeLOSMovement, Vector3.Zero, FunkyTrinity.Bot.Character.ShouldFlee, true))
									 {
										  CurrentTarget=new CacheObject(SafeLOSMovement, TargetType.Avoidance, 20000, "SafetyMovement", 2.5f, -1);
										  //Reset Avoidance Timer so we don't trigger it while moving towards the target!
										  FunkyTrinity.Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
										  FunkyTrinity.Bot.Combat.iMillisecondsCancelledEmergencyMoveFor=1000+((int)(FunkyTrinity.Bot.Target.CurrentTarget.CentreDistance/25f)*1000);
									 }
									 else
									 {
										  resetTarget=true;
									 }
								}
						  }

						  if (resetTarget)
						  {
								CurrentTarget=CurrentTargetRAGUID!=-1?ObjectCache.Objects[CurrentTargetRAGUID]:null;
								continue;
						  }

						  iHighestWeightFound=thisobj.Weight;
					 }

				} // Loop through all the objects and give them a weight
		  }


	 }
}
