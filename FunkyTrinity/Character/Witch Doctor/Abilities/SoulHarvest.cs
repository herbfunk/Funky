using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.WitchDoctor
{
	 public class SoulHarvest : Ability, IAbility
	 {
		  public SoulHarvest()
				: base()
		  {
		  }



		  public override void Initialize()
		  {
				Cooldown=15000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=59;
				Counter=5;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.High;

				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast|AbilityPreCastFlags.CheckEnergy);
				ClusterConditions=new ClusterConditions(6d, 9f, 2, false, useRadiusDistance: true);
				FcriteriaCombat=new Func<bool>(() =>
				{

					 double lastCast=this.LastUsedMilliseconds;
					 int RecastMS=this.RuneIndex==1?45000:20000;
					 bool recast=lastCast>RecastMS; //if using soul to waste -- 45ms, else 20ms.
					 int stackCount=Bot.Class.GetBuffStacks(SNOPower.Witchdoctor_SoulHarvest);
					 if (stackCount<5)
						  return true;
					 else if (recast)
						  return true;
					 else
						  return false;

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
				get { return SNOPower.Witchdoctor_SoulHarvest; }
		  }
	 }
}
