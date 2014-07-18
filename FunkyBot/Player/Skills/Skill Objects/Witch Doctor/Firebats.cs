using System;
using fBaseXtensions.Game;
using FunkyBot.Skills.Conditions;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.WitchDoctor
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
			var range = RuneIndex == 0 ? 40 : RuneIndex == 4 ? 14 : 25;

			WaitVars = new WaitLoops(0, 0, true);
			Range = range;
			IsChanneling = true;
			
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: range));
			ClusterConditions.Add(new SkillClusterConditions(5d, range, 2, true));



			FcriteriaCombat = () => (FunkyGame.Hero.dCurrentEnergy >= CastingCost() || FunkyGame.Hero.dCurrentEnergy >= ChannelingCost()&& IsCurrentlyChanneling());
		}
		private bool IsCurrentlyChanneling()
		{
			return FunkyGame.Hero.CurrentAnimationState == AnimationState.Channeling &&
					 FunkyGame.Hero.CurrentSNOAnim.HasFlag(SNOAnim.WitchDoctor_Female_1HT_spell_channel |
																				 SNOAnim.WitchDoctor_Female_2HT_spell_channel |
																				 SNOAnim.WitchDoctor_Female_HTH_spell_channel |
																				 SNOAnim.WitchDoctor_Male_1HT_Spell_Channel |
																				 SNOAnim.WitchDoctor_Male_HTH_Spell_Channel);
		}
		private double ChannelingCost()
		{
			if (FunkyGame.Hero.iMyLevel > 55) return 66;
			return 42 + ((FunkyGame.Hero.iMyLevel - 5) / 2);
		}
		private double CastingCost()
		{
			if (FunkyGame.Hero.iMyLevel > 55) return 220;
			return Math.Round((140 + (((FunkyGame.Hero.iMyLevel - 5) / 2) * 3.2)), MidpointRounding.AwayFromZero);
		}


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_Firebats; }
		}
	}
}
