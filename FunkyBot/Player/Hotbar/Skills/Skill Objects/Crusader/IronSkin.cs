using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class IronSkin : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_IronSkin; }
		}


		public override double Cooldown { get { return 30000; } }

		public override WaitLoops WaitVars { get { return WaitLoops.Default; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }
	
		public override SkillUseage UseageType { get { return SkillUseage.Combat; } }

		public override void Initialize()
		{
	
			Priority = SkillPriority.High;
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckRecastTimer);

			//Make sure we are targeting something!
			SingleUnitCondition.Add(new UnitTargetConditions());
			FcriteriaCombat = () => Bot.Character.Data.dCurrentHealthPct < 0.5d;
		}
	}
}
