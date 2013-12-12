using System;
using System.Linq;
using FunkyBot.Cache;
using FunkyBot.Cache.Objects;
using FunkyBot.Movement;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;
using FunkyBot.Movement.Clustering;

namespace FunkyBot.Player.HotBar.Skills
{


	 ///<summary>
	 ///Cached Object that Describes an individual Ability.
	 ///</summary>
	 public class Skill : SkillCriteria
	 {
		 public Skill()
		  {
				WaitVars=new WaitLoops(0, 0, true);
				IsRanged=false;
				IsProjectile=false;
				UseageType=AbilityUseage.Anywhere;
				ExecutionType=AbilityExecuteFlags.None;
				IsSpecialAbility=false;
				IsChanneling=false;
                IsCombat = false;
				Range=0;
				Priority=AbilityPriority.None;
				LastUsed=DateTime.Today;
				IsADestructiblePower=PowerCacheLookup.AbilitiesDestructiblePriority.Contains(Power);
				IsASpecialMovementPower=PowerCacheLookup.SpecialMovementAbilities.Contains(Power);
				Initialize();
		  }

		  public virtual void Initialize()
		  {

		  }

		 #region Properties
		  public AbilityPriority Priority { get; set; }
		  ///<summary>
		  ///Describes variables for use of Ability: PreWait Loops, PostWait Loops, Reuseable
		  ///</summary>
		  public WaitLoops WaitVars { get; set; }
		  public int Range { get; set; }
		  public double Cost { get; set; }
		  public bool SecondaryEnergy { get; set; }
		  ///<summary>
		  ///This is used to determine how the Ability will be used
		  ///</summary>
		  public AbilityExecuteFlags ExecutionType { get; set; }

		  private AbilityUseage useageType;
		  public AbilityUseage UseageType
		  {
				get { return useageType; }
				set { useageType=value; if (value.HasFlag(AbilityUseage.OutOfCombat|AbilityUseage.Anywhere)) FcriteriaBuff=() => true; }
		  }

		  public virtual int RuneIndex { get { return -1; } }

		 internal bool IsADestructiblePower { get; set; }

		 internal bool IsASpecialMovementPower { get; set; }


		 ///<summary>
		  ///Ability will trigger WaitingForSpecial if Energy Check fails.
		  ///</summary>
		  public bool IsSpecialAbility { get; set; }
		  public bool IsBuff { get; set; }



		  ///<summary>
		  ///
		  ///</summary>
		  public bool IsRanged { get; set; }
		  ///<summary>
		  ///Ability is a projectile -- meaning it starts from bot position and travels to destination.
		  ///</summary>
		  public bool IsProjectile { get; set; }

		  ///<summary>
		  ///Ability is continously casted -- allowing other abilities to be used too.
		  ///</summary>
		  public bool IsChanneling { get; set; }

		  private DateTime LastUsed { get; set; }
		  internal double LastUsedMilliseconds
		  {
				get { return DateTime.Now.Subtract(LastUsed).TotalMilliseconds; }
		  }

		  internal double Cooldown
		  {
				get;
				set;
		  }

		  internal bool AbilityUseTimer(bool bReCheck=false)
		  {
				double lastUseMS=LastUsedMilliseconds;
				if (lastUseMS>=Cooldown)
					 return true;
				if (bReCheck&&lastUseMS>=150&&lastUseMS<=600)
					 return true;
				return false;
		  }


		  ///<summary>
		  ///Holds int value that describes pet count or buff stacks.
		  ///</summary>
		  public int Counter { get; set; }

          public bool IsCombat { get; set; }

		  #endregion



		  #region Test Methods
		  ///<summary>
		  ///Check Ability Buff Conditions
		  ///</summary>
		  public bool CheckBuffConditionMethod()
		  {
			  foreach (Func<bool> item in FcriteriaBuff.GetInvocationList())
			  {
				  if (!item()) return false;
			  }

			  return true;
		  }

		  public bool CheckCustomCombatMethod()
		  {
			  foreach (Func<bool> item in FcriteriaCombat.GetInvocationList())
				  if (!item()) return false;

			  return true;
		  }
		  ///<summary>
		  ///Check Ability is valid to use.
		  ///</summary>
		  public bool CheckPreCastConditionMethod()
		  {
			  return PreCast.Criteria.GetInvocationList().Cast<Func<Skill,bool>>().All(item => item(this));
		  }

