using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class Provoke : Skill
	{
		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_Provoke; }
		}

		public override void Initialize()
		{
			Cooldown = 20000;
			Range = 20;
			Priority = SkillPriority.High;
			ExecutionType = SkillExecutionFlags.Buff;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			UseageType = SkillUseage.Combat;

			ClusterConditions.Add(new SkillClusterConditions(10d, 15f, 4, false));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 15, 0.95d, TargetProperties.Normal));

			FcriteriaCombat = () => Bot.Character.Data.dCurrentEnergyPct < 0.5d;
		}
	}
}
