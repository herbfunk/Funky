using System;

using Zeta.CommonBot;
using Zeta.Internals.SNO;

namespace FunkyTrinity.Ability
{

	 public abstract class AbilityCriteria
	 {
			public AbilityCriteria()
			{
				 Fprecast=new Func<bool>(() => { return true; });
				 Fcriteria=new Func<bool>(() => { return true; });
				 Fbuff=new Func<bool>(() => { return true; });
				 FMovement=new Func<bool>(() => { return true; });
				 
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
			///ability precast conditions
			///</summary>
			internal Func<bool> Fprecast;
			///<summary>
			///Custom Conditions for Combat
			///</summary>
			internal Func<bool> Fcriteria;
			///<summary>
			///Custom Conditions for Buffing
			///</summary>
			internal Func<bool> Fbuff;
			///<summary>
			///Custom Conditions for Movement Abilities
			///</summary>
			internal Func<bool> FMovement;

			internal Func<bool> FClusterConditions;
			internal Func<bool> FUnitsInRangeConditions;
			internal Func<bool> FElitesInRangeConditions;
			internal Func<bool> FSingleTargetUnitCriteria;



			///<summary>
			///Check ability Buff Conditions
			///</summary>
			public bool CheckBuffConditionMethod()
			{
				 foreach (Func<bool> item in Fbuff.GetInvocationList())
				 {
						if (!item()) return false;
				 }

				 return true;
			}
			///<summary>
			///Check ability movement Conditions
			///</summary>
			public bool CheckMovementConditionMethod()
			{
				 foreach (Func<bool> item in FMovement.GetInvocationList())
				 {
					  if (!item()) return false;
				 }

				 return true;
			}
		 public bool CheckCustomCombatMethod()
		 {
				foreach (Func<bool> item in this.Fcriteria.GetInvocationList())
					 if (!item()) return false;

				return true;
		 }
		 ///<summary>
		 ///Check ability is valid to use.
		 ///</summary>
		 public bool CheckPreCastConditionMethod()
		 {
			  foreach (Func<bool> item in Fprecast.GetInvocationList())
			  {
					if (!item()) return false;
			  }


			  return true;
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
			  if ((!TestCustomConditions||FailedCondition)&&conditions.HasFlag(ConditionCriteraTypes.UnitsInRange)&&FUnitsInRangeConditions!=null)
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
			  if ((!TestCustomConditions||FailedCondition)&&conditions.HasFlag(ConditionCriteraTypes.Cluster)&&FClusterConditions!=null)
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
			  if ((!TestCustomConditions||FailedCondition)&&conditions.HasFlag(ConditionCriteraTypes.SingleTarget)&&FSingleTargetUnitCriteria!=null)
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

			///<summary>
			///Describes the pre casting conditions - when set it will create the precast method used.
			///</summary>
			public AbilityPreCastFlags PreCastPreCastFlags
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
			public ClusterConditions ClusterConditions
			{
				 get { return ClusterConditions_; }
				 set
				 {
						ClusterConditions_=value;
				 }
			}
			private ClusterConditions ClusterConditions_=null;


			///<summary>
			///Units within Range Conditions
			///</summary>
			public Tuple<RangeIntervals, int> UnitsWithinRangeConditions
			{
				 get { return UnitsWithinRangeConditions_; }
				 set
				 {
						UnitsWithinRangeConditions_=value;
				 }
			}
			private Tuple<RangeIntervals, int> UnitsWithinRangeConditions_=null;

			///<summary>
			///Elites within Range Conditions
			///</summary>
			public Tuple<RangeIntervals, int> ElitesWithinRangeConditions
			{
				 get { return elitesWithinRangeConditions; }
				 set { elitesWithinRangeConditions=value; }
			}
			private Tuple<RangeIntervals, int> elitesWithinRangeConditions=null;

			///<summary>
			///Single Target Conditions -- Should only used if ability is offensive!
			///</summary>
			public UnitTargetConditions TargetUnitConditionFlags
			{
				 get { return targetUnitConditionFlags; }
				 set { targetUnitConditionFlags=value; }
			}
			private UnitTargetConditions targetUnitConditionFlags=null;




	 }

}
