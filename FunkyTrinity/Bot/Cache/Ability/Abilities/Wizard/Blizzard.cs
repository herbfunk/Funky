using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Wizard
{
	public class Blizzard : Ability, IAbility
	{
		public Blizzard() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = PowerExecutionTypes.ClusterTarget | PowerExecutionTypes.Target;
			WaitVars = new WaitLoops(1, 2, true);
			Cost = 40;
			Range = 50;
			IsRanged = true;
			UseFlagsType=AbilityUseFlags.Combat;
			Priority = AbilityPriority.Low;
			PreCastConditions = (CastingConditionTypes.CheckPlayerIncapacitated | CastingConditionTypes.CheckEnergy |
			                     CastingConditionTypes.CheckRecastTimer);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial);
			ClusterConditions = new ClusterConditions(5d, 50f, 2, true);
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
			get { return SNOPower.Wizard_Blizzard; }
		}
	}
}
