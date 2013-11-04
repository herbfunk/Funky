using FunkyBot.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zeta;
using Zeta.Internals.Actors;

namespace FunkyBot.Character
{
	public class Hotbar
	{
		public Hotbar() { }

		internal HashSet<SNOPower> PassivePowers = new HashSet<SNOPower>();
		internal HashSet<SNOPower> HotbarPowers = new HashSet<SNOPower>();

		//Cached Powers Used Only for Archon Wizards!
		internal HashSet<SNOPower> CachedPowers = new HashSet<SNOPower>();

		internal Dictionary<SNOPower, int> RuneIndexCache = new Dictionary<SNOPower, int>();
		internal Dictionary<SNOPower, int> AbilityCooldowns = new Dictionary<SNOPower, int>();
		internal Dictionary<int, int> CurrentBuffs = new Dictionary<int, int>();
		internal List<int> CurrentDebuffs = new List<int>();
		internal List<SNOPower> destructibleabilities = new List<SNOPower>();

		///<summary>
		///Enumerates through the ActiveSkills and adds them to the HotbarAbilities collection.
		///</summary>
		internal void RefreshHotbar()
		{
			HotbarPowers = new HashSet<SNOPower>();
			destructibleabilities = new List<SNOPower>();
			RuneIndexCache = new Dictionary<SNOPower, int>();

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

						//Check if the SNOPower is a destructible Ability
						if (PowerCacheLookup.AbilitiesDestructiblePriority.Contains(ability))
						{
							if (!this.destructibleabilities.Contains(ability))
								this.destructibleabilities.Add(ability);
						}

					}

					//Cache each Rune Index
					foreach (HotbarSlot item in Enum.GetValues(typeof(HotbarSlot)))
					{
						if (item == HotbarSlot.Invalid) continue;

						SNOPower hotbarPower = ZetaDia.CPlayer.GetPowerForSlot(item);

						if (!this.HotbarPowers.Contains(hotbarPower)) continue;

						try
						{
							int RuneIndex = ZetaDia.CPlayer.GetRuneIndexForSlot(item);
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
		///Sets each current hotbar Ability repeat timer with adjustments made based upon passives.
		///</summary>
		internal void UpdateRepeatAbilityTimes()
		{
			AbilityCooldowns = new Dictionary<SNOPower, int>();
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
							if (AbilityCooldowns.ContainsKey(SNOPower.Barbarian_CallOfTheAncients | SNOPower.Barbarian_WrathOfTheBerserker | SNOPower.Barbarian_Earthquake))
							{
								List<SNOPower> AdjustedPowers = AbilityCooldowns.Keys.Where(K => K.HasFlag(SNOPower.Barbarian_CallOfTheAncients | SNOPower.Barbarian_WrathOfTheBerserker | SNOPower.Barbarian_Earthquake)).ToList();

								foreach (var Ability in AdjustedPowers)
								{
									AbilityCooldowns[Ability] -= 30000;
								}
							}
							break;
						case SNOPower.Monk_Passive_BeaconOfYtar:
						case SNOPower.Wizard_Passive_Evocation:
							double PctReduction = item == SNOPower.Wizard_Passive_Evocation ? 0.85 : 0.80;
							foreach (var Ability in AbilityCooldowns.Keys)
							{
								AbilityCooldowns[Ability] = (int)(AbilityCooldowns[Ability] * PctReduction);
							}
							break;
						case SNOPower.Witchdoctor_Passive_SpiritVessel:
							//Horrify, Spirit Walk, and Soul Harvest spells by 2 seconds
							if (AbilityCooldowns.ContainsKey(SNOPower.Witchdoctor_SoulHarvest | SNOPower.Witchdoctor_SpiritWalk | SNOPower.Witchdoctor_Horrify))
							{
								List<SNOPower> AdjustedPowers = AbilityCooldowns.Keys.Where(K => K.HasFlag(SNOPower.Witchdoctor_SoulHarvest | SNOPower.Witchdoctor_SpiritWalk | SNOPower.Witchdoctor_Horrify)).ToList();
								foreach (var Ability in AdjustedPowers)
								{
									AbilityCooldowns[Ability] -= 2000;
								}
							}
							break;
						case SNOPower.Witchdoctor_Passive_TribalRites:
							//The cooldowns of your Fetish Army, Big Bad Voodoo, Hex, Gargantuan, Summon Zombie Dogs and Mass Confusion abilities are reduced by 25%.
							if (AbilityCooldowns.ContainsKey(SNOPower.Witchdoctor_FetishArmy | SNOPower.Witchdoctor_BigBadVoodoo | SNOPower.Witchdoctor_Hex | SNOPower.Witchdoctor_Gargantuan | SNOPower.Witchdoctor_SummonZombieDog | SNOPower.Witchdoctor_MassConfusion))
							{
								List<SNOPower> AdjustedPowers = AbilityCooldowns.Keys.Where(K => K.HasFlag(SNOPower.Witchdoctor_FetishArmy | SNOPower.Witchdoctor_BigBadVoodoo | SNOPower.Witchdoctor_Hex | SNOPower.Witchdoctor_Gargantuan | SNOPower.Witchdoctor_SummonZombieDog | SNOPower.Witchdoctor_MassConfusion)).ToList();
								foreach (var Ability in AdjustedPowers)
								{
									AbilityCooldowns[Ability] = (int)(AbilityCooldowns[Ability] * 0.75);
								}
							}
							break;
					}
				}
			}
			if (!AbilityCooldowns.ContainsKey(SNOPower.DrinkHealthPotion))
				AbilityCooldowns.Add(SNOPower.DrinkHealthPotion, 30000);
		}

		internal void RefreshHotbarBuffs()
		{
			this.RefreshCurrentBuffs();
			this.RefreshCurrentDebuffs();
		}
		///<summary>
		///Enumerates through GetAllBuffs and adds them to the CurrentBuffs collection.
		///</summary>
		private void RefreshCurrentBuffs()
		{
			CurrentBuffs = new Dictionary<int, int>();
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
		private void RefreshCurrentDebuffs()
		{
			CurrentDebuffs = new List<int>();
			using (ZetaDia.Memory.AcquireFrame())
			{
				foreach (var item in ZetaDia.Me.GetAllDebuffs())
				{
					CurrentDebuffs.Add(item.SNOId);
				}
			}
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
			int id = (int)power;
			return CurrentBuffs.Keys.Any(u => u == id);
		}
		internal bool HasDebuff(SNOPower power)
		{
			int id = (int)power;
			return CurrentDebuffs.Contains(id);
		}
		///<summary>
		///Checks if hotbar contains any of the "Primary" abilities.
		///</summary>
		internal bool HotbarContainsAPrimaryAbility()
		{
			return this.HotbarPowers.Any(p => PowerCacheLookup.PrimaryAbilities.Contains(p));
		}
	}
}
