using System;

using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Wizard
{
	public class MirrorImage : ability, IAbility
	{
		public MirrorImage() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Buff;
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 10;
			Range = 48;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.High;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckCanCast);

			FcriteriaCombat = new Func<bool>(() =>
			{
				return (Bot.Character.dCurrentHealthPct <= 0.50 ||
				        Bot.Combat.iAnythingWithinRange[(int) RangeIntervals.Range_30] >= 5 || Bot.Character.bIsIncapacitated ||
				        Bot.Character.bIsRooted || Bot.Target.CurrentTarget.ObjectIsSpecial);
			});
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
			get { return SNOPower.Wizard_MirrorImage; }
		}
	}
}
