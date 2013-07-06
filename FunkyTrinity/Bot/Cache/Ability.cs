using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;
using System.Collections.Generic;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  //Condition Type -- Condition Parameters

		  //CheckClusterConditions

		  //PlayerCurrentHealth

		  //Units In Range -- Distance, Count, ElitesOnly

		  //Target Distance -- Optional Use RadiusDistance
		  //Target Special
		  //Target Health
		  //Target Special Property -- (Add Enum for each one) [Reflecting/MissleDampening/Shielding/Boss/Rare&Elites/Unique/TreasureGoblin]

		  [Flags]
		  public enum ConditionCriteraTypes
		  {
				None=0,
				Cluster=1,
				UnitsInRange=2,
				ElitesInRange=4,
				SingleTarget=8,
		  }
		  [Flags]
		  public enum TargetProperties
		  {
				None=0,
				Reflecting=1,
				MissileDampening=2,
				Shielding=4,
				Boss=8,
				RareElite=16,
				Unique=32,
				TreasureGoblin=64,
				Stealthable=128,
				Burrowing=256,
				SucideBomber=512,
				LowHealth=1028,
				FullHealth=2048,
				IsSpecial=4096,
		  }

		  //Describes a condition for a single unit
		  public class UnitTargetConditions
		  {
				public UnitTargetConditions(TargetProperties conditionalFlags, int MinimumRadiusDistance=-1, double MinimumHealthPercent=0d)
				{
					 ConditionFlags=conditionalFlags;
					 Distance=MinimumRadiusDistance;
					 HealthPercent=MinimumHealthPercent;
				}

				public TargetProperties ConditionFlags { get; set; }
				public readonly int Distance;
				public readonly double HealthPercent;
		  }






		  public struct Range
		  {
				public float minimum, maximum;
				public Range(float min, float max)
				{
					 minimum=min;
					 maximum=max;
				}

		  }
		  ///<summary>
		  ///Describes how to use the Ability (SNOPower)
		  ///</summary>
		  [Flags]
		  public enum AbilityUseType
		  {
				None=0,
				Buff=1,
				Location=2,
				Target=4,
				ClusterTarget=8,
				ClusterLocation=16,
				ZigZagPathing=32,
				Self=64,
		  }
		  ///<summary>
		  ///Priority assigned to abilities
		  ///</summary>
		  public enum AbilityPriority
		  {
				//None is used for non-cost reusable abilities.
				//Low is used for costing abilities.
				//High is used for buffs.

				None=0,
				Low=1,
				High=2,
		  }

		  ///<summary>
		  ///Conditions used to determine if ability is capable of use.
		  ///</summary>
		  [Flags]
		  public enum AbilityConditions
		  {
				None=0,
				CheckEnergy=1,
				CheckExisitingBuff=2,
				CheckPetCount=4,
				CheckRecastTimer=8,
				CheckCanCast=16,
				CheckPlayerIncapacitated=32,
		  }
		  ///<summary>
		  ///Cached Object that Describes an individual ability.
		  ///</summary>
		  public class Ability
		  {
				//Ability describes the hotbar power.
				//Contains Condtional Methods which are used to determine if the power should be used.
				//	 -Precast
				//	 -Combat Criteria (UnitsInRange/Clusters/SingleTarget)
				//		  *These are either a Tuple Type or Custom Class
				//		  *When set, they create the delegate func that is used to validate the conditions.
				//	 -Final Custom Conditional Check

				//And it contains old Power Class properties and methods

				public Ability()
				{
					 Power=SNOPower.None;
					 Fcriteria=new Func<bool>(() => { return true; });
					 AbilityWaitVars=new Tuple<int, int, bool>(0, 0, USE_SLOWLY);
					 Cooldown=new Zeta.Common.Helpers.WaitTimer(new TimeSpan(0, 0, 0, 0, 0));
					 IsRanged=false;
					 LastConditionPassed=ConditionCriteraTypes.None;
				}

				#region Properties
				private SNOPower power_;
				public SNOPower Power
				{
					 get { return power_; }
					 set
					 {
						  power_=value;
					 }
				}

				public int RuneIndex { get; set; }
				public double Cost { get; set; }
				public bool SecondaryEnergy { get; set; }

				private int range_;
				public int Range
				{
					 get { return range_; }
					 set
					 {
						  range_=value;
						  minimumRange_=value;
					 }
				}

				///<summary>
				///Ability is either projectile or is usable at a further location then melee
				///</summary>
				public bool IsRanged { get; set; }

				public Zeta.Common.Helpers.WaitTimer Cooldown { get; set; }
				private TimeSpan cooldowntimespan_;
				public TimeSpan CooldownTimeSpan
				{
					 get
					 {
						  if (cooldowntimespan_==null)
								cooldowntimespan_=new TimeSpan(0, 0, 0, 0, Bot.Class.AbilityCooldowns[Power]);

						  return cooldowntimespan_;
					 }
				}
				///<summary>
				///Describes variables for use of ability: PreWait Loops, PostWait Loops, Reuseable
				///</summary>
				public Tuple<int, int, bool> AbilityWaitVars { get; set; }

				///<summary>
				///Holds int vaule that describes pet count or buff stacks.
				///</summary>
				public int Counter { get; set; }

				public AbilityPriority Priority { get; set; }

				public AbilityUseType UsageType { get; set; }

				///<summary>
				///This ability is allowed for buffing.
				///</summary>
				public bool UseOOCBuff { get; set; }
				///<summary>
				///This ability is allowed during avoidance movements.
				///</summary>
				public bool UseAvoiding { get; set; }

				public ConditionCriteraTypes LastConditionPassed { get; set; }
				#endregion



				///<summary>
				///Method that is used to determine if current ability precast conditions are valid.
				///</summary>
				internal Func<bool> Fprecast { get; set; }

				private AbilityConditions precastconditions_;
				///<summary>
				///Describes the pre casting conditions - when set it will create the precast method used.
				///</summary>
				public AbilityConditions PreCastConditions
				{
					 get
					 {
						  return precastconditions_;
					 }
					 set
					 {
						  precastconditions_=value;


						  Fprecast=null;
						  if (precastconditions_.HasFlag(AbilityConditions.CheckPlayerIncapacitated))
						  {
								Fprecast+=(new Func<bool>(() => { return PlayerIsIncapacitated(); }));
						  }
						  if (precastconditions_.HasFlag(AbilityConditions.CheckEnergy))
						  {
								if (!this.SecondaryEnergy)
									 Fprecast+=(new Func<bool>(() => { return AbilityEnergyCheck(this.Cost); }));
								else
									 Fprecast+=(new Func<bool>(() => { return AbilityEnergySecondaryCheck(this.Cost); }));
						  }
						  if (precastconditions_.HasFlag(AbilityConditions.CheckCanCast))
						  {
								Fprecast+=(new Func<bool>(() => { return CanCastAbility(this.Power); }));
						  }
						  if (precastconditions_.HasFlag(AbilityConditions.CheckExisitingBuff))
						  {
								Fprecast+=(new Func<bool>(() => { return CheckBuffAbility(this.Power); }));
						  }
						  if (precastconditions_.HasFlag(AbilityConditions.CheckPetCount))
						  {
								Fprecast+=(new Func<bool>(() => { return CheckPetCounter(this.Counter); }));
						  }
						  if (precastconditions_.HasFlag(AbilityConditions.CheckRecastTimer))
						  {
								Fprecast+=(new Func<bool>(() => { return CheckRecastTimer(this.Power); }));
						  }
					 }
				}

				///<summary>
				///Describes values for clustering used for target (Cdistance, DistanceFromBot, MinUnits, IgnoreNonTargetable)
				///</summary>
				public Tuple<double, float, int, bool> ClusterConditions
				{
					 get { return ClusterConditions_; }
					 set
					 {
						  ClusterConditions_=value;
						  FClusterConditions+=new Func<bool>(() => { return CheckClusterConditions(this); });
					 }
				}
				private Tuple<double, float, int, bool> ClusterConditions_;
				private Func<bool> FClusterConditions { get; set; }

				///<summary>
				///Units within Range Conditions
				///</summary>
				public Tuple<RangeIntervals, int> UnitsWithinRangeConditions
				{
					 get { return UnitsWithinRangeConditions_; }
					 set
					 {
						  UnitsWithinRangeConditions_=value;
						  FUnitsInRangeConditions+=new Func<bool>(() => { return Bot.Combat.iAnythingWithinRange[(int)value.Item1]>=value.Item2; });
					 }
				}
				private Tuple<RangeIntervals, int> UnitsWithinRangeConditions_;
				private Func<bool> FUnitsInRangeConditions { get; set; }

				///<summary>
				///Elites within Range Conditions
				///</summary>
				public Tuple<RangeIntervals, int> ElitesWithinRangeConditions
				{
					 get { return ElitesWithinRangeConditions_; }
					 set
					 {
						  ElitesWithinRangeConditions_=value;
						  FElitesInRangeConditions+=new Func<bool>(() => { return Bot.Combat.iElitesWithinRange[(int)value.Item1]>=value.Item2; });
					 }
				}
				private Tuple<RangeIntervals, int> ElitesWithinRangeConditions_;
				private Func<bool> FElitesInRangeConditions { get; set; }


				///<summary>
				///Single Target Conditions
				///</summary>
				public UnitTargetConditions TargetUnitConditionFlags
				{
					 get { return TargetUnitConditionFlags_; }
					 set
					 {
						  TargetUnitConditionFlags_=value;

						  if (value.Distance>-1)
								FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.RadiusDistance<=value.Distance; });

						  if (value.HealthPercent>0d)
								FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.CurrentHealthPct.Value<=value.HealthPercent; });


						  if (value.ConditionFlags.HasFlag(TargetProperties.Boss))
								FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsBoss; });
						  if (value.ConditionFlags.HasFlag(TargetProperties.Burrowing))
								FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsBurrowableUnit; });
						  if (value.ConditionFlags.HasFlag(TargetProperties.FullHealth))
								FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.CurrentHealthPct.Value==1d; });
						  if (value.ConditionFlags.HasFlag(TargetProperties.IsSpecial))
								FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.ObjectIsSpecial; });
						  if (value.ConditionFlags.HasFlag(TargetProperties.LowHealth))
								FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.CurrentHealthPct.Value<0.25d; });
						  if (value.ConditionFlags.HasFlag(TargetProperties.MissileDampening))
								FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.MonsterMissileDampening; });
						  if (value.ConditionFlags.HasFlag(TargetProperties.RareElite))
								FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.IsEliteRareUnique; });
						  if (value.ConditionFlags.HasFlag(TargetProperties.Reflecting))
								FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsMissileReflecting; });
						  if (value.ConditionFlags.HasFlag(TargetProperties.Shielding))
								FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.MonsterShielding; });
						  if (value.ConditionFlags.HasFlag(TargetProperties.Stealthable))
								FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsStealthableUnit; });
						  if (value.ConditionFlags.HasFlag(TargetProperties.SucideBomber))
								FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsSucideBomber; });
						  if (value.ConditionFlags.HasFlag(TargetProperties.TreasureGoblin))
								FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsTreasureGoblin; });
						  if (value.ConditionFlags.HasFlag(TargetProperties.Unique))
								FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.MonsterUnique; });
					 }
				}
				private UnitTargetConditions TargetUnitConditionFlags_;
				private Func<bool> FSingleTargetUnitCriteria { get; set; }

				///<summary>
				///Custom Conditions for Combat
				///</summary>
				public Func<bool> Fcriteria { get; set; }


				#region Condition Static Methods
				public static bool PlayerIsIncapacitated()
				{
					 return !Bot.Character.bIsIncapacitated;
				}
				public static bool AbilityEnergyCheck(double Cost)
				{
					 bool EnergyTest=Bot.Character.dCurrentEnergy>=Cost;
					 return EnergyTest;
				}
				public static bool AbilityEnergySecondaryCheck(double Cost)
				{
					 bool EnergyTest=Bot.Character.dDiscipline>=Cost;
					 return EnergyTest;
				}
				public static bool CanCastAbility(SNOPower P)
				{
					 return Zeta.CommonBot.PowerManager.CanCast(P);
				}
				public static bool CheckBuffAbility(SNOPower P)
				{
					 return !Bot.Class.HasBuff(P);
				}
				public static bool CheckPetCounter(int C)
				{
					 return Bot.Class.MainPetCount<C;
				}
				public static bool CheckRecastTimer(SNOPower P)
				{
					 return Bot.Class.AbilityUseTimer(P);
				}
				public static bool CheckClusterConditions(Ability A)
				{
					 return Clusters(A.ClusterConditions.Item1, A.ClusterConditions.Item2, A.ClusterConditions.Item3, A.ClusterConditions.Item4).Count>0;

				}
				#endregion



				///<summary>
				///Check Ability is valid to use.
				///</summary>
				public bool CheckPreCastConditionMethod()
				{
					 foreach (Func<bool> item in this.Fprecast.GetInvocationList())
					 {
						  if (!item()) return false;
					 }

					 //Reset Last Condition
					 LastConditionPassed=ConditionCriteraTypes.None;
					 return true;
				}
				///<summary>
				///Check Combat
				///</summary>
				public bool CheckCombatConditionMethod()
				{
					 //Order in which tests are conducted..

					 //Units in Range (Not Cluster)
					 //Clusters
					 //Single Target

					 //If all are null or any of them are successful, then we test Custom Conditions
					 //Custom Condition


					 bool TestCustomConditions=false;

					 bool FailedCondition=false;
					 if (ElitesWithinRangeConditions!=null)
					 {
						  foreach (Func<bool> item in this.FElitesInRangeConditions.GetInvocationList())
						  {
								if (!item())
								{
									 FailedCondition=true;
									 break;
								}
						  }
						  if (!FailedCondition)
						  {
								TestCustomConditions=true;
								LastConditionPassed=ConditionCriteraTypes.ElitesInRange;
						  }
					 }
					 if ((!TestCustomConditions||FailedCondition)&&UnitsWithinRangeConditions!=null)
					 {
						  FailedCondition=false;
						  foreach (Func<bool> item in this.FUnitsInRangeConditions.GetInvocationList())
						  {
								if (!item())
								{
									 FailedCondition=true;
									 break;
								}
						  }
						  if (!FailedCondition)
						  {
								LastConditionPassed=ConditionCriteraTypes.UnitsInRange;
								TestCustomConditions=true;
						  }
					 }
					 if ((!TestCustomConditions||FailedCondition)&&ClusterConditions!=null)
					 {
						  FailedCondition=false;
						  foreach (Func<bool> item in this.FClusterConditions.GetInvocationList())
						  {
								if (!item())
								{
									 FailedCondition=true;
									 break;
								}
						  }
						  if (!FailedCondition)
						  {
								LastConditionPassed=ConditionCriteraTypes.Cluster;
								TestCustomConditions=true;
						  }
					 }
					 if ((!TestCustomConditions||FailedCondition)&&TargetUnitConditionFlags!=null)
					 {
						  FailedCondition=false;
						  foreach (Func<bool> item in this.FSingleTargetUnitCriteria.GetInvocationList())
						  {
								if (!item())
								{
									 FailedCondition=true;
									 break;
								}
						  }
						  if (!FailedCondition)
						  {
								LastConditionPassed=ConditionCriteraTypes.SingleTarget;
								TestCustomConditions=true;
						  }
					 }

					 //Tested Conditions but Ended Up With Failed Attempt.
					 if (TestCustomConditions&&FailedCondition) return false;


					 foreach (Func<bool> item in this.Fcriteria.GetInvocationList())
						  if (!item()) return false;
						  

					 return true;
				}




				#region UseAbilityVars

				private float minimumRange_;
				public float MinimumRange
				{
					 get { return minimumRange_; }
					 set { minimumRange_=value; }
				}

				private Vector3 TargetPosition_;
				public Vector3 TargetPosition
				{
					 get { return TargetPosition_; }
					 set { TargetPosition_=value; }
				}

				public int WorldID
				{
					 get { return Bot.Character.iCurrentWorldID; }
				}

				private int TargetRAGUID_;
				public int TargetRAGUID
				{
					 get { return TargetRAGUID_; }
					 set { TargetRAGUID_=value; }
				}

				private int WaitLoopsBefore_;
				public int WaitLoopsBefore
				{
					 get { return WaitLoopsBefore_; }
					 set { WaitLoopsBefore_=value; }
				}

				private int WaitLoopsAfter_;
				public int WaitLoopsAfter
				{
					 get { return WaitLoopsAfter_; }
					 set { WaitLoopsAfter_=value; }
				}

				public bool WaitWhileAnimating
				{
					 get { return AbilityWaitVars.Item3; }
				}
				private bool? SuccessUsed_;
				public bool? SuccessUsed
				{
					 get { return SuccessUsed_; }
					 set { SuccessUsed_=value; }
				}

				//Resets Properties
				internal void SetupAbilityForUse()
				{
					 MinimumRange=Range;
					 TargetPosition_=vNullLocation;
					 TargetRAGUID_=-1;
					 WaitLoopsBefore_=this.AbilityWaitVars.Item1;
					 WaitLoopsAfter_=this.AbilityWaitVars.Item2;

					 CanCastFlags=Zeta.CommonBot.PowerManager.CanCastFlags.None;
					 SuccessUsed_=null;

					 //Check Clustering First.. we verify that cluster condition was last to be tested.
					 if (this.UsageType.HasFlag(AbilityUseType.ClusterTarget)&&this.LastConditionPassed==ConditionCriteraTypes.Cluster) //Cluster ACDGUID
					 {
						  TargetRAGUID_=Clusters(this.ClusterConditions.Item1, this.ClusterConditions.Item2, this.ClusterConditions.Item3, this.ClusterConditions.Item4)[0].ListUnits[0].AcdGuid.Value;
						  return;
					 }
					 if (this.UsageType.HasFlag(AbilityUseType.ClusterLocation)&&this.LastConditionPassed==ConditionCriteraTypes.Cluster) //Cluster Target Position
					 {
						  TargetPosition_=Clusters(this.ClusterConditions.Item1, this.ClusterConditions.Item2, this.ClusterConditions.Item3, this.ClusterConditions.Item4)[0].ListUnits.First(u => u.ObjectIsValidForTargeting).Position;
						  return;
					 }

					 if (this.UsageType.HasFlag(AbilityUseType.Location)) //Current Target Position
						  TargetPosition_=Bot.Target.CurrentTarget.Position;
					 else if (this.UsageType.HasFlag(AbilityUseType.Self)) //Current Bot Position
						  TargetPosition_=Bot.Character.Position;
					 else if (this.UsageType.HasFlag(AbilityUseType.ZigZagPathing)) //Zig-Zag Pathing
					 {
						  Bot.Combat.vPositionLastZigZagCheck=Bot.Character.Position;
						  if (Bot.Class.ShouldGenerateNewZigZagPath())
								Bot.Class.GenerateNewZigZagPath();

						  TargetPosition_=Bot.Combat.vSideToSideTarget;
					 }
					 else if (this.UsageType.HasFlag(AbilityUseType.Target)) //Current Target ACDGUID
						  TargetRAGUID_=Bot.Target.CurrentTarget.AcdGuid.Value;
				}

				public Zeta.CommonBot.PowerManager.CanCastFlags CanCastFlags;
				#endregion


				public void UsePower()
				{
					 Zeta.CommonBot.PowerManager.CanCast(this.Power, out CanCastFlags);
					 SuccessUsed_=ZetaDia.Me.UsePower(this.Power, this.TargetPosition_, this.WorldID, this.TargetRAGUID_);
				}

				///<summary>
				///Sets values related to ability usage
				///</summary>
				public void SuccessfullyUsed()
				{
					 dictAbilityLastUse[Power]=DateTime.Now;
					 lastGlobalCooldownUse=DateTime.Now;
					 Cooldown=new Zeta.Common.Helpers.WaitTimer(CooldownTimeSpan);
				}


				public float CurrentBotRange
				{
					 get
					 {
						  //If Vector is null then we check if acdguid matches current target
						  if (TargetPosition_==vNullLocation)
						  {
								if (TargetRAGUID_!=-1&&Bot.Target.CurrentTarget.AcdGuid.HasValue&&TargetRAGUID_==Bot.Target.CurrentTarget.AcdGuid.Value)
								{
									 return Bot.Target.CurrentTarget.RadiusDistance;
								}
								return Range;
						  }
						  else
						  {
								return Bot.Character.Position.Distance(TargetPosition_);
						  }
					 }
				}
				public Vector3 DestinationVector
				{
					 get
					 {
						  if (TargetPosition_==vNullLocation)
						  {
								if (TargetRAGUID_!=-1&&Bot.Target.CurrentTarget.AcdGuid.HasValue&&TargetRAGUID_==Bot.Target.CurrentTarget.AcdGuid.Value)
								{
									 return Bot.Target.CurrentTarget.Position;
								}
								return vNullLocation;
						  }
						  else
								return TargetPosition_;
					 }
				}



				public override int GetHashCode()
				{
					 return (int)this.Power;
				}
				public override bool Equals(object obj)
				{
					 //Check for null and compare run-time types. 
					 if (obj==null||this.GetType()!=obj.GetType())
					 {
						  return false;
					 }
					 else
					 {
						  Ability p=(Ability)obj;
						  return this.Power==p.Power;
					 }
				}

		  }

		  public static readonly Ability Instant_Melee_Attack=new Ability
		  {
				Range=8,
				Power=SNOPower.Weapon_Melee_Instant,
				UsageType=AbilityUseType.Target,
				AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
		  };
		  public static readonly Ability Instant_Range_Attack=new Ability
		  {
				Range=25,
				Power=SNOPower.Weapon_Ranged_Instant,
				UsageType=AbilityUseType.Target,
				AbilityWaitVars=new Tuple<int, int, bool>(0, 0, true),
		  };
	 }
}