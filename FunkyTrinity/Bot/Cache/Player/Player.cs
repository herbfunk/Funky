using System;
using System.Linq;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.CommonBot;
using Zeta.Internals.SNO;
using FunkyTrinity.ability;
using FunkyTrinity.Cache;

namespace FunkyTrinity
{


		  ///<summary>
		  ///Used to describe the Current Player -- Class, Hotbar Abilities, Passives, etc..
		  ///</summary>
		  public abstract class Player
		  {
				//Base class for each individual class!
				public Player(ActorClass a)
				{
					 AC=a;
					 RefreshHotbar();
					 RefreshPassives();
					 UpdateRepeatAbilityTimes();

					 Logging.WriteVerbose("[Funky] Finished Creating Player Class");
				}


				///<summary>
				///The actor class of this bot.
				///</summary>
				internal readonly ActorClass AC;

				private bool IsMeleeClass_=false;
				///<summary>
				///This is used to determine things such as how we preform certain checks (I.E. Line Of Sight)
				///</summary>
				public virtual bool IsMeleeClass
				{
					 get { return IsMeleeClass_; }
					 set { IsMeleeClass_=value; }
				}

				private LOSConditions losconditions;
				///<summary>
				///This is used to determine things such as how we preform certain checks (I.E. Line Of Sight)
				///</summary>
				public LOSConditions LOSconditions
				{
					 get { return losconditions; }
					 set { losconditions=value; }
				}

				//method to go thru the abilities and recreate the LOSConditions used.
				internal void UpdateLOSConditions()
				{
					 losconditions=new LOSConditions(this.Abilities);
				}

				// This is used so we don't use certain skills until we "top up" our primary resource by enough
				internal double iWaitingReservedAmount=0d;
				internal bool bWaitingForSpecial=false;
				
				///<summary>
				///Create ability (Derieved classes override this!)
				///</summary>
				public virtual Ability CreateAbility(SNOPower P)
				{
					 return this.DefaultAttack;
				}

			  public virtual Ability DefaultAttack
			  {
				  get { return new ability.Abilities.WeaponMeleeInsant(); }
			  }

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
				///Used to check for a secondary hotbar set. Currently only used for wizards with Archon.
				///</summary>
				public virtual bool SecondaryHotbarBuffPresent()
				{
					 if (AC==ActorClass.Wizard)
					 {
						  bool ArchonBuffPresent=this.HasBuff(SNOPower.Wizard_Archon);

						  //Confirm we don't have archon ability without archon buff.
						  bool RefreshNeeded=((!ArchonBuffPresent&&Abilities.ContainsKey(SNOPower.Wizard_Archon_ArcaneBlast))
													 ||(ArchonBuffPresent&&!Abilities.ContainsKey(SNOPower.Wizard_Archon_ArcaneBlast)));

						  if (RefreshNeeded)
						  {
								Logging.WriteVerbose("Updating Hotbar abilities!");
								CachedPowers=new HashSet<SNOPower>(HotbarPowers);
								RefreshHotbar();
								UpdateRepeatAbilityTimes();
								RecreateAbilities();
								return true;
						  }
					 }
					 return false;
				}


				public virtual void RecreateAbilities()
				{
					
				}

				///<summary>
				///Checks if hotbar contains any of the "Primary" abilities.
				///</summary>
				public bool HotbarContainsAPrimaryAbility()
				{
					 return this.HotbarPowers.Any(p => PowerCacheLookup.PrimaryAbilities.Contains(p));
				}


				///<summary>
				///Selects first ability that is successful in precast and combat testing.
				///</summary>
				public virtual Ability AbilitySelector(ConditionCriteraTypes criteria=ConditionCriteraTypes.All)
				{
					 Ability returnAbility=this.DefaultAttack;
					 foreach (var item in this.SortedAbilities)
					 {
						  //Check precast conditions
						  if (!item.AbilityTestConditions.CheckPreCastConditionMethod()) continue;

						  //Check Combat Conditions!
						  if (!item.AbilityTestConditions.CheckCombatConditionMethod(criteria))
						  {
								continue;
						  }

						  returnAbility=item;
						  break;
					 }

					 //Setup Ability (sets vars according to current cache)
					 Ability.SetupAbilityForUse(ref returnAbility);
					 return returnAbility;
				}

