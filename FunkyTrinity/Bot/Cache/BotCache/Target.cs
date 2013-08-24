using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.TreeSharp;
using System.Collections.Generic;
using Zeta.Internals.SNO;
using Zeta.CommonBot;
using System.Globalization;
using FunkyTrinity.Enums;
using FunkyTrinity.ability;
using FunkyTrinity.Cache;
using FunkyTrinity.Movement;
using FunkyTrinity.Movement.Clustering;

namespace FunkyTrinity
{


			public class TargetHandler
			{
				 //Constructor
				 public TargetHandler()
				 {
						CurrentState=RunStatus.Running;
						CurrentTarget=null;
				 }


				 ///<summary>
				 ///This method handles the current object target.
				 ///</summary>
				 public RunStatus HandleThis()
				 {

						//Prechecks
						bool Continue=PreChecks();


						//Refresh
						if (!Continue)
							 return CurrentState;
						else
							 Continue=Refresh();


						//Combat logic
						if (!Continue)
							 return CurrentState;
						else
							 Continue=CombatLogic();


						//Movement
						if (!Continue)
							 return CurrentState;
						else
							 Continue=Movement();


						//Interaction
						if (!Continue)
							 return CurrentState;
						else
							 Continue=ObjectInteraction();


						//Return status
						return CurrentState;
				 }

				 //The current state which is used to return from the handler
				 public RunStatus CurrentState { get; set; }

				 //This is the object that is our target which is the data used in methods.
				 //This data is continiously updated by RefreshDiaObjects.
				 ///<summary>
				 ///This should reference Cached Data.
				 ///Used throughout the code as the Bot.CurrentTarget data.
				 ///This must be set to a valid cacheobject in order to properly handle it.
				 ///</summary>
				 public CacheObject CurrentTarget;

				 //Used to reduce additional unboxing when target is an unit.
				 internal CacheUnit CurrentUnitTarget;

