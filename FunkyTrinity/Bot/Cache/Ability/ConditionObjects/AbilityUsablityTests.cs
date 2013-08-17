using System;
using Zeta.CommonBot;
using Zeta.Internals.SNO;

namespace FunkyTrinity.ability
{
	 ///<summary>
	 ///Creates Funcs from a created Ability and is to be used in testing of usability.
	 ///</summary>
	public class AbilityUsablityTests
	{

		public AbilityUsablityTests(Ability ability)
		{
			 this.LastConditionPassed=ConditionCriteraTypes.None;
			 this.Fcriteria=ability.Fcriteria;
			this.CreatePreCastConditions(ref Fprecast, ability);
			this.CreateTargetConditions(ref FSingleTargetUnitCriteria, ability);
			this.CreateUnitsInRangeConditions(ref FUnitsInRangeConditions, ability);
			this.CreateElitesInRangeConditions(ref FElitesInRangeConditions, ability);
			this.CreateClusterConditions(ref FClusterConditions, ability);
		}

		 public AbilityUsablityTests()
		 {
				this.LastConditionPassed=ConditionCriteraTypes.None;
				Fprecast=null;
				Fcriteria=new Func<bool>(() => { return true; });
				FClusterConditions=null;
				FUnitsInRangeConditions=null;
				FElitesInRangeConditions=null;
			 FSingleTargetUnitCriteria = null;
		 }
		///<summary>
		///ability precast conditions
		///</summary>
		 private Func<bool> Fprecast;
		 private Func<bool> Fcriteria;
		 private Func<bool> FClusterConditions;
		 private Func<bool> FUnitsInRangeConditions;
		 private Func<bool> FElitesInRangeConditions;
		 private Func<bool> FSingleTargetUnitCriteria;


		 #region Function Creation Methods
		 private void CreateClusterConditions(ref Func<bool> FClusterConditions, Ability ability)
		 {
				FClusterConditions=null;
				if (ability.ClusterConditions==null) return;

				FClusterConditions=new Func<bool>(() => { return Ability.CheckClusterConditions(ability.ClusterConditions); });
		 }

		private void CreatePreCastConditions(ref Func<bool> Fprecast, Ability ability)
		 {
				Fprecast=null;
				AbilityConditions precastconditions_=ability.PreCastConditions;

				if (precastconditions_.Equals(AbilityConditions.None))
					 Fprecast+=(new Func<bool>(() => { return true; }));
				else
				{
					 if (precastconditions_.HasFlag(AbilityConditions.CheckPlayerIncapacitated))
							Fprecast+=(new Func<bool>(() => { return !Bot.Character.bIsIncapacitated; }));

					 if (precastconditions_.HasFlag(AbilityConditions.CheckPlayerRooted))
							Fprecast+=(new Func<bool>(() => { return !Bot.Character.bIsRooted; }));

					 if (precastconditions_.HasFlag(AbilityConditions.CheckExisitingBuff))
							Fprecast+=(new Func<bool>(() => { return !Bot.Class.HasBuff(ability.Power); }));

					 if (precastconditions_.HasFlag(AbilityConditions.CheckPetCount))
							Fprecast+=(new Func<bool>(() => { return Bot.Class.MainPetCount<ability.Counter; }));

					 if (precastconditions_.HasFlag(AbilityConditions.CheckRecastTimer))
							Fprecast+=(new Func<bool>(() => { return ability.LastUsedMilliseconds>ability.Cooldown; }));

					 if (precastconditions_.HasFlag(AbilityConditions.CheckCanCast))
					 {
							Fprecast+=(new Func<bool>(() =>
							{
								 bool cancast=PowerManager.CanCast(ability.Power, out ability.CanCastFlags);

								 //Special Ability -- Trigger Waiting For Special When Not Enough Resource to Cast.
								 if (ability.IsSpecialAbility)
								 {
										if (!cancast&&ability.CanCastFlags.HasFlag(PowerManager.CanCastFlags.PowerNotEnoughResource))
											 Bot.Class.bWaitingForSpecial=true;
										else
											 Bot.Class.bWaitingForSpecial=false;
								 }

								 return cancast;
							}));
					 }

					 if (precastconditions_.HasFlag(AbilityConditions.CheckEnergy))
					 {
							if (!ability.SecondaryEnergy)
								 Fprecast+=(new Func<bool>(() =>
								 {
										bool energyCheck=Bot.Character.dCurrentEnergy>=ability.Cost;
										if (ability.IsSpecialAbility) //we trigger waiting for special here.
											 Bot.Class.bWaitingForSpecial=!energyCheck;
										return energyCheck;
								 }));
							else
								 Fprecast+=(new Func<bool>(() =>
								 {
										bool energyCheck=Bot.Character.dDiscipline>=ability.Cost;
										if (ability.IsSpecialAbility) //we trigger waiting for special here.
											 Bot.Class.bWaitingForSpecial=!energyCheck;
										return energyCheck;
								 }));
					 }
				}

		 }

