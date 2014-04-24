using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	public class Piranhas : Skill
	 {
		 public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=8000;
				ExecutionType=SkillExecutionFlags.ClusterLocation|SkillExecutionFlags.Location;

				WaitVars=new WaitLoops(0, 3, true);
				Cost=250;
				Range=45;
				IsRanged=true;
				UseageType=SkillUseage.Combat;
				Priority=SkillPriority.High;

				PreCast=new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast|SkillPrecastFlags.CheckEnergy));

				ClusterConditions.Add(new SkillClusterConditions(4d, 45f, 5, true));
			    SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, 45, 0.95d));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.IsSpecial, 45, 0.95d, TargetProperties.Fast|TargetProperties.Weak));
				FcriteriaCombat=() => !Bot.Character.Class.bWaitingForSpecial;
		  }

		  #region IAbility

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
				get { return SNOPower.Witchdoctor_Piranhas; }
		  }
	 }
}
