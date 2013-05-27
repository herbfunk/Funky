using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using System.Collections.Generic;
using Zeta.Internals.Actors;
using Zeta.Internals.SNO;
using Zeta.Internals.Actors.Gizmos;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  public partial class dbRefresh
		  {
				///<summary>
				///Iterates through CachedCollection, and returns a list of objects that are should be considered for current target.
				///</summary>
				public static List<CacheObject> ReturnUsableList()
				{
					 List<CacheObject> ReturnList=new List<CacheObject>();

					 foreach (CacheObject obj in ObjectCache.Objects.Values)
					 {
						  if (!obj.ObjectIsValidForTargeting)
								continue;

						  ReturnList.Add(obj);

					 }
					 return ReturnList;
				}

				///<summary>
				///Iterates through Usable objects and sets the Bot.CurrentTarget to the highest weighted object found inside the given list.
				///</summary>
				public static void WeightEvaluationObjList(List<CacheObject> listObjCache)
				{
					 // Store if we are ignoring all units this cycle or not
					 bool bIgnoreAllUnits=!Bot.Combat.bAnyChampionsPresent&&!Bot.Combat.bAnyMobsInCloseRange&&((!Bot.Combat.bAnyTreasureGoblinsPresent&&SettingsFunky.GoblinPriority>=2)||SettingsFunky.GoblinPriority<2)&&
									 Bot.Character.dCurrentHealthPct>=0.85d;

					 //clear our last "avoid" list..
					 ObjectCache.Objects.objectsIgnoredDueToAvoidance.Clear();

					 bool bPrioritizeCloseRange=(Bot.Combat.bForceCloseRangeTarget||Bot.Character.bIsRooted);
					 bool bIsBerserked=HasBuff(SNOPower.Barbarian_WrathOfTheBerserker);
					 Bot.Combat.SurroundingUnits=ObjectCache.Objects.Values.OfType<CacheUnit>().Where(unit => unit.RadiusDistance<=11f).Count();
					 double iHighestWeightFound=0;


					 foreach (CacheObject thisobj in listObjCache)
					 {

						  thisobj.UpdateWeight();


						  if (Bot.Combat.PrioritizedRAGUIDs.Contains(thisobj.RAGUID))
						  {//Prioritized by target handler.. so lets make it so!
								if (thisobj.LastPriortized<1500)
									 thisobj.Weight+=12500;
								else
									 Bot.Combat.PrioritizedRAGUIDs.Remove(thisobj.RAGUID);
						  }

						  //Avoidance OverSteps Prioritized
						  if (ObjectCache.Objects.objectsIgnoredDueToAvoidance.Contains(thisobj))
						  {//we didn't want to skip weighting the object because we want to know which ones are "priority" to attempt finding a safe location for.

								Vector3 SafeLOSMovement;
								//Lets see if we can find a location..
								if (thisobj.Weight>iHighestWeightFound)
								{
									 if (GridPointAreaCache.AttemptFindTargetSafeLocation(out SafeLOSMovement, thisobj, true, Bot.Class.KiteDistance>0f))
									 {
										  Bot.Target.ObjectData=new CacheObject(SafeLOSMovement, TargetType.Avoidance, 20000, "SafetyMovement", 5f, -1);
										  iHighestWeightFound=thisobj.Weight;
										  continue;
									 }
									 else
										  thisobj.Weight=1;
								}
								else
									 continue;
						  }

						  if (thisobj.Weight==1)
						  ////This target will cause us to enter an avoidance zone!
						  {// Force the character to stay where it is if there is nothing available that is out of avoidance stuff and we aren't already in avoidance stuff
								thisobj.Weight=0;
								Bot.Combat.bStayPutDuringAvoidance=true;
								continue;
						  }

						  // Is the weight of this one higher than the current-highest weight? Then make this the new primary target!
						  if (thisobj.Weight>iHighestWeightFound&&thisobj.Weight>0)
						  {
								//Check combat looting
								if (iHighestWeightFound>0
												&&thisobj.targetType.Value==TargetType.Item
												&&!Zeta.CommonBot.Settings.CharacterSettings.Instance.CombatLooting
												&&Bot.Target.ObjectData.targetType.Value==TargetType.Unit) continue;


								//Set our current target to this object!
								Bot.Target.ObjectData=ObjectCache.Objects[thisobj.RAGUID];
								iHighestWeightFound=thisobj.Weight;
						  }

					 } // Loop through all the objects and give them a weight


					 //No Target? Not Staying Put? Recheck remaining units that may have been ignored.
					 if (ObjectCache.Objects.objectsIgnoredDueToAvoidance.Count>0
								&&Bot.Target.Equals(null)&&!Bot.Combat.bStayPutDuringAvoidance)
					 {
						  Bot.Combat.bStayPutDuringAvoidance=true;
						  return;
					 }

					 #region RangeClassTargetUnit

					 if (Bot.Target.ObjectData!=null&&Bot.Target.ObjectData.targetType.Value==TargetType.Unit&&!Bot.Class.IsMeleeClass)
					 {
						  cacheSNOPower tmpSNOPowerAbility=GilesAbilitySelector(false, false, false);
						  float range=Math.Min(Bot.Target.ObjectData.RadiusDistance, tmpSNOPowerAbility.iMinimumRange);
						  Vector3 abilityPosition=MathEx.GetPointAt(Bot.Character.Position, range, FindDirection(Bot.Character.Position, Bot.Target.ObjectData.Position, true));

						  if (range>0f)
						  {
								Bot.Combat.bForceCloseRangeTarget=true;

								if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(Bot.Character.Position, abilityPosition))
									 Bot.Target.ObjectData.BlacklistLoops=1;

								if (ObjectCache.Objects.IsPointNearbyMonsters(abilityPosition, Bot.Class.KiteDistance)&&ObjectCache.Objects.Values.OfType<CacheUnit>().Any(unit => unit.Position.Distance(abilityPosition)+unit.Radius<Bot.Class.KiteDistance))
									 Bot.Target.ObjectData=ObjectCache.Objects.Values.OfType<CacheUnit>().First(unit => unit.Position.Distance(abilityPosition)+unit.Radius<Bot.Class.KiteDistance);
						  }
					 }


					 #endregion
				}
		  }
	 }
}