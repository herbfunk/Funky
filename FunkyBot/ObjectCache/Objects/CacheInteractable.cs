using System;
using System.Linq;
using FunkyBot.Cache.Enums;
using FunkyBot.Movement;
using FunkyBot.Targeting;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.Internals.Actors.Gizmos;
using Zeta.Internals.SNO;
using Zeta.TreeSharp;

namespace FunkyBot.Cache
{
	public class CacheInteractable : CacheGizmo
	{
		public CacheInteractable(CacheObject baseobj)
			: base(baseobj)
		{
		}

		public override string DebugString
		{
			get
			{
				return String.Format("{0}\r\n PhysSNO={1} HandleAsObstacle={2} Operated={3}",
					base.DebugString, this.PhysicsSNO.HasValue?this.PhysicsSNO.Value.ToString():"NULL",
					this.HandleAsObstacle.HasValue?this.HandleAsObstacle.Value.ToString():"NULL",
					this.GizmoHasBeenUsed.HasValue?this.GizmoHasBeenUsed.Value.ToString():"NULL");
			}
		}
		private double LootRadius
		{
			 get
			 {
				  if (this.targetType.Value==TargetType.Shrine)
						return this.IsHealthWell?Bot.Settings.Ranges.ShrineRange*2:Bot.Settings.Ranges.ShrineRange;

				  if (this.targetType.Value==TargetType.Container)
				  {
						if (this.IsResplendantChest&&Bot.Settings.Targeting.UseExtendedRangeRepChest)
							 return Bot.Settings.Ranges.ContainerOpenRange*2;
						else
							 return Bot.Settings.Ranges.ContainerOpenRange;
				  }

				  return Bot.Targeting.iCurrentMaxLootRadius;
			 }
		}

