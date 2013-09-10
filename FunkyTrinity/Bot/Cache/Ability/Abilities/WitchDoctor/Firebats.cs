using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.WitchDoctor
{
	public class Firebats : Ability, IAbility
	{
		public Firebats() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityUseType.ClusterLocation | AbilityUseType.Target;

			WaitVars = new WaitLoops(1, 1, true);
			Range = Bot.Class.RuneIndexCache[Power] == 0 ? 0 : Bot.Class.RuneIndexCache[Power] == 4 ? 14 : 25;
			IsRanged = true;
			IsProjectile=true;
			UseageType = AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastConditions = (AbilityConditions.CheckPlayerIncapacitated);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial);
			ClusterConditions = new ClusterConditions(5d, Bot.Class.RuneIndexCache[Power] == 4 ? 12f : 20f, 1, true);



			Fcriteria = new Func<bool>(() =>
			{
				return (Bot.Character.dCurrentEnergy >= 551 || (Bot.Character.dCurrentEnergy > 66
				                                                && Bot.Character.CurrentAnimationState == AnimationState.Channeling &&
				                                                Bot.Character.CurrentSNOAnim.HasFlag(
					                                                SNOAnim.WitchDoctor_Female_1HT_spell_channel |
					                                                SNOAnim.WitchDoctor_Female_2HT_spell_channel |
					                                                SNOAnim.WitchDoctor_Female_HTH_spell_channel |
					                                                SNOAnim.WitchDoctor_Male_1HT_Spell_Channel |
					                                                SNOAnim.WitchDoctor_Male_HTH_Spell_Channel)));
			});
		}

		public override void InitCriteria()
		{
			base.AbilityTestConditions = new AbilityUsablityTests(this);
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
				Ability p = (Ability) obj;
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
