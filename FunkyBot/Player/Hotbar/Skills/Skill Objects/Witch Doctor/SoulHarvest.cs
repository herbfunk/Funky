using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	 public class SoulHarvest : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=15000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(0, 1, true);
				Cost=59;
				Counter=5;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.High;

				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast|AbilityPreCastFlags.CheckEnergy));
				ClusterConditions=new SkillClusterConditions(6d, 9f, 2, false, useRadiusDistance: true);
				FcriteriaCombat=() =>
				{

					double lastCast=LastUsedMilliseconds;
					int RecastMS=RuneIndex==1?45000:20000;
					bool recast=lastCast>RecastMS; //if using soul to waste -- 45ms, else 20ms.
					int stackCount=Bot.Character.Class.HotBar.GetBuffStacks(SNOPower.Witchdoctor_SoulHarvest);
					if (stackCount<5)
						return true;
					if (recast)
						return true;
					return false;
				};
		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; }
		  }

		  public override int GetHashCode()
		  {
				return (int)Power;
		  }

		  public override bool Equals(object obj)
		  {
				//Check for null and compare run-time types. 
				if (obj==null||GetType()!=obj.GetType())
				{
					 return false;
				}
			  Skill p=(Skill)obj;
			  return Power==p.Power;
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.Witchdoctor_SoulHarvest; }
		  }
	 }
}