		  ///<summary>
		  ///Check Combat
		  ///</summary>
		  public bool CheckCombatConditionMethod(ConditionCriteraTypes conditions = ConditionCriteraTypes.All)
		  {
			  //Order in which tests are conducted..

			  //Units in Range (Not Cluster)
			  //Clusters
			  //Single Target

			  //If all are null or any of them are successful, then we test Custom Conditions
			  //Custom Condition

			  //Reset Last Condition
			  LastConditionPassed = ConditionCriteraTypes.None;
			  bool TestCustomConditions = false;
			  bool FailedCondition = false;

			  if (conditions.HasFlag(ConditionCriteraTypes.ElitesInRange) && FElitesInRangeConditions != null)
			  {
				  foreach (Func<bool> item in FElitesInRangeConditions.GetInvocationList())
				  {
					  if (!item())
					  {
						  FailedCondition = true;
						  break;
					  }
				  }
				  if (!FailedCondition)
				  {
					  TestCustomConditions = true;
					  LastConditionPassed = ConditionCriteraTypes.ElitesInRange;
				  }
			  }
			  if ((!TestCustomConditions || FailedCondition) && conditions.HasFlag(ConditionCriteraTypes.UnitsInRange) && FUnitsInRangeConditions != null)
			  {
				  FailedCondition = false;
				  foreach (Func<bool> item in FUnitsInRangeConditions.GetInvocationList())
				  {
					  if (!item())
					  {
						  FailedCondition = true;
						  break;
					  }
				  }
				  if (!FailedCondition)
				  {
					  LastConditionPassed = ConditionCriteraTypes.UnitsInRange;
					  TestCustomConditions = true;
				  }
			  }
			  if ((!TestCustomConditions || FailedCondition) && conditions.HasFlag(ConditionCriteraTypes.Cluster) && FClusterConditions != null)
			  {
				  FailedCondition = false;

				  if (!FClusterConditions.Invoke())
				  {
					  FailedCondition = true;
				  }

				  if (!FailedCondition)
				  {
					  LastConditionPassed = ConditionCriteraTypes.Cluster;
					  TestCustomConditions = true;
				  }
			  }
			  if ((!TestCustomConditions || FailedCondition) && conditions.HasFlag(ConditionCriteraTypes.SingleTarget) && FSingleTargetUnitCriteria != null)
			  {
				  FailedCondition = false;
				  foreach (Func<bool> item in FSingleTargetUnitCriteria.GetInvocationList())
				  {
					  if (!item())
					  {
						  FailedCondition = true;
						  break;
					  }
				  }
				  if (!FailedCondition)
				  {
					  LastConditionPassed = ConditionCriteraTypes.SingleTarget;
					  TestCustomConditions = true;
				  }
			  }

			  //If TestCustomCondtion failed, and FailedCondition is true.. then we tested a combat condition.
			  //If FailedCondition is false, then we never tested a condition.
			  if (!TestCustomConditions && !TestCustomCombatConditions) return false; //&&FailedCondition


			  foreach (Func<bool> item in FcriteriaCombat.GetInvocationList())
				  if (!item()) return false;


			  return true;
		  }

		  #endregion



		  internal static bool CheckClusterConditions(SkillClusterConditions CC)
		  {
			  return Bot.Targeting.Clusters.AbilityClusterCache(CC).Count > 0;
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
				get { return Bot.Character.Data.iCurrentWorldID; }
		  }

