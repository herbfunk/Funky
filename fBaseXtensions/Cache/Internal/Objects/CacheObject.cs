﻿using System;
using System.Collections.Generic;
using System.Globalization;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Cache.Internal.Blacklist;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Navigation.Gridpoint;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;

namespace fBaseXtensions.Cache.Internal.Objects
{


	public class CacheObject : CachedSNOEntry, IComparable
	{

		#region Constructors
		public CacheObject(int sno, int raguid, int acdguid, Vector3 position, string Name = null)
			: base(sno)
		{
			WorldID = FunkyGame.Hero.CurrentWorldID;
			LevelAreaID = FunkyGame.Hero.iCurrentLevelID;
			CreationTime=DateTime.Now;
			RAGUID = raguid;
			NeedsUpdate = true;
			removal_ = false;
			BlacklistFlag = BlacklistType.None;
			AcdGuid = acdguid;
			radius_ = 0f;
			position_ = position;
			QuestMonster = false;
			RequiresLOSCheck = !(base.IgnoresLOSCheck); //require a LOS check initally on a new object!
			lineofsight = new LOSInfo(this);
			LosSearchRetryMilliseconds_ = 1000;
			PrioritizedDate = DateTime.Today;
			PriorityCounter = 0;
			losv3_ = Vector3.Zero;
			HandleAsAvoidanceObject = false;
			Properties = TargetProperties.None;
			SkillsUsedOnObject= new Dictionary<SNOPower, DateTime>();
			//Keep track of each unique RaGuid that is created and uses this SNO during each level.
			//if (!UsedByRaGuids.Contains(RAGUID)) UsedByRaGuids.Add(RAGUID);
		}
		///<summary>
		///Used to create objects to use as temp targeting
		///</summary>
		public CacheObject(Vector3 thisposition, TargetType thisobjecttype = TargetType.None, double thisweight = 0, string name = null, float thisradius = 0f, int thisractorguid = -1, int thissno = 0)
			: base(thissno)
		{
			WorldID = FunkyGame.Hero.CurrentWorldID;
			LevelAreaID = FunkyGame.Hero.iCurrentLevelID;
			CreationTime = DateTime.Now;
			RAGUID = thisractorguid;
			position_ = thisposition;
			weight_ = thisweight;
			radius_ = thisradius;
			BlacklistFlag = BlacklistType.None;
			targetType = thisobjecttype;
			InternalName = name;
			HandleAsAvoidanceObject = false;
		}
		///<summary>
		///Used to recreate from temp into obstacle object.
		///</summary>
		public CacheObject(CacheObject parent)
			: base(parent)
		{
			WorldID = parent.WorldID;
			LevelAreaID = parent.LevelAreaID;
			CreationTime=parent.CreationTime;
			AcdGuid = parent.AcdGuid;
			BlacklistFlag = parent.BlacklistFlag;
			BlacklistLoops_ = parent.BlacklistLoops_;
			gprect_ = parent.gprect_;
			InteractionAttempts = parent.InteractionAttempts;
			lineofsight = new LOSInfo(this);
			LoopsUnseen_ = parent.LoopsUnseen_;
			losv3_ = parent.losv3_;
			LosSearchRetryMilliseconds_ = parent.LosSearchRetryMilliseconds_;
			NeedsRemoved = parent.NeedsRemoved;
			NeedsUpdate = parent.NeedsUpdate;
			PrioritizedDate = parent.PrioritizedDate;
			PriorityCounter = parent.PriorityCounter;
			position_ = parent.Position;
			radius_ = parent.Radius;
			RAGUID = parent.RAGUID;
			ref_DiaObject = parent.ref_DiaObject;
			removal_ = parent.removal_;
			RequiresLOSCheck = parent.RequiresLOSCheck;
			SummonerID = parent.SummonerID;
			weight_ = parent.Weight;
			HandleAsAvoidanceObject = parent.HandleAsAvoidanceObject;
			Properties = parent.Properties;
			QuestMonster=parent.QuestMonster;
			_skillsUsedOnObject = new Dictionary<SNOPower, DateTime>();
		}

