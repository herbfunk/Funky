using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using FunkyBot.Cache.Enums;
using FunkyBot.Game;
using FunkyBot.Movement;
using FunkyBot.Player.HotBar.Skills;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;

namespace FunkyBot.Cache.Objects
{

	public class CacheUnit : CacheObject
	{
		public CacheUnit(CacheObject baseobj)
			: base(baseobj)
		{
			//update properties
		}

		public DiaUnit ref_DiaUnit { get; set; }

		public override TargetProperties Properties
		{
			get
			{
				return AbilityLogicConditions.EvaluateUnitProperties(this);
			}
		}
		public override void UpdateProperties()
		{
			base.UpdateProperties();
			/*
				   Things we know about the object based on cached values
			*/

			//if (this.IsBoss)
			//	 this.Properties|=TargetProperties.Boss;

			//if (this.IsBurrowableUnit)
			//	 this.Properties|=TargetProperties.Burrowing;

			//if (this.IsMissileReflecting)
			//	 this.Properties|=TargetProperties.MissileReflecting;

			//if (this.IsStealthableUnit)
			//	 this.Properties|=TargetProperties.Stealthable;

			//if (this.IsSucideBomber)
			//	 this.Properties|=TargetProperties.SucideBomber;

			//if (this.IsTreasureGoblin)
			//	 this.Properties|=TargetProperties.TreasureGoblin;

			//if (this.IsFast)
			//	 this.Properties|=TargetProperties.Fast;

			//if (this.Monstersize.HasValue&&this.Monstersize.Value==MonsterSize.Ranged)
			//	 this.Properties|=TargetProperties.Ranged;
			base.Properties = AbilityLogicConditions.EvaluateUnitProperties(this);
		}


		#region Monster Affixes Related
		//TODO:: Add property for Reflect -- And check for animation.

		private bool CheckedMonsterAffixes_;
		private void CheckMonsterAffixes(MonsterAffixes theseaffixes)
		{
			MonsterRare = theseaffixes.HasFlag(MonsterAffixes.Rare);
			MonsterUnique = theseaffixes.HasFlag(MonsterAffixes.Unique);
			MonsterElite = theseaffixes.HasFlag(MonsterAffixes.Elite);
			MonsterMinion = theseaffixes.HasFlag(MonsterAffixes.Minion);

			if (IsEliteRareUnique)
			{
				////Type Properties
				//if (this.MonsterRare||this.MonsterElite||this.MonsterMinion)
				//	 this.Properties|=TargetProperties.RareElite;

				//if (MonsterUnique)
				//	 this.Properties|=TargetProperties.Unique;

				MonsterShielding = theseaffixes.HasFlag(MonsterAffixes.Shielding);
				//if (MonsterShielding)
				//	 this.Properties|=TargetProperties.Shielding;

				MonsterMissileDampening = theseaffixes.HasFlag(MonsterAffixes.MissileDampening);
				//if (this.MonsterMissileDampening)
				//	 this.Properties|=TargetProperties.MissileDampening;

				MonsterIlluionist = theseaffixes.HasFlag(MonsterAffixes.Illusionist);
				MonsterExtraHealth = theseaffixes.HasFlag(MonsterAffixes.ExtraHealth);
				MonsterLifeLink = theseaffixes.HasFlag(MonsterAffixes.HealthLink);
				MonsterReflectDamage = theseaffixes.HasFlag(MonsterAffixes.ReflectsDamage);
				MonsterTeleport = theseaffixes.HasFlag(MonsterAffixes.Teleporter);
				MonsterElectrified = theseaffixes.HasFlag(MonsterAffixes.Electrified);
				MonsterFast = theseaffixes.HasFlag(MonsterAffixes.Fast);
				MonsterNormal = false;
			}
			else
			{
				MonsterFast = false;
				MonsterShielding = false;
				MonsterMissileDampening = false;
				MonsterIlluionist = false;
				MonsterExtraHealth = false;
				MonsterLifeLink = false;
				MonsterReflectDamage = false;
				MonsterTeleport = false;
				MonsterElectrified = false;
				MonsterNormal = !IsBoss && !IsTreasureGoblin;
			}

			CheckedMonsterAffixes_ = true;
		}
		public bool MonsterRare { get; set; }
		public bool MonsterUnique { get; set; }
		public bool MonsterElite { get; set; }
		public bool MonsterMinion { get; set; }
		public bool MonsterNormal { get; set; }

		public bool MonsterShielding { get; set; }
		public bool MonsterMissileDampening { get; set; }
		public bool MonsterIlluionist { get; set; }
		public bool MonsterExtraHealth { get; set; }
		public bool MonsterLifeLink { get; set; }
		public bool MonsterReflectDamage { get; set; }
		public bool MonsterElectrified { get; set; }
		public bool MonsterTeleport { get; set; }
		public bool MonsterFast { get; set; }

		public bool IsEliteRareUnique
		{
			get
			{
				return (MonsterRare || MonsterUnique || MonsterElite || MonsterMinion);
			}
		}
		#endregion

		private bool ismoving;
		public bool IsMoving
		{
			get { return ismoving; }
			set { ismoving = value; }
		}

		private DateTime LastAvoidanceIgnored = DateTime.Today;
		private bool? IsNPC { get; set; }
		public bool ForceLeap { get; set; }

		///<summary>
		///Check of Certain Properties that refreshes the SNOAnim during Update method.
		///</summary>
		public bool ShouldUpdateSNOAnim
		{
			get
			{
				return IsMissileReflecting || MonsterTeleport || IsTransformUnit;
			}
		}

		private bool? hasDOTdps_;
		public bool? HasDOTdps
		{
			get
			{
				return hasDOTdps_;
			}
			set
			{

				//update properties
				//if (value.Value==true&&!AbilityLogicConditions.CheckTargetPropertyFlag(this.Properties,TargetProperties.DOTDPS))
				//	 this.Properties|=TargetProperties.DOTDPS;
				//else if (value.Value==false&&AbilityLogicConditions.CheckTargetPropertyFlag(this.Properties,TargetProperties.DOTDPS))
				//	 this.Properties&=TargetProperties.DOTDPS;

				hasDOTdps_ = value;


			}
		}
		public bool QuestMonster { get; set; }
		public bool? IsTargetable { get; set; }
		public bool? IsAttackable { get; set; }
		public bool IsTargetableAndAttackable
		{
			get
			{
				return ((IsAttackable.HasValue && IsAttackable.Value)
						   && (IsTargetable.HasValue && IsTargetable.Value)
						   && (!IsBurrowed.HasValue || !IsBurrowed.Value));
			}
		}
		//public int? KillRadius { get; set; }
		internal bool? burrowed_;
		public bool? IsBurrowed
		{
			get
			{
				return burrowed_;
			}
			set
			{

				if (value.Value && (!CanBurrow.HasValue || !CanBurrow.Value))
					CanBurrow = true;

				burrowed_ = value;
			}
		}

