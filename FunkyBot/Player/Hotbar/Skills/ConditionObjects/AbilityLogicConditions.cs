using System;
using FunkyBot.Cache;
using Zeta.CommonBot;
using Zeta.Internals.SNO;

namespace FunkyBot.Player.HotBar.Skills
{
	 ///<summary>
	 ///Creates Funcs from a created Ability and is to be used in testing of usability.
	 ///</summary>
	 public static class AbilityLogicConditions
	 {

		  public static void CreateAbilityLogicConditions(ref Skill ability)
		  {
				CreatePreCastConditions(ref ability.FcriteriaPreCast, ability);
				CreateTargetConditions(ref ability.FSingleTargetUnitCriteria, ability);
				CreateUnitsInRangeConditions(ref ability.FUnitsInRangeConditions, ability);
				CreateElitesInRangeConditions(ref ability.FElitesInRangeConditions, ability);
				CreateClusterConditions(ref ability.FClusterConditions, ability);

				//Check if the 4 primary combat conditions are null -- and if the custom condition is not..
				if (ability.FSingleTargetUnitCriteria==null&&ability.FUnitsInRangeConditions==null&&ability.FElitesInRangeConditions==null&&ability.FClusterConditions==null&&ability.FcriteriaCombat!=null)
					 ability.TestCustomCombatConditions=true;
		  }


