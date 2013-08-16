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

	 ///<summary>
	 ///Cached Object that Describes an individual ability.
	 ///</summary>
	 public partial class Ability : IAbility
	 {

			//Conditional Methods which are used to determine if the power should be used.
			//	 -Precast Conditions
			//	 -Combat Criteria
			//		  *These are either a Tuple Type or Custom Class
			//		  *When set, they create the delegate func that is used to validate the conditions.
			//	 -Final Custom Conditional Check



			public Ability()
			{
				 PreCastConditions=AbilityConditions.None;
				 Fcriteria=new Func<bool>(() => { return true; });
				 WaitVars=new WaitLoops(0, 0, true);
				 IsRanged=false;
				 UseageType=AbilityUseage.Anywhere;
				 ExecutionType=AbilityUseType.None;
				 LastConditionPassed=ConditionCriteraTypes.None;
				 IsSpecialAbility=false;
				 Range=0;
				 Priority=AbilityPriority.None;
				Initialize();
			}
			protected virtual void Initialize()
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

			internal ConditionCriteraTypes LastConditionPassed { get; set; }


			///<summary>
			///Method that is used to determine if current ability precast conditions are valid.
			///</summary>
			internal Func<bool> Fprecast { get; set; }

			private AbilityConditions precastconditions_;
			///<summary>
			///Describes the pre casting conditions - when set it will create the precast method used.
			///</summary>
			public virtual AbilityConditions PreCastConditions
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

			internal static void CreatePreCastConditions(out Func<bool> Fprecast, Ability ability)
		 {
				Fprecast=null;
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

			///<summary>
			///Describes values for clustering used for target (Cdistance, DistanceFromBot, MinUnits, IgnoreNonTargetable)
			///</summary>
			///<value>
			///Clustering Distance, Distance From Bot, Minimum Unit Count, Ignore Non-Targetables
			///</value>
			public virtual ClusterConditions ClusterConditions
			{
				 get { return ClusterConditions_; }
				 set
				 {
						ClusterConditions_=value;
				 }
			}
			private ClusterConditions ClusterConditions_;
			internal Func<bool> FClusterConditions { get; set; }

		 internal static void CreateClusterConditions(out Func<bool> FClusterConditions, Ability ability)
		 {
				FClusterConditions=new Func<bool>(() => { return CheckClusterConditions(ability.ClusterConditions); });
		 }
			internal static bool CheckClusterConditions(ClusterConditions CC)
			{
				 return Bot.Combat.Clusters(CC).Count>0;
			}

			///<summary>
			///Units within Range Conditions
			///</summary>
			internal Tuple<RangeIntervals, int> UnitsWithinRangeConditions
			{
				 get { return UnitsWithinRangeConditions_; }
				 set
				 {
						UnitsWithinRangeConditions_=value;
						FUnitsInRangeConditions+=new Func<bool>(() => { return Bot.Combat.iAnythingWithinRange[(int)UnitsWithinRangeConditions_.Item1]>=UnitsWithinRangeConditions_.Item2; });
				 }
			}
			private Tuple<RangeIntervals, int> UnitsWithinRangeConditions_;
			private Func<bool> FUnitsInRangeConditions { get; set; }

			///<summary>
			///Elites within Range Conditions
			///</summary>
			internal Tuple<RangeIntervals, int> ElitesWithinRangeConditions
			{
				 get { return ElitesWithinRangeConditions_; }
				 set
				 {
						ElitesWithinRangeConditions_=value;
						FElitesInRangeConditions+=new Func<bool>(() => { return Bot.Combat.iElitesWithinRange[(int)ElitesWithinRangeConditions_.Item1]>=ElitesWithinRangeConditions_.Item2; });
				 }
			}
			private Tuple<RangeIntervals, int> ElitesWithinRangeConditions_;
			private Func<bool> FElitesInRangeConditions { get; set; }


			///<summary>
			///Single Target Conditions -- Should only used if ability is offensive!
			///</summary>
			internal UnitTargetConditions TargetUnitConditionFlags
			{
				 get { return TargetUnitConditionFlags_; }
				 set
				 {
						TargetUnitConditionFlags_=value;


						if (TargetUnitConditionFlags_.Distance>-1)
							 FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.RadiusDistance<=TargetUnitConditionFlags_.Distance; });
						if (TargetUnitConditionFlags_.HealthPercent>0d)
							 FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.CurrentHealthPct.Value<=TargetUnitConditionFlags_.HealthPercent; });

						//TRUE CONDITIONS
						if (value.TrueConditionFlags.Equals(TargetProperties.None))
							 FSingleTargetUnitCriteria+=new Func<bool>(() => { return true; });
						else
						{
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.Boss))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsBoss; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.Burrowing))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsBurrowableUnit; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.FullHealth))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.CurrentHealthPct.Value==1d; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.IsSpecial))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.ObjectIsSpecial; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.Weak))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.UnitMaxHitPointAverageWeight<0; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.MissileDampening))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.MonsterMissileDampening; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.RareElite))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.IsEliteRareUnique; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.MissileReflecting))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsMissileReflecting; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.Shielding))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.MonsterShielding; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.Stealthable))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsStealthableUnit; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.SucideBomber))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsSucideBomber; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.TreasureGoblin))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsTreasureGoblin; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.Unique))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.MonsterUnique; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.Ranged))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.Monstersize.Value==MonsterSize.Ranged; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.TargetableAndAttackable))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.IsTargetableAndAttackable; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.Fast))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.IsFast; });
							 if (value.TrueConditionFlags.HasFlag(TargetProperties.DOTDPS))
								  FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.HasDOTdps.HasValue&&Bot.Target.CurrentUnitTarget.HasDOTdps.Value; });
						}

						//FALSE CONDITIONS
						if (value.FalseConditionFlags.Equals(TargetProperties.None))
							 FSingleTargetUnitCriteria+=new Func<bool>(() => { return true; });
						else
						{
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.Boss))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsBoss; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.Burrowing))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsBurrowableUnit; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.FullHealth))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.CurrentHealthPct.Value!=1d; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.IsSpecial))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.ObjectIsSpecial; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.Weak))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.UnitMaxHitPointAverageWeight>0; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.MissileDampening))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.MonsterMissileDampening; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.RareElite))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.IsEliteRareUnique; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.MissileReflecting))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsMissileReflecting; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.Shielding))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.MonsterShielding; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.Stealthable))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsStealthableUnit; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.SucideBomber))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsSucideBomber; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.TreasureGoblin))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsTreasureGoblin; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.Unique))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.MonsterUnique; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.Ranged))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.Monstersize.Value!=MonsterSize.Ranged; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.TargetableAndAttackable))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.IsTargetableAndAttackable; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.Fast))
									FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.IsFast; });
							 if (value.FalseConditionFlags.HasFlag(TargetProperties.DOTDPS))
								  FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.HasDOTdps.HasValue||!Bot.Target.CurrentUnitTarget.HasDOTdps.Value; });
						}
				 }
			}
			private UnitTargetConditions TargetUnitConditionFlags_;
			private Func<bool> FSingleTargetUnitCriteria { get; set; }

			///<summary>
			///Custom Conditions for Combat
			///</summary>
			internal Func<bool> Fcriteria { get; set; }

			///<summary>
			///Custom Conditions for Buffing
			///</summary>
			internal Func<bool> Fbuff { get; set; }



			///<summary>
			///Check Ability Buff Conditions
			///</summary>
			public bool CheckBuffConditionMethod()
			{
				 foreach (Func<bool> item in this.Fbuff.GetInvocationList())
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

			//Resets Properties
			public virtual void SetupAbilityForUse()
			{
				 MinimumRange=Range;
				 TargetPosition_=Vector3.Zero;
				 TargetRAGUID_=-1;
				 WaitLoopsBefore_=this.WaitVars.PreLoops;
				 WaitLoopsAfter_=this.WaitVars.PostLoops;

				 CanCastFlags=PowerManager.CanCastFlags.None;
				 SuccessUsed_=null;

			}

			internal PowerManager.CanCastFlags CanCastFlags;
			#endregion


			public virtual void UsePower()
			{

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