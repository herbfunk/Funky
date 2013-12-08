using System;
using System.Linq;
using FunkyBot.Movement.Clustering;
using Zeta.Common;

namespace FunkyBot.Player.HotBar.Skills
{

	 public abstract class SkillCriteria
	 {
			public SkillCriteria()
			{
				TargetUnitConditionFlags = null;
				ElitesWithinRangeConditions = null;
				UnitsWithinRangeConditions = null;
				ClusterConditions = null;
				TestCustomCombatConditions=false;
				 FcriteriaPreCast=() => true;
				 FcriteriaCombat=() => true;
				 FcriteriaBuff=() => true;
			}
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
			///Ability precast conditions
			///</summary>
			internal Func<bool> FcriteriaPreCast;
			///<summary>
			///Custom Conditions for Combat
			///</summary>
			internal Func<bool> FcriteriaCombat;
			///<summary>
			///Custom Conditions for Buffing
			///</summary>
			internal Func<bool> FcriteriaBuff;

			internal Func<bool> FClusterConditions;
			internal Func<bool> FUnitsInRangeConditions;
			internal Func<bool> FElitesInRangeConditions;
			internal Func<bool> FSingleTargetUnitCriteria;

			///<summary>
			///Used during Player Movement
			///</summary>
			internal Func<Vector3, Vector3> FOutOfCombatMovement;
			///<summary>
			///Used during Target Movement
			///</summary>
			internal Func<Vector3, Vector3> FCombatMovement;

			///<summary>
			///Determines if an Ability has none of the combat conditions set -- but has custom combat conditions. (allowing it to test the custom conditions)
			///</summary>
			internal bool TestCustomCombatConditions;

			///<summary>
			///Check Ability Buff Conditions
			///</summary>
			public bool CheckBuffConditionMethod()
			{
				 foreach (Func<bool> item in FcriteriaBuff.GetInvocationList())
				 {
						if (!item()) return false;
				 }

				 return true;
			}

			public bool CheckCustomCombatMethod()
			{
				 foreach (Func<bool> item in FcriteriaCombat.GetInvocationList())
					  if (!item()) return false;

				 return true;
			}
			///<summary>
			///Check Ability is valid to use.
			///</summary>
			public bool CheckPreCastConditionMethod()
			{
				return FcriteriaPreCast.GetInvocationList().Cast<Func<bool>>().All(item => item());
			}

		 ///<summary>
			///Check Combat
			///</summary>
			public bool CheckCombatConditionMethod(ConditionCriteraTypes conditions=ConditionCriteraTypes.All)
			{
				 //Order in which tests are conducted..

				 //Units in Range (Not Cluster)
				 //Clusters
				 //Single Target

				 //If all are null or any of them are successful, then we test Custom Conditions
				 //Custom Condition

				 //Reset Last Condition
				 LastConditionPassed=ConditionCriteraTypes.None;
				 bool TestCustomConditions=false;
				 bool FailedCondition=false;

				 if (conditions.HasFlag(ConditionCriteraTypes.ElitesInRange)&&FElitesInRangeConditions!=null)
				 {
					  foreach (Func<bool> item in FElitesInRangeConditions.GetInvocationList())
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
				 if ((!TestCustomConditions||FailedCondition)&&conditions.HasFlag(ConditionCriteraTypes.UnitsInRange)&&FUnitsInRangeConditions!=null)
				 {
					  FailedCondition=false;
					  foreach (Func<bool> item in FUnitsInRangeConditions.GetInvocationList())
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
				 if ((!TestCustomConditions||FailedCondition)&&conditions.HasFlag(ConditionCriteraTypes.Cluster)&&FClusterConditions!=null)
				 {
					  FailedCondition=false;

					  if (!FClusterConditions.Invoke())
					  {
							FailedCondition=true;
					  }

					  if (!FailedCondition)
					  {
							LastConditionPassed=ConditionCriteraTypes.Cluster;
							TestCustomConditions=true;
					  }
				 }
				 if ((!TestCustomConditions||FailedCondition)&&conditions.HasFlag(ConditionCriteraTypes.SingleTarget)&&FSingleTargetUnitCriteria!=null)
				 {
					  FailedCondition=false;
					  foreach (Func<bool> item in FSingleTargetUnitCriteria.GetInvocationList())
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
				 if (!TestCustomConditions&&!TestCustomCombatConditions) return false; //&&FailedCondition


				 foreach (Func<bool> item in FcriteriaCombat.GetInvocationList())
					  if (!item()) return false;


				 return true;
			}



			///<summary>
			///Describes the pre casting conditions - when set it will create the precast method used.
			///</summary>
			public AbilityPreCastFlags PreCastFlags
			{
				 get
				 {
						return precastconditions_;
				 }
				 set
				 {
						precastconditions_=value;

				 }
			}
			private AbilityPreCastFlags precastconditions_=AbilityPreCastFlags.None;

		 ///<summary>
		 ///Describes values for clustering used for target (Cdistance, DistanceFromBot, MinUnits, IgnoreNonTargetable)
		 ///</summary>
		 ///<value>
		 ///Clustering Distance, Distance From Bot, Minimum Unit Count, Ignore Non-Targetables
		 ///</value>
		 public ClusterConditions ClusterConditions { get; set; }


		 ///<summary>
		 ///Units within Range Conditions
		 ///</summary>
		 public Tuple<RangeIntervals, int> UnitsWithinRangeConditions { get; set; }

		 ///<summary>
		 ///Elites within Range Conditions
		 ///</summary>
		 public Tuple<RangeIntervals, int> ElitesWithinRangeConditions { get; set; }

		 ///<summary>
		 ///Single Target Conditions -- Should only used if Ability is offensive!
		 ///</summary>
		 public UnitTargetConditions TargetUnitConditionFlags { get; set; }
	 }

}
