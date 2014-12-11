using System;
using System.Collections.Generic;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using Zeta.Common;

namespace fBaseXtensions.Game.Hero.Skills
{

	public abstract class SkillCriteria
	{
		protected SkillCriteria()
		{
			SingleUnitCondition = new List<UnitTargetConditions>();
			ClusterConditions = new List<SkillClusterConditions>();
			TestCustomCombatConditions = false;
			FcriteriaCombat = (u) => true;
			FcriteriaBuff = () => true;
			PreCast = new SkillPreCast();
		}
		///<summary>
		///Tracks last successful condition if any.
		///</summary>
		public ConditionCriteraTypes LastConditionPassed
		{
			get { return lastConditionPassed; }
			set { lastConditionPassed = value; }
		}
		private ConditionCriteraTypes lastConditionPassed = ConditionCriteraTypes.None;

		///<summary>
		///Ability precast conditions
		///</summary>
		internal SkillPreCast PreCast;
		///<summary>
		///Custom Conditions for Combat
		///</summary>
		internal Func<CacheUnit, bool> FcriteriaCombat;
		///<summary>
		///Custom Conditions for Buffing
		///</summary>
		internal Func<bool> FcriteriaBuff;

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
		public List<SkillClusterConditions> ClusterConditions { get; set; }
		internal SkillClusterConditions LastClusterConditionSuccessful { get; set; }

		///<summary>
		///Single Target Conditions -- Should only used if Ability is offensive!
		///</summary>
		public List<UnitTargetConditions> SingleUnitCondition { get; set; }
	}

}
