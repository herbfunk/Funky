using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	public class Firebats : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }


		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.Target; } }


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

			WaitVars = new WaitLoops(0, 0, true);
			Range = range;
			IsChanneling = true;
			
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: range));
			ClusterConditions.Add(new SkillClusterConditions(5d, range, 2, true));



			FcriteriaCombat = () => (Bot.Character.Data.dCurrentEnergy >= CastingCost() || Bot.Character.Data.dCurrentEnergy >= ChannelingCost()&& IsCurrentlyChanneling());
		}
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


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_Firebats; }
		}
	}
}
