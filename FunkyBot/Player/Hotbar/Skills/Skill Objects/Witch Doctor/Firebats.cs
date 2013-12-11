using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	 public class Firebats : Skill
	 {
		 private bool IsCurrentlyChanneling()
		  {
				return Bot.Character.Data.CurrentAnimationState==AnimationState.Channeling&&
						 Bot.Character.Data.CurrentSNOAnim.HasFlag(SNOAnim.WitchDoctor_Female_1HT_spell_channel|
																					 SNOAnim.WitchDoctor_Female_2HT_spell_channel|
																					 SNOAnim.WitchDoctor_Female_HTH_spell_channel|
																					 SNOAnim.WitchDoctor_Male_1HT_Spell_Channel|
																					 SNOAnim.WitchDoctor_Male_HTH_Spell_Channel);
		  }

		  public override void Initialize()
		  {
				Cooldown=5;
				ExecutionType=AbilityExecuteFlags.ClusterLocation|AbilityExecuteFlags.Target;
				WaitVars=new WaitLoops(0, 0, true);
				Range=Bot.Character.Class.HotBar.RuneIndexCache[Power]==0?0:Bot.Character.Class.HotBar.RuneIndexCache[Power]==4?14:25;
				IsRanged=true;
				IsProjectile=true;
				IsChanneling=true;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.High;
				PreCast=new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated));
				SingleUnitCondition=new UnitTargetConditions(TargetProperties.IsSpecial, 14);
				ClusterConditions=new SkillClusterConditions(5d, Bot.Character.Class.HotBar.RuneIndexCache[Power]==4?12f:25f, 2, true);



				FcriteriaCombat=() => (Bot.Character.Data.dCurrentEnergy>=551||(Bot.Character.Data.dCurrentEnergy>70&&IsCurrentlyChanneling()));
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
				get { return SNOPower.Witchdoctor_Firebats; }
		  }
	 }
}
