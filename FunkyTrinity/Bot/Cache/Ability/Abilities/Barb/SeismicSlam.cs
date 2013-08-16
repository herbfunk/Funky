using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Barb
{
	public class SeismicSlam : Ability, IAbility
	{
		public SeismicSlam() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_SeismicSlam; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		protected override void Initialize()
		{
			ExecutionType = AbilityUseType.ClusterLocation | AbilityUseType.Location;
			WaitVars = new WaitLoops(2, 2, true);
			Cost = Bot.Class.RuneIndexCache[SNOPower.Barbarian_SeismicSlam] == 3 ? 15 : 30;
			Range = 40;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;

			PreCastConditions = (AbilityConditions.CheckRecastTimer | AbilityConditions.CheckEnergy |
			                     AbilityConditions.CheckCanCast | AbilityConditions.CheckPlayerIncapacitated);
			ClusterConditions=new ClusterConditions(Bot.Class.RuneIndexCache[Power]==4?4d:6d, 40f, 2, true);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.IsSpecial,
				falseConditionalFlags: TargetProperties.TreasureGoblin | TargetProperties.Fast);

			Fcriteria=new Func<bool>(() => { return !Bot.Class.bWaitingForSpecial; });
		}
		#region IAbility
		public override int GetHashCode()
		{
			 return (int)this.Power;
		}
		public override bool Equals(object obj)
		{
			 //Check for null and compare run-time types. 
			 if (obj==null||this.GetType()!=obj.GetType())
			 {
				  return false;
			 }
			 else
			 {
				  Ability p=(Ability)obj;
				  return this.Power==p.Power;
			 }
		}
		

		#endregion
	}
}
