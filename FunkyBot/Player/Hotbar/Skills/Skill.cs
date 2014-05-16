using System;
using System.Linq;
using FunkyBot.Cache;
using FunkyBot.Cache.Objects;
using FunkyBot.Movement;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace FunkyBot.Player.HotBar.Skills
{


	///<summary>
	///Cached Object that Describes an individual Ability.
	///</summary>
	public class Skill : SkillCriteria
	{
		public Skill()
		{
			WaitVars = new WaitLoops(0, 0, true);
			IsRanged = false;
			IsProjectile = false;
			UseageType = SkillUseage.Anywhere;
			ExecutionType = SkillExecutionFlags.None;
			IsSpecialAbility = false;
			IsChanneling = false;
			IsCombat = false;
			Range = 0;
			Priority = SkillPriority.Low;
			LastUsed = DateTime.Today;
			IsADestructiblePower = PowerCacheLookup.AbilitiesDestructiblePriority.Contains(Power);
			IsASpecialMovementPower = PowerCacheLookup.SpecialMovementAbilities.Contains(Power);
			ShouldTrack = false;

			Initialize();
		}

		public virtual void Initialize()
		{

		}

		private bool preActivationFinished = false;
		internal virtual bool PreActivation()
		{
			// Force waiting for global cooldown timer or long-animation abilities
			if (WaitLoopsBefore >= 1 || (WaitWhileAnimating && DateTime.Now.Subtract(PowerCacheLookup.lastGlobalCooldownUse).TotalMilliseconds <= 50))
			{
				//Logger.DBLog.DebugFormat("Debug: Force waiting BEFORE Ability " + powerPrime.powerThis.ToString() + "...");
				Bot.Targeting.Cache.bWaitingForPower = true;
				if (WaitLoopsBefore >= 1)
					WaitLoopsBefore--;
				return false;
			}
			Bot.Targeting.Cache.bWaitingForPower = false;

			// Wait while animating before an attack
			if (WaitWhileAnimating)
				Bot.Character.Data.WaitWhileAnimating(5);

			preActivationFinished = true;
			return true;
		}

		private bool ActivationFinished = false;
		internal virtual bool Activation()
		{
			if (Power != SNOPower.Barbarian_Whirlwind && Power != SNOPower.DemonHunter_Strafe)
			{
				UsePower();
				Bot.NavigationCache.lastChangedZigZag = DateTime.Today;
				Bot.NavigationCache.vPositionLastZigZagCheck = Vector3.Zero;
			}
			else
			{
				// Special code to prevent whirlwind double-spam, this helps save fury
				bool bUseThisLoop = Power != Bot.Character.Class.LastUsedAbility.Power;
				if (!bUseThisLoop)
				{
					//powerLastSnoPowerUsed = SNOPower.None;
					if (LastUsedMilliseconds >= 200)
						bUseThisLoop = true;
				}
				if (bUseThisLoop)
				{
					UsePower();
				}
			}

			if (SuccessUsed.HasValue && SuccessUsed.Value)
			{
				OnSuccessfullyUsed();
			}
			else
			{
				PowerCacheLookup.dictAbilityLastFailed[Power] = DateTime.Now;
			}

			ActivationFinished = true;
			return true;
		}

		private bool postActivationFinished = false;
		internal virtual bool PostActivation()
		{
			// Wait for animating AFTER the attack
			if (Bot.Character.Class.PowerPrime.WaitWhileAnimating)
				Bot.Character.Data.WaitWhileAnimating(3);

			Bot.Targeting.Cache.bPickNewAbilities = true;

			// See if we should force a long wait AFTERWARDS, too
			// Force waiting AFTER power use for certain abilities
			Bot.Targeting.Cache.bWaitingAfterPower = false;
			if (Bot.Character.Class.PowerPrime.WaitLoopsAfter >= 1)
			{
				//Logger.DBLog.DebugFormat("Force waiting AFTER Ability " + powerPrime.powerThis.ToString() + "...");
				Bot.Targeting.Cache.bWaitingAfterPower = true;
			}

			postActivationFinished = true;
			return true;
		}

		public bool ActivateSkill()
		{
			if (!preActivationFinished)
			{
				if (!PreActivation())
					return false;
			}

			if (!ActivationFinished)
			{
				if (!Activation())
					return false;
			}

			if (!postActivationFinished)
			{
				if (!PostActivation())
					return false;
			}

			return true;
		}

		private void UsePower()
		{
			if (!ExecutionType.HasFlag(SkillExecutionFlags.RemoveBuff))
			{
				SuccessUsed = ZetaDia.Me.UsePower(Power, TargetPosition, WorldID, TargetACDGUID);
			}
			else
			{
				ZetaDia.Me.GetBuff(Power).Cancel();
				SuccessUsed = true;
			}
		}

		#region Properties
		public SkillPriority Priority { get; set; }
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
		public SkillExecutionFlags ExecutionType { get; set; }

		private SkillUseage useageType;
		public SkillUseage UseageType
		{
			get { return useageType; }
			set { useageType = value; if (value.HasFlag(SkillUseage.OutOfCombat | SkillUseage.Anywhere)) FcriteriaBuff = () => true; }
		}

		public virtual int RuneIndex { get { return -1; } }

		internal bool IsADestructiblePower { get; set; }

		///<summary>
		///Teleport, Leap, etc.. skills that transport the character.
		///</summary>
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

		public DateTime LastUsed { get; set; }
		internal double LastUsedMilliseconds
		{
			get { return DateTime.Now.Subtract(LastUsed).TotalMilliseconds; }
		}

		internal double Cooldown
		{
			get;
			set;
		}

		internal bool AbilityUseTimer(bool bReCheck = false)
		{
			double lastUseMS = LastUsedMilliseconds;
			if (lastUseMS >= Cooldown)
				return true;
			if (bReCheck && lastUseMS >= 150 && lastUseMS <= 600)
				return true;
			return false;
		}


		///<summary>
		///Holds int value that describes pet count or buff stacks.
		///</summary>
		public int Counter { get; set; }

		public bool IsCombat { get; set; }

		/// <summary>
		/// Adds/Updates Entries to the object it was used upon. (Power/Date)
		/// </summary>
		public bool ShouldTrack { get; set; }

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
			return PreCast.Criteria.GetInvocationList().Cast<Func<Skill, bool>>().All(item => item(this));
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
			if ((!TestCustomConditions || FailedCondition) && conditions.HasFlag(ConditionCriteraTypes.Cluster) && ClusterConditions.Count > 0)
			{
				FailedCondition = false;
				foreach (var condition in ClusterConditions)
				{
					foreach (Func<bool> item in condition.Criteria.GetInvocationList())
					{
						if (!item())
						{
							FailedCondition = true;
							break;
						}
					}

					//Last Condition did not fail.. (Success!)
					if (!FailedCondition)
					{
						LastConditionPassed = ConditionCriteraTypes.Cluster;
						LastClusterConditionSuccessful=condition;
						TestCustomConditions = true;
						break;
					}
				}
			}
			if ((!TestCustomConditions || FailedCondition) && conditions.HasFlag(ConditionCriteraTypes.SingleTarget) && SingleUnitCondition.Count > 0)
			{
				FailedCondition = false;
				
				//We iterate each condition in the list and test the criteria.
				foreach (var condition in SingleUnitCondition)
				{
					foreach (Func<bool> item in condition.Criteria.GetInvocationList())
					{
						if (!item())
						{
							FailedCondition = true;
							break;
						}
					}

					//Last Condition did not fail.. (Success!)
					if (!FailedCondition)
					{
						LastConditionPassed = ConditionCriteraTypes.SingleTarget;
						TestCustomConditions = true;
						break;
					}
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
			return Bot.Targeting.Cache.Clusters.AbilityClusterCache(CC).Count > 0;
		}



		#region UseAbilityVars

		private float minimumRange_;
		internal float MinimumRange
		{
			get { return minimumRange_; }
			set { minimumRange_ = value; }
		}

		private Vector3 TargetPosition_;
		internal Vector3 TargetPosition
		{
			get { return TargetPosition_; }
			set { TargetPosition_ = value; }
		}

		internal int WorldID
		{
			get { return Bot.Character.Data.CurrentWorldDynamicID; }
		}

		private int _targetAcdguid;
		internal int TargetACDGUID
		{
			get { return _targetAcdguid; }
			set { _targetAcdguid = value; }
		}

		private int WaitLoopsBefore_;
		internal int WaitLoopsBefore
		{
			get { return WaitLoopsBefore_; }
			set { WaitLoopsBefore_ = value; }
		}

		private int WaitLoopsAfter_;
		internal int WaitLoopsAfter
		{
			get { return WaitLoopsAfter_; }
			set { WaitLoopsAfter_ = value; }
		}

		internal bool WaitWhileAnimating
		{
			get { return WaitVars.Reusable; }
		}
		private bool? SuccessUsed_;
		internal bool? SuccessUsed
		{
			get { return SuccessUsed_; }
			set { SuccessUsed_ = value; }
		}

		private CacheUnit Target_;

		internal PowerManager.CanCastFlags CanCastFlags;
		#endregion


		public static void UsePower(ref Skill ability)
		{
			if (!ability.ExecutionType.HasFlag(SkillExecutionFlags.RemoveBuff))
			{
				ability.SuccessUsed = ZetaDia.Me.UsePower(ability.Power, ability.TargetPosition, ability.WorldID, ability.TargetACDGUID);
			}
			else
			{
				ZetaDia.Me.GetBuff(ability.Power).Cancel();
				ability.SuccessUsed = true;
			}
		}

		public delegate void AbilitySuccessfullyUsed(Skill ability, bool ReorderAbilities);
		public event AbilitySuccessfullyUsed SuccessfullyUsed;

		///<summary>
		///Sets values related to Ability usage
		///</summary>
		public void OnSuccessfullyUsed(bool reorderAbilities = true)
		{
			LastUsed = DateTime.Now;
			PowerCacheLookup.lastGlobalCooldownUse = DateTime.Now;

			//Chart our starting combat location.. 
			if (Bot.Settings.Backtracking.EnableBacktracking && Bot.Settings.Backtracking.TrackStartOfCombatEngagment && Bot.Targeting.Cache.StartingLocation.Equals(Vector3.Zero))
				Bot.Targeting.Cache.StartingLocation = Bot.Character.Data.Position;

			if (ExecutionType.HasFlag(SkillExecutionFlags.ZigZagPathing))
			{
				//Reset Blockcounter --
				Bot.Targeting.Movement.BlockedMovementCounter = 0;
				Bot.Targeting.Movement.NonMovementCounter = 0;
				Bot.Targeting.Movement.LastMovementDuringCombat = DateTime.Now;
			}

			if (ShouldTrack)
			{
				if (!Bot.Targeting.Cache.CurrentTarget.SkillsUsedOnObject.ContainsKey(this.Power))
					Bot.Targeting.Cache.CurrentTarget.SkillsUsedOnObject.Add(Power, DateTime.Now);
				else
					Bot.Targeting.Cache.CurrentTarget.SkillsUsedOnObject[Power] = DateTime.Now;
			}

			//Disable Reordering for Channeling Abilities!
			if (IsChanneling)
				reorderAbilities = false;

			if (SuccessfullyUsed != null)
				SuccessfullyUsed(this, reorderAbilities);
		}

		public static void SetupAbilityForUse(ref Skill ability, bool Destructible = false)
		{
			ability.MinimumRange = ability.Range;
			ability.TargetPosition_ = Vector3.Zero;
			ability._targetAcdguid = -1;
			ability.WaitLoopsBefore_ = ability.WaitVars.PreLoops;
			ability.WaitLoopsAfter_ = ability.WaitVars.PostLoops;
			ability.CanCastFlags = PowerManager.CanCastFlags.None;
			ability.SuccessUsed_ = null;
			ability.Target_ = null;
			ability.preActivationFinished = false;
			ability.ActivationFinished = false;
			ability.postActivationFinished = false;

			//Destructible Setup
			if (Destructible)
			{
				if (!ability.IsRanged)
					ability.MinimumRange = 8f;
				else
					ability.MinimumRange = 30f;

				bool LocationalAttack = (CacheIDLookup.hashDestructableLocationTarget.Contains(Bot.Targeting.Cache.CurrentTarget.SNOID)
												  || DateTime.Now.Subtract(PowerCacheLookup.dictAbilityLastFailed[ability.Power]).TotalMilliseconds < 1000);

				if (LocationalAttack)
				{
					Vector3 attacklocation = Bot.Targeting.Cache.CurrentTarget.Position;

					if (!ability.IsRanged)
					{
						//attacklocation=MathEx.CalculatePointFrom(Bot.Character_.Data.Position,Bot.Target.CurrentTarget.Position, 0.25f);
						attacklocation = MathEx.GetPointAt(Bot.Character.Data.Position, 0.50f, Navigation.FindDirection(Bot.Character.Data.Position, Bot.Targeting.Cache.CurrentTarget.Position, true));
					}
					else
					{
						attacklocation = MathEx.GetPointAt(Bot.Targeting.Cache.CurrentTarget.Position, 1f, Navigation.FindDirection(Bot.Targeting.Cache.CurrentTarget.Position, Bot.Character.Data.Position, true));
					}

					attacklocation.Z = Navigation.MGP.GetHeight(attacklocation.ToVector2());
					ability.TargetPosition = attacklocation;
				}
				else
				{
					ability.TargetACDGUID = Bot.Targeting.Cache.CurrentTarget.AcdGuid.Value;
				}

				return;
			}


			if (ability.LastConditionPassed == ConditionCriteraTypes.Cluster)
			{
				CacheUnit ClusterUnit;
				//Cluster Target -- Aims for Centeroid Unit
				if (ability.ExecutionType.HasFlag(SkillExecutionFlags.ClusterTarget) && CheckClusterConditions(ability.LastClusterConditionSuccessful)) //Cluster ACDGUID
				{
					ClusterUnit = Bot.Targeting.Cache.Clusters.AbilityClusterCache(ability.LastClusterConditionSuccessful)[0].GetNearestUnitToCenteroid();
					ability.TargetACDGUID = ClusterUnit.AcdGuid.Value;
					ability.Target_ = ClusterUnit;
					return;
				}
				//Cluster Location -- Aims for Center of Cluster
				if (ability.ExecutionType.HasFlag(SkillExecutionFlags.ClusterLocation) && CheckClusterConditions(ability.LastClusterConditionSuccessful)) //Cluster Target Position
				{
					ability.TargetPosition = (Vector3)Bot.Targeting.Cache.Clusters.AbilityClusterCache(ability.LastClusterConditionSuccessful)[0].Midpoint;
					return;
				}
				//Cluster Target Nearest -- Gets nearest unit in cluster as target.
				if (ability.ExecutionType.HasFlag(SkillExecutionFlags.ClusterTargetNearest) && CheckClusterConditions(ability.LastClusterConditionSuccessful)) //Cluster Target Position
				{
					ClusterUnit = Bot.Targeting.Cache.Clusters.AbilityClusterCache(ability.LastClusterConditionSuccessful)[0].ListUnits[0];
					ability.TargetACDGUID = ClusterUnit.AcdGuid.Value;
					ability.Target_ = ClusterUnit;
					return;
				}
			}

			if (ability.ExecutionType.HasFlag(SkillExecutionFlags.Location)) //Current Target Position
			{
				ability.TargetPosition = Bot.Targeting.Cache.CurrentTarget.Position;
				ability.Target_ = Bot.Targeting.Cache.CurrentUnitTarget;
			}
			else if (ability.ExecutionType.HasFlag(SkillExecutionFlags.Self)) //Current Bot Position
				ability.TargetPosition = Bot.Character.Data.Position;
			else if (ability.ExecutionType.HasFlag(SkillExecutionFlags.ZigZagPathing)) //Zig-Zag Pathing
			{
				Bot.NavigationCache.vPositionLastZigZagCheck = Bot.Character.Data.Position;
				if (Bot.Character.Class.ShouldGenerateNewZigZagPath())
					Bot.Character.Class.GenerateNewZigZagPath();

				ability.TargetPosition = Bot.NavigationCache.vSideToSideTarget;
			}
			else if (ability.ExecutionType.HasFlag(SkillExecutionFlags.Target)) //Current Target ACDGUID
			{
				ability.Target_ = Bot.Targeting.Cache.CurrentUnitTarget;
				ability.TargetACDGUID = Bot.Targeting.Cache.CurrentTarget.AcdGuid.Value;
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
				if (TargetPosition_ == Vector3.Zero)
				{
					if (Target_ != null)
						destinationV = Target_.BotMeleeVector;
					else
						return Vector3.Zero;
				}
				else
					destinationV = TargetPosition_;

				float DistanceFromTarget = Vector3.Distance(Bot.Character.Data.Position, destinationV);

				if (IsRanged)
				{
					if (MinimumRange < DistanceFromTarget)
					{
						float RangeNeeded = Math.Max(0f, (MinimumRange - DistanceFromTarget));
						Vector3 destinationVector = MathEx.CalculatePointFrom(Bot.Character.Data.Position, destinationV, RangeNeeded);
						if (!Navigation.MGP.CanStandAt(destinationVector))
						{
							Logger.Write(LogLevel.Ability, "Destination for Ability {0} requires further searching!", Power.ToString());


							Vector3 NewDestinationV3;

							if (Bot.NavigationCache.AttemptFindSafeSpot(out NewDestinationV3, destinationV, Bot.Settings.Plugin.AvoidanceFlags))
							{
								return NewDestinationV3;
							}
							return Vector3.Zero;
						}

						return destinationVector;
					}
					return Bot.Character.Data.Position;
				}
				if (DistanceFromTarget <= MinimumRange)
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
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Skill p = (Skill)obj;
			return Power == p.Power;
		}

		public string DebugString()
		{
			return String.Format("Ability: {0} [RuneIndex={1}] " + " Cost=" + Cost + "\r\n" +
									  "Range={2} ReuseMS={3} Priority [{4}] UseType [{5}] Usage {6} \r\n" +
									  "Last Condition {7} -- Last Used {8} \r\n" +
									  "Used Successfully=[{9}] -- CanCastFlags={10}",
																	Power, RuneIndex,
																	Range, Cooldown, Priority, ExecutionType,
																	UseageType,
																	LastConditionPassed, LastUsedMilliseconds < 100000 ? LastUsedMilliseconds + "ms" : "Never",
																	SuccessUsed.HasValue ? SuccessUsed.Value.ToString() : "NULL", CanCastFlags);
		}





		#region IAbility Members

		public virtual SNOPower Power
		{
			get { return SNOPower.None; }
		}
		#endregion


		#region Skill Logic Functions
		public static void CreateSkillLogicConditions(ref Skill ability)
		{
			//CreatePreCastConditions(ref ability.FcriteriaPreCast, ability);
			CreateTargetConditions(ref ability);
			CreateUnitsInRangeConditions(out ability.FUnitsInRangeConditions, ability);
			CreateElitesInRangeConditions(out ability.FElitesInRangeConditions, ability);
			CreateClusterConditions(ref ability);

			//Check if the 4 primary combat conditions are null -- and if the custom condition is not..
			if (ability.SingleUnitCondition.Count == 0 && ability.FUnitsInRangeConditions == null && ability.FElitesInRangeConditions == null && ability.ClusterConditions.Count == 0 && ability.FcriteriaCombat != null)
				ability.TestCustomCombatConditions = true;
		}


		#region Function Creation Methods
		private static void CreateClusterConditions(ref Skill ability)
		{
			if (ability.ClusterConditions.Count == 0)
				return;

			if (ability.ExecutionType.HasFlag(SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.ClusterTargetNearest))
			{
				foreach (var condition in ability.ClusterConditions)
				{
					Func<bool> combatCriteria = condition.Criteria;
					CreateLineOfSightTargetCheck(ref combatCriteria, ability);
					condition.Criteria = combatCriteria;
				}
			}
		}

		private static void CreateTargetConditions(ref Skill ability)
		{
			//No Conditions Set by default.. (?? May have to verify Ability execution can be Target)
			//-- Ranged Abilities that do not set any single target conditions will never be checked for LOS.
			if (ability.SingleUnitCondition.Count == 0)
			{
				//No Default Conditions Set.. however if Ability uses target as a execution type then we implement the LOS conditions.
				if (ability.ExecutionType.HasFlag(SkillExecutionFlags.Target))
					ability.SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None));
				else
					return;
			}

			//Attach Line of Sight Criteria to each entry
			foreach (UnitTargetConditions condition in ability.SingleUnitCondition)
			{
				Func<bool> combatCriteria = condition.Criteria;
				CreateLineOfSightTargetCheck(ref combatCriteria, ability);
				condition.Criteria = combatCriteria;
			}
		}

		private static void CreateLineOfSightTargetCheck(ref Func<bool> CombatCriteria, Skill ability)
		{
			if (ability.IsRanged)
			{
				CombatCriteria += () =>
				{
					if (!Bot.Targeting.Cache.CurrentUnitTarget.IgnoresLOSCheck && Bot.Targeting.Cache.CurrentUnitTarget.IsTargetableAndAttackable)
					{
						LOSInfo LOSINFO = Bot.Targeting.Cache.CurrentTarget.LineOfSight;
						if (LOSINFO.LastLOSCheckMS > 2000)
						{
							if (!LOSINFO.LOSTest(Bot.Character.Data.Position, true, false, true, ability.IsProjectile ? NavCellFlags.AllowProjectile:NavCellFlags.None))
							{
								//Raycast failed.. reset LOS Check -- for valid checking.
								if (!LOSINFO.RayCast.Value || (LOSINFO.ObjectIntersection.HasValue && !LOSINFO.ObjectIntersection.Value))
								{
									Bot.Targeting.Cache.CurrentTarget.RequiresLOSCheck = true;
									return false;
								}

								//if (LOSINFO.NavCellProjectile.HasValue && !LOSINFO.NavCellProjectile.Value) //NavCellFlag Walk Failed
								//{
								//	bool MovementException = ((Bot.Targeting.Cache.CurrentUnitTarget.MonsterTeleport || Bot.Targeting.Cache.CurrentTarget.IsTransformUnit) && Bot.Targeting.Cache.CurrentUnitTarget.AnimState == AnimationState.Transform);
								//	if (!MovementException) return false;
								//}
							}
						}
						//else if (LOSINFO.ObjectIntersection.HasValue && !LOSINFO.ObjectIntersection.Value)
						//{
						//	return false;
						//}
					}
					return true;
				};
			}
			else if (ability.Range > 0)
			{//Melee
				CombatCriteria += () =>
				{
					if (!Bot.Targeting.Cache.CurrentUnitTarget.IgnoresLOSCheck && Bot.Targeting.Cache.CurrentUnitTarget.IsTargetableAndAttackable)
					{
						float radiusDistance = Bot.Targeting.Cache.CurrentTarget.RadiusDistance;
						//Check if within interaction range..
						if (radiusDistance > ability.Range)
						{
							//Verify LOS walk
							LOSInfo LOSINFO = Bot.Targeting.Cache.CurrentTarget.LineOfSight;
							if (LOSINFO.LastLOSCheckMS > 2000)//||!LOSINFO.NavCellWalk.HasValue)
							{
								if (!LOSINFO.LOSTest(Bot.Character.Data.Position, NavRayCast: true, ServerObjectIntersection: false))
								{
									//bool MovementException=((Bot.Targeting.Cache.CurrentUnitTarget.MonsterTeleport||Bot.Targeting.Cache.CurrentTarget.IsTransformUnit)&&Bot.Targeting.Cache.CurrentUnitTarget.AnimState==Zeta.Internals.Actors.AnimationState.Transform);
									//Raycast failed.. reset LOS Check -- for valid checking.
									if (!LOSINFO.RayCast.Value)
										Bot.Targeting.Cache.CurrentTarget.RequiresLOSCheck = true;
									//else if (!LOSINFO.NavCellWalk.Value) //NavCellFlag Walk Failed
									//{
									//    bool MovementException = ((Bot.Targeting.Cache.CurrentUnitTarget.MonsterTeleport || Bot.Targeting.Cache.CurrentTarget.IsTransformUnit) && Bot.Targeting.Cache.CurrentUnitTarget.AnimState == Zeta.Internals.Actors.AnimationState.Transform);
									//    if (!MovementException)
									//        return false;
									//}
								}
							}
							//else if (LOSINFO.NavCellWalk.HasValue&&!LOSINFO.NavCellWalk.Value)
							//{
							//     return false;
							//}
						}
					}
					return true;
				};
			}
		}

		private static void CreateUnitsInRangeConditions(out Func<bool> FUnitRange, Skill ability)
		{
			FUnitRange = null;
			if (ability.UnitsWithinRangeConditions != null)
				FUnitRange += () => Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)ability.UnitsWithinRangeConditions.Item1] >= ability.UnitsWithinRangeConditions.Item2;
		}

		private static void CreateElitesInRangeConditions(out Func<bool> FUnitRange, Skill ability)
		{
			FUnitRange = null;
			if (ability.ElitesWithinRangeConditions != null)
				FUnitRange += () => Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)ability.ElitesWithinRangeConditions.Item1] >= ability.ElitesWithinRangeConditions.Item2;
		}
		#endregion

		#endregion
	}

}