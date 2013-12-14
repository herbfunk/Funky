using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	 public class AcidCloud : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=1500;
				ExecutionType=AbilityExecuteFlags.ClusterTarget|AbilityExecuteFlags.Target;

				WaitVars=new WaitLoops(1, 1, true);
				Cost=250;
				Range=Bot.Character.Class.HotBar.RuneIndexCache[Power]==4?20:40;
				IsRanged=true;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Medium;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckEnergy|
				                          AbilityPreCastFlags.CheckCanCast));

				PreCast.Criteria += (s) => !Bot.Character.Class.HotBar.HasDebuff(SNOPower.Succubus_BloodStar);

				SingleUnitCondition=new UnitTargetConditions(TargetProperties.IsSpecial,
					falseConditionalFlags: TargetProperties.Fast);
				ClusterConditions=new SkillClusterConditions(4d, Bot.Character.Class.HotBar.RuneIndexCache[Power]==4?20f:40f, 2, true);

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
				get { return SNOPower.Witchdoctor_AcidCloud; }
		  }
	 }
}
