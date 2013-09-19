using System;
using System.Linq;
using FunkyTrinity.ability;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.Internals.Actors.Gizmos;
using Zeta.CommonBot;
using Zeta.Internals.SNO;
using System.Collections.Generic;
using FunkyTrinity.Enums;
using FunkyTrinity.Movement;

namespace FunkyTrinity.Cache
{


		  public class CacheObject : CachedSNOEntry, IComparable
		  {

				#region Constructors
				public CacheObject(int sno, int raguid, int acdguid, Vector3 position, string Name=null)
					 : base(sno)
				{
					 this.RAGUID=raguid;
					 this.NeedsUpdate=true;
					 this.removal_=false;
					 this.BlacklistFlag=BlacklistType.None;
					 this.AcdGuid=acdguid;
					 this.radius_=0f;
					 this.position_=position;
					 this.RequiresLOSCheck=!(base.IgnoresLOSCheck); //require a LOS check initally on a new object!
					 this.lineofsight=new ability.LOSInfo(this);
					 this.LosSearchRetryMilliseconds_=1000;
					 this.PrioritizedDate=DateTime.Today;
					 this.PriorityCounter=0;
					 this.losv3_=Vector3.Zero;
					 this.HandleAsAvoidanceObject=false;
					 this.Properties=TargetProperties.None;
					 //Keep track of each unique RaGuid that is created and uses this SNO during each level.
					 //if (!UsedByRaGuids.Contains(RAGUID)) UsedByRaGuids.Add(RAGUID);
				}
				///<summary>
				///Used to create objects to use as temp targeting
				///</summary>
				public CacheObject(Vector3 thisposition, TargetType thisobjecttype=TargetType.None, double thisweight=0, string name=null, float thisradius=0f, int thisractorguid=-1, int thissno=0)
					 : base(thissno)
				{
					 this.RAGUID=thisractorguid;
					 this.position_=thisposition;
					 this.weight_=thisweight;
					 this.radius_=thisradius;
					 this.BlacklistFlag=BlacklistType.None;
					 this.targetType=thisobjecttype;
					 this.InternalName=name;
					 this.HandleAsAvoidanceObject=false;
				}
				///<summary>
				///Used to recreate from temp into obstacle object.
				///</summary>
				public CacheObject(CacheObject parent)
					 : base(parent)
				{
					 this.AcdGuid=parent.AcdGuid;
					 this.BlacklistFlag=parent.BlacklistFlag;
					 this.BlacklistLoops_=parent.BlacklistLoops_;
					 this.gprect_=parent.gprect_;
					 this.InteractionAttempts=parent.InteractionAttempts;
					 this.lineofsight=new ability.LOSInfo(this);
					 this.LoopsUnseen_=parent.LoopsUnseen_;
					 this.losv3_=parent.losv3_;
					 this.LosSearchRetryMilliseconds_=parent.LosSearchRetryMilliseconds_;
					 this.NeedsRemoved=parent.NeedsRemoved;
					 this.NeedsUpdate=parent.NeedsUpdate;
					 this.PrioritizedDate=parent.PrioritizedDate;
					 this.PriorityCounter=parent.PriorityCounter;
					 this.position_=parent.Position;
					 this.radius_=parent.Radius;
					 this.RAGUID=parent.RAGUID;
					 this.ref_DiaObject=parent.ref_DiaObject;
					 this.removal_=parent.removal_;
					 this.RequiresLOSCheck=parent.RequiresLOSCheck;
					 this.SummonerID=parent.SummonerID;
					 this.weight_=parent.Weight;
					 this.HandleAsAvoidanceObject=parent.HandleAsAvoidanceObject;
					 this.Properties=parent.Properties;
				}
				#endregion

				public int? AcdGuid { get; set; }
				public int RAGUID { get; set; }

				private double weight_;
				public virtual double Weight { get { return weight_; } set { weight_=value; } }
				private float radius_;
				public virtual float Radius { get { return radius_; } set { radius_=value; } }

				public bool HandleAsAvoidanceObject { get; set; }

				public virtual TargetProperties Properties { get; set; }

				public virtual void UpdateProperties()
				{
					 Properties=TargetProperties.None;
				}

