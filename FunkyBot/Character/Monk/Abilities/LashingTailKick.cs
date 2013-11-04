using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.Monk
{
	 public class LashingTailKick : Ability, IAbility
	 {
		  public LashingTailKick()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=250;
				ExecutionType=AbilityExecuteFlags.Target;
				WaitVars=new WaitLoops(1, 1, true);
				Cost=30;
				Range=10;
				Priority=AbilityPriority.Low;
				UseageType=AbilityUseage.Combat;
				PreCastFlags=(AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckCanCast|
											AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckPlayerIncapacitated);
				ClusterConditions=new ClusterConditions(4d, 18f, 3, true);
				TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 10);


				FcriteriaCombat=new Func<bool>(() =>
				{
					 return
						  // Either doesn't have sweeping wind, or does but the buff is already up
						 (!Bot.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)||
						  (Bot.Class.HotBar.HotbarPowers.Contains(SNOPower.Monk_SweepingWind)&&Bot.Class.HotBar.HasBuff(SNOPower.Monk_SweepingWind)))&&
						 (!Bot.Class.bWaitingForSpecial||Bot.Character.dCurrentEnergy>=Bot.Class.iWaitingReservedAmount);
				});
		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Class.HotBar.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.HotBar.RuneIndexCache[this.Power]:-1; }
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

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.Monk_LashingTailKick; }
		  }
	 }
}
