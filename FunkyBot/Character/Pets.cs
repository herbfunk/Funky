using System.Collections.Generic;

namespace FunkyBot.Character
{
	 public enum PetTypes
	 {
		  MONK_MysticAlly=1,
		  WITCHDOCTOR_Gargantuan=2,
		  WITCHDOCTOR_ZombieDogs=4,
		  DEMONHUNTER_Pet=8,
		  WIZARD_Hydra=16,
		  DEMONHUNTER_SpikeTrap=32,
	 }

	public class Pets
	{
		public Dictionary<PetTypes, int> dictPetCounter=new Dictionary<PetTypes, int>();

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
					dictPetCounter[PetTypes.MONK_MysticAlly]=value;
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
					dictPetCounter[PetTypes.WITCHDOCTOR_Gargantuan]=value;
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
					dictPetCounter[PetTypes.WITCHDOCTOR_ZombieDogs]=value;
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
					dictPetCounter[PetTypes.DEMONHUNTER_Pet]=value;
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
						dictPetCounter[PetTypes.DEMONHUNTER_SpikeTrap]=value;
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
					dictPetCounter[PetTypes.WIZARD_Hydra]=value;
			}
		}

		public void Reset()
		{
			MysticAlly=0;
			Gargantuan=0;
			ZombieDogs=0;
			DemonHunterPet=0;
			WizardHydra=0;
			DemonHunterSpikeTraps=0;
		}

	}
}