				public AnimationState AnimState
				{
					 //Return live data.
					 get
					 {
						  using (ZetaDia.Memory.AcquireFrame())
						  {
								try
								{
									 return (this.ref_DiaObject.CommonData.AnimationState);
								} catch (NullReferenceException)
								{
									 return AnimationState.Invalid;
								}
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
				public virtual Vector3 Position { get { return position_; } set { position_=value; } }

				private GridPoint pointposition_;
				public GridPoint PointPosition
				{
					 get
					 {
						  if (positionUpdated)
						  {
								pointposition_=position_;
								positionUpdated=false;
						  }

						  return pointposition_;
					 }
				}

				public virtual float CentreDistance
				{
					 get
					 {
						  return Bot.Character.Position.Distance(this.Position);
					 }
				}

				public virtual float RadiusDistance
				{
					 get
					 {
						  return Math.Max(0f, CentreDistance-this.Radius);
					 }
				}

				private DateTime lastUpdatedPosition=DateTime.Today;
				private bool positionUpdated=true;

				public virtual void UpdatePosition(bool force=false)
				{
					 if (!force&&DateTime.Now.Subtract(lastUpdatedPosition).TotalMilliseconds<300)
						  return;

					 using (ZetaDia.Memory.AcquireFrame())
					 {
						  try
						  {
								this.Position=this.ref_DiaObject.Position;
						  } catch (NullReferenceException)
						  {
								Logging.WriteVerbose("Safely Handled Updating Position for Object {0}", this.InternalName);
						  }
						  this.lastUpdatedPosition=DateTime.Now;
						  this.positionUpdated=true;

						  //Update Properties
						  if (this.RadiusDistance<5f)
						  {
								if (!AbilityUsablityTests.CheckTargetPropertyFlag(this.Properties,TargetProperties.CloseDistance))
									 this.Properties|=TargetProperties.CloseDistance;
						  }
						  else
						  {
								if (AbilityUsablityTests.CheckTargetPropertyFlag(this.Properties,TargetProperties.CloseDistance))
									 this.Properties&=TargetProperties.CloseDistance;
						  }
					 }
				}
				///<summary>
				///Returns adjusted position using direction of current bot and radius of object to reduce distance.
				///</summary>
				public Vector3 BotMeleeVector
				{
					 get
					 {
						  float distance=this.ActorSphereRadius.HasValue?this.ActorSphereRadius.Value
												:this.CollisionRadius.HasValue?this.CollisionRadius.Value:this.Radius;

						  Vector3 GroundedVector=new Vector3(this.position_.X, this.position_.Y, this.position_.Z+this.radius_/2);
							return MathEx.GetPointAt(GroundedVector, (distance*1.15f), Navigation.FindDirection(GroundedVector, Bot.Character.Position, true));
					 }
				}

				private GPRectangle gprect_;
				internal virtual GPRectangle GPRect
				{
					 get
					 {
						  //Create new one..
						  if (gprect_==null)
								 gprect_=new GPRectangle(Position, (int)(Math.Sqrt(this.ActorSphereRadius.Value)));

						  return gprect_;
					 }
					 set { gprect_=value; }
				}
				#endregion


				#region Blacklist, Removal, and Valid
				private int LoopsUnseen_=0;
				///<summary>
				///Counter which increases when object is not seen during the refresh stage.
				///</summary>
				public int LoopsUnseen
				{
					 get { return LoopsUnseen_; }
					 set { LoopsUnseen_=value; }
				}

				private int BlacklistLoops_=0;
				///<summary>
				///Amount of loops this object is being ignored during the Usable Object iteration. If set to -1 it will be ignored indefinitely.
				///</summary>
				public int BlacklistLoops
				{
					 get { return BlacklistLoops_; }
					 set { BlacklistLoops_=value; }
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
						  removal_=value;
						  //This helps reduce code by flagging this here instead of after everytime we flag removal of an object!
						  if (value==true) Bot.Refresh.RemovalCheck=true;
					 }
				}
				internal bool removal_;

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
				public virtual bool IsStillValid()
				{
					 //Check DiaObject first
					 if (ref_DiaObject==null||!ref_DiaObject.IsValid||ref_DiaObject.BaseAddress==IntPtr.Zero)
						  return false;

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
					 set { requiresLOSCheck=value; }
				}

				private ability.LOSInfo lineofsight;
				public ability.LOSInfo LineOfSight
				{
					 get { return lineofsight; }
				}

				public DateTime LastLOSSearch { get; set; }
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
					 set { LosSearchRetryMilliseconds_=value; }
				}


				private Vector3 losv3_=Vector3.Zero;
				private DateTime losv3LastChanged=DateTime.Today;
				///<summary>
				///Used during targeting as destination vector
				///</summary>
				public Vector3 LOSV3
				{
					 get
					 {
						  //invalidate los vector after 4 seconds
						  if (DateTime.Now.Subtract(losv3LastChanged).TotalSeconds>4)
						  {
								losv3_=Vector3.Zero;
						  }

						  return losv3_;
					 }
					 set { losv3_=value; losv3LastChanged=DateTime.Now; }
				}

				private Zeta.Navigation.MoveResult lastLOSmoveresult=Zeta.Navigation.MoveResult.Moved;
				public Zeta.Navigation.MoveResult LastLOSMoveResult
				{
					 get { return lastLOSmoveresult; }
					 set { lastLOSmoveresult=value; }
				}

				public bool UsingLOSV3
				{
					 get
					 {
						  return losv3_!=Vector3.Zero;
					 }
				}
				#endregion


				public virtual bool BotIsFacing()
				{
					 Vector3 NormalizedVector=this.Position;
					 NormalizedVector.Z=0f; //Use Zero for Z -- this helps with units that hover..
					 NormalizedVector.Normalize();

					 Vector3 BotPositionNormalized=Bot.Character.Position;
					 BotPositionNormalized.Z=0f;
					 BotPositionNormalized.Normalize();

					 float angleDegrees=Vector3.AngleBetween(BotPositionNormalized, NormalizedVector);

					 return (angleDegrees<=0.0045||angleDegrees>0.0315);

				}
				public virtual bool BotIsFacing(Vector3 DestinationVector)
				{
					 Vector3 NormalizedVector=this.Position;
					 NormalizedVector.Z=0f;
					 NormalizedVector.Normalize();

					 Vector3 NormalizedBotDestination=Vector3.NormalizedDirection(Bot.Character.Position, DestinationVector);
					 NormalizedBotDestination.Z=0f;

					 float angleDegrees=Vector3.AngleBetween(NormalizedVector, NormalizedBotDestination);
					 return (angleDegrees<=0.0045||angleDegrees>0.0315);
				}
				///<summary>
				///Check if the distance between the object and vector is less than range. (Factors the radius of object too)
				///</summary>
				public bool IsPositionWithinRange(Vector3 V, float Range)
				{
					 return Math.Max(0f, this.Position.Distance(V)-this.Radius)<=Range;
				}

				public override bool UpdateData(DiaObject thisObj, int RaGuid)
				{
					 //Reference the object (just in case!)
					 if (this.ref_DiaObject==null)
						  this.ref_DiaObject=thisObj;

					 return base.UpdateData(thisObj, this.RAGUID);
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
						  //Blacklist loop counter checks
						  if (this.BlacklistLoops_<0) return false;
						  if (this.BlacklistLoops_>0)
						  {
								this.BlacklistLoops_--;
								return false;
						  }

						  //Skip objects if not seen during cache refresh
						  if (this.LoopsUnseen_>0) return false;

						  //Check if we are doing something important.. if so we only want to check units!
							if (Bot.IsInNonCombatBehavior&&(!this.targetType.HasValue||!(TargetType.Unit|TargetType.Item|TargetType.Gold|TargetType.Globe).HasFlag(this.targetType.Value)))
								return false;

						  //Validate refrence still remains
						  if (!this.IsStillValid())
						  {
								//Flag for removal, and blacklist
								this.NeedsRemoved=true;
								this.BlacklistFlag=BlacklistType.Temporary;
								return false;
						  }


						  //Far away object?
						  if (this.CentreDistance>300f)
						  {
								this.NeedsRemoved=true;
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
					 if (Bot.Combat.PrioritizedRAGUIDs.Contains(this.RAGUID))
					 {
						  this.PriorityCounter=this.PriorityCounter+1;
						  this.PrioritizedDate=DateTime.Now;
							Bot.Combat.PrioritizedRAGUIDs.Remove(this.RAGUID);
					 }

					 // Just to make sure each one starts at 0 weight...
					 this.Weight=0d;

					 if (this.PriorityCounter>0)
					 {
						  //adjust weight -- based upon close they are
						  this.Weight+=(25000*this.PriorityCounter)-(100*this.RadiusDistance);

						  //decrease priority
						  if (this.LastPriortized>(this.PriorityCounter*500)+2500)
								this.PriorityCounter=this.PriorityCounter-1;
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
					 this.DistanceFromTarget=Bot.Character.Position.Distance(this.Position);
					 return (this.Radius<=0f||this.DistanceFromTarget<=this.Radius);
				}
				///<summary>
				///Called during target handling interaction stage. (Derieved classes override this)
				///</summary>
				public virtual RunStatus Interact()
				{
					 this.InteractionAttempts++;

					 // If we've tried interacting too many times, blacklist this for a while
					 if (this.InteractionAttempts>3)
					 {
						  this.NeedsRemoved=true;
						  this.BlacklistFlag=BlacklistType.Temporary;
					 }
					 this.BlacklistLoops_=10;

					 return RunStatus.Success;
				}


				public new CacheObject Clone()
				{
					 return (CacheObject)this.MemberwiseClone();
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
						  CacheObject p=(CacheObject)obj;
						  return this.RAGUID==p.RAGUID;
					 }
				}

				public override int GetHashCode()
				{
					 return this.RAGUID;
				}

				public int CompareTo(object obj)
				{
					 if (obj==null) return 1;
					 CacheObject other=obj as CacheObject;

					 if (other!=null)
						  return other.CentreDistance.CompareTo(this.CentreDistance);
					 else
						  return 1;
				}

				public override string DebugString
				{
					 get
					 {
						  return String.Format("RAGUID {0}: \r\n {1} Distance (Centre{2} / Radius{3}) \r\n ReqLOS={4} -- {5} -- [LOSV3: {6}] \r\n BotFacing={7} \r\n BlackListLoops[{8}]",
								this.RAGUID.ToString(), base.DebugString, this.CentreDistance.ToString(), this.RadiusDistance.ToString(),
								this.RequiresLOSCheck.ToString(), this.LineOfSight!=null?String.Format("-- {0} --",this.LineOfSight.DebugString):"", this.LOSV3.ToString(),
								this.BotIsFacing().ToString(), this.BlacklistLoops.ToString());
					 }
				}

		  }
	 
}