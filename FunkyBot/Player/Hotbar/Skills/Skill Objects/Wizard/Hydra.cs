using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class Hydra : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=1500;
				ExecutionType=AbilityExecuteFlags.ClusterLocation|AbilityExecuteFlags.Location;
				WaitVars=new WaitLoops(1, 2, true);
				Counter=1;
				Cost=15;
				Range=50;
				IsRanged=true;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Low;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckEnergy|
				                          AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckPetCount));
				ClusterConditions=new SkillClusterConditions(7d, 50f, 2, true);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.IsSpecial,
					falseConditionalFlags: TargetProperties.Fast);
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
				get { return SNOPower.Wizard_Hydra; }
		  }
	 }
}
