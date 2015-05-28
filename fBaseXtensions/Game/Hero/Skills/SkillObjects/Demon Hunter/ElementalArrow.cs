﻿using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Demonhunter
{
	public class ElementalArrow : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }
		public override bool IsDestructiblePower { get { return true; } }
        public override bool IsPiercing { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 1, true);
			Cost = 10;
			Range = 48;

			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckEnergy));
			ClusterConditions.Add(new SkillClusterConditions(4d, 40, 2, true));

			FcriteriaCombat = (u) => !FunkyGame.Hero.Class.bWaitingForSpecial && (!u.IsTreasureGoblin &&
								   u.SNOID != 5208 && u.SNOID != 5209 &&
								   u.SNOID != 5210);
		}


		public override SNOPower Power
		{
			get { return SNOPower.DemonHunter_ElementalArrow; }
		}
	}
}
