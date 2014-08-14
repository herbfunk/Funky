using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Crusader
{
	public class AkaratsChampion : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_AkaratsChampion; }
		}


		
		public override double Cooldown { get { return 90000; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }
		
		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			Cost = 10;

			//Make sure we are targeting something!
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 30, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
			ClusterConditions.Add(new SkillClusterConditions(6d, 30, 10, true));

			//Akkhan Set?
			if (FunkyBaseExtension.Settings.Crusader.FullAkkhanSet)
			{
				//Clusters of 4 units..
				ClusterConditions.Add(new SkillClusterConditions(10d, 30, 4, false));
			}
		}
	}
}
