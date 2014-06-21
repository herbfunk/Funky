using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.DemonHunter
{
	public class Chakram : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 1, true);
			Cost = 10;
			Range = 50;
			IsDestructiblePower = true;

			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckEnergy));

			ClusterConditions.Add(new SkillClusterConditions(4d, 40, 2, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 50, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));

			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial && ((!Bot.Character.Class.HotBar.HotbarPowers.Contains(SNOPower.DemonHunter_ClusterArrow)) ||
																			  LastUsedMilliseconds >= 110000);
		}


		public override SNOPower Power
		{
			get { return SNOPower.DemonHunter_Chakram; }
		}
	}
}
