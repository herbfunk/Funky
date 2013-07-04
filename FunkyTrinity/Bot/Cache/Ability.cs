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
				public Ability()
				{
					 Power=SNOPower.None;
					 Fcriteria=new Func<bool>(() => { return true; });
					 AbilityWaitVars=new Tuple<int, int, bool>(0, 0, USE_SLOWLY);
					 Cooldown=new Zeta.Common.Helpers.WaitTimer(new TimeSpan(0, 0, 0, 0, 0));
				}
				public Ability(Ability A)
				{
					 this.Power=A.Power;
					 this.UsageType=A.UsageType;
					 this.Range=A.Range;
					 this.AbilityWaitVars=A.AbilityWaitVars;
				}

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
				public Tuple<double, float, int, bool> ClusterConditions { get; set; }

				///<summary>
				///Method that is used to determine if current combat conditions are valid.
				///</summary>
				public Func<bool> Fcriteria { get; set; }

				#region Precast Static Methods
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
				#endregion

				public static bool CheckClusterConditions(Ability A)
				{
					 return Clusters(A.ClusterConditions.Item1, A.ClusterConditions.Item2, A.ClusterConditions.Item3, A.ClusterConditions.Item4).Count>0;
						   
				}

				///<summary>
				///Check Ability is valid to use.
				///</summary>
				public bool CheckConditionMethod()
				{
					 //Logging.WriteVerbose("Testing Ability Condition {0}", this.Power.ToString());

					 foreach (Func<bool> item in this.Fprecast.GetInvocationList())
					 {
						  if (!item())
						  {
								//Logging.WriteVerbose("Ability Condition {0} has failed", this.Power.ToString());
								return false;
						  }
					 }

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

				internal void SetupAbilityForUse()
				{
					 MinimumRange=Range;
					 TargetPosition_=vNullLocation;
					 TargetRAGUID_=-1;
					 WaitLoopsBefore_=this.AbilityWaitVars.Item1;
					 WaitLoopsAfter_=this.AbilityWaitVars.Item2;

					 CanCastFlags=Zeta.CommonBot.PowerManager.CanCastFlags.None;
					 SuccessUsed_=null;

					 //Check Clustering First.. we verify that clusters exists.
					 if (this.UsageType.HasFlag(AbilityUseType.ClusterTarget)&&Ability.CheckClusterConditions(this)) //Cluster ACDGUID
					 {
						  TargetRAGUID_=Clusters(this.ClusterConditions.Item1, this.ClusterConditions.Item2, this.ClusterConditions.Item3, this.ClusterConditions.Item4)[0].ListUnits[0].AcdGuid.Value;
						  return;
					 }
					 if (this.UsageType.HasFlag(AbilityUseType.ClusterLocation)&&Ability.CheckClusterConditions(this)) //Cluster Target Position
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
				Power= SNOPower.Weapon_Melee_Instant,
				UsageType= AbilityUseType.Target,
				AbilityWaitVars=new Tuple<int,int,bool>(0,0,true),
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