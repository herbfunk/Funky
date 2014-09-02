using System.Linq;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Monk
{
	public class DeadlyReach : Skill
	{
		public override double Cooldown { get { return _cooldown; } set { _cooldown = value; } }
		private double _cooldown = 5;

		public override bool IsDestructiblePower { get { return true; } }
		public override bool IsPrimarySkill { get { return true; } }

		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Target; } }

		public override void Initialize()
		{
			Priority = SkillPriority.Low;
			Range = 16;

			//Combot Strike? lets enforce recast timer and cast check
			//if (FunkyBaseExtension.Settings.Monk.bMonkComboStrike)
				//precastflags |= SkillPrecastFlags.CheckRecastTimer | SkillPrecastFlags.CheckCanCast;

			
			if (FunkyBaseExtension.Settings.Monk.bMonkComboStrike)
			{
				PreCast = new SkillPreCast
				{
					Flags = SkillPrecastFlags.CheckPlayerIncapacitated
				};
				PreCast.Criteria += skill => FunkyGame.Hero.Class.LastUsedAbilities.IndexOf(this) >= FunkyBaseExtension.Settings.Monk.iMonkComboStrikeAbilities-1;
				PreCast.CreatePrecastCriteria();

				//Use as third hitter!
				if (RuneIndex==0)
					Priority=SkillPriority.None;
			}
			else
				PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated);
		}

		public override void OnSuccessfullyUsed(bool reorderAbilities = true)
		{
			
			base.OnSuccessfullyUsed(reorderAbilities);
		}


		public override SNOPower Power
		{
			get { return SNOPower.Monk_DeadlyReach; }
		}
	}
}
