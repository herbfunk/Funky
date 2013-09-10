using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Wizard
{
	public class Electrocute : Ability, IAbility
	{
		public Electrocute() : base()
		{
		}



		public override void Initialize()
		{
			ExecutionType = AbilityUseType.Target | AbilityUseType.ClusterTarget;
			WaitVars = new WaitLoops(0, 0, true);
			Range=(Bot.Class.RuneIndexCache[SNOPower.Wizard_Electrocute]==2?15:40);
			IsRanged = true;
			IsProjectile=true;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.None;
			PreCastConditions = (AbilityConditions.CheckPlayerIncapacitated);

								//Aim for cluster with 2 units very close together.
			ClusterConditions = new ClusterConditions(3d, Bot.Class.RuneIndexCache[SNOPower.Wizard_Electrocute] == 2 ? 15 : 40, 2,
				true);
								//No conditions for a single target.
			TargetUnitConditionFlags = new UnitTargetConditions();
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
			get { return SNOPower.Wizard_Electrocute; }
		}
	}
}
