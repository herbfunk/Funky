using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	public class MantraOfHealing : Skill
	{
		public override double Cooldown { get { return 3000; } }
		public override bool IsBuff { get { return true; } }
		public override bool IsSpecialAbility { get { return true; } }
		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 1, true);
			Cost = Hotbar.PassivePowers.Contains(SNOPower.Monk_Passive_ChantOfResonance) ? 25 : 50;


			Priority = SkillPriority.High;

			if (Equipment.CheckLegendaryItemCount(LegendaryItemTypes.MonkeyKingsGarb, 4))
			{
				PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast));
				FcriteriaBuff = () => FunkyBaseExtension.Settings.Monk.bMonkSpamMantra && FunkyGame.Targeting.Cache.Environment.SurroundingUnits > 0;
				FcriteriaCombat = () => 
					!FunkyGame.Hero.Class.bWaitingForSpecial && 
					FunkyBaseExtension.Settings.Monk.bMonkSpamMantra && 
					FunkyGame.Targeting.Cache.Environment.SurroundingUnits > 0;
			}
			else
			{
				PreCast = new SkillPreCast((SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckRecastTimer));
				FcriteriaBuff = () => FunkyBaseExtension.Settings.Monk.bMonkSpamMantra && FunkyGame.Targeting.Cache.Environment.SurroundingUnits > 0 && LastUsedMilliseconds > 3000;
				FcriteriaCombat = () => FunkyBaseExtension.Settings.Monk.bMonkSpamMantra && LastUsedMilliseconds > 3000;
			}

			
		}

		public override SNOPower Power
		{
			get { return SNOPower.X1_Monk_MantraOfHealing_v2; }
		}
	}
}