		public override bool ObjectIsValidForTargeting
		{
			 get
			 {
				  if (!base.ObjectIsValidForTargeting) return false;



				  float centreDistance=this.CentreDistance;
				  float radiusDistance=this.RadiusDistance;

				  if (!this.targetType.HasValue)
						return false;

				  // Ignore it if it's not in range yet
				  if (centreDistance>this.LootRadius)
				  {
						this.BlacklistLoops=10;
						return false;
				  }

				  if (this.RequiresLOSCheck&&!this.IgnoresLOSCheck)
				  {
						//Get the wait time since last used LOSTest
						double lastLOSCheckMS=base.LineOfSight.LastLOSCheckMS;

						//unless its in front of us.. we wait 500ms mandatory.
						if (lastLOSCheckMS<500&&centreDistance>1f)
						{
							 if (this.ObjectIsSpecial) Bot.Combat.LoSMovementObjects.Add(this);
							 return false;
						}
						else
						{
							 //Set the maximum wait time
							 double ReCheckTime=3500;

							 //Close Range.. we change the recheck timer based on how close
							 if (this.CentreDistance<25f)
								  ReCheckTime=(this.CentreDistance*125);
							 else if (this.ObjectIsSpecial)
								  ReCheckTime*=0.25;

							 if (lastLOSCheckMS<ReCheckTime)
							 {
								  if (this.ObjectIsSpecial) Bot.Combat.LoSMovementObjects.Add(this);
								  return false;
							 }
						}

						if (!base.LineOfSight.LOSTest(Bot.Character.Position, true, false))
						{
							 if (this.ObjectIsSpecial) Bot.Combat.LoSMovementObjects.Add(this);
							 return false;
						}

						this.RequiresLOSCheck=false;
				  }

				  // Now for the specifics
				  switch (this.targetType.Value)
				  {
						#region Interactable
						case TargetType.Interactable:
						case TargetType.Door:
							 if (this.GizmoHasBeenUsed.HasValue&&this.GizmoHasBeenUsed.Value==true)
							 {
								  this.NeedsRemoved=true;
								  this.BlacklistFlag=BlacklistType.Permanent;
								  return false;
							 }

							 if (this.targetType.Value==TargetType.Door
									&&this.PriorityCounter==0
									&&radiusDistance>5f)
							 {
								  Vector3 BarricadeTest=this.Position;
								  if (radiusDistance>1f)
								  {

											 BarricadeTest=MathEx.GetPointAt(this.Position, 10f, Navigation.FindDirection(Bot.Character.Position, this.Position, true));
											 bool intersectionTest=!MathEx.IntersectsPath(this.Position, this.CollisionRadius.Value, Bot.Character.Position, BarricadeTest);
											 if (!intersectionTest)
											 {
												  return false;
											 }
										
								  }
							 }

							 if (centreDistance>30f)
							 {
								  this.BlacklistLoops=3;
								  return false;
							 }

							 return true;
						#endregion
						#region Shrine
						case TargetType.Shrine:
							 if (this.GizmoHasBeenUsed.HasValue&&this.GizmoHasBeenUsed.Value==true)
							 {
								  this.NeedsRemoved=true;
								  this.BlacklistFlag=BlacklistType.Permanent;
								  return false;
							 }

							 bool IgnoreThis=false;
							 if (this.IsHealthWell)
							 {
								  //Health wells..
								  if (Bot.Character.dCurrentHealthPct>Bot.Settings.Combat.HealthWellHealthPercent)
								  {
										this.BlacklistLoops=5;
										return false;
								  }
							 }
							 else
							 {
								  ShrineTypes shrinetype=CacheIDLookup.FindShrineType(this.SNOID);

								  //Ignore XP Shrines at MAX Paragon Level!
								  //if (this.SNOID==176075&&Bot.Character.iMyParagonLevel==100)
								  IgnoreThis=!Bot.Settings.Targeting.UseShrineTypes[(int)shrinetype];
							 }

							 //Ignoring..?
							 if (IgnoreThis)
							 {
								  this.NeedsRemoved=true;
								  this.BlacklistFlag=BlacklistType.Permanent;
								  return false;
							 }

							 // Bag it!
							 this.Radius=5.1f;
							 break;
						#endregion
						#region Container
						case TargetType.Container:
							 if (this.GizmoHasBeenUsed.HasValue&&this.GizmoHasBeenUsed.Value==true)
							 {
								  this.NeedsRemoved=true;
								  this.BlacklistFlag=BlacklistType.Permanent;
								  return false;
							 }

							 //Vendor Run and DB Settings check
							 if (Funky.TownRunManager.bWantToTownRun
								  ||!this.IsChestContainer&&!Zeta.CommonBot.Settings.CharacterSettings.Instance.OpenLootContainers
								  ||this.IsChestContainer&&!Zeta.CommonBot.Settings.CharacterSettings.Instance.OpenChests)
							 {
								  this.BlacklistLoops=25;
								  return false;
							 }

							 if (this.IsCorpseContainer&&Bot.Settings.Targeting.IgnoreCorpses)
							 {
								  this.NeedsRemoved=true;
								  this.BlacklistFlag=BlacklistType.Permanent;
								  return false;
							 }


							 // Any physics mesh? Give a minimum distance of 5 feet
							 if (this.PhysicsSNO.HasValue&&this.PhysicsSNO<=0)
							 {
								  this.NeedsRemoved=true;
								  return false;
							 }
								 

							 // Superlist for rare chests etc.
							 if (this.IsResplendantChest)
							 {
								  //setup wait time. (Unlike Units, we blacklist right after we interact)
								  if (Bot.Targeting.LastCachedTarget==this)
								  {
										Bot.Targeting.lastHadContainerAsTarget=DateTime.Now;
										Bot.Targeting.lastHadRareChestAsTarget=DateTime.Now;
								  }
							 }

							 // Bag it!
							 if (this.IsChestContainer)
								  this.Radius=5.1f;
							 else
								  this.Radius=4f;

							 break;
						#endregion
				  } // Object switch on type (to seperate shrines, destructibles, barricades etc.)




				  return true;

			 }
		}

		public override void UpdateWeight()
		{
			base.UpdateWeight();

			if (this.CentreDistance>=4f&&Bot.Combat.NearbyAvoidances.Count>0)
			{
				Vector3 TestPosition=this.Position;
				if (ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(TestPosition))
					this.Weight=1;
				else if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(TestPosition)) //intersecting avoidances..
					this.Weight=1;
			}

