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
	 ///<summary>
	 ///Cached Object that Describes an individual ability.
	 ///</summary>
	 public partial class Ability
	 {

			//Conditional Methods which are used to determine if the power should be used.
			//	 -Precast Conditions
			//	 -Combat Criteria
			//		  *These are either a Tuple Type or Custom Class
			//		  *When set, they create the delegate func that is used to validate the conditions.
			//	 -Final Custom Conditional Check



			public Ability()
			{
				 Power=SNOPower.None;
				 PreCastConditions=AbilityConditions.None;
				 Fcriteria=new Func<bool>(() => { return true; });
				 WaitVars=new WaitLoops(0, 0, true);
				 IsRanged=false;
				 LastConditionPassed=ConditionCriteraTypes.None;
				 //TestCustomCombatConditionAlways=false;
				 IsSpecialAbility=false;

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

			internal int RuneIndex { get { return Bot.Class.RuneIndexCache[this.Power]; } }
			internal bool IsADestructiblePower { get { return PowerCacheLookup.AbilitiesDestructiblePriority.Contains(this.Power); } }
			internal bool IsASpecialMovementPower { get { return PowerCacheLookup.SpecialMovementAbilities.Contains(this.Power); } }

			public double Cost { get; set; }
			public bool SecondaryEnergy { get; set; }
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

			internal DateTime LastUsed
			{
				 get
				 {
						return PowerCacheLookup.dictAbilityLastUse[this.power_];
				 }
				 set
				 {
						PowerCacheLookup.dictAbilityLastUse[this.power_]=value;
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
			///Describes variables for use of ability: PreWait Loops, PostWait Loops, Reuseable
			///</summary>
			public WaitLoops WaitVars { get; set; }

			///<summary>
			///Holds int value that describes pet count or buff stacks.
			///</summary>
			public int Counter { get; set; }

			public AbilityPriority Priority { get; set; }

			///<summary>
			///This is used to determine how the ability will be used
			///</summary>
			public AbilityUseType UsageType { get; set; }

			///<summary>
			///This ability is allowed for buffing.
			///</summary>
			public bool UseOOCBuff { get; set; }
			///<summary>
			///This ability is allowed during avoidance movements.
			///</summary>
			public bool UseAvoiding { get; set; }

			/////<summary>
			/////will test final custom conditions even if all combat criteria conditions failed or none were ever set.
			/////</summary>
			//public bool TestCustomCombatConditionAlways { get; set; }

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
			internal AbilityConditions PreCastConditions
			{
				 get
				 {
						return precastconditions_;
				 }
				 set
				 {
						precastconditions_=value;
						Fprecast=null;

						if (precastconditions_.Equals(AbilityConditions.None))
							 Fprecast+=(new Func<bool>(() => { return true; }));
						else
						{
							 if (precastconditions_.HasFlag(AbilityConditions.CheckPlayerIncapacitated))
									Fprecast+=(new Func<bool>(() => { return !Bot.Character.bIsIncapacitated; }));

							 if (precastconditions_.HasFlag(AbilityConditions.CheckPlayerRooted))
									Fprecast+=(new Func<bool>(() => { return !Bot.Character.bIsRooted; }));

							 if (precastconditions_.HasFlag(AbilityConditions.CheckExisitingBuff))
									Fprecast+=(new Func<bool>(() => { return !Bot.Class.HasBuff(this.Power); }));

							 if (precastconditions_.HasFlag(AbilityConditions.CheckPetCount))
									Fprecast+=(new Func<bool>(() => { return Bot.Class.MainPetCount<this.Counter; }));

							 if (precastconditions_.HasFlag(AbilityConditions.CheckRecastTimer))
									Fprecast+=(new Func<bool>(() => { return this.LastUsedMilliseconds>this.Cooldown; }));

							 if (precastconditions_.HasFlag(AbilityConditions.CheckCanCast))
							 {
									Fprecast+=(new Func<bool>(() =>
									{
										 bool cancast=PowerManager.CanCast(this.Power, out this.CanCastFlags);

										 //Special Ability -- Trigger Waiting For Special When Not Enough Resource to Cast.
										 if (this.IsSpecialAbility)
										 {
												if (!cancast&&this.CanCastFlags.HasFlag(PowerManager.CanCastFlags.PowerNotEnoughResource))
													 Bot.Class.bWaitingForSpecial=true;
												else
													 Bot.Class.bWaitingForSpecial=false;
										 }

										 return cancast;
									}));
							 }

							 if (precastconditions_.HasFlag(AbilityConditions.CheckEnergy))
							 {
									if (!this.SecondaryEnergy)
										 Fprecast+=(new Func<bool>(() =>
										 {
												bool energyCheck=Bot.Character.dCurrentEnergy>=this.Cost;
												if (this.IsSpecialAbility) //we trigger waiting for special here.
													 Bot.Class.bWaitingForSpecial=!energyCheck;
												return energyCheck;
										 }));
									else
										 Fprecast+=(new Func<bool>(() =>
										 {
												bool energyCheck=Bot.Character.dDiscipline>=this.Cost;
												if (this.IsSpecialAbility) //we trigger waiting for special here.
													 Bot.Class.bWaitingForSpecial=!energyCheck;
												return energyCheck;
										 }));
							 }
						}
				 }
			}

			///<summary>
			///Describes values for clustering used for target (Cdistance, DistanceFromBot, MinUnits, IgnoreNonTargetable)
			///</summary>
			///<value>
			///Clustering Distance, Distance From Bot, Minimum Unit Count, Ignore Non-Targetables
			///</value>
			internal ClusterConditions ClusterConditions
			{
				 get { return ClusterConditions_; }
				 set
				 {
						ClusterConditions_=value;
						FClusterConditions=new Func<bool>(() => { return CheckClusterConditions(this.ClusterConditions); });
				 }
			}
			private ClusterConditions ClusterConditions_;
			private Func<bool> FClusterConditions { get; set; }

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
			internal void SetupAbilityForUse()
			{
				 MinimumRange=Range;
				 TargetPosition_=Vector3.Zero;
				 TargetRAGUID_=-1;
				 WaitLoopsBefore_=this.WaitVars.PreLoops;
				 WaitLoopsAfter_=this.WaitVars.PostLoops;

				 CanCastFlags=PowerManager.CanCastFlags.None;
				 SuccessUsed_=null;

				 //Check Clustering First.. we verify that cluster condition was last to be tested.
				 if (this.UsageType.HasFlag(AbilityUseType.ClusterTarget)&&CheckClusterConditions(this.ClusterConditions)) //Cluster ACDGUID
				 {
						//ListUnits[0].AcdGuid.Value;
						TargetRAGUID_=Bot.Combat.Clusters(this.ClusterConditions)[0].GetNearestUnitToCenteroid().AcdGuid.Value;
						return;
				 }
				 if (this.UsageType.HasFlag(AbilityUseType.ClusterLocation)&&CheckClusterConditions(this.ClusterConditions)) //Cluster Target Position
				 {
						//.ListUnits.First(u => u.ObjectIsValidForTargeting).Position;
					  TargetPosition_=(Vector3)Bot.Combat.Clusters(this.ClusterConditions)[0].Midpoint;
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

			internal PowerManager.CanCastFlags CanCastFlags;
			#endregion


			public void UsePower()
			{
				 if (!this.UsageType.HasFlag(AbilityUseType.RemoveBuff))
				 {
						PowerManager.CanCast(this.Power, out CanCastFlags);
						SuccessUsed_=ZetaDia.Me.UsePower(this.Power, this.TargetPosition_, this.WorldID, this.TargetRAGUID_);
				 }
				 else
				 {
						ZetaDia.Me.GetBuff(this.Power).Cancel();
						SuccessUsed_=true;
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
						Ability p=(Ability)obj;
						return this.Power==p.Power;
				 }
			}

			public string DebugString()
			{
				 return String.Format("Ability: {0} [RuneIndex={1}] \r\n"+
											"Range={2} ReuseMS={3} Priority [{4}] UseType [{5}] \r\n"+
											"Avoid {6} Buff {7} \r\n"+
											"Last Condition {8} -- Last Used {9}",
																		 this.power_.ToString(), this.RuneIndex.ToString(),
																		 this.Range.ToString(), this.Cooldown.ToString(), this.Priority.ToString(), this.UsageType.ToString(),
																		 this.UseAvoiding.ToString(), this.UseOOCBuff.ToString(),
																		 this.LastConditionPassed.ToString(), this.LastUsedMilliseconds<100000?this.LastUsedMilliseconds.ToString()+"ms":"Never");
			}




	 }

}