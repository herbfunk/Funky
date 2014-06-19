using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class HeavensFury : Skill
	{
		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }

		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_HeavensFury3; }
		}

		public override void Initialize()
		{
			Cooldown = RuneIndex==4?5:20000;
			Range = 49;
			Cost = RuneIndex == 4 ? 40 : 0;

			Priority = SkillPriority.High;
			ExecutionType = SkillExecutionFlags.Location | SkillExecutionFlags.ClusterLocation;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			UseageType = SkillUseage.Combat;

			//Fires of Heaven (Straight Line Fire)
			ClusterConditions.Add(RuneIndex == 4
				? new SkillClusterConditions(5d, 45f, 3, true)
				: new SkillClusterConditions(8d, 45f, 4, true));

			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 45, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

			//Fire of Heaven (No CD!)
			if (RuneIndex == 4)
			{
				SingleUnitCondition.Add(new UnitTargetConditions
				{
					TrueConditionFlags = TargetProperties.None,
					Criteria = () => Bot.Character.Data.dCurrentEnergyPct >  0.90d,
					FalseConditionFlags = TargetProperties.LowHealth|TargetProperties.Weak,
				});
			}
		}
	}
}
