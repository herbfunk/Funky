using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	 public class MarkedForDeath : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=10000;
				ExecutionType=AbilityExecuteFlags.Target|AbilityExecuteFlags.ClusterTarget;
				WaitVars=new WaitLoops(1, 1, true);
				Cost=3;
				SecondaryEnergy=true;
				Range=40;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Medium;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckEnergy|
				                          AbilityPreCastFlags.CheckRecastTimer));
				ClusterConditions=new SkillClusterConditions(4d, 35f, 2, true);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.RareElite, 40);

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
				get { return SNOPower.DemonHunter_MarkedForDeath; }
		  }
	 }
}
