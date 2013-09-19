using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.WitchDoctor
{
	public class GraspOfTheDead : Ability, IAbility
	{
		public GraspOfTheDead() : base()
		{
		}



		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		public override void Initialize()
		{
			ExecutionType = AbilityUseType.ClusterTarget | AbilityUseType.Target;

			WaitVars = new WaitLoops(0, 3, true);
			Cost = 122;
			Range = 45;
			IsRanged = true;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;

			PreCastConditions = (AbilityConditions.CheckPlayerIncapacitated | AbilityConditions.CheckCanCast |
			                     AbilityConditions.CheckEnergy);

			Fprecast=new Func<bool>(() => { return !Bot.Class.HasDebuff(SNOPower.Succubus_BloodStar); });

			ClusterConditions = new ClusterConditions(4d, 45f, 2, true);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial, 45,
				falseConditionalFlags: TargetProperties.Fast | TargetProperties.Weak);
			Fcriteria = new Func<bool>(() =>
			{
				return !Bot.Class.bWaitingForSpecial;
			});
		}

		public override void InitCriteria()
		{
			base.AbilityTestConditions = new AbilityUsablityTests(this);
		}

		#region IAbility

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
			get { return SNOPower.Witchdoctor_GraspOfTheDead; }
		}
	}
}
