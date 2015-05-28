﻿using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Items.Enums;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Wizard
{
	public class RayOfFrost : Skill
	{
		public override double Cooldown { get { return 5; } }

		public override bool IsRanged { get { return true; } }
		public override bool IsProjectile { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }
        public override bool IsPiercing { get { return _ispiercing; } }
	    private bool _ispiercing = false;

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 35;
			Range = 48;

			
			Priority = SkillPriority.Medium;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast));

            //Check for piercing
		    if (Equipment.CheckLegendaryItemCount(LegendaryItemTypes.LightofGrace))
		    {
                _ispiercing = true;
		    }
		        
		}


		public override SNOPower Power
		{
			get { return SNOPower.Wizard_RayOfFrost; }
		}
	}
}
