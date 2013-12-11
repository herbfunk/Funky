using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Internals.Actors;

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
			ExecutionType = AbilityExecuteFlags.Self;
			WaitVars = new WaitLoops(3, 3, true);
			Cost = 20;
			UseageType = AbilityUseage.Combat;
			Priority = AbilityPriority.Low;
			PreCast=new SkillPreCast((AbilityPreCastFlags.CheckRecastTimer | AbilityPreCastFlags.CheckEnergy |
			                          AbilityPreCastFlags.CheckCanCast | AbilityPreCastFlags.CheckPlayerIncapacitated));

			ClusterConditions = new SkillClusterConditions(5d, 8, 2, true, 0.90d);

			SingleUnitCondition = new UnitTargetConditions(TargetProperties.None, 10,
				falseConditionalFlags: TargetProperties.DOTDPS);

			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial
								  || (Bot.Settings.Class.bFuryDumpWrath && Bot.Character.Data.dCurrentEnergyPct >= 0.95 &&
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
