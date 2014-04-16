using FunkyBot.Movement.Clustering;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	public class Avalanche : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Barbarian_Avalanche_v2; }
		}

		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override void Initialize()
		{
			Cooldown = 30000;
			Range = 50;
			ExecutionType = AbilityExecuteFlags.Location|AbilityExecuteFlags.ClusterLocation;
			WaitVars = new WaitLoops(4, 4, true);

			UseageType = AbilityUseage.Combat;
			Priority = AbilityPriority.High;
			PreCast = new SkillPreCast((AbilityPreCastFlags.CheckCanCast | AbilityPreCastFlags.CheckPlayerIncapacitated));

			//SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.IsSpecial, 50, 0.50d, falseConditionalFlags: TargetProperties.Fast));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Boss, 40, 0.75d));

			ClusterConditions.Add(new SkillClusterConditions(6d, 40, 8, false));
			ClusterConditions.Add(new SkillClusterConditions(6d, 40, 2, false, clusterflags: ClusterProperties.Elites));
		}

		#region IAbility
		public override int GetHashCode()
		{
			return (int)Power;
		}
		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			else
			{
				Skill p = (Skill)obj;
				return Power == p.Power;
			}
		}


		#endregion
	}
}
