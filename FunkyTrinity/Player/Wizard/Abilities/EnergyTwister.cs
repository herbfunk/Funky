using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Wizard
{
	public class EnergyTwister : ability, IAbility
	{
		public EnergyTwister() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Location;
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 35;
			Range=UsingCriticalMass()?9:28;
			IsRanged = true;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated | AbilityPreCastFlags.CheckEnergy |
			                     AbilityPreCastFlags.CheckCanCast);

			FcriteriaCombat = new Func<bool>(() =>
			{
				return (!HasSignatureAbility() || Bot.Class.GetBuffStacks(SNOPower.Wizard_EnergyTwister) < 1) &&
				       (Bot.Combat.iElitesWithinRange[(int) RangeIntervals.Range_30] >= 1 ||
				        Bot.Combat.iAnythingWithinRange[(int) RangeIntervals.Range_25] >= 1 ||
				        Bot.Target.CurrentTarget.RadiusDistance <= 12f) &&
				       (!Bot.Class.HotbarPowers.Contains(SNOPower.Wizard_Electrocute) ||
				        !FunkyTrinity.Cache.CacheIDLookup.hashActorSNOFastMobs.Contains(Bot.Target.CurrentTarget.SNOID)) &&
							 ((this.UsingCriticalMass()&&(!HasSignatureAbility()||Bot.Character.dCurrentEnergy>=35))||
								(!this.UsingCriticalMass()&&Bot.Character.dCurrentEnergy>=35));
			});
		}

		private bool HasSignatureAbility()
		{
			 return (Bot.Class.HotbarPowers.Contains(SNOPower.Wizard_MagicMissile)||Bot.Class.HotbarPowers.Contains(SNOPower.Wizard_ShockPulse)||
									 Bot.Class.HotbarPowers.Contains(SNOPower.Wizard_SpectralBlade)||Bot.Class.HotbarPowers.Contains(SNOPower.Wizard_Electrocute));
		}
		private bool UsingCriticalMass()
		{
			 return Bot.Class.PassivePowers.Contains(SNOPower.Wizard_Passive_CriticalMass); ;
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
			get { return SNOPower.Wizard_EnergyTwister; }
		}
	}
}
