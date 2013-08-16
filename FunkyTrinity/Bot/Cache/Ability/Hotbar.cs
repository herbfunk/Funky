using System;
using System.Collections.Generic;
using System.Linq;
using FunkyTrinity.Cache;
using Zeta;
using Zeta.Common;
using Zeta.Internals.Actors;

namespace FunkyTrinity.ability
{
	public class Hotbar
	{

		public Hotbar()
		{
			 RefreshHotbar();
			 RefreshPassives();
			 UpdateRepeatAbilityTimes();
		}

		public void RecreateAbilities()
		{
			 Abilities=new Dictionary<SNOPower, Ability>();

			 //Create the abilities
			 foreach (var item in HotbarPowers)
			 {
					Abilities.Add(item, Bot.Class.CreateAbility(item));
			 }

			 //No default rage generation ability.. then we add the Instant Melee Ability.
			 if (!HotbarContainsAPrimaryAbility())
			 {
					Ability defaultAbility=Bot.Class.DefaultAttack;
					Abilities.Add(defaultAbility.Power, defaultAbility);
					RuneIndexCache.Add(defaultAbility.Power, -1);
			 }

			 //Sort Abilities
			 SortedAbilities=Abilities.Values.OrderByDescending(a => a.Priority).ThenBy(a => a.Range).ToList();
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
		 public virtual Ability AbilitySelector(bool bCurrentlyAvoiding=false, bool bOOCBuff=false)
		 {
				foreach (var item in this.SortedAbilities)
				{
					 //Check Avoidance and Buff only parameters!
					 if (bCurrentlyAvoiding&&item.UseageType.HasFlag(AbilityUseage.Anywhere)==false) continue;
					 if (bOOCBuff&&item.UseageType.HasFlag(AbilityUseage.OutOfCombat|AbilityUseage.Anywhere)==false) continue;

					 //Check precast conditions
					 if (!item.CheckPreCastConditionMethod()) continue;

					 //Check Combat Conditions!
					 if (item.CheckCombatConditionMethod())
					 {
							//Setup Ability (sets vars according to current cache)
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
				Ability returnAbility=Bot.Class.DefaultAttack;

				foreach (var item in this.Abilities.Values)
				{
					 if (item.IsADestructiblePower)
					 {
							if (item.CheckPreCastConditionMethod())
							{
								 returnAbility=item;
								 break;
							}
					 }
				}


				returnAbility.SetupAbilityForUse();
				return returnAbility;
		 }


		 internal HashSet<SNOPower> PassivePowers=new HashSet<SNOPower>();
		 internal HashSet<SNOPower> HotbarPowers=new HashSet<SNOPower>();

		 //Cached Powers Used Only for Archon Wizards!
		 internal HashSet<SNOPower> CachedPowers=new HashSet<SNOPower>();

		 internal Dictionary<SNOPower, int> RuneIndexCache=new Dictionary<SNOPower, int>();
		 internal Dictionary<SNOPower, int> AbilityCooldowns=new Dictionary<SNOPower, int>();
		 internal Dictionary<int, int> CurrentBuffs=new Dictionary<int, int>();
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

					 if (this.Abilities[item].CheckPreCastConditionMethod())
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

					 if (item.CheckPreCastConditionMethod())
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
				foreach (var item in this.Abilities.Values.Where(A => A.UseageType.HasFlag(AbilityUseage.OutOfCombat)))
				{
					 if (item.CheckPreCastConditionMethod())
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
								 }
								 catch
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


					 if (!AbilityCooldowns.ContainsKey(Bot.Class.DefaultAttack.Power))
						  AbilityCooldowns.Add(Bot.Class.DefaultAttack.Power, 5);
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
	}
}
