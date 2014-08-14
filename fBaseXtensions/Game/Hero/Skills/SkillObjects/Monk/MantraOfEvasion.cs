using fBaseXtensions.Game.Hero;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	public class MantraOfEvasion : Skill
	{
		public override double Cooldown { get { return 3300; } }

		public override bool IsBuff { get { return true; } }
		public override bool IsSpecialAbility { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 1, true);
			Cost = Hotbar.PassivePowers.Contains(SNOPower.Monk_Passive_ChantOfResonance) ? 25 : 50;


			Priority = SkillPriority.High;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckRecastTimer));
		
			FcriteriaBuff = () => !Hotbar.HasBuff(SNOPower.X1_Monk_MantraOfEvasion_v2_Passive);

			FcriteriaCombat = () => !Hotbar.HasBuff(SNOPower.X1_Monk_MantraOfEvasion_v2_Passive)
								  ||
								  FunkyBaseExtension.Settings.Monk.bMonkSpamMantra && FunkyGame.Targeting.Cache.CurrentTarget != null &&
								  (FunkyGame.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_25] > 0 ||
								   FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20] >= 2 ||
								   (FunkyGame.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_20] >= 1 && FunkyBaseExtension.Settings.Monk.bMonkInnaSet) ||
								   (FunkyGame.Targeting.Cache.CurrentUnitTarget.IsEliteRareUnique || FunkyGame.Targeting.Cache.CurrentTarget.IsBoss) &&
								   FunkyGame.Targeting.Cache.CurrentTarget.RadiusDistance <= 25f) &&
				// Check if either we don't have blinding flash, or we do and it's been cast in the last 6000ms
				//DateTime.Now.Subtract(dictAbilityLastUse[SNOPower.Monk_BlindingFlash]).TotalMilliseconds <= 6000)) &&
								  (!Hotbar.HasPower(SNOPower.Monk_BlindingFlash) ||
								   (Hotbar.HasPower(SNOPower.Monk_BlindingFlash) && (Hotbar.HasBuff(SNOPower.Monk_BlindingFlash))));
		}


		public override SNOPower Power
		{
			get { return SNOPower.X1_Monk_MantraOfEvasion_v2; }
		}
	}
}
