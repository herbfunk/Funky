using System;
using System.Linq;
using FunkyBot.AbilityFunky;
using FunkyBot.AbilityFunky.Abilities;
using FunkyBot.Cache;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.CommonBot;
using Zeta.Internals.SNO;

namespace FunkyBot.Character
{


		  ///<summary>
		  ///Used to describe the Current Player -- Class, Hotbar Abilities, Passives, etc..
		  ///</summary>
		  public abstract class Player
		  {
				//Base class for each individual class!
				public Player()
				{
					 HotBar.RefreshHotbar();
					 HotBar.RefreshPassives();
					 HotBar.UpdateRepeatAbilityTimes();

					 Ability healthPotionSkill=new DrinkHealthPotion();
					 AbilityLogicConditions.CreateAbilityLogicConditions(ref healthPotionSkill);
					 this.HealthPotionAbility=(DrinkHealthPotion)healthPotionSkill;

					 LastUsedAbility=this.DefaultAttack;
					 PowerPrime=this.DefaultAttack;
					 Logging.WriteVerbose("[Funky] Finished Creating Player Class");
				}


				///<summary>
				///The actor class of this bot.
				///</summary>
				internal virtual ActorClass AC { get { return ActorClass.Invalid; } }
				internal Hotbar HotBar = new Hotbar();

				///<summary>
				///This is used to determine things such as how we preform certain checks (I.E. Line Of Sight)
				///</summary>
				public virtual bool IsMeleeClass
				{
					 get { return false; }
				}


				// This is used so we don't use certain skills until we "top up" our primary resource by enough
				internal double iWaitingReservedAmount=0d;
				internal bool bWaitingForSpecial=false;
				
				///<summary>
				///Create Ability (Derieved classes override this!)
				///</summary>
				public virtual Ability CreateAbility(SNOPower P)
				{
					 return this.DefaultAttack;
				}

				public DrinkHealthPotion HealthPotionAbility=new DrinkHealthPotion();

				public virtual Ability DefaultAttack
				{
					 get { return new WeaponMeleeInsant(); }
				}

			  ///<summary>
			  ///
			  ///</summary>
			  public bool CanUseDefaultAttack { get; set; }

			  ///<summary>
				///Check if Bot should generate a new ZigZag location.
				///</summary>
				public virtual bool ShouldGenerateNewZigZagPath()
				{
					 return true;
				}

				///<summary>
				///Generates a new ZigZag location.
				///</summary>
				public virtual void GenerateNewZigZagPath() { }

				///<summary>
				///
				///</summary>
				public virtual int MainPetCount { get { return 0; } }

				///<summary>
				///Animations that determine if character has been "vortexed"
				///</summary>
				public virtual HashSet<SNOAnim> KnockbackLandAnims
				{
					 get { return null; }
				}



			  ///<summary>
				///Used to check for a secondary hotbar set. Currently only used for wizards with Archon.
				///</summary>
				public virtual bool SecondaryHotbarBuffPresent()
				{
					 return false;
				}


				public virtual void RecreateAbilities()
				{
					 Abilities=new Dictionary<SNOPower, Ability>();

					 //No default rage generation Ability.. then we add the Instant Melee Ability.
					 if (!HotBar.HotbarContainsAPrimaryAbility())
					 {
						  Ability defaultAbility=Bot.Class.DefaultAttack;
						  AbilityLogicConditions.CreateAbilityLogicConditions(ref defaultAbility);
						  Abilities.Add(defaultAbility.Power, defaultAbility);
						  HotBar.RuneIndexCache.Add(defaultAbility.Power, -1);
					 }


					 //Create the abilities
					 foreach (var item in HotBar.HotbarPowers)
					 {
						  Ability newAbility=Bot.Class.CreateAbility(item);
						  AbilityLogicConditions.CreateAbilityLogicConditions(ref newAbility);
						  newAbility.SuccessfullyUsed+=new Ability.AbilitySuccessfullyUsed(this.AbilitySuccessfullyUsed);
                         
                         //combat ability set property
                          if ((AbilityExecuteFlags.ClusterLocation | AbilityExecuteFlags.ClusterTarget | AbilityExecuteFlags.ClusterTargetNearest | AbilityExecuteFlags.Location | AbilityExecuteFlags.Target).HasFlag(newAbility.ExecutionType))
                              newAbility.IsCombat = true;

						  Abilities.Add(item, newAbility);
					 }

					 //Sort Abilities
					 SortedAbilities=Abilities.Values.OrderByDescending(a => a.Priority).ThenBy(a => a.Range).ToList();
				}



