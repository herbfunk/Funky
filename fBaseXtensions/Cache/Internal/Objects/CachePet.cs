using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Game;
using Zeta.Game;

namespace fBaseXtensions.Cache.Internal.Objects
{
	public class CachePet : CacheObject
	{
		public CachePet(CacheObject parent, PetTypes type) : base(parent) 
		{
			PetType = type; 
		}

		public PetTypes PetType { get; set; }

		public override bool ObjectShouldBeRecreated
		{
			get { return false; }
		}

		public override bool ObjectIsValidForTargeting
		{
			get 
			{
				//Tally Pet Count Here!
				if (FunkyGame.CurrentActorClass == ActorClass.Monk)
					FunkyGame.Targeting.Cache.Environment.HeroPets.MysticAlly++;
				else if (FunkyGame.CurrentActorClass == ActorClass.DemonHunter)
				{
					if (PetType == PetTypes.DEMONHUNTER_Pet)
						FunkyGame.Targeting.Cache.Environment.HeroPets.DemonHunterPet++;
					else if (PetType == PetTypes.DEMONHUNTER_SpikeTrap && CentreDistance <= 50f)
						FunkyGame.Targeting.Cache.Environment.HeroPets.DemonHunterSpikeTraps++;
					else if (PetType == PetTypes.DEMONHUNTER_Sentry && CentreDistance <= 60f)
						FunkyGame.Targeting.Cache.Environment.HeroPets.DemonHunterSentry++;
				}
				else if (FunkyGame.CurrentActorClass == ActorClass.Witchdoctor)
				{
					if (PetType == PetTypes.WITCHDOCTOR_ZombieDogs)
						FunkyGame.Targeting.Cache.Environment.HeroPets.ZombieDogs++;
					else if (PetType == PetTypes.WITCHDOCTOR_Gargantuan)
						FunkyGame.Targeting.Cache.Environment.HeroPets.Gargantuan++;
					else if (PetType == PetTypes.WITCHDOCTOR_Fetish)
						FunkyGame.Targeting.Cache.Environment.HeroPets.WitchdoctorFetish++;
				}
				else if (FunkyGame.CurrentActorClass == ActorClass.Wizard)
				{
					//only count when range is within 45f (so we can summon a new one)
					if (PetType == PetTypes.WIZARD_Hydra && CentreDistance <= 50f)
						FunkyGame.Targeting.Cache.Environment.HeroPets.WizardHydra++;
				}

				return false; 
			}
		}
	}
}