		#endregion

		public int? AcdGuid { get; set; }
		public int RAGUID { get; set; }
		public readonly int WorldID, LevelAreaID;

		public readonly DateTime CreationTime;
		public double CreationTimeSeconds
		{
			get { return DateTime.Now.Subtract(CreationTime).TotalSeconds; }
		}
		internal bool DebugDataChecked;
		private double weight_;
		public virtual double Weight { get { return weight_; } set { weight_ = value; } }
		private float radius_;
		public virtual float Radius { get { return radius_; } set { radius_ = value; } }

		public bool HandleAsAvoidanceObject { get; set; }

		public virtual TargetProperties Properties { get; set; }

		public virtual void UpdateProperties()
		{
			Properties = TargetProperties.None;
		}

		private AnimationState _animationState = AnimationState.Invalid;
		public AnimationState AnimState { get { return _animationState; } set { _animationState = value; } }

		private SNOAnim _snoAnim = SNOAnim.Invalid;
		public SNOAnim SnoAnim { get { return _snoAnim; } set { _snoAnim = value; } }
		public void UpdateAnimationState()
		{
			//Return live data.
			using (ZetaDia.Memory.AcquireFrame())
			{
				try
				{
					_animationState = ref_DiaObject.CommonData.AnimationState;
				}
				catch (Exception)
				{
					_animationState = AnimationState.Invalid;
				}
			}
		}
		public void UpdateSNOAnim()
		{
			//Return live data.
			using (ZetaDia.Memory.AcquireFrame())
			{
				try
				{
					_snoAnim = ref_DiaObject.CommonData.CurrentAnimation;
				}
				catch (Exception)
				{
					_snoAnim = SNOAnim.Invalid;
				}
			}
		}

		///<summary>
		///Used only if the object is a summonable pet.
		///</summary>
		public int? SummonerID { get; set; }
		///<summary>
		///The number of interaction attempts made.
		///</summary>
		public int InteractionAttempts { get; set; }
		///<summary>
		///References the actual DiaObject
		///</summary>
		public DiaObject ref_DiaObject { get; set; }

		public bool QuestMonster { get; set; }

		private Dictionary<SNOPower,DateTime> _skillsUsedOnObject;
		public Dictionary<SNOPower, DateTime> SkillsUsedOnObject
		{
			get { return _skillsUsedOnObject; }
			set { _skillsUsedOnObject = value; }
		}

		public int PriorityCounter { get; set; }
		public DateTime PrioritizedDate { get; set; }
		///<summary>
		///Milliseconds since last prioritized.
		///</summary>
		public double LastPriortized
		{
			get
			{
				return DateTime.Now.Subtract(PrioritizedDate).TotalMilliseconds;
			}
		}


		#region Position Properties & Methods
		private Vector3 position_;
		public virtual Vector3 Position { get { return position_; } set { position_ = value; } }

		private GridPoint pointposition_;
		public GridPoint PointPosition
		{
			get
			{
				if (positionUpdated)
				{
					pointposition_ = position_;
					positionUpdated = false;
				}

				return pointposition_;
			}
		}

		public virtual float CentreDistance
		{
			get
			{
				return FunkyGame.Hero.Position.Distance(Position);
			}
		}

		public virtual float RadiusDistance
		{
			get
			{
				return Math.Max(0f, CentreDistance - Radius);
			}
		}

		private DateTime lastUpdatedPosition = DateTime.Today;
		private bool positionUpdated = true;

		public virtual void UpdatePosition(bool force = false)
		{
            if (!force || (UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Stationary)))
			{
				double lastUpdated=DateTime.Now.Subtract(lastUpdatedPosition).TotalMilliseconds;

				if (UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Stationary) && lastUpdated < 100000)
					return;

				if (lastUpdated < 150)
					return;
			}

