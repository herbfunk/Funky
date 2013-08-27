using System;
using System.Linq;
using Zeta;
using System.Windows;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.Internals.SNO;
using Zeta.TreeSharp;
using FunkyTrinity.Enums;
using FunkyTrinity.Cache;
using FunkyTrinity.Movement;
using FunkyTrinity.ability;
using FunkyTrinity.Movement.Clustering;

namespace FunkyTrinity.Cache
{

		  public class CacheUnit : CacheObject
		  {

				private TargetProperties EvaluateUnitProperties(CacheUnit unit)
				{
					 TargetProperties properties=TargetProperties.None;


						  if (unit.IsBoss)
								properties|=TargetProperties.Boss;

						  if (unit.IsBurrowableUnit)
								properties|=TargetProperties.Burrowing;

						  if (unit.MonsterMissileDampening)
								properties|=TargetProperties.MissileDampening;

						  if (unit.IsMissileReflecting)
								properties|=TargetProperties.MissileReflecting;

						  if (unit.MonsterShielding)
								properties|=TargetProperties.Shielding;

						  if (unit.IsStealthableUnit)
								properties|=TargetProperties.Stealthable;

						  if (unit.IsSucideBomber)
								properties|=TargetProperties.SucideBomber;

						  if (unit.IsTreasureGoblin)
								properties|=TargetProperties.TreasureGoblin;

						  if (unit.IsFast)
								properties|=TargetProperties.Fast;
					 


						  if (unit.IsEliteRareUnique)
								properties|=TargetProperties.RareElite;

						  if (unit.MonsterUnique)
								properties|=TargetProperties.Unique;

						  if (unit.ObjectIsSpecial)
								properties|=TargetProperties.IsSpecial;

						  if (unit.CurrentHealthPct.Value==1d)
								properties|=TargetProperties.FullHealth;

						  if (unit.UnitMaxHitPointAverageWeight<0)
								properties|=TargetProperties.Weak;


						  if (unit.Monstersize.Value==MonsterSize.Ranged)
								properties|=TargetProperties.Ranged;


						  if (unit.IsTargetableAndAttackable)
								properties|=TargetProperties.TargetableAndAttackable;


						  if (unit.HasDOTdps.HasValue&&unit.HasDOTdps.Value)
								properties|=TargetProperties.DOTDPS;
					 

					 return properties;
				}
				public CacheUnit(CacheObject baseobj)
					 : base(baseobj)
				{
				}

				public DiaUnit ref_DiaUnit { get; set; }

				public TargetProperties Properties { get { return EvaluateUnitProperties(this); } }


				#region Monster Affixes Related
				private bool CheckedMonsterAffixes_=false;
				private void CheckMonsterAffixes(MonsterAffixes theseaffixes)
				{
					 MonsterRare=theseaffixes.HasFlag(MonsterAffixes.Rare);
					 MonsterUnique=theseaffixes.HasFlag(MonsterAffixes.Unique);
					 MonsterElite=theseaffixes.HasFlag(MonsterAffixes.Elite);
					 MonsterMinion=theseaffixes.HasFlag(MonsterAffixes.Minion);

					 if (IsEliteRareUnique)
					 {
						  MonsterShielding=theseaffixes.HasFlag(MonsterAffixes.Shielding);
						  MonsterMissileDampening=theseaffixes.HasFlag(MonsterAffixes.MissileDampening);
						  MonsterIlluionist=theseaffixes.HasFlag(MonsterAffixes.Illusionist);
						  MonsterExtraHealth=theseaffixes.HasFlag(MonsterAffixes.ExtraHealth);
						  MonsterLifeLink=theseaffixes.HasFlag(MonsterAffixes.HealthLink);
					 }
					 else
					 {
						  MonsterShielding=false;
						  MonsterMissileDampening=false;
						  MonsterIlluionist=false;
						  MonsterExtraHealth=false;
						  MonsterLifeLink=false;
					 }

					 CheckedMonsterAffixes_=true;
				}
				public bool MonsterRare { get; set; }
				public bool MonsterUnique { get; set; }
				public bool MonsterElite { get; set; }
				public bool MonsterMinion { get; set; }


				public bool MonsterShielding { get; set; }
				public bool MonsterMissileDampening { get; set; }
				public bool MonsterIlluionist { get; set; }
				public bool MonsterExtraHealth { get; set; }
				public bool MonsterLifeLink { get; set; }

				public bool IsEliteRareUnique
				{
					 get
					 {
						  return (MonsterRare||MonsterUnique||MonsterElite||MonsterMinion);
					 }
				}
				#endregion

				private bool ismoving=false;
				public bool IsMoving
				{
					 get { return ismoving; }
					 set { ismoving=value; }
				}

				private DateTime LastAvoidanceIgnored=DateTime.Today;
				private bool? IsNPC { get; set; }
				public bool ForceLeap { get; set; }
				public bool? HasDOTdps { get; set; }
				public bool? IsTargetable { get; set; }
				public bool? IsAttackable { get; set; }
				public bool IsTargetableAndAttackable
				{
					 get
					 {
						  return ((this.IsAttackable.HasValue&&this.IsAttackable.Value)
									 &&(this.IsTargetable.HasValue&&this.IsTargetable.Value));
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

						  if (value.Value==true&&(!base.CanBurrow.HasValue||!base.CanBurrow.Value))
								base.CanBurrow=true;

						  burrowed_=value;
					 }
				}

				public bool ShouldBeKited
				{
					 get
					 {
						  return ((!this.BeingIgnoredDueToClusterLogic||this.PriorityCounter>0) //not ignored because of clusters
										&&(!this.IsBurrowed.HasValue||!this.IsBurrowed.Value) //ignore burrowed!
										&&(!Bot.HashActorSNOKitingIgnore.Contains(base.SNOID)||this.MonsterRare||this.MonsterMinion||this.MonsterElite));
					 }
				}

