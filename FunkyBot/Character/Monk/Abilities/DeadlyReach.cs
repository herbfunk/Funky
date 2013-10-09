using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.Monk
{
	 public class DeadlyReach : Ability, IAbility
	 {
		  public DeadlyReach()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				if (!Bot.Settings.Class.bMonkComboStrike)
					 Cooldown=5;
				else
					 Cooldown=250+(250*Bot.Settings.Class.iMonkComboStrikeAbilities);

				ExecutionType=AbilityExecuteFlags.Target;
				UseageType=AbilityUseage.Combat;
				WaitVars=new WaitLoops(1, 4, true);
				Priority=Bot.Settings.Class.bMonkComboStrike?AbilityPriority.Low:AbilityPriority.None;
				Range=16;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated);
				if (Bot.Settings.Class.bMonkComboStrike)
					 PreCastFlags|=AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckCanCast;
		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; }
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
				get { return SNOPower.Monk_DeadlyReach; }
		  }
	 }
}