				///<summary>
				///Sets criteria based on object given.
				///</summary>
				public virtual Ability AbilitySelector(CacheUnit obj, bool IgnoreOutOfRange=false)
				{
					 //Reset default attack can use
					 this.CanUseDefaultAttack=!this.HotBar.HotbarPowers.Contains(this.DefaultAttack.Power)?false:true;
					 //Reset waiting for special!
					 this.bWaitingForSpecial = false;

					 ConditionCriteraTypes criterias=ConditionCriteraTypes.All;

					 //Although the unit is a cluster exception.. we should verify it is not a clustered object.
					 if (obj.IsClusterException&&obj.BeingIgnoredDueToClusterLogic)
					 {
						  criterias=ConditionCriteraTypes.SingleTarget;
					 }

					 return this.AbilitySelector(criterias, IgnoreOutOfRange);
				}
				///<summary>
				///Selects first Ability that is successful in precast and combat testing.
				///</summary>
				private Ability AbilitySelector(ConditionCriteraTypes criteria=ConditionCriteraTypes.All, bool IgnoreOutOfRange=false)
				{
					 Ability returnAbility=this.DefaultAttack;
					 foreach (var item in this.SortedAbilities)
					 {
						  //Check precast conditions
						  if (!item.CheckPreCastConditionMethod()) continue;

						  //Check Combat Conditions!
						  if (!item.CheckCombatConditionMethod(criteria))
						  {
								continue;
						  }

						  //Check if we can execute or if it requires movement
						  if (IgnoreOutOfRange)
						  {
								if (item.DestinationVector!=Bot.Character.Position)
									 continue;
						  }

						  returnAbility=item;
						  break;
					 }

					 //Setup Ability (sets vars according to current cache)
					 Ability.SetupAbilityForUse(ref returnAbility);
					 return returnAbility;
				}

				public List<Ability> ReturnAllUsableAbilities(CacheUnit obj, bool IgnoreOutOfRange=false)
				{
					 //Reset default attack can use
					 this.CanUseDefaultAttack=!this.Abilities.ContainsKey(this.DefaultAttack.Power)?false:true;

					 ConditionCriteraTypes criterias=ConditionCriteraTypes.All;

					 //Although the unit is a cluster exception.. we should verify it is not a clustered object.
					 if (obj.IsClusterException&&obj.BeingIgnoredDueToClusterLogic)
					 {
						  criterias=ConditionCriteraTypes.SingleTarget;
					 }

					 List<Ability> UsableAbilities=new List<Ability>();
					 foreach (var item in this.SortedAbilities)
					 {
						  //Check precast conditions
						  if (!item.CheckPreCastConditionMethod()) continue;

						  //Check Combat Conditions!
						  if (!item.CheckCombatConditionMethod(criterias))
						  {
								continue;
						  }

						  //Check if we can execute or if it requires movement
						  if (IgnoreOutOfRange)
						  {
								if (item.DestinationVector!=Bot.Character.Position)
									 continue;
						  }

						  Ability ability=item;
						  Ability.SetupAbilityForUse(ref ability);
						  UsableAbilities.Add(ability);
					 }

					 return UsableAbilities;
				}

