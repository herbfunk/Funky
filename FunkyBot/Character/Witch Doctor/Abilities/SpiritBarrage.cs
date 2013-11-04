using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.WitchDoctor
{
	 public class SpiritBarrage : Ability, IAbility
	 {
		  public SpiritBarrage()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=15000;
				ExecutionType=AbilityExecuteFlags.ClusterTargetNearest|AbilityExecuteFlags.Target;
				ClusterConditions=new ClusterConditions(5d, 20f, 1, true);
				TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.None, 25,
					falseConditionalFlags: TargetProperties.DOTDPS);
				WaitVars=new WaitLoops(1, 1, true);
				Cost=108;
				Range=21;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Low;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast|
												AbilityPreCastFlags.CheckEnergy);

				FcriteriaPreCast=new Func<bool>(() => { return !Bot.Class.HotBar.HasDebuff(SNOPower.Succubus_BloodStar); });

				FcriteriaCombat=new Func<bool>(() =>
				{
					 return !Bot.Class.bWaitingForSpecial;
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
				get { return SNOPower.Witchdoctor_SpiritBarrage; }
		  }
	 }
}
