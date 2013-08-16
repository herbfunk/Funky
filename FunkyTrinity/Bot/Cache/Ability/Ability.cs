using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;
using System.Collections.Generic;
using FunkyTrinity.Enums;
using Zeta.Internals.SNO;
using FunkyTrinity.Cache;
using FunkyTrinity.Movement;

namespace FunkyTrinity.ability
{
	 [Flags]
	 public enum AbilityUseage
	 {
		  Anywhere=1,
		  OutOfCombat=2,
		  Combat=4,
	 }

	 public abstract class AbilityCriteria
	 {
		  public AbilityCriteria() 
		  {
				LastConditionPassed=ConditionCriteraTypes.None;
		  }

		  public ConditionCriteraTypes LastConditionPassed
		  {
				get { return lastConditionPassed; }
				set { lastConditionPassed=value; }
		  }
		  private ConditionCriteraTypes lastConditionPassed=ConditionCriteraTypes.None;

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

				}
		  }
		  private AbilityConditions precastconditions_= AbilityConditions.None;

		  ///<summary>
		  ///Describes values for clustering used for target (Cdistance, DistanceFromBot, MinUnits, IgnoreNonTargetable)
		  ///</summary>
		  ///<value>
		  ///Clustering Distance, Distance From Bot, Minimum Unit Count, Ignore Non-Targetables
		  ///</value>
		  public ClusterConditions ClusterConditions
		  {
				get { return ClusterConditions_; }
				set
				{
					 ClusterConditions_=value;
				}
		  }
		  private ClusterConditions ClusterConditions_=null;
		  ///<summary>
		  ///Units within Range Conditions
		  ///</summary>
		  public Tuple<RangeIntervals, int> UnitsWithinRangeConditions
		  {
				get { return UnitsWithinRangeConditions_; }
				set
				{
					 UnitsWithinRangeConditions_=value;
				}
		  }
		  private Tuple<RangeIntervals, int> UnitsWithinRangeConditions_=null;


		  public Tuple<RangeIntervals, int> ElitesWithinRangeConditions
		  {
				get { return elitesWithinRangeConditions; }
				set { elitesWithinRangeConditions=value; }
		  }
		  private Tuple<RangeIntervals, int> elitesWithinRangeConditions=null;

		  ///<summary>
		  ///Single Target Conditions -- Should only used if ability is offensive!
		  ///</summary>
		  public UnitTargetConditions TargetUnitConditionFlags
		  {
				get { return targetUnitConditionFlags; }
				set { targetUnitConditionFlags=value; }
		  }
		  private UnitTargetConditions targetUnitConditionFlags=null;



		  public static void CreateClusterConditions(ref Func<bool> FClusterConditions, Ability ability)
		  {
				FClusterConditions=null;
				if (ability.ClusterConditions==null) return;

				FClusterConditions=new Func<bool>(() => { return Ability.CheckClusterConditions(ability.ClusterConditions); });
		  }
		  public static void CreatePreCastConditions(ref Func<bool> Fprecast, Ability ability)
		  {
				Fprecast=null;
				if (ability.PreCastConditions==null) return;
				AbilityConditions precastconditions_=ability.PreCastConditions;

				if (precastconditions_.Equals(AbilityConditions.None))
					 Fprecast+=(new Func<bool>(() => { return true; }));
				else
				{
					 if (precastconditions_.HasFlag(AbilityConditions.CheckPlayerIncapacitated))
						  Fprecast+=(new Func<bool>(() => { return !Bot.Character.bIsIncapacitated; }));

					 if (precastconditions_.HasFlag(AbilityConditions.CheckPlayerRooted))
						  Fprecast+=(new Func<bool>(() => { return !Bot.Character.bIsRooted; }));

					 if (precastconditions_.HasFlag(AbilityConditions.CheckExisitingBuff))
						  Fprecast+=(new Func<bool>(() => { return !Bot.Class.HasBuff(ability.Power); }));

					 if (precastconditions_.HasFlag(AbilityConditions.CheckPetCount))
						  Fprecast+=(new Func<bool>(() => { return Bot.Class.MainPetCount<ability.Counter; }));

					 if (precastconditions_.HasFlag(AbilityConditions.CheckRecastTimer))
						  Fprecast+=(new Func<bool>(() => { return ability.LastUsedMilliseconds>ability.Cooldown; }));

					 if (precastconditions_.HasFlag(AbilityConditions.CheckCanCast))
					 {
						  Fprecast+=(new Func<bool>(() =>
						  {
								bool cancast=PowerManager.CanCast(ability.Power, out ability.CanCastFlags);

								//Special Ability -- Trigger Waiting For Special When Not Enough Resource to Cast.
								if (ability.IsSpecialAbility)
								{
									 if (!cancast&&ability.CanCastFlags.HasFlag(PowerManager.CanCastFlags.PowerNotEnoughResource))
										  Bot.Class.bWaitingForSpecial=true;
									 else
										  Bot.Class.bWaitingForSpecial=false;
								}

								return cancast;
						  }));
					 }

					 if (precastconditions_.HasFlag(AbilityConditions.CheckEnergy))
					 {
						  if (!ability.SecondaryEnergy)
								Fprecast+=(new Func<bool>(() =>
								{
									 bool energyCheck=Bot.Character.dCurrentEnergy>=ability.Cost;
									 if (ability.IsSpecialAbility) //we trigger waiting for special here.
										  Bot.Class.bWaitingForSpecial=!energyCheck;
									 return energyCheck;
								}));
						  else
								Fprecast+=(new Func<bool>(() =>
								{
									 bool energyCheck=Bot.Character.dDiscipline>=ability.Cost;
									 if (ability.IsSpecialAbility) //we trigger waiting for special here.
										  Bot.Class.bWaitingForSpecial=!energyCheck;
									 return energyCheck;
								}));
					 }
				}

		  }
		  public static void CreateTargetConditions(ref Func<bool> FSingleTargetUnitCriteria, Ability ability)
		  {
				
				FSingleTargetUnitCriteria=null;

				if (ability.TargetUnitConditionFlags==null) return;
				UnitTargetConditions TargetUnitConditionFlags_=ability.TargetUnitConditionFlags;

				if (TargetUnitConditionFlags_.Distance>-1)
					 FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.RadiusDistance<=TargetUnitConditionFlags_.Distance; });
				if (TargetUnitConditionFlags_.HealthPercent>0d)
					 FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.CurrentHealthPct.Value<=TargetUnitConditionFlags_.HealthPercent; });

				//TRUE CONDITIONS
				if (TargetUnitConditionFlags_.TrueConditionFlags.Equals(TargetProperties.None))
					 FSingleTargetUnitCriteria+=new Func<bool>(() => { return true; });
				else
				{
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Boss))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsBoss; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Burrowing))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsBurrowableUnit; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.FullHealth))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.CurrentHealthPct.Value==1d; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.IsSpecial))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.ObjectIsSpecial; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Weak))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.UnitMaxHitPointAverageWeight<0; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.MissileDampening))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.MonsterMissileDampening; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.RareElite))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.IsEliteRareUnique; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.MissileReflecting))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsMissileReflecting; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Shielding))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.MonsterShielding; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Stealthable))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsStealthableUnit; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.SucideBomber))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsSucideBomber; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.TreasureGoblin))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsTreasureGoblin; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Unique))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.MonsterUnique; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Ranged))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.Monstersize.Value==MonsterSize.Ranged; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.TargetableAndAttackable))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.IsTargetableAndAttackable; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Fast))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.IsFast; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.DOTDPS))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.HasDOTdps.HasValue&&Bot.Target.CurrentUnitTarget.HasDOTdps.Value; });
				}

				//FALSE CONDITIONS
				if (TargetUnitConditionFlags_.FalseConditionFlags.Equals(TargetProperties.None))
					 FSingleTargetUnitCriteria+=new Func<bool>(() => { return true; });
				else
				{
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Boss))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsBoss; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Burrowing))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsBurrowableUnit; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.FullHealth))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.CurrentHealthPct.Value!=1d; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.IsSpecial))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.ObjectIsSpecial; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Weak))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.UnitMaxHitPointAverageWeight>0; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.MissileDampening))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.MonsterMissileDampening; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.RareElite))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.IsEliteRareUnique; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.MissileReflecting))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsMissileReflecting; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Shielding))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.MonsterShielding; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Stealthable))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsStealthableUnit; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.SucideBomber))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsSucideBomber; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.TreasureGoblin))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsTreasureGoblin; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Unique))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.MonsterUnique; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Ranged))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.Monstersize.Value!=MonsterSize.Ranged; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.TargetableAndAttackable))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.IsTargetableAndAttackable; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Fast))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.IsFast; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.DOTDPS))
						  FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.HasDOTdps.HasValue||!Bot.Target.CurrentUnitTarget.HasDOTdps.Value; });
				}
		  }
		  public static void CreateUnitsInRangeConditions(ref Func<bool> FUnitRange, Ability ability)
		  {
				FUnitRange=null;
				if (ability.UnitsWithinRangeConditions!=null)
					 FUnitRange+=new Func<bool>(() => { return Bot.Combat.iAnythingWithinRange[(int)ability.UnitsWithinRangeConditions.Item1]>=ability.UnitsWithinRangeConditions.Item2; });
		  }
		  public static void CreateElitesInRangeConditions(ref Func<bool> FUnitRange, Ability ability)
		  {
				FUnitRange=null;
				if (ability.ElitesWithinRangeConditions!=null)
					 FUnitRange+=new Func<bool>(() => { return Bot.Combat.iElitesWithinRange[(int)ability.ElitesWithinRangeConditions.Item1]>=ability.ElitesWithinRangeConditions.Item2; });
		  }
	 }

	 ///<summary>
	 ///Cached Object that Describes an individual ability.
	 ///</summary>
	 public partial class Ability : AbilityCriteria, IAbility
	 {

		  //Conditional Methods which are used to determine if the power should be used.
		  //	 -Precast Conditions
		  //	 -Combat Criteria
		  //		  *These are either a Tuple Type or Custom Class
		  //		  *When set, they create the delegate func that is used to validate the conditions.
		  //	 -Final Custom Conditional Check



		  public Ability()
				: base()
		  {

				Fcriteria=new Func<bool>(() => { return true; });
				Fbuff=new Func<bool>(() => { return false; });

				WaitVars=new WaitLoops(0, 0, true);
				IsRanged=false;
				UseageType=AbilityUseage.Anywhere;
				ExecutionType=AbilityUseType.None;
				
				IsSpecialAbility=false;
				Range=0;
				Priority=AbilityPriority.None;


				Initialize();
				InitCriteria();
		  }
		  protected virtual void Initialize()
		  {

		  }
		  protected void InitCriteria()
		  {
				AbilityCriteria.CreatePreCastConditions(ref Fprecast, this);
				AbilityCriteria.CreateTargetConditions(ref FSingleTargetUnitCriteria, this);
				AbilityCriteria.CreateUnitsInRangeConditions(ref FUnitsInRangeConditions, this);
				AbilityCriteria.CreateElitesInRangeConditions(ref FElitesInRangeConditions, this);
				AbilityCriteria.CreateClusterConditions(ref FClusterConditions, this);
		  }

		  #region Properties
		  public AbilityPriority Priority { get; set; }
		  ///<summary>
		  ///Describes variables for use of ability: PreWait Loops, PostWait Loops, Reuseable
		  ///</summary>
		  public WaitLoops WaitVars { get; set; }
		  public int Range { get; set; }
		  public double Cost { get; set; }
		  public bool SecondaryEnergy { get; set; }
		  ///<summary>
		  ///This is used to determine how the ability will be used
		  ///</summary>
		  public AbilityUseType ExecutionType { get; set; }

		  private AbilityUseage useageType;
		  public AbilityUseage UseageType
		  {
				get { return useageType; }
				set { useageType=value; if (value.HasFlag(AbilityUseage.OutOfCombat|AbilityUseage.Anywhere)) Fbuff=new Func<bool>(() => { return true; }); }
		  }

		  public virtual int RuneIndex { get { return -1; } }
		  internal bool IsADestructiblePower { get { return PowerCacheLookup.AbilitiesDestructiblePriority.Contains(this.Power); } }
		  internal bool IsASpecialMovementPower { get { return PowerCacheLookup.SpecialMovementAbilities.Contains(this.Power); } }

		  ///<summary>
		  ///Ability will trigger WaitingForSpecial if Energy Check fails.
		  ///</summary>
		  public bool IsSpecialAbility { get; set; }

		  private bool isNavigationSpecial=false;
		  public bool IsNavigationSpecial
		  {
				get { return isNavigationSpecial; }
				set { isNavigationSpecial=value; }
		  }



		  ///<summary>
		  ///Ability is either projectile or is usable at a further location then melee
		  ///</summary>
		  public bool IsRanged { get; set; }

		  internal DateTime LastUsed
		  {
				get
				{
					 return PowerCacheLookup.dictAbilityLastUse[this.Power];
				}
				set
				{
					 PowerCacheLookup.dictAbilityLastUse[this.Power]=value;
				}
		  }
		  internal double LastUsedMilliseconds
		  {
				get { return DateTime.Now.Subtract(LastUsed).TotalMilliseconds; }
		  }

		  internal double Cooldown
		  {
				get { return Bot.Class.AbilityCooldowns[this.Power]; }
		  }


		  ///<summary>
		  ///Holds int value that describes pet count or buff stacks.
		  ///</summary>
		  public int Counter { get; set; }


		  #endregion


		  ///<summary>
		  ///Method that is used to determine if current ability precast conditions are valid.
		  ///</summary>
		  private Func<bool> Fprecast;
		  private Func<bool> FClusterConditions;
		  private Func<bool> FUnitsInRangeConditions;
		  private Func<bool> FElitesInRangeConditions;
		  private Func<bool> FSingleTargetUnitCriteria;
		  ///<summary>
		  ///Custom Conditions for Combat
		  ///</summary>
		  protected Func<bool> Fcriteria;
		  ///<summary>
		  ///Custom Conditions for Buffing
		  ///</summary>
		  protected Func<bool> Fbuff;


		  ///<summary>
		  ///Check Ability Buff Conditions
		  ///</summary>
		  public bool CheckBuffConditionMethod()
		  {
				foreach (Func<bool> item in Fbuff.GetInvocationList())
				{
					 if (!item()) return false;
				}

				return true;
		  }
		  ///<summary>
		  ///Check Ability is valid to use.
		  ///</summary>
		  public bool CheckPreCastConditionMethod()
		  {
				foreach (Func<bool> item in Fprecast.GetInvocationList())
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

					 if (!this.FClusterConditions.Invoke())
					 {
						  FailedCondition=true;
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

				//If TestCustomCondtion failed, and FailedCondition is true.. then we tested a combat condition.
				//If FailedCondition is false, then we never tested a condition.
				if (!TestCustomConditions&&FailedCondition) return false;


				foreach (Func<bool> item in this.Fcriteria.GetInvocationList())
					 if (!item()) return false;


				return true;
		  }


		  internal static bool CheckClusterConditions(ClusterConditions CC)
		  {
				return Bot.Combat.Clusters(CC).Count>0;
		  }



		  #region UseAbilityVars

		  private float minimumRange_;
		  internal float MinimumRange
		  {
				get { return minimumRange_; }
				set { minimumRange_=value; }
		  }

		  private Vector3 TargetPosition_;
		  internal Vector3 TargetPosition
		  {
				get { return TargetPosition_; }
				set { TargetPosition_=value; }
		  }

		  internal int WorldID
		  {
				get { return Bot.Character.iCurrentWorldID; }
		  }

		  private int TargetRAGUID_;
		  internal int TargetRAGUID
		  {
				get { return TargetRAGUID_; }
				set { TargetRAGUID_=value; }
		  }

		  private int WaitLoopsBefore_;
		  internal int WaitLoopsBefore
		  {
				get { return WaitLoopsBefore_; }
				set { WaitLoopsBefore_=value; }
		  }

		  private int WaitLoopsAfter_;
		  internal int WaitLoopsAfter
		  {
				get { return WaitLoopsAfter_; }
				set { WaitLoopsAfter_=value; }
		  }

		  internal bool WaitWhileAnimating
		  {
				get { return WaitVars.Reusable; }
		  }
		  private bool? SuccessUsed_;
		  internal bool? SuccessUsed
		  {
				get { return SuccessUsed_; }
				set { SuccessUsed_=value; }
		  }

		  internal PowerManager.CanCastFlags CanCastFlags;
		  #endregion


		  public static void UsePower(ref Ability ability)
		  {
				if (!ability.ExecutionType.HasFlag(AbilityUseType.RemoveBuff))
				{
					 ability.SuccessUsed=ZetaDia.Me.UsePower(ability.Power, ability.TargetPosition, ability.WorldID, ability.TargetRAGUID);
				}
				else
				{
					 ZetaDia.Me.GetBuff(ability.Power).Cancel();
					 ability.SuccessUsed=true;
				}
		  }

		  ///<summary>
		  ///Sets values related to ability usage
		  ///</summary>
		  public void SuccessfullyUsed()
		  {
				this.LastUsed=DateTime.Now;
				PowerCacheLookup.lastGlobalCooldownUse=DateTime.Now;
				//Reset Blockcounter --
				TargetMovement.BlockedMovementCounter=0;
		  }

		  public static void SetupAbilityForUse(ref Ability ability)
		  {
				ability.MinimumRange=ability.Range;
				ability.TargetPosition_=Vector3.Zero;
				ability.TargetRAGUID_=-1;
				ability.WaitLoopsBefore_=ability.WaitVars.PreLoops;
				ability.WaitLoopsAfter_=ability.WaitVars.PostLoops;

				ability.CanCastFlags=PowerManager.CanCastFlags.None;
				ability.SuccessUsed_=null;

				//Cluster Target -- Aims for Centeroid Unit
				if (ability.ExecutionType.HasFlag(AbilityUseType.ClusterTarget)&&CheckClusterConditions(ability.ClusterConditions)) //Cluster ACDGUID
				{
					 ability.TargetRAGUID=Bot.Combat.Clusters(ability.ClusterConditions)[0].GetNearestUnitToCenteroid().AcdGuid.Value;
					 return;
				}
				//Cluster Location -- Aims for Center of Cluster
				if (ability.ExecutionType.HasFlag(AbilityUseType.ClusterLocation)&&CheckClusterConditions(ability.ClusterConditions)) //Cluster Target Position
				{
					 ability.TargetPosition=(Vector3)Bot.Combat.Clusters(ability.ClusterConditions)[0].Midpoint;
					 return;
				}
				//Cluster Target Nearest -- Gets nearest unit in cluster as target.
				if (ability.ExecutionType.HasFlag(AbilityUseType.ClusterTargetNearest)&&CheckClusterConditions(ability.ClusterConditions)) //Cluster Target Position
				{
					 ability.TargetRAGUID=Bot.Combat.Clusters(ability.ClusterConditions)[0].ListUnits[0].AcdGuid.Value;
					 return;
				}

				if (ability.ExecutionType.HasFlag(AbilityUseType.Location)) //Current Target Position
					 ability.TargetPosition=Bot.Target.CurrentTarget.Position;
				else if (ability.ExecutionType.HasFlag(AbilityUseType.Self)) //Current Bot Position
					 ability.TargetPosition=Bot.Character.Position;
				else if (ability.ExecutionType.HasFlag(AbilityUseType.ZigZagPathing)) //Zig-Zag Pathing
				{
					 Bot.Combat.vPositionLastZigZagCheck=Bot.Character.Position;
					 if (Bot.Class.ShouldGenerateNewZigZagPath())
						  Bot.Class.GenerateNewZigZagPath();

					 ability.TargetPosition=Bot.Combat.vSideToSideTarget;
				}
				else if (ability.ExecutionType.HasFlag(AbilityUseType.Target)) //Current Target ACDGUID
					 ability.TargetRAGUID=Bot.Target.CurrentTarget.AcdGuid.Value;
		  }

		  ///<summary>
		  ///Returns an estimated destination using the minimum range and distance from the radius of target.
		  ///</summary>
		  internal Vector3 DestinationVector
		  {
				get
				{
					 Vector3 DestinationV=Vector3.Zero;
					 if (TargetPosition_==Vector3.Zero)
					 {
						  if (TargetRAGUID_!=-1&&Bot.Target.CurrentTarget.AcdGuid.HasValue&&TargetRAGUID_==Bot.Target.CurrentTarget.AcdGuid.Value)
								DestinationV=Bot.Target.CurrentTarget.BotMeleeVector;
						  else
								return Vector3.Zero;
					 }
					 else
						  DestinationV=TargetPosition_;


					 if (this.IsRanged)
					 {
						  float DistanceFromTarget=Vector3.Distance(Bot.Character.Position, DestinationV);
						  if (this.MinimumRange>DistanceFromTarget)
						  {
								float RangeNeeded=Math.Max(0f, (this.MinimumRange-DistanceFromTarget));
								return MathEx.GetPointAt(Bot.Character.Position, RangeNeeded, Navigation.FindDirection(Bot.Character.Position, DestinationV, true));
						  }
						  else
								return Bot.Character.Position;
					 }
					 else
						  return Bot.Target.CurrentTarget.BotMeleeVector;
				}
		  }



		  public virtual int GetHashCode()
		  {
				return (int)this.Power;
		  }
		  public virtual bool Equals(object obj)
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

		  public string DebugString()
		  {
				return String.Format("Ability: {0} [RuneIndex={1}] \r\n"+
										  "Range={2} ReuseMS={3} Priority [{4}] UseType [{5}] \r\n"+
										  "Usage {6} \r\n"+
										  "Last Condition {7} -- Last Used {8}",
																		this.Power.ToString(), this.RuneIndex.ToString(),
																		this.Range.ToString(), this.Cooldown.ToString(), this.Priority.ToString(), this.ExecutionType.ToString(),
																		this.UseageType.ToString(),
																		this.LastConditionPassed.ToString(), this.LastUsedMilliseconds<100000?this.LastUsedMilliseconds.ToString()+"ms":"Never");
		  }





		  #region IAbility Members

		  public virtual SNOPower Power
		  {
				get { return SNOPower.None; }
		  }
		  #endregion
	 }

}