		  private int _targetAcdguid;
		  internal int TargetACDGUID
		  {
				get { return _targetAcdguid; }
				set { _targetAcdguid=value; }
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

		  private CacheUnit Target_;

		  internal PowerManager.CanCastFlags CanCastFlags;
		  #endregion


		  public static void UsePower(ref Skill ability)
		  {
				if (!ability.ExecutionType.HasFlag(AbilityExecuteFlags.RemoveBuff))
				{
					 ability.SuccessUsed=ZetaDia.Me.UsePower(ability.Power, ability.TargetPosition, ability.WorldID, ability.TargetACDGUID);
				}
				else
				{
					 ZetaDia.Me.GetBuff(ability.Power).Cancel();
					 ability.SuccessUsed=true;
				}
		  }

		  public delegate void AbilitySuccessfullyUsed(Skill ability, bool ReorderAbilities);
		  public event AbilitySuccessfullyUsed SuccessfullyUsed;

		  ///<summary>
		  ///Sets values related to Ability usage
		  ///</summary>
		  public void OnSuccessfullyUsed(bool reorderAbilities=true)
		  {
				LastUsed=DateTime.Now;
				PowerCacheLookup.lastGlobalCooldownUse=DateTime.Now;

			    //Chart our starting combat location.. 
				if (Bot.Settings.Backtracking.EnableBacktracking && Bot.Settings.Backtracking.TrackStartOfCombatEngagment && Bot.Targeting.StartingLocation.Equals(Vector3.Zero))
					Bot.Targeting.StartingLocation = Bot.Character.Data.Position;

				if (ExecutionType.HasFlag(AbilityExecuteFlags.ZigZagPathing))
				{
					 //Reset Blockcounter --
					Bot.Targeting.TargetMover.BlockedMovementCounter = 0;
					Bot.Targeting.TargetMover.NonMovementCounter = 0;
					Bot.Targeting.TargetMover.LastMovementDuringCombat = DateTime.Now;
				}

				//Disable Reordering for Channeling Abilities!
				if (IsChanneling)
					 reorderAbilities=false;

				if (SuccessfullyUsed!=null)
					 SuccessfullyUsed(this, reorderAbilities);
		  }

		  public static void SetupAbilityForUse(ref Skill ability, bool Destructible=false)
		  {
				ability.MinimumRange=ability.Range;
				ability.TargetPosition_=Vector3.Zero;
				ability._targetAcdguid=-1;
				ability.WaitLoopsBefore_=ability.WaitVars.PreLoops;
				ability.WaitLoopsAfter_=ability.WaitVars.PostLoops;
				ability.CanCastFlags=PowerManager.CanCastFlags.None;
				ability.SuccessUsed_=null;
				ability.Target_=null;

				 //Destructible Setup
			  if (Destructible)
			  {
					if (!ability.IsRanged)
						 ability.MinimumRange=8f;
					else
						 ability.MinimumRange=30f;

				  bool LocationalAttack = (CacheIDLookup.hashDestructableLocationTarget.Contains(Bot.Targeting.CurrentTarget.SNOID)
													||DateTime.Now.Subtract(PowerCacheLookup.dictAbilityLastFailed[ability.Power]).TotalMilliseconds<1000);

				  if (LocationalAttack)
				  {
					  Vector3 attacklocation = Bot.Targeting.CurrentTarget.Position;

					  if (!ability.IsRanged)
					  {
							//attacklocation=MathEx.CalculatePointFrom(Bot.Character_.Data.Position,Bot.Target.CurrentTarget.Position, 0.25f);
							attacklocation=MathEx.GetPointAt(Bot.Character.Data.Position, 0.50f, Navigation.FindDirection(Bot.Character.Data.Position, Bot.Targeting.CurrentTarget.Position, true));
					  }
					  else
					  {
							attacklocation=MathEx.GetPointAt(Bot.Targeting.CurrentTarget.Position, 1f, Navigation.FindDirection(Bot.Targeting.CurrentTarget.Position, Bot.Character.Data.Position, true));
					  }

					  attacklocation.Z=Navigation.MGP.GetHeight(attacklocation.ToVector2());
					  ability.TargetPosition = attacklocation;
				  }
				  else
				  {
						 ability.TargetACDGUID=Bot.Targeting.CurrentTarget.AcdGuid.Value;
				  }

					return;
			  }


			  if (ability.LastConditionPassed==ConditionCriteraTypes.Cluster)
				{
					 CacheUnit ClusterUnit;
					 //Cluster Target -- Aims for Centeroid Unit
					 if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.ClusterTarget)&&CheckClusterConditions(ability.ClusterConditions)) //Cluster ACDGUID
					 {
						 ClusterUnit = Bot.Targeting.Clusters.AbilityClusterCache(ability.ClusterConditions)[0].GetNearestUnitToCenteroid();
						  ability.TargetACDGUID=ClusterUnit.AcdGuid.Value;
						  ability.Target_=ClusterUnit;
						  return;
					 }
					 //Cluster Location -- Aims for Center of Cluster
					 if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.ClusterLocation)&&CheckClusterConditions(ability.ClusterConditions)) //Cluster Target Position
					 {
						 ability.TargetPosition = (Vector3)Bot.Targeting.Clusters.AbilityClusterCache(ability.ClusterConditions)[0].Midpoint;
						  return;
					 }
					 //Cluster Target Nearest -- Gets nearest unit in cluster as target.
					 if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.ClusterTargetNearest)&&CheckClusterConditions(ability.ClusterConditions)) //Cluster Target Position
					 {
						 ClusterUnit = Bot.Targeting.Clusters.AbilityClusterCache(ability.ClusterConditions)[0].ListUnits[0];
						  ability.TargetACDGUID=ClusterUnit.AcdGuid.Value;
						  ability.Target_=ClusterUnit;
						  return;
					 }
				}

				if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.Location)) //Current Target Position
				{
					 ability.TargetPosition=Bot.Targeting.CurrentTarget.Position;
					 ability.Target_=Bot.Targeting.CurrentUnitTarget;
				}
				else if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.Self)) //Current Bot Position
					 ability.TargetPosition=Bot.Character.Data.Position;
				else if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.ZigZagPathing)) //Zig-Zag Pathing
				{
					 Bot.NavigationCache.vPositionLastZigZagCheck=Bot.Character.Data.Position;
					 if (Bot.Character.Class.ShouldGenerateNewZigZagPath())
						  Bot.Character.Class.GenerateNewZigZagPath();

					 ability.TargetPosition=Bot.NavigationCache.vSideToSideTarget;
				}
				else if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.Target)) //Current Target ACDGUID
				{
					 ability.Target_=Bot.Targeting.CurrentUnitTarget;
					 ability.TargetACDGUID=Bot.Targeting.CurrentTarget.AcdGuid.Value;
				}
		  }

		  ///<summary>
		  ///Returns an estimated destination using the minimum range and distance from the radius of target.
		  ///</summary>
		  internal Vector3 DestinationVector
		  {
				get
				{
					 Vector3 destinationV;
					 if (TargetPosition_==Vector3.Zero)
					 {
						  if (Target_!=null)
								destinationV=Target_.BotMeleeVector;
						  else
								return Vector3.Zero;
					 }
					 else
						  destinationV=TargetPosition_;

					 float DistanceFromTarget=Vector3.Distance(Bot.Character.Data.Position, destinationV);
					 if (IsRanged)
					 {
						 if (MinimumRange<DistanceFromTarget)
						  {
								float RangeNeeded=Math.Max(0f, (MinimumRange-DistanceFromTarget));
								Vector3 destinationVector=MathEx.CalculatePointFrom(Bot.Character.Data.Position, destinationV, RangeNeeded);
								if (!Navigation.MGP.CanStandAt(destinationVector))
								{
									 if (Bot.Settings.Debug.FunkyLogFlags.HasFlag(LogLevel.Ability))
										  Logger.Write(LogLevel.Ability, "Destination for Ability {0} requires further searching!", Power.ToString());


									 Vector3 NewDestinationV3;

                                     if(Bot.NavigationCache.AttemptFindSafeSpot(out NewDestinationV3, destinationV, Bot.Settings.Plugin.AvoidanceFlags))
									 {
										  return NewDestinationV3;
									 }
									return Vector3.Zero;
								}

								return destinationVector;
						  }
						 return Bot.Character.Data.Position;
					 }
					if (DistanceFromTarget<=MinimumRange)
						return Bot.Character.Data.Position;

					return destinationV;
				}
		  }



		  public override int GetHashCode()
		  {
				return (int)Power;
		  }
		  public override bool Equals(object obj)
		  {
				//Check for null and compare run-time types. 
				if (obj==null||GetType()!=obj.GetType())
				{
					 return false;
				}
			  Skill p=(Skill)obj;
			  return Power==p.Power;
		  }

		  public string DebugString()
		  {
				return String.Format("Ability: {0} [RuneIndex={1}] " + " Cost="+Cost+"\r\n"+
										  "Range={2} ReuseMS={3} Priority [{4}] UseType [{5}] Usage {6} \r\n"+
										  "Last Condition {7} -- Last Used {8} \r\n" + 
										  "Used Successfully=[{9}] -- CanCastFlags={10}",
																		Power, RuneIndex,
																		Range, Cooldown, Priority, ExecutionType,
																		UseageType,
																		LastConditionPassed, LastUsedMilliseconds<100000?LastUsedMilliseconds+"ms":"Never",
																		SuccessUsed.HasValue?SuccessUsed.Value.ToString():"NULL", CanCastFlags);
		  }





		  #region IAbility Members

		  public virtual SNOPower Power
		  {
				get { return SNOPower.None; }
		  }
		  #endregion
	 }

}