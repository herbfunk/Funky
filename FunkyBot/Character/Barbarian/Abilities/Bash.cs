
using System;
using Zeta.Internals.Actors;
using FunkyBot.AbilityFunky;
using Zeta.CommonBot;
using Zeta;
using Zeta.Common;
namespace FunkyBot.AbilityFunky.Abilities.Barb
{

	 public class Bash : Ability, IAbility
	 {
		  public Bash() : base() { }

		  public override SNOPower Power
		  {
				get { return SNOPower.Barbarian_Bash; }
		  }

		  public override int RuneIndex { get { return Bot.Class.HotBar.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.HotBar.RuneIndexCache[this.Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=AbilityExecuteFlags.Target;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=0;
				Range=10;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.None;
				PreCastFlags=(AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckCanCast|
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
					 Ability p=(Ability)obj;
					 return this.Power==p.Power;
				}
		  }

		  #endregion
	 }
}
