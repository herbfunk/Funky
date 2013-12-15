using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta;
using Zeta.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	public class Firebats : Skill
	{
		private bool IsCurrentlyChanneling()
		{
			return Bot.Character.Data.CurrentAnimationState == AnimationState.Channeling &&
					 Bot.Character.Data.CurrentSNOAnim.HasFlag(SNOAnim.WitchDoctor_Female_1HT_spell_channel |
																				 SNOAnim.WitchDoctor_Female_2HT_spell_channel |
																				 SNOAnim.WitchDoctor_Female_HTH_spell_channel |
																				 SNOAnim.WitchDoctor_Male_1HT_Spell_Channel |
																				 SNOAnim.WitchDoctor_Male_HTH_Spell_Channel);
		}

		private double ChannelingCost()
		{
			if (Bot.Character.Data.iMyLevel > 55) return 66;
			return 42 + ((Bot.Character.Data.iMyLevel - 5) / 2);
		}
		private double CastingCost()
		{
			if (Bot.Character.Data.iMyLevel > 55) return 220;
			return Math.Round((140 + (((Bot.Character.Data.iMyLevel - 5) / 2) * 3.2)), MidpointRounding.AwayFromZero);
		}

		//42 Channeling Base Cost
		//Level > 55 == 66
		//((Level - 5) / 2) + 42
		//
		//140 Base Cost
		//Level > 55 == 220
		//(((Level - 5) / 2) * 3.2)+140)
		public override void Initialize()
		{
			var range = Bot.Character.Class.HotBar.RuneIndexCache[Power] == 0 ? 40 : Bot.Character.Class.HotBar.RuneIndexCache[Power] == 4 ? 14 : 25;
			//Cost = CastingCost();
			Cooldown = 5;
			ExecutionType = AbilityExecuteFlags.ClusterLocation | AbilityExecuteFlags.Target;
			WaitVars = new WaitLoops(0, 0, true);
			Range = range;
			IsRanged = true;
			IsProjectile = true;
			IsChanneling = true;
			UseageType = AbilityUseage.Combat;
			Priority = AbilityPriority.High;
			PreCast = new SkillPreCast((AbilityPreCastFlags.CheckPlayerIncapacitated));
			SingleUnitCondition = new UnitTargetConditions(TargetProperties.None, range);
			ClusterConditions = new SkillClusterConditions(5d, range, 2, true);



			FcriteriaCombat = () => (Bot.Character.Data.dCurrentEnergy >= CastingCost() || Bot.Character.Data.dCurrentEnergy >= ChannelingCost()&& IsCurrentlyChanneling());
		}

		#region IAbility

		public override int RuneIndex
		{
			get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; }
		}

		public override int GetHashCode()
		{
			return (int)Power;
		}

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Skill p = (Skill)obj;
			return Power == p.Power;
		}

		#endregion

		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_Firebats; }
		}
	}
}
