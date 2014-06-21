using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
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

			FcriteriaCombat = () => (!HasSignatureAbility() || Bot.Character.Class.HotBar.GetBuffStacks(SNOPower.Wizard_EnergyTwister) < 1) &&
								  (Bot.Targeting.Cache.Environment.iElitesWithinRange[(int)RangeIntervals.Range_30] >= 1 ||
								   Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_25] >= 1 ||
								   Bot.Targeting.Cache.CurrentTarget.RadiusDistance <= 12f) &&
								  (!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Wizard_Electrocute) ||
								   !Bot.Targeting.Cache.CurrentUnitTarget.IsFast) &&
								  (Bot.Character.Data.dCurrentEnergy >= 35);
		}

		private bool HasSignatureAbility()
		{
			return (Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Wizard_MagicMissile) || Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Wizard_ShockPulse) ||
									Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Wizard_SpectralBlade) || Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.Wizard_Electrocute));
		}


		public override SNOPower Power
		{
			get { return SNOPower.Wizard_EnergyTwister; }
		}
	}
}
