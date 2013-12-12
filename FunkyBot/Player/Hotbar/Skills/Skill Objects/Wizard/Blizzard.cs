using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class Blizzard : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=2500;
				ExecutionType=AbilityExecuteFlags.ClusterTarget|AbilityExecuteFlags.Target;
				WaitVars=new WaitLoops(1, 2, true);
				Cost=40;
				Range=50;
				IsRanged=true;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Low;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckEnergy|
				                          AbilityPreCastFlags.CheckRecastTimer));
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.IsSpecial);
				ClusterConditions=new SkillClusterConditions(5d, 50f, 2, true);
				FcriteriaCombat=() => !Bot.Character.Class.bWaitingForSpecial;
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
				get { return SNOPower.Wizard_Blizzard; }
		  }
	 }
}
