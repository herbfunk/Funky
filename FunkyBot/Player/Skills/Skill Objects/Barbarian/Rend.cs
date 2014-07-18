using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.Barb
{
	public class Rend : Skill
	{
		public override SNOPower Power { get { return SNOPower.Barbarian_Rend; } }

	
		public override double Cooldown { get { return 3500; } }

		private readonly WaitLoops _waitVars = new WaitLoops(3, 3, true);
		public override WaitLoops WaitVars { get { return _waitVars; } }

		public override bool IsDestructiblePower { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Self; } }
	
		public override SkillUseage UseageType { get { return SkillUseage.Anywhere; } }

		public override void Initialize()
		{
			Priority = SkillPriority.Medium;
			Cost = 20;
			PreCast=new SkillPreCast((SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckEnergy |
			                          SkillPrecastFlags.CheckCanCast | SkillPrecastFlags.CheckPlayerIncapacitated));

			ClusterConditions.Add(new SkillClusterConditions(5d, 8, 2, true, 0.90d));

			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 10,
				falseConditionalFlags: TargetProperties.DOTDPS | TargetProperties.SucideBomber));

			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial
								  || (Bot.Settings.Barbarian.bFuryDumpWrath && FunkyGame.Hero.dCurrentEnergyPct >= 0.95 &&
									 Hotbar.HasBuff(SNOPower.Barbarian_WrathOfTheBerserker) && !Bot.Character.Class.Abilities.ContainsKey(SNOPower.Barbarian_Rend));
		}

	}
}