		public bool IsRanged
		{
			get
			{
				return (ObjectCache.SnoUnitPropertyCache.RangedUnits.Contains(SNOID) || (Monstersize.HasValue && Monstersize.Value == MonsterSize.Ranged));
			}
		}
		public bool IsFast
		{
			get
			{
				return (ObjectCache.SnoUnitPropertyCache.FastUnits.Contains(SNOID) || MonsterFast);
			}
		}

		public bool ShouldFlee
		{
			get
			{
				return ((!BeingIgnoredDueToClusterLogic || PriorityCounter > 0) && //not ignored because of clusters
							  (!IsBurrowed.HasValue || !IsBurrowed.Value) && //ignore burrowed!
							  (!IsTreasureGoblin) &&
							  (!IsFast || !Bot.Settings.Fleeing.FleeUnitIgnoreFast) &&
							  ((IsEliteRareUnique && Bot.Settings.Fleeing.FleeUnitRareElite) || (!IsEliteRareUnique && Bot.Settings.Fleeing.FleeUnitNormal)) &&
							  (!MonsterElectrified || Bot.Settings.Fleeing.FleeUnitElectrified) &&
							  (UnitMaxHitPointAverageWeight > 0 || !Bot.Settings.Fleeing.FleeUnitAboveAverageHitPoints) &&
							  (!IsSucideBomber || !Bot.Settings.Fleeing.FleeUnitIgnoreSucideBomber) &&
							  (!IsRanged || !Bot.Settings.Fleeing.FleeUnitIgnoreRanged));


			}
		}

		public bool BeingIgnoredDueToClusterLogic
		{
			get
			{

				if (ProfileCache.ClusterSettingsTag.EnableClusteringTargetLogic
					&& (!ProfileCache.ClusterSettingsTag.IgnoreClusteringWhenLowHP || Bot.Character.Data.dCurrentHealthPct > ProfileCache.ClusterSettingsTag.IgnoreClusterLowHPValue)
					&& !Bot.IsInNonCombatBehavior && !Bot.Character.Data.bIsInBossEncounter)
				{
					//Check if this unit is valid based on if its contained in valid clusters
					if (!Bot.Targeting.Cache.Clusters.ValidClusterUnits.Contains(RAGUID))
					//&&(!this.ObjectIsSpecial&&(!Bot.Targeting.Cache.Environment.AvoidanceLastTarget||this.CentreDistance>59f)))
					{
						return true;
					}
				}

				return false;
			}
		}
		public bool IsClusterException
		{
			get
			{
				return
					 (QuestMonster) ||
					 (IsSucideBomber && Bot.Settings.Targeting.UnitExceptionSucideBombers) ||
					 (IsTreasureGoblin && Bot.Settings.Ranges.TreasureGoblinRange > 1) ||
					 (IsRanged && Bot.Settings.Targeting.UnitExceptionRangedUnits) ||
					 (IsSpawnerUnit && Bot.Settings.Targeting.UnitExceptionSpawnerUnits) ||
					 ((Bot.Settings.Targeting.UnitExceptionLowHP && ((CurrentHealthPct < 0.25 && UnitMaxHitPointAverageWeight > 0)
									   && ((!Bot.Character.Class.IsMeleeClass && CentreDistance < 30f) || (Bot.Character.Class.IsMeleeClass && RadiusDistance < 12f)))));
			}
		}

		public bool AllowLOSMovement
		{
			get
			{
				return
					 ((IsSucideBomber && Bot.Settings.LOSMovement.AllowSucideBomber) ||
					 (IsTreasureGoblin && Bot.Settings.LOSMovement.AllowTreasureGoblin) ||
					 (IsSpawnerUnit && Bot.Settings.LOSMovement.AllowSpawnerUnits) ||
					 ((MonsterRare || MonsterElite) && Bot.Settings.LOSMovement.AllowRareElites) ||
					 ((IsBoss || MonsterUnique) && Bot.Settings.LOSMovement.AllowUniqueBoss) ||
					 (IsRanged && Bot.Settings.LOSMovement.AllowRanged)
					 &&//Enforce A Maximum Range
					 CentreDistance <= Bot.Settings.LOSMovement.MaximumRange);
			}
		}


		#region Health Related
		//Monter Hitpoints
		public double? MaximumHealth { get; set; }

		internal DateTime LastHealthChange = DateTime.Today;
		private double LastCurrentHealth_;
		public double LastCurrentHealth
		{
			get { return LastCurrentHealth_; }
			set { LastCurrentHealth_ = value; LastHealthChange = DateTime.Now; }
		}

		public double? CurrentHealthPct { get; set; }
		private int HealthChecks;

		public int UnitMaxHitPointAverageWeight
		{
			//Using Average Hitpoint Value from ObjectCache we can divide this units value by the average and give it a value
			//-2 == more than 25% below average
			//-1 == between 11% and 25% below average
			//0 == between 10% from average either more or less
			//1 == between 11% and 25% above average
			//2 == more than 25% above average

			get
			{
				if (!MaximumHealth.HasValue) return 0;

				double averageRatio = (MaximumHealth.Value / ObjectCache.Objects.MaximumHitPointAverage);

				int assignedWeight;

				if (averageRatio >= 0.9 && averageRatio <= 1.10)
					assignedWeight = 0;
				else if (averageRatio < 0.25)
					assignedWeight = -2;
				else if (averageRatio < 0.9)
					assignedWeight = -1;
				else if (averageRatio > 1.25)
					assignedWeight = 2;
				else
					assignedWeight = 1;

				//if (assignedWeight<0&&!AbilityLogicConditions.CheckTargetPropertyFlag(this.Properties,TargetProperties.Weak))
				//	 this.Properties|=TargetProperties.Weak;

				return (assignedWeight);
			}
		}

