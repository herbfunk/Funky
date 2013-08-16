using System;
using System.Linq;
using FunkyTrinity.ability.Abilities;
using Zeta;
using Zeta.Internals.Actors;
using Zeta.Common;
using System.Collections.Generic;
using Zeta.CommonBot;
using Zeta.Internals.SNO;
using FunkyTrinity.Enums;
using FunkyTrinity.ability;

namespace FunkyTrinity
{

		  internal class WitchDoctor : Player
		  {
				 enum WitchDoctorActiveSkills
				 {
						Witchdoctor_Gargantuan=30624,
						Witchdoctor_Hex=30631,
						Witchdoctor_Firebomb=67567,
						Witchdoctor_MassConfusion=67600,
						Witchdoctor_SoulHarvest=67616,
						Witchdoctor_Horrify=67668,
						Witchdoctor_GraspOfTheDead=69182,
						Witchdoctor_CorpseSpider=69866,
						Witchdoctor_Locust_Swarm=69867,
						Witchdoctor_AcidCloud=70455,
						Witchdoctor_FetishArmy=72785,
						Witchdoctor_ZombieCharger=74003,
						Witchdoctor_Haunt=83602,
						Witchdoctor_Sacrifice=102572,
						Witchdoctor_SummonZombieDog=102573,
						Witchdoctor_Firebats=105963,
						Witchdoctor_SpiritWalk=106237,
						Witchdoctor_PlagueOfToads=106465,
						Witchdoctor_SpiritBarrage=108506,
						Witchdoctor_BigBadVoodoo=117402,
						Witchdoctor_WallOfZombies=134837,

				 }
				 enum WitchDoctorPassiveSkills
				 {
						Witchdoctor_Passive_SpiritVessel=218501,
						Witchdoctor_Passive_FetishSycophants=218588,
						Witchdoctor_Passive_GraveInjustice=218191,
						Witchdoctor_Passive_BadMedicine=217826,
						Witchdoctor_Passive_JungleFortitude=217968,
						Witchdoctor_Passive_VisionQuest=209041,
						Witchdoctor_Passive_PierceTheVeil=208628,
						Witchdoctor_Passive_FierceLoyalty=208639,
						Witchdoctor_Passive_ZombieHandler=208563,
						Witchdoctor_Passive_RushOfEssence=208565,
						Witchdoctor_Passive_BloodRitual=208568,
						Witchdoctor_Passive_SpiritualAttunement=208569,
						Witchdoctor_Passive_CircleOfLife=208571,
						Witchdoctor_Passive_GruesomeFeast=208594,
						Witchdoctor_Passive_TribalRites=208601,

				 }

				//Base class for each individual class!
				public WitchDoctor(ActorClass a)
					 : base(a)
				{
					 this.RecreateAbilities();
				}
				public virtual Ability DefaultAttack
				{
					 get { return new WeaponMeleeInsant(); }
				}
				public override int MainPetCount
				{
					 get
					 {
						  return Bot.Character.PetData.Gargantuan;
					 }
				}
				public override bool IsMeleeClass
				{
					 get
					 {
						  return false;
					 }
				}

				public override Ability CreateAbility(SNOPower Power)
				{
					 WitchDoctorActiveSkills power=(WitchDoctorActiveSkills)Enum.Parse(typeof(WitchDoctorActiveSkills), Power.ToString());

					 switch (power)
					 {
							default:
								 return new Ability();
					 }
				}

		  }
	 
}