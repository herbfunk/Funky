using System;
using System.Linq;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;

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

        public struct Ability
        {
				private SNOPower Power_;
				public SNOPower Power
				{
					 get { return Power_; }
					 set { Power_=value; }
				}

				private float MinimumRange_;
				public float MinimumRange
				{
					 get { return MinimumRange_; }
					 set { MinimumRange_=value; }
				}

				private Vector3 TargetPosition_;
				public Vector3 TargetPosition
				{
					 get { return TargetPosition_; }
					 set { TargetPosition_=value; }
				}

				private int WorldID_;
				public int WorldID
				{
					 get { return WorldID_; }
					 set { WorldID_=value; }
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

				private bool WaitWhileAnimating_;
				public bool WaitWhileAnimating
				{
					 get { return WaitWhileAnimating_; }
					 set { WaitWhileAnimating_=value; }
				}

				private bool? SuccessUsed_;
				public bool? SuccessUsed
				{
					 get { return SuccessUsed_; }
					 set { SuccessUsed_=value; }
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
								return MinimumRange_;
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

				public Zeta.CommonBot.PowerManager.CanCastFlags CanCastFlags;

            public Ability(SNOPower thispower, float thisrange, Vector3 thisloc, int thisworld, int thisguid, int waitloops, int afterloops, bool repeat)
            {
                Power_ = thispower;
                MinimumRange_ = thisrange;
                TargetPosition_ = thisloc;
                WorldID_ = thisworld;
                TargetRAGUID_ = thisguid;
                WaitLoopsBefore_ = waitloops;
                WaitLoopsAfter_ = afterloops;
                WaitWhileAnimating_ = repeat;
					 CanCastFlags=Zeta.CommonBot.PowerManager.CanCastFlags.None;
					 SuccessUsed_=null;
            }

				public void UsePower()
				{
					 Zeta.CommonBot.PowerManager.CanCast(this.Power_, out CanCastFlags);
					 SuccessUsed_=ZetaDia.Me.UsePower(this.Power_, this.TargetPosition_, this.WorldID_, this.TargetRAGUID_);
				}
				///<summary>
				///Sets values related to ability usage and resets the SNOPower
				///</summary>
				public void SuccessfullyUsed()
				{
					 dictAbilityLastUse[Power_]=DateTime.Now;
					 lastGlobalCooldownUse=DateTime.Now;
					 Power_=SNOPower.None;
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
						  return this.Power_==p.Power_;
					 }
				}
				public override int GetHashCode()
				{
					 return (int)this.Power_;
				}
        }
    }
}