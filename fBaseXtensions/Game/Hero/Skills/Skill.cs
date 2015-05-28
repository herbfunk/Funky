﻿using System;
using System.Linq;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Navigation.Gridpoint;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;

namespace fBaseXtensions.Game.Hero.Skills
{


	///<summary>
	///Cached Object that Describes an individual Ability.
	///</summary>
	public class Skill : SkillCriteria
	{
		public Skill()
		{
			IsCombat = false;
			LastUsed = DateTime.MinValue;
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
				FunkyGame.Targeting.Cache.bWaitingForPower = true;
				if (WaitLoopsBefore >= 1)
					WaitLoopsBefore--;
				return false;
			}
			FunkyGame.Targeting.Cache.bWaitingForPower = false;

			// Wait while animating before an attack
			if (WaitWhileAnimating)
				FunkyGame.Hero.WaitWhileAnimating(5);

			preActivationFinished = true;
			return true;
		}

		private bool ActivationFinished = false;
		internal virtual bool Activation()
		{
			if (Power != SNOPower.Barbarian_Whirlwind && Power != SNOPower.DemonHunter_Strafe)
			{
				UsePower();
				FunkyGame.Navigation.lastChangedZigZag = DateTime.Today;
				FunkyGame.Navigation.vPositionLastZigZagCheck = Vector3.Zero;
			}
			else
			{
				// Special code to prevent whirlwind double-spam, this helps save fury
				bool bUseThisLoop = Power != FunkyGame.Hero.Class.LastUsedAbility.Power;
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
			    _failureToUseCounter = 0;
                OnSuccessfullyUsed();

			}
			else
			{
			    _lastFailureDateTime = DateTime.Now;
			    _failureToUseCounter++;
				PowerCacheLookup.dictAbilityLastFailed[Power] = DateTime.Now;
			}

			ActivationFinished = true;
			return true;
		}