			using (ZetaDia.Memory.AcquireFrame())
			{

				try
				{
					Position = ref_DiaObject.Position;
				}
				catch
				{
					Logger.Write(LogLevel.Cache, "Safely Handled Updating Position for Object {0}", InternalName);
				}
				lastUpdatedPosition = DateTime.Now;
				positionUpdated = true;
			}

			//Special Position Adjustment!
			if (UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Stationary))
			{
				UpdateRotation();

				float distance = ActorSphereRadius.HasValue ? ActorSphereRadius.Value
									  : CollisionRadius.HasValue ? CollisionRadius.Value : Radius;

				Vector3 curPosition = Position;
				Position=MathEx.GetPointAt(curPosition, distance, Rotation);
			}
		}
		///<summary>
		///Returns adjusted position using direction of current bot and radius of object to reduce distance.
		///</summary>
		public Vector3 BotMeleeVector
		{
			get
			{
				float distance = ActorSphereRadius.HasValue ? ActorSphereRadius.Value
									  : CollisionRadius.HasValue ? CollisionRadius.Value : Radius;

				Vector3 GroundedVector = new Vector3(position_.X, position_.Y, position_.Z + radius_ / 2);
				return MathEx.GetPointAt(GroundedVector, (distance * 1.15f), Navigation.Navigation.FindDirection(GroundedVector, FunkyGame.Hero.Position, true));
			}
		}

		private GPRectangle gprect_;
		internal virtual GPRectangle GPRect
		{
			get
			{
				//Create new one..
				if (gprect_ == null)
					gprect_ = new GPRectangle(Position, (int)(Math.Sqrt(ActorSphereRadius.Value)));

				return gprect_;
			}
			set { gprect_ = value; }
		}

		private float rotation_;
		internal virtual float Rotation
		{
			get { return rotation_; }
			set { rotation_ = value; }
		}
		internal void UpdateRotation()
		{
			using (ZetaDia.Memory.AcquireFrame())
			{

				try
				{
					Rotation = ref_DiaObject.Movement.Rotation;
				}
				catch (Exception)
				{
					Logger.Write(LogLevel.Cache, "Safely Handled Updating Rotation for Object {0}", InternalName);
				}
			}
		}
		#endregion


		#region Blacklist, Removal, and Valid
		private TargetingIgnoreTypes _ignoredtype = TargetingIgnoreTypes.None;
		/// <summary>
		/// Reason why object failed during targeting valid check
		/// </summary>
		public TargetingIgnoreTypes IgnoredType
		{
			get { return _ignoredtype; }
			set { _ignoredtype = value; }
		}

		private int LoopsUnseen_ = 0;
		///<summary>
		///Counter which increases when object is not seen during the refresh stage.
		///</summary>
		public int LoopsUnseen
		{
			get { return LoopsUnseen_; }
			set { LoopsUnseen_ = value; }
		}

		private int BlacklistLoops_ = 0;
		///<summary>
		///Amount of loops this object is being ignored during the Usable Object iteration. If set to -1 it will be ignored indefinitely.
		///</summary>
		public int BlacklistLoops
		{
			get { return BlacklistLoops_; }
			set { BlacklistLoops_ = value; }
		}

		///<summary>
		///Flag that determines if the object will be updated during Refresh (Live Data).
		///</summary>
		public bool NeedsUpdate { get; set; }
		///<summary>
		///Flag that determines if the object should be removed from the collection.
		///</summary>
		public bool NeedsRemoved
		{
			get
			{
				return removal_;
			}
			set
			{
				removal_ = value;
				//This helps reduce code by flagging this here instead of after everytime we flag removal of an object!
				if (value == true) ObjectCache.RemovalCheck = true;
			}
		}
		internal bool removal_;

	    public RemovalTypes RemovalType
	    {
            get { return _removaltype; }
            set { _removaltype = value; }
	    }
        private RemovalTypes _removaltype = RemovalTypes.Unknown;

		///<summary>
		///This is evaluated during removal and when set to something other than none it will be blacklisted.
		///</summary>
		public BlacklistType BlacklistFlag { get; set; }
		///<summary>
		///All derieved objects override this and will return false.
		///</summary>
		public virtual bool ObjectShouldBeRecreated
		{
			get
			{
				return true;
			}
		}

	    internal bool _IsStillValid = true;
		public virtual bool IsStillValid()
		{
			//Check DiaObject first
		    if (ref_DiaObject == null || !ref_DiaObject.IsValid || ref_DiaObject.BaseAddress == IntPtr.Zero)
		    {
                //Logger.Write(LogLevel.Cache, "Reference DiaObject not valid for {0}", DebugStringSimple);
		        _IsStillValid = false;
                return false;
		    }

		    if (ref_DiaObject.CommonData == null || !ref_DiaObject.CommonData.IsValid || ref_DiaObject.CommonData.ACDGuid == -1)
		    {
                //Logger.Write(LogLevel.Cache, "Reference DiaObject -- CommonData not valid for {0}", DebugStringSimple);
                _IsStillValid = false;
                return false;
		    }

            _IsStillValid = true;
			return true;
		}
		#endregion


		#region Line Of Sight Properties & Methods
		private bool requiresLOSCheck;
		///<summary>
		///Line Of Sight Checking Variable
		///</summary>
		public virtual bool RequiresLOSCheck
		{
			get { return requiresLOSCheck; }
			set { requiresLOSCheck = value; }
		}

		private LOSInfo lineofsight;
		public LOSInfo LineOfSight
		{
			get { return lineofsight; }
		}

		private DateTime lastLOSSearch = DateTime.Today;
		public DateTime LastLOSSearch
		{
			get { return lastLOSSearch; }
			set { lastLOSSearch = value; }
		}
		///<summary>
		///Last time we preformed a LOS vector search
		///</summary>
		public double LastLOSSearchMS
		{
			get
			{
				return (DateTime.Now.Subtract(LastLOSSearch).TotalMilliseconds);
			}
		}
		private double LosSearchRetryMilliseconds_;
		public double LosSearchRetryMilliseconds
		{
			get { return LosSearchRetryMilliseconds_; }
			set { LosSearchRetryMilliseconds_ = value; }
		}


		private Vector3 losv3_ = Vector3.Zero;
		private DateTime losv3LastChanged = DateTime.Today;
		///<summary>
		///Used during targeting as destination vector
		///</summary>
		public Vector3 LOSV3
		{
			get
			{
				//invalidate los vector after 4 seconds
				if (DateTime.Now.Subtract(losv3LastChanged).TotalSeconds > 4)
				{
					losv3_ = Vector3.Zero;
				}

				return losv3_;
			}
			set { losv3_ = value; losv3LastChanged = DateTime.Now; }
		}

		private MoveResult lastLOSmoveresult = MoveResult.Moved;
		public MoveResult LastLOSMoveResult
		{
			get { return lastLOSmoveresult; }
			set { lastLOSmoveresult = value; }
		}

		public bool UsingLOSV3
		{
			get
			{
				return losv3_ != Vector3.Zero;
			}
		}
		#endregion


		public virtual bool BotIsFacing()
		{
			Vector3 NormalizedVector = Position;
			NormalizedVector.Z = 0f; //Use Zero for Z -- this helps with units that hover..
			NormalizedVector.Normalize();

			Vector3 BotPositionNormalized = FunkyGame.Hero.Position;
			BotPositionNormalized.Z = 0f;
			BotPositionNormalized.Normalize();

			float angleDegrees = Vector3.AngleBetween(BotPositionNormalized, NormalizedVector);

			return (angleDegrees <= 0.0045 || angleDegrees > 0.0315);

		}
		public virtual bool BotIsFacing(Vector3 DestinationVector)
		{
			Vector3 NormalizedVector = Position;
			NormalizedVector.Z = 0f;
			NormalizedVector.Normalize();

			Vector3 NormalizedBotDestination = Vector3.NormalizedDirection(FunkyGame.Hero.Position, DestinationVector);
			NormalizedBotDestination.Z = 0f;

			float angleDegrees = Vector3.AngleBetween(NormalizedVector, NormalizedBotDestination);
			return (angleDegrees <= 0.0045 || angleDegrees > 0.0315);
		}
		public bool IsFacingBot()
		{
			Vector3 NormalizedVector = Vector3.NormalizedDirection(Position, MathEx.GetPointAt(Position, 10f, Rotation)); 
			NormalizedVector.Z = 0f;
			NormalizedVector.Normalize();

			Vector3 BotPositionNormalized = FunkyGame.Hero.Position;
			BotPositionNormalized.Z = 0f;
			BotPositionNormalized.Normalize();

			float angleDegrees = Vector3.AngleBetween(NormalizedVector, BotPositionNormalized);
			return (angleDegrees <= 0.0045 || angleDegrees > 0.0315);
		}
		///<summary>
		///Check if the distance between the object and vector is less than range. (Factors the radius of object too)
		///</summary>
		public bool IsPositionWithinRange(Vector3 V, float Range)
		{
			return Math.Max(0f, Position.Distance(V) - Radius) <= Range;
		}

		public override bool UpdateData(DiaObject thisObj, int RaGuid)
		{
			//Reference the object (just in case!)
			if (ref_DiaObject == null)
				ref_DiaObject = thisObj;

			return base.UpdateData(thisObj, RAGUID);
		}
		///<summary>
		///Updates the object values (Used by derieved classes only!)
		///</summary>
		public virtual bool UpdateData()
		{
			return true;
		}

		///<summary>
		///Base Testing Method for object targeting.
		///</summary>
		public virtual bool ObjectIsValidForTargeting
		{
			get
			{
				//Reset Ignore Reason
				IgnoredType = TargetingIgnoreTypes.None;

				if (NeedsRemoved)
					return false;

				//Blacklist loop counter checks
				if (BlacklistLoops_ < 0)
				{
					//IgnoredType=TargetingIgnoreTypes.Blacklisted;
					return false;
				}
				if (BlacklistLoops_ > 0)
				{
					//IgnoredType = TargetingIgnoreTypes.Blacklisted;
					BlacklistLoops_--;
					return false;
				}

				//Skip objects if not seen during cache refresh
				if (LoopsUnseen_ > 0)
				{
					IgnoredType = TargetingIgnoreTypes.CacheUnseen;
					return false;
				}
			
				//Check if we are doing something important.. if so we only want to check units!
				if (FunkyGame.IsInNonCombatBehavior)
				{
					TargetType typesValid = TargetType.Unit | TargetType.Item | TargetType.Gold | TargetType.Globe | TargetType.PowerGlobe;

					if (FunkyGame.Profile.ProfileBehaviorIsInteractive && !FunkyGame.Hero.bIsInTown)
					{
						typesValid |= TargetType.Door | TargetType.Barricade;
					}

					if (!ObjectCache.CheckFlag(targetType.Value, typesValid))
					{
						IgnoredType = TargetingIgnoreTypes.IgnoredTargetType;
						return false;
					}
				}

				//Validate refrence still remains
				if (!IsStillValid())
				{
					//Flag for removal, and blacklist
					NeedsRemoved = true;
					BlacklistFlag = BlacklistType.Temporary;
					return false;
				}


				//Far away object?
				if (CentreDistance > 500f)
				{
					NeedsRemoved = true;
					return false;
				}

				return true;
			}
		}

		///<summary>
		///Base Weighting Method
		///</summary>
		public virtual void UpdateWeight()
		{
			//Prioritized (Blocked/Intersecting Objects)
			if (FunkyGame.Navigation.PrioritizedRAGUIDs.Contains(RAGUID))
			{
				PriorityCounter = PriorityCounter + 1;
				PrioritizedDate = DateTime.Now;
				FunkyGame.Navigation.PrioritizedRAGUIDs.Remove(RAGUID);
			}

			// Just to make sure each one starts at 0 weight...
			Weight = 0d;

			if (PriorityCounter > 0)
			{
				//adjust weight -- based upon close they are
				Weight += (25000 * PriorityCounter) - (100 * RadiusDistance);

				//decrease priority
				if (LastPriortized > (PriorityCounter * 500) + 2500)
					PriorityCounter = PriorityCounter - 1;
			}

			if (QuestMonster)
			{
				Weight += 10000;
			}
		}

		/// <summary>
		/// Maximum range required for targeting.
		/// </summary>
		public virtual int InteractionRange
		{
			get
			{
				return 0;
			}
		}

		///<summary>
		///Validate the object is still capable of interaction. (Used by derieved classes only!)
		///</summary>
		public virtual bool CanInteract()
		{
			return true;
		}

		public virtual bool IsZDifferenceValid
		{
			get
			{
				return true;
			}

		}
		///<summary>
		///Used to quickly check if object is considered special. (Used by derieved classes only!)
		///</summary>
		public virtual bool ObjectIsSpecial
		{
			get
			{
				return false;
			}
		}
		///<summary>
		///Value is set when object calls WithinInteractionRange.
		///</summary>
		public float DistanceFromTarget { get; set; }
		///<summary>
		///Called during target handling movement check stage. (Derieved classes override this)
		///</summary>
		public virtual bool WithinInteractionRange()
		{
			DistanceFromTarget = FunkyGame.Hero.Position.Distance(Position);
			return (Radius <= 0f || DistanceFromTarget <= Radius);
		}
		///<summary>
		///Called during target handling interaction stage. (Derieved classes override this)
		///</summary>
		public virtual RunStatus Interact()
		{
			InteractionAttempts++;

			// If we've tried interacting too many times, blacklist this for a while
			if (InteractionAttempts > 3)
			{
				NeedsRemoved = true;
				BlacklistFlag = BlacklistType.Temporary;
			}
			BlacklistLoops_ = 10;

			return RunStatus.Success;
		}


		public new CacheObject Clone()
		{
			return (CacheObject)MemberwiseClone();
		}

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			else
			{
				CacheObject p = (CacheObject)obj;
				return RAGUID == p.RAGUID;
			}
		}

		public override int GetHashCode()
		{
			return RAGUID;
		}

		public int CompareTo(object obj)
		{
			if (obj == null) return 1;
			CacheObject other = obj as CacheObject;

			if (other != null)
				return other.CentreDistance.CompareTo(CentreDistance);
			else
				return 1;
		}

		public override string DebugString
		{
			get
			{
				return String.Format("{1}" +
				                     "RAGUID {0}: Created {13} seconds ago" +
                                     "\r\nPosition[{14}] Distance (Centre{2} / Radius{3}) Weight {16}" +
				                     "\r\nSnoAnim={9} -- AnimState={10}" +
				                     "\r\nReqLOS={4} -- {5} -- [LOSV3: {6}]" +
				                     "\r\nBotFacing={7}" +
									 "\r\nBlackListLoops[{8}] UnseenLoops[{11}]\r\n" +
                                     "IgnoreReason: {12} NeedsRemoved: {15} RemovalType: {17} IsStillValid: {18}\r\n" ,
					  RAGUID.ToString(CultureInfo.InvariantCulture), base.DebugString, CentreDistance.ToString(CultureInfo.InvariantCulture), RadiusDistance.ToString(CultureInfo.InvariantCulture),
					  RequiresLOSCheck, LineOfSight != null ? String.Format("-- {0} --", LineOfSight.DebugString) : "", LOSV3,
					  BotIsFacing(), BlacklistLoops.ToString(CultureInfo.InvariantCulture), SnoAnim, AnimState, LoopsUnseen, IgnoredType,
					  DateTime.Now.Subtract(CreationTime).TotalSeconds, Position.ToString(), NeedsRemoved, Weight, RemovalType, _IsStillValid);
			}
		}


	}

}