		///<summary>
		///This only updates hitpoints every 5th call to help reduce CPU!
		///</summary>
		public bool UpdateHitPoints()
		{
			//Last Target skips the counter checks
			if (this == Bot.Targeting.Cache.LastCachedTarget)
			{
				UpdateCurrentHitPoints();
				return true;
			}


			HealthChecks++;

			if (HealthChecks > 6)
				HealthChecks = 1;

			if (HealthChecks == 1)
				UpdateCurrentHitPoints();

			return true;
		}

		///<summary>
		///Updates current health percent by reading current health from DiaUnit and dividing it by cache value maximum health.
		///</summary>
		[DebuggerNonUserCode]
		public void UpdateCurrentHitPoints()
		{
			double dThisCurrentHealth;


			using (ZetaDia.Memory.AcquireFrame())
			{
				try
				{
					try
					{
						dThisCurrentHealth = ref_DiaUnit.HitpointsCurrent;
					}
					catch (NullReferenceException)
					{
						return;
					}

					if (!MaximumHealth.HasValue)
						MaximumHealth = ref_DiaUnit.CommonData.GetAttribute<float>(ActorAttributeType.HitpointsMax);

				}
				catch (AccessViolationException)
				{
					// This happens so frequently in DB/D3 that this fails, let's not even bother logging it anymore
					//Logger.DBLog.DebugFormat("[GilesTrinity] Safely handled exception getting current health for unit " + tmp_sThisInternalName + " [" + tmp_iThisActorSNO.ToString() + "]");
					// Add this monster to our very short-term ignore list
					NeedsRemoved = true;
					return;
				}
			}



			// And finally put the two together for a current health percentage
			double dCurrentHealthPct = dThisCurrentHealth / MaximumHealth.Value;
			if (dCurrentHealthPct != CurrentHealthPct)
			{
				LastCurrentHealth = CurrentHealthPct.HasValue ? CurrentHealthPct.Value : 1d;
				CurrentHealthPct = dCurrentHealthPct;

				//update properties
				//if (dCurrentHealthPct==1)
				//	 this.Properties|=TargetProperties.FullHealth;
				//else if (AbilityLogicConditions.CheckTargetPropertyFlag(this.Properties,TargetProperties.FullHealth))
				//	 this.Properties&=TargetProperties.FullHealth;
			}
		}



		#endregion

		internal override GPRectangle GPRect
		{
			get
			{
				if (base.GPRect.CreationVector != Position)
					base.GPRect = new GPRectangle(Position, (int)Math.Sqrt(ActorSphereRadius.Value) * 2);

				return base.GPRect;
			}
		}