		private bool postActivationFinished = false;
		internal virtual bool PostActivation()
		{
			// Wait for animating AFTER the attack
			if (FunkyGame.Hero.Class.PowerPrime.WaitWhileAnimating)
				FunkyGame.Hero.WaitWhileAnimating(3);

			FunkyGame.Targeting.Cache.bPickNewAbilities = true;
			
			// See if we should force a long wait AFTERWARDS, too
			// Force waiting AFTER power use for certain abilities
			FunkyGame.Targeting.Cache.bWaitingAfterPower = false;
			if (FunkyGame.Hero.Class.PowerPrime.WaitLoopsAfter >= 1)
			{
				//Logger.DBLog.DebugFormat("Force waiting AFTER Ability " + powerPrime.powerThis.ToString() + "...");
				FunkyGame.Targeting.Cache.bWaitingAfterPower = true;
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
			if (!ObjectCache.CheckFlag(ExecutionType, SkillExecutionFlags.RemoveBuff))
			{
				SuccessUsed = ZetaDia.Me.UsePower(Power, TargetPosition, WorldID, TargetACDGUID);
			}
			else
			{
				ZetaDia.Me.GetBuff(Power).Cancel();
				SuccessUsed = true;
			}
		}

		public int RuneIndex { get { return Hotbar.ReturnRuneIndex(Power); } }

		#region Properties
		public virtual SkillPriority Priority
		{
			get { return _priority; }
			set { _priority = value; }
		}
		private SkillPriority _priority = SkillPriority.None;
		///<summary>
		///Describes variables for use of Ability: PreWait Loops, PostWait Loops, Reuseable
		///</summary>
		public virtual WaitLoops WaitVars
		{
			get { return _waitvars; }
			set { _waitvars = value; }
		}
		private WaitLoops _waitvars=WaitLoops.Default;

		/// <summary>
		/// The Minimum Distance Required Before Activation will Occur.
		/// </summary>
		public int Range
		{
			get { return _range; }
			set { _range = value; }
		}
		private int _range = 0;

		public double Cost
		{
			get { return _cost; }
			set { _cost = value; }
		}
		private double _cost = 0;

		public virtual bool SecondaryEnergy
		{
			get { return _secondaryenergy; }
			set { _secondaryenergy = value; }
		}
		private bool _secondaryenergy = false;
		///<summary>
		///This is used to determine how the Ability will be used
		///</summary>
		public virtual SkillExecutionFlags ExecutionType
		{
			get { return _executiontype; }
			set { _executiontype = value; }
		}
		private SkillExecutionFlags _executiontype = SkillExecutionFlags.None;

		
		public virtual SkillUseage UseageType
		{
			get { return useageType; }
			set { useageType = value; if (value.HasFlag(SkillUseage.OutOfCombat | SkillUseage.Anywhere)) FcriteriaBuff = () => true; }
		}
		private SkillUseage useageType = SkillUseage.Anywhere;

		

		public virtual bool IsPrimarySkill
		{
			get { return _isprimaryskill; }
			set { _isprimaryskill = value; }
		}
		private bool _isprimaryskill = false;
		/// <summary>
		/// Used to destroy destructible objects
		/// </summary>
		public virtual bool IsDestructiblePower
		{
			get { return _isdstructiblepower; }
			set { _isdstructiblepower = value; }
		}
		private bool _isdstructiblepower = false;
		///<summary>
		///Teleport, Leap, etc.. skills that transport the character.
		///</summary>
		public virtual bool IsMovementSkill
		{
			get { return _ismovementskill; }
			set { _ismovementskill = value; }
		}
		private bool _ismovementskill = false;
		/// <summary>
		/// Spirit Walk and Steed Charge.. skills that modify the range while active
		/// </summary>
		public virtual bool IsSpecialMovementSkill
		{
			get { return _isspecialmovementskill; }
			set { _isspecialmovementskill = value; }
		}
		private bool _isspecialmovementskill = false;

		///<summary>
		///Ability will trigger WaitingForSpecial if Energy Check fails.
		///</summary>
		public virtual bool IsSpecialAbility
		{
			get { return _isspecialability; }
			set { _isspecialability = value; }
		}
		private bool _isspecialability = false;
		public virtual bool IsBuff
		{
			get { return _isbuff; }
			set { _isbuff = value; }
		}
		private bool _isbuff = false;



		///<summary>
		///
		///</summary>
		public virtual bool IsRanged
		{
			get { return _isranged; }
			set { _isranged = value; }
		}
		private bool _isranged = false;
		///<summary>
		///Ability is a projectile -- meaning it starts from bot position and travels to destination.
		///</summary>
		public virtual bool IsProjectile
		{
			get { return _isprojectile; }
			set { _isprojectile = value; }
		}
		private bool _isprojectile = false;

		///<summary>
		///Ability is continously casted -- allowing other abilities to be used too.
		///</summary>
		public virtual bool IsChanneling
		{
			get { return _ischanneling; }
			set { _ischanneling = value; }
		}
		private bool _ischanneling = false;

        /// <summary>
        /// Ability should check pierce count on targets
        /// </summary>
	    public virtual bool IsPiercing
	    {
            get { return _ispiercing; }
            set { _ispiercing = value; }
	    }
	    private bool _ispiercing = false;

		public DateTime LastUsed { get; set; }
		internal double LastUsedMilliseconds
		{
			get { return DateTime.Now.Subtract(LastUsed).TotalMilliseconds; }
		}

		public virtual double Cooldown
		{
			get { return _cooldown; }
			set { _cooldown = value; }
		}
		private double _cooldown = 0;

        /// <summary>
        /// Adds/Updates Entries to the object it was used upon. (Power/Date)
        /// </summary>
        public virtual bool ShouldTrack
        {
            get { return _shouldtrack; }
            set { _shouldtrack = value; }
        }
        private bool _shouldtrack = false;


		///<summary>
		///Holds int value that describes pet count or buff stacks.
		///</summary>
		public int Counter { get; set; }

		public bool IsCombat { get; set; }

	   
	    

        public int FailureToUseCounter
        {
            get { return _failureToUseCounter; }
            set { _failureToUseCounter = value; }
        }
        private int _failureToUseCounter=0;

        public DateTime LastFailureDateTime
        {
            get { return _lastFailureDateTime; }
            set { _lastFailureDateTime = value; }
        }
        private DateTime _lastFailureDateTime=DateTime.Now;
		#endregion

        
		#region Test Methods
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

		public bool CheckCustomCombatMethod(CacheUnit unit)
		{
			foreach (Func<CacheUnit, bool> item in FcriteriaCombat.GetInvocationList())
                if (!item(unit)) return false;

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
		public bool CheckCombatConditionMethod(ConditionCriteraTypes conditions = ConditionCriteraTypes.All, CacheUnit unit = null)
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

			if (conditions.HasFlag(ConditionCriteraTypes.Cluster) && ClusterConditions.Count > 0)
			{
				foreach (var condition in ClusterConditions)
				{
					FailedCondition = false;

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
				//We iterate each condition in the list and test the criteria.
				foreach (var condition in SingleUnitCondition)
				{
					FailedCondition = false;

					foreach (Func<CacheUnit,bool> item in condition.Criteria.GetInvocationList())
					{
						if (!item(unit))
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


			foreach (Func<CacheUnit, bool> item in FcriteriaCombat.GetInvocationList())
				if (!item(unit)) return false;


			return true;
		}

		#endregion



		internal static bool CheckClusterConditions(SkillClusterConditions CC)
		{
			return FunkyGame.Targeting.Cache.Clusters.AbilityClusterCache(CC).Count > 0;
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
			get { return FunkyGame.Hero.CurrentWorldDynamicID; }
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
			if (!ObjectCache.CheckFlag(ability.ExecutionType, SkillExecutionFlags.RemoveBuff))
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
		public virtual void OnSuccessfullyUsed(bool reorderAbilities = true)
		{
			LastUsed = DateTime.Now;
			PowerCacheLookup.lastGlobalCooldownUse = DateTime.Now;

			//Chart our starting combat location.. 
			if (FunkyBaseExtension.Settings.Backtracking.EnableBacktracking && FunkyBaseExtension.Settings.Backtracking.TrackStartOfCombatEngagment && FunkyGame.Targeting.Cache.StartingLocation.Equals(Vector3.Zero))
				FunkyGame.Targeting.Cache.StartingLocation = FunkyGame.Hero.Position;

			if (ObjectCache.CheckFlag(ExecutionType, SkillExecutionFlags.ZigZagPathing))
			{
				//Reset Blockcounter --
				FunkyGame.Targeting.cMovement.BlockedMovementCounter = 0;
				FunkyGame.Targeting.cMovement.NonMovementCounter = 0;
				FunkyGame.Targeting.cMovement.LastMovementDuringCombat = DateTime.Now;
			}

			if (ShouldTrack)
			{
				if (!FunkyGame.Targeting.Cache.CurrentTarget.SkillsUsedOnObject.ContainsKey(Power))
					FunkyGame.Targeting.Cache.CurrentTarget.SkillsUsedOnObject.Add(Power, DateTime.Now);
				else
					FunkyGame.Targeting.Cache.CurrentTarget.SkillsUsedOnObject[Power] = DateTime.Now;
			}

			//Disable Reordering for Channeling Abilities!
			if (IsChanneling)
				reorderAbilities = false;

			if (SuccessfullyUsed != null)
				SuccessfullyUsed(this, reorderAbilities);
		}

	    /// <summary>
	    /// Resets usage variables and sets the target location or target ID depending on what condition passed.
	    /// </summary>
	    /// <param name="ability"></param>
	    /// <param name="obj"></param>
	    /// <param name="Destructible"></param>
	    public static void SetupAbilityForUse(ref Skill ability, CacheObject obj, bool Destructible = false)
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

                bool LocationalAttack = (CacheIDLookup.hashDestructableLocationTarget.Contains(obj.SNOID)
												  || DateTime.Now.Subtract(PowerCacheLookup.dictAbilityLastFailed[ability.Power]).TotalMilliseconds < 1000);

				if (LocationalAttack)
				{
                    Vector3 attacklocation = obj.Position;

					if (!ability.IsRanged)
					{
						//attacklocation=MathEx.CalculatePointFrom(FunkyGame.Hero.Class_.Data.Position,Bot.Target.CurrentTarget.Position, 0.25f);
                        attacklocation = MathEx.GetPointAt(FunkyGame.Hero.Position, 0.50f, Navigation.Navigation.FindDirection(FunkyGame.Hero.Position, obj.Position, true));
					}
					else
					{
                        attacklocation = MathEx.GetPointAt(obj.Position, 1f, Navigation.Navigation.FindDirection(obj.Position, FunkyGame.Hero.Position, true));
					}

					attacklocation.Z = Navigation.Navigation.MGP.GetHeight(attacklocation.ToVector2());
					ability.TargetPosition = attacklocation;
				}
				else
				{
                    if (obj.AcdGuid.HasValue)
                        ability.TargetACDGUID = obj.AcdGuid.Value;
				}

				return;
			}


			if (ability.LastConditionPassed == ConditionCriteraTypes.Cluster)
			{
				CacheUnit ClusterUnit;
				//Cluster Target -- Aims for Centeroid Unit
				if (ObjectCache.CheckFlag(ability.ExecutionType, SkillExecutionFlags.ClusterTarget) && CheckClusterConditions(ability.LastClusterConditionSuccessful)) //Cluster ACDGUID
				{
					ClusterUnit = FunkyGame.Targeting.Cache.Clusters.AbilityClusterCache(ability.LastClusterConditionSuccessful)[0].GetNearestUnitToCenteroid();
                    if (ClusterUnit.AcdGuid.HasValue)
                        ability.TargetACDGUID = ClusterUnit.AcdGuid.Value;

					ability.Target_ = ClusterUnit;
					return;
				}
				//Cluster Location -- Aims for Center of Cluster
				if (ObjectCache.CheckFlag(ability.ExecutionType, SkillExecutionFlags.ClusterLocation) && CheckClusterConditions(ability.LastClusterConditionSuccessful)) //Cluster Target Position
				{
					ability.TargetPosition = (Vector3)FunkyGame.Targeting.Cache.Clusters.AbilityClusterCache(ability.LastClusterConditionSuccessful)[0].Midpoint;
					return;
				}
				//Cluster Location Nearest
				if (ObjectCache.CheckFlag(ability.ExecutionType, SkillExecutionFlags.ClusterLocationNearest) && CheckClusterConditions(ability.LastClusterConditionSuccessful)) //Cluster Target Position
				{
					ability.TargetPosition = FunkyGame.Targeting.Cache.Clusters.AbilityClusterCache(ability.LastClusterConditionSuccessful)[0].ListUnits[0].Position;
					return;
				}
				//Cluster Target Nearest -- Gets nearest unit in cluster as target.
				if (ObjectCache.CheckFlag(ability.ExecutionType, SkillExecutionFlags.ClusterTargetNearest) && CheckClusterConditions(ability.LastClusterConditionSuccessful)) //Cluster Target Position
				{
					ClusterUnit = FunkyGame.Targeting.Cache.Clusters.AbilityClusterCache(ability.LastClusterConditionSuccessful)[0].ListUnits[0];
                    if (ClusterUnit.AcdGuid.HasValue)
                        ability.TargetACDGUID = ClusterUnit.AcdGuid.Value;

					ability.Target_ = ClusterUnit;
					return;
				}
			}

			if (ObjectCache.CheckFlag(ability.ExecutionType, SkillExecutionFlags.Location)) //Current Target Position
			{
                ability.TargetPosition = obj.Position;
                ability.Target_ = (CacheUnit)obj;
			}
			else if (ObjectCache.CheckFlag(ability.ExecutionType, SkillExecutionFlags.Self)) //Current Bot Position
				ability.TargetPosition = FunkyGame.Hero.Position;
			else if (ObjectCache.CheckFlag(ability.ExecutionType, SkillExecutionFlags.ZigZagPathing)) //Zig-Zag Pathing
			{
				FunkyGame.Navigation.vPositionLastZigZagCheck = FunkyGame.Hero.Position;
				if (FunkyGame.Hero.Class.ShouldGenerateNewZigZagPath()) FunkyGame.Hero.Class.GenerateNewZigZagPath();

				ability.TargetPosition = FunkyGame.Navigation.vSideToSideTarget;
			}
			else if (ObjectCache.CheckFlag(ability.ExecutionType, SkillExecutionFlags.Target)) //Current Target ACDGUID
			{
			    if (obj is CacheUnit)
			        ability.Target_ = (CacheUnit)obj;

                if (obj.AcdGuid.HasValue)
                    ability.TargetACDGUID = obj.AcdGuid.Value;
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

				float DistanceFromTarget = Vector3.Distance(FunkyGame.Hero.Position, destinationV);

				if (IsRanged)
				{
					if (MinimumRange < DistanceFromTarget)
					{
						float RangeNeeded = Math.Max(0f, (MinimumRange - DistanceFromTarget));
						Vector3 destinationVector = MathEx.CalculatePointFrom(FunkyGame.Hero.Position, destinationV, RangeNeeded);
						if (!Navigation.Navigation.MGP.CanStandAt(destinationVector))
						{
							Logger.Write(LogLevel.Ability, "Destination for Ability {0} requires further searching!", Power.ToString());


                            //Vector3 NewDestinationV3;

                            //if (FunkyGame.Navigation.AttemptFindSafeSpot(out NewDestinationV3, destinationV, PointCheckingFlags.RaycastWalkable))
                            //{
                            //    return NewDestinationV3;
                            //}
							return Vector3.Zero;
						}

						return destinationVector;
					}
					return FunkyGame.Hero.Position;
				}
				if (DistanceFromTarget <= MinimumRange)
					return FunkyGame.Hero.Position;

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

			return String.Format("Skill: {0} [RuneIndex={1}] " + " Cost=" + Cost + "\r\n" +
									  "Range={2} ReuseMS={3} Priority [{4}] UseType [{5}] Usage {6} \r\n" +
									  "Last Condition {7} -- Last Used {8} \r\n" +
									  "Used Successfully=[{9}] -- CanCastFlags={10}\r\n" +
			                          "Last Failed Use=[{20}secs] Total Failure Count={21}\r\n" +
									  "IsPrimarySkill {11} IsBuff {12} IsDestructiblePower {13}\r\n" +
									  "IsRanged {14} IsProjectile {15}\r\n" +
			                          "{16}{17}{18}{19}",

									Power, RuneIndex,
									Range, Cooldown, Priority, ExecutionType,
									UseageType,
									LastConditionPassed, LastUsedMilliseconds < 100000 ? LastUsedMilliseconds + "ms" : "Never",
									SuccessUsed.HasValue ? SuccessUsed.Value.ToString() : "NULL", CanCastFlags,
									IsPrimarySkill, IsBuff, IsDestructiblePower, IsRanged, IsProjectile,
                                    IsSpecialAbility?" IsSpecialAbility ":"",
                                    ShouldTrack?" ShouldTrack ":"",
                                    IsChanneling?" IsChanneling ":"",
                                    IsMovementSkill?" IsMovementSkill ":"",
                                    DateTime.Now.Subtract(LastFailureDateTime).TotalSeconds, FailureToUseCounter);
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
			CreateClusterConditions(ref ability);

			//Check if the 4 primary combat conditions are null -- and if the custom condition is not..
			if (ability.SingleUnitCondition.Count == 0 && ability.ClusterConditions.Count == 0 && ability.FcriteriaCombat != null)
				ability.TestCustomCombatConditions = true;
		}


		#region Function Creation Methods
		private static void CreateClusterConditions(ref Skill ability)
		{
			if (ability.ClusterConditions.Count == 0)
				return;

            //if (ObjectCache.CheckFlag(ability.ExecutionType, SkillExecutionFlags.ClusterTarget) || ObjectCache.CheckFlag(ability.ExecutionType, SkillExecutionFlags.ClusterTargetNearest))
            //{
            //    foreach (var condition in ability.ClusterConditions)
            //    {
            //        Func<bool> combatCriteria = condition.Criteria;
            //        CreateLineOfSightTargetCheck(ref combatCriteria, ability);
            //        condition.Criteria = combatCriteria;
            //    }
            //}
		}

		private static void CreateTargetConditions(ref Skill ability)
		{
			//No Conditions Set by default.. (?? May have to verify Ability execution can be Target)
			//-- Ranged Abilities that do not set any single target conditions will never be checked for LOS.
			if (ability.SingleUnitCondition.Count == 0)
			{
				//No Default Conditions Set.. however if Ability uses target as a execution type then we implement the LOS conditions.
				if (ObjectCache.CheckFlag(ability.ExecutionType, SkillExecutionFlags.Target))
					ability.SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None));
				else
					return;
			}

			//Attach Line of Sight Criteria to each entry
			foreach (UnitTargetConditions condition in ability.SingleUnitCondition)
			{
				var combatCriteria = condition.Criteria;
				CreateLineOfSightTargetCheck(ref combatCriteria, ability);
				condition.Criteria = combatCriteria;
			}
		}

		private static void CreateLineOfSightTargetCheck(ref Func<CacheUnit, bool> CombatCriteria, Skill ability)
		{
			if (ability.IsRanged)
			{
				CombatCriteria += (unit) =>
				{
                    if (!unit.IgnoresLOSCheck && unit.IsTargetableAndAttackable)
					{
                        LOSInfo LOSINFO = unit.LineOfSight;
						if (LOSINFO.LastLOSCheckMS > 2000)
						{
							//!LOSINFO.LOSTest(FunkyGame.Hero.Position, true, ServerObjectIntersection: ability.IsProjectile, Flags: NavCellFlags.AllowProjectile)
							if (!LOSINFO.LOSTest(FunkyGame.Hero.Position, true, false, ability.IsProjectile, ability.IsProjectile ? NavCellFlags.AllowProjectile:NavCellFlags.None))
							{
								//Raycast failed.. reset LOS Check -- for valid checking.
								if (!LOSINFO.RayCast.Value || (LOSINFO.ObjectIntersection.HasValue && !LOSINFO.ObjectIntersection.Value))
								{
                                    unit.RequiresLOSCheck = true;
									return false;
								}

								//if (LOSINFO.NavCellProjectile.HasValue && !LOSINFO.NavCellProjectile.Value) //NavCellFlag Walk Failed
								//{
								//	bool MovementException = ((FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterTeleport || FunkyGame.Targeting.Cache.CurrentTarget.IsTransformUnit) && FunkyGame.Targeting.Cache.CurrentUnitTarget.AnimState == AnimationState.Transform);
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
                CombatCriteria += (unit) =>
				{
                    if (!unit.IgnoresLOSCheck && unit.IsTargetableAndAttackable)
					{
                        float radiusDistance = unit.RadiusDistance;
						//Check if within interaction range..
						if (radiusDistance > ability.Range)
						{
							//Verify LOS walk
                            LOSInfo LOSINFO = unit.LineOfSight;
							if (LOSINFO.LastLOSCheckMS > 2000)//||!LOSINFO.NavCellWalk.HasValue)
							{
								if (!LOSINFO.LOSTest(FunkyGame.Hero.Position, true, ServerObjectIntersection: false))
								{
									//bool MovementException=((FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterTeleport||FunkyGame.Targeting.Cache.CurrentTarget.IsTransformUnit)&&FunkyGame.Targeting.Cache.CurrentUnitTarget.AnimState==Zeta.Internals.Actors.AnimationState.Transform);
									//Raycast failed.. reset LOS Check -- for valid checking.
									if (!LOSINFO.RayCast.Value)
                                        unit.RequiresLOSCheck = true;
									//else if (!LOSINFO.NavCellWalk.Value) //NavCellFlag Walk Failed
									//{
									//    bool MovementException = ((FunkyGame.Targeting.Cache.CurrentUnitTarget.MonsterTeleport || FunkyGame.Targeting.Cache.CurrentTarget.IsTransformUnit) && FunkyGame.Targeting.Cache.CurrentUnitTarget.AnimState == Zeta.Internals.Actors.AnimationState.Transform);
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


		#endregion

		#endregion
	}

}