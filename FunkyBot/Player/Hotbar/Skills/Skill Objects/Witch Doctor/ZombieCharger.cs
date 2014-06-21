using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	public class ZombieCharger : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 1, true);
			Cost = 134;
			Range = 15;

			Priority = SkillPriority.Medium;
			IsDestructiblePower = true;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckEnergy |
									  SkillPrecastFlags.CheckCanCast));

			//FcriteriaPreCast=new Func<bool>(() => { return !Bot.Character_.Class.HotBar.HasDebuff(SNOPower.Succubus_BloodStar); });

			ClusterConditions.Add(new SkillClusterConditions(5d, 20f, 2, true));
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None));
		}


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_ZombieCharger; }
		}
	}
}
