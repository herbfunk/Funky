using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.WitchDoctor
{
	 public class ZombieCharger : Ability, IAbility
	 {
		  public ZombieCharger()
				: base()
		  {
		  }



		  public override int RuneIndex { get { return Bot.Class.HotBar.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.HotBar.RuneIndexCache[this.Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=AbilityExecuteFlags.ClusterTarget|AbilityExecuteFlags.Target;

				WaitVars=new WaitLoops(0, 1, true);
				Cost=134;
				Range=11;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Low;

				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckEnergy|
											AbilityPreCastFlags.CheckCanCast);

				//FcriteriaPreCast=new Func<bool>(() => { return !Bot.Class.HotBar.HasDebuff(SNOPower.Succubus_BloodStar); });

				ClusterConditions=new ClusterConditions(5d, 20f, 2, true);
				TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.None);
		  }

		  #region IAbility

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
				get { return SNOPower.Witchdoctor_ZombieCharger; }
		  }
	 }
}
