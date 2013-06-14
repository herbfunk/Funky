using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals;
using Zeta.Internals.Actors;
using Zeta.Internals.SNO;
using Zeta.Navigation;
using Zeta.TreeSharp;
using System.Globalization;
using System.Drawing;
using Zeta.Pathfinding;

namespace FunkyTrinity
{

	 public partial class Funky
	 {


		  // **********************************************************************************************
		  // *****               Special Zig-Zag movement for whirlwind/tempest                       *****
		  // **********************************************************************************************
		  public static Vector3 FindZigZagTargetLocation(Vector3 vTargetLocation, float fDistanceOutreach, bool bRandomizeDistance=false, bool bRandomizeStart=false, bool bCheckGround=false)
		  {
				Vector3 vThisZigZag=vNullLocation;
				bool useTargetBasedZigZag=false;
				float minDistance=6f;
				float maxDistance=16f;
				int minTargets=2;

				if (useTargetBasedZigZag&&!Bot.Combat.bAnyTreasureGoblinsPresent&&Bot.Combat.UnitRAGUIDs.Count>=minTargets)
				{
					 List<CacheObject> units_=ObjectCache.Objects.Values.Where(obj => Bot.Combat.UnitRAGUIDs.Contains(obj.RAGUID)).ToList();

					 IEnumerable<CacheObject> zigZagTargets=
						  from u in units_
						  where u.CentreDistance>minDistance&&u.CentreDistance<maxDistance&&u.RAGUID!=Bot.Target.CurrentTarget.RAGUID&&
						  !ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(u.Position)
						  select u;

					 if (zigZagTargets.Count()>=minTargets)
					 {
						  vThisZigZag=zigZagTargets.OrderByDescending(u => u.Weight).FirstOrDefault().Position;
						  return vThisZigZag;
					 }
				}

				Random rndNum=new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), NumberStyles.HexNumber));
				bool bCanRayCast;
				float iFakeStart=0;
				//K: bRandomizeStart is for boss and elite, they usually jump around, make obstacles, let you Incapacitated. 
				//   you usually have to move back and forth to hit them
				if (bRandomizeStart)
					 iFakeStart=rndNum.Next(18)*5;
				if (bRandomizeDistance)
					 fDistanceOutreach+=rndNum.Next(18);
				float fDirectionToTarget=FindDirection(Bot.Character.Position, vTargetLocation);

				float fPointToTarget;

				float fHighestWeight=float.NegativeInfinity;
				Vector3 vBestLocation=vNullLocation;

				bool bFoundSafeSpotsFirstLoop=false;
				float fAdditionalRange=0f;
				//K: Direction is more important than distance

				for (int iMultiplier=1; iMultiplier<=2; iMultiplier++)
				{
					 if (iMultiplier==2)
					 {
						  if (bFoundSafeSpotsFirstLoop)
								break;
						  fAdditionalRange=150f;
						  if (bRandomizeStart)
								iFakeStart=30f+(rndNum.Next(16)*5);
						  else
								iFakeStart=(rndNum.Next(17)*5);
					 }
					 float fRunDistance=fDistanceOutreach;
					 for (float iDegreeChange=iFakeStart; iDegreeChange<=30f+fAdditionalRange; iDegreeChange+=5)
					 {
						  float iPosition=iDegreeChange;
						  //point to target is better, otherwise we have to avoid obstacle first 
						  if (iPosition>105f)
								iPosition=90f-iPosition;
						  else if (iPosition>30f)
								iPosition-=15f;
						  else
								iPosition=15f-iPosition;
						  fPointToTarget=iPosition;

						  iPosition+=fDirectionToTarget;
						  if (iPosition<0)
								iPosition=360f+iPosition;
						  if (iPosition>=360f)
								iPosition=iPosition-360f;

						  vThisZigZag=MathEx.GetPointAt(Bot.Character.Position, fRunDistance, MathEx.ToRadians(iPosition));

						  if (fPointToTarget<=30f||fPointToTarget>=330f)
						  {
								vThisZigZag.Z=vTargetLocation.Z;
						  }
						  else if (fPointToTarget<=60f||fPointToTarget>=300f)
						  {
								//K: we are trying to find position that we can circle around the target
								//   but we shouldn't run too far away from target
								vThisZigZag.Z=(vTargetLocation.Z+Bot.Character.Position.Z)/2;
								fRunDistance=fDistanceOutreach-5f;
						  }
						  else
						  {
								//K: don't move too far if we are not point to target, we just try to move
								//   this can help a lot when we are near stairs
								fRunDistance=8f;
						  }

						  bCanRayCast=mgp.CanStandAt(vThisZigZag);



						  // Give weight to each zigzag point, so we can find the best one to aim for
						  if (bCanRayCast)
						  {
								bool bAnyAvoidance=false;

								// Starting weight is 1000f
								float fThisWeight=1000f;
								if (iMultiplier==2)
									 fThisWeight-=80f;

								if (Bot.Class.KiteDistance>0&&ObjectCache.Objects.IsPointNearbyMonsters(vThisZigZag, Bot.Class.KiteDistance))
									 continue;

								if (ObjectCache.Obstacles.Navigations.Any(obj => obj.Obstacletype.Value!=ObstacleType.Monster&&obj.TestIntersection(Bot.Character.Position, vThisZigZag, false)))
									 continue;

								float distanceToPoint=vThisZigZag.Distance2D(Bot.Character.Position);
								float distanceToTarget=vTargetLocation.Distance2D(Bot.Character.Position);

								fThisWeight+=(distanceToTarget*10f);

								// Use this one if it's more weight, or we haven't even found one yet, or if same weight as another with a random chance
								if (fThisWeight>fHighestWeight)
								{
									 fHighestWeight=fThisWeight;


									 vBestLocation=new Vector3(vThisZigZag.X, vThisZigZag.Y, mgp.GetHeight(vThisZigZag.ToVector2()));


									 if (!bAnyAvoidance)
										  bFoundSafeSpotsFirstLoop=true;
								}
						  }
						  // Can we raycast to the point at minimum?
					 }
					 // Loop through degrees
				}
				// Loop through multiplier
				return vBestLocation;
		  }


		  // **********************************************************************************************
		  // *****                            Find A Safe Movement Location                           *****
		  // **********************************************************************************************
		  public static Vector3 FindSafeZone(bool bFindAntiStuckSpot, int iAntiStuckAttempts, Vector3 vNearbyPoint, bool bKitingSpot=false)
		  {
				if (!bFindAntiStuckSpot)
				{
					 // Already searched & found a safe spot in last 800 milliseconds, stick to it
					 if (!Bot.Combat.TravellingAvoidance&&DateTime.Now.Subtract(lastFoundSafeSpot).TotalMilliseconds<=800&&vlastSafeSpot!=vNullLocation)
					 {
						  return vlastSafeSpot;
					 }



					 // Wizards can look for bee stings in range and try a wave of force to dispel them
					 if (!bKitingSpot&&Bot.Class.AC==ActorClass.Wizard&&HotbarAbilitiesContainsPower(SNOPower.Wizard_WaveOfForce)&&Bot.Character.dCurrentEnergy>=25&&
						  DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.Wizard_WaveOfForce]).TotalMilliseconds>=dictAbilityRepeatDelay[SNOPower.Wizard_WaveOfForce]&&
						  !Bot.Character.bIsIncapacitated&&ObjectCache.Obstacles.Values.OfType<CacheAvoidance>().Count(u => u.SNOID==5212&&u.Position.Distance(Bot.Character.Position)<=15f)>=2&&
						  (SettingsFunky.Class.bEnableCriticalMass||PowerManager.CanCast(SNOPower.Wizard_WaveOfForce)))
					 {
						  ZetaDia.Me.UsePower(SNOPower.Wizard_WaveOfForce, vNullLocation, Bot.Character.iCurrentWorldID, -1);
					 }
				} // Only looking for an anti-stuck location?


				float fHighestWeight=0f;
				Vector3 vBestLocation=vNullLocation;

				//Non-Stuck
				if (!bFindAntiStuckSpot)
				{
					 //Use new safe spot finding method.
					 //return FindSafeBacktrackPosition(vNearbyPoint);
				}

				// Now find a randomized safe point we can actually move to
				// We randomize the order so we don't spam walk by accident back and forth
				Random rndNum=new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), NumberStyles.HexNumber));
				int iFakeStart=(rndNum.Next(36))*10;
				// Start off checking every 12 degrees (which is 30 loops for a full circle)
				const int iMaxRadiusChecks=30;
				const int iRadiusMultiplier=12;
				for (int iStep=1; iStep<=8; iStep++)
				{
					 // Distance of 10 for each step loop at first
					 int iDistanceOut=10;
					 if (!bKitingSpot)
					 {
						  switch (iStep)
						  {
								case 1: iDistanceOut=10; break;
								case 2: iDistanceOut=18; break;
								case 3: iDistanceOut=26; break;
								case 4: iDistanceOut=34; break;
								case 5: iDistanceOut=42; break;
								case 6: iDistanceOut=50; break;
								case 7: iDistanceOut=58; break;
								case 8: iDistanceOut=66; break;
						  }
					 }
					 else
					 {
						  switch (iStep)
						  {
								case 8: iDistanceOut=10; break;
								case 7: iDistanceOut=18; break;
								case 6: iDistanceOut=26; break;
								case 5: iDistanceOut=34; break;
								case 4: iDistanceOut=42; break;
								case 3: iDistanceOut=50; break;
								case 2: iDistanceOut=58; break;
								case 1: iDistanceOut=66; break;
						  }
					 }
					 int iRandomUse=3+((iStep-1)*4);
					 // Try to return "early", or as soon as possible, beyond step 4, except when unstucking, when the max steps is based on the unstuck attempt
					 if (fHighestWeight>0&&
						  ((!bFindAntiStuckSpot&&iStep>5)||(bFindAntiStuckSpot&&iStep>iAntiStuckAttempts))
						  )
					 {
						  lastFoundSafeSpot=DateTime.Now;
						  vlastSafeSpot=vBestLocation;
						  break;
					 }
					 // Loop through all possible radii
					 for (int iThisRadius=0; iThisRadius<iMaxRadiusChecks; iThisRadius++)
					 {
						  int iPosition=iFakeStart+(iThisRadius*iRadiusMultiplier);
						  if (iPosition>=360)
								iPosition-=360;

						  float fBonusAmount=0f;

						  Vector3 vTestPoint=MathEx.GetPointAt(Bot.Character.Position, iDistanceOut, MathEx.ToRadians(iPosition));
						  // First check no avoidance obstacles in this spot
						  if (!ObjectCache.Obstacles.Values.OfType<CacheAvoidance>().Any(u => u.Position.Distance(vTestPoint)<=u.Radius))
						  {
								bool bCanRaycast=GilesCanRayCast(Bot.Character.Position, vTestPoint, NavCellFlags.AllowWalk);
								// Now see if the client can navigate there, and we haven't temporarily blacklisted this spot
								if (bCanRaycast)
								{
									 // Now calculate a weight to pick the "best" avoidance safety spot at the moment
									 float fThisWeight=1000f+fBonusAmount;
									 if (!bFindAntiStuckSpot)
									 {
										  fThisWeight-=((iStep-1)*150);
									 }
									 // is it near a point we'd prefer to be close to?
									 if (vNearbyPoint!=vNullLocation)
									 {
										  float fDistanceToNearby=Vector3.Distance(vTestPoint, vNearbyPoint);
										  if (fDistanceToNearby<=25f)
										  {
												if (!bKitingSpot)
													 fThisWeight+=(160*(1-(fDistanceToNearby/25)));
												else
													 fThisWeight-=(300*(1-(fDistanceToNearby/25)));
										  }
									 }
									 if (fThisWeight<=1)
										  fThisWeight=1;
									 // Use this one if it's more weight, or we haven't even found one yet, or if same weight as another with a random chance
									 if (fThisWeight>fHighestWeight||fHighestWeight==0f||(fThisWeight==fHighestWeight&&rndNum.Next(iRandomUse)==1))
									 {
										  fHighestWeight=fThisWeight;
										  vBestLocation=vTestPoint;
										  // Found a very good spot so just use this one!
										  //if (iAOECount == 0 && fThisWeight > 400)
										  //    break;
									 }
								}
						  }
					 } // Loop through the circle
				} // Loop through distance-range steps
				if (fHighestWeight>0)
				{
					 lastFoundSafeSpot=DateTime.Now;
					 vlastSafeSpot=vBestLocation;
				}
				return vBestLocation;
		  }

	 }
}