using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Monk
{
	 public class WaveOfLight : Skill
	 {
		 public override void Initialize()
		  {
				Cooldown=250;
				ExecutionType=Bot.Character.Class.HotBar.RuneIndexCache[SNOPower.Monk_WaveOfLight]==1
					?AbilityExecuteFlags.Self
					:AbilityExecuteFlags.ClusterLocation|AbilityExecuteFlags.Location;
				WaitVars=new WaitLoops(2, 4, true);
				Cost=Bot.Character.Class.HotBar.RuneIndexCache[SNOPower.Monk_WaveOfLight]==3?40:75;
				Range=16;
				Priority=AbilityPriority.Medium;
				UseageType=AbilityUseage.Combat;

				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckEnergy|AbilityPreCastFlags.CheckCanCast|
				                          AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckPlayerIncapacitated));
				ClusterConditions=new SkillClusterConditions(6d, 35f, 3, true);
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.IsSpecial, 20, falseConditionalFlags: TargetProperties.MissileReflecting);

				FcriteriaCombat=() => !Bot.Character.Class.bWaitingForSpecial;

				FcriteriaBuff=() => Bot.Character.Data.dCurrentHealthPct<0.25d;
				IsBuff=true;

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
				get { return SNOPower.Monk_WaveOfLight; }
		  }
	 }
}
