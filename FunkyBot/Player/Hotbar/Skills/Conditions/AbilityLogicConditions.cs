using System;
using FunkyBot.Cache.Objects;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace FunkyBot.Player.HotBar.Skills.Conditions
{
	 ///<summary>
	 ///Creates Funcs from a created Ability and is to be used in testing of usability.
	 ///</summary>
	 public static class AbilityLogicConditions
	 {

		  public static void CreateAbilityLogicConditions(ref Skill ability)
		  {
				//CreatePreCastConditions(ref ability.FcriteriaPreCast, ability);
				CreateTargetConditions(out ability.FSingleTargetUnitCriteria, ability);
				CreateUnitsInRangeConditions(out ability.FUnitsInRangeConditions, ability);
				CreateElitesInRangeConditions(out ability.FElitesInRangeConditions, ability);
				CreateClusterConditions(out ability.FClusterConditions, ability);

				//Check if the 4 primary combat conditions are null -- and if the custom condition is not..
				if (ability.FSingleTargetUnitCriteria==null&&ability.FUnitsInRangeConditions==null&&ability.FElitesInRangeConditions==null&&ability.FClusterConditions==null&&ability.FcriteriaCombat!=null)
					 ability.TestCustomCombatConditions=true;
		  }


		  #region Function Creation Methods
		  private static void CreateClusterConditions(out Func<bool> FClusterConditions, Skill ability)
		  {
			  FClusterConditions = null;

			  if (ability.ClusterConditions == null)
				  return;

				FClusterConditions=ability.ClusterConditions.Criteria;

				if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.ClusterTarget|AbilityExecuteFlags.ClusterTargetNearest))
				{
					 //Attach Line Of Sight Check to Criteria
					 CreateLineOfSightTargetCheck(ref FClusterConditions, ability);
				}
		  }

		  private static void CreateTargetConditions(out Func<bool> FSingleTargetUnitCriteria, Skill ability)
		  {
				//TODO:: Redesign to allow multiple single target conditions to be used.

				FSingleTargetUnitCriteria=null;

				//No Conditions Set by default.. (?? May have to verify Ability execution can be Target)
				//-- Ranged Abilities that do not set any single target conditions will never be checked for LOS.
				if (ability.SingleUnitCondition == null)
				{
					//No Default Conditions Set.. however if Ability uses target as a execution type then we implement the LOS conditions.
					if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.Target))
						FSingleTargetUnitCriteria += () => true;
					else
						return;
				}
				else
					FSingleTargetUnitCriteria = ability.SingleUnitCondition.Criteria;
					 //CreateTargetFlagConditions(ref FSingleTargetUnitCriteria, ability.SingleUnitCondition);	//Create conditions using TargetUnitCondition object

				//Attach Line Of Sight Check to Criteria
				CreateLineOfSightTargetCheck(ref FSingleTargetUnitCriteria, ability);
		  }

		  private static void CreateLineOfSightTargetCheck(ref Func<bool> CombatCriteria, Skill ability)
		  {
				if (ability.IsRanged)
				{
					 CombatCriteria+=() =>
					 {
						 if (!Bot.Targeting.CurrentUnitTarget.IgnoresLOSCheck && Bot.Targeting.CurrentUnitTarget.IsTargetableAndAttackable)
						 {
							 LOSInfo LOSINFO=Bot.Targeting.CurrentTarget.LineOfSight;
							 if (LOSINFO.LastLOSCheckMS>2000||!LOSINFO.NavCellProjectile.HasValue)
							 {
								 if (!LOSINFO.LOSTest(Bot.Character.Data.Position, true, false, NavCellFlags.AllowProjectile))
								 {
                                          

									 //Raycast failed.. reset LOS Check -- for valid checking.
									 if (!LOSINFO.RayCast.Value) 
										 Bot.Targeting.CurrentTarget.RequiresLOSCheck=true;
									 else if (!LOSINFO.NavCellProjectile.Value) //NavCellFlag Walk Failed
									 {
										 bool MovementException = ((Bot.Targeting.CurrentUnitTarget.MonsterTeleport || Bot.Targeting.CurrentTarget.IsTransformUnit) && Bot.Targeting.CurrentUnitTarget.AnimState == AnimationState.Transform);
										 if (!MovementException)
											 return false;
										 //else
										 //LOSINFO.NavCellProjectile=true;
									 }
								 }
							 }
							 else if (LOSINFO.NavCellProjectile.HasValue&&!LOSINFO.NavCellProjectile.Value)
							 {
								 return false;
							 }
						 }
						 return true;
					 };
				}
				else if (ability.Range>0)
				{//Melee
					 CombatCriteria+=() =>
					 {
						 if (!Bot.Targeting.CurrentUnitTarget.IgnoresLOSCheck && Bot.Targeting.CurrentUnitTarget.IsTargetableAndAttackable)
						 {
							 float radiusDistance=Bot.Targeting.CurrentTarget.RadiusDistance;
							 //Check if within interaction range..
							 if (radiusDistance>ability.Range)
							 {
								 //Verify LOS walk
								 LOSInfo LOSINFO=Bot.Targeting.CurrentTarget.LineOfSight;
								 if (LOSINFO.LastLOSCheckMS>2000)//||!LOSINFO.NavCellWalk.HasValue)
								 {
									 if (!LOSINFO.LOSTest(Bot.Character.Data.Position, true, false))
									 {
										 //bool MovementException=((Bot.Targeting.CurrentUnitTarget.MonsterTeleport||Bot.Targeting.CurrentTarget.IsTransformUnit)&&Bot.Targeting.CurrentUnitTarget.AnimState==Zeta.Internals.Actors.AnimationState.Transform);
										 //Raycast failed.. reset LOS Check -- for valid checking.
										 if (!LOSINFO.RayCast.Value)
											 Bot.Targeting.CurrentTarget.RequiresLOSCheck=true;
										 //else if (!LOSINFO.NavCellWalk.Value) //NavCellFlag Walk Failed
										 //{
										 //    bool MovementException = ((Bot.Targeting.CurrentUnitTarget.MonsterTeleport || Bot.Targeting.CurrentTarget.IsTransformUnit) && Bot.Targeting.CurrentUnitTarget.AnimState == Zeta.Internals.Actors.AnimationState.Transform);
										 //    if (!MovementException)
										 //        return false;
										 //}
									 }
								 }
								 //else if (LOSINFO.NavCellWalk.HasValue&&!LOSINFO.NavCellWalk.Value)
								 //{
								 //     return false;
								 //}
							 }
						 }
						 return true;
					 };
				}
		  }

		  //Quick research showed Enum.HasFlag is slower compared to the below method.
		  internal static bool CheckTargetPropertyFlag(TargetProperties property, TargetProperties flag)
		  {
				return (property&flag)!=0;
		  }

		  internal static TargetProperties EvaluateUnitProperties(CacheUnit unit)
		  {
				TargetProperties properties=TargetProperties.None;

				if (unit.IsBoss)
					 properties|=TargetProperties.Boss;

				if (unit.IsBurrowableUnit)
					 properties|=TargetProperties.Burrowing;

				if (unit.MonsterMissileDampening)
					 properties|=TargetProperties.MissileDampening;

				if (unit.IsMissileReflecting)
					 properties|=TargetProperties.MissileReflecting;

				if (unit.MonsterShielding)
					 properties|=TargetProperties.Shielding;

				if (unit.IsStealthableUnit)
					 properties|=TargetProperties.Stealthable;

				if (unit.IsSucideBomber)
					 properties|=TargetProperties.SucideBomber;

				if (unit.IsTreasureGoblin)
					 properties|=TargetProperties.TreasureGoblin;

				if (unit.IsFast)
					 properties|=TargetProperties.Fast;



				if (unit.IsEliteRareUnique)
					 properties|=TargetProperties.RareElite;

				if (unit.MonsterUnique)
					 properties|=TargetProperties.Unique;

				if (unit.ObjectIsSpecial)
					 properties|=TargetProperties.IsSpecial;

				if (unit.CurrentHealthPct.HasValue&&unit.CurrentHealthPct.Value==1d)
					 properties|=TargetProperties.FullHealth;

				if (unit.UnitMaxHitPointAverageWeight<0)
					 properties|=TargetProperties.Weak;


				if (unit.IsRanged)
					 properties|=TargetProperties.Ranged;


				if (unit.IsTargetableAndAttackable)
					 properties|=TargetProperties.TargetableAndAttackable;


				if (unit.HasDOTdps.HasValue&&unit.HasDOTdps.Value)
					 properties|=TargetProperties.DOTDPS;

				if (unit.RadiusDistance<10f)
					 properties|=TargetProperties.CloseDistance;

				if (unit.MonsterReflectDamage)
					 properties|=TargetProperties.ReflectsDamage;

                if (unit.MonsterElectrified)
                    properties |= TargetProperties.Electrified;

				return properties;
		  }

		  private static void CreateUnitsInRangeConditions(out Func<bool> FUnitRange, Skill ability)
		  {
				FUnitRange=null;
				if (ability.UnitsWithinRangeConditions!=null)
					 FUnitRange+=() => Bot.Targeting.Environment.iAnythingWithinRange[(int)ability.UnitsWithinRangeConditions.Item1]>=ability.UnitsWithinRangeConditions.Item2;
		  }

		  private static void CreateElitesInRangeConditions(out Func<bool> FUnitRange, Skill ability)
		  {
				FUnitRange=null;
				if (ability.ElitesWithinRangeConditions!=null)
					 FUnitRange+=() => Bot.Targeting.Environment.iElitesWithinRange[(int)ability.ElitesWithinRangeConditions.Item1]>=ability.ElitesWithinRangeConditions.Item2;
		  }
		  #endregion




	 }
}