				public bool BeingIgnoredDueToClusterLogic
				{
					 get
					 {

							if (Bot.SettingsFunky.EnableClusteringTargetLogic
								&&(!Bot.SettingsFunky.IgnoreClusteringWhenLowHP||Bot.Character.dCurrentHealthPct>Bot.SettingsFunky.IgnoreClusterLowHPValue)
								&&!Bot.IsInNonCombatBehavior)
						  {
								//Check if this unit is valid based on if its contained in valid clusters
								 if (!Bot.Combat.ValidClusterUnits.Contains(this.RAGUID)
									 &&(!this.ObjectIsSpecial&&(!Bot.Combat.AvoidanceLastTarget||this.CentreDistance>45f)))
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
						 return this.IsSucideBomber
							  ||(this.IsTreasureGoblin&&Bot.SettingsFunky.TreasureGoblinRange>1)
							  ||(this.Monstersize.Value==MonsterSize.Ranged&&Bot.SettingsFunky.ClusteringAllowRangedUnits)
							  ||(this.IsSpawnerUnit&&Bot.SettingsFunky.ClusteringAllowSpawnerUnits)
							  ||((Bot.SettingsFunky.ClusterKillLowHPUnits&&((this.CurrentHealthPct<0.25&&this.UnitMaxHitPointAverageWeight>0)
												&&((!Bot.Class.IsMeleeClass&&this.CentreDistance<30f)||(Bot.Class.IsMeleeClass&&this.RadiusDistance<12f)))));
					}
			  }


				#region Health Related
				//Monter Hitpoints
				public double? MaximumHealth { get; set; }

				internal DateTime LastHealthChange=DateTime.Today;
				private double LastCurrentHealth_=0d;
				public double LastCurrentHealth
				{
					 get { return LastCurrentHealth_; }
					 set { LastCurrentHealth_=value; LastHealthChange=DateTime.Now; }
				}

				public double? CurrentHealthPct { get; set; }
				private int HealthChecks=0;
				private bool SIGNATURE_SPAM;

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
						  double averageRatio=(this.MaximumHealth.Value/ObjectCache.Objects.MaximumHitPointAverage);

						  int assignedWeight;

						  if (averageRatio>=0.9&&averageRatio<=1.10)
								assignedWeight=0;
						  else if (averageRatio<0.25)
								assignedWeight=-2;
						  else if (averageRatio<0.9)
								assignedWeight=-1;
						  else if (averageRatio>1.25)
								assignedWeight=2;
						  else
								assignedWeight=1;

						  return (assignedWeight);
					 }
				}

				///<summary>
				///This only updates hitpoints every 5th call to help reduce CPU!
				///</summary>
				public bool UpdateHitPoints()
				{
					 //Current Target skips the counter checks
					 if (this==Bot.Target.CurrentTarget)
					 {
						  this.UpdateCurrentHitPoints();
						  if (this.LastCurrentHealth_!=this.CurrentHealthPct)
						  {
								 Bot.Combat.LastHealthChange=DateTime.Now;
								 Bot.Combat.LastHealthDropPct=(this.LastCurrentHealth_-this.CurrentHealthPct.Value);
						  }

						  return true;
					 }


					 this.HealthChecks++;

					 if (this.HealthChecks>6)
						  this.HealthChecks=1;

					 if (this==Bot.Combat.LastCachedTarget&&this.HealthChecks>3)
						  this.HealthChecks=1;

					 if (this.HealthChecks==1)
						  this.UpdateCurrentHitPoints();


					 return true;
				}

				///<summary>
				///Updates current health percent by reading current health from DiaUnit and dividing it by cache value maximum health.
				///</summary>
				[System.Diagnostics.DebuggerNonUserCode]
				public void UpdateCurrentHitPoints()
				{
					 double dThisCurrentHealth;


					 using (ZetaDia.Memory.AcquireFrame())
					 {
						  try
						  {
								try
								{
									 dThisCurrentHealth=this.ref_DiaUnit.HitpointsCurrent;
								} catch (NullReferenceException)
								{
									 return;
								}

								if (!this.MaximumHealth.HasValue)
									 this.MaximumHealth=this.ref_DiaUnit.CommonData.GetAttribute<float>(ActorAttributeType.HitpointsMax);

						  } catch (AccessViolationException)
						  {
								// This happens so frequently in DB/D3 that this fails, let's not even bother logging it anymore
								//Logging.WriteDiagnostic("[GilesTrinity] Safely handled exception getting current health for unit " + tmp_sThisInternalName + " [" + tmp_iThisActorSNO.ToString() + "]");
								// Add this monster to our very short-term ignore list
								base.NeedsRemoved=true;
								return;
						  }
					 }




					 // And finally put the two together for a current health percentage
					 double dCurrentHealthPct=dThisCurrentHealth/this.MaximumHealth.Value;
					 if (dCurrentHealthPct!=this.CurrentHealthPct)
					 {
						  this.LastCurrentHealth_=this.CurrentHealthPct.HasValue?this.CurrentHealthPct.Value:0d;
						  this.CurrentHealthPct=dCurrentHealthPct;
					 }
				}


				private double KillRadius
				{
					 get
					 {
						  //Set our current radius to the settings of profile.
							double dUseKillRadius=Bot.iCurrentMaxKillRadius;


						  // Special short-range list to ignore weakling mobs
						  if (CacheIDLookup.hashActorSNOShortRangeOnly.Contains(this.SNOID)) dUseKillRadius=12;

						  // Prevent long-range mobs beign ignored while they may be pounding on us
						  if (dUseKillRadius<=30&&CacheIDLookup.hashActorSNORanged.Contains(this.SNOID)) dUseKillRadius=30;

							if (this.CentreDistance<=Bot.NonEliteRange) Bot.Combat.bAnyMobsInCloseRange=true;



						  // Bosses get extra radius
							if (this.IsBoss)
							{
								 // Kulle Exception
								 if (this.SNOID!=80509) dUseKillRadius*=1.5;

								 //more if they're already injured
								 if (this.CurrentHealthPct<=0.98) dUseKillRadius*=4;

								 // And make sure we have a MINIMUM range for bosses - incase they are at screen edge etc.
								 if (dUseKillRadius<=200)
									  if (this.SNOID==218947||this.SNOID==256000)
											dUseKillRadius=75;
									  else if (this.SNOID!=80509) //Kulle Exception
											dUseKillRadius=200;
							}
							// Tressure Goblins
							else if (this.IsTreasureGoblin)
							{
								 //Check if this goblin is in combat and we are not to close..

								 if (this.CurrentHealthPct.Value>=1d
									  &&this.RadiusDistance>20f)
								 {
									  //Lets calculate if we want to bum rush this goblin by checking surrounding units.

									  System.Collections.Generic.List<CacheUnit> surroundingList;
									  ObjectCache.Objects.FindSurroundingObjects(this.Position, 50f, out surroundingList);
									  surroundingList.RemoveAll(p => !p.IsEliteRareUnique&&!p.IsBoss);
									  surroundingList.TrimExcess();

									  if (surroundingList.Count>0)
									  {
											//See if any of those elites/rares/bosses are closer than the goblin but further than 20f from the goblin..
											float distanceFromGoblin=this.RadiusDistance;
											if (surroundingList.Any(u => u.RadiusDistance<distanceFromGoblin
																				 &&u.Position.Distance(this.Position)>20f))
											{
												 return 0f;
											}


											//Either no above average units or they are farther than the goblin is..
											//We will let calculations below preform instead!
									  }
								 }


								 //Use a shorter range if not yet noticed..
								 if (this.CurrentHealthPct<=0.10)
									  dUseKillRadius+=Bot.TreasureGoblinRange+(Bot.SettingsFunky.GoblinPriority*24);
								 else if (this.CurrentHealthPct<=0.99)
									  dUseKillRadius+=Bot.TreasureGoblinRange+(Bot.SettingsFunky.GoblinPriority*16);
								 else
									  dUseKillRadius+=Bot.TreasureGoblinRange+(Bot.SettingsFunky.GoblinPriority*12);

								 this.ForceLeap=true;
							}
							// Elitey type mobs and things
							else if ((this.IsEliteRareUnique))
							{
								 dUseKillRadius+=Bot.EliteRange;
								 this.ForceLeap=true;
							}
							else
								 //Not Boss, Goblin, Elite/Rare/Unique..
								 dUseKillRadius+=Bot.NonEliteRange;


						  // Standard 50f range when preforming OOC behaviors!
							if (Bot.IsInNonCombatBehavior)
								dUseKillRadius=50;
							
						  return dUseKillRadius;
					 }
				}

