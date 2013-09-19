using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Monk
{
	public class DeadlyReach : ability, IAbility
	{
		public DeadlyReach() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Target;
			UseageType=AbilityUseage.Combat;
			WaitVars = new WaitLoops(0, 1, true);
			Priority = AbilityPriority.None;
			Range = 16;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated);
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
			get { return SNOPower.Monk_DeadlyReach; }
		}
	}
}
