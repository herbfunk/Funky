using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	 public class ZombieCharger : Skill
	 {
		 public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power)?Bot.Character.Class.HotBar.RuneIndexCache[Power]:-1; } }

		  public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=AbilityExecuteFlags.ClusterTarget|AbilityExecuteFlags.Target;

				WaitVars=new WaitLoops(0, 1, true);
				Cost=134;
				Range=11;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Medium;

				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckEnergy|
				                          AbilityPreCastFlags.CheckCanCast));

				//FcriteriaPreCast=new Func<bool>(() => { return !Bot.Character_.Class.HotBar.HasDebuff(SNOPower.Succubus_BloodStar); });

				ClusterConditions=new SkillClusterConditions(5d, 20f, 2, true);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.None);
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
				get { return SNOPower.Witchdoctor_ZombieCharger; }
		  }
	 }
}
