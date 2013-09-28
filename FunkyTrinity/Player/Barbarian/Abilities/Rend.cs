using System;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.Ability.Abilities.Barb
{
	public class Rend : ability, IAbility
	{
		public Rend() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_Rend; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		public override void Initialize()
		{
			ExecutionType = AbilityExecuteFlags.Self;
			WaitVars = new WaitLoops(3, 3, true);
			Cost = 20;
			UseageType=AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCastFlags=(AbilityPreCastFlags.CheckRecastTimer|AbilityPreCastFlags.CheckEnergy|
													 AbilityPreCastFlags.CheckCanCast|AbilityPreCastFlags.CheckPlayerIncapacitated);

			ClusterConditions = new ClusterConditions(5d, 8, 2, true, 0.90d);
			TargetUnitConditionFlags = new UnitTargetConditions(TargetProperties.None, 10,
				falseConditionalFlags: TargetProperties.DOTDPS);

			FcriteriaCombat = new Func<bool>(() =>
			{
				return !Bot.Class.bWaitingForSpecial
					 ||(Bot.SettingsFunky.Class.bFuryDumpWrath&&Bot.Character.dCurrentEnergyPct>=0.95&&
									Bot.Class.HasBuff(SNOPower.Barbarian_WrathOfTheBerserker)&&!Bot.Class.Abilities.ContainsKey(SNOPower.Barbarian_Rend));
			});
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
				  ability p=(ability)obj;
				  return this.Power==p.Power;
			 }
		}


		#endregion
	}
}