				///<summary>
				///Returns Ability used for destructibles
				///</summary>
				public virtual Ability DestructibleAbility()
				{
					 Ability returnAbility=Bot.Class.DefaultAttack;
					 List<Ability> nonDestructibleAbilities=new List<Ability>();
					 foreach (var item in this.Abilities.Values)
					 {
						  if (item.IsADestructiblePower)
						  {

								//Check LOS -- Projectiles
								if (item.IsRanged&&!Bot.Targeting.CurrentTarget.IgnoresLOSCheck)
								{
									 LOSInfo LOSINFO=Bot.Targeting.CurrentTarget.LineOfSight;
									 if (LOSINFO.LastLOSCheckMS>3000||(item.IsProjectile&&!LOSINFO.ObjectIntersection.HasValue)||!LOSINFO.NavCellProjectile.HasValue)
									 {
										  if (!LOSINFO.LOSTest(Bot.Character.Position, true, false, NavCellFlags.AllowProjectile))
										  {
												//Raycast failed.. reset LOS Check -- for valid checking.
												if (!LOSINFO.RayCast.Value) Bot.Targeting.CurrentTarget.RequiresLOSCheck=true;
												continue;
										  }
									 }
									 else if (!LOSINFO.NavCellProjectile.Value)
									 {
										  continue;
									 }
								}

								if (item.CheckPreCastConditionMethod())
								{
									 returnAbility=item;
									 Ability.SetupAbilityForUse(ref returnAbility, true);
									 return returnAbility;
								}
						  }
						  else if (item.ExecutionType.HasFlag(AbilityExecuteFlags.Target)||item.ExecutionType.HasFlag(AbilityExecuteFlags.Location))
						  {

								//Check LOS -- Projectiles
								if (item.IsRanged&&!Bot.Targeting.CurrentTarget.IgnoresLOSCheck)
								{
									 LOSInfo LOSINFO=Bot.Targeting.CurrentTarget.LineOfSight;
									 if (LOSINFO.LastLOSCheckMS>3000||(item.IsProjectile&&!LOSINFO.ObjectIntersection.HasValue)||!LOSINFO.NavCellProjectile.HasValue)
									 {
										  if (!LOSINFO.LOSTest(Bot.Character.Position, true, item.IsProjectile, NavCellFlags.AllowProjectile))
										  {
												//Raycast failed.. reset LOS Check -- for valid checking.
												if (!LOSINFO.RayCast.Value) Bot.Targeting.CurrentTarget.RequiresLOSCheck=true;
												continue;
										  }
									 }
									 else if ((item.IsProjectile&&LOSINFO.ObjectIntersection.Value)||!LOSINFO.NavCellProjectile.Value)
									 {
										  continue;
									 }
								}

								//Add this Ability to our list.. incase we cant find an offical Ability to use.
								if (item.CheckPreCastConditionMethod())
								{
									 nonDestructibleAbilities.Add(item);
								}
						  }
					 }

					 //Use non-destructible Ability..
					 if (nonDestructibleAbilities.Count>0)
						  returnAbility=nonDestructibleAbilities[0];

					 Ability.SetupAbilityForUse(ref returnAbility, true);
					 return returnAbility;
				}

				


				internal Dictionary<SNOPower, Ability> Abilities=new Dictionary<SNOPower, Ability>();
				internal List<Ability> SortedAbilities=new List<Ability>();


				///<summary>
				///Searches for any abilities that have set the OutOfCombat Movement Criteria.
				///</summary>
				internal Vector3 FindOutOfCombatMovementPower(out Ability MovementAbility, Vector3 Destination)
				{
					 MovementAbility=null;
					 foreach (var item in this.Abilities.Values.Where(A => A.FOutOfCombatMovement!=null))
					 {

						  if (item.CheckPreCastConditionMethod())
						  {
								Vector3 AbilityTargetVector=item.FOutOfCombatMovement.Invoke(Destination);
								if (AbilityTargetVector!=Vector3.Zero)
								{
									 MovementAbility=item;
									 return AbilityTargetVector;
								}
						  }
					 }
					 return Vector3.Zero;
				}
				///<summary>
				///Searches for any abilities that have set the Combat Movement Criteria.
				///</summary>
				internal Vector3 FindCombatMovementPower(out Ability MovementAbility,Vector3 Destination)
				{
					 MovementAbility=null;
					 foreach (var item in this.Abilities.Values.Where(A => A.FCombatMovement!=null))
					 {

						  if (item.CheckPreCastConditionMethod())
						  {
								Vector3 AbilityTargetVector=item.FCombatMovement.Invoke(Destination);
								if (AbilityTargetVector!=Vector3.Zero)
								{
									 MovementAbility=item;
									 return AbilityTargetVector;
								}
						  }
					 }
					 return Vector3.Zero;
				}


