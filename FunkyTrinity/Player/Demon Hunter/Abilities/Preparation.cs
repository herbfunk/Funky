using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.DemonHunter
{
	 public class Preparation : Ability, IAbility
	 {
		  public Preparation()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=5000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(1, 1, true);
				UseageType=AbilityUseage.Anywhere;
				Priority=AbilityPriority.High;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckRecastTimer|
											AbilityPreCastFlags.CheckCanCast);
				Cost=Bot.Class.RuneIndexCache[SNOPower.DemonHunter_Preparation]==0?25:0;
				FcriteriaCombat=new Func<bool>(() =>
				{
					 return Bot.Character.dDisciplinePct<0.25d
						  //Rune: Punishment (Restores all Hatered for 25 disc)
							  ||(Bot.Class.RuneIndexCache[Power]==0&&Bot.Character.dCurrentEnergyPct<0.20d);
				});

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
				get { return SNOPower.DemonHunter_Preparation; }
		  }
	 }
}
