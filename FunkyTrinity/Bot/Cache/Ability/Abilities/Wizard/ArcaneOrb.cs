using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Wizard
{
	public class ArcaneOrb : Ability, IAbility
	{
		public ArcaneOrb() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityUseType.ClusterTarget | AbilityUseType.Target;
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 35;
			Range=this.UsingCriticalMass()?20:40;
			IsRanged = true;
			IsProjectile=true;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastConditions = (AbilityConditions.CheckPlayerIncapacitated | AbilityConditions.CheckRecastTimer |
			                     AbilityConditions.CheckEnergy);
			ClusterConditions=new ClusterConditions(4d, this.UsingCriticalMass()?20:40, 3, true);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial, 25, 0.5d, TargetProperties.Fast);
			Fcriteria = new Func<bool>(() => { return !Bot.Class.bWaitingForSpecial; });
		}

		public override void InitCriteria()
		{
			base.AbilityTestConditions = new AbilityUsablityTests(this);
		}
		private bool UsingCriticalMass()
		{
			 return Bot.Class.PassivePowers.Contains(SNOPower.Wizard_Passive_CriticalMass); ;
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
			get { return SNOPower.Wizard_ArcaneOrb; }
		}
	}
}