				private void TallyTarget()
				{
					 bool bIsRended=false;
					 bool bCountAsElite=false;

					 bIsRended=(this.HasDOTdps.HasValue&&this.HasDOTdps.Value);
					 bCountAsElite=(this.IsEliteRareUnique||this.IsTreasureGoblin||this.IsBoss);
					 float RadiusDistance=this.RadiusDistance;

					 if (Bot.SettingsFunky.EnableFleeingBehavior&&RadiusDistance<=Bot.SettingsFunky.FleeMaxMonsterDistance&&this.ShouldBeKited)
							Bot.Combat.FleeTriggeringUnits.Add(this);


					 if (RadiusDistance<=6f)
					 {
						  Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_6]++;
						  if (bCountAsElite)
								 Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_6]++;
					 }
					 if (RadiusDistance<=12f)
					 {
						  //Tally close units
						  Bot.Combat.SurroundingUnits++;
						  //Herbfunk: non-rend count only if within 8f and is attackable..
						  if (Bot.Class.AC==Zeta.Internals.Actors.ActorClass.Barbarian&&!bIsRended&&RadiusDistance<=7f&&this.IsTargetable.Value)
								Bot.Combat.iNonRendedTargets_6++;

							Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_12]++;
						  if (bCountAsElite)
								 Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_12]++;
					 }
					 if (RadiusDistance<=15f)
					 {
							Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_15]++;
						  if (bCountAsElite)
								 Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_15]++;
					 }
					 if (RadiusDistance<=20f)
					 {
							Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_20]++;
						  if (bCountAsElite)
								 Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_20]++;
					 }
					 if (RadiusDistance<=25f)
					 {
						  if (!Bot.Combat.bAnyNonWWIgnoreMobsInRange&&!CacheIDLookup.hashActorSNOWhirlwindIgnore.Contains(this.SNOID))
								Bot.Combat.bAnyNonWWIgnoreMobsInRange=true;
							Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_25]++;
						  if (bCountAsElite)
								 Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_25]++;
					 }
					 if (RadiusDistance<=30f)
					 {
							Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_30]++;
						  if (bCountAsElite)
								 Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_30]++;
					 }
					 if (RadiusDistance<=40f)
					 {
							Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_40]++;
						  if (bCountAsElite)
								 Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_40]++;
					 }
					 if (RadiusDistance<=50f)
					 {
							Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_50]++;
						  if (bCountAsElite)
								 Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_50]++;
					 }
				}
				#endregion

				internal override GPRectangle GPRect
				{
					 get
					 {
						  if (base.GPRect.CreationVector!=this.Position)
								 base.GPRect=new GPRectangle(this.Position, (int)Math.Sqrt(this.ActorSphereRadius.Value)*2);

						  return base.GPRect;
					 }
				}

				internal UnitCluster CurrentTargetCluster
				{
					 get
					 {
						  if (Bot.Combat.TargetClusterCollection.CurrentClusters.Count>0)
						  {
								var containedClusters=Bot.Combat.TargetClusterCollection.CurrentClusters.Where(c => c.RAGUIDS.Contains(this.RAGUID));
								if (containedClusters.Any())
									 return containedClusters.First();
						  }

						  return null;
					 }
				}

				public override bool IsZDifferenceValid
				{
					 get
					 {
						  float fThisHeightDifference=Funky.Difference(Bot.Character.Position.Z, this.Position.Z);
						  if (fThisHeightDifference>=10f)
						  {
								//raycast.. 
								 if (!Navigation.CanRayCast(Bot.Character.Position, this.Position))
								{
									 return false;
								}
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

				public override void UpdateWeight()
				{
					 base.UpdateWeight();

					 //Ignore non-clustered, *only when not prioritized!*

					 if (this.BeingIgnoredDueToClusterLogic
						  &&this.PriorityCounter==0
						  &&(Bot.Target.CurrentTarget!=null
						  ||Bot.Combat.iAnythingWithinRange[(int)RangeIntervals.Range_30]==0
						  ||ObjectCache.Objects.objectsIgnoredDueToAvoidance.Count==0))
					 {
						  this.Weight=0;
						  return;
					 }

					 if (this.RadiusDistance>=5f&&Bot.Class.IsMeleeClass)
					 {
						  if (DateTime.Now.Subtract(LastAvoidanceIgnored).TotalMilliseconds<1000&&Bot.Combat.NearbyAvoidances.Count>0)
						  {
								this.Weight=1;
						  }
						  else
						  {
								Vector3 TestPosition=this.BotMeleeVector;
								if (ObjectCache.Obstacles.IsPositionWithinAvoidanceArea(TestPosition))
									 this.Weight=1;

								//intersecting avoidances..
								if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(TestPosition))
								{
									 if (this.Weight!=1&&this.ObjectIsSpecial)
									 {//Only add this to the avoided list when its not currently inside avoidance area
										  ObjectCache.Objects.objectsIgnoredDueToAvoidance.Add(this);
									 }
									 else
										  this.Weight=1;
								}
						  }
					 }

					 //Range Class Ignore (Avoid/Kite last target!)
					 if ((Bot.Combat.FleeingLastTarget&&Bot.Combat.FleeTriggeringUnits.Count>0&&Bot.Combat.FleeTriggeringUnits.Contains(this))||
						  (Bot.Combat.AvoidanceLastTarget&&Bot.Combat.TriggeringAvoidances.Count>0))
						  this.Weight=1;


					 if (this.Weight!=1)
					 {
						  float centreDistance=this.CentreDistance;
						  float radiusDistance=this.RadiusDistance;

						  // Flag up any bosses in range
						  if (this.IsBoss&&centreDistance<=50f)
								this.Weight+=9999;

						  // Force a close range target because we seem to be stuck *OR* if not ranged and currently rooted
						  if (Bot.Combat.bForceCloseRangeTarget||Bot.Character.bIsRooted)
						  {

								this.Weight=20000-(Math.Floor(radiusDistance)*200);

								// Goblin priority KAMIKAZEEEEEEEE
								if (this.IsTreasureGoblin&&Bot.SettingsFunky.GoblinPriority>1)
									 this.Weight+=10250*(Bot.SettingsFunky.GoblinPriority-1);
						  }
						  else
						  {
								// Not attackable, could be shielded, make super low priority
								if (!this.IsTargetable.Value&&!this.IsWormBoss)
								{
									 // Only 500 weight helps prevent it being prioritized over an unshielded
									 this.Weight=500;
								}
								// Not forcing close-ranged targets from being stuck, so let's calculate a weight!
								else
								{
									 // Starting weight of 5000 to beat a lot of crap weight stuff
									 this.Weight=5000;

									 // Distance as a percentage of max radius gives a value up to 1000 (1000 would be point-blank range)

									 if (radiusDistance<Bot.iCurrentMaxKillRadius)
									 {
										  int RangeModifier=1200;
										  //Increase Distance Modifier if recently kited.
										  if (Bot.SettingsFunky.EnableFleeingBehavior&&DateTime.Now.Subtract(Bot.Combat.LastFleeAction).TotalMilliseconds<3000)
												RangeModifier=12000;


										  this.Weight+=(RangeModifier*(1-(radiusDistance/Bot.iCurrentMaxKillRadius)));
									 }

									 // Give extra weight to ranged enemies
									 if ((Bot.Class.IsMeleeClass)&&
										 (this.Monstersize==MonsterSize.Ranged||CacheIDLookup.hashActorSNORanged.Contains(this.SNOID)))
									 {
										  this.Weight+=1100;
										  this.ForceLeap=true;
									 }

									 // Give more weight to elites and minions
									 if (this.IsEliteRareUnique)
										  this.Weight+=2000;

									 // Give more weight to bosses
									 if (this.IsBoss)
										  this.Weight+=4000;

									 // Barbarians with wrath of the berserker up should prioritize elites more
									 if (Bot.Class.HasBuff(SNOPower.Barbarian_WrathOfTheBerserker)&&(this.IsEliteRareUnique||this.IsTreasureGoblin||this.IsBoss))
										  this.Weight+=2000;


									 // Swarmers/boss-likes get more weight
									 if (this.Monstersize==MonsterSize.Swarm||this.Monstersize==MonsterSize.Boss)
										  this.Weight+=900;

									 // Standard/big get a small bonus incase of "unknown" monster types being present
									 if (this.Monstersize==MonsterSize.Standard||this.Monstersize==MonsterSize.Big)
										  this.Weight+=150;

									 // Lower health gives higher weight - health is worth up to 300 extra weight
									 if (this.CurrentHealthPct<0.20)
										  this.Weight+=(300*(1-(this.CurrentHealthPct.Value/0.5)));

									 // Elites on low health get extra priority - up to 1500
									 if ((this.IsEliteRareUnique||this.IsTreasureGoblin)&&this.CurrentHealthPct<0.20)
										  this.Weight+=(1500*(1-(this.CurrentHealthPct.Value/0.45)));

									 // Goblins on low health get extra priority - up to 2500
									 if (Bot.SettingsFunky.GoblinPriority>=2&&this.IsTreasureGoblin&&this.CurrentHealthPct<=0.98)
										  this.Weight+=(3000*(1-(this.CurrentHealthPct.Value/0.85)));

									 // Bonuses to priority type monsters from the dictionary/hashlist set at the top of the code
									 int iExtraPriority;
									 if (CacheIDLookup.dictActorSNOPriority.TryGetValue(this.SNOID, out iExtraPriority))
									 {
										  this.Weight+=iExtraPriority;
									 }

									 // Close range get higher weights the more of them there are, to prevent body-blocking
									 // Plus a free bonus to anything close anyway
									 if (radiusDistance<=11f)
									 {
										  // Extra bonus for point-blank range
										  //iUnitsSurrounding++;
										  // Give special "surrounded" weight to each unit
										  this.Weight+=(200*Bot.Combat.SurroundingUnits);
									 }

									 // Special additional weight for corrupt growths in act 4 ONLY if they are at close range (not a standard priority thing)
									 if ((this.SNOID==210120||this.SNOID==210268)&&centreDistance<=35f)
										  this.Weight+=2000;

									 // Was already a target and is still viable, give it some free extra weight, to help stop flip-flopping between two targets
									 if (this==Bot.Character.LastCachedTarget&&centreDistance<=25f)
										  this.Weight+=400;

									 //// Lower the priority for EACH AOE *BETWEEN* us and the target, NOT counting the one directly under-foot, up to a maximum of 1500 reduction
									 //Vector3 thisObjPosition=this.Position;
									 //float fWeightRemoval=0;
									 //Vector3 BotPosition=Bot.Character.Position;
									 //foreach (CacheAvoidance tempobstacle in ObjectCache.Obstacles.Avoidances.Where(cp => cp.TestIntersection(this, BotPosition)))
									 //{
									 //    fWeightRemoval+=(float)tempobstacle.Weight*8;
									 //}
									 //if (fWeightRemoval>1500)
									 //    fWeightRemoval=1500;
									 //this.Weight-=fWeightRemoval;

									 // Lower the priority if there is AOE *UNDER* the target, by the HIGHEST weight there only
									 //fWeightRemoval=0;

									 //foreach (CacheAvoidance tempobstacle in ObjectCache.Obstacles.Avoidances.Where(cp => cp.PointInside(this.Position)))
									 //{
									 //    // Up to 200 weight for a high-priority AOE - maximum 3400 weight reduction
									 //    if (tempobstacle.Weight>fWeightRemoval)
									 //        fWeightRemoval=(float)tempobstacle.Weight*30;
									 //}
									 //this.Weight-=fWeightRemoval;

									 // Prevent going less than 300 yet to prevent annoyances (should only lose this much weight from priority reductions in priority list?)
									 if (this.Weight<300)
										  this.Weight=300;


									 // Deal with treasure goblins - note, of priority is set to "0", then the is-a-goblin flag isn't even set for use here - the monster is ignored
									 if (this.IsTreasureGoblin)
									 {
										  // Logging goblin sightings
										  if (Bot.Combat.lastGoblinTime==DateTime.Today)
										  {
												Bot.Combat.iTotalNumberGoblins++;
												Bot.Combat.lastGoblinTime=DateTime.Now;
												Logging.Write("[Funky] Goblin #"+Bot.Combat.iTotalNumberGoblins.ToString()+" in sight. Distance="+centreDistance);
										  }
										  else
										  {
												if (DateTime.Now.Subtract(Bot.Combat.lastGoblinTime).TotalMilliseconds>30000)
													 Bot.Combat.lastGoblinTime=DateTime.Today;
										  }
										  // Original Trinity stuff for priority handling now
										  switch (Bot.SettingsFunky.GoblinPriority)
										  {
												case 2:
													 // Super-high priority option below... 
													 this.Weight+=10101;
													 break;
												case 3:
													 // KAMIKAZE SUICIDAL TREASURE GOBLIN RAPE AHOY!
													 this.Weight+=40000;
													 break;
												// PS: 58008 is an awesome number on any calculator.

										  }

										  
									 }

								} // Forcing close range target or not?
						  } // This is an attackable unit
					 }
					 else
					 {
						  LastAvoidanceIgnored=DateTime.Now;
						  if (!this.ObjectIsSpecial)
								this.Weight=0;
					 }

				}

				public override bool ObjectIsValidForTargeting
				{
					 get
					 {
						  if (!base.ObjectIsValidForTargeting)
								return false;

						  //Z-Height Difference Check
						  if (!this.IsZDifferenceValid)
						  {
								//this.BlacklistLoops=3;
								return false;
						  }

						  #region Validations
						  // Unit is already dead
						  if (this.CurrentHealthPct.HasValue&&(this.CurrentHealthPct.Value<=0d))
						  {

								if (!base.IsRespawnable)
								{
									 this.NeedsRemoved=true;
									 return false;
								}
								else
								{
									 this.BlacklistLoops=5;
									 return false;
								}
						  }

						  #endregion

						  //Attackable/Burrowed?
						  if ((this.IsTargetable.HasValue&&this.IsTargetable==false)||
								 (this.IsBurrowed.HasValue&&this.IsBurrowed.Value==true)||
								(this.IsAttackable.HasValue&&this.IsAttackable.Value==false))
						  {
								//We skip all but worm bosses in A2 and monsters who can shield.
								if (!this.IsWormBoss&&!this.MonsterShielding&&(!this.IsEliteRareUnique||this.IsGrotesqueActor))
								{
									 if (base.IsGrotesqueActor)
									 {
										  //Setup this as an avoidance object now!
										  this.HandleAsAvoidanceObject=true;
										  CacheAvoidance newAvoidance=new CacheAvoidance(this, AvoidanceType.GrotesqueExplosion);
										  ObjectCache.Obstacles[this.RAGUID]=newAvoidance;

										  return false;
									 }

									 //Stealthable units -- low blacklist counter
									 if (base.IsStealthableUnit)
										  this.BlacklistLoops=2;
									 else if (base.IsBurrowableUnit)
										  this.BlacklistLoops=5;
									 else
										  this.BlacklistLoops=10;

									 return false;
								}
						  }


						  float centreDistance=this.CentreDistance;

						  bool distantUnit=false;
						  bool validUnit=false;
						  //Distant Units List
						  if (centreDistance>=Bot.SettingsFunky.GroupingMinimumUnitDistance&&Bot.SettingsFunky.AttemptGroupingMovements)
						  {
							  	
								distantUnit=true;
						  }


						  if (centreDistance>this.KillRadius&&!distantUnit)
						  {
								return false;
						  }
						  else
								validUnit=true;
								


						  //Line of sight pre-check
						  if (this.RequiresLOSCheck)
						  {

								double lastLOSCheckMS=this.LastLOSCheckMS;


								if (lastLOSCheckMS<500)
									 return false;
								else
								{
									 //Last Health Changed, Distance, Special Unit..
									 double ReCheckTime=7500;

									 if (DateTime.Now.Subtract(this.LastHealthChange).TotalMilliseconds<5000)
										  ReCheckTime*=0.75;

									 //Close Range.. we change the recheck timer based on how close
									 if (this.CentreDistance<25f)
										  ReCheckTime=(this.CentreDistance*225);
									 else if (this.ObjectIsSpecial)
										  ReCheckTime*=0.25;

									 if (lastLOSCheckMS<ReCheckTime)
										  return false;
								}

								NavCellFlags LOSNavFlags=NavCellFlags.None;
								//if ((Bot.Class.IsMeleeClass||!this.WithinInteractionRange())&&!distantUnit)
								//	 LOSNavFlags=NavCellFlags.AllowWalk;
								//if (!Bot.Class.IsMeleeClass)
								//    LOSNavFlags|=NavCellFlags.AllowProjectile;

								if (!base.LOSTest(Bot.Character.Position, true, (!Bot.Class.IsMeleeClass&&!distantUnit), LOSNavFlags))
								{
									 if (!Bot.Character.bIsIncapacitated)
										  this.BlacklistLoops=10;
									 else//Incapacitated we reset check
										  this.LastLOSCheck=DateTime.Today;

									 return false;
								}

								this.RequiresLOSCheck=false;
						  } 
	


						  #region CombatFlags

						  if (this.IsBoss||this.IsEliteRareUnique)
						  {
								//Ignore Setting?
								if (Bot.SettingsFunky.IgnoreAboveAverageMobs&&this.PriorityCounter<=1&&!Bot.IsInNonCombatBehavior&&!this.IsBoss)
									 return false;

								Bot.Combat.bAnyChampionsPresent=true;
						  }

						  if (this.IsTreasureGoblin)
								Bot.Combat.bAnyTreasureGoblinsPresent=true;



						  // Units with very high priority (1900+) allow an extra 50% on the non-elite kill slider range
						  if (!Bot.Combat.bAnyMobsInCloseRange&&!Bot.Combat.bAnyChampionsPresent&&!Bot.Combat.bAnyTreasureGoblinsPresent&&this.CentreDistance<=(Bot.NonEliteRange*1.5))
						  {
								int iExtraPriority;
								// Enable extended kill radius for specific unit-types
								if (CacheIDLookup.hashActorSNORanged.Contains(this.SNOID))
								{
									 Bot.Combat.bAnyMobsInCloseRange=true;
								}
								if (!Bot.Combat.bAnyMobsInCloseRange&&CacheIDLookup.dictActorSNOPriority.TryGetValue(this.SNOID, out iExtraPriority))
								{
									 if (iExtraPriority>=1900)
									 {
										  Bot.Combat.bAnyMobsInCloseRange=true;
									 }
								}
						  }

						  // Total up monsters at various ranges
						  if (centreDistance<=50f)
						  {
								this.TallyTarget();
						  }

						  #endregion


						  //Profile Blacklisted.
						  if (ObjectCache.hashProfileSNOTargetBlacklist.Contains(this.SNOID))
						  {
								//Only if not prioritized..
								if (!Bot.IsInNonCombatBehavior)
									 return false;
						  }

						  if (distantUnit)
								Bot.Combat.DistantUnits.Add(this); //Add this valid unit to Distant List.
						  if (validUnit) //Add this valid unit RAGUID to list
								Bot.Combat.UnitRAGUIDs.Add(this.RAGUID);



						  return true;
					 }
				}

				public override bool UpdateData()
				{
					 
					 if (!base.IsStillValid())
						  return false;

					 if (this.ref_DiaUnit==null)
					 {
						  try
						  {
								this.ref_DiaUnit=base.ref_DiaObject as DiaUnit;
						  } catch (NullReferenceException) { Logger.Write(LogLevel.Exception, "Failure to convert obj to DiaUnit!"); return false; }
					 }

					 ACD CommonData=base.ref_DiaObject.CommonData;
					 if (CommonData==null)
					 {
						  Logger.Write(LogLevel.Exception, "Common Data Null!");
						  return false;
					 }


					 if (!base.Monstertype.HasValue)
						  return false;
					 //Update Monster Type?
					 if (base.ShouldRefreshMonsterType)
					 {
						  if (!base.UpdateData(base.ref_DiaObject, base.RAGUID))
								return false;
					 }

					 //NPC Check
					 bool isNPC=false;
					 if (!this.IsNPC.HasValue||this.IsNPC.Value==true)
					 {
						  try
						  {
								this.IsNPC=(base.ref_DiaObject.CommonData.GetAttribute<float>(ActorAttributeType.IsNPC)>0);
								isNPC=this.IsNPC.Value;
						  } catch (Exception)
						  {
								Logger.Write(LogLevel.Exception, "Safely Handled Getting Attribute IsNPC for object {0}", this.InternalName);
						  }

					 }



					 // Make sure it's a valid monster type
					 if (!base.MonsterTypeIsHostile()||isNPC)
					 {
						  if (Bot.Character.bIsInTown)
						  {
								//Perma Ignore all NPCs we find in town..
								if (isNPC)
									 ObjectCache.IgnoreThisObject(this, true, true);

								return false;
						  }

						  if (isNPC||!this.IsBoss)
								return false;
					 }



					 //Position update
					 base.UpdatePosition();

					 if (this.Radius==0f)
					 {
						  if (base.ActorSphereRadius.HasValue)
						  {
								this.Radius=base.ActorSphereRadius.Value;

								if (this.Monstersize.Value==Zeta.Internals.SNO.MonsterSize.Big||this.Monstersize.Value==Zeta.Internals.SNO.MonsterSize.Boss)
									 this.Radius*=0.25f;

								if (this.Radius<0f)
									 this.Radius=1f;
						  }
					 }

					 //Affixes
					 if (!this.CheckedMonsterAffixes_)
					 {
						  try
						  {
								this.CheckMonsterAffixes(CommonData.MonsterAffixes);
						  } catch (NullReferenceException)
						  {
								Logger.Write(LogLevel.Exception, "Failure to check monster affixes for unit {0}", base.InternalName);
								return false;
						  }

					 }

					 //Hitpoints
					 if (!this.MaximumHealth.HasValue)
					 {
						  try
						  {
								this.MaximumHealth=this.ref_DiaUnit.HitpointsMaxTotal;

								if (!this.IsEliteRareUnique||!this.IsBoss)
								{
									 if (!ObjectCache.Objects.HealthEntriesForAverageValue.ContainsKey(this.RAGUID))
									 {
										  ObjectCache.Objects.HealthEntriesForAverageValue.Add(this.RAGUID, this.MaximumHealth.Value);
										  ObjectCache.Objects.UpdateMaximumHealthAverage();
									 }
								}
						  } catch (Exception) { Logger.Write(LogLevel.Exception, "Failure to get maximum health for {0}", base.InternalName); return false; }
					 }


					 //update HPs
					 this.UpdateHitPoints();

					 if (this.CurrentHealthPct.HasValue&&this.CurrentHealthPct.Value<=0d)
					 {
						  this.NeedsRemoved=true;
						  return false;
					 }



					 //Burrowing?
					 #region Burrowed?
					 if ((this.CurrentHealthPct.HasValue&&this.CurrentHealthPct.Value>=1d||CacheIDLookup.hashActorSNOBurrowableUnits.Contains(base.SNOID)||(!this.IsBurrowed.HasValue||this.IsBurrowed.Value)))
					 {
						  try
						  {
								//this.IsBurrowed=this.ref_DiaUnit.IsBurrowed;
								this.IsBurrowed=base.ref_DiaObject.CommonData.GetAttribute<float>(ActorAttributeType.Burrowed)>0;

								//ignore units who are stealthed completly (exception when object is special!)
								//if (this.IsBurrowed.Value&&!this.ObjectIsSpecial)
								//return false;
						  } catch (Exception) { }
					 }
					 #endregion

					 //Targetable
					 if (!this.IsTargetable.HasValue||!this.IsTargetable.Value)
					 {
						  try
						  {
								//this.IsAttackable=this.ref_DiaUnit.IsAttackable;
								bool stealthed=false;
								//Special units who can stealth
								if (base.IsStealthableUnit)
									 stealthed=(this.ref_DiaUnit.CommonData.GetAttribute<float>(ActorAttributeType.Stealthed)<=0);

								if (!stealthed)
									 this.IsTargetable=(this.ref_DiaUnit.CommonData.GetAttribute<float>(ActorAttributeType.Untargetable)<=0);
								else
								{
									 this.IsTargetable=stealthed;
									 //since stealth is similar to being burrowed we skip non-special units
									 //if (!this.ObjectIsSpecial)
									 //return false;
								}
						  } catch (Exception ex)
						  {
								Logger.Write(LogLevel.Exception, "[Funky] Safely handled exception getting is-targetable attribute for unit "+this.InternalName+" ["+this.SNOID.ToString()+"]");
								Logging.WriteDiagnostic(ex.ToString());
								this.IsTargetable=true;
						  }
					 }

					 //Attackable
					 if (this.MonsterShielding||(this.IsGrotesqueActor&&this.CurrentHealthPct.HasValue&&(this.CurrentHealthPct.Value<1d||this.CurrentHealthPct.Value>1d)))
					 {
						  try
						  {
								this.IsAttackable=(this.ref_DiaUnit.IsAttackable);
						  } catch (NullReferenceException)
						  {

						  }
					 }


					 //Clustering Target Engaged Check
					 //if (Bot.SettingsFunky.AttemptGroupingMovements)
					 //{
					 //    try
					 //    {
					 //        this.IsMoving=this.ref_DiaObject.Movement.IsMoving;
					 //    } catch
					 //    {
					 //        Logging.WriteVerbose("[Funky] Safely handled exception getting LastACDAttackedBy attribute for unit "+this.InternalName+" ["+this.SNOID.ToString()+"]");
					 //    }
					 //}

					 #region Class DOT DPS Check
					 //Barb specific updates
					 if (Bot.Class.AC==ActorClass.Barbarian)
					 {
						  //Rend DotDPS update
							if (Bot.Class.HotbarPowers.Contains(SNOPower.Barbarian_Rend))
						  {
								try
								{
									 this.HasDOTdps=(this.ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.Bleeding)>0&&this.ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.DOTDPS)>0);
								} catch (AccessViolationException) { }
						  }
					 }
					 else if (Bot.Class.AC==Zeta.Internals.Actors.ActorClass.WitchDoctor)
					 {
						  //Haunted DotDPS update
							if (Bot.Class.HotbarPowers.Contains(SNOPower.Witchdoctor_Haunt)||Bot.Class.HotbarPowers.Contains(SNOPower.Witchdoctor_Locust_Swarm))
						  {
								Bot.Combat.UsesDOTDPSAbility=true;
								try
								{
									 //Haunted units always have buff visual effect!
									 //bool buffVisualEffect=(this.ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.BuffVisualEffect)>0);

									 int dotDPS=this.ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.DOTDPS);
									 int visualBuff=this.ref_DiaUnit.CommonData.GetAttribute<int>(ActorAttributeType.BuffVisualEffect);
									 this.HasDOTdps=(dotDPS>0&&visualBuff>0);
									 /*
									 if (Bot.Class.HotbarAbilities.Contains(SNOPower.Witchdoctor_Haunt))
									 {
										  this.HasDOTdps=(visualBuff>0&&(dotDPS==1194344448||dotDPS==1194786816||dotDPS==1202929855||dotDPS==1194983424||dotDPS==1196072960||dotDPS==1194770432));
									 }
									 else if (Bot.Class.HotbarAbilities.Contains(SNOPower.Witchdoctor_Locust_Swarm))
									 {
										  this.HasDOTdps=(visualBuff>0&&(dotDPS==1178820608||dotDPS==1197301760||dotDPS==1182662656));
									 }
									 */

									 //haunt DotDPS values
									 //dotDPS==1194344448||dotDPS==1194786816||dotDPS==1202929855||dotDPS==1194983424||dotDPS==1196072960||dotDPS==1194770432)

									 //lotus swarm
									 //1178820608 1197301760 1182662656
								} catch (AccessViolationException) { }
						  }
					 }
					 #endregion

					 return true;
				}

				public override bool IsStillValid()
				{
					 if (ref_DiaUnit==null||!ref_DiaUnit.IsValid||ref_DiaUnit.BaseAddress==IntPtr.Zero)
						  return false;






					 return base.IsStillValid();
				}

				public override RunStatus Interact()
				{
					 if (Bot.Combat.powerPrime.Power!=SNOPower.None)
					 {
						  //Check our Interaction LOS!
						  this.CheckInteractionLOS();

						  // Force waiting for global cooldown timer or long-animation abilities
							if (Bot.Combat.powerPrime.WaitLoopsBefore>=1||(Bot.Combat.powerPrime.WaitWhileAnimating!=false&&DateTime.Now.Subtract(PowerCacheLookup.lastGlobalCooldownUse).TotalMilliseconds<=50))
						  {
								//Logging.WriteDiagnostic("Debug: Force waiting BEFORE ability " + powerPrime.powerThis.ToString() + "...");
								Bot.Combat.bWaitingForPower=true;
								if (Bot.Combat.powerPrime.WaitLoopsBefore>=1)
									 Bot.Combat.powerPrime.WaitLoopsBefore--;
								return RunStatus.Running;
						  }
						  Bot.Combat.bWaitingForPower=false;

						  // Wait while animating before an attack
						  if (Bot.Combat.powerPrime.WaitWhileAnimating)
								Bot.Character.WaitWhileAnimating(5, false);

						  // Note that whirlwinds use an off-on-off-on to avoid spam
						  if (Bot.Combat.powerPrime.Power!=SNOPower.Barbarian_Whirlwind&&Bot.Combat.powerPrime.Power!=SNOPower.DemonHunter_Strafe)
						  {
								Ability.UsePower(ref Bot.Combat.powerPrime);
								Bot.Combat.lastChangedZigZag=DateTime.Today;
								Bot.Combat.vPositionLastZigZagCheck=Vector3.Zero;
						  }
						  else
						  {
								// Special code to prevent whirlwind double-spam, this helps save fury
								bool bUseThisLoop=Bot.Combat.powerPrime.Power!=Bot.Combat.powerLastSnoPowerUsed;
								if (!bUseThisLoop)
								{
									 //powerLastSnoPowerUsed = SNOPower.None;
									 if (DateTime.Now.Subtract(PowerCacheLookup.dictAbilityLastUse[Bot.Combat.powerPrime.Power]).TotalMilliseconds>=200)
										  bUseThisLoop=true;
								}
								if (bUseThisLoop)
								{
									 Ability.UsePower(ref Bot.Combat.powerPrime);
								}
						  }

						  if (Bot.Combat.powerPrime.SuccessUsed.HasValue&&Bot.Combat.powerPrime.SuccessUsed.Value)
						  {
								//Logging.Write(powerPrime.powerThis.ToString() + " used successfully");
								Bot.Combat.powerLastSnoPowerUsed=Bot.Combat.powerPrime.Power;
								Bot.Combat.powerPrime.SuccessfullyUsed();
						  }
						  else
						  {
								 PowerCacheLookup.dictAbilityLastFailed[Bot.Combat.powerPrime.Power]=DateTime.Now;
						  }

						  // Wait for animating AFTER the attack
						  if (Bot.Combat.powerPrime.WaitWhileAnimating)
								Bot.Character.WaitWhileAnimating(3, false);

						  Bot.Combat.bPickNewAbilities=true;

						  // See if we should force a long wait AFTERWARDS, too
						  // Force waiting AFTER power use for certain abilities
						  Bot.Combat.bWaitingAfterPower=false;
						  if (Bot.Combat.powerPrime.WaitLoopsAfter>=1)
						  {
								//Logging.WriteDiagnostic("Force waiting AFTER ability " + powerPrime.powerThis.ToString() + "...");
								Bot.Combat.bWaitingAfterPower=true;
						  }

						  return RunStatus.Running;
					 }

					 return RunStatus.Running;
				}

				private void CheckInteractionLOS()
				{
					 if (this.LastLOSCheckMS>3000)
					 {
						  if (!this.IgnoresLOSCheck)
						  {
								NavCellFlags LOSNavFlags=NavCellFlags.None;

								if (!this.WithinInteractionRange())//Ability requires movement prior to use, so we test nav flags.
									 LOSNavFlags=NavCellFlags.AllowWalk;
								
								 if (Bot.Combat.powerPrime.IsRanged) //Add Projectile Testing!
										LOSNavFlags|=NavCellFlags.AllowProjectile;


								 if (!this.LOSTest(Bot.Character.Position, true, Bot.Combat.powerPrime.IsRanged, LOSNavFlags))
								{
									 bool Valid=false;
									 //LOS failed.. now we should decide if we want to find a spot for this target, or just ignore it.
									 if (this.ObjectIsSpecial&&this.LastLOSSearchMS>2500)
									 {
										  this.LastLOSSearch=DateTime.Now;

										  GPRectangle TargetGPRect=this.GPRect;
										  //Expand GPRect into 5x5, 7x7 for ranged!
										  TargetGPRect.FullyExpand();

										  if (!Bot.Class.IsMeleeClass) TargetGPRect.FullyExpand();

										  Vector3 LOSV3;
										  if (TargetGPRect.TryFindSafeSpot(Bot.Character.Position, out LOSV3, this.BotMeleeVector))
										  {
												this.LOSV3=LOSV3;
												Logging.WriteVerbose("Using LOS Vector at {0} to move to", LOSV3.ToString());
												this.RequiresLOSCheck=false;
												Valid=true;
										  }
									 }

									 if (!Valid)
									 {
										  this.RequiresLOSCheck=true;
									 }
								}
								else
									 this.RequiresLOSCheck=false;
						  }
					 }
				}

				public override bool CanInteract()
				{
					 if (!this.IsTargetable.Value)
						  return false;

					 return base.CanInteract();
				}

				public override bool WithinInteractionRange()
				{
					 float fRangeRequired=0f;
					 float fDistanceReduction=0f;

					 if (Bot.Combat.bForceCloseRangeTarget)
						  fDistanceReduction-=3f;
					 if (fDistanceReduction<=0f)
						  fDistanceReduction=0f;

					 //Check if we should mod our distance:: used for worm bosses
					 if (base.IsWormBoss)
						  Bot.Combat.powerPrime.MinimumRange=Bot.Class.IsMeleeClass?14:16;
					 else if (base.IgnoresLOSCheck)
						  Bot.Combat.powerPrime.MinimumRange=(int)(base.ActorSphereRadius.Value*1.5);
					 else if (this.IsBurrowed.HasValue&&this.IsBurrowed.Value&&this.IsEliteRareUnique)//Force close range on burrowed elites!
						  Bot.Combat.powerPrime.MinimumRange=15;
					 else if (this.IsStealthableUnit&&this.IsAttackable.HasValue&&this.IsAttackable.Value==false&&this.IsEliteRareUnique)
						  Bot.Combat.powerPrime.MinimumRange=15;
					 else if (this.IsTreasureGoblin&&!Bot.Class.IsMeleeClass&&Bot.SettingsFunky.Class.GoblinMinimumRange>0)
						  Bot.Combat.powerPrime.MinimumRange=Bot.SettingsFunky.Class.GoblinMinimumRange;
					 else if (this.MonsterMissileDampening&&Bot.SettingsFunky.MissleDampeningEnforceCloseRange)
						  Bot.Combat.powerPrime.MinimumRange=15;
					 else
						  fDistanceReduction=base.Radius;

					 // Pick a range to try to reach
					 fRangeRequired=Bot.Combat.powerPrime.Power==SNOPower.None?9f:Bot.Combat.powerPrime.MinimumRange;

					 base.DistanceFromTarget=base.CentreDistance-fDistanceReduction;


					 return (fRangeRequired<=0f||base.DistanceFromTarget<=fRangeRequired);
				}

				public override bool ObjectIsSpecial
				{
					 get
					 {
						  if ((this.IsEliteRareUnique&&!Bot.SettingsFunky.IgnoreAboveAverageMobs)||
									 (this.PriorityCounter>0)||
									 (this.IsBoss&&!Bot.SettingsFunky.IgnoreAboveAverageMobs&&this.CurrentHealthPct<=0.99d)||
									 ((this.IsSucideBomber||this.IsCorruptantGrowth)&&this.CentreDistance<45f)||
									 (this.IsSpawnerUnit&&Bot.SettingsFunky.ClusteringAllowSpawnerUnits)||
									 ((this.IsTreasureGoblin&&Bot.SettingsFunky.GoblinPriority>1))||
									 (this.Monstersize.Value==MonsterSize.Ranged&&Bot.SettingsFunky.ClusteringAllowRangedUnits
										  &&(!this.IsEliteRareUnique||!Bot.SettingsFunky.IgnoreAboveAverageMobs))||
									 ((Bot.SettingsFunky.ClusterKillLowHPUnits&&((this.CurrentHealthPct<0.25&&this.UnitMaxHitPointAverageWeight>0)
												&&(!this.IsEliteRareUnique||!Bot.SettingsFunky.IgnoreAboveAverageMobs)
												&&((!Bot.Class.IsMeleeClass&&this.CentreDistance<30f)||(Bot.Class.IsMeleeClass&&this.RadiusDistance<12f))))))



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
								this.IsBurrowed.HasValue?this.IsBurrowed.Value.ToString():"",
								this.IsTargetable.HasValue?this.IsTargetable.Value.ToString():"",
								this.IsAttackable.HasValue?this.IsAttackable.Value.ToString():"",
								this.CurrentHealthPct.HasValue?this.CurrentHealthPct.Value.ToString():"",
								this.MaximumHealth.HasValue?this.MaximumHealth.Value.ToString():"",
								this.IsMoving.ToString(),
								this.PriorityCounter.ToString(),
								this.Properties.ToString());
					 }
				}


		  }


	 
}