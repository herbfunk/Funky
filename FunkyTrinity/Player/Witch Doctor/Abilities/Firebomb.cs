using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.WitchDoctor
{
	public class Firebomb : ability, IAbility
	{
		public Firebomb() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Target | AbilityExecuteFlags.ClusterTarget;
			WaitVars = new WaitLoops(0, 1, true);
			Range = 35;
			IsRanged = true;
			IsProjectile=true;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.None;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated);

			ClusterConditions = new ClusterConditions(4d, 35, 2, true);
			TargetUnitConditionFlags = new UnitTargetConditions();
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
			get { return SNOPower.Witchdoctor_Firebomb; }
		}
	}
}
