using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class BolaShot : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=SkillExecutionFlags.ClusterTarget|SkillExecutionFlags.Target;
				WaitVars=new WaitLoops(0, 0, false);
				Cost=0;
				Range=50;
				IsRanged=true;
				IsProjectile=true;
				UseageType=SkillUseage.Combat;
				Priority=SkillPriority.Low;
				PreCast=new SkillPreCast((SkillPrecastFlags.CheckCanCast));

				ClusterConditions.Add(new SkillClusterConditions(5d, 49f, 2, true));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 49));
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
				else
				{
					 Skill p=(Skill)obj;
					 return Power==p.Power;
				}
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.DemonHunter_Bolas; }
		  }
	 }
}
