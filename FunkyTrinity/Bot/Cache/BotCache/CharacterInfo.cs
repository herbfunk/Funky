using System;
using System.Linq;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Internals;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.Internals.SNO;
using Zeta.CommonBot;

namespace FunkyTrinity
{
	 public partial class Funky
	 {
		  internal static partial class Bot
		  {
				///<summary>
				///Cache of all values Character Class related (Individual Bot Info).
				///</summary>
				internal class CharacterInfo
				{
					 public CharacterInfo(ActorClass a)
					 {
						  AC=a;
						  IsMeleeClass=AC==ActorClass.Barbarian||AC==ActorClass.Monk?true:false;
						  RefreshHotbar();
						  RefreshPassives();
						  UpdateRepeatAbilityTimes();

						  //compile kiting ignoring cache
						  if (KiteDistance>0)
						  {
								hashActorSNOKitingIgnore=new HashSet<int> { 4095, 144315 };
								//burrowing units
								hashActorSNOKitingIgnore.UnionWith(SnoCacheLookup.hashActorSNOBurrowableUnits);
								//grunts
								hashActorSNOKitingIgnore.UnionWith(SnoCacheLookup.hashActorSNOSummonedUnit);
								//LOS exceptions (gyser, heart of sin)
								hashActorSNOKitingIgnore.UnionWith(SnoCacheLookup.hashActorSNOIgnoreLOSCheck);
						  }

						  UpdateActiveSkillCache();

						  Logging.WriteVerbose("Finished Creating Class Data");
					 }

					 ///<summary>
					 ///The actor class of this bot.
					 ///</summary>
					 internal readonly ActorClass AC;
					 internal readonly bool IsMeleeClass;

					 // This is used so we don't use certain skills until we "top up" our primary resource by enough
					 internal double iWaitingReservedAmount=0d;
					 internal bool bWaitingForSpecial=false;

					 internal HashSet<SNOPower> PassiveAbilities=new HashSet<SNOPower>();
					 internal HashSet<SNOPower> HotbarAbilities=new HashSet<SNOPower>();
					 private HashSet<SNOPower> CachedHotbarAbilities=new HashSet<SNOPower>();
					 private bool UsingSecondaryHotbarAbilities=false;
					 private Dictionary<HotbarSlot, SNOPower> HotbarPowerCache=new Dictionary<HotbarSlot, SNOPower>();
					 internal Dictionary<SNOPower, int> RuneIndexCache=new Dictionary<SNOPower, int>();

					 internal Dictionary<SNOPower, int> AbilityCooldowns=new Dictionary<SNOPower, int>();

					 private List<ItemType> CurrentEquippedItemTypes=new List<ItemType>();




					 internal Dictionary<int, int> CurrentBuffs=new Dictionary<int, int>();

					 internal List<int> OffensiveAbilities=new List<int>();
					 internal List<int> EnergyGenerationAbilities=new List<int>();

					 private bool specialMovementUseCheck(SNOPower P)
					 {
						  int repeatTimer=dictAbilityRepeatDelay[P];
						  double LastUsedMiliseconds=DateTime.Now.Subtract(dictAbilityLastUse[P]).TotalMilliseconds;

						  //Tempest Rush does not need to be checked if currently being used.
						  if (P==SNOPower.Monk_TempestRush&&LastUsedMiliseconds<250)
								return true;

						  return (LastUsedMiliseconds>=repeatTimer&&
								PowerManager.CanCast(P));
					 }
					 ///<summary>
					 ///Returns a power for special movement if any are currently present in the hotbar abilities. None will return SnoPower.None.
					 ///</summary>
					 internal bool FindSpecialMovementPower(out SNOPower Power)
					 {
						  Power=SNOPower.None;
						  foreach (var item in HotbarAbilities.Where(A => SpecialMovementAbilities.Contains(A)))
						  {
								if (specialMovementUseCheck(item))
								{
									 Power=item;
									 return true;
								}
						  }
						  return false;
					 }

					 internal HashSet<int> hashActorSNOKitingIgnore=new HashSet<int>();

					 private List<SNOPower> destructibleabilities=new List<SNOPower>();
					 ///<summary>
					 ///Returns a SnoPower based upon current hotbar abilities. If an ability not found than weapon melee/ranged instant is returned.
					 ///</summary>
					 internal SNOPower DestructiblePower(SNOPower ignore=Zeta.Internals.Actors.SNOPower.None)
					 {
						  if (destructibleabilities.Count>0)
								return destructibleabilities.First(a => a!=ignore);

						  return IsMeleeClass?SNOPower.Weapon_Melee_Instant:SNOPower.Weapon_Ranged_Instant;
					 }


