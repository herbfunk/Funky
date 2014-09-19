using System;
using System.Collections.Generic;
using System.Linq;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Game.Hero
{
	public static class Hotbar
	{
		public static HashSet<SNOPower> PassivePowers = new HashSet<SNOPower>();
		public static Dictionary<int, HotbarBuff> CurrentBuffs = new Dictionary<int, HotbarBuff>();
		public static List<int> CurrentDebuffs = new List<int>();
		public static List<HotbarSkill> HotbarSkills = new List<HotbarSkill>();

		private static DateTime _lastCheckedSkills = DateTime.MinValue;
		private const int _skillRecheckSeconds = 10;
		private static bool ShouldRecheckSkills
		{
			get
			{
				return DateTime.Now.Subtract(_lastCheckedSkills).TotalSeconds >= _skillRecheckSeconds;
			}
		}
		internal static void CheckSkills()
		{
			if (OnSkillsChanged == null) return;

			if (ShouldRecheckSkills)
			{
				_lastCheckedSkills=DateTime.Now;
				var compareList = ReturnHotbarSkills();

				if (compareList.Except(HotbarSkills).Any())
				{
					HotbarSkills = new List<HotbarSkill>(compareList);
					OnSkillsChanged();
				}
			}
		}

		public delegate void SkillsChanged();
		public static event SkillsChanged OnSkillsChanged;

		private static List<HotbarSkill> ReturnHotbarSkills()
		{
			List<HotbarSkill> returnList = new List<HotbarSkill>();
			using (ZetaDia.Memory.AcquireFrame())
			{
				if (ZetaDia.CPlayer.IsValid)
				{
					foreach (HotbarSlot item in Enum.GetValues(typeof(HotbarSlot)))
					{
						if (item == HotbarSlot.Invalid) continue;

						SNOPower hotbarPower = ZetaDia.CPlayer.GetPowerForSlot(item);
						if (hotbarPower.Equals(SNOPower.None)) continue;

						int runeIndex = -1;
						try
						{
							runeIndex = ZetaDia.CPlayer.GetRuneIndexForSlot(item);
						}
						catch{}

						returnList.Add(new HotbarSkill(item, hotbarPower, runeIndex));
					}
				}
			}

			return returnList;
		}

		public static bool HasPower(SNOPower power)
		{
			return HotbarSkills.Any(s => s.Power == power);
		}

		public static int ReturnRuneIndex(SNOPower power)
		{
			if (HotbarSkills.Any(s => s.Power==power))
			{
				var hotbarskill = HotbarSkills.First(s => s.Power == power);
				return hotbarskill.RuneIndex;
			}
			return -1;
		}

		///<summary>
		///Enumerates through the ActiveSkills and adds them to the HotbarAbilities collection.
		///</summary>
		public static void RefreshHotbar()
		{
			var hotbarskills = ReturnHotbarSkills();
			HotbarSkills = new List<HotbarSkill>(hotbarskills);
			_lastCheckedSkills = DateTime.Now;
		}


		///<summary>
		///Enumerates through the PassiveSkills and adds them to the PassiveAbilities collection. Used to adjust repeat timers of abilities.
		///</summary>
		public static void RefreshPassives()
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
		private static DateTime _lastRefreshedBuffs=DateTime.Today;
		private const int refreshBuffMilliseconds = 150;
		private static bool ShouldRefreshBuffs
		{
			get
			{
				return DateTime.Now.Subtract(_lastRefreshedBuffs).TotalMilliseconds >= refreshBuffMilliseconds;
			}
		}
		public static void RefreshHotbarBuffs()
		{
			RefreshCurrentBuffs();
			RefreshCurrentDebuffs();
		}
		///<summary>
		///Enumerates through GetAllBuffs and adds them to the CurrentBuffs collection.
		///</summary>
		private static void RefreshCurrentBuffs()
		{
			CurrentBuffs = new Dictionary<int, HotbarBuff>();
			using (ZetaDia.Memory.AcquireFrame())
			{
				foreach (var item in ZetaDia.Me.GetAllBuffs())
				{
					int snoid = item.SNOId;
					HotbarBuff b = new HotbarBuff(item);

					if (CurrentBuffs.ContainsKey(snoid))
					{
						if (!CurrentBuffs[snoid].Equals(b))
						{
							CurrentBuffs[snoid] = b;
						}
						continue;
					}

					CurrentBuffs.Add(snoid, b);
				}
			}
			_lastRefreshedBuffs=DateTime.Now;
		}
		internal static readonly HashSet<int> PowerStackImportant = new HashSet<int>
		{
			(int)SNOPower.Witchdoctor_SoulHarvest,
			(int)SNOPower.Wizard_EnergyTwister,
			(int)SNOPower.Monk_SweepingWind,
		};
		///<summary>
		///
		///</summary>
		private static void RefreshCurrentDebuffs()
		{
			CurrentDebuffs = new List<int>();
			using (ZetaDia.Memory.AcquireFrame())
			{
				foreach (var item in ZetaDia.Me.GetAllDebuffs())
				{
					CurrentDebuffs.Add(item.SNOId);
				}
			}

			_lastRefreshedBuffs = DateTime.Now;
		}

		public static int GetBuffStacks(SNOPower thispower)
		{
			if (ShouldRefreshBuffs) RefreshCurrentBuffs();

			HotbarBuff buff;
			if (CurrentBuffs.TryGetValue((int)thispower, out buff))
			{
				return buff.StackCount;
			}
			return 0;
		}
		public static bool HasBuff(SNOPower power)
		{
			if (ShouldRefreshBuffs) RefreshCurrentBuffs();

			int id = (int)power;
			return CurrentBuffs.Keys.Any(u => u == id);
		}
		public static bool HasDebuff(SNOPower power)
		{
			if (ShouldRefreshBuffs) RefreshCurrentDebuffs();

			int id = (int)power;
			return CurrentDebuffs.Contains(id);
		}

		public class HotbarSkill
		{
			public HotbarSlot Slot { get; set; }
			public SNOPower Power { get; set; }
			public int RuneIndex { get; set; }

			public HotbarSkill()
			{
				Slot= HotbarSlot.Invalid;
				Power = SNOPower.None;
				RuneIndex = -1;
			}
			public HotbarSkill(HotbarSlot slot, SNOPower power, int runeindex)
			{
				Slot = slot;
				Power = power;
				RuneIndex = runeindex;
			}

			public override int GetHashCode()
			{
				return (int)Power^RuneIndex;
			}
			public override bool Equals(object obj)
			{
				//Check for null and compare run-time types. 
				if (obj == null || GetType() != obj.GetType())
				{
					return false;
				}
				HotbarSkill p = (HotbarSkill)obj;
				return Power == p.Power && RuneIndex == p.RuneIndex;
			}
		}

		public class HotbarBuff
		{
			public int SNOId { get; set; }
			public int StackCount { get; set; }
			public bool IsCancelable { get; set; }

			public HotbarBuff()
			{
				SNOId = -1;
				StackCount = 0;
				IsCancelable = false;
			}
			public HotbarBuff(Buff buff)
			{
				SNOId = buff.SNOId;
				StackCount = buff.StackCount;
				IsCancelable = buff.IsCancelable;
			}

			public override string ToString()
			{
				string Power = Enum.GetName(typeof(SNOPower), SNOId);
				return String.Format("{0}  Stack Count {1}  IsCancelable {2}",
										Power, StackCount, IsCancelable);
			}

			public override int GetHashCode()
			{
				return SNOId;
			}

			public override bool Equals(object obj)
			{
				//Check for null and compare run-time types. 
				if (obj == null || GetType() != obj.GetType())
				{
					return false;
				}
				else
				{
					HotbarBuff p = (HotbarBuff)obj;
					return (SNOId == p.SNOId) && (StackCount==p.StackCount) && (IsCancelable==p.IsCancelable);
				}
			}
		}
	}
}
