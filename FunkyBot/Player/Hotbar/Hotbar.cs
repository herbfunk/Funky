using FunkyBot.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using FunkyBot.Player.Class;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace FunkyBot.Player.HotBar
{
	public class Hotbar
	{
		internal HashSet<SNOPower> PassivePowers = new HashSet<SNOPower>();
		

		internal Dictionary<SNOPower, int> SkillCooldowns = new Dictionary<SNOPower, int>();
		internal Dictionary<int, int> CurrentBuffs = new Dictionary<int, int>();
		internal List<int> CurrentDebuffs = new List<int>();



		internal List<HotbarSkill> HotbarSkills = new List<HotbarSkill>();
		private DateTime _lastCheckedSkills = DateTime.MinValue;
		private const int _skillRecheckSeconds = 10;
		private bool ShouldRecheckSkills
		{
			get
			{
				return DateTime.Now.Subtract(_lastCheckedSkills).TotalSeconds >= _skillRecheckSeconds;
			}
		}
		internal void CheckSkills()
		{
			if (!PlayerClass.ShouldRecreatePlayerClass && ShouldRecheckSkills)
			{
				_lastCheckedSkills=DateTime.Now;
				var compareList = ReturnHotbarSkills();

				if (compareList.Except(HotbarSkills).Any())
				{
					PlayerClass.ShouldRecreatePlayerClass = true;
				}
			}
		}

		private List<HotbarSkill> ReturnHotbarSkills()
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

		internal bool HasPower(SNOPower power)
		{
			return HotbarSkills.Any(s => s.Power == power);
		}

		internal int ReturnRuneIndex(SNOPower power)
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
		internal void RefreshHotbar()
		{
			var hotbarskills = ReturnHotbarSkills();
			HotbarSkills = new List<HotbarSkill>(hotbarskills);
			_lastCheckedSkills = DateTime.Now;
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

		internal void RefreshHotbarBuffs()
		{
			RefreshCurrentBuffs();
			RefreshCurrentDebuffs();
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
	}
}
