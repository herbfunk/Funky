using System;
using FunkyTrinity.Cache;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability.Abilities.Barb
{
	public class Warcry : Ability, IAbility
	{
		public Warcry() : base()
		{
		}

		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_WarCry; }
		}

		public override int RuneIndex { get { return Bot.Class.RuneIndexCache.ContainsKey(this.Power)?Bot.Class.RuneIndexCache[this.Power]:-1; } }

		protected override void Initialize()
		{
			ExecutionType = AbilityUseType.Buff;
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 0;
			Range = 0;
			UseageType = AbilityUseage.Anywhere;
			Priority = AbilityPriority.High;
			PreCastConditions = (AbilityConditions.CheckCanCast | AbilityConditions.CheckPlayerIncapacitated);
			Fbuff = new Func<bool>(() => { return !Bot.Class.HasBuff(SNOPower.Barbarian_WarCry); });
			Fcriteria = new Func<bool>(() =>
			{
				return (!Bot.Class.HasBuff(SNOPower.Barbarian_WarCry)
				        ||
				        (Bot.Class.PassivePowers.Contains(SNOPower.Barbarian_Passive_InspiringPresence) &&
				         DateTime.Now.Subtract(PowerCacheLookup.dictAbilityLastUse[SNOPower.Barbarian_WarCry]).TotalSeconds > 59
				         || Bot.Character.dCurrentEnergyPct < 0.10));
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
					Ability p=(Ability)obj;
					return this.Power==p.Power;
			 }
		}
		public override void UsePower()
		{
			 if (!this.ExecutionType.HasFlag(AbilityUseType.RemoveBuff))
			 {
					PowerManager.CanCast(this.Power, out CanCastFlags);
					SuccessUsed=ZetaDia.Me.UsePower(this.Power, this.TargetPosition, this.WorldID, this.TargetRAGUID);
			 }
			 else
			 {
					ZetaDia.Me.GetBuff(this.Power).Cancel();
					SuccessUsed=true;
			 }
		}
		public override void SetupAbilityForUse()
		{
			 base.SetupAbilityForUse();
			 //Cluster Target -- Aims for Centeroid Unit
			 if (this.ExecutionType.HasFlag(AbilityUseType.ClusterTarget)&&CheckClusterConditions(this.ClusterConditions)) //Cluster ACDGUID
			 {
					TargetRAGUID=Bot.Combat.Clusters(this.ClusterConditions)[0].GetNearestUnitToCenteroid().AcdGuid.Value;
					return;
			 }
			 //Cluster Location -- Aims for Center of Cluster
			 if (this.ExecutionType.HasFlag(AbilityUseType.ClusterLocation)&&CheckClusterConditions(this.ClusterConditions)) //Cluster Target Position
			 {
					TargetPosition=(Vector3)Bot.Combat.Clusters(this.ClusterConditions)[0].Midpoint;
					return;
			 }
			 //Cluster Target Nearest -- Gets nearest unit in cluster as target.
			 if (this.ExecutionType.HasFlag(AbilityUseType.ClusterTargetNearest)&&CheckClusterConditions(this.ClusterConditions)) //Cluster Target Position
			 {
					TargetRAGUID=Bot.Combat.Clusters(this.ClusterConditions)[0].ListUnits[0].AcdGuid.Value;
					return;
			 }

			 if (this.ExecutionType.HasFlag(AbilityUseType.Location)) //Current Target Position
					TargetPosition=Bot.Target.CurrentTarget.Position;
			 else if (this.ExecutionType.HasFlag(AbilityUseType.Self)) //Current Bot Position
					TargetPosition=Bot.Character.Position;
			 else if (this.ExecutionType.HasFlag(AbilityUseType.ZigZagPathing)) //Zig-Zag Pathing
			 {
					Bot.Combat.vPositionLastZigZagCheck=Bot.Character.Position;
					if (Bot.Class.ShouldGenerateNewZigZagPath())
						 Bot.Class.GenerateNewZigZagPath();

					TargetPosition=Bot.Combat.vSideToSideTarget;
			 }
			 else if (this.ExecutionType.HasFlag(AbilityUseType.Target)) //Current Target ACDGUID
					TargetRAGUID=Bot.Target.CurrentTarget.AcdGuid.Value;
		}
		public override AbilityConditions PreCastConditions
		{
			 get
			 {
					return base.PreCastConditions;
			 }
			 set
			 {
					base.PreCastConditions=value;
					Func<bool> precastFunc;
					Ability.CreatePreCastConditions(out precastFunc, this);
					base.Fprecast=precastFunc;
			 }
		}
		public override ClusterConditions ClusterConditions
		{
			 get
			 {
					return base.ClusterConditions;
			 }
			 set
			 {
					base.ClusterConditions=value;
					Func<bool> Fclustercondition;
					Ability.CreateClusterConditions(out Fclustercondition, this);
					base.FClusterConditions=Fclustercondition;
			 }
		}
		#endregion
	}
}
