using FunkyBot.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Skills.DemonHunter
{
	public class Multishot : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTarget; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(1, 1, true);
			Cost = 30;
			Range = 50;

		
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckEnergy));
			ClusterConditions.Add(new SkillClusterConditions(10d, 40, 3, true));

			FcriteriaCombat = () => !Bot.Character.Class.bWaitingForSpecial;
		}


		public override SNOPower Power
		{
			get { return SNOPower.DemonHunter_Multishot; }
		}
	}
}
