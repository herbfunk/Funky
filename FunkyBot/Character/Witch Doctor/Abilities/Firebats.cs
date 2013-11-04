using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyBot.AbilityFunky.Abilities.WitchDoctor
{
	 public class Firebats : Ability, IAbility
	 {
		  public Firebats()
				: base()
		  {
		  }

		  private bool IsCurrentlyChanneling()
		  {
				return Bot.Character.CurrentAnimationState==AnimationState.Channeling&&
						 Bot.Character.CurrentSNOAnim.HasFlag(SNOAnim.WitchDoctor_Female_1HT_spell_channel|
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
				Range=Bot.Class.HotBar.RuneIndexCache[Power]==0?0:Bot.Class.HotBar.RuneIndexCache[Power]==4?14:25;
				IsRanged=true;
				IsProjectile=true;
				IsChanneling=true;
				UseageType=AbilityUseage.Combat;
				Priority=AbilityPriority.High;
				PreCastFlags=(AbilityPreCastFlags.CheckPlayerIncapacitated);
				TargetUnitConditionFlags=new UnitTargetConditions(TargetProperties.IsSpecial, 14);
				ClusterConditions=new ClusterConditions(5d, Bot.Class.HotBar.RuneIndexCache[Power]==4?12f:25f, 2, true);



				FcriteriaCombat=new Func<bool>(() =>
				{
					 return (Bot.Character.dCurrentEnergy>=551||(Bot.Character.dCurrentEnergy>70&&this.IsCurrentlyChanneling()));
				});
		  }

		  #region IAbility

		  public override int RuneIndex
		  {
				get { return Bot.Class.HotBar.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.HotBar.RuneIndexCache[this.Power]:-1; }
		  }

		  public override int GetHashCode()
		  {
				return (int)this.Power;
		  }

		  public override bool Equals(object obj)
		  {
				//Check for null and compare run-time types. 
				if (obj==null||this.GetType()!=obj.GetType())
				{
					 return false;
				}
				else
				{
					 Ability p=(Ability)obj;
					 return this.Power==p.Power;
				}
		  }

		  #endregion

		  public override SNOPower Power
		  {
				get { return SNOPower.Witchdoctor_Firebats; }
		  }
	 }
}
