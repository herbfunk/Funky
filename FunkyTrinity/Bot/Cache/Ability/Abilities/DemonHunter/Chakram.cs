using System;
using FunkyTrinity.Cache;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.DemonHunter
{
	public class Chakram : Ability, IAbility
	{
		public Chakram() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = PowerExecutionTypes.ClusterTarget | PowerExecutionTypes.Target;
			WaitVars = new WaitLoops(0, 1, true);
			Cost = 10;
			Range = 50;
			IsADestructiblePower=true;
			UseFlagsType=AbilityUseFlags.Combat;
			Priority = AbilityPriority.Low;
			PreCastConditions = (CastingConditionTypes.CheckPlayerIncapacitated | CastingConditionTypes.CheckEnergy);

			ClusterConditions = new ClusterConditions(4d, 40, 2, true);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial);

			Fcriteria = new Func<bool>(() =>
			{
				return ((!Bot.Class.HotbarPowers.Contains(SNOPower.DemonHunter_ClusterArrow)) ||
				        DateTime.Now.Subtract(this.LastUsed).TotalMilliseconds >=
				        110000);
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
				Ability p = (Ability) obj;
				return this.Power == p.Power;
			}
		}

		#endregion

		public override SNOPower Power
		{
			get { return SNOPower.DemonHunter_Chakram; }
		}
	}
}
