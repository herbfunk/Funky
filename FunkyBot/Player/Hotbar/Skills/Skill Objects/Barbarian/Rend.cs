using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Barb
{
	public class Rend : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.Barbarian_Rend; }
		}

		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override void Initialize()
		{
			Cooldown = 3500;
			ExecutionType = SkillExecutionFlags.Self;
			WaitVars = new WaitLoops(3, 3, true);
			Cost = 20;
			UseageType = SkillUseage.Combat;
			Priority = SkillPriority.Medium;
			PreCast=new SkillPreCast((SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckEnergy |
			                          SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated));

			ClusterConditions.Add(new SkillClusterConditions(5d, 8, 2, true, 0.90d));

			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 10,
				falseConditionalFlags: TargetProperties.DOTDPS | TargetProperties.SucideBomber));

			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial
								  || (Bot.Settings.Barbarian.bFuryDumpWrath && Bot.Character.Data.dCurrentEnergyPct >= 0.95 &&
									 Bot.Character.Class.HotBar.HasBuff(SNOPower.Barbarian_WrathOfTheBerserker) && !Bot.Character.Class.Abilities.ContainsKey(SNOPower.Barbarian_Rend));
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
