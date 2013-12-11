using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	 public class SpiritBarrage : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=15000;
				ExecutionType=AbilityExecuteFlags.ClusterTargetNearest|AbilityExecuteFlags.Target;
				ClusterConditions=new SkillClusterConditions(5d, 20f, 1, true);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.None, 25,
					falseConditionalFlags: TargetProperties.DOTDPS);
				WaitVars=new WaitLoops(1, 1, true);
				Cost=108;
				Range=21;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.Low;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated|AbilityPreCastFlags.CheckCanCast|
				                          AbilityPreCastFlags.CheckEnergy));

				PreCast.Criteria += (s) => !Bot.Character.Class.HotBar.HasDebuff(SNOPower.Succubus_BloodStar);

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
				get { return SNOPower.Witchdoctor_SpiritBarrage; }
		  }
	 }
}
