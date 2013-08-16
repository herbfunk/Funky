using System;
using FunkyTrinity.Enums;
using Zeta.CommonBot;
using Zeta.Internals.SNO;

namespace FunkyTrinity.ability
{

	 public abstract class AbilityCriteria
	 {
			public AbilityCriteria()
			{
				 Fcriteria=new Func<bool>(() => { return true; });
				 AbilityTestConditions=new AbilityUsablityTests();
			}

			public virtual AbilityUsablityTests AbilityTestConditions
			{
				 get;
				 set;
			}

			///<summary>
			///Custom Conditions for Combat
			///</summary>
			internal Func<bool> Fcriteria;
			///<summary>
			///Custom Conditions for Buffing
			///</summary>
			internal Func<bool> Fbuff;
			///<summary>
			///Check Ability Buff Conditions
			///</summary>
			public bool CheckBuffConditionMethod()
			{
				 foreach (Func<bool> item in Fbuff.GetInvocationList())
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
			///Describes the pre casting conditions - when set it will create the precast method used.
			///</summary>
			public AbilityConditions PreCastConditions
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
			private AbilityConditions precastconditions_=ability.AbilityConditions.None;

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
