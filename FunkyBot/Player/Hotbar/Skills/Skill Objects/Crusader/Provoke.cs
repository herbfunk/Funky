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

		/*
		 * Rune Indexs
		 * 0 == Cleanse (Life on Hit increased)
		 * 1 == Flee Fool (Fear Enemies)
		 * 2 == Too Scared to Run (Reduced Attack Speed and Movement)
		 * 3 == Charged Up (Lightning Damage)
		 * 4 == Hit Me (Increased Block Chance)
		*/
		public override void Initialize()
		{
			Cooldown = 20000;
			Range = 20;
			Priority = SkillPriority.High;
			ExecutionType = SkillExecutionFlags.Buff;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			UseageType = SkillUseage.Combat;

			//Low On Wrath Buff Option (less than 10%)
			IsBuff = true;
			FcriteriaBuff = () => Bot.Character.Data.dCurrentEnergyPct < 0.10d;

			ClusterConditions.Add(new SkillClusterConditions(10d, 15f, 3, false));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 20, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

			//Minimum Wrath Missing 10%
			FcriteriaCombat = () => Bot.Character.Data.dCurrentEnergyPct < 0.90d;
		}
	}
}
