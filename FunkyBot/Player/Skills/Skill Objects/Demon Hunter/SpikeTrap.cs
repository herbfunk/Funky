using fBaseXtensions.Game.Hero;
using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.DemonHunter
{
	public class SpikeTrap : Skill
	{
		public override double Cooldown { get { return 1000; } }

		public override SkillExecutionFlags ExecutionType { get { return _executiontype; } set { _executiontype = value; } }
		private SkillExecutionFlags _executiontype = SkillExecutionFlags.Location | SkillExecutionFlags.ClusterTargetNearest;

		public override void Initialize()
		{
			//Runeindex 2 == Sticky Trap!
			if (RuneIndex == 2)
				ExecutionType = SkillExecutionFlags.Target | SkillExecutionFlags.ClusterTarget;

			WaitVars = new WaitLoops(1, 1, true);
			Cost = 30;
			Range = 40;

			Priority = SkillPriority.Medium;

			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckEnergy));

			if (RuneIndex == 2) //sticky trap on weak non-full HP units!
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.Weak, falseConditionalFlags: TargetProperties.FullHealth));
			else
				SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.RareElite));


			ClusterConditions.Add(new SkillClusterConditions(6d, 45f, 2, true));

			FcriteriaCombat = () => Bot.Targeting.Cache.Environment.HeroPets.DemonHunterSpikeTraps <
								  (Hotbar.PassivePowers.Contains(SNOPower.DemonHunter_Passive_CustomEngineering) ? 6 : 3);
		}


		public override SNOPower Power
		{
			get { return SNOPower.DemonHunter_SpikeTrap; }
		}
	}
}
