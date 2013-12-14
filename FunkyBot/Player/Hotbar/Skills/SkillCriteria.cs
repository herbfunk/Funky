using System;
using System.Linq;
using FunkyBot.Movement.Clustering;
using FunkyBot.Player.HotBar.Skills.Conditions;
using Zeta.Common;

namespace FunkyBot.Player.HotBar.Skills
{

	 public abstract class SkillCriteria
	 {
		 protected SkillCriteria()
			{
				SingleUnitCondition = null;
				ElitesWithinRangeConditions = null;
				UnitsWithinRangeConditions = null;
				ClusterConditions = null;
				TestCustomCombatConditions=false;
				 FcriteriaCombat=() => true;
				 FcriteriaBuff=() => true;
				PreCast=new SkillPreCast();
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
			internal SkillPreCast PreCast;
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
		 ///Describes values for clustering used for target (Cdistance, DistanceFromBot, MinUnits, IgnoreNonTargetable)
		 ///</summary>
		 ///<value>
		 ///Clustering Distance, Distance From Bot, Minimum Unit Count, Ignore Non-Targetables
		 ///</value>
		 public SkillClusterConditions ClusterConditions { get; set; }


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
		 public UnitTargetConditions SingleUnitCondition { get; set; }
	 }

}