		  #region Function Creation Methods
		  private static void CreateClusterConditions(ref Func<bool> FClusterConditions, Skill ability)
		  {
				FClusterConditions=null;
				if (ability.ClusterConditions==null) return;

				FClusterConditions=() => Skill.CheckClusterConditions(ability.ClusterConditions);

				if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.ClusterTarget|AbilityExecuteFlags.ClusterTargetNearest))
				{
					 //Attach Line Of Sight Check to Criteria
					 CreateLineOfSightTargetCheck(ref FClusterConditions, ability);
				}
		  }

		  private static void CreatePreCastConditions(ref Func<bool> Fprecast, Skill ability)
		  {
				AbilityPreCastFlags precastconditions_=ability.PreCastFlags;
				if (precastconditions_.Equals(AbilityPreCastFlags.None))
					 return;
			  if (precastconditions_.HasFlag(AbilityPreCastFlags.CheckPlayerIncapacitated))
				  Fprecast+=(() => !Bot.Character.Data.bIsIncapacitated);

			  if (precastconditions_.HasFlag(AbilityPreCastFlags.CheckPlayerRooted))
				  Fprecast+=(() => !Bot.Character.Data.bIsRooted);

			  if (precastconditions_.HasFlag(AbilityPreCastFlags.CheckExisitingBuff))
				  Fprecast+=(() => !Bot.Character.Class.HotBar.HasBuff(ability.Power));

			  if (precastconditions_.HasFlag(AbilityPreCastFlags.CheckPetCount))
				  Fprecast+=(() => Bot.Character.Class.MainPetCount<ability.Counter);

			  if (precastconditions_.HasFlag(AbilityPreCastFlags.CheckRecastTimer))
				  Fprecast+=(() => ability.LastUsedMilliseconds>ability.Cooldown);

			  if (precastconditions_.HasFlag(AbilityPreCastFlags.CheckCanCast))
			  {
				  Fprecast+=(() =>
				  {
					  bool cancast=PowerManager.CanCast(ability.Power, out ability.CanCastFlags);


					  if (!cancast&&ability.CanCastFlags.HasFlag(PowerManager.CanCastFlags.PowerNotEnoughResource))
					  {
						  if (ability.IsSpecialAbility)
							  Bot.Character.Class.bWaitingForSpecial=true;

						  if (ability.IsRanged||ability.Range>0)
							  Bot.Character.Class.CanUseDefaultAttack=true;
					  }
					  //else if (ability.IsSpecialAbility)
					  //	 Bot.Character_.Class.bWaitingForSpecial=false;

					  return cancast;
				  });
			  }

			  if (precastconditions_.HasFlag(AbilityPreCastFlags.CheckEnergy))
			  {
				  if (!ability.SecondaryEnergy)
					  Fprecast+=(() =>
					  {
						  bool energyCheck=Bot.Character.Data.dCurrentEnergy>=ability.Cost;
						  if (ability.IsSpecialAbility&&!energyCheck) //we trigger waiting for special here.
							  Bot.Character.Class.bWaitingForSpecial=true;
						  if (!energyCheck&&(ability.IsRanged||ability.Range>0))
							  Bot.Character.Class.CanUseDefaultAttack=true;

						  return energyCheck;
					  });
				  else
					  Fprecast+=(() =>
					  {
						  bool energyCheck=Bot.Character.Data.dDiscipline>=ability.Cost;
						  if (ability.IsSpecialAbility&&!energyCheck) //we trigger waiting for special here.
							  Bot.Character.Class.bWaitingForSpecial=true;

						  if (!energyCheck&&(ability.IsRanged||ability.Range>0))
							  Bot.Character.Class.CanUseDefaultAttack=true;

						  return energyCheck;
					  });
			  }
		  }

		  private static void CreateTargetConditions(ref Func<bool> FSingleTargetUnitCriteria, Skill ability)
		  {
				//TODO:: Redesign to allow multiple single target conditions to be used.

				FSingleTargetUnitCriteria=null;

				//No Conditions Set by default.. (?? May have to verify Ability execution can be Target)
				//-- Ranged Abilities that do not set any single target conditions will never be checked for LOS.
				if (ability.TargetUnitConditionFlags==null)
				{
					 //No Default Conditions Set.. however if Ability uses target as a execution type then we implement the LOS conditions.
					 if (ability.ExecutionType.HasFlag(AbilityExecuteFlags.Target))
						  FSingleTargetUnitCriteria+=() => true;
					 else
						  return;
				}
				else
					 CreateTargetFlagConditions(ref FSingleTargetUnitCriteria, ability.TargetUnitConditionFlags);	//Create conditions using TargetUnitCondition object

				//Attach Line Of Sight Check to Criteria
				CreateLineOfSightTargetCheck(ref FSingleTargetUnitCriteria, ability);
		  }

		  private static void CreateLineOfSightTargetCheck(ref Func<bool> CombatCriteria, Skill ability)
		  {
				if (ability.IsRanged)
				{
					 CombatCriteria+=() =>
					 {
						 if (!Bot.Targeting.CurrentUnitTarget.IgnoresLOSCheck&&Bot.Targeting.CurrentUnitTarget.IsTargetableAndAttackable)
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
										 bool MovementException = ((Bot.Targeting.CurrentUnitTarget.MonsterTeleport || Bot.Targeting.CurrentTarget.IsTransformUnit) && Bot.Targeting.CurrentUnitTarget.AnimState == Zeta.Internals.Actors.AnimationState.Transform);
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
						 if (!Bot.Targeting.CurrentUnitTarget.IgnoresLOSCheck&&Bot.Targeting.CurrentUnitTarget.IsTargetableAndAttackable)
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

		  //We take the enums given by UnitTargetConditions (Ability property) and add any new conditions to the func for testing
		  private static void CreateTargetFlagConditions(ref Func<bool> FSingleTargetUnitCriteria, UnitTargetConditions TargetUnitConditionFlags_)
		  {
				//Distance
				if (TargetUnitConditionFlags_.Distance>-1)
					 FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentTarget.RadiusDistance<=TargetUnitConditionFlags_.Distance;
				//Health
				if (TargetUnitConditionFlags_.HealthPercent>0d)
					 FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentUnitTarget.CurrentHealthPct.Value<=TargetUnitConditionFlags_.HealthPercent;


				//TRUE CONDITIONS
				if (TargetUnitConditionFlags_.TrueConditionFlags.Equals(TargetProperties.None))
					 FSingleTargetUnitCriteria+=() => true;
				else
				{
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.Boss))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentTarget.IsBoss;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.Burrowing))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentTarget.IsBurrowableUnit;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.FullHealth))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentUnitTarget.CurrentHealthPct.Value==1d;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.IsSpecial))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentTarget.ObjectIsSpecial;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.Weak))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentUnitTarget.UnitMaxHitPointAverageWeight<0;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.MissileDampening))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentUnitTarget.MonsterMissileDampening;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.RareElite))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentUnitTarget.IsEliteRareUnique;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.MissileReflecting))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentTarget.IsMissileReflecting;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.Shielding))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentUnitTarget.MonsterShielding;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.Stealthable))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentTarget.IsStealthableUnit;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.SucideBomber))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentTarget.IsSucideBomber;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.TreasureGoblin))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentTarget.IsTreasureGoblin;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.Unique))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentUnitTarget.MonsterUnique;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.Ranged))
						 FSingleTargetUnitCriteria += () => Bot.Targeting.CurrentUnitTarget.IsRanged;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.TargetableAndAttackable))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentUnitTarget.IsTargetableAndAttackable;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.Fast))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentUnitTarget.IsFast;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.DOTDPS))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentUnitTarget.HasDOTdps.HasValue&&Bot.Targeting.CurrentUnitTarget.HasDOTdps.Value;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.CloseDistance))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentTarget.RadiusDistance<10f;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.ReflectsDamage))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentUnitTarget.MonsterReflectDamage;
                     if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.TrueConditionFlags, TargetProperties.Electrified))
                         FSingleTargetUnitCriteria += () => Bot.Targeting.CurrentUnitTarget.MonsterElectrified;
				}

				//FALSE CONDITIONS
				if (TargetUnitConditionFlags_.FalseConditionFlags.Equals(TargetProperties.None))
					 FSingleTargetUnitCriteria+=() => true;
				else
				{
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.Boss))
						  FSingleTargetUnitCriteria+=() => !Bot.Targeting.CurrentTarget.IsBoss;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.Burrowing))
						  FSingleTargetUnitCriteria+=() => !Bot.Targeting.CurrentTarget.IsBurrowableUnit;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.FullHealth))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentUnitTarget.CurrentHealthPct.Value!=1d;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.IsSpecial))
						  FSingleTargetUnitCriteria+=() => !Bot.Targeting.CurrentTarget.ObjectIsSpecial;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.Weak))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentUnitTarget.UnitMaxHitPointAverageWeight>0;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.MissileDampening))
						  FSingleTargetUnitCriteria+=() => !Bot.Targeting.CurrentUnitTarget.MonsterMissileDampening;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.RareElite))
						  FSingleTargetUnitCriteria+=() => !Bot.Targeting.CurrentUnitTarget.IsEliteRareUnique;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.MissileReflecting))
						  FSingleTargetUnitCriteria+=() => !Bot.Targeting.CurrentTarget.IsMissileReflecting;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.Shielding))
						  FSingleTargetUnitCriteria+=() => !Bot.Targeting.CurrentUnitTarget.MonsterShielding;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.Stealthable))
						  FSingleTargetUnitCriteria+=() => !Bot.Targeting.CurrentTarget.IsStealthableUnit;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.SucideBomber))
						  FSingleTargetUnitCriteria+=() => !Bot.Targeting.CurrentTarget.IsSucideBomber;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.TreasureGoblin))
						  FSingleTargetUnitCriteria+=() => !Bot.Targeting.CurrentTarget.IsTreasureGoblin;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.Unique))
						  FSingleTargetUnitCriteria+=() => !Bot.Targeting.CurrentUnitTarget.MonsterUnique;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.Ranged))
						 FSingleTargetUnitCriteria += () => !Bot.Targeting.CurrentUnitTarget.IsRanged;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.TargetableAndAttackable))
						  FSingleTargetUnitCriteria+=() => !Bot.Targeting.CurrentUnitTarget.IsTargetableAndAttackable;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.Fast))
						  FSingleTargetUnitCriteria+=() => !Bot.Targeting.CurrentUnitTarget.IsFast;
					 if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.DOTDPS))
						  FSingleTargetUnitCriteria+=() => !Bot.Targeting.CurrentUnitTarget.HasDOTdps.HasValue||!Bot.Targeting.CurrentUnitTarget.HasDOTdps.Value;
                     if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.CloseDistance))
						  FSingleTargetUnitCriteria+=() => Bot.Targeting.CurrentTarget.RadiusDistance>10f;
                     if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.ReflectsDamage))
						  FSingleTargetUnitCriteria+=() => !Bot.Targeting.CurrentUnitTarget.MonsterReflectDamage;
                     if (CheckTargetPropertyFlag(TargetUnitConditionFlags_.FalseConditionFlags, TargetProperties.Electrified))
                         FSingleTargetUnitCriteria += () => !Bot.Targeting.CurrentUnitTarget.MonsterElectrified;
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

		  private static void CreateUnitsInRangeConditions(ref Func<bool> FUnitRange, Skill ability)
		  {
				FUnitRange=null;
				if (ability.UnitsWithinRangeConditions!=null)
					 FUnitRange+=() => { return Bot.Targeting.Environment.iAnythingWithinRange[(int)ability.UnitsWithinRangeConditions.Item1]>=ability.UnitsWithinRangeConditions.Item2; };
		  }

		  private static void CreateElitesInRangeConditions(ref Func<bool> FUnitRange, Skill ability)
		  {
				FUnitRange=null;
				if (ability.ElitesWithinRangeConditions!=null)
					 FUnitRange+=() => { return Bot.Targeting.Environment.iElitesWithinRange[(int)ability.ElitesWithinRangeConditions.Item1]>=ability.ElitesWithinRangeConditions.Item2; };
		  }
		  #endregion




	 }
}
