﻿using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class BlackHole : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=20000;
				ExecutionType = AbilityExecuteFlags.ClusterLocation;
				WaitVars=new WaitLoops(1, 2, true);
				Cost=20;
				Range=50;
				IsRanged=true;
				IsProjectile = true;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckEnergy|
				                          AbilityPreCastFlags.CheckRecastTimer));
				ClusterConditions=new SkillClusterConditions(6d, 50f, 5, true);
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
				get { return SNOPower.X1_Wizard_Wormhole; }
		  }
	 }
}
