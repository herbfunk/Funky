using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.WitchDoctor
{
	public class Firebats : ability, IAbility
	{
		public Firebats() : base()
		{
		}

		 private bool IsChanneling()
		 {
			  Bot.Character.UpdateAnimationState(false);
			  return Bot.Character.CurrentAnimationState==AnimationState.Channeling &&
						Bot.Character.CurrentSNOAnim.HasFlag(SNOAnim.WitchDoctor_Female_1HT_spell_channel|
																					SNOAnim.WitchDoctor_Female_2HT_spell_channel|
																					SNOAnim.WitchDoctor_Female_HTH_spell_channel|
																					SNOAnim.WitchDoctor_Male_1HT_Spell_Channel|
																					SNOAnim.WitchDoctor_Male_HTH_Spell_Channel);
		 }

		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.ClusterLocation | AbilityExecuteFlags.Target;
			WaitVars = new WaitLoops(0, 0, true);
			Range = Bot.Class.RuneIndexCache[Power] == 0 ? 0 : Bot.Class.RuneIndexCache[Power] == 4 ? 14 : 25;
			IsRanged = true;
			IsProjectile=true;
			UseageType = AbilityUseage.Combat;
			Priority = AbilityPriority.High;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial, 14);
			ClusterConditions = new ClusterConditions(5d, Bot.Class.RuneIndexCache[Power] == 4 ? 12f : 20f, 2, true);



			FcriteriaCombat = new Func<bool>(() =>
			{
				return (Bot.Character.dCurrentEnergy >= 551 || (Bot.Character.dCurrentEnergy > 70  && this.IsChanneling()));
			});
		}

		#region IAbility

		public override int RuneIndex
		{
			get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power) ? Bot.Class.RuneIndexCache[this.Power] : -1; }
		}

		public override int GetHashCode()
		{
			return (int) this.Power;
		}

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj == null || this.GetType() != obj.GetType())
			{
				return false;
			}
			else
			{
				ability p = (ability) obj;
				return this.Power == p.Power;
			}
		}

		#endregion

		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_Firebats; }
		}
	}
}
