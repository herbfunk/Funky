using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	 public class Electrocute : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=AbilityExecuteFlags.Target|AbilityExecuteFlags.ClusterTarget;
				WaitVars=new WaitLoops(0, 0, true);
				Range=(Bot.Character.Class.HotBar.RuneIndexCache[SNOPower.Wizard_Electrocute]==2?15:40);
				IsRanged=true;
				IsProjectile=true;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.None;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated));

				//Aim for cluster with 2 units very close together.
				ClusterConditions=new SkillClusterConditions(3d, Bot.Character.Class.HotBar.RuneIndexCache[SNOPower.Wizard_Electrocute]==2?15:40, 2,
					true);
				//No conditions for a single target.
				SingleUnitCondition=new UnitTargetConditions();
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
				get { return SNOPower.Wizard_Electrocute; }
		  }
	 }
}
