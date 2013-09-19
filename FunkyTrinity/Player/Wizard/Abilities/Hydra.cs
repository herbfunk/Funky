using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Wizard
{
	public class Hydra : ability, IAbility
	{
		public Hydra() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.ClusterLocation | AbilityExecuteFlags.Location;
			WaitVars = new WaitLoops(1, 2, true);
			Counter = 1;
			Cost = 15;
			Range = 50;
			IsRanged = true;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated | AbilityPreCastFlags.CheckEnergy |
			                     AbilityPreCastFlags.CheckRecastTimer | AbilityPreCastFlags.CheckPetCount);
			ClusterConditions = new ClusterConditions(7d, 50f, 2, true);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial,
				falseConditionalFlags: TargetProperties.Fast);
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
			get { return SNOPower.Wizard_Hydra; }
		}
	}
}
