using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.WitchDoctor
{
	 public class AcidCloud : Ability, IAbility
	 {
		  public AcidCloud()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=1500;
				ExecutionType=AbilityExecuteFlags.ClusterTarget|AbilityExecuteFlags.Target;

				WaitVars=new WaitLoops(1, 1, true);
				Cost=250;
				Range=Bot.Class.HotBar.RuneIndexCache[Power]==4?20:40;
				IsRanged=true;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Low;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckEnergy|
											AbilityPreCastFlags.CheckCanCast);

				FcriteriaPreCast=new Func<bool>(() => { return !Bot.Class.HotBar.HasDebuff(SNOPower.Succubus_BloodStar); });

				TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial,
					falseConditionalFlags: TargetProperties.Fast);
				ClusterConditions=new ClusterConditions(4d, Bot.Class.HotBar.RuneIndexCache[Power]==4?20f:40f, 2, true);

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
				get { return SNOPower.Witchdoctor_AcidCloud; }
		  }
	 }
}
