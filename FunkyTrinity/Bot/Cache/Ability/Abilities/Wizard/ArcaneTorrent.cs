using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Wizard
{
	public class ArcaneTorrent : Ability, IAbility
	{
		public ArcaneTorrent() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = PowerExecutionTypes.Target;
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 16;
			Range = 40;
			IsRanged = true;
			IsProjectile=true;
			UseFlagsType=AbilityUseFlags.Combat;
			Priority = AbilityPriority.Low;
			PreCastConditions = (CastingConditionTypes.CheckPlayerIncapacitated | CastingConditionTypes.CheckEnergy |
			                     CastingConditionTypes.CheckRecastTimer);
			Fcriteria = new Func<bool>(() => { return !Bot.Class.bWaitingForSpecial; });
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
				Ability p = (Ability) obj;
				return this.Power == p.Power;
			}
		}

		#endregion

		public override SNOPower Power
		{
			get { return SNOPower.Wizard_ArcaneTorrent; }
		}
	}
}
