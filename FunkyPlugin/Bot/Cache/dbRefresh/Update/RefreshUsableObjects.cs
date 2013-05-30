﻿using System;
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
                List<CacheObject> ReturnList = new List<CacheObject>();

                foreach(CacheObject obj in ObjectCache.Objects.Values)
                {
                    if(!obj.ObjectIsValidForTargeting)
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
                bool bIgnoreAllUnits = !Bot.Combat.bAnyChampionsPresent && !Bot.Combat.bAnyMobsInCloseRange && ((!Bot.Combat.bAnyTreasureGoblinsPresent && SettingsFunky.GoblinPriority >= 2) || SettingsFunky.GoblinPriority < 2) &&
                                Bot.Character.dCurrentHealthPct >= 0.85d;

                //clear our last "avoid" list..
                ObjectCache.Objects.objectsIgnoredDueToAvoidance.Clear();

                bool bPrioritizeCloseRange = (Bot.Combat.bForceCloseRangeTarget || Bot.Character.bIsRooted);
                bool bIsBerserked = HasBuff(SNOPower.Barbarian_WrathOfTheBerserker);
                Bot.Combat.SurroundingUnits = ObjectCache.Objects.Values.OfType<CacheUnit>().Where(unit => unit.RadiusDistance <= 11f).Count();
                double iHighestWeightFound = 0;


                foreach(CacheObject thisobj in listObjCache)
                {

                    thisobj.UpdateWeight();

                    //Prioritized Units (Blocked/Intersecting Objects)
                    if(Bot.Combat.PrioritizedRAGUIDs.Contains(thisobj.RAGUID))
                    {
                        //remove from list after time based on number of prioritized count
                        if(thisobj.LastPriortized > (thisobj.PriorityCounter*250))
                            Bot.Combat.PrioritizedRAGUIDs.Remove(thisobj.RAGUID);

                        //weight variable based on number of timers prioritized.
                        thisobj.Weight += (250 * thisobj.PriorityCounter);
                    }


                    //Avoidance (Melee Only) Attempt to find a location where we can attack!
                    if(ObjectCache.Objects.objectsIgnoredDueToAvoidance.Contains(thisobj))
                    {
                        Vector3 SafeLOSMovement;
                        if(thisobj.Weight > iHighestWeightFound)
                        {//Only if we don't have a higher priority already..

                            if(thisobj.GPRect.TryFindSafeSpot(out SafeLOSMovement, Bot.Character.Position, Bot.Class.KiteDistance > 0f, true))
                            {
                                Bot.Target.ObjectData = new CacheObject(SafeLOSMovement, TargetType.Avoidance, 20000, "SafetyMovement", 2.5f, -1);
                                iHighestWeightFound = thisobj.Weight;
                            }
                        }

                        continue;
                    }


                    if(thisobj.Weight == 1)
                    {// Force the character to stay where it is if there is nothing available that is out of avoidance stuff and we aren't already in avoidance stuff
                        thisobj.Weight = 0;
                        Bot.Combat.bStayPutDuringAvoidance = true;
                        continue;
                    }

                    // Is the weight of this one higher than the current-highest weight? Then make this the new primary target!
                    if(thisobj.Weight > iHighestWeightFound && thisobj.Weight > 0)
                    {
                        //Check combat looting
                        if(iHighestWeightFound > 0
                                        && thisobj.targetType.Value == TargetType.Item
                                        && !Zeta.CommonBot.Settings.CharacterSettings.Instance.CombatLooting
                                        && Bot.Target.ObjectData.targetType.Value == TargetType.Unit) continue;


                        //Set our current target to this object!
                        Bot.Target.ObjectData = ObjectCache.Objects[thisobj.RAGUID];
                        iHighestWeightFound = thisobj.Weight;
                    }

                } // Loop through all the objects and give them a weight


                #region RangeClassTargetUnit

                if(Bot.Target.ObjectData != null && Bot.Target.ObjectData.targetType.Value == TargetType.Unit && !Bot.Class.IsMeleeClass)
                {
                    cacheSNOPower tmpSNOPowerAbility = GilesAbilitySelector(false, false, false);
                    float range = Math.Min(Bot.Target.ObjectData.RadiusDistance, tmpSNOPowerAbility.iMinimumRange);
                    Vector3 abilityPosition = MathEx.GetPointAt(Bot.Character.Position, range, FindDirection(Bot.Character.Position, Bot.Target.ObjectData.Position, true));

                    if(range > 0f)
                    {
                        Bot.Combat.bForceCloseRangeTarget = true;

                        if(ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(Bot.Character.Position, abilityPosition))
                            Bot.Target.ObjectData.BlacklistLoops = 1;

                        if(ObjectCache.Objects.IsPointNearbyMonsters(abilityPosition, Bot.Class.KiteDistance) && ObjectCache.Objects.Values.OfType<CacheUnit>().Any(unit => unit.Position.Distance(abilityPosition) + unit.Radius < Bot.Class.KiteDistance))
                            Bot.Target.ObjectData = ObjectCache.Objects.Values.OfType<CacheUnit>().First(unit => unit.Position.Distance(abilityPosition) + unit.Radius < Bot.Class.KiteDistance);
                    }
                }


                #endregion

            }
        }
    }
}