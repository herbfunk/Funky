using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class BlessedShield : Skill
	{
		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_BlessedShield; }
		}

		public override void Initialize()
		{
			Cooldown = 5;
			Range = 49;
			Cost = 20;
			Priority = SkillPriority.Medium;
			ExecutionType = SkillExecutionFlags.Target|SkillExecutionFlags.ClusterTargetNearest;
			IsRanged = true;
			IsProjectile = true;

			WaitVars = new WaitLoops(0, 4, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			UseageType = SkillUseage.Combat;


			ClusterConditions.Add(new SkillClusterConditions(10d, 35f, 3, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.IsSpecial, 20, 0.95d, TargetProperties.Normal));
		}
	}
}