using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Wizard
{
	public class EnergyTwister : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override bool IsRanged { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Location; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 35;
			Range = 28;

			IsDestructiblePower = true;
		
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckEnergy |
									  SkillPrecastFlags.CheckCanCast));

			FcriteriaCombat = () => (!HasSignatureAbility() || Hotbar.GetBuffStacks(SNOPower.Wizard_EnergyTwister) < 1) &&
								  (Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_30] >= 1 ||
								   Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_25] >= 1 ||
								   Bot.Targeting.Cache.CurrentTarget.RadiusDistance <= 12f) &&
								  (!Hotbar.HasPower(SNOPower.Wizard_Electrocute) ||
								   !Bot.Targeting.Cache.CurrentUnitTarget.IsFast) &&
								  (FunkyGame.Hero.dCurrentEnergy >= 35);
		}

		private bool HasSignatureAbility()
		{
			return (Hotbar.HasPower(SNOPower.Wizard_MagicMissile) || Hotbar.HasPower(SNOPower.Wizard_ShockPulse) ||
									Hotbar.HasPower(SNOPower.Wizard_SpectralBlade) || Hotbar.HasPower(SNOPower.Wizard_Electrocute));
		}


		public override SNOPower Power
		{
			get { return SNOPower.Wizard_EnergyTwister; }
		}
	}
}
