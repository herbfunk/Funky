
using System;
using Zeta.Internals.Actors;
using FunkyTrinity.Ability;
using Zeta.CommonBot;
using Zeta;
using Zeta.Common;
namespace FunkyTrinity.Ability.Abilities.Barb
{

	 public class Bash : ability, IAbility
	 {
		  public Bash() : base() { }

		  public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_Bash; }
		  }

		  public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		  public override void Initialize()
		  {
			  ExecutionType = AbilityExecuteFlags.Target;
			  WaitVars = new WaitLoops(0, 1, true);
			  Cost = 0;
			  Range = 10;
				UseageType=AbilityUseage.Combat;
			  Priority = AbilityPriority.None;
			  PreCastPreCastFlags = (AbilityPreCastFlags.CheckRecastTimer | AbilityPreCastFlags.CheckCanCast |
			                       AbilityPreCastFlags.CheckPlayerIncapacitated);

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
					 ability p=(ability)obj;
					 return this.Power==p.Power;
				}
		  }
	
		  #endregion
	 }
}