			if (this.Weight!=1)
			{
				float centreDistance=this.CentreDistance;
				Vector3 BotPosition=Bot.Character.Position;
				switch (this.targetType.Value)
				{
					case TargetType.Shrine:
						this.Weight=14500d-(Math.Floor(centreDistance)*170d);
						// Very close shrines get a weight increase
						if (centreDistance<=20f)
							this.Weight+=1000d;

						// health pool
						if (base.IsHealthWell)
						{
							if (Bot.Character.dCurrentHealthPct>0.75d)
								this.Weight=0;
							else
								//Give weight based upon current health percent.
								this.Weight+=1000d/(Bot.Character.dCurrentHealthPct);
						}

						if (this.Weight>0)
						{
							// Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
							if (this==Bot.Targeting.LastCachedTarget)
								this.Weight+=600;
							// Are we prioritizing close-range stuff atm? If so limit it at a value 3k lower than monster close-range priority
							if (Bot.Character.bIsRooted)
								this.Weight=18500d-(Math.Floor(centreDistance)*200);
							// If there's a monster in the path-line to the item, reduce the weight by 25%
							if (ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(this, BotPosition)))
								this.Weight*=0.75;
						}
						break;
					case TargetType.Interactable:
					case TargetType.Door:
						this.Weight=15000d-(Math.Floor(centreDistance)*170d);
						if (centreDistance<=12f)
							this.Weight+=800d;
						// Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
						if (this==Bot.Targeting.LastCachedTarget&&centreDistance<=25f)
							this.Weight+=400;
						// If there's a monster in the path-line to the item, reduce the weight by 50%
						if (ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(this, BotPosition)))
							this.Weight*=0.5;
						break;
					case TargetType.Container:
						this.Weight=11000d-(Math.Floor(centreDistance)*190d);
						if (centreDistance<=12f)
							this.Weight+=600d;
						// Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
						if (this==Bot.Targeting.LastCachedTarget&&centreDistance<=25f)
						{
							this.Weight+=400;
						}
						// If there's a monster in the path-line to the item, reduce the weight by 50%
						if (ObjectCache.Obstacles.Monsters.Any(cp => cp.TestIntersection(this, BotPosition)))
						{
							this.Weight*=0.5;
						}
						if (this.IsResplendantChest)
							this.Weight+=1500;

						break;
				}
			}
			else
			{
				this.Weight=0;
				this.BlacklistLoops=15;
			}
		}

		public override bool IsZDifferenceValid
		{
			get
			{
				float fThisHeightDifference=Funky.Difference(Bot.Character.Position.Z, this.Position.Z);
				if (fThisHeightDifference>=10f)
				{
					return false;

				}
				return base.IsZDifferenceValid;
			}
		}

		public override bool ObjectShouldBeRecreated
		{
			get
			{
				return false;
			}
		}

		public override RunStatus Interact()
		{
			Bot.Character.WaitWhileAnimating(20);
			ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, this.Position, Bot.Character.iCurrentWorldID, base.AcdGuid.Value);
			this.InteractionAttempts++;

			if (this.InteractionAttempts==1)
			{
				// Force waiting AFTER power use for certain abilities
				Bot.Targeting.bWaitingAfterPower=true;
				Bot.Class.PowerPrime.WaitLoopsAfter=5;
			}

			// Interactables can have a long channeling time...
			if (this.targetType.Value.HasFlag(TargetType.Interactable))
			{
				Bot.Character.WaitWhileAnimating(1500);
			}

			Bot.Character.WaitWhileAnimating(75, true);

			// If we've tried interacting too many times, blacklist this for a while
			if (this.InteractionAttempts>5)
			{
				this.BlacklistLoops=50;
				this.InteractionAttempts=0;
			}

			if (!Bot.Targeting.bWaitingAfterPower)
			{
				// Now tell Trinity to get a new target!
				 Bot.NavigationCache.lastChangedZigZag=DateTime.Today;
				 Bot.NavigationCache.vPositionLastZigZagCheck=Vector3.Zero;
				 Bot.Targeting.bForceTargetUpdate=true;
			}
			return RunStatus.Running;
		}

		public override bool WithinInteractionRange()
		{
			float fRangeRequired=0f;
			float fDistanceReduction=0f;

			if (targetType.Value==TargetType.Interactable)
			{
				// Treat the distance as closer based on the radius of the object
				//fDistanceReduction=ObjectData.Radius;
				fRangeRequired=this.CollisionRadius.Value*0.75f;

				// Check if it's in our interactable range dictionary or not
				int iTempRange;
				if (CacheIDLookup.dictInteractableRange.TryGetValue(this.SNOID, out iTempRange))
				{
					fRangeRequired=(float)iTempRange;
				}
				// Treat the distance as closer if the X & Y distance are almost point-blank, for objects
				if (this.RadiusDistance<=2f)
					fDistanceReduction+=1f;

				base.DistanceFromTarget=Vector3.Distance(Bot.Character.Position, this.Position)-fDistanceReduction;

			}
			else
			{
				fDistanceReduction=(this.Radius*0.33f);
				fRangeRequired=8f;


				if (Bot.Character.Position.Distance(this.Position)<=1.5f)
					fDistanceReduction+=1f;

				base.DistanceFromTarget=base.RadiusDistance-fDistanceReduction;

			}



			return (fRangeRequired<=0f||base.DistanceFromTarget<=fRangeRequired);
		}

		public override bool ObjectIsSpecial
		{
			 get
			 {
				  //Rep Chests
				  return this.IsResplendantChest&&Bot.Settings.Targeting.UseExtendedRangeRepChest;
			 }
		}
	}
}