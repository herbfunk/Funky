using FunkyBot.Movement.Clustering;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	public class Vengeance : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=90000;
				ExecutionType=AbilityExecuteFlags.Buff;
				WaitVars=new WaitLoops(0, 1, true);
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.High;
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, 50, 0.75d));
				ClusterConditions.Add(new SkillClusterConditions(10d, 50f, 4, true, clusterflags: ClusterProperties.Elites));
				PreCast = new SkillPreCast(AbilityPreCastFlags.CheckRecastTimer);

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
				get { return SNOPower.X1_DemonHunter_Vengeance; }
		  }
	 }
}
