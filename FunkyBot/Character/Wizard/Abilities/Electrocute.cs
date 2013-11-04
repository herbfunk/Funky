using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.Wizard
{
	 public class Electrocute : Ability, IAbility
	 {
		  public Electrocute()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=AbilityExecuteFlags.Target|AbilityExecuteFlags.ClusterTarget;
				WaitVars=new WaitLoops(0, 0, true);
				Range=(Bot.Class.HotBar.RuneIndexCache[SNOPower.Wizard_Electrocute]==2?15:40);
				IsRanged=true;
				IsProjectile=true;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.None;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated);

				//Aim for cluster with 2 units very close together.
				ClusterConditions=new ClusterConditions(3d, Bot.Class.HotBar.RuneIndexCache[SNOPower.Wizard_Electrocute]==2?15:40, 2,
					true);
				//No conditions for a single target.
				TargetUnitConditionFlags=new UnitTargetConditions();
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
				get { return SNOPower.Wizard_Electrocute; }
		  }
	 }
}
