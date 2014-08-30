using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Navigation.Clustering;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero.Skills.SkillObjects.Witchdoctor
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
			//FcriteriaBuff=() => FunkyBaseExtension.Settings.General.OutOfCombatMovement;

			//Use buff at location (no prediction required!)
			FOutOfCombatMovement = (v) => v;

			FCombatMovement = (v) =>
			{
				float fDistanceFromTarget = FunkyGame.Hero.Position.Distance(v);
				if (!FunkyGame.Hero.Class.bWaitingForSpecial && FunkyBaseExtension.Difference(FunkyGame.Hero.Position.Z, v.Z) <= 4 && fDistanceFromTarget >= 20f)
				{
					if (fDistanceFromTarget > 35f)
						return MathEx.CalculatePointFrom(v, FunkyGame.Hero.Position, 35f);
					else
						return v;
				}

				return Vector3.Zero;
			};

			Range = 6;
			ClusterConditions.Add(new SkillClusterConditions(7d, 30f, 4, false, 0, ClusterProperties.None, 10f, true));
			FcriteriaCombat = () => ((LastConditionPassed == ConditionCriteraTypes.Cluster) ||
									   (FunkyGame.Hero.dCurrentHealthPct <= 0.35d) ||
									   (RuneIndex == 3 && FunkyGame.Hero.dCurrentEnergyPct < 0.25d) ||
									   (FunkyGame.Targeting.Cache.Environment.FleeTriggeringUnits.Count > 0) ||
									   (FunkyGame.Targeting.Cache.Environment.TriggeringAvoidances.Count > 0) ||
									   (FunkyGame.Hero.bIsIncapacitated || FunkyGame.Hero.bIsRooted));
		}


		public override SNOPower Power
		{
			get { return SNOPower.Witchdoctor_SpiritWalk; }
		}
	}
}
