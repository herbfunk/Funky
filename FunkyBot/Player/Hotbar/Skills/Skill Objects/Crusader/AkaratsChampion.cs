using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class AkaratsChampion : Skill
	{
		public override int RuneIndex { get { return Bot.Character.Class.HotBar.RuneIndexCache.ContainsKey(Power) ? Bot.Character.Class.HotBar.RuneIndexCache[Power] : -1; } }


		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_AkaratsChampion; }
		}

		public override void Initialize()
		{
			Cooldown = 90000;
			Priority = SkillPriority.High;
			ExecutionType = SkillExecutionFlags.Buff;

			WaitVars = new WaitLoops(0, 0, true);
			PreCast = new SkillPreCast(SkillPrecastFlags.CheckCanCast);
			UseageType = SkillUseage.Combat;

			//Make sure we are targeting something!
			SingleUnitCondition.Add(new UnitTargetConditions());

			//Activate on Bosses or Low Health!
			FcriteriaCombat = () => Bot.Targeting.Cache.CurrentTarget.IsBoss || Bot.Character.Data.dCurrentHealthPct < 0.5d;
		}
	}
}