				///<summary>
				///Returns ability used for destructibles
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
								if (item.IsRanged&&!Bot.Target.CurrentTarget.IgnoresLOSCheck)
								{
									 ability.LOSInfo LOSINFO=Bot.Target.CurrentTarget.LineOfSight;
									 if (LOSINFO.LastLOSCheckMS>3000||(item.IsProjectile&&!LOSINFO.ObjectIntersection.HasValue)||!LOSINFO.NavCellProjectile.HasValue)
									 {
										  if (!LOSINFO.LOSTest(Bot.Character.Position, true, item.IsProjectile, NavCellFlags.AllowProjectile))
										  {
												//Raycast failed.. reset LOS Check -- for valid checking.
												if (!LOSINFO.RayCast.Value) Bot.Target.CurrentTarget.RequiresLOSCheck=true;
												continue;
										  }
									 }
									 else if ((item.IsProjectile&&LOSINFO.ObjectIntersection.Value)||!LOSINFO.NavCellProjectile.Value)
									 {
										  continue;
									 }
								}

								if (item.AbilityTestConditions.CheckPreCastConditionMethod())
								{
									 returnAbility=item;
									 Ability.SetupAbilityForUse(ref returnAbility, true);
									 return returnAbility;
								}
						  }
						  else if (item.ExecutionType.HasFlag(ability.AbilityUseType.Target)||item.ExecutionType.HasFlag(AbilityUseType.Location))
						  {

								//Check LOS -- Projectiles
								if (item.IsRanged&&!Bot.Target.CurrentTarget.IgnoresLOSCheck)
								{
									 ability.LOSInfo LOSINFO=Bot.Target.CurrentTarget.LineOfSight;
									 if (LOSINFO.LastLOSCheckMS>3000||(item.IsProjectile&&!LOSINFO.ObjectIntersection.HasValue)||!LOSINFO.NavCellProjectile.HasValue)
									 {
										  if (!LOSINFO.LOSTest(Bot.Character.Position, true, item.IsProjectile, NavCellFlags.AllowProjectile))
										  {
												//Raycast failed.. reset LOS Check -- for valid checking.
												if (!LOSINFO.RayCast.Value) Bot.Target.CurrentTarget.RequiresLOSCheck=true;
												continue;
										  }
									 }
									 else if ((item.IsProjectile&&LOSINFO.ObjectIntersection.Value)||!LOSINFO.NavCellProjectile.Value)
									 {
										  continue;
									 }
								}

								//Add this ability to our list.. incase we cant find an offical ability to use.
								if (item.AbilityTestConditions.CheckPreCastConditionMethod())
								{
									 nonDestructibleAbilities.Add(item);
								}
						  }
					 }

					 //Use non-destructible ability..
					 if (nonDestructibleAbilities.Count>0)
						  returnAbility=nonDestructibleAbilities[0];

					 Ability.SetupAbilityForUse(ref returnAbility, true);
					 return returnAbility;
				}


				internal HashSet<SNOPower> PassivePowers=new HashSet<SNOPower>();
				internal HashSet<SNOPower> HotbarPowers=new HashSet<SNOPower>();

				//Cached Powers Used Only for Archon Wizards!
				internal HashSet<SNOPower> CachedPowers=new HashSet<SNOPower>();

				internal Dictionary<SNOPower, int> RuneIndexCache=new Dictionary<SNOPower, int>();
				internal Dictionary<SNOPower, int> AbilityCooldowns=new Dictionary<SNOPower, int>();
				internal Dictionary<int, int> CurrentBuffs=new Dictionary<int, int>();
				internal List<int> CurrentDebuffs=new List<int>();
				internal List<SNOPower> destructibleabilities=new List<SNOPower>();


				internal Dictionary<SNOPower, Ability> Abilities=new Dictionary<SNOPower, Ability>();
				internal List<Ability> SortedAbilities=new List<Ability>();


				///<summary>
				///Returns a power for special movement if any are currently present in the abilities.
				///</summary>
				internal bool FindMovementPower(out Ability MovementAbility)
				{
					 MovementAbility=null;
					 foreach (var item in this.Abilities.Keys.Where(A => PowerCacheLookup.SpecialMovementAbilities.Contains(A)))
					 {

						  if (this.Abilities[item].AbilityTestConditions.CheckPreCastConditionMethod())
						  {
								MovementAbility=this.Abilities[item];
								return true;
						  }
					 }
					 return false;
				}
				///<summary>
				///Returns a power for special movement if any are currently present in the abilities.
				///</summary>
				internal bool FindSpecialMovementPower(out Ability MovementAbility)
				{
					 MovementAbility=null;
					 foreach (var item in this.Abilities.Values.Where(A => A.IsASpecialMovementPower))
					 {

						  if (item.AbilityTestConditions.CheckPreCastConditionMethod())
						  {
								MovementAbility=item;
								return true;
						  }
					 }
					 return false;
				}

				///<summary>
				///Returns a power for Buffing.
				///</summary>
				internal bool FindBuffPower(out Ability BuffAbility)
				{
					 BuffAbility=null;
					 foreach (var item in this.Abilities.Values.Where(A => A.IsBuff))
					 {
						  if (item.AbilityTestConditions.CheckPreCastConditionMethod())
						  {
								if (item.CheckBuffConditionMethod())
								{
									 BuffAbility=item;
									 return true;
								}
						  }
					 }
					 return false;
				}

				///<summary>
				///Returns a SnoPower based upon current hotbar abilities. If an ability not found than weapon melee/ranged instant is returned.
				///</summary>
				internal SNOPower DestructiblePower(SNOPower ignore=Zeta.Internals.Actors.SNOPower.None)
				{
					 if (destructibleabilities.Count>0)
						  return destructibleabilities.First(a => a!=ignore);

					 return Bot.Class.DefaultAttack.Power;
				}

				///<summary>
				///Enumerates through the ActiveSkills and adds them to the HotbarAbilities collection.
				///</summary>
				internal void RefreshHotbar()
				{
					 HotbarPowers=new HashSet<SNOPower>();
					 destructibleabilities=new List<SNOPower>();
					 RuneIndexCache=new Dictionary<SNOPower, int>();

					 using (ZetaDia.Memory.AcquireFrame())
					 {
						  if (ZetaDia.CPlayer.IsValid)
						  {

								//Cache each hotbar SNOPower
								foreach (SNOPower ability in ZetaDia.CPlayer.ActiveSkills)
								{
									 //"None" -- Occuring during Wizard Archon (Exceptions)
									 if (ability.Equals(SNOPower.None)) continue;

									 if (!this.HotbarPowers.Contains(ability))
										  this.HotbarPowers.Add(ability);

									 //Check if the SNOPower is a destructible ability
									 if (PowerCacheLookup.AbilitiesDestructiblePriority.Contains(ability))
									 {
										  if (!this.destructibleabilities.Contains(ability))
												this.destructibleabilities.Add(ability);
									 }

								}

								//Cache each Rune Index
								foreach (HotbarSlot item in Enum.GetValues(typeof(HotbarSlot)))
								{
									 if (item==HotbarSlot.Invalid) continue;

									 SNOPower hotbarPower=ZetaDia.CPlayer.GetPowerForSlot(item);

									 if (!this.HotbarPowers.Contains(hotbarPower)) continue;

									 try
									 {
										  int RuneIndex=ZetaDia.CPlayer.GetRuneIndexForSlot(item);
										  if (!this.RuneIndexCache.ContainsKey(hotbarPower))
												this.RuneIndexCache.Add(hotbarPower, RuneIndex);
									 } catch
									 {
										  if (!this.RuneIndexCache.ContainsKey(hotbarPower))
												this.RuneIndexCache.Add(hotbarPower, -1);
									 }
								}

						  }
					 }
				}



				///<summary>
				///Enumerates through the PassiveSkills and adds them to the PassiveAbilities collection. Used to adjust repeat timers of abilities.
				///</summary>
				internal void RefreshPassives()
				{

					 using (ZetaDia.Memory.AcquireFrame())
					 {
						  if (ZetaDia.CPlayer.IsValid)
						  {
								foreach (var item in ZetaDia.CPlayer.PassiveSkills)
								{
									 PassivePowers.Add(item);
								}
						  }
					 }

				}

				///<summary>
				///Sets each current hotbar ability repeat timer with adjustments made based upon passives.
				///</summary>
				internal void UpdateRepeatAbilityTimes()
				{
					 AbilityCooldowns=new Dictionary<SNOPower, int>();
					 foreach (var item in HotbarPowers)
					 {
						  if (PowerCacheLookup.dictAbilityRepeatDefaults.ContainsKey(item))
								AbilityCooldowns.Add(item, PowerCacheLookup.dictAbilityRepeatDefaults[item]);
					 }
					 foreach (var item in PassivePowers)
					 {
						  if (PowerCacheLookup.PassiveAbiltiesReduceRepeatTime.Contains(item))
						  {
								switch (item)
								{
									 case SNOPower.Barbarian_Passive_BoonOfBulKathos:
										  if (AbilityCooldowns.ContainsKey(SNOPower.Barbarian_CallOfTheAncients|SNOPower.Barbarian_WrathOfTheBerserker|SNOPower.Barbarian_Earthquake))
										  {
												List<SNOPower> AdjustedPowers=AbilityCooldowns.Keys.Where(K => K.HasFlag(SNOPower.Barbarian_CallOfTheAncients|SNOPower.Barbarian_WrathOfTheBerserker|SNOPower.Barbarian_Earthquake)).ToList();

												foreach (var Ability in AdjustedPowers)
												{
													 AbilityCooldowns[Ability]-=30000;
												}
										  }
										  break;
									 case SNOPower.Monk_Passive_BeaconOfYtar:
									 case SNOPower.Wizard_Passive_Evocation:
										  double PctReduction=item==SNOPower.Wizard_Passive_Evocation?0.85:0.80;
										  foreach (var Ability in AbilityCooldowns.Keys)
										  {
												AbilityCooldowns[Ability]=(int)(AbilityCooldowns[Ability]*PctReduction);
										  }
										  break;
									 case SNOPower.Witchdoctor_Passive_SpiritVessel:
										  //Horrify, Spirit Walk, and Soul Harvest spells by 2 seconds
										  if (AbilityCooldowns.ContainsKey(SNOPower.Witchdoctor_SoulHarvest|SNOPower.Witchdoctor_SpiritWalk|SNOPower.Witchdoctor_Horrify))
										  {
												List<SNOPower> AdjustedPowers=AbilityCooldowns.Keys.Where(K => K.HasFlag(SNOPower.Witchdoctor_SoulHarvest|SNOPower.Witchdoctor_SpiritWalk|SNOPower.Witchdoctor_Horrify)).ToList();
												foreach (var Ability in AdjustedPowers)
												{
													 AbilityCooldowns[Ability]-=2000;
												}
										  }
										  break;
									 case SNOPower.Witchdoctor_Passive_TribalRites:
										  //The cooldowns of your Fetish Army, Big Bad Voodoo, Hex, Gargantuan, Summon Zombie Dogs and Mass Confusion abilities are reduced by 25%.
										  if (AbilityCooldowns.ContainsKey(SNOPower.Witchdoctor_FetishArmy|SNOPower.Witchdoctor_BigBadVoodoo|SNOPower.Witchdoctor_Hex|SNOPower.Witchdoctor_Gargantuan|SNOPower.Witchdoctor_SummonZombieDog|SNOPower.Witchdoctor_MassConfusion))
										  {
												List<SNOPower> AdjustedPowers=AbilityCooldowns.Keys.Where(K => K.HasFlag(SNOPower.Witchdoctor_FetishArmy|SNOPower.Witchdoctor_BigBadVoodoo|SNOPower.Witchdoctor_Hex|SNOPower.Witchdoctor_Gargantuan|SNOPower.Witchdoctor_SummonZombieDog|SNOPower.Witchdoctor_MassConfusion)).ToList();
												foreach (var Ability in AdjustedPowers)
												{
													 AbilityCooldowns[Ability]=(int)(AbilityCooldowns[Ability]*0.75);
												}
										  }
										  break;
								}
						  }
					 }
					 if (!AbilityCooldowns.ContainsKey(SNOPower.DrinkHealthPotion))
						  AbilityCooldowns.Add(SNOPower.DrinkHealthPotion, 30000);
				}

				///<summary>
				///Enumerates through GetAllBuffs and adds them to the CurrentBuffs collection.
				///</summary>
				internal void RefreshCurrentBuffs()
				{
					 CurrentBuffs=new Dictionary<int, int>();
					 using (ZetaDia.Memory.AcquireFrame())
					 {
						  foreach (var item in ZetaDia.Me.GetAllBuffs())
						  {
								if (CurrentBuffs.ContainsKey(item.SNOId))
									 continue;

								if (PowerCacheLookup.PowerStackImportant.Contains(item.SNOId))
									 CurrentBuffs.Add(item.SNOId, item.StackCount);
								else
									 CurrentBuffs.Add(item.SNOId, 1);
						  }
					 }
				}
				///<summary>
				///
				///</summary>
				internal void RefreshCurrentDebuffs()
				{
					 CurrentDebuffs=new List<int>();
					 using (ZetaDia.Memory.AcquireFrame())
					 {
						  foreach (var item in ZetaDia.Me.GetAllDebuffs())
						  {
								CurrentDebuffs.Add(item.SNOId);
						  }
					 }
				}
				internal bool AbilityUseTimer(SNOPower thispower, bool bReCheck=false)
				{
					 double lastUseMS=AbilityLastUseMS(thispower);
					 if (lastUseMS>=this.AbilityCooldowns[thispower])
						  return true;
					 if (bReCheck&&lastUseMS>=150&&lastUseMS<=600)
						  return true;
					 return false;
				}
				internal double AbilityLastUseMS(SNOPower P)
				{
					 return DateTime.Now.Subtract(PowerCacheLookup.dictAbilityLastUse[P]).TotalMilliseconds;
				}
				internal int GetBuffStacks(SNOPower thispower)
				{
					 int iStacks;
					 if (CurrentBuffs.TryGetValue((int)thispower, out iStacks))
					 {
						  return iStacks;
					 }
					 return 0;
				}
				internal bool HasBuff(SNOPower power)
				{
					 int id=(int)power;
					 return CurrentBuffs.Keys.Any(u => u==id);
				}
				internal bool HasDebuff(SNOPower power)
				{
					 int id=(int)power;
					 return CurrentDebuffs.Contains(id);
				}
				public void DebugString()
				{
					 Logging.Write("Character Information\r\nRadius {0}\r\nHotBar Abilities [{1}]\r\n", Bot.Character.fCharacterRadius, HotbarPowers.Count);

					 foreach (Zeta.Internals.Actors.SNOPower item in Bot.Class.HotbarPowers)
					 {
						  Logging.Write("{0} with current rune index {1}", item.ToString(), RuneIndexCache.ContainsKey(item)?RuneIndexCache[item].ToString():"none");
					 }

					 Logging.Write("Created Abilities [{0}]", Abilities.Count);
					 foreach (var item in Abilities.Values)
					 {
						  Logging.Write("Power [{0}] -- Priority {1} --", item.Power.ToString(), item.Priority, item.DebugString());
					 }

					 Bot.Character.UpdateAnimationState();
					 Logging.Write("State: {0} -- SNOAnim {1}", Bot.Character.CurrentAnimationState.ToString(), Bot.Character.CurrentSNOAnim.ToString());
					 Logging.Write("Current Buffs");
					 foreach (Zeta.Internals.Actors.SNOPower item in CurrentBuffs.Keys)
					 {
						  Logging.Write("Buff: {0}", Enum.GetName(typeof(SNOPower), item));
					 }
					 Logging.Write("Current Debuffs");
					 foreach (Zeta.Internals.Actors.SNOPower item in CurrentDebuffs)
					 {
						  Logging.Write("Debuff: {0}", Enum.GetName(typeof(SNOPower), item));
					 }

				}
		  }
	 
}