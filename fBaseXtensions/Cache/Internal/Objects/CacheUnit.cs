using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Cache.External.Objects;
using fBaseXtensions.Cache.Internal.Blacklist;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Bounty;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Navigation.Gridpoint;
using fBaseXtensions.Settings;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;
using fBaseXtensions.Game.Hero.Skills.Conditions;

namespace fBaseXtensions.Cache.Internal.Objects
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
				return UnitTargetConditions.EvaluateUnitProperties(this);
			}
		}
		public override void UpdateProperties()
		{
			base.UpdateProperties();
			base.Properties = UnitTargetConditions.EvaluateUnitProperties(this);
		}

		private UnitFlags GenerateUnitFlags()
		{
			UnitFlags _flags;

			if (Properties.HasFlag(TargetProperties.Boss))
				_flags = UnitFlags.Boss;
			else if (Properties.HasFlag(TargetProperties.RareElite))
				_flags = UnitFlags.Rare;
			else if (Properties.HasFlag(TargetProperties.Unique))
				_flags = UnitFlags.Unique;
			else if (Properties.HasFlag(TargetProperties.TreasureGoblin))
				_flags = UnitFlags.TreasureGoblin;
			else if (Properties.HasFlag(TargetProperties.SucideBomber))
				_flags = UnitFlags.SucideBomber;
			else
				_flags = UnitFlags.Normal;


			if (Properties.HasFlag(TargetProperties.Stealthable))
				_flags |= UnitFlags.Stealthable;
			if (Properties.HasFlag(TargetProperties.Burrowing))
				_flags |= UnitFlags.Burrowing;
			if (Properties.HasFlag(TargetProperties.Fast))
				_flags |= UnitFlags.Fast;
			if (Properties.HasFlag(TargetProperties.Ranged))
				_flags |= UnitFlags.Ranged;

			if (IsFlyingHoverUnit)
				_flags |= UnitFlags.Flying;
			if (IsSpawnerUnit)
				_flags |= UnitFlags.Summoner;
			if (IsGrotesqueActor)
				_flags |= UnitFlags.Grotesque;
			if (IsMissileReflecting)
				_flags |= UnitFlags.ReflectiveMissle;

			return _flags;
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
				MonsterFireChains = theseaffixes.HasFlag(MonsterAffixes.FireChains);
				MonsterAvenger = theseaffixes.HasFlag(MonsterAffixes.Avenger);
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
				MonsterFireChains = false;
				MonsterAvenger = false;
				MonsterNormal = !IsBoss && !IsTreasureGoblin;
			}

			CheckedMonsterAffixes_ = true;
			UpdateProperties();
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
		public bool MonsterFireChains { get; set; }
		public bool MonsterAvenger { get; set; }
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
		public bool? IsFriendly { get; set; }
		public bool IsQuestGiver { get; set; }
		public bool? IsMinimapActive { get; set; }

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

		private int dotdpsValue = 0;
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
				return (TheCache.ObjectIDCache.Units.RangedUnits.Contains(SNOID) || (Monstersize.HasValue && Monstersize.Value == MonsterSize.Ranged));
			}
		}
		public bool IsFast
		{
			get
			{
				return (TheCache.ObjectIDCache.Units.FastUnits.Contains(SNOID) || MonsterFast);
			}
		}

		public bool ShouldFlee
		{
			get
			{
				return ((!BeingIgnoredDueToClusterLogic || PriorityCounter > 0) && //not ignored because of clusters
							  (!IsBurrowed.HasValue || !IsBurrowed.Value) && //ignore burrowed!
							  (!IsTreasureGoblin) &&
							  (!IsFast || !FunkyBaseExtension.Settings.Fleeing.FleeUnitIgnoreFast) &&
							  ((IsEliteRareUnique && FunkyBaseExtension.Settings.Fleeing.FleeUnitRareElite) || (!IsEliteRareUnique && FunkyBaseExtension.Settings.Fleeing.FleeUnitNormal)) &&
							  (!MonsterElectrified || FunkyBaseExtension.Settings.Fleeing.FleeUnitElectrified) &&
							  (UnitMaxHitPointAverageWeight > 0 || !FunkyBaseExtension.Settings.Fleeing.FleeUnitAboveAverageHitPoints) &&
							  (!IsSucideBomber || !FunkyBaseExtension.Settings.Fleeing.FleeUnitIgnoreSucideBomber) &&
							  (!IsRanged || !FunkyBaseExtension.Settings.Fleeing.FleeUnitIgnoreRanged));


			}
		}

		public bool BeingIgnoredDueToClusterLogic
		{
			get
			{

				if (SettingCluster.ClusterSettingsTag.EnableClusteringTargetLogic
					&& (FunkyGame.Hero.dCurrentHealthPct > SettingCluster.ClusterSettingsTag.IgnoreClusterLowHPValue)
					&& !FunkyGame.IsInNonCombatBehavior && !FunkyGame.Hero.bIsInBossEncounter)
				{
					//Check if this unit is valid based on if its contained in valid clusters
					if (!FunkyGame.Targeting.Cache.Clusters.ValidClusterUnits.Contains(RAGUID))
					//&&(!this.ObjectIsSpecial&&(!FunkyGame.Targeting.Cache.Environment.AvoidanceLastTarget||this.CentreDistance>59f)))
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
					 (QuestMonster || IsMinimapActive.HasValue && IsMinimapActive.Value) ||
					 (IsBoss) ||
					 (IsSucideBomber && FunkyBaseExtension.Settings.Targeting.UnitExceptionSucideBombers) ||
					 (IsTreasureGoblin && FunkyBaseExtension.Settings.Ranges.TreasureGoblinRange > 1) ||
					 (IsRanged && FunkyBaseExtension.Settings.Targeting.UnitExceptionRangedUnits) ||
					 (IsSpawnerUnit && FunkyBaseExtension.Settings.Targeting.UnitExceptionSpawnerUnits) ||
					 ((FunkyBaseExtension.Settings.Targeting.UnitExceptionLowHP && ((CurrentHealthPct < 0.25 && UnitMaxHitPointAverageWeight > 0)
									   && ((!FunkyGame.Hero.Class.IsMeleeClass && CentreDistance < 30f) || (FunkyGame.Hero.Class.IsMeleeClass && RadiusDistance < 12f)))));
			}
		}

		public bool AllowLOSMovement
		{
			get
			{
				return
					//(ProfileCache.LineOfSightSNOIds.Contains(SNOID)) ||
					 (((FunkyBaseExtension.Settings.AdventureMode.EnableAdventuringMode && FunkyGame.AdventureMode && FunkyGame.Game.AllowAnyUnitForLOSMovement) ||
						(IsSucideBomber && SettingLOSMovement.LOSSettingsTag.AllowSucideBomber) ||
						(IsTreasureGoblin && SettingLOSMovement.LOSSettingsTag.AllowTreasureGoblin) ||
						(IsSpawnerUnit && SettingLOSMovement.LOSSettingsTag.AllowSpawnerUnits) ||
						((MonsterRare || MonsterElite) && SettingLOSMovement.LOSSettingsTag.AllowRareElites) ||
						((IsBoss || MonsterUnique) && SettingLOSMovement.LOSSettingsTag.AllowUniqueBoss) ||
						(IsRanged && SettingLOSMovement.LOSSettingsTag.AllowRanged))
						&&
						(CentreDistance <= SettingLOSMovement.LOSSettingsTag.MaximumRange &&//Enforce A Maximum Range
						SNOID != 347363)); //Exclude A5 MastaBlasta event
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
			if (this == FunkyGame.Targeting.Cache.LastCachedTarget)
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
				float fThisHeightDifference = FunkyBaseExtension.Difference(FunkyGame.Hero.Position.Z, Position.Z);
				if (fThisHeightDifference > 12f)
				{
					//raycast.. 
					// if (!Navigation.CanRayCast(FunkyGame.Hero.Class_.Data.Position, this.Position))
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

		public override int InteractionRange
		{
			get
			{
				//Set our current radius to the settings of profile.
				double dUseKillRadius = FunkyGame.Targeting.Cache.iCurrentMaxKillRadius;


				// Special short-range list to ignore weakling mobs
				if (CacheIDLookup.hashActorSNOShortRangeOnly.Contains(SNOID)) dUseKillRadius = 12;

				// Prevent long-range mobs beign ignored while they may be pounding on us
				if (dUseKillRadius <= 30 && TheCache.ObjectIDCache.Units.RangedUnits.Contains(SNOID)) dUseKillRadius = 30;
				

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
								return 0;
							}


							//Either no above average units or they are farther than the goblin is..
							//We will let calculations below preform instead!
						}
					}


					//Use a shorter range if not yet noticed..
					if (CurrentHealthPct <= 0.10)
						dUseKillRadius += FunkyBaseExtension.Settings.Ranges.TreasureGoblinRange + (FunkyBaseExtension.Settings.Targeting.GoblinPriority * 24);
					else if (CurrentHealthPct <= 0.99)
						dUseKillRadius += FunkyBaseExtension.Settings.Ranges.TreasureGoblinRange + (FunkyBaseExtension.Settings.Targeting.GoblinPriority * 16);
					else
						dUseKillRadius += FunkyBaseExtension.Settings.Ranges.TreasureGoblinRange + (FunkyBaseExtension.Settings.Targeting.GoblinPriority * 12);

					ForceLeap = true;
				}
				// Elitey type mobs and things
				else if ((IsEliteRareUnique))
				{
					dUseKillRadius += FunkyBaseExtension.Settings.Ranges.EliteCombatRange;
					ForceLeap = true;
				}
				else
					//Not Boss, Goblin, Elite/Rare/Unique..
					dUseKillRadius += FunkyBaseExtension.Settings.Ranges.NonEliteCombatRange;


				// Standard 50f range when preforming OOC behaviors!
				if (FunkyGame.IsInNonCombatBehavior)
					dUseKillRadius = FunkyBaseExtension.Settings.Plugin.OutofCombatMaxDistance;
				else if ((QuestMonster||IsMinimapActive.HasValue && IsMinimapActive.Value) && dUseKillRadius < 200f) //"Quest Monster" set 200f min distance.
					dUseKillRadius = 200f;

				return (int)dUseKillRadius;
			}
		}

		private void TallyTarget()
		{
			bool bIsRended;
			bool bCountAsElite;

			bIsRended = (HasDOTdps.HasValue && HasDOTdps.Value);
			bCountAsElite = (IsEliteRareUnique || IsTreasureGoblin || IsBoss);
			float RadiusDistance = this.RadiusDistance;

			if (FunkyBaseExtension.Settings.Fleeing.EnableFleeingBehavior && RadiusDistance <= FunkyBaseExtension.Settings.Fleeing.FleeMaxMonsterDistance && ShouldFlee)
			{
				FunkyGame.Targeting.Cache.Environment.FleeTriggeringUnits.Add(this);
			}

			if (RadiusDistance <= 6f)
			{
				FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_6]++;
				if (bCountAsElite)
					FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_6]++;
			}
			if (RadiusDistance <= 12f)
			{
				//Tally close units
				FunkyGame.Targeting.Cache.Environment.SurroundingUnits++;
				//Herbfunk: non-rend count only if within 8f and is attackable..
				if (FunkyGame.Hero.Class.AC == ActorClass.Barbarian && !bIsRended && RadiusDistance <= 7f && IsTargetable.Value)
					FunkyGame.Targeting.Cache.Environment.iNonRendedTargets_6++;

				FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_12]++;
				if (bCountAsElite)
					FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_12]++;
			}
			if (RadiusDistance <= 15f)
			{
				FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_15]++;
				if (bCountAsElite)
					FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_15]++;
			}
			if (RadiusDistance <= 20f)
			{
				FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20]++;
				if (bCountAsElite)
					FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_20]++;
			}
			if (RadiusDistance <= 25f)
			{
				if (!FunkyGame.Targeting.Cache.Environment.bAnyNonWWIgnoreMobsInRange && !CacheIDLookup.hashActorSNOWhirlwindIgnore.Contains(SNOID))
					FunkyGame.Targeting.Cache.Environment.bAnyNonWWIgnoreMobsInRange = true;
				FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_25]++;
				if (bCountAsElite)
					FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_25]++;
			}
			if (RadiusDistance <= 30f)
			{
				FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_30]++;
				if (bCountAsElite)
					FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_30]++;
			}
			if (RadiusDistance <= 40f)
			{
				FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_40]++;
				if (bCountAsElite)
					FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_40]++;
			}
			if (RadiusDistance <= 50f)
			{
				FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_50]++;
				if (bCountAsElite)
					FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_50]++;
			}
		}

		public override void UpdateWeight()
		{
			base.UpdateWeight();

			//Ignore Non Clusters
			if (BeingIgnoredDueToClusterLogic && !IsClusterException
				 && PriorityCounter == 0
				 && (FunkyGame.Targeting.Cache.CurrentTarget != null
				 || FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_30] == 0
				 || FunkyGame.Targeting.Cache.objectsIgnoredDueToAvoidance.Count == 0))
			{
				Weight = 0;
				return;
			}



			if (RadiusDistance >= 5f && FunkyGame.Hero.Class.IsMeleeClass)
			{
				if (DateTime.Now.Subtract(LastAvoidanceIgnored).TotalMilliseconds < 1000 && FunkyGame.Targeting.Cache.Environment.NearbyAvoidances.Count > 0)
				{
					Weight = 1;
				}
				else
				{
					Vector3 TestPosition = BotMeleeVector;
					//if (ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(TestPosition))
					//	Weight = 1;

					////intersecting avoidances..
					if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(TestPosition))
					{
						FunkyGame.Targeting.Cache.objectsIgnoredDueToAvoidance.Add(this);
						Weight = 1;

						//if (Weight != 1) //&& ObjectIsSpecial)
						//{//Only add this to the avoided list when its not currently inside avoidance area
						//	FunkyGame.Targeting.Cache.objectsIgnoredDueToAvoidance.Add(this);
						//}
						//else
						//Weight = 1;
					}
				}
			}


			if (Weight != 1)
			{
				float centreDistance = CentreDistance;
				float radiusDistance = RadiusDistance;

				// Flag up any bosses in range
				if (IsBoss && centreDistance <= 50f)
					Weight += 9999;

				// Force a close range target because we seem to be stuck *OR* if not ranged and currently rooted
				if (FunkyGame.Targeting.Cache.bPrioritizeCloseRangeUnits || FunkyGame.Hero.bIsRooted)
				{

					Weight = 20000 - (Math.Floor(radiusDistance) * 200);

					// Goblin priority KAMIKAZEEEEEEEE
					if (IsTreasureGoblin && FunkyBaseExtension.Settings.Targeting.GoblinPriority > 1)
						Weight += 10250 * (FunkyBaseExtension.Settings.Targeting.GoblinPriority - 1);
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

						if (radiusDistance < FunkyGame.Targeting.Cache.iCurrentMaxKillRadius)
						{
							int RangeModifier = 1200;
							//Increase Distance Modifier if recently kited.
							if (FunkyBaseExtension.Settings.Fleeing.EnableFleeingBehavior && DateTime.Now.Subtract(FunkyGame.Targeting.Cache.LastFleeAction).TotalMilliseconds < 3000)
								RangeModifier = 12000;


							Weight += (RangeModifier * (1 - (radiusDistance / FunkyGame.Targeting.Cache.iCurrentMaxKillRadius)));
						}

						// Give extra weight to ranged enemies
						if (FunkyGame.Hero.Class.IsMeleeClass && IsRanged)
						{
							Weight += 1100;
							ForceLeap = true;
						}

						// Give more weight to elites and minions
						if (IsEliteRareUnique)
						{
							//Non-Illusion
							if (!MonsterIlluionist || !SummonerID.HasValue || SummonerID.Value<=0)
							{
								Weight += 2000;

								if (MonsterAvenger && CurrentHealthPct.HasValue)
								{
									if (CurrentHealthPct.Value > 0.75d)
										Weight += 4000;
									else if (CurrentHealthPct.Value > 0.25d)
										Weight += 2000;
								}
							}
						}
						else
						{//Normal Units

							if (ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.AvoidanceSummoner) || ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Debuffing))
								Weight += 2000;

							if (ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Summoner))
								Weight += 500;
						}

						// Give more weight to bosses
						if (IsBoss)
							Weight += 4000;

						// Barbarians with wrath of the berserker up should prioritize elites more
						if (Hotbar.HasBuff(SNOPower.Barbarian_WrathOfTheBerserker) && (IsEliteRareUnique || IsTreasureGoblin || IsBoss))
							Weight += 2000;

						// Exploding Palm Bleeding Prioritize
						if (FunkyGame.Hero.Class.AC == ActorClass.Monk
							&& Hotbar.HasPower(SNOPower.Monk_ExplodingPalm)
							&& centreDistance < 20f)
						{
							if (HasDOTdps.HasValue && HasDOTdps.Value) //Exploding Palm -- Bleeding Already!
							{
								Weight += 500;
							}
							else if (CurrentHealthPct < 0.15d) //Exploding Palm -- Low HP, Not Bleeding!
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
						if (FunkyBaseExtension.Settings.Targeting.GoblinPriority >= 2 && IsTreasureGoblin && CurrentHealthPct <= 0.98)
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
							Weight += (200 * FunkyGame.Targeting.Cache.Environment.SurroundingUnits);
						}

						// Special additional weight for corrupt growths in act 4 ONLY if they are at close range (not a standard priority thing)
						if ((SNOID == 210120 || SNOID == 210268) && centreDistance <= 35f)
							Weight += 2000;

						// Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
						if (Equals(FunkyGame.Targeting.Cache.LastCachedTarget) && centreDistance <= 25f)
							Weight += 400;


						// Prevent going less than 300 yet to prevent annoyances (should only lose this much weight from priority reductions in priority list?)
						if (Weight < 300)
							Weight = 300;

						// Deal with treasure goblins - note, of priority is set to "0", then the is-a-goblin flag isn't even set for use here - the monster is ignored
						if (IsTreasureGoblin)
						{
							// Logging goblin sightings
							if (FunkyGame.Targeting.Cache.lastGoblinTime == DateTime.Today)
							{
								FunkyGame.Targeting.Cache.iTotalNumberGoblins++;
								FunkyGame.Targeting.Cache.lastGoblinTime = DateTime.Now;
								Logger.DBLog.InfoFormat("[Funky] Goblin #" + FunkyGame.Targeting.Cache.iTotalNumberGoblins.ToString(CultureInfo.InvariantCulture) + " in sight. Distance=" + centreDistance);
							}
							else
							{
								if (DateTime.Now.Subtract(FunkyGame.Targeting.Cache.lastGoblinTime).TotalMilliseconds > 30000)
									FunkyGame.Targeting.Cache.lastGoblinTime = DateTime.Today;
							}
							// Original Trinity stuff for priority handling now
							switch (FunkyBaseExtension.Settings.Targeting.GoblinPriority)
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

				//Profile disabled Killing of monsters.
				if (!ProfileManager.CurrentProfile.KillMonsters)
				{
					IgnoredType = TargetingIgnoreTypes.IgnoredTargetType;
					return false;
				}

				//Z-Height Difference Check
				if (!IsZDifferenceValid)
				{
					IgnoredType = TargetingIgnoreTypes.ZDifferenceFailure;
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
						//Logger.Write(LogLevel.Cache, "Unit Is Dead {0}", DebugStringSimple);
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

						IgnoredType = TargetingIgnoreTypes.UnitUntargetable;
						return false;
					}
				}






				float centreDistance = CentreDistance;
				bool distantUnit = false;
				bool validUnit = false;
				//Distant Units List
				if (!FunkyGame.IsInNonCombatBehavior && FunkyBaseExtension.Settings.Grouping.AttemptGroupingMovements && centreDistance >= FunkyBaseExtension.Settings.Grouping.GroupingMinimumUnitDistance)
				{
					distantUnit = true;
				}

				if (centreDistance > InteractionRange && !distantUnit)
				{
					//Since special objects are subject to LOS movement, we do not ignore just yet.
					if (!AllowLOSMovement)
					{
						IgnoredType = TargetingIgnoreTypes.DistanceFailure;
						return false;
					}
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
							//if (FunkyBaseExtension.Settings.Debugging.FunkyLogFlags.HasFlag(LogLevel.Target))
							//	Logger.Write(LogLevel.Target, "Adding {0} to LOS Movement Objects", InternalName);
							FunkyGame.Targeting.Cache.Environment.LoSMovementObjects.Add(this);
						}
						IgnoredType = TargetingIgnoreTypes.LineOfSightFailure;
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
							//if (FunkyBaseExtension.Settings.Debugging.FunkyLogFlags.HasFlag(LogLevel.Target))
							//	Logger.Write(LogLevel.Target, "Adding {0} to LOS Movement Objects", InternalName);
							FunkyGame.Targeting.Cache.Environment.LoSMovementObjects.Add(this);
						}
						IgnoredType = TargetingIgnoreTypes.LineOfSightFailure;
						return false;
					}

					//This is intial test to validate we can "see" the unit.. 
					if (!LineOfSight.LOSTest(FunkyGame.Hero.Position, true, false, !FunkyGame.Hero.Class.ContainsNonRangedCombatSkill, NavCellFlags.None, false))
					{
						//LOS Movement -- Check for special objects
						//LOS failed.. now we should decide if we want to find a spot for this target, or just ignore it.
						// if (this.IsEliteRareUnique||this.IsTreasureGoblin)
						if (AllowLOSMovement)
						{
							Logger.Write(LogLevel.LineOfSight, "Adding {0} to LOS Movement Objects", InternalName);
							FunkyGame.Targeting.Cache.Environment.LoSMovementObjects.Add(this);
						}

						//Valid?? Did we find a location we could move to for LOS?
						if (!FunkyGame.Hero.bIsIncapacitated && !ObjectIsSpecial)
							BlacklistLoops = 2;
						else//Incapacitated we reset check
							LineOfSight.LastLOSCheck = DateTime.Today;

						IgnoredType = TargetingIgnoreTypes.LineOfSightFailure;
						return false;
					}

					RequiresLOSCheck = false;
				}






				#region CombatFlags

				if (IsBoss || IsEliteRareUnique)
				{
					//Ignore Setting?
					if (FunkyBaseExtension.Settings.Targeting.IgnoreAboveAverageMobs && PriorityCounter <= 1 && !FunkyGame.IsInNonCombatBehavior && !IsBoss)
					{
						IgnoredType = TargetingIgnoreTypes.IgnoredTargetType;
						return false;
					}
					FunkyGame.Targeting.Cache.Environment.bAnyChampionsPresent = true;
				}

				if (IsTreasureGoblin)
					FunkyGame.Targeting.Cache.Environment.bAnyTreasureGoblinsPresent = true;



				// Total up monsters at various ranges
				if (centreDistance <= 50f)
				{
					TallyTarget();
				}

				#endregion


				//Profile Blacklisted.
				if (!FunkyBaseExtension.Settings.Ranges.IgnoreProfileBlacklists && BlacklistCache.hashProfileSNOTargetBlacklist.Contains(SNOID))
				{
					//Only if not prioritized..
					if (!FunkyGame.IsInNonCombatBehavior)
					{
						IgnoredType = TargetingIgnoreTypes.IgnoredTargetType;
						return false;
					}
				}

				if (distantUnit)
					FunkyGame.Targeting.Cache.Environment.DistantUnits.Add(this); //Add this valid unit to Distant List.
				if (validUnit) //Add this valid unit RAGUID to list
					FunkyGame.Targeting.Cache.Environment.UnitRAGUIDs.Add(RAGUID);



				return true;
			}
		}

		public override bool UpdateData()
		{
			if (!base.IsStillValid())
			{
				//Logger.Write(LogLevel.Cache,"ref object not valid for {0}", DebugStringSimple);
				return false;
			}

			if (ref_DiaUnit == null)
			{
				try
				{
					ref_DiaUnit = ref_DiaObject as DiaUnit;
				}
				catch
				{
					Logger.Write(LogLevel.Cache, "Failure to convert obj to DiaUnit {0}", DebugStringSimple);

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
			{
				//Logger.Write(LogLevel.Cache, "No Monster Type for object {0}", DebugStringSimple);
				return false;
			}

			//Update Monster Type?
			if (ShouldRefreshMonsterType)
			{
				if (!base.UpdateData(ref_DiaObject, RAGUID))
				{
					//Logger.Write(LogLevel.Cache, "Monster Refresh Failed for object {0}", DebugStringSimple);
					return false;
				}
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
					Logger.Write(LogLevel.Cache, "Handled IsNPC for Unit {0}", DebugStringSimple);
				}

			}


			// Make sure it's a valid monster type
			if (!MonsterTypeIsHostile() || isNPC)
			{
				if (FunkyGame.Hero.bIsInTown)
				{
					//Perma Ignore all NPCs we find in town..
					if (isNPC) BlacklistCache.IgnoreThisObject(this);

					return false;
				}


				//Special Bounty Check for Events only!
				if (FunkyBaseExtension.Settings.AdventureMode.EnableAdventuringMode && FunkyGame.AdventureMode && FunkyGame.Bounty.CurrentBountyCacheEntry != null && FunkyGame.Bounty.CurrentBountyCacheEntry.Type == BountyQuestTypes.Event)
				{
					if (!IsQuestGiver)
					{
						try
						{
							IsQuestGiver = ref_DiaUnit.IsQuestGiver;
						}
						catch (Exception ex)
						{
							Logger.Write(LogLevel.Cache, "Handled IsQuestGiver for Unit {0}", DebugStringSimple);
						}
					}

					if (IsQuestGiver)
					{//Is A Quest Giver..
						
						if (!ObjectCache.InteractableObjectCache.ContainsKey(RAGUID))
						{//Check if MarkerType is Exclamation..

							bool shouldInteract = false;
							try
							{
								shouldInteract = ref_DiaUnit.MarkerType == MarkerType.Exclamation;
							}
							catch (Exception)
							{
								Logger.Write(LogLevel.Cache, "Handled MarkerType for Unit {0}", DebugStringSimple);
							}

							if (shouldInteract)
							{
								ObjectCache.InteractableObjectCache.Add(RAGUID, this);
								targetType = TargetType.Interaction;
							}
							else
							{
								//SNOID should be ignored?
								if(!CacheIDLookup.hashSnoNpcNoIgnore.Contains(SNOID))
								{
									BlacklistCache.IgnoreThisObject(this);
									return false;
								}
							}
						}

						//Quest Giver No Good For Targeting!
						return false;
					}

					//Minimap Active?
					if (!IsMinimapActive.HasValue)
					{
						try
						{
							IsMinimapActive = ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.MinimapActive) != 0;
						}
						catch (Exception ex)
						{
							Logger.Write(LogLevel.Cache, "Handled MinimapActive for Unit {0}", DebugStringSimple);
						}
					}

					if (IsMinimapActive.HasValue && IsMinimapActive.Value)
					{
						if (!ObjectCache.InteractableObjectCache.ContainsKey(RAGUID))
						{
							bool shouldInteract = false;
							try
							{
								shouldInteract = ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.NPCIsOperatable) != 0;
							}
							catch (Exception)
							{
								Logger.Write(LogLevel.Cache, "Handled NPCIsOperatable for Unit {0}", DebugStringSimple);
							}

							if (shouldInteract)
							{
								ObjectCache.InteractableObjectCache.Add(RAGUID, this);
								targetType = TargetType.Interaction;
							}
							else
							{
								//SNOID should be ignored?
								if (!CacheIDLookup.hashSnoNpcNoIgnore.Contains(SNOID))
								{
									BlacklistCache.IgnoreThisObject(this);
								}
							}
						}

						return false;
					}
					
				}


				//Either not hostile or NPC. (Bosses we exclude!)
				if (!IsBoss && !CacheIDLookup.hashSnoNpcNoIgnore.Contains(SNOID))
				{
					//Logger.Write(LogLevel.Cache, "Monster Is NPC {0}", DebugStringSimple);
					if (isNPC) BlacklistCache.IgnoreThisObject(this);
					return false;
				}
			}

			bool isFriendly = false;
			if (!IsFriendly.HasValue || IsFriendly.Value)
			{
				try
				{
					IsFriendly = ref_DiaUnit.IsFriendly;
					isFriendly = IsFriendly.Value;
				}
				catch (Exception)
				{
					Logger.Write(LogLevel.Cache, "Handled IsFriendly for Unit {0}", DebugStringSimple);
				}
			}

			if (isFriendly)
			{
				return false;
			}


			//Position update
			base.UpdatePosition();

			if (Radius == 0f)
			{
				if (ActorSphereRadius.HasValue)
				{
					Radius = ActorSphereRadius.Value;

					//Reduce the radius for Corruptant Growths.
					if (IsCorruptantGrowth)
						Radius *= 0.80f;

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
					Logger.Write(LogLevel.Cache, "Failure to check monster affixes for unit {0}", DebugStringSimple);
					return false;
				}
			}

			if (IsEliteRareUnique)
			{
				//Illusionist -- Update the SummonedByACDID!
				if (MonsterIlluionist)
				{
					// Get the summoned-by info, cached if possible
					if (!SummonerID.HasValue)
					{
						try
						{
							SummonerID = CommonData.GetAttribute<int>(ActorAttributeType.SummonedByACDID);
						}
						catch (Exception ex)
						{
							Logger.Write(LogLevel.Cache, "Failure to get Summoned By ACDID for {0}", DebugStringSimple);
						}
					}
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
					Logger.Write(LogLevel.Cache, "Failure to get maximum health for {0}", DebugStringSimple);
					return false;
				}
			}


			//update HPs
			UpdateHitPoints();

			if (CurrentHealthPct.HasValue && CurrentHealthPct.Value <= 0d)
			{
				//Logger.Write(LogLevel.Cache, "Unit Is Dead {0}", DebugStringSimple);
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
					Logger.Write(LogLevel.Cache, "[Funky] Safely handled exception getting is-targetable attribute for unit {0}", DebugStringSimple);
					//Logger.DBLog.DebugFormat(ex.ToString());
					IsTargetable = true;
				}
			}

			if (IsTargetable.HasValue && !IsTargetable.Value)
			{
				return false;
			}

			//Attackable
			if (MonsterShielding || (CurrentHealthPct.HasValue && (CurrentHealthPct.Value < 1d || CurrentHealthPct.Value > 1d) && IsGrotesqueActor))
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


			//if (IsEliteRareUnique && MonsterFireChains && (!MonsterIlluionist || !SummonerID.HasValue || SummonerID.Value==-1))
			//{
				
			//}

			#region Class DOT DPS Check
			//Barb specific updates
			if (FunkyGame.CurrentActorClass == ActorClass.Barbarian)
			{
				//Rend DotDPS update
				if (Hotbar.HasPower(SNOPower.Barbarian_Rend))
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
			else if (FunkyGame.CurrentActorClass == ActorClass.Monk)
			{
				//1195139072
				if (Hotbar.HasPower(SNOPower.Monk_ExplodingPalm))
				{
					if (CentreDistance < 30f)
					{
						FunkyGame.Targeting.Cache.Environment.UsesDOTDPSAbility = true;
						try
						{
							int dotDPS = ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.DOTDPS);
							int visualBuff = ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.Bleeding);
							HasDOTdps = (dotDPS > 0 && visualBuff > 0);

							//DotDPS values
							//dotDPS==1195139072
							//1215532915 || 1215532915
							//1209914057 || 1220659971
						}
						catch
						{
						}
					}
				}
			}
			else if (FunkyGame.CurrentActorClass == ActorClass.Witchdoctor)
			{
				//Haunted DotDPS update
				if (Hotbar.HasPower(SNOPower.Witchdoctor_Haunt) || Hotbar.HasPower(SNOPower.Witchdoctor_Locust_Swarm))
				{
					FunkyGame.Targeting.Cache.Environment.UsesDOTDPSAbility = true;
					try
					{
						//Haunted units always have buff visual effect!
						//bool buffVisualEffect=(this.ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.BuffVisualEffect)>0);

						dotdpsValue = ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.DOTDPS);
						int visualBuff = ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.BuffVisualEffect);
						HasDOTdps = (dotdpsValue > 0 && visualBuff > 0);
						/*
						if (FunkyGame.Hero.Class_.Class.HotbarAbilities.Contains(SNOPower.Witchdoctor_Haunt))
						{
							 this.HasDOTdps=(visualBuff>0&&(dotDPS==1194344448||dotDPS==1194786816||dotDPS==1202929855||dotDPS==1194983424||dotDPS==1196072960||dotDPS==1194770432));
						}
						else if (FunkyGame.Hero.Class_.Class.HotbarAbilities.Contains(SNOPower.Witchdoctor_Locust_Swarm))
						{
							 this.HasDOTdps=(visualBuff>0&&(dotDPS==1178820608||dotDPS==1197301760||dotDPS==1182662656));
						}
						*/
						//1216447279
						//1222043435

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

			if (IsMalletLordUnit && CentreDistance < 25f)
			{
				UpdateSNOAnim();
				if (SnoAnim == SNOAnim.malletDemon_attack_01)
				{
					//update rotation!
					UpdateRotation();

					if (IsFacingBot())
					{
						Vector3 avoidPosition = MathEx.GetPointAt(Position, Radius, Rotation);
						int positionHash = avoidPosition.GetHashCode();
						CacheObstacle outvalue;
						if (!ObjectCache.Obstacles.TryGetValue(positionHash, out outvalue))
						{
							Logger.DBLog.DebugFormat("[Funky] Adding Mallet Lord to Watch List!");
							ObjectCache.Obstacles.Add(positionHash, new CacheAvoidance(AvoidanceType.MalletLord, 100, positionHash, AcdGuid.Value, avoidPosition, "MalletLordAvoidance"));
						}
					}
				}
			}

			//Update Quest Monster?
			if (FunkyGame.Targeting.Cache.UpdateQuestMonsterProperty || FunkyGame.Game.QuestMode)
			{
				try
				{
					QuestMonster = ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.QuestMonster) != 0;
				}
				catch (Exception)
				{
					QuestMonster = false;
				}

				try
				{
					IsMinimapActive = ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.MinimapActive) != 0;
				}
				catch (Exception)
				{
					IsMinimapActive = false;
				}
			}

			if (!UnitPropertyFlags.HasValue)
			{
				UnitPropertyFlags = GenerateUnitFlags();
				if (!DebugDataChecked && FunkyBaseExtension.Settings.Debugging.DebuggingData && FunkyBaseExtension.Settings.Debugging.DebuggingDataTypes.HasFlag(DebugDataTypes.Units))
				{
					DebugDataChecked = true;
					ObjectCache.DebuggingData.CheckEntry(this);
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
			if (FunkyGame.Hero.Class.PowerPrime.Power != SNOPower.None)
			{
				if (!FunkyGame.Hero.Class.PowerPrime.ActivateSkill())
					return RunStatus.Running;

				//Check health changes -- only when single target or cluster with targeting is used.
				if (FunkyGame.Targeting.Cache.LastCachedTarget.Equals(this) &&
					  DateTime.Now.Subtract(FunkyGame.Hero.Class.LastUsedACombatAbility).TotalMilliseconds < 2500 &&
					  DateTime.Now.Subtract(FunkyGame.Targeting.Cache.LastChangeOfTarget).TotalMilliseconds > 3000 &&
					 !FunkyGame.Hero.bIsInBossEncounter)
				{
					double LastHealthChangedMS = DateTime.Now.Subtract(LastHealthChange).TotalMilliseconds;
					if (LastHealthChangedMS > 5000)
					{
						Logger.Write(LogLevel.Target, "Ignore Unit {0} due to health last changed of {1}ms", InternalName, LastHealthChangedMS);
						BlacklistLoops = 20;
						FunkyGame.Targeting.Cache.bForceTargetUpdate = true;
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
				FunkyGame.Hero.Class.PowerPrime.MinimumRange = FunkyGame.Hero.Class.IsMeleeClass ? 14 : 16;
			else if (IgnoresLOSCheck)
				FunkyGame.Hero.Class.PowerPrime.MinimumRange = (int)(ActorSphereRadius.Value * 1.5);
			else if (Hotbar.HasBuff(SNOPower.Pages_Buff_Electrified))
			{
				if (FunkyGame.Hero.Class.PowerPrime.MinimumRange>20)
					FunkyGame.Hero.Class.PowerPrime.MinimumRange = 20;
			}
			else if(FunkyGame.Hero.Class.LastUsedAbility.IsSpecialMovementSkill && FunkyGame.Hero.Class.HasSpecialMovementBuff())
			{
				FunkyGame.Hero.Class.PowerPrime.MinimumRange=FunkyGame.Hero.Class.LastUsedAbility.Range;
			}
			else if (IsBurrowed.HasValue && IsBurrowed.Value && IsEliteRareUnique)//Force close range on burrowed elites!
				FunkyGame.Hero.Class.PowerPrime.MinimumRange = 15;
			else if (IsStealthableUnit && IsTargetable.HasValue && IsTargetable.Value == false && IsEliteRareUnique)
				FunkyGame.Hero.Class.PowerPrime.MinimumRange = 15;
			else if (IsTreasureGoblin && !FunkyGame.Hero.Class.IsMeleeClass && FunkyBaseExtension.Settings.Combat.GoblinMinimumRange > 0 && FunkyGame.Hero.Class.PowerPrime.MinimumRange > FunkyBaseExtension.Settings.Combat.GoblinMinimumRange)
				FunkyGame.Hero.Class.PowerPrime.MinimumRange = FunkyBaseExtension.Settings.Combat.GoblinMinimumRange;
			else if (MonsterMissileDampening && FunkyBaseExtension.Settings.Targeting.MissleDampeningEnforceCloseRange)
				FunkyGame.Hero.Class.PowerPrime.MinimumRange = 15;
			else if (targetType.HasValue && targetType.Value == TargetType.Interaction)
				FunkyGame.Hero.Class.PowerPrime.MinimumRange = 7;

			// Pick a range to try to reach
			fRangeRequired = FunkyGame.Hero.Class.PowerPrime.Power == SNOPower.None ? 9f : FunkyGame.Hero.Class.PowerPrime.MinimumRange;

			DistanceFromTarget = RadiusDistance;

			return (fRangeRequired <= 0f || DistanceFromTarget <= fRangeRequired);
		}

		public override bool ObjectIsSpecial
		{
			get
			{
				if ((IsEliteRareUnique && !FunkyBaseExtension.Settings.Targeting.IgnoreAboveAverageMobs) ||
						   (PriorityCounter > 0) ||
						   (IsBoss && !FunkyBaseExtension.Settings.Targeting.IgnoreAboveAverageMobs && CurrentHealthPct.HasValue && CurrentHealthPct <= 0.99d) ||
						   (((IsSucideBomber && FunkyBaseExtension.Settings.Targeting.UnitExceptionSucideBombers) || IsCorruptantGrowth) && CentreDistance < 45f) ||
						   (IsSpawnerUnit && FunkyBaseExtension.Settings.Targeting.UnitExceptionSpawnerUnits) ||
						   ((IsTreasureGoblin && FunkyBaseExtension.Settings.Targeting.GoblinPriority > 1)) ||
						   (IsRanged && FunkyBaseExtension.Settings.Targeting.UnitExceptionRangedUnits
								&& (!IsEliteRareUnique || !FunkyBaseExtension.Settings.Targeting.IgnoreAboveAverageMobs)) ||
					//Low HP (25% or Less) & Is Not Considered Weak
						   ((FunkyBaseExtension.Settings.Targeting.UnitExceptionLowHP && CurrentHealthPct.HasValue && ((CurrentHealthPct <= 0.25 && UnitMaxHitPointAverageWeight > 0)
									  && (!IsEliteRareUnique || !FunkyBaseExtension.Settings.Targeting.IgnoreAboveAverageMobs) && (RadiusDistance <= FunkyBaseExtension.Settings.Targeting.UnitExceptionLowHPMaximumDistance)))))



					return true;

				return base.ObjectIsSpecial;
			}
		}

		public override string DebugString
		{
			get
			{



				return String.Format("{0}Burrowed {1} / Targetable {2} / Attackable {3}\r\n" +
									 "HP {4} / MaxHP {5} -- IsMoving: {6}\r\n" +
									 "PriorityCounter={7}\r\n" +
				                     "IgnoredDueToClusterLogic {15} IsClusterException {16}\r\n" +
									 "QuestMonster={9} MiniMapActive={14}\r\n" +
									 "IsNpc {11} IsFriendly {12}\r\n" +
									 "{10}\r\n" +
									 "{13}\r\n" +
									 "Unit Properties {8}",
					  base.DebugString,
					  IsBurrowed.HasValue ? IsBurrowed.Value.ToString() : "",
					  IsTargetable.HasValue ? IsTargetable.Value.ToString() : "",
					  IsAttackable.HasValue ? IsAttackable.Value.ToString() : "",
					  CurrentHealthPct.HasValue ? CurrentHealthPct.Value.ToString(CultureInfo.InvariantCulture) : "",
					  MaximumHealth.HasValue ? MaximumHealth.Value.ToString(CultureInfo.InvariantCulture) : "",
					  IsMoving,
					  PriorityCounter,
					  Properties,
					  QuestMonster,
					  HasDOTdps.HasValue ? HasDOTdps.Value + " dotdps: " + dotdpsValue : "",
					  IsNPC.HasValue ? IsNPC.Value.ToString() : "",
					  IsFriendly.HasValue ? IsFriendly.ToString() : "",
					  SkillsUsedOnObject.Count > 0 ?
							SkillsUsedOnObject.Aggregate("Skills Used\r\n:", (current, skill) => current + ("Power: " + skill.Key + " Date: " + skill.Value.ToString() + " LastUsedMS: " + DateTime.Now.Subtract(skill.Value).Milliseconds + "\r\n"))
							: "",
					  IsMinimapActive.HasValue?IsMinimapActive.Value.ToString():"",
					  BeingIgnoredDueToClusterLogic, IsClusterException);
			}
		}


	}



}