					 ///<summary>
					 ///Used to check for a secondary hotbar set. Currently only used for wizards with Archon.
					 ///</summary>
					 internal void SecondaryHotbarBuffPresent()
					 {

						  if (AC==ActorClass.Wizard)
						  {
								bool ArchonBuffPresent=(CurrentBuffs.ContainsKey((int)SNOPower.Wizard_Archon));
								bool RefreshNeeded=false;

								if (ArchonBuffPresent&&!UsingSecondaryHotbarAbilities)
									 RefreshNeeded=true;
								else if (!ArchonBuffPresent&&UsingSecondaryHotbarAbilities)
									 RefreshNeeded=true;

								if (RefreshNeeded)
								{
									 Logging.WriteVerbose("Updating Hotbar abilities!");
									 RefreshHotbar(ArchonBuffPresent);
									 UpdateRepeatAbilityTimes();
								}
						  }
					 }

					 ///<summary>
					 ///Enumerates KnownSkills and adds them to specific collections.
					 ///</summary>
					 internal void UpdateActiveSkillCache()
					 {
						  OffensiveAbilities=new List<int>();

						  if (ZetaDia.Me.IsValid)
						  {
								using (ZetaDia.Memory.AcquireFrame())
								{
									 foreach (var item in ZetaDia.Me.KnownSkills)
									 {
										  if (item.Category.HasFlag(ActiveSkillCategory.Offensive))
										  {
												OffensiveAbilities.Add(item.SNOPower);
										  }

										  //Energy Generating Abilities
										  if ((ActiveSkillCategory.FuryGenerator|ActiveSkillCategory.HatredGenerator|ActiveSkillCategory.SpiritGenerator).HasFlag(item.Category))
										  {
												EnergyGenerationAbilities.Add(item.SNOPower);
										  }
									 }
								}
						  }
					 }

					 ///<summary>
					 ///Enumerates through the ActiveSkills and adds them to the HotbarAbilities collection.
					 ///</summary>
					 internal void RefreshHotbar(bool Secondary=false)
					 {
						  if (Secondary)
						  {
								CachedHotbarAbilities=new HashSet<SNOPower>(HotbarAbilities);
								UsingSecondaryHotbarAbilities=true;
						  }
						  else
								UsingSecondaryHotbarAbilities=false;

						  HotbarAbilities=new HashSet<SNOPower>();
						  destructibleabilities=new List<SNOPower>();
						  HotbarPowerCache=new Dictionary<HotbarSlot, SNOPower>();
						  RuneIndexCache=new Dictionary<SNOPower, int>();

						  using (ZetaDia.Memory.AcquireFrame())
						  {
								if (ZetaDia.CPlayer.IsValid)
								{

									 foreach (SNOPower ability in ZetaDia.CPlayer.ActiveSkills)
									 {
										  if (!HotbarAbilities.Contains(ability))
												HotbarAbilities.Add(ability);

										  if (AbilitiesDestructiblePriority.Contains(ability))
										  {
												if (!destructibleabilities.Contains(ability))
													 destructibleabilities.Add(ability);
										  }

									 }

									 foreach (HotbarSlot item in Enum.GetValues(typeof(HotbarSlot)))
									 {
										  if (item==HotbarSlot.Invalid) continue;

										  SNOPower hotbarPower=ZetaDia.CPlayer.GetPowerForSlot(item);

										  if (!HotbarAbilities.Contains(hotbarPower)) continue;

										  if (!HotbarPowerCache.ContainsKey(item))
												HotbarPowerCache.Add(item, hotbarPower);

										  int RuneIndex=ZetaDia.CPlayer.GetRuneIndexForSlot(item);

										  if (!RuneIndexCache.ContainsKey(hotbarPower))
												RuneIndexCache.Add(hotbarPower, RuneIndex);

										  //Logging.WriteVerbose("Hotbar Slot {0} contains Power {1} with RuneIndex {2}", item.ToString(), hotbarPower.ToString(), RuneIndex.ToString());
										  //HotbarPowerCache
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
										  PassiveAbilities.Add(item);
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
						  foreach (var item in HotbarAbilities)
						  {
								if (dictAbilityRepeatDefaults.ContainsKey(item))
									 AbilityCooldowns.Add(item, dictAbilityRepeatDefaults[item]);
						  }
						  foreach (var item in PassiveAbilities)
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
												SettingsFunky.Class.bEnableCriticalMass=true;
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


					 private readonly HashSet<int> PowerStackImportant=new HashSet<int>
				{
					 (int)SNOPower.Witchdoctor_SoulHarvest,
					 (int)SNOPower.Wizard_EnergyTwister
				};

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
				}
		  }
	 }
}