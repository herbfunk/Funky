using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Wizard
{
	public class Meteor : Ability, IAbility
	{
		public Meteor() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityUseType.ClusterTarget | AbilityUseType.Target;
			WaitVars = new WaitLoops(1, 2, true);
			Cost = 50;
			Range = 50;
			IsRanged = true;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastConditions = (AbilityConditions.CheckPlayerIncapacitated | AbilityConditions.CheckEnergy |
			                     AbilityConditions.CheckRecastTimer);
			ClusterConditions = new ClusterConditions(4d, 50f, 2, true);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial,
				falseConditionalFlags: TargetProperties.Fast);
			Fcriteria = new Func<bool>(() => { return !Bot.Class.bWaitingForSpecial; });
		}

		public override void InitCriteria()
		{
			base.AbilityTestConditions = new AbilityUsablityTests(this);
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
			get { return SNOPower.Wizard_Meteor; }
		}
	}
}