		public override bool IsZDifferenceValid
		{
			get
			{
				float fThisHeightDifference = Funky.Difference(Bot.Character.Data.Position.Z, Position.Z);
				if (fThisHeightDifference > 12f)
				{
					//raycast.. 
					// if (!Navigation.CanRayCast(Bot.Character_.Data.Position, this.Position))
					//{
					//	 return false;
					//}
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

		private double KillRadius
		{
			get
			{
				//Set our current radius to the settings of profile.
				double dUseKillRadius = Bot.Targeting.Cache.iCurrentMaxKillRadius;


				// Special short-range list to ignore weakling mobs
				if (CacheIDLookup.hashActorSNOShortRangeOnly.Contains(SNOID)) dUseKillRadius = 12;

				// Prevent long-range mobs beign ignored while they may be pounding on us
				if (dUseKillRadius <= 30 && ObjectCache.SnoUnitPropertyCache.RangedUnits.Contains(SNOID)) dUseKillRadius = 30;


				// Bosses get extra radius
				if (IsBoss)
				{
					// Kulle Exception
					if (SNOID != 80509) dUseKillRadius *= 1.5;

					//more if they're already injured
					if (CurrentHealthPct <= 0.98) dUseKillRadius *= 4;

					// And make sure we have a MINIMUM range for bosses - incase they are at screen edge etc.
					if (dUseKillRadius <= 200)
						if (SNOID == 218947 || SNOID == 256000)
							dUseKillRadius = 75;
						else if (SNOID != 80509) //Kulle Exception
							dUseKillRadius = 200;
				}
				// Tressure Goblins
				else if (IsTreasureGoblin)
				{
					//Check if this goblin is in combat and we are not to close..

					if (CurrentHealthPct.Value >= 1d
						 && RadiusDistance > 20f)
					{
						//Lets calculate if we want to bum rush this goblin by checking surrounding units.

						List<CacheUnit> surroundingList;
						ObjectCache.Objects.FindSurroundingObjects(Position, 50f, out surroundingList);
						surroundingList.RemoveAll(p => !p.IsEliteRareUnique && !p.IsBoss);
						surroundingList.TrimExcess();

						if (surroundingList.Count > 0)
						{
							//See if any of those elites/rares/bosses are closer than the goblin but further than 20f from the goblin..
							float distanceFromGoblin = RadiusDistance;
							if (surroundingList.Any(u => u.RadiusDistance < distanceFromGoblin
																  && u.Position.Distance(Position) > 20f))
							{
								return 0f;
							}


							//Either no above average units or they are farther than the goblin is..
							//We will let calculations below preform instead!
						}
					}


					//Use a shorter range if not yet noticed..
					if (CurrentHealthPct <= 0.10)
						dUseKillRadius += Bot.Settings.Ranges.TreasureGoblinRange + (Bot.Settings.Targeting.GoblinPriority * 24);
					else if (CurrentHealthPct <= 0.99)
						dUseKillRadius += Bot.Settings.Ranges.TreasureGoblinRange + (Bot.Settings.Targeting.GoblinPriority * 16);
					else
						dUseKillRadius += Bot.Settings.Ranges.TreasureGoblinRange + (Bot.Settings.Targeting.GoblinPriority * 12);

					ForceLeap = true;
				}
				// Elitey type mobs and things
				else if ((IsEliteRareUnique))
				{
					dUseKillRadius += Bot.Settings.Ranges.EliteCombatRange;
					ForceLeap = true;
				}
				else
					//Not Boss, Goblin, Elite/Rare/Unique..
					dUseKillRadius += Bot.Settings.Ranges.NonEliteCombatRange;


				// Standard 50f range when preforming OOC behaviors!
				if (Bot.IsInNonCombatBehavior)
					dUseKillRadius = Bot.Settings.Plugin.OutofCombatMaxDistance;

				return dUseKillRadius;
			}
		}

		private void TallyTarget()
		{
			bool bIsRended;
			bool bCountAsElite;

			bIsRended = (HasDOTdps.HasValue && HasDOTdps.Value);
			bCountAsElite = (IsEliteRareUnique || IsTreasureGoblin || IsBoss);
			float RadiusDistance = this.RadiusDistance;

			if (Bot.Settings.Fleeing.EnableFleeingBehavior && RadiusDistance <= Bot.Settings.Fleeing.FleeMaxMonsterDistance && ShouldFlee)
			{
				Bot.Targeting.Cache.Environment.FleeTriggeringUnits.Add(this);
			}

			if (RadiusDistance <= 6f)
			{
				Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_6]++;
				if (bCountAsElite)
					Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_6]++;
			}
			if (RadiusDistance <= 12f)
			{
				//Tally close units
				Bot.Targeting.Cache.Environment.SurroundingUnits++;
				//Herbfunk: non-rend count only if within 8f and is attackable..
				if (Bot.Character.Class.AC == ActorClass.Barbarian && !bIsRended && RadiusDistance <= 7f && IsTargetable.Value)
					Bot.Targeting.Cache.Environment.iNonRendedTargets_6++;

				Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_12]++;
				if (bCountAsElite)
					Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_12]++;
			}
			if (RadiusDistance <= 15f)
			{
				Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]++;
				if (bCountAsElite)
					Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_15]++;
			}
			if (RadiusDistance <= 20f)
			{
				Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20]++;
				if (bCountAsElite)
					Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_20]++;
			}
			if (RadiusDistance <= 25f)
			{
				if (!Bot.Targeting.Cache.Environment.bAnyNonWWIgnoreMobsInRange && !CacheIDLookup.hashActorSNOWhirlwindIgnore.Contains(SNOID))
					Bot.Targeting.Cache.Environment.bAnyNonWWIgnoreMobsInRange = true;
				Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_25]++;
				if (bCountAsElite)
					Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_25]++;
			}
			if (RadiusDistance <= 30f)
			{
				Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_30]++;
				if (bCountAsElite)
					Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_30]++;
			}
			if (RadiusDistance <= 40f)
			{
				Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_40]++;
				if (bCountAsElite)
					Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_40]++;
			}
			if (RadiusDistance <= 50f)
			{
				Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_50]++;
				if (bCountAsElite)
					Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_50]++;
			}
		}

		public override void UpdateWeight()
		{
			base.UpdateWeight();

			//Ignore non-clustered, *only when not prioritized!*

			if (BeingIgnoredDueToClusterLogic
				 && PriorityCounter == 0
				 && !IsClusterException
				 && (Bot.Targeting.Cache.CurrentTarget != null
				 || Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_30] == 0
				 || Bot.Targeting.Cache.objectsIgnoredDueToAvoidance.Count == 0))
			{
				Weight = 0;
				return;
			}

			if (RadiusDistance >= 5f && Bot.Character.Class.IsMeleeClass)
			{
				if (DateTime.Now.Subtract(LastAvoidanceIgnored).TotalMilliseconds < 1000 && Bot.Targeting.Cache.Environment.NearbyAvoidances.Count > 0)
				{
					Weight = 1;
				}
				else
				{
					Vector3 TestPosition = BotMeleeVector;
					if (ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(TestPosition))
						Weight = 1;

					//intersecting avoidances..
					if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(TestPosition))
					{
						if (Weight != 1) //&& ObjectIsSpecial)
						{//Only add this to the avoided list when its not currently inside avoidance area
							Bot.Targeting.Cache.objectsIgnoredDueToAvoidance.Add(this);
						}
						else
							Weight = 1;
					}
				}
			}

			//Range Class Ignore (Avoid/Kite last target!)
			//if ((Bot.Targeting.Cache.Environment.FleeingLastTarget&&Bot.Targeting.Cache.Environment.FleeTriggeringUnits.Count>0&&Bot.Targeting.Cache.Environment.FleeTriggeringUnits.Contains(this))||
			//	 (Bot.Targeting.Cache.Environment.AvoidanceLastTarget&&Bot.Targeting.Cache.Environment.TriggeringAvoidances.Count>0))
			//	 this.Weight=1;


			if (Weight != 1)
			{
				float centreDistance = CentreDistance;
				float radiusDistance = RadiusDistance;

				// Flag up any bosses in range
				if (IsBoss && centreDistance <= 50f)
					Weight += 9999;

				// Force a close range target because we seem to be stuck *OR* if not ranged and currently rooted
				if (Bot.Targeting.Cache.bPrioritizeCloseRangeUnits || Bot.Character.Data.bIsRooted)
				{

					Weight = 20000 - (Math.Floor(radiusDistance) * 200);

					// Goblin priority KAMIKAZEEEEEEEE
					if (IsTreasureGoblin && Bot.Settings.Targeting.GoblinPriority > 1)
						Weight += 10250 * (Bot.Settings.Targeting.GoblinPriority - 1);
				}
				else
				{
					// Not attackable, could be shielded, make super low priority
					if (!IsTargetableAndAttackable && !IsWormBoss)
					{
						// Only 500 weight helps prevent it being prioritized over an unshielded
						Weight = 500;
					}
					// Not forcing close-ranged targets from being stuck, so let's calculate a weight!
					else
					{
						// Starting weight of 5000 to beat a lot of crap weight stuff
						Weight = 5000;

						// Distance as a percentage of max radius gives a value up to 1000 (1000 would be point-blank range)

						if (radiusDistance < Bot.Targeting.Cache.iCurrentMaxKillRadius)
						{
							int RangeModifier = 1200;
							//Increase Distance Modifier if recently kited.
							if (Bot.Settings.Fleeing.EnableFleeingBehavior && DateTime.Now.Subtract(Bot.Targeting.Cache.LastFleeAction).TotalMilliseconds < 3000)
								RangeModifier = 12000;


							Weight += (RangeModifier * (1 - (radiusDistance / Bot.Targeting.Cache.iCurrentMaxKillRadius)));
						}

						// Give extra weight to ranged enemies
						if (Bot.Character.Class.IsMeleeClass && IsRanged)
						{
							Weight += 1100;
							ForceLeap = true;
						}

						// Give more weight to elites and minions
						if (IsEliteRareUnique)
							Weight += 2000;

						// Give more weight to bosses
						if (IsBoss)
							Weight += 4000;

						// Barbarians with wrath of the berserker up should prioritize elites more
						if (Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_WrathOfTheBerserker) && (IsEliteRareUnique || IsTreasureGoblin || IsBoss))
							Weight += 2000;

						// Exploding Palm Bleeding Prioritize
						if (Bot.Character.Class.AC == ActorClass.Monk 
							&& Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_ExplodingPalm))
						{
							if (HasDOTdps.HasValue && HasDOTdps.Value) //Exploding Palm -- Bleeding Already!
							{
								Weight += 500;
							}
							else if(CurrentHealthPct<0.15d) //Exploding Palm -- Low HP, Not Bleeding!
							{
								Weight += 1000;
							}
						}
														


						// Swarmers/boss-likes get more weight
						if (Monstersize == MonsterSize.Swarm || Monstersize == MonsterSize.Boss)
							Weight += 900;

						// Standard/big get a small bonus incase of "unknown" monster types being present
						if (Monstersize == MonsterSize.Standard || Monstersize == MonsterSize.Big)
							Weight += 150;

						// Lower health gives higher weight - health is worth up to 300 extra weight
						if (CurrentHealthPct < 0.20)
							Weight += (300 * (1 - (CurrentHealthPct.Value / 0.5)));

						// Elites on low health get extra priority - up to 1500
						if ((IsEliteRareUnique || IsTreasureGoblin) && CurrentHealthPct < 0.20)
							Weight += (1500 * (1 - (CurrentHealthPct.Value / 0.45)));

						// Goblins on low health get extra priority - up to 2500
						if (Bot.Settings.Targeting.GoblinPriority >= 2 && IsTreasureGoblin && CurrentHealthPct <= 0.98)
							Weight += (3000 * (1 - (CurrentHealthPct.Value / 0.85)));

						// Bonuses to priority type monsters from the dictionary/hashlist set at the top of the code
						int iExtraPriority;
						if (CacheIDLookup.dictActorSNOPriority.TryGetValue(SNOID, out iExtraPriority))
						{
							Weight += iExtraPriority;
						}

						// Close range get higher weights the more of them there are, to prevent body-blocking
						// Plus a free bonus to anything close anyway
						if (radiusDistance <= 11f)
						{
							// Extra bonus for point-blank range
							//iUnitsSurrounding++;
							// Give special "surrounded" weight to each unit
							Weight += (200 * Bot.Targeting.Cache.Environment.SurroundingUnits);
						}

						// Special additional weight for corrupt growths in act 4 ONLY if they are at close range (not a standard priority thing)
						if ((SNOID == 210120 || SNOID == 210268) && centreDistance <= 35f)
							Weight += 2000;

						// Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
						if (this == Bot.Targeting.Cache.LastCachedTarget && centreDistance <= 25f)
							Weight += 400;


						// Prevent going less than 300 yet to prevent annoyances (should only lose this much weight from priority reductions in priority list?)
						if (Weight < 300)
							Weight = 300;

						// Deal with treasure goblins - note, of priority is set to "0", then the is-a-goblin flag isn't even set for use here - the monster is ignored
						if (IsTreasureGoblin)
						{
							// Logging goblin sightings
							if (Bot.Targeting.Cache.lastGoblinTime == DateTime.Today)
							{
								Bot.Targeting.Cache.iTotalNumberGoblins++;
								Bot.Targeting.Cache.lastGoblinTime = DateTime.Now;
								Logger.DBLog.InfoFormat("[Funky] Goblin #" + Bot.Targeting.Cache.iTotalNumberGoblins.ToString(CultureInfo.InvariantCulture) + " in sight. Distance=" + centreDistance);
							}
							else
							{
								if (DateTime.Now.Subtract(Bot.Targeting.Cache.lastGoblinTime).TotalMilliseconds > 30000)
									Bot.Targeting.Cache.lastGoblinTime = DateTime.Today;
							}
							// Original Trinity stuff for priority handling now
							switch (Bot.Settings.Targeting.GoblinPriority)
							{
								case 2:
									// Super-high priority option below... 
									Weight += 10101;
									break;
								case 3:
									// KAMIKAZE SUICIDAL TREASURE GOBLIN RAPE AHOY!
									Weight += 40000;
									break;
								// PS: 58008 is an awesome number on any calculator.

							}
						}
					}
				} // Forcing close range target or not?

			}
			else
			{
				LastAvoidanceIgnored = DateTime.Now;
				if (!ObjectIsSpecial)
					Weight = 0;
			}

		}

		public override bool ObjectIsValidForTargeting
		{
			get
			{
				if (!base.ObjectIsValidForTargeting)
					return false;

				//Z-Height Difference Check
				if (!IsZDifferenceValid)
				{
					//this.BlacklistLoops=3;
					return false;
				}

				#region Validations
				// Unit is already dead
				if (CurrentHealthPct.HasValue && (CurrentHealthPct.Value <= 0d))
				{
					//Respawnable Units -- Only when they are not elite/rare/uniques!
					if (!IsRespawnable || IsEliteRareUnique)
					{
						BlacklistLoops = -1;
						NeedsRemoved = true;
						return false;
					}
					BlacklistLoops = 5;
					return false;
				}

				#endregion

				//Attackable/Burrowed?
				if ((IsTargetable.HasValue && IsTargetable == false) ||
					   (IsBurrowed.HasValue && IsBurrowed.Value) ||
					  (IsAttackable.HasValue && IsAttackable.Value == false))
				{
					//We skip all but worm bosses in A2 and monsters who can shield.
					if (!IsWormBoss && !MonsterShielding && (!IsEliteRareUnique || IsGrotesqueActor))
					{
						if (IsGrotesqueActor)
						{
							//Setup this as an avoidance object now!
							HandleAsAvoidanceObject = true;
							CacheAvoidance newAvoidance = new CacheAvoidance(this, AvoidanceType.GrotesqueExplosion);
							ObjectCache.Obstacles[RAGUID] = newAvoidance;

							return false;
						}

						//Stealthable units -- low blacklist counter
						if (IsStealthableUnit)
							BlacklistLoops = 2;
						else if (IsBurrowableUnit)
							BlacklistLoops = 5;
						else
							BlacklistLoops = 10;

						return false;
					}
				}






				float centreDistance = CentreDistance;
				bool distantUnit = false;
				bool validUnit = false;
				//Distant Units List
				if (!Bot.IsInNonCombatBehavior && Bot.Settings.Grouping.AttemptGroupingMovements && centreDistance >= Bot.Settings.Grouping.GroupingMinimumUnitDistance)
				{
					distantUnit = true;
				}

				if (centreDistance > KillRadius && !distantUnit)
				{
					//Since special objects are subject to LOS movement, we do not ignore just yet.
					if (!AllowLOSMovement)
						return false;
				}
				else
					validUnit = true;

				//Line of sight pre-check
				if (RequiresLOSCheck)
				{
					//Get the wait time since last used LOSTest
					double lastLOSCheckMS = LineOfSight.LastLOSCheckMS;

					//unless its in front of us.. we wait 500ms mandatory.
					if (lastLOSCheckMS < 500 && centreDistance > 1f)
					{
						// if (this.IsEliteRareUnique||this.IsTreasureGoblin)
						if (AllowLOSMovement)
						{
							//if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Target))
							//	Logger.Write(LogLevel.Target, "Adding {0} to LOS Movement Objects", InternalName);
							Bot.Targeting.Cache.Environment.LoSMovementObjects.Add(this);
						}

						return false;
					}
					//Set the maximum wait time
					double ReCheckTime = 5500;

					//health recently changed
					if (DateTime.Now.Subtract(LastHealthChange).TotalMilliseconds < 5000)
						ReCheckTime *= 0.75;

					//Close Range.. we change the recheck timer based on how close
					if (CentreDistance < 25f)
						ReCheckTime = (CentreDistance * 125);
					else if (ObjectIsSpecial)
						ReCheckTime *= 0.25;

					if (lastLOSCheckMS < ReCheckTime)
					{
						//if (this.IsEliteRareUnique||this.IsTreasureGoblin) 
						if (AllowLOSMovement)
						{
							//if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Target))
							//	Logger.Write(LogLevel.Target, "Adding {0} to LOS Movement Objects", InternalName);
							Bot.Targeting.Cache.Environment.LoSMovementObjects.Add(this);
						}
						return false;
					}

					//This is intial test to validate we can "see" the unit.. 
					if (!LineOfSight.LOSTest(Bot.Character.Data.Position, true, false, NavCellFlags.None, false))
					{
						//LOS Movement -- Check for special objects
						//LOS failed.. now we should decide if we want to find a spot for this target, or just ignore it.
						// if (this.IsEliteRareUnique||this.IsTreasureGoblin)
						if (AllowLOSMovement)
						{
							Logger.Write(LogLevel.Target, "Adding {0} to LOS Movement Objects", InternalName);
							Bot.Targeting.Cache.Environment.LoSMovementObjects.Add(this);
						}

						//Valid?? Did we find a location we could move to for LOS?
						if (!Bot.Character.Data.bIsIncapacitated && !ObjectIsSpecial)
							BlacklistLoops = 2;
						else//Incapacitated we reset check
							LineOfSight.LastLOSCheck = DateTime.Today;

						return false;
					}

					RequiresLOSCheck = false;
				}






				#region CombatFlags

				if (IsBoss || IsEliteRareUnique)
				{
					//Ignore Setting?
					if (Bot.Settings.Targeting.IgnoreAboveAverageMobs && PriorityCounter <= 1 && !Bot.IsInNonCombatBehavior && !IsBoss)
						return false;

					Bot.Targeting.Cache.Environment.bAnyChampionsPresent = true;
				}

				if (IsTreasureGoblin)
					Bot.Targeting.Cache.Environment.bAnyTreasureGoblinsPresent = true;



				// Total up monsters at various ranges
				if (centreDistance <= 50f)
				{
					TallyTarget();
				}

				#endregion


				//Profile Blacklisted.
				if (!Bot.Settings.Ranges.IgnoreProfileBlacklists && BlacklistCache.hashProfileSNOTargetBlacklist.Contains(SNOID))
				{
					//Only if not prioritized..
					if (!Bot.IsInNonCombatBehavior)
						return false;
				}

				if (distantUnit)
					Bot.Targeting.Cache.Environment.DistantUnits.Add(this); //Add this valid unit to Distant List.
				if (validUnit) //Add this valid unit RAGUID to list
					Bot.Targeting.Cache.Environment.UnitRAGUIDs.Add(RAGUID);



				return true;
			}
		}

		public override bool UpdateData()
		{

			if (!base.IsStillValid())
				return false;

			if (ref_DiaUnit == null)
			{
				try
				{
					ref_DiaUnit = ref_DiaObject as DiaUnit;
				}
				catch
				{
					Logger.Write(LogLevel.Cache, "Failure to convert obj to DiaUnit!");

					NeedsRemoved = true;
					return false;
				}
			}

			ACD CommonData = ref_DiaObject.CommonData;
			if (CommonData == null)
			{
				Logger.Write(LogLevel.Cache, "Common Data Null!");
				return false;
			}


			if (!Monstertype.HasValue)
				return false;
			//Update Monster Type?
			if (ShouldRefreshMonsterType)
			{
				if (!base.UpdateData(ref_DiaObject, RAGUID))
					return false;
			}

			//NPC Check
			bool isNPC = false;
			if (!IsNPC.HasValue || IsNPC.Value)
			{
				try
				{
					IsNPC = (ref_DiaUnit.CommonData.GetAttribute<float>(ActorAttributeType.IsNPC) > 0);
					isNPC = IsNPC.Value;
				}
				catch (Exception)
				{
					Logger.Write(LogLevel.Cache, "Safely Handled Getting Attribute IsNPC for object {0}", InternalName);
				}

			}



			// Make sure it's a valid monster type
			if (!MonsterTypeIsHostile() || isNPC)
			{
				if (Bot.Character.Data.bIsInTown)
				{
					//Perma Ignore all NPCs we find in town..
					if (isNPC)
						BlacklistCache.IgnoreThisObject(this);

					return false;
				}

				if (isNPC || !IsBoss)
					return false;
			}



			//Position update
			base.UpdatePosition();

			if (Radius == 0f)
			{
				if (ActorSphereRadius.HasValue)
				{
					Radius = ActorSphereRadius.Value;

					//if (this.Monstersize.Value==Zeta.Internals.SNO.MonsterSize.Big||this.Monstersize.Value==Zeta.Internals.SNO.MonsterSize.Boss)
					//	 this.Radius*=0.25f;

					if (Radius < 0f)
						Radius = 1f;
				}
			}

			//Affixes
			if (!CheckedMonsterAffixes_)
			{
				try
				{
					CheckMonsterAffixes(CommonData.MonsterAffixes);
				}
				catch
				{
					Logger.Write(LogLevel.Cache, "Failure to check monster affixes for unit {0}", InternalName);
					return false;
				}

			}

			//Hitpoints
			if (!MaximumHealth.HasValue)
			{
				try
				{
					MaximumHealth = ref_DiaUnit.HitpointsMaxTotal;

					if (!IsEliteRareUnique || !IsBoss)
					{
						if (!ObjectCache.Objects.HealthEntriesForAverageValue.ContainsKey(RAGUID))
						{
							ObjectCache.Objects.HealthEntriesForAverageValue.Add(RAGUID, MaximumHealth.Value);
							ObjectCache.Objects.UpdateMaximumHealthAverage();
						}
					}
				}
				catch
				{
					Logger.Write(LogLevel.Cache, "Failure to get maximum health for {0}", InternalName);
					return false;
				}
			}


			//update HPs
			UpdateHitPoints();

			if (CurrentHealthPct.HasValue && CurrentHealthPct.Value <= 0d)
			{
				NeedsRemoved = true;
				return false;
			}



			//Burrowing?
			#region Burrowed?
			if ((CurrentHealthPct.HasValue && CurrentHealthPct.Value >= 1d || this.IsBurrowableUnit || (!IsBurrowed.HasValue || IsBurrowed.Value)))
			{
				try
				{
					//this.IsBurrowed=this.ref_DiaUnit.IsBurrowed;
					IsBurrowed = ref_DiaUnit.CommonData.GetAttribute<float>(ActorAttributeType.Burrowed) > 0;

					//ignore units who are stealthed completly (exception when object is special!)
					//if (this.IsBurrowed.Value&&!this.ObjectIsSpecial)
					//return false;
				}
				catch (Exception) { }
			}
			#endregion

			//Targetable
			if (!IsTargetable.HasValue || !IsTargetable.Value || IsStealthableUnit || IsBoss)
			{
				try
				{
					//this.IsAttackable=this.ref_DiaUnit.IsAttackable;
					bool stealthed = false;
					//Special units who can stealth
					if (IsStealthableUnit)
						stealthed = (ref_DiaUnit.CommonData.GetAttribute<float>(ActorAttributeType.Stealthed) <= 0);

					if (!stealthed)
						IsTargetable = (ref_DiaUnit.CommonData.GetAttribute<float>(ActorAttributeType.Untargetable) <= 0);
					else
					{
						IsTargetable = stealthed;
						//since stealth is similar to being burrowed we skip non-special units
						//if (!this.ObjectIsSpecial)
						//return false;
					}
				}
				catch (Exception ex)
				{
					Logger.Write(LogLevel.Cache, "[Funky] Safely handled exception getting is-targetable attribute for unit " + InternalName + " [" + SNOID.ToString(CultureInfo.InvariantCulture) + "]");
					//Logger.DBLog.DebugFormat(ex.ToString());
					IsTargetable = true;
				}
			}

			//Attackable
			if (MonsterShielding || (IsGrotesqueActor && CurrentHealthPct.HasValue && (CurrentHealthPct.Value < 1d || CurrentHealthPct.Value > 1d)))
			{
				try
				{
					IsAttackable = (ref_DiaUnit.IsAttackable);

					//update properties
					//if (this.IsAttackable.Value&&this.IsTargetable.Value)
					//{
					//	 if (!AbilityLogicConditions.CheckTargetPropertyFlag(this.Properties, TargetProperties.TargetableAndAttackable))
					//		  this.Properties|=TargetProperties.TargetableAndAttackable;
					//}
					//else
					//{
					//	 if (AbilityLogicConditions.CheckTargetPropertyFlag(this.Properties, TargetProperties.TargetableAndAttackable))
					//		  this.Properties&=TargetProperties.TargetableAndAttackable;
					//}


				}
				catch (Exception)
				{

				}
			}
			else
				IsAttackable = true;

			#region Class DOT DPS Check
			//Barb specific updates
			if (Bot.Character.Class.AC == ActorClass.Barbarian)
			{
				//Rend DotDPS update
				if (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Barbarian_Rend))
				{
					try
					{
						HasDOTdps = (ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.Bleeding) > 0 && ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.DOTDPS) > 0);
					}
					catch
					{
					}
				}
			}
			else if (Bot.Character.Class.AC == ActorClass.Monk)
			{
				//1195139072
				if (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_ExplodingPalm))
				{
					Bot.Targeting.Cache.Environment.UsesDOTDPSAbility = true;
					try
					{
						int dotDPS = ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.DOTDPS);
						int visualBuff = ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.Bleeding);
						HasDOTdps = (dotDPS > 0 && visualBuff > 0);

						//DotDPS values
						//dotDPS==1195139072
						//1215532915 || 1215532915

					}
					catch
					{
					}
				}
			}
			else if (Bot.Character.Class.AC == ActorClass.Witchdoctor)
			{
				//Haunted DotDPS update
				if (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Witchdoctor_Haunt) || Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Witchdoctor_Locust_Swarm))
				{
					Bot.Targeting.Cache.Environment.UsesDOTDPSAbility = true;
					try
					{
						//Haunted units always have buff visual effect!
						//bool buffVisualEffect=(this.ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.BuffVisualEffect)>0);

						int dotDPS = ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.DOTDPS);
						int visualBuff = ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.BuffVisualEffect);
						HasDOTdps = (dotDPS > 0 && visualBuff > 0);
						/*
						if (Bot.Character_.Class.HotbarAbilities.Contains(SNOPower.Witchdoctor_Haunt))
						{
							 this.HasDOTdps=(visualBuff>0&&(dotDPS==1194344448||dotDPS==1194786816||dotDPS==1202929855||dotDPS==1194983424||dotDPS==1196072960||dotDPS==1194770432));
						}
						else if (Bot.Character_.Class.HotbarAbilities.Contains(SNOPower.Witchdoctor_Locust_Swarm))
						{
							 this.HasDOTdps=(visualBuff>0&&(dotDPS==1178820608||dotDPS==1197301760||dotDPS==1182662656));
						}
						*/

						//haunt DotDPS values
						//dotDPS==1194344448||dotDPS==1194786816||dotDPS==1202929855||dotDPS==1194983424||dotDPS==1196072960||dotDPS==1194770432)

						//lotus swarm
						//1178820608 1197301760 1182662656
					}
					catch
					{
					}
				}
			}
			#endregion

			//Update SNOAnim?
			if (ShouldUpdateSNOAnim)
			{
				try
				{
					AnimState = (base.ref_DiaObject.CommonData.AnimationState);
				}
				catch (Exception)
				{
					AnimState = AnimationState.Invalid;
				}
			}

			//Update Quest Monster?
			if (Bot.Targeting.Cache.UpdateQuestMonsterProperty)
			{
				try
				{
					QuestMonster = ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.QuestMonster) != 0;
				}
				catch (Exception)
				{
					QuestMonster = false;
				}
			}

			return true;
		}

		public override bool IsStillValid()
		{
			if (ref_DiaUnit == null || !ref_DiaUnit.IsValid || ref_DiaUnit.BaseAddress == IntPtr.Zero)
				return false;






			return base.IsStillValid();
		}

		public override RunStatus Interact()
		{
			if (Bot.Character.Class.PowerPrime.Power != SNOPower.None)
			{
				if (!Bot.Character.Class.PowerPrime.ActivateSkill())
					return RunStatus.Running;

				//Check health changes -- only when single target or cluster with targeting is used.
				if (Bot.Targeting.Cache.LastCachedTarget.Equals(this) &&
					  DateTime.Now.Subtract(Bot.Character.Class.LastUsedACombatAbility).TotalMilliseconds < 2500 &&
					  DateTime.Now.Subtract(Bot.Targeting.Cache.LastChangeOfTarget).TotalMilliseconds > 3000 &&
					 !Bot.Character.Data.bIsInBossEncounter)
				{
					double LastHealthChangedMS = DateTime.Now.Subtract(LastHealthChange).TotalMilliseconds;
					if (LastHealthChangedMS > 5000)
					{
						Logger.Write(LogLevel.Target, "Ignore Unit {0} due to health last changed of {1}ms", InternalName, LastHealthChangedMS);
						BlacklistLoops = 20;
						Bot.Targeting.Cache.bForceTargetUpdate = true;
					}
				}
			}

			return RunStatus.Running;
		}

		public override bool CanInteract()
		{
			if (!IsTargetable.Value)
				return false;

			return base.CanInteract();
		}

		public override bool WithinInteractionRange()
		{
			float fRangeRequired;

			//Check if we should mod our distance:: used for worm bosses
			if (IsWormBoss)
				Bot.Character.Class.PowerPrime.MinimumRange = Bot.Character.Class.IsMeleeClass ? 14 : 16;
			else if (IgnoresLOSCheck)
				Bot.Character.Class.PowerPrime.MinimumRange = (int)(ActorSphereRadius.Value * 1.5);
			else if (IsBurrowed.HasValue && IsBurrowed.Value && IsEliteRareUnique)//Force close range on burrowed elites!
				Bot.Character.Class.PowerPrime.MinimumRange = 15;
			else if (IsStealthableUnit && IsTargetable.HasValue && IsTargetable.Value == false && IsEliteRareUnique)
				Bot.Character.Class.PowerPrime.MinimumRange = 15;
			else if (IsTreasureGoblin && !Bot.Character.Class.IsMeleeClass && Bot.Settings.Class.GoblinMinimumRange > 0 && Bot.Character.Class.PowerPrime.MinimumRange > Bot.Settings.Class.GoblinMinimumRange)
				Bot.Character.Class.PowerPrime.MinimumRange = Bot.Settings.Class.GoblinMinimumRange;
			else if (MonsterMissileDampening && Bot.Settings.Targeting.MissleDampeningEnforceCloseRange)
				Bot.Character.Class.PowerPrime.MinimumRange = 15;


			// Pick a range to try to reach
			fRangeRequired = Bot.Character.Class.PowerPrime.Power == SNOPower.None ? 9f : Bot.Character.Class.PowerPrime.MinimumRange;

			DistanceFromTarget = RadiusDistance;

			return (fRangeRequired <= 0f || DistanceFromTarget <= fRangeRequired);
		}

		public override bool ObjectIsSpecial
		{
			get
			{
				if ((IsEliteRareUnique && !Bot.Settings.Targeting.IgnoreAboveAverageMobs) ||
						   (PriorityCounter > 0) ||
						   (IsBoss && !Bot.Settings.Targeting.IgnoreAboveAverageMobs && CurrentHealthPct.HasValue && CurrentHealthPct <= 0.99d) ||
						   (((IsSucideBomber && Bot.Settings.Targeting.UnitExceptionSucideBombers) || IsCorruptantGrowth) && CentreDistance < 45f) ||
						   (IsSpawnerUnit && Bot.Settings.Targeting.UnitExceptionSpawnerUnits) ||
						   ((IsTreasureGoblin && Bot.Settings.Targeting.GoblinPriority > 1)) ||
						   (IsRanged && Bot.Settings.Targeting.UnitExceptionRangedUnits
								&& (!IsEliteRareUnique || !Bot.Settings.Targeting.IgnoreAboveAverageMobs)) ||
					//Low HP (25% or Less) & Is Not Considered Weak
						   ((Bot.Settings.Targeting.UnitExceptionLowHP && CurrentHealthPct.HasValue && ((CurrentHealthPct <= 0.25 && UnitMaxHitPointAverageWeight > 0)
									  && (!IsEliteRareUnique || !Bot.Settings.Targeting.IgnoreAboveAverageMobs) && (RadiusDistance <= Bot.Settings.Targeting.UnitExceptionLowHPMaximumDistance)))))



					return true;

				return base.ObjectIsSpecial;
			}
		}

		public override string DebugString
		{
			get
			{
				return String.Format("{0} Burrowed {1} / Targetable {2} / Attackable {3} \r\n HP {4} / MaxHP {5} -- IsMoving: {6} \r\n PriorityCounter={7}\r\nUnit Properties {8}",
					  base.DebugString,
					  IsBurrowed.HasValue ? IsBurrowed.Value.ToString() : "",
					  IsTargetable.HasValue ? IsTargetable.Value.ToString() : "",
					  IsAttackable.HasValue ? IsAttackable.Value.ToString() : "",
					  CurrentHealthPct.HasValue ? CurrentHealthPct.Value.ToString(CultureInfo.InvariantCulture) : "",
					  MaximumHealth.HasValue ? MaximumHealth.Value.ToString(CultureInfo.InvariantCulture) : "",
					  IsMoving,
					  PriorityCounter,
					  Properties);
			}
		}


	}



}