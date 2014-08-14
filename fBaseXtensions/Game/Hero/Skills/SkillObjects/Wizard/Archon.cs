using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Wizard
{
	public class Archon : Skill
	{
		public override double Cooldown { get { return 100000; } }

		public override bool IsSpecialAbility { get { return true; } }
		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		//RuneIndex == 4 //Combustion
		//RuneIndex == 2 //Teleport
		//RuneIndex == 3 //FirePower
		//RuneIndex == 1 //Slow Time
		//RuneIndex == 0 //Improved

		public override void Initialize()
		{
			WaitVars = new WaitLoops(4, 5, true);
			Cost = 25;
			
		
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);

			if (RuneIndex==4)
			{
				Range = 10;
				ClusterConditions.Add(new SkillClusterConditions(8d, 30, 10, true));
			}

			//Any non-normal unit within 30 yards that is 95% HP or less!
			SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, maxdistance: 30, MinimumHealthPercent: 0.95d, falseConditionalFlags: TargetProperties.Normal));
		}


		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Archon; }
		}
	}
}
