using System.Collections.Generic;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;

namespace fBaseXtensions.Targeting
{
	public class Environment
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
		}

		public List<CacheServerObject> NearbyObstacleObjects = new List<CacheServerObject>();
		public List<int> NearbyAvoidances = new List<int>();
		public List<CacheUnit> FleeTriggeringUnits = new List<CacheUnit>();
		public List<CacheAvoidance> TriggeringAvoidances = new List<CacheAvoidance>();
		public List<int> TriggeringAvoidanceRAGUIDs = new List<int>();

		public List<int> UnitRAGUIDs = new List<int>();
		public List<CacheUnit> DistantUnits = new List<CacheUnit>();
		public List<CacheObject> LoSMovementObjects = new List<CacheObject>();

		// Variables relating to quick-reference of monsters within sepcific ranges (if anyone has suggestion for similar functionality with reduced CPU use, lemme know, but this is fast atm!)
		public int[] iElitesWithinRange;
		public int[] iAnythingWithinRange;

		public int iNonRendedTargets_6 { get; set; }
	
		public int SurroundingUnits { get; set; }

		public bool bAnyLootableItemsNearby { get; set; }
		public bool bAnyChampionsPresent { get; set; }
		public bool bAnyTreasureGoblinsPresent { get; set; }
		public bool bAnyBossesInRange { get; set; }
		// A flag to say whether any NONE-hashActorSNOWhirlwindIgnore things are around
		public bool bAnyNonWWIgnoreMobsInRange { get; set; }

		public Pets HeroPets { get; set; }

		public void Reset()
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

		public class Pets
		{


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

			public int WizardArcaneOrbs
			{
				get
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WIZARD_ArcaneOrbs))
						dictPetCounter.Add(PetTypes.WIZARD_ArcaneOrbs, 0);

					return dictPetCounter[PetTypes.WIZARD_ArcaneOrbs];
				}
				set
				{
					if (!dictPetCounter.ContainsKey(PetTypes.WIZARD_ArcaneOrbs))
						dictPetCounter.Add(PetTypes.WIZARD_ArcaneOrbs, value);
					else
						dictPetCounter[PetTypes.WIZARD_ArcaneOrbs] = value;
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
				WizardArcaneOrbs = 0;
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
				if (WizardArcaneOrbs > 0) PetString += "WizardArcaneOrbs: " + WizardArcaneOrbs + "\r\n";

				return PetString;
			}

		}
	}
}