		private void CreateTargetConditions(ref Func<bool> FSingleTargetUnitCriteria, Ability ability)
		 {

				FSingleTargetUnitCriteria=null;

				if (ability.TargetUnitConditionFlags==null) return;
				UnitTargetConditions TargetUnitConditionFlags_=ability.TargetUnitConditionFlags;

				if (TargetUnitConditionFlags_.Distance>-1)
					 FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.RadiusDistance<=TargetUnitConditionFlags_.Distance; });
				if (TargetUnitConditionFlags_.HealthPercent>0d)
					 FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.CurrentHealthPct.Value<=TargetUnitConditionFlags_.HealthPercent; });

				//TRUE CONDITIONS
				if (TargetUnitConditionFlags_.TrueConditionFlags.Equals(TargetProperties.None))
					 FSingleTargetUnitCriteria+=new Func<bool>(() => { return true; });
				else
				{
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Boss))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsBoss; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Burrowing))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsBurrowableUnit; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.FullHealth))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.CurrentHealthPct.Value==1d; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.IsSpecial))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.ObjectIsSpecial; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Weak))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.UnitMaxHitPointAverageWeight<0; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.MissileDampening))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.MonsterMissileDampening; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.RareElite))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.IsEliteRareUnique; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.MissileReflecting))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsMissileReflecting; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Shielding))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.MonsterShielding; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Stealthable))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsStealthableUnit; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.SucideBomber))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsSucideBomber; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.TreasureGoblin))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentTarget.IsTreasureGoblin; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Unique))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.MonsterUnique; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Ranged))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.Monstersize.Value==MonsterSize.Ranged; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.TargetableAndAttackable))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.IsTargetableAndAttackable; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.Fast))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.IsFast; });
					 if (TargetUnitConditionFlags_.TrueConditionFlags.HasFlag(TargetProperties.DOTDPS))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.HasDOTdps.HasValue&&Bot.Target.CurrentUnitTarget.HasDOTdps.Value; });
				}

				//FALSE CONDITIONS
				if (TargetUnitConditionFlags_.FalseConditionFlags.Equals(TargetProperties.None))
					 FSingleTargetUnitCriteria+=new Func<bool>(() => { return true; });
				else
				{
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Boss))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsBoss; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Burrowing))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsBurrowableUnit; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.FullHealth))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.CurrentHealthPct.Value!=1d; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.IsSpecial))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.ObjectIsSpecial; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Weak))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.UnitMaxHitPointAverageWeight>0; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.MissileDampening))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.MonsterMissileDampening; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.RareElite))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.IsEliteRareUnique; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.MissileReflecting))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsMissileReflecting; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Shielding))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.MonsterShielding; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Stealthable))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsStealthableUnit; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.SucideBomber))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsSucideBomber; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.TreasureGoblin))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentTarget.IsTreasureGoblin; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Unique))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.MonsterUnique; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Ranged))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return Bot.Target.CurrentUnitTarget.Monstersize.Value!=MonsterSize.Ranged; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.TargetableAndAttackable))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.IsTargetableAndAttackable; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.Fast))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.IsFast; });
					 if (TargetUnitConditionFlags_.FalseConditionFlags.HasFlag(TargetProperties.DOTDPS))
							FSingleTargetUnitCriteria+=new Func<bool>(() => { return !Bot.Target.CurrentUnitTarget.HasDOTdps.HasValue||!Bot.Target.CurrentUnitTarget.HasDOTdps.Value; });
				}
		 }

		private void CreateUnitsInRangeConditions(ref Func<bool> FUnitRange, Ability ability)
		 {
				FUnitRange=null;
				if (ability.UnitsWithinRangeConditions!=null)
					 FUnitRange+=new Func<bool>(() => { return Bot.Combat.iAnythingWithinRange[(int)ability.UnitsWithinRangeConditions.Item1]>=ability.UnitsWithinRangeConditions.Item2; });
		 }

		private void CreateElitesInRangeConditions(ref Func<bool> FUnitRange, Ability ability)
		 {
				FUnitRange=null;
				if (ability.ElitesWithinRangeConditions!=null)
					 FUnitRange+=new Func<bool>(() => { return Bot.Combat.iElitesWithinRange[(int)ability.ElitesWithinRangeConditions.Item1]>=ability.ElitesWithinRangeConditions.Item2; });
		 }
		 #endregion

		///<summary>
		///Tracks last successful condition if any.
		///</summary>
		public ConditionCriteraTypes LastConditionPassed
		{
			 get { return lastConditionPassed; }
			 set { lastConditionPassed=value; }
		}
		private ConditionCriteraTypes lastConditionPassed=ConditionCriteraTypes.None;

		///<summary>
		///Check Ability is valid to use.
		///</summary>
		public bool CheckPreCastConditionMethod()
		  {
				foreach (Func<bool> item in Fprecast.GetInvocationList())
				{
					 if (!item()) return false;
				}

				//Reset Last Condition
				LastConditionPassed=ConditionCriteraTypes.None;
				return true;
		  }
		///<summary>
		///Check Combat
		///</summary>
		public bool CheckCombatConditionMethod()
		{
			 //Order in which tests are conducted..

			 //Units in Range (Not Cluster)
			 //Clusters
			 //Single Target

			 //If all are null or any of them are successful, then we test Custom Conditions
			 //Custom Condition


			 bool TestCustomConditions=false;

			 bool FailedCondition=false;
			 if (FElitesInRangeConditions!=null)
			 {
					foreach (Func<bool> item in this.FElitesInRangeConditions.GetInvocationList())
					{
						 if (!item())
						 {
								FailedCondition=true;
								break;
						 }
					}
					if (!FailedCondition)
					{
						 TestCustomConditions=true;
						 LastConditionPassed=ConditionCriteraTypes.ElitesInRange;
					}
			 }
			 if ((!TestCustomConditions||FailedCondition)&&FUnitsInRangeConditions!=null)
			 {
					FailedCondition=false;
					foreach (Func<bool> item in this.FUnitsInRangeConditions.GetInvocationList())
					{
						 if (!item())
						 {
								FailedCondition=true;
								break;
						 }
					}
					if (!FailedCondition)
					{
						 LastConditionPassed=ConditionCriteraTypes.UnitsInRange;
						 TestCustomConditions=true;
					}
			 }
			 if ((!TestCustomConditions||FailedCondition)&&FClusterConditions!=null)
			 {
					FailedCondition=false;

					if (!this.FClusterConditions.Invoke())
					{
						 FailedCondition=true;
					}

					if (!FailedCondition)
					{
						 LastConditionPassed=ConditionCriteraTypes.Cluster;
						 TestCustomConditions=true;
					}
			 }
			 if ((!TestCustomConditions||FailedCondition)&&FSingleTargetUnitCriteria!=null)
			 {
					FailedCondition=false;
					foreach (Func<bool> item in this.FSingleTargetUnitCriteria.GetInvocationList())
					{
						 if (!item())
						 {
								FailedCondition=true;
								break;
						 }
					}
					if (!FailedCondition)
					{
						 LastConditionPassed=ConditionCriteraTypes.SingleTarget;
						 TestCustomConditions=true;
					}
			 }

			 //If TestCustomCondtion failed, and FailedCondition is true.. then we tested a combat condition.
			 //If FailedCondition is false, then we never tested a condition.
			 if (!TestCustomConditions&&FailedCondition) return false;


			 foreach (Func<bool> item in this.Fcriteria.GetInvocationList())
					if (!item()) return false;


			 return true;
		}
	}
}
