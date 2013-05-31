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
				public SNOPower Power;
				public float iMinimumRange;
				public Vector3 TargetPosition;
				public int WorldID;
				public int TargetRaGuid;
				public int iForceWaitLoopsBefore;
				public int iForceWaitLoopsAfter;
				public bool bWaitWhileAnimating;
				public bool? Successful;
				public Zeta.CommonBot.PowerManager.CanCastFlags CanCastFlags;

            public Ability(SNOPower thispower, float thisrange, Vector3 thisloc, int thisworld, int thisguid, int waitloops, int afterloops, bool repeat)
            {
                Power = thispower;
                iMinimumRange = thisrange;
                TargetPosition = thisloc;
                WorldID = thisworld;
                TargetRaGuid = thisguid;
                iForceWaitLoopsBefore = waitloops;
                iForceWaitLoopsAfter = afterloops;
                bWaitWhileAnimating = repeat;
					 CanCastFlags=Zeta.CommonBot.PowerManager.CanCastFlags.None;
					 Successful=null;
            }

				public void UsePower()
				{
					 Zeta.CommonBot.PowerManager.CanCast(this.Power, out CanCastFlags);
					 Successful=ZetaDia.Me.UsePower(this.Power, this.TargetPosition, this.WorldID, this.TargetRaGuid);
				}



				///<summary>
				///Sets values related to ability usage and resets the SNOPower
				///</summary>
				public void SuccessfullyUsed()
				{
					 dictAbilityLastUse[Power]=DateTime.Now;
					 lastGlobalCooldownUse=DateTime.Now;
					 Power=SNOPower.None;
				}
        }
    }
}