using System;
using FunkyTrinity.Cache;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.DemonHunter
{
	public class Chakram : ability, IAbility
	{
		public Chakram() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.ClusterTarget | AbilityExecuteFlags.Target;
			WaitVars = new WaitLoops(0, 1, true);
			Cost = 10;
			Range = 50;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastPreCastFlags = (AbilityPreCastFlags.CheckPlayerIncapacitated | AbilityPreCastFlags.CheckEnergy);

			ClusterConditions = new ClusterConditions(4d, 40, 2, true);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial);

			Fcriteria = new Func<bool>(() =>
			{
				return ((!Bot.Class.HotbarPowers.Contains(SNOPower.DemonHunter_ClusterArrow)) ||
				        DateTime.Now.Subtract(PowerCacheLookup.dictAbilityLastUse[SNOPower.DemonHunter_Chakram]).TotalMilliseconds >=
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
				ability p = (ability) obj;
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
