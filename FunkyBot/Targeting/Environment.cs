using System.Collections.Generic;
using FunkyBot.Cache.Objects;

namespace FunkyBot.Targeting
{
	class Environment
	{

		public Environment()
		{
			HeroPets = new Pets();
			bAnyLootableItemsNearby = false;
			iElitesWithinRange = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
			iAnythingWithinRange = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
			iNonRendedTargets_6 = 0;
			bAnyBossesInRange = false;
			bAnyChampionsPresent = false;
			bAnyTreasureGoblinsPresent = false;
			bAnyNonWWIgnoreMobsInRange = false;
			SurroundingUnits = 0;
			UsesDOTDPSAbility = false;
		}

		internal List<CacheServerObject> NearbyObstacleObjects = new List<CacheServerObject>();
		internal List<int> NearbyAvoidances = new List<int>();
		internal List<CacheUnit> FleeTriggeringUnits = new List<CacheUnit>();
		internal List<CacheAvoidance> TriggeringAvoidances = new List<CacheAvoidance>();
		internal List<int> TriggeringAvoidanceRAGUIDs = new List<int>();

		internal List<int> UnitRAGUIDs = new List<int>();
		internal List<CacheUnit> DistantUnits = new List<CacheUnit>();
		internal List<CacheObject> LoSMovementObjects = new List<CacheObject>();

		// Variables relating to quick-reference of monsters within sepcific ranges (if anyone has suggestion for similar functionality with reduced CPU use, lemme know, but this is fast atm!)
		internal int[] iElitesWithinRange;
		internal int[] iAnythingWithinRange;

		internal int iNonRendedTargets_6 { get; set; }
		internal bool UsesDOTDPSAbility { get; set; }
		internal int SurroundingUnits { get; set; }

		internal bool bAnyLootableItemsNearby { get; set; }
		internal bool bAnyChampionsPresent { get; set; }
		internal bool bAnyTreasureGoblinsPresent { get; set; }
		internal bool bAnyBossesInRange { get; set; }
		// A flag to say whether any NONE-hashActorSNOWhirlwindIgnore things are around
		internal bool bAnyNonWWIgnoreMobsInRange { get; set; }

		internal Pets HeroPets { get; set; }

		internal void Reset()
		{
			HeroPets.Reset();
			iElitesWithinRange = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
			iAnythingWithinRange = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
			iNonRendedTargets_6 = 0;

			bAnyBossesInRange = false;
			bAnyChampionsPresent = false;
			bAnyTreasureGoblinsPresent = false;

			bAnyNonWWIgnoreMobsInRange = false;
			bAnyLootableItemsNearby = false;

			UsesDOTDPSAbility = false;

			UnitRAGUIDs.Clear();
			SurroundingUnits = 0;
			TriggeringAvoidances.Clear();
			TriggeringAvoidanceRAGUIDs.Clear();
			NearbyAvoidances.Clear();
			NearbyObstacleObjects.Clear();
			FleeTriggeringUnits.Clear();
			DistantUnits.Clear();
			LoSMovementObjects.Clear();
			
		}

		internal class Pets
		{
			public enum PetTypes
			{
				MONK_MysticAlly = 1,
				WITCHDOCTOR_Gargantuan = 2,
				WITCHDOCTOR_ZombieDogs = 4,
				DEMONHUNTER_Pet = 8,
				WIZARD_Hydra = 16,
				DEMONHUNTER_SpikeTrap = 32,
				DEMONHUNTER_Sentry = 64,
				WITCHDOCTOR_Fetish = 128,
			}

			public Dictionary<PetTypes, int> dictPetCounter = new Dictionary<PetTypes, int>();

			public Pets()
			{
				Reset();
			}

			// A count for player mystic ally, gargantuans, and zombie dogs
			public int MysticAlly
			{
				get
				{
					if (!dictPetCounter.ContainsKey(PetTypes.MONK_MysticAlly))
						dictPetCounter.Add(PetTypes.MONK_MysticAlly, 0);

					return dictPetCounter[PetTypes.MONK_MysticAlly];
				}
				set
				{
					if (!dictPetCounter.ContainsKey(PetTypes.MONK_MysticAlly))
						dictPetCounter.Add(PetTypes.MONK_MysticAlly, value);
					else
						dictPetCounter[PetTypes.MONK_MysticAlly] = value;
				}
			}
			public int Gargantuan
			{
				get
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WITCHDOCTOR_Gargantuan))
						dictPetCounter.Add(PetTypes.WITCHDOCTOR_Gargantuan, 0);

