using System;
using System.Linq;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.CommonBot;
using Zeta.Internals.SNO;

namespace FunkyTrinity
{
	 public partial class Funky
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
					 Logging.WriteVerbose("[Funky] Created the Player Class Data");
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

				// This is used so we don't use certain skills until we "top up" our primary resource by enough
				internal double iWaitingReservedAmount=0d;
				internal bool bWaitingForSpecial=false;
				internal bool bUsingCriticalMassPassive=false;

				public virtual void RecreateAbilities()
				{

				}
				///<summary>
				///Selects first ability that is successful in precast and combat testing.
				///</summary>
				public virtual Ability AbilitySelector(bool bCurrentlyAvoiding=false, bool bOOCBuff=false)
				{
					 foreach (var item in this.SortedAbilities)
					 {
						  //if (!item.Cooldown.IsFinished) continue;
						  if (bCurrentlyAvoiding&&item.UseAvoiding==false) continue;
						  if (bOOCBuff&&item.UseOOCBuff==false) continue;


						  if (!item.CheckPreCastConditionMethod()) continue;

						  //
						  if (item.CheckCombatConditionMethod())
						  {
								item.SetupAbilityForUse();
								return item;
						  }
					 }

					 return new Ability();
				}
				///<summary>
				///Returns ability used for destructibles
				///</summary>
				public virtual Ability DestructibleAbility()
				{
					 return new Ability();
				}
				///<summary>
				///Create ability (Derieved classes override this!)
				///</summary>
				public virtual Ability CreateAbility(SNOPower P)
				{
					 return new Ability();
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
				public virtual void GenerateNewZigZagPath()
				{

				}
				///<summary>
				///
				///</summary>
				public virtual int MainPetCount
				{
					 get
					 {
						  return 0;
					 }
				}

				internal HashSet<SNOPower> PassivePowers=new HashSet<SNOPower>();
				internal HashSet<SNOPower> HotbarPowers=new HashSet<SNOPower>();
				internal HashSet<SNOPower> CachedPowers=new HashSet<SNOPower>();

				private bool UsingSecondaryHotbarAbilities=false;
				internal Dictionary<SNOPower, int> RuneIndexCache=new Dictionary<SNOPower, int>();
				internal Dictionary<SNOPower, int> AbilityCooldowns=new Dictionary<SNOPower, int>();
				internal Dictionary<int, int> CurrentBuffs=new Dictionary<int, int>();

				internal Dictionary<SNOPower, Ability> Abilities=new Dictionary<SNOPower, Ability>();
				internal List<Ability> SortedAbilities=new List<Ability>();

				///<summary>
				///Returns a power for special movement if any are currently present in the hotbar abilities. None will return SnoPower.None.
				///</summary>
				internal bool FindSpecialMovementPower(out Ability MovementAbility)
				{
					 MovementAbility=null;
					 foreach (var item in this.Abilities.Keys.Where(A => SpecialMovementAbilities.Contains(A)))
					 {

						  if (this.Abilities[item].CheckPreCastConditionMethod())
						  {
								MovementAbility=this.Abilities[item];
								return true;
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

					 return IsMeleeClass?SNOPower.Weapon_Melee_Instant:SNOPower.Weapon_Ranged_Instant;
				}
				private List<SNOPower> destructibleabilities=new List<SNOPower>();

				///<summary>
				///Used to check for a secondary hotbar set. Currently only used for wizards with Archon.
				///</summary>
				internal bool SecondaryHotbarBuffPresent()
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
								RefreshHotbar(ArchonBuffPresent);
								UpdateRepeatAbilityTimes();
								RecreateAbilities();
								return true;
						  }
					 }
					 return false;
				}



				///<summary>
				///Enumerates through the ActiveSkills and adds them to the HotbarAbilities collection.
				///</summary>
				internal void RefreshHotbar(bool Secondary=false)
				{
					 UsingSecondaryHotbarAbilities=Secondary;
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
									 if (AbilitiesDestructiblePriority.Contains(ability))
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
						  if (dictAbilityRepeatDefaults.ContainsKey(item))
								AbilityCooldowns.Add(item, dictAbilityRepeatDefaults[item]);
					 }
					 foreach (var item in PassivePowers)
					 {
						  if (PassiveAbiltiesReduceRepeatTime.Contains(item))
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
									 case SNOPower.Witchdoctor_Passive_GraveInjustice:
									 case SNOPower.Wizard_Passive_CriticalMass:
										  this.bUsingCriticalMassPassive=true;
										  break;
								}
						  }
					 }
					 if (!AbilityCooldowns.ContainsKey(SNOPower.DrinkHealthPotion))
						  AbilityCooldowns.Add(SNOPower.DrinkHealthPotion, 30000);

					 if (IsMeleeClass)
					 {
						  if (!AbilityCooldowns.ContainsKey(SNOPower.Weapon_Melee_Instant))
								AbilityCooldowns.Add(SNOPower.Weapon_Melee_Instant, 5);
					 }
					 else
					 {
						  if (!AbilityCooldowns.ContainsKey(SNOPower.Weapon_Ranged_Instant))
								AbilityCooldowns.Add(SNOPower.Weapon_Ranged_Instant, 5);
					 }
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

								if (PowerStackImportant.Contains(item.SNOId))
									 CurrentBuffs.Add(item.SNOId, item.StackCount);
								else
									 CurrentBuffs.Add(item.SNOId, 1);
						  }
					 }
				}
				internal bool AbilityUseTimer(SNOPower thispower, bool bReCheck=false)
				{
					 double lastUseMS=AbilityLastUseMS(thispower);
					 if (lastUseMS>=Bot.Class.AbilityCooldowns[thispower])
						  return true;
					 if (bReCheck&&lastUseMS>=150&&lastUseMS<=600)
						  return true;
					 return false;
				}
				internal double AbilityLastUseMS(SNOPower P)
				{
					 return DateTime.Now.Subtract(dictAbilityLastUse[P]).TotalMilliseconds;
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


				public void DebugString()
				{


						  Logging.Write("Character Information\r\nRadius {0}\r\nHotBar Abilities [{1}]\r\n", Bot.Character.fCharacterRadius, this.HotbarPowers.Count);

						  foreach (Zeta.Internals.Actors.SNOPower item in Bot.Class.HotbarPowers)
						  {
								Logging.Write("{0} with current rune index {1}", item.ToString(), Bot.Class.RuneIndexCache.ContainsKey(item)?Bot.Class.RuneIndexCache[item].ToString():"none");
						  }

						  Logging.Write("Created Abilities [{0}]", Abilities.Count);
						  foreach (var item in this.Abilities.Values)
						  {
								Logging.Write("Power [{0}] -- Priority {1} --", item.Power.ToString(), item.Priority);
						  }

						  Bot.Character.UpdateAnimationState();
						  Logging.Write("State: {0} -- SNOAnim {1}", Bot.Character.CurrentAnimationState.ToString(), Bot.Character.CurrentSNOAnim.ToString());
						  Logging.Write("Current Buffs");
						  foreach (Zeta.Internals.Actors.SNOPower item in Bot.Class.CurrentBuffs.Keys)
						  {
								Logging.Write("Buff: {0}", Enum.GetName(typeof(SNOPower), item));
						  }
					 
				}
		  }
	 }
}