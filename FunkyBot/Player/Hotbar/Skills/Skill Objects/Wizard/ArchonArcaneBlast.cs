using System;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Wizard
{
	public class ArchonArcaneBlast : Skill
	{
		public override double Cooldown { get { return 1000; } }

		public override bool IsBuff { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override void Initialize()
		{
			Priority = SkillPriority.High;

			FcriteriaBuff = () => false;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckPlayerIncapacitated | SkillPrecastFlags.CheckCanCast);
			//SingleUnitCondition.Add(new UnitTargetConditions(TargetProperties.None, 10));
			FcriteriaCombat = () => Bot.Targeting.Cache.Environment.iAnythingWithinRange[(int)RangeIntervals.Range_12] > 0;
		}


		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Archon_ArcaneBlast; }
		}
	}

	public class ArchonArcaneBlastCold : ArchonArcaneBlast
	{
		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Archon_ArcaneBlast_Cold; }
		}
	}

	public class ArchonArcaneBlastFire : ArchonArcaneBlast
	{
		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Archon_ArcaneBlast_Fire; }
		}
	}

	public class ArchonArcaneBlastLightning : ArchonArcaneBlast
	{
		public override SNOPower Power
		{
			get { return SNOPower.Wizard_Archon_ArcaneBlast_Lightning; }
		}
	}


}
