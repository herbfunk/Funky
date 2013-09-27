using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;
using System.Collections.Generic;

using Zeta.Internals.SNO;
using FunkyTrinity.Cache;
using FunkyTrinity.Movement;

namespace FunkyTrinity.Ability
{


	 ///<summary>
	 ///Cached Object that Describes an individual ability.
	 ///</summary>
	 public abstract class ability : AbilityCriteria, IAbility
	 {

		  //Conditional Methods which are used to determine if the power should be used.
		  //	 -Precast Conditions
		  //	 -Combat Criteria
		  //		  *These are either a Tuple Type or Custom Class
		  //		  *When set, they create the delegate func that is used to validate the conditions.
		  //	 -Final Custom Conditional Check



		  public ability()
				: base()
		  {
				WaitVars=new WaitLoops(0, 0, true);
				IsRanged=false;
				IsProjectile=false;
				UseageType=AbilityUseage.Anywhere;
				ExecutionType=AbilityExecuteFlags.None;
				IsSpecialAbility=false;
				Range=0;
				Priority=AbilityPriority.None;
				LastUsed=DateTime.Today;
				isdestructiblepower=PowerCacheLookup.AbilitiesDestructiblePriority.Contains(this.Power);
				isASpecialMovementPower=PowerCacheLookup.SpecialMovementAbilities.Contains(this.Power);
				Initialize();
		  }
		  public virtual void Initialize()
		  {

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
		  public AbilityExecuteFlags ExecutionType { get; set; }

		  private AbilityUseage useageType;
		  public AbilityUseage UseageType
		  {
				get { return useageType; }
				set { useageType=value; if (value.HasFlag(AbilityUseage.OutOfCombat|AbilityUseage.Anywhere)) FcriteriaBuff=new Func<bool>(() => { return true; }); }
		  }

		  public virtual int RuneIndex { get { return -1; } }

		  private bool isdestructiblepower;
		  internal bool IsADestructiblePower
		  {
				get { return isdestructiblepower; }
				set { isdestructiblepower=value; }
		  }

		  private bool isASpecialMovementPower;
		  internal bool IsASpecialMovementPower
		  {
				get { return isASpecialMovementPower; }
				set { isASpecialMovementPower=value; }
		  }
		

		  ///<summary>
		  ///ability will trigger WaitingForSpecial if Energy Check fails.
		  ///</summary>
		  public bool IsSpecialAbility { get; set; }
		  public bool IsBuff { get; set; }



		  ///<summary>
		  ///
		  ///</summary>
		  public bool IsRanged { get; set; }
		  ///<summary>
		  ///ability is a projectile -- meaning it starts from bot position and travels to destination.
		  ///</summary>
		  public bool IsProjectile { get; set; }

		  private DateTime LastUsed { get; set; }
		  internal double LastUsedMilliseconds
		  {
				get { return DateTime.Now.Subtract(LastUsed).TotalMilliseconds; }
		  }

		  internal double Cooldown
		  {
				get { return Bot.Class.AbilityCooldowns[this.Power]; }
		  }

		  internal bool AbilityUseTimer(bool bReCheck=false)
		  {
				double lastUseMS=this.LastUsedMilliseconds;
				if (lastUseMS>=this.Cooldown)
					 return true;
				if (bReCheck&&lastUseMS>=150&&lastUseMS<=600)
					 return true;
				return false;
		  }


		  ///<summary>
		  ///Holds int value that describes pet count or buff stacks.
		  ///</summary>
		  public int Counter { get; set; }


		  #endregion







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


		  public static void UsePower(ref ability ability)
		  {
				if (!ability.ExecutionType.HasFlag(AbilityExecuteFlags.RemoveBuff))
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
				Bot.Class.LastUsedAbility=this;
		  }

		  public static void SetupAbilityForUse(ref ability ability, bool Destructible=false)
		  {
				ability.MinimumRange=ability.Range;
				ability.TargetPosition_=Vector3.Zero;
				ability.TargetRAGUID_=-1;
				ability.WaitLoopsBefore_=ability.WaitVars.PreLoops;
				ability.WaitLoopsAfter_=ability.WaitVars.PostLoops;
				ability.CanCastFlags=PowerManager.CanCastFlags.None;
				ability.SuccessUsed_=null;

				 //Destructible Setup
			  if (Destructible)
			  {
					if (!ability.IsRanged)
						 ability.MinimumRange=8f;
					else
						 ability.MinimumRange=30f;

				  bool LocationalAttack = (CacheIDLookup.hashDestructableLocationTarget.Contains(Bot.Target.CurrentTarget.SNOID)
													||DateTime.Now.Subtract(PowerCacheLookup.dictAbilityLastFailed[ability.Power]).TotalMilliseconds<1000);

				  if (LocationalAttack)
				  {
					  Vector3 attacklocation = Bot.Target.CurrentTarget.Position;

					  if (!ability.IsRanged)
					  {
							//attacklocation=MathEx.CalculatePointFrom(Bot.Character.Position,Bot.Target.CurrentTarget.Position, 0.25f);
							attacklocation=MathEx.GetPointAt(Bot.Character.Position, 0.50f, Navigation.FindDirection(Bot.Character.Position, Bot.Target.CurrentTarget.Position, true));
					  }
					  else
					  {
							attacklocation=MathEx.GetPointAt(Bot.Target.CurrentTarget.Position, 1f, Navigation.FindDirection(Bot.Target.CurrentTarget.Position, Bot.Character.Position, true));
					  }

					  attacklocation.Z=Navigation.MGP.GetHeight(attacklocation.ToVector2());
					  ability.TargetPosition = attacklocation;
				  }
				  else
				  {
						 ability.TargetRAGUID=Bot.Target.CurrentTarget.AcdGuid.Value;
				  }

					return;
			  }


			  if (ability.LastConditionPassed==ConditionCriteraTypes.Cluster)
				{
						 //Cluster Target -- Aims for Centeroid Unit
						 if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.ClusterTarget)&&CheckClusterConditions(ability.ClusterConditions)) //Cluster ACDGUID
						 {
							  ability.TargetRAGUID=Bot.Combat.Clusters(ability.ClusterConditions)[0].GetNearestUnitToCenteroid().AcdGuid.Value;
							  return;
						 }
						 //Cluster Location -- Aims for Center of Cluster
						 if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.ClusterLocation)&&CheckClusterConditions(ability.ClusterConditions)) //Cluster Target Position
						 {
							  ability.TargetPosition=(Vector3)Bot.Combat.Clusters(ability.ClusterConditions)[0].Midpoint;
							  return;
						 }
						 //Cluster Target Nearest -- Gets nearest unit in cluster as target.
						 if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.ClusterTargetNearest)&&CheckClusterConditions(ability.ClusterConditions)) //Cluster Target Position
						 {
							  ability.TargetRAGUID=Bot.Combat.Clusters(ability.ClusterConditions)[0].ListUnits[0].AcdGuid.Value;
							  return;
						 }
				}

				if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.Location)) //Current Target Position
					 ability.TargetPosition=Bot.Target.CurrentTarget.Position;
				else if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.Self)) //Current Bot Position
					 ability.TargetPosition=Bot.Character.Position;
				else if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.ZigZagPathing)) //Zig-Zag Pathing
				{
					 Bot.Combat.vPositionLastZigZagCheck=Bot.Character.Position;
					 if (Bot.Class.ShouldGenerateNewZigZagPath())
						  Bot.Class.GenerateNewZigZagPath();

					 ability.TargetPosition=Bot.Combat.vSideToSideTarget;
				}
				else if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.Target)) //Current Target ACDGUID
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
					 ability p=(ability)obj;
					 return this.Power==p.Power;
				}
		  }

		  public string DebugString()
		  {
				return String.Format("ability: {0} [RuneIndex={1}] \r\n"+
										  "Range={2} ReuseMS={3} Priority [{4}] UseType [{5}] \r\n"+
										  "Usage {6} \r\n"+
										  "Last Condition {7} -- Last Used {8} -- Used Successfully=[{9}]",
																		this.Power.ToString(), this.RuneIndex.ToString(),
																		this.Range.ToString(), this.Cooldown.ToString(), this.Priority.ToString(), this.ExecutionType.ToString(),
																		this.UseageType.ToString(),
																		this.LastConditionPassed.ToString(), this.LastUsedMilliseconds<100000?this.LastUsedMilliseconds.ToString()+"ms":"Never",
																		this.SuccessUsed.HasValue?this.SuccessUsed.Value.ToString():"NULL");
		  }





		  #region IAbility Members

		  public virtual SNOPower Power
		  {
				get { return SNOPower.None; }
		  }
		  #endregion
	 }

}