				///<summary>
				///Returns a power for Buffing.
				///</summary>
				internal bool FindBuffPower(out Ability BuffAbility)
				{
					 BuffAbility=null;
					 foreach (var item in this.Abilities.Values.Where(A => A.IsBuff))
					 {
						  if (item.CheckPreCastConditionMethod())
						  {
								if (item.CheckBuffConditionMethod())
								{
									 BuffAbility=item;
									 Ability.SetupAbilityForUse(ref BuffAbility);
									 return true;
								}
						  }
					 }
					 return false;
				}

				///<summary>
				///Returns a power for Combat Buffing.
				///</summary>
				internal bool FindCombatBuffPower(out Ability BuffAbility)
				{
					 BuffAbility=null;
					 foreach (var item in this.Abilities.Values.Where(A => A.IsBuff&&A.UseageType.HasFlag(AbilityUseage.Combat|AbilityUseage.Anywhere)))
					 {
						  if (item.CheckPreCastConditionMethod())
						  {
								if (item.CheckCombatConditionMethod())
								{
									 BuffAbility=item;
									 Ability.SetupAbilityForUse(ref BuffAbility);
									 return true;
								}
						  }
					 }
					 return false;
				}


				public void DebugString()
				{
					Logging.Write("Character Information\r\nRadius {0}\r\nHotBar Abilities [{1}]\r\n", Bot.Character.fCharacterRadius, HotBar.HotbarPowers.Count);

					 foreach (SNOPower item in Bot.Class.HotBar.HotbarPowers)
					 {
						  Logging.Write("{0} with current rune index {1}", item.ToString(), HotBar.RuneIndexCache.ContainsKey(item)?HotBar.RuneIndexCache[item].ToString():"none");
					 }

					 Logging.Write("Created Abilities [{0}]", Abilities.Count);
					 foreach (var item in Abilities.Values)
					 {
						  Logging.Write("Power [{0}] -- Priority {1} --", item.Power.ToString(), item.Priority, item.DebugString());
					 }

					 Logging.Write("Current Buffs");
					 foreach (SNOPower item in HotBar.CurrentBuffs.Keys)
					 {
						  Logging.Write("Buff: {0}", Enum.GetName(typeof(SNOPower), item));
					 }
					 Logging.Write("Current Debuffs");
					 foreach (SNOPower item in HotBar.CurrentDebuffs)
					 {
						  Logging.Write("Debuff: {0}", Enum.GetName(typeof(SNOPower), item));
					 }

				}

				///<summary>
				///Last successful Ability used.
				///</summary>
				public Ability LastUsedAbility { get; set; }
                public DateTime LastUsedACombatAbility { get; set; }

				internal void AbilitySuccessfullyUsed(Ability ability, bool reorderAbilities)
				{
					if (ability.IsCombat)
                    {
                        this.LastUsedACombatAbility = DateTime.Now;
                    }

                    this.LastUsedAbility=ability;


					 //Only Sort When Non-Channeling!
					 if (reorderAbilities)
						  this.SortedAbilities=this.Abilities.Values.OrderByDescending(a => a.Priority).ThenByDescending(a => a.LastUsedMilliseconds).ToList();
				}

				internal Ability PowerPrime;

				internal static void CreateBotClass()
				{
					if (Bot.Game != null && Bot.Game.ActorClass != ActorClass.Invalid)
					{
						//Create Specific Player Class
						switch (Bot.Game.ActorClass)
						{
							case ActorClass.Barbarian:
								Bot.Class = new Barbarian();
								break;
							case ActorClass.DemonHunter:
								Bot.Class = new DemonHunter();
								break;
							case ActorClass.Monk:
								Bot.Class = new Monk();
								break;
							case ActorClass.WitchDoctor:
								Bot.Class = new WitchDoctor();
								break;
							case ActorClass.Wizard:
								Bot.Class = new Wizard();
								break;
						}

						Bot.Class.RecreateAbilities();
					}
				}
		  }
	 
}