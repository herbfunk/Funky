using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	public class MantraOfEvasion : Skill
	{
		public override double Cooldown { get { return 3300; } }

		public override bool IsBuff { get { return true; } }
		public override bool IsSpecialAbility { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		private bool HasInnaSetBonus = false;

		public override void Initialize()
		{
			HasInnaSetBonus = Equipment.CheckLegendaryItemCount(LegendaryItemTypes.InnasMantra, 3);

			WaitVars = new WaitLoops(0, 1, true);
			Cost = Hotbar.PassivePowers.Contains(SNOPower.Monk_Passive_ChantOfResonance) ? 25 : 50;


			Priority = SkillPriority.High;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckRecastTimer));

			FcriteriaBuff = () => FunkyBaseExtension.Settings.Monk.bMonkSpamMantra && FunkyGame.Targeting.Cache.Environment.TriggeringAvoidances.Count > 0;

			FcriteriaCombat = () => FunkyBaseExtension.Settings.Monk.bMonkSpamMantra && FunkyGame.Targeting.Cache.Environment.TriggeringAvoidances.Count > 0;
		}


		public override SNOPower Power
		{
			get { return SNOPower.X1_Monk_MantraOfEvasion_v2; }
		}
	}
}
