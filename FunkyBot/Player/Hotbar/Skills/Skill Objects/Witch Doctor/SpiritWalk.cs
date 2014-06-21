using FunkyBot.Movement.Clustering;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar.Skills.WitchDoctor
{
	public class SpiritWalk : Skill
	{
		public override double Cooldown { get { return 15200; } }

		public override bool IsSpecialAbility { get { return true; } }

		public override SkillExecutionFlags ExecutionType { get { return SkillExecutionFlags.Buff; } }

		public override void Initialize()
		{
			WaitVars = new WaitLoops(0, 0, true);
			Cost = 49;


			Priority = SkillPriority.High;
			PreCast = new SkillPreCast((SkillPrecastFlags.CheckEnergy | SkillPrecastFlags.CheckCanCast));


			//IsSpecialMovementSkill = true;
			//IsBuff=true;
			//FcriteriaBuff=() => Bot.Settings.General.OutOfCombatMovement;

			//Use buff at location (no prediction required!)
			FOutOfCombatMovement = (v) => v;

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

			Range = 6;
			ClusterConditions.Add(new SkillClusterConditions(7d, 30f, 4, false, 0, ClusterProperties.None, 10f, true));
			FcriteriaCombat = () => ((LastConditionPassed == ConditionCriteraTypes.Cluster) ||
									   (Bot.Character.Data.dCurrentHealthPct <= 0.35d) ||
									   (RuneIndex == 3 && Bot.Character.Data.dCurrentEnergyPct < 0.25d) ||
									   (Bot.Targeting.Cache.Environment.FleeTriggeringUnits.Count > 0) ||
									   (Bot.Targeting.Cache.Environment.TriggeringAvoidances.Count > 0) ||
									   (Bot.Character.Data.bIsIncapacitated || Bot.Character.Data.bIsRooted));
		}


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_SpiritWalk; }
		}
	}
}
