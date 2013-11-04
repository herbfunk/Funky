using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.DemonHunter
{
	 public class MarkedForDeath : Ability, IAbility
	 {
		  public MarkedForDeath()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=10000;
				ExecutionType=AbilityExecuteFlags.Target|AbilityExecuteFlags.ClusterTarget;
				WaitVars=new WaitLoops(1, 1, true);
				Cost=3;
				SecondaryEnergy=true;
				Range=40;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Low;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckEnergy|
											AbilityPreCastFlags.CheckRecastTimer);
				ClusterConditions=new ClusterConditions(4d, 35f, 2, true);
				TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.RareElite, 40);

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
				get { return SNOPower.DemonHunter_MarkedForDeath; }
		  }
	 }
}
