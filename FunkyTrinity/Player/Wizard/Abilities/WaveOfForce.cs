using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Wizard
{
	public class WaveOfForce : ability, IAbility
	{
		public WaveOfForce() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Buff;
			WaitVars = new WaitLoops(1, 2, true);
			Cost = 25;
UseageType= AbilityUseage.Anywhere;
			Priority = AbilityPriority.High;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated | AbilityPreCastFlags.CheckEnergy |
			                     AbilityPreCastFlags.CheckCanCast | AbilityPreCastFlags.CheckRecastTimer);

			Fcriteria = new Func<bool>(() =>
			{
				return
					// Check this isn't a critical mass wizard, cos they won't want to use this except for low health unless they don't have nova/blast in which case go for it
					((UsingCriticalMass()&&
					  ((!Bot.Class.HotbarPowers.Contains(SNOPower.Wizard_FrostNova) &&
					    !Bot.Class.HotbarPowers.Contains(SNOPower.Wizard_ExplosiveBlast)) ||
					   (Bot.Character.dCurrentHealthPct <= 0.7 &&
					    (Bot.Combat.iElitesWithinRange[(int) RangeIntervals.Range_15] > 0 ||
					     Bot.Combat.iAnythingWithinRange[(int) RangeIntervals.Range_15] > 0 ||
					     (Bot.Target.CurrentTarget.ObjectIsSpecial && Bot.Target.CurrentTarget.RadiusDistance <= 23f)))))
						// Else normal wizard in which case check standard stuff
					 ||
					 (!UsingCriticalMass()&&Bot.Combat.iElitesWithinRange[(int)RangeIntervals.Range_15]>0||
					  Bot.Combat.iAnythingWithinRange[(int) RangeIntervals.Range_15] > 3 || Bot.Character.dCurrentHealthPct <= 0.7 ||
					  (Bot.Target.CurrentTarget.ObjectIsSpecial && Bot.Target.CurrentTarget.RadiusDistance <= 23f)));
			});
		}

		private bool UsingCriticalMass()
		{
			 return Bot.Class.PassivePowers.Contains(SNOPower.Wizard_Passive_CriticalMass);;
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
			get { return SNOPower.Wizard_WaveOfForce; }
		}
	}
}
