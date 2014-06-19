using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	 public class SpiritBarrage : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=15000;
				ExecutionType=SkillExecutionFlags.Target;
				IsProjectile = true;
				IsRanged = true;
				//ClusterConditions.Add(new SkillClusterConditions(5d, 20f, 1, true));
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 45,falseConditionalFlags: TargetProperties.DOTDPS));
				WaitVars=new WaitLoops(0, 0, false);
				Cost=100;
				Range=45;
				UseageType=SkillUseage.Combat;
				Priority=SkillPriority.Medium;
				PreCast=new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated|SkillPrecastFlags.CheckCanCast);

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