					return dictPetCounter[PetTypes.WITCHDOCTOR_Gargantuan];
				}
				set
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WITCHDOCTOR_Gargantuan))
						dictPetCounter.Add(PetTypes.WITCHDOCTOR_Gargantuan, value);
					else
						dictPetCounter[PetTypes.WITCHDOCTOR_Gargantuan] = value;
				}
			}
			public int ZombieDogs
			{
				get
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WITCHDOCTOR_ZombieDogs))
						dictPetCounter.Add(PetTypes.WITCHDOCTOR_ZombieDogs, 0);

					return dictPetCounter[PetTypes.WITCHDOCTOR_ZombieDogs];
				}
				set
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WITCHDOCTOR_ZombieDogs))
						dictPetCounter.Add(PetTypes.WITCHDOCTOR_ZombieDogs, value);
					else
						dictPetCounter[PetTypes.WITCHDOCTOR_ZombieDogs] = value;
				}
			}
			public int WitchdoctorFetish
			{
				get
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WITCHDOCTOR_Fetish))
						dictPetCounter.Add(PetTypes.WITCHDOCTOR_Fetish, 0);

					return dictPetCounter[PetTypes.WITCHDOCTOR_Fetish];
				}
				set
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WITCHDOCTOR_Fetish))
						dictPetCounter.Add(PetTypes.WITCHDOCTOR_Fetish, value);
					else
						dictPetCounter[PetTypes.WITCHDOCTOR_Fetish] = value;
				}
			}
			public int DemonHunterPet
			{
				get
				{
					if (!dictPetCounter.ContainsKey(PetTypes.DEMONHUNTER_Pet))
						dictPetCounter.Add(PetTypes.DEMONHUNTER_Pet, 0);

					return dictPetCounter[PetTypes.DEMONHUNTER_Pet];
				}
				set
				{
					if (!dictPetCounter.ContainsKey(PetTypes.DEMONHUNTER_Pet))
						dictPetCounter.Add(PetTypes.DEMONHUNTER_Pet, value);
					else
						dictPetCounter[PetTypes.DEMONHUNTER_Pet] = value;
				}
			}
			public int DemonHunterSpikeTraps
			{
				get
				{
					if (!dictPetCounter.ContainsKey(PetTypes.DEMONHUNTER_SpikeTrap))
						dictPetCounter.Add(PetTypes.DEMONHUNTER_SpikeTrap, 0);

					return dictPetCounter[PetTypes.DEMONHUNTER_SpikeTrap];
				}
				set
				{
					if (!dictPetCounter.ContainsKey(PetTypes.DEMONHUNTER_SpikeTrap))
						dictPetCounter.Add(PetTypes.DEMONHUNTER_SpikeTrap, value);
					else
						dictPetCounter[PetTypes.DEMONHUNTER_SpikeTrap] = value;
				}
			}

			public int DemonHunterSentry
			{
				get
				{
					if (!dictPetCounter.ContainsKey(PetTypes.DEMONHUNTER_Sentry))
						dictPetCounter.Add(PetTypes.DEMONHUNTER_Sentry, 0);

					return dictPetCounter[PetTypes.DEMONHUNTER_Sentry];
				}
				set
				{
					if (!dictPetCounter.ContainsKey(PetTypes.DEMONHUNTER_Sentry))
						dictPetCounter.Add(PetTypes.DEMONHUNTER_Sentry, value);
					else
						dictPetCounter[PetTypes.DEMONHUNTER_Sentry] = value;
				}
			}
			public int WizardHydra
			{
				get
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WIZARD_Hydra))
						dictPetCounter.Add(PetTypes.WIZARD_Hydra, 0);

					return dictPetCounter[PetTypes.WIZARD_Hydra];
				}
				set
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WIZARD_Hydra))
						dictPetCounter.Add(PetTypes.WIZARD_Hydra, value);
					else
						dictPetCounter[PetTypes.WIZARD_Hydra] = value;
				}
			}

			public void Reset()
			{
				MysticAlly = 0;
				Gargantuan = 0;
				ZombieDogs = 0;
				DemonHunterPet = 0;
				WizardHydra = 0;
				DemonHunterSpikeTraps = 0;
				DemonHunterSentry = 0;
				WitchdoctorFetish = 0;
			}

			public string DebugString()
			{
				string PetString = "Pets\r\n";
				if (MysticAlly > 0) PetString += "Mystic Ally: " + MysticAlly + "\r\n";
				if (Gargantuan > 0) PetString += "Gargantuan: " + Gargantuan + "\r\n";
				if (ZombieDogs > 0) PetString += "ZombieDogs: " + ZombieDogs + "\r\n";
				if (DemonHunterPet > 0) PetString += "DemonHunterPet: " + DemonHunterPet + "\r\n";
				if (WizardHydra > 0) PetString += "WizardHydra: " + WizardHydra + "\r\n";
				if (DemonHunterSpikeTraps > 0) PetString += "DemonHunterSpikeTraps: " + DemonHunterSpikeTraps + "\r\n";
				if (DemonHunterSentry > 0) PetString += "DemonHunterSentry: " + DemonHunterSentry + "\r\n";
				if (WitchdoctorFetish > 0) PetString += "WitchdoctorFetish: " + WitchdoctorFetish + "\r\n";

				return PetString;
			}

		}
	}
}
