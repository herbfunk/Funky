using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.Crusader
{
	public class SteedCharge : Skill
	{
		public override SNOPower Power
		{
			get { return SNOPower.X1_Crusader_SteedCharge; }
		}

		public override void Initialize()
		{
			Cooldown = 15000;
			Priority = SkillPriority.High;
			ExecutionType = SkillExecutionFlags.Buff;
			WaitVars = new WaitLoops(0, 0, true);
			UseageType = SkillUseage.Combat;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckCanCast));

			IsBuff = true;
			FcriteriaBuff = () => Bot.Settings.OutOfCombatMovement;

			FCombatMovement = (v) =>
			{
				float fDistanceFromTarget = Bot.Character.Data.Position.Distance(v);
				if (!Bot.Character.Class.bWaitingForSpecial && Funky.Difference(Bot.Character.Data.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
				{
					if (fDistanceFromTarget > 35f)
						return MathEx.CalculatePointFrom(v, Bot.Character.Data.Position, 35f);
					else
						return v;
				}

				return Vector3.Zero;
			};
		}
	}
}