				 ///<summary>
				 ///Update our current object data ("Current Target")
				 ///</summary>
				 public bool UpdateTarget()
				 {
						//Generate a vaild object list using our cached collection!
						Bot.ValidObjects=ObjectCache.Objects.Values.Where(obj => obj.ObjectIsValidForTargeting).ToList();

						//Check avoidance requirement still valid
						if (Bot.Combat.RequiresAvoidance)
						{
							 if (Bot.Combat.TriggeringAvoidances.Count==0)
							 {
								  if (!Bot.SettingsFunky.EnableFleeingBehavior||Bot.Character.dCurrentHealthPct>0.25d)
										Bot.Combat.RequiresAvoidance=false;
							 }
							 //else
							 //{
							 //	//Triggering avoidance means we are inside something.
							 //	//If we are moving.. then we will ignore it this time!
							 //	 if (Bot.NavigationCache.IsMoving)
							 //		  Bot.Combat.RequiresAvoidance=false;
							 //}
						}
									


						#region Grouping Behavior Resume

						if (Bot.NavigationCache.groupRunningBehavior)
						{
							 if (!Bot.NavigationCache.groupReturningToOrgin)
							 {
								  Bot.Combat.UpdateGroupClusteringVariables();

								  bool EndBehavior=false;
								  if (!Bot.NavigationCache.groupingCurrentUnit.ObjectIsValidForTargeting)
								  {
										if (Bot.SettingsFunky.LogGroupingOutput)
											 Logging.WriteVerbose("[Grouping] Target is no longer valid. Starting return to Orgin.");

										EndBehavior=true;
								  }
								  else if (Bot.NavigationCache.groupingCurrentUnit.CurrentHealthPct.Value<1d
										&&Bot.NavigationCache.groupingCurrentUnit.IsMoving)
								  {
										if (Bot.SettingsFunky.LogGroupingOutput)
											 Logging.WriteVerbose("[Grouping] Target has been engaged. Starting return to Orgin.");

										EndBehavior=true;
								  }


								  if (!EndBehavior)
								  {
										CurrentTarget=Bot.NavigationCache.groupingCurrentUnit;
										return true;
								  }
								  else
								  {
										Bot.NavigationCache.groupingCurrentUnit=null;
										Bot.NavigationCache.groupReturningToOrgin=true;
										CurrentTarget=Bot.NavigationCache.groupingOrginUnit;
										return true;
								  }


							 }
							 else
							 {
								  bool endBehavior=false;

								  //Returning to Orgin Unit..
								  if (!Bot.NavigationCache.groupingOrginUnit.ObjectIsValidForTargeting)
								  {
										endBehavior=true;

										if (Bot.SettingsFunky.LogGroupingOutput)
											 Logging.WriteVerbose("[Grouping] Orgin Target is no longer valid for targeting.");
								  }
								  else if (Bot.NavigationCache.groupingOrginUnit.CentreDistance<(Bot.Class.IsMeleeClass?25f:45f))
								  {
										if (Bot.SettingsFunky.LogGroupingOutput)
											 Logging.WriteVerbose("[Grouping] Orgin Target is within {0}f of the bot.", (Bot.Class.IsMeleeClass?25f:45f).ToString());

										endBehavior=true;
								  }

								  if (!endBehavior)
								  {
										CurrentTarget=Bot.NavigationCache.groupingOrginUnit;
										return true;
								  }
								  else
										Bot.NavigationCache.GroupingFinishBehavior();
							 }
						} 
						#endregion


						Vector3 LOS=Vector3.Zero;

						//Check if we require avoidance movement.
						#region AvodianceMovementCheck

						if (Bot.Combat.RequiresAvoidance&&(!Bot.Combat.bAnyTreasureGoblinsPresent||Bot.SettingsFunky.GoblinPriority<2)
							&&(DateTime.Now.Subtract(Bot.Combat.timeCancelledEmergencyMove).TotalMilliseconds>Bot.Combat.iMillisecondsCancelledEmergencyMoveFor))
						{

							 //Reuse the last generated safe spot...
							 if (DateTime.Now.Subtract(Bot.Combat.LastAvoidanceMovement).TotalMilliseconds>=
									 Bot.Combat.iSecondsEmergencyMoveFor)
							 {
									Vector3 reuseV3=Bot.NavigationCache.AttemptToReuseLastLocationFound();
									if (reuseV3!=Vector3.Zero)
									{

										 CurrentTarget=new CacheObject(reuseV3, TargetType.Avoidance, 20000f, "SafeAvoid", 2.5f, -1);
										 return true;

									}
							 }

							 Vector3 vAnySafePoint;
							 //if (CurrentTarget!=null&&CurrentTarget.targetType.HasValue&&TargetType.ServerObjects.HasFlag(CurrentTarget.targetType.Value))
							 //    LOS=CurrentTarget.Position;
							 //else
							 //    LOS=Vector3.Zero;

							 if (Bot.NavigationCache.AttemptFindSafeSpot(out vAnySafePoint, LOS, Bot.Character.ShouldFlee))
							 {
									float distance=vAnySafePoint.Distance(Bot.Character.Position);

									float losdistance=0f;
									if (LOS!=Vector3.Zero) losdistance=LOS.Distance(Bot.Character.Position);

									string los=LOS!=Vector3.Zero?("\r\n using LOS location "+LOS.ToString()+" distance "+losdistance.ToString()):" ";

									Logging.WriteDiagnostic("Avoid Movement found AT {0} with {1} Distance", vAnySafePoint.ToString(), distance);
									//bFoundSafeSpot = true;

									//setup avoidance target
									if (CurrentTarget!=null) Bot.Combat.LastCachedTarget=CurrentTarget.Clone();
									CurrentTarget=new CacheObject(vAnySafePoint, TargetType.Avoidance, 20000f, "SafeAvoid", 2.5f, -1);

									//Estimate time we will be reusing this movement vector3
									Bot.Combat.iSecondsEmergencyMoveFor=1+(int)(distance/25f);

									//Avoidance takes priority over kiting..
									Bot.Combat.timeCancelledFleeMove=DateTime.Now;
									Bot.Combat.iMillisecondsCancelledFleeMoveFor=((Bot.Combat.iSecondsEmergencyMoveFor+1)*1000);

									return true;
							 }

							 Bot.UpdateAvoidKiteRates();
						}
						#endregion

						Bot.Combat.bStayPutDuringAvoidance=false;

						//cluster update
						Bot.Combat.UpdateTargetClusteringVariables();

						//Standard weighting of valid objects -- updates current target.
						this.WeightEvaluationObjList();

						//check Fleeing
						#region Fleeing
						if (Bot.SettingsFunky.EnableFleeingBehavior&&Bot.Character.dCurrentHealthPct<=Bot.SettingsFunky.FleeBotMinimumHealthPercent&&Bot.Combat.FleeTriggeringUnits.Count>0
							 &&(DateTime.Now.Subtract(Bot.Combat.timeCancelledFleeMove).TotalMilliseconds>Bot.Combat.iMillisecondsCancelledFleeMoveFor)
							 &&(!Bot.Combat.bAnyTreasureGoblinsPresent||Bot.SettingsFunky.GoblinPriority<2)
							 &&(Bot.Class.AC!=ActorClass.Wizard||(Bot.Class.AC==ActorClass.Wizard&&(!Bot.Class.HasBuff(SNOPower.Wizard_Archon)||!Bot.SettingsFunky.Class.bKiteOnlyArchon))))
						{

							 //Resuse last safespot until timer expires!
							 if (DateTime.Now.Subtract(Bot.Combat.LastFleeAction).TotalSeconds<Bot.Combat.iSecondsFleeMoveFor)
							 {
									Vector3 reuseV3=Bot.NavigationCache.AttemptToReuseLastLocationFound();
									if (reuseV3!=Vector3.Zero)
									{
										 CurrentTarget=new CacheObject(reuseV3, TargetType.Avoidance, 20000f, "FleeSpot", 2.5f, -1);
										 return true;
									}
							 }

							 if (CurrentTarget!=null&&CurrentTarget.targetType.HasValue&&TargetType.ServerObjects.HasFlag(CurrentTarget.targetType.Value)
								 &&(Bot.NavigationCache.CurrentGPArea==null||!Bot.NavigationCache.CurrentGPArea.AllGPRectsFailed))
									LOS=CurrentTarget.Position;
							 else
									LOS=Vector3.Zero;

							 Vector3 vAnySafePoint;
							 if (Bot.NavigationCache.AttemptFindSafeSpot(out vAnySafePoint, LOS, true))
							 {
									Logging.WriteDiagnostic("Flee Movement found AT {0} with {1} Distance", vAnySafePoint.ToString(), vAnySafePoint.Distance(Bot.Character.Position));
									Bot.Combat.IsFleeing=true;

									if (CurrentTarget!=null)
										 Bot.Character.LastCachedTarget=CurrentTarget;

									CurrentTarget=new CacheObject(vAnySafePoint, TargetType.Avoidance, 20000f, "FleeSpot", 2.5f, -1);

									Bot.Combat.iSecondsFleeMoveFor=1+(int)(Vector3.Distance(Bot.Character.Position, vAnySafePoint)/25f);
									return true;
							 }
							 Bot.UpdateAvoidKiteRates();

						}

						//If we have a cached kite target.. and no current target, lets swap back!
						if (Bot.Combat.FleeingLastTarget&&CurrentTarget==null
									 &&Bot.Character.LastCachedTarget!=null
									 &&ObjectCache.Objects.ContainsKey(Bot.Character.LastCachedTarget.RAGUID))
						{
							 //Swap back to our orginal "kite" target
							 CurrentTarget=ObjectCache.Objects[Bot.Character.LastCachedTarget.RAGUID];
							 Logging.WriteVerbose("Swapping back to unit {0} after fleeing", CurrentTarget.InternalName);
							 return true;
						}
						#endregion


						// No valid targets but we were told to stay put?
						if (CurrentTarget==null&&Bot.Combat.bStayPutDuringAvoidance)
						{
							 if (Bot.Combat.TriggeringAvoidances.Count==0)
							 {
									CurrentTarget=new CacheObject(Bot.Character.Position, TargetType.Avoidance, 20000, "StayPutPoint", 2.5f, -1);
									return true;
							 }
							 else
									Bot.Combat.iMillisecondsCancelledEmergencyMoveFor=0; //reset wait time
						}

						//Final Possible Target Check
						if (CurrentTarget==null)
						{
							 // See if we should wait for milliseconds for possible loot drops before continuing run
							 if (DateTime.Now.Subtract(Bot.Combat.lastHadUnitInSights).TotalMilliseconds<=Bot.SettingsFunky.AfterCombatDelay&&DateTime.Now.Subtract(Bot.Combat.lastHadEliteUnitInSights).TotalMilliseconds<=10000||
									//Cut the delay time in half for non-elite monsters!
								 DateTime.Now.Subtract(Bot.Combat.lastHadUnitInSights).TotalMilliseconds<=Bot.SettingsFunky.AfterCombatDelay)
							 {
									CurrentTarget=new CacheObject(Bot.Character.Position, TargetType.Avoidance, 20000, "WaitForLootDrops", 2f, -1);
									return true;
							 }
							 //Herbfunks wait after loot containers are opened. 3s for rare chests, half the settings delay for everything else.
							 if ((DateTime.Now.Subtract(Bot.Combat.lastHadRareChestAsTarget).TotalMilliseconds<=3750)||
								 (DateTime.Now.Subtract(Bot.Combat.lastHadContainerAsTarget).TotalMilliseconds<=(Bot.SettingsFunky.AfterCombatDelay*1.25)))
							 {
									CurrentTarget=new CacheObject(Bot.Character.Position, TargetType.Avoidance, 20000, "ContainerLootDropsWait", 2f, -1);
									return true;
							 }

							 // Finally, a special check for waiting for wrath of the berserker cooldown before engaging Azmodan
							 if (Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_WrathOfTheBerserker)&&Bot.SettingsFunky.Class.bWaitForWrath&&!Bot.Class.AbilityUseTimer(SNOPower.Barbarian_WrathOfTheBerserker)&&
								 ZetaDia.CurrentWorldId==121214&&
								 (Vector3.Distance(Bot.Character.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(Bot.Character.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
							 {
									Logging.Write("[Funky] Waiting for Wrath Of The Berserker cooldown before continuing to Azmodan.");
									CurrentTarget=new CacheObject(Bot.Character.Position, TargetType.Avoidance, 20000, "GilesWaitForWrath", 0f, -1);
									InactivityDetector.Reset();
									return true;
							 }
							 // And a special check for wizard archon
							 if (Bot.Class.HotbarPowers.Contains(SNOPower.Wizard_Archon)&&!Bot.Class.AbilityUseTimer(SNOPower.Wizard_Archon)&&Bot.SettingsFunky.Class.bWaitForArchon&&ZetaDia.CurrentWorldId==121214&&
								 (Vector3.Distance(Bot.Character.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(Bot.Character.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
							 {
									Logging.Write("[Funky] Waiting for Wizard Archon cooldown before continuing to Azmodan.");
									CurrentTarget=new CacheObject(Bot.Character.Position, TargetType.Avoidance, 20000, "GilesWaitForArchon", 0f, -1);
									InactivityDetector.Reset();
									return true;
							 }
							 // And a very sexy special check for WD BigBadVoodoo
							 if (Bot.Class.HotbarPowers.Contains(SNOPower.Witchdoctor_BigBadVoodoo)&&!PowerManager.CanCast(SNOPower.Witchdoctor_BigBadVoodoo)&&ZetaDia.CurrentWorldId==121214&&
								 (Vector3.Distance(Bot.Character.Position, new Vector3(711.25f, 716.25f, 80.13903f))<=40f||Vector3.Distance(Bot.Character.Position, new Vector3(546.8467f, 551.7733f, 1.576313f))<=40f))
							 {
									Logging.Write("[Funky] Waiting for WD BigBadVoodoo cooldown before continuing to Azmodan.");
									CurrentTarget=new CacheObject(Bot.Character.Position, TargetType.Avoidance, 20000, "GilesWaitForVoodooo", 0f, -1);
									InactivityDetector.Reset();
									return true;
							 }


							 //Check if our current path intersects avoidances. (When not in town, and not currently inside avoidance)
							 if (!Bot.Character.bIsInTown&&(Bot.SettingsFunky.AttemptAvoidanceMovements||Bot.Combat.CriticalAvoidance)
									 &&Navigation.NP.CurrentPath.Count>0
									 &&Bot.Combat.TriggeringAvoidances.Count==0)
							 {
									Vector3 curpos=Bot.Character.Position;
									IndexedList<Vector3> curpath=Navigation.NP.CurrentPath;

									var CurrentNearbyPath=curpath.Where(v => curpos.Distance(v)<=40f);
									if (CurrentNearbyPath!=null&&CurrentNearbyPath.Any())
									{
										 CurrentNearbyPath.OrderBy(v => curpath.IndexOf(v));

										 Vector3 lastV3=Vector3.Zero;
										 foreach (var item in CurrentNearbyPath)
										 {
												if (lastV3==Vector3.Zero)
													 lastV3=item;
												else if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(item, lastV3))
												{
													 CurrentTarget=new CacheObject(Bot.Character.Position, TargetType.Avoidance, 20000, "AvoidanceIntersection", 2.5f, -1);
													 return true;
												}
										 }
									}
							 }
						}
						else
						{
							 if (CurrentTarget.targetType.Equals(TargetType.Unit))
							 {
									//Update CurrentUnitTarget Variable.
									if (CurrentUnitTarget==null) CurrentUnitTarget=(CacheUnit)CurrentTarget;

									//Grouping Movements
									if (Bot.SettingsFunky.AttemptGroupingMovements
										 &&CurrentUnitTarget.CurrentHealthPct.Value<1d
										 &&DateTime.Compare(DateTime.Now,Bot.NavigationCache.groupingSuspendedDate)>0
										 &&!CurrentUnitTarget.IsTreasureGoblin||Bot.SettingsFunky.GoblinPriority<2) //only after we engaged the target.
									{
										 Bot.Combat.UpdateGroupClusteringVariables();



										 if (Bot.Combat.CurrentGroupClusters.Count>0)
										 {
											  UnitCluster currentTargetCluster=CurrentUnitTarget.CurrentTargetCluster;
												if (currentTargetCluster!=null)
												{
													 bool ShouldTriggerBehavior=(!Bot.SettingsFunky.IgnoreAboveAverageMobs&&currentTargetCluster.Info.Properties.HasFlag(ClusterProperties.Elites)||
																						  currentTargetCluster.Info.Properties.HasFlag(ClusterProperties.Large)||
																						  currentTargetCluster.Info.Properties.HasFlag(ClusterProperties.Strong)||
																						  currentTargetCluster.Info.Properties.HasFlag(ClusterProperties.Fast));

													 //Trigger for grouping..
													 if (ShouldTriggerBehavior)
													 {
														  var PossibleGroups=Bot.Combat.CurrentGroupClusters
																	 .Where(c =>
																		  (!Bot.SettingsFunky.IgnoreAboveAverageMobs&&c.Info.Properties.HasFlag(ClusterProperties.Elites))||
																		  c.Info.Properties.HasFlag(ClusterProperties.Large)||
																		  c.Info.Properties.HasFlag(ClusterProperties.Strong));


															if (PossibleGroups.Any())
															{
																 if (Bot.SettingsFunky.LogGroupingOutput)
																	 Logging.WriteVerbose("Starting Grouping Behavior");

																 //Activate Behavior
																 Bot.NavigationCache.groupRunningBehavior=true;
																 Bot.NavigationCache.groupingOrginUnit=(CacheUnit)ObjectCache.Objects[CurrentTarget.RAGUID];

																 //Get Cluster
																 UnitCluster groupCluster=PossibleGroups.First();
																 Bot.NavigationCache.groupingCurrentCluster=groupCluster;

																 if (Bot.SettingsFunky.LogGroupingOutput)
																	  Logging.WriteVerbose("Group Cluster Propeties {0}" , groupCluster.Info.Properties.ToString());

																 //Find initial grouping target..
																 CurrentTarget=groupCluster.ListUnits[0];
																 CurrentUnitTarget=(CacheUnit)CurrentTarget;
																 Bot.NavigationCache.groupingCurrentUnit=CurrentUnitTarget;
															}
													 }
												}
										 }
									}

							 }


							 return true;
						}


						return false;
				 }
				 ///<summary>
				 ///Iterates through Usable objects and sets the Bot.CurrentTarget to the highest weighted object found inside the given list.
				 ///</summary>
				 private void WeightEvaluationObjList()
				 {
						// Store if we are ignoring all units this cycle or not
						bool bIgnoreAllUnits=!Bot.Combat.bAnyChampionsPresent
													&&!Bot.Combat.bAnyMobsInCloseRange
													&&((!Bot.Combat.bAnyTreasureGoblinsPresent&&Bot.SettingsFunky.GoblinPriority>=2)||Bot.SettingsFunky.GoblinPriority<2)
													&&Bot.Character.dCurrentHealthPct>=0.85d;


						//clear our last "avoid" list..
						ObjectCache.Objects.objectsIgnoredDueToAvoidance.Clear();

						double iHighestWeightFound=0;

						foreach (CacheObject thisobj in Bot.ValidObjects)
						{
							 thisobj.UpdateWeight();

							 if (thisobj.Weight==1)
							 {
									// Force the character to stay where it is if there is nothing available that is out of avoidance stuff and we aren't already in avoidance stuff
									thisobj.Weight=0;
									if (!Bot.Combat.RequiresAvoidance) Bot.Combat.bStayPutDuringAvoidance=true;
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
									if (!Bot.Class.IsMeleeClass&&CurrentTarget.targetType.Value==TargetType.Unit&&Bot.Combat.NearbyAvoidances.Count>0)
									{
										 //set unit target (for Ability selector).
										 CurrentUnitTarget=(CacheUnit)CurrentTarget;

										 //Generate next Ability..
										 Ability nextAbility=Bot.Class.AbilitySelector();

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
															if (!Bot.Combat.RequiresAvoidance)
															{
																 Bot.Combat.bStayPutDuringAvoidance=true;
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
										 if (!Bot.Combat.RequiresAvoidance)
												Bot.Combat.bStayPutDuringAvoidance=true;

										 //Check Bot Navigationally blocked
										 Bot.NavigationCache.RefreshNavigationBlocked();
										 if (!Bot.NavigationCache.BotIsNavigationallyBlocked)
										 {
												Vector3 SafeLOSMovement;
												if (thisobj.GPRect.TryFindSafeSpot(Bot.Character.Position, out SafeLOSMovement, Vector3.Zero, Bot.Character.ShouldFlee, true))
												{
													 CurrentTarget=new CacheObject(SafeLOSMovement, TargetType.Avoidance, 20000, "SafetyMovement", 2.5f, -1);
													 //Reset Avoidance Timer so we don't trigger it while moving towards the target!
													 Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
													 Bot.Combat.iMillisecondsCancelledEmergencyMoveFor=1000+((int)(Bot.Target.CurrentTarget.CentreDistance/25f)*1000);
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


				 //Prechecks are things prior to target checks and actual target handling.. This is always called first.
				 public virtual bool PreChecks()
				 {
						// If we aren't in the game of a world is loading, don't do anything yet
						if (!ZetaDia.IsInGame||ZetaDia.IsLoadingWorld)
						{
							 CurrentState=RunStatus.Success;
							 return false;
						}

						// See if we should update hotbar abilities
						Bot.Class.SecondaryHotbarBuffPresent();


						// Special pausing *AFTER* using certain powers
						#region PauseCheck
						if (Bot.Combat.bWaitingAfterPower&&Bot.Combat.powerPrime.WaitLoopsAfter>=1)
						{
							 if (Bot.Combat.powerPrime.WaitLoopsAfter>=1) Bot.Combat.powerPrime.WaitLoopsAfter--;
							 if (Bot.Combat.powerPrime.WaitLoopsAfter<=0) Bot.Combat.bWaitingAfterPower=false;

							 CurrentState=RunStatus.Running;
							 return false;
						}
						#endregion

						// Update player-data cache -- Special combat call
						Bot.Character.Update(true);

						// Check for death / player being dead
						#region DeadCheck
						if (Bot.Character.dCurrentHealthPct<=0)
						{
							 //Disable OOC IDing behavior if dead!
							 if (Funky.shouldPreformOOCItemIDing) Funky.shouldPreformOOCItemIDing=false;

							 CurrentState=RunStatus.Success;
							 return false;
						}
						#endregion

						//Herbfunk
						//Confirmation of item looted
						#region ItemLootedConfirmationCheck
						if (Bot.Combat.ShouldCheckItemLooted)
						{
							 //Reset?
							 if (CurrentTarget==null||CurrentTarget.targetType.HasValue&&CurrentTarget.targetType.Value!=TargetType.Item)
							 {
									Bot.Combat.ShouldCheckItemLooted=false;
									return false;
							 }

							 //Vendor Behavior
							 if (Zeta.CommonBot.Logic.BrainBehavior.IsVendoring)
							 {
									CurrentState=RunStatus.Success;
									return false;
							 }
							 else if (Bot.Character.bIsIncapacitated)
							 {
									CurrentState=RunStatus.Running;
									return false;
							 }

							 //Count each attempt to confirm.
							 Bot.Combat.recheckCount++;
							 string statusText="[Item Confirmation] Current recheck count "+Bot.Combat.recheckCount;
							 bool LootedSuccess=Bot.Character.BackPack.ContainsItem(CurrentTarget.AcdGuid.Value);
							 //Verify item is non-stackable!

							 statusText+=" [ItemFound="+LootedSuccess+"]";
							 if (LootedSuccess)
							 {
									Zeta.CommonBot.GameEvents.FireItemLooted(CurrentTarget.AcdGuid.Value);

									if (Bot.SettingsFunky.DebugStatusBar) BotMain.StatusText=statusText;

									//This is where we should manipulate information of both what dropped and what was looted.
									Funky.LogItemInformation();

									//Reset if we reach here..
									Bot.Combat.reCheckedFinished=false;
									Bot.Combat.recheckCount=0;
									Bot.Combat.ShouldCheckItemLooted=false;
									Bot.Combat.bForceTargetUpdate=true;
							 }
							 else
							 {
									CacheItem thisObjItem=(CacheItem)CurrentTarget;

									statusText+=" [Quality";
									//Quality of the item determines the recheck attempts.
									ItemQuality curQuality=thisObjItem.Itemquality.Value;
									#region QualityRecheckSwitch
									switch (curQuality)
									{
										 case ItemQuality.Inferior:
										 case ItemQuality.Invalid:
										 case ItemQuality.Special:
										 case ItemQuality.Superior:
										 case ItemQuality.Normal:
										 case ItemQuality.Magic1:
										 case ItemQuality.Magic2:
										 case ItemQuality.Magic3:
												statusText+="<=Magical]";
												//Non-Quality items get skipped quickly.
												if (Bot.Combat.recheckCount>1)
													 Bot.Combat.reCheckedFinished=true;
												break;

										 case ItemQuality.Rare4:
										 case ItemQuality.Rare5:
										 case ItemQuality.Rare6:
												statusText+="=Rare]";
												if (Bot.Combat.recheckCount>2)
													 Bot.Combat.reCheckedFinished=true;
												//else
												//bItemForcedMovement = true;

												break;

										 case ItemQuality.Legendary:
												statusText+="=Legendary]";
												if (Bot.Combat.recheckCount>3)
													 Bot.Combat.reCheckedFinished=true;
												//else
												//bItemForcedMovement = true;

												break;
									}
									#endregion

									//If we are still rechecking then use the waitAfter (powerprime Ability related) to wait a few loops.
									if (!Bot.Combat.reCheckedFinished)
									{
										 statusText+=" RECHECKING";
										 if (Bot.SettingsFunky.DebugStatusBar)
										 {
												BotMain.StatusText=statusText;
										 }
										 Bot.Combat.bWaitingAfterPower=true;
										 Bot.Combat.powerPrime.WaitLoopsAfter=3;
										 CurrentState=RunStatus.Running;
										 return false;
									}
									else
									{
										 //We Rechecked Max Confirmation Checking Count, now we check if we want to retry confirmation, or simply try once more then ignore for a few.
										 bool stackableItem=(ItemType.Potion|ItemType.CraftingPage|ItemType.CraftingPlan|ItemType.CraftingReagent).HasFlag(thisObjItem.BalanceData.thisItemType);
										 if (thisObjItem.Itemquality.Value>ItemQuality.Magic3||stackableItem)
										 {
												//Items above rare quality don't get blacklisted, just ignored for a few loops.
												//This will force a movement if stuck.. but 5 loops is only 750ms
												CurrentTarget.BlacklistLoops=5;
										 }
										 else
										 {
												//Blacklist items below rare quality!
												CurrentTarget.BlacklistFlag=BlacklistType.Temporary;
												CurrentTarget.NeedsRemoved=true;
										 }

										 // Now tell Trinity to get a new target!
										 Bot.Combat.bForceTargetUpdate=true;
									}
							 }

							 //Reset flag, and continue..
							 Bot.Combat.ShouldCheckItemLooted=false;
						}
						#endregion


						// See if we have been "newly rooted", to force target updates
						if (Bot.Character.bIsRooted&&!Bot.Combat.bWasRootedLastTick)
						{
							 Bot.Combat.bWasRootedLastTick=true;
							 Bot.Combat.bForceTargetUpdate=true;
						}

						if (!Bot.Character.bIsRooted) Bot.Combat.bWasRootedLastTick=false;

						return true;
				 }

				 //This is the 2nd step in handling.. we recheck target, get a new Ability if needed, and check potion/special movement avoidance here.
				 public virtual bool Refresh()
				 {
						// Make sure we reset unstucker stuff here
						Funky.PlayerMover.iTimesReachedStuckPoint=0;
						Funky.PlayerMover.vSafeMovementLocation=Vector3.Zero;
						Funky.PlayerMover.timeLastRecordedPosition=DateTime.Now;

						// Let's calculate whether or not we want a new target list...
						#region NewtargetChecks
						// Whether we should refresh the target list or not
						bool bShouldRefreshDiaObjects=false;

						if (!Bot.Combat.bWholeNewTarget&&!Bot.Combat.bWaitingForPower&&!Bot.Combat.bWaitingForPotion)
						{
							 // Update targets at least once every 80 milliseconds
							 if (Bot.Combat.bForceTargetUpdate
								 ||Bot.Combat.TravellingAvoidance
								 ||((DateTime.Now.Subtract(Bot.Refresh.lastRefreshedObjects).TotalMilliseconds>=80&&CurrentTarget.targetType.Value!=TargetType.Avoidance)
								 ||DateTime.Now.Subtract(Bot.Refresh.lastRefreshedObjects).TotalMilliseconds>=1200))
							 {
									bShouldRefreshDiaObjects=true;
							 }

							 // If we AREN'T getting new targets - find out if we SHOULD because the current unit has died etc.
							 if (!bShouldRefreshDiaObjects&&CurrentTarget.targetType.Value==TargetType.Unit&&!CurrentTarget.IsStillValid())
									bShouldRefreshDiaObjects=true;

						}

						// So, after all that, do we actually want a new target list?
						if (!Bot.Combat.bWholeNewTarget&&!Bot.Combat.bWaitingForPower&&!Bot.Combat.bWaitingForPotion)
						{
							 // If we *DO* want a new target list, do this... 
							 if (bShouldRefreshDiaObjects)
							 {
									// Now call the function that refreshes targets
								  Bot.Refresh.RefreshDiaObjects();

									// No target, return success
									if (CurrentTarget==null)
									{
										 CurrentState=RunStatus.Success;
										 return false;
									}
									else if (Bot.Character.LastCachedTarget!=null&&
										 Bot.Character.LastCachedTarget.RAGUID!=CurrentTarget.RAGUID&&CurrentTarget.targetType.Value==TargetType.Item)
									{
										 //Reset Item Vars
										 Bot.Combat.recheckCount=0;
										 Bot.Combat.reCheckedFinished=false;
									}

									// Been trying to handle the same target for more than 30 seconds without damaging/reaching it? Blacklist it!
									// Note: The time since target picked updates every time the current target loses health, if it's a monster-target
									if (CurrentTarget.targetType.Value!=TargetType.Avoidance
										 &&((CurrentTarget.targetType.Value!=TargetType.Unit&&DateTime.Now.Subtract(Bot.Combat.dateSincePickedTarget).TotalSeconds>12)
										 ||(CurrentTarget.targetType.Value==TargetType.Unit&&!CurrentTarget.IsBoss&&DateTime.Now.Subtract(Bot.Combat.dateSincePickedTarget).TotalSeconds>40)))
									{
										 // NOTE: This only blacklists if it's remained the PRIMARY TARGET that we are trying to actually directly attack!
										 // So it won't blacklist a monster "on the edge of the screen" who isn't even being targetted
										 // Don't blacklist monsters on <= 50% health though, as they can't be in a stuck location... can they!? Maybe give them some extra time!
										 bool bBlacklistThis=true;
										 // PREVENT blacklisting a monster on less than 90% health unless we haven't damaged it for more than 2 minutes
										 if (CurrentTarget.targetType.Value==TargetType.Unit)
										 {
												if (CurrentTarget.IsTreasureGoblin&&Bot.SettingsFunky.GoblinPriority>=3) bBlacklistThis=false;
												if (DateTime.Now.Subtract(Bot.Combat.dateSincePickedTarget).TotalSeconds<=120) bBlacklistThis=false;
										 }

										 if (bBlacklistThis)
										 {
												if (CurrentTarget.targetType.Value==TargetType.Unit)
												{
													 //Logging.WriteDiagnostic("[Funky] Blacklisting a monster because of possible stuck issues. Monster="+ObjectData.InternalName+" {"+
													 //ObjectData.SNOID.ToString()+"}. Range="+ObjectData.CentreDistance.ToString()+", health %="+ObjectData.CurrentHealthPct.ToString());
												}

												CurrentTarget.NeedsRemoved=true;
												CurrentTarget.BlacklistFlag=BlacklistType.Temporary;
										 }
									}
									// Make sure we start trying to move again should we need to!
									Bot.Combat.bPickNewAbilities=true;

									TargetMovement.NewTargetResetVars();
							 }
							 // Ok we didn't want a new target list, should we at least update the position of the current target, if it's a monster?
							 else if (CurrentTarget.targetType.Value==TargetType.Unit&&CurrentTarget.IsStillValid())
							 {
									try
									{
										 CurrentTarget.UpdatePosition();
									}
									catch
									{
										 // Keep going anyway if we failed to get a new position from DemonBuddy
										 Logging.WriteDiagnostic("GSDEBUG: Caught position read failure in main target handler loop.");
									}
							 }
						}
						#endregion

						// This variable just prevents an instant 2-target update after coming here from the main decorator function above
						Bot.Combat.bWholeNewTarget=false;


						//Health Change Timer
						if (CurrentTarget.targetType.Value==TargetType.Unit)
						{
							 //Update CurrentUnitTarget Variable.
							 if (CurrentUnitTarget==null) CurrentUnitTarget=(CacheUnit)CurrentTarget;

							 double HealthChangeMS=DateTime.Now.Subtract(Bot.Combat.LastHealthChange).TotalMilliseconds;

							 if (HealthChangeMS>3000&&!CurrentTarget.ObjectIsSpecial||HealthChangeMS>6000)
							 {
									Logging.WriteVerbose("Health change has not occured within 3 seconds for unit {0}", CurrentTarget.InternalName);
									Bot.Combat.bForceTargetUpdate=true;
									CurrentState=RunStatus.Running;
									CurrentTarget.BlacklistLoops=10;
									return false;
							 }
						}


						//We are ready for the specific object type interaction
						return true;
				 }

				 public virtual bool CombatLogic()
				 {
						// Find a valid Ability if the target is a monster
						#region AbilityPick
						if (Bot.Combat.bPickNewAbilities&&!Bot.Combat.bWaitingForPower&&!Bot.Combat.bWaitingForPotion)
						{
							 Bot.Combat.bPickNewAbilities=false;
							 if (CurrentTarget.targetType.Value==TargetType.Unit&&CurrentTarget.AcdGuid.HasValue)
							 {
								  // Pick a suitable Ability			
								  if (CurrentUnitTarget.IsClusterException)
										Bot.Combat.powerPrime=Bot.Class.AbilitySelector(false, false, ConditionCriteraTypes.SingleTarget);
								  else
										Bot.Combat.powerPrime=Bot.Class.AbilitySelector(false, false);
							 }

							 // Select an Ability for destroying a destructible with in advance
							 if (CurrentTarget.targetType.Value==TargetType.Destructible||CurrentTarget.targetType==TargetType.Barricade)
									Bot.Combat.powerPrime=Bot.Class.DestructibleAbility();
						}
						#endregion

						// Pop a potion when necessary
						// Note that we force a single-loop pause first, to help potion popping "go off"
						#region PotionCheck
						if (Bot.Character.dCurrentHealthPct<=Bot.EmergencyHealthPotionLimit
							 &&!Bot.Combat.bWaitingForPower&&!Bot.Combat.bWaitingForPotion&&!Bot.Character.bIsIncapacitated&&Bot.Class.AbilityUseTimer(SNOPower.DrinkHealthPotion))
						{
							 Bot.Combat.bWaitingForPotion=true;
							 CurrentState=RunStatus.Running;
							 return false;
						}
						if (Bot.Combat.bWaitingForPotion)
						{
							 Bot.Combat.bWaitingForPotion=false;
							 if (!Bot.Character.bIsIncapacitated&&Bot.Class.AbilityUseTimer(SNOPower.DrinkHealthPotion))
							 {
									Bot.AttemptToUseHealthPotion();
							 }
						}
						#endregion

						// See if we can use any special buffs etc. while in avoidance
						#region AvoidanceSpecialAbilityCheck
						if (TargetType.Avoidance.HasFlag(CurrentTarget.targetType.Value))
						{
							 Ability movement;
							 if (Bot.Class.FindSpecialMovementPower(out movement))
							 {
								  ability.Ability.SetupAbilityForUse(ref movement);
								  ability.Ability.UsePower(ref movement);
									movement.SuccessfullyUsed();
							 }
						}

						//if ((TargetType.Gold|TargetType.Globe|TargetType.Gizmo|TargetType.Item).HasFlag(CurrentTarget.targetType.Value))
						//{
						//    Bot.Combat.powerBuff=Bot.Class.AbilitySelector(true, false);
						//    if (Bot.Combat.powerBuff.Power!=SNOPower.None)
						//    {
						//         Bot.Combat.powerBuff.UsePower();
						//         //	ZetaDia.Me.UsePower(Bot.Combat.powerBuff.Power, Bot.Combat.powerBuff.TargetPosition, Bot.Combat.powerBuff.WorldID, Bot.Combat.powerBuff.TargetRAGUID);
						//         Bot.Combat.powerBuff.SuccessfullyUsed();
						//    }
						//}
						#endregion

						return true;
				 }


				 public virtual bool Movement()
				 {
						// Set current destination to our current target's destination
						TargetMovement.CurrentTargetLocation=CurrentTarget.Position;
						if (CurrentTarget.LOSV3!=Vector3.Zero)
						{
							 //Recheck LOS every second
							 if (CurrentTarget.LastLOSCheckMS>2500)
							 {
									NavCellFlags LOSNavFlags=NavCellFlags.None;
									if (Bot.Class.IsMeleeClass||!CurrentTarget.WithinInteractionRange())
									{
										 if (Bot.Combat.powerPrime.IsRanged) //Add Projectile Testing!
												LOSNavFlags=NavCellFlags.AllowWalk|NavCellFlags.AllowProjectile;
										 else
												LOSNavFlags=NavCellFlags.AllowWalk;
									}

									if (CurrentTarget.LOSTest(Bot.Character.Position, true, Bot.Combat.powerPrime.IsRanged, LOSNavFlags))
									{
										 //Los Passed!
										 CurrentTarget.LOSV3=Vector3.Zero;
										 Bot.Combat.bWholeNewTarget=true;
										 return false;
									}
							 }

							 TargetMovement.CurrentTargetLocation=CurrentTarget.LOSV3;
							 if (Bot.Character.Position.Distance(CurrentTarget.LOSV3)>2.5f)
							 {
									CurrentState=TargetMovement.TargetMoveTo(CurrentTarget);
									return false;
							 }
						}



						//Check if we are in range for interaction..
						if (CurrentTarget.WithinInteractionRange())
							 return true;
						else
						{//Movement required..
							 CurrentState=TargetMovement.TargetMoveTo(CurrentTarget);
							 return false;
						}
				 }

				 //This is the final step in handling.. here we actually switch to a specific method based upon the target object we are handling.
				 public virtual bool ObjectInteraction()
				 {

						#region DebugInfo
						if (Bot.SettingsFunky.DebugStatusBar)
						{
							 Funky.sStatusText="[Interact- ";
							 switch (CurrentTarget.targetType.Value)
							 {
									case TargetType.Avoidance:
										 Funky.sStatusText+="Avoid] ";
										 break;
									case TargetType.Unit:
										 Funky.sStatusText+="Combat] ";
										 break;
									case TargetType.Item:
									case TargetType.Gold:
									case TargetType.Globe:
										 Funky.sStatusText+="Pickup] ";
										 break;
									case TargetType.Interactable:
										 Funky.sStatusText+="Interact] ";
										 break;
									case TargetType.Container:
										 Funky.sStatusText+="Open] ";
										 break;
									case TargetType.Destructible:
									case TargetType.Barricade:
										 Funky.sStatusText+="Destroy] ";
										 break;
									case TargetType.Shrine:
										 Funky.sStatusText+="Click] ";
										 break;
							 }
							 Funky.sStatusText+="Target="+CurrentTarget.InternalName+" C-Dist="+Math.Round(CurrentTarget.CentreDistance, 2).ToString()+". "+
									 "R-Dist="+Math.Round(CurrentTarget.RadiusDistance, 2).ToString()+". ";

							 if (CurrentTarget.targetType.Value==TargetType.Unit&&Bot.Combat.powerPrime.Power!=SNOPower.None)
									Funky.sStatusText+="Power="+Bot.Combat.powerPrime.Power.ToString()+" (range "+Bot.Combat.powerPrime.MinimumRange.ToString()+") ";


							 Funky.sStatusText+="Weight="+CurrentTarget.Weight.ToString();
							 BotMain.StatusText=Funky.sStatusText;
							 Funky.bResetStatusText=true;
						}
						#endregion

						switch (CurrentTarget.targetType.Value)
						{
							 case TargetType.Unit:
							 case TargetType.Item:
							 case TargetType.Gold:
							 case TargetType.Globe:
							 case TargetType.Shrine:
							 case TargetType.Interactable:
							 case TargetType.Container:
							 case TargetType.Door:
							 case TargetType.Destructible:
							 case TargetType.Barricade:
									CurrentState=CurrentTarget.Interact();
									break;
							 case TargetType.Avoidance:
									Bot.Combat.timeCancelledEmergencyMove=DateTime.Now;
									Bot.Combat.timeCancelledFleeMove=DateTime.Now;
									CurrentState=RunStatus.Running;
									break;
						}

						// Now tell Trinity to get a new target!
						Bot.Combat.lastChangedZigZag=DateTime.Today;
						Bot.Combat.vPositionLastZigZagCheck=Vector3.Zero;
						//Bot.Combat.bForceTargetUpdate=true;
						
						return false;
				 }

				 internal void UpdateStatusText(string Action)
				 {
						Funky.sStatusText=Action+" ";

						Funky.sStatusText+="Target="+CurrentTarget.InternalName+" C-Dist="+Math.Round(CurrentTarget.CentreDistance, 2).ToString()+". "+
							 "R-Dist="+Math.Round(CurrentTarget.RadiusDistance, 2).ToString()+". ";

						if (CurrentTarget.targetType.Value==TargetType.Unit&&Bot.Combat.powerPrime.Power!=SNOPower.None)
							 Funky.sStatusText+="Power="+Bot.Combat.powerPrime.Power.ToString()+" (range "+Bot.Combat.powerPrime.MinimumRange.ToString()+") ";

						Funky.sStatusText+="Weight="+CurrentTarget.Weight.ToString();
						BotMain.StatusText=Funky.sStatusText;
						Funky.bResetStatusText=true;
				 }

				 public override bool Equals(object obj)
				 {
						//Check for null and compare run-time types. 
						if (obj==null)
						{
							 if (this.CurrentTarget!=null)
									return false;
							 else
									return true;
						}
						else
						{
							 Type ta=obj.GetType();
							 Type tb=this.CurrentTarget!=null?this.CurrentTarget.GetType():this.GetType();

							 if (ta.Equals(tb))
							 {
									return ((CacheObject)obj)==(this.CurrentTarget);
							 }
							 else
									return false;
						}
				 }
				 public override int GetHashCode()
				 {
						return this.CurrentTarget!=null?this.CurrentTarget.GetHashCode():base.GetHashCode();
				 }
			}
	 
}