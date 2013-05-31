using System;
using Zeta;
using System.Collections.Generic;

namespace FunkyTrinity
{
    public partial class Funky
    {
		  public enum AvoidanceType
		  {
				ArcaneSentry,
				AzmodanBodies,
				AzmodanFireball,
				AzmodanPool,
				AzmodanOrb,
				BeeProjectile,
				BelialGround,
				Dececrator,
				DiabloMetor,
				DiabloPrison,
				Frozen,
				LacuniBomb,
				MageFirePool,
				MoltenCore,
				MoltenTrail,
				PlagueCloud,
				PlagueHand,
				PoisonTree,
				ShamanFireBall,
				SuccubusProjectile,
				Unknown,
				Wall,
		  }

		  //monsterAffix_Desecrator_telegraph
		  //monsterAffix_frozen
		  //monsterAffix_Molten_death
		  //monsterAffix_Molten_trail
		  //monsterAffix_Plagued
		  //monsterAffix_waller_model

		  //Monsters:
		  //skeletonMage_fire_groundPool
		  //FallenShaman_fireball_projectile
		  //woodwraith_sporecloud_emitter

		  //Special Bosses:
		  //Azmodan_orbOfAnnihilation_projectile
		  //Belial_armSlam_projectile
		  //Belial_groundProjectile

		  //??
		  //: 60108 Name: Belial_Spray_Marker-17841 Type: Unit Position <815.2499, 777.6721, -7.629395E-05> Distance 12.36347 Radius 6.043896


		  //Walls with 0 degree rotation are facing "north to south".
		  //Walls have 10-20f length, with 1-3f width

		  private static AvoidanceType FindAvoidanceUsingName(string Name)
		  {
				Name=Name.ToLower();
				if (Name.StartsWith("monsteraffix_"))
				{
					 if (Name.Contains("dececrator")) return AvoidanceType.Dececrator;
					 if (Name.Contains("frozen")) return AvoidanceType.Frozen;
					 if (Name.Contains("molten"))
					 {
						  if (Name.Contains("trail")) return AvoidanceType.MoltenTrail; else return AvoidanceType.MoltenCore;
					 }
					 if (Name.Contains("plagued")) return AvoidanceType.PlagueCloud;
					 if (Name.Contains("wall")) return AvoidanceType.Wall;
				}
				else if (Name.Contains("azmodan")||Name.Contains("belial")||Name.Contains("diablo"))
				{
					 //Bosses
					 if (Name.StartsWith("belial_armslam_projectile")) return AvoidanceType.BelialGround;
					 if (Name.StartsWith("belial_groundprojectile")) return AvoidanceType.BelialGround;
				}
				else
				{
					 if (Name.StartsWith("skeletonmage_fire_groundpool")) return AvoidanceType.MageFirePool;
					 if (Name.StartsWith("fallenshaman_fireball_projectile")) return AvoidanceType.ShamanFireBall;
					 if (Name.StartsWith("woodwraith_sporecloud_emitter")) return AvoidanceType.PoisonTree;
				}

				return AvoidanceType.Unknown;
		  }

		  private static AvoidanceType FindAvoidanceUsingSNOID(int SNOID)
		  {
				switch (SNOID)
				{
					 case 219702:
					 case 221225:
						  return AvoidanceType.ArcaneSentry;
					 case 84608:
						  return AvoidanceType.Dececrator;
					 case 5482:
					 case 6578:
						  return AvoidanceType.PoisonTree;
					 case 4803:
					 case 4804:
						  return AvoidanceType.MoltenCore;
					 case 95868:
						  return AvoidanceType.MoltenTrail;
					 case 108869:
						  return AvoidanceType.PlagueCloud;
					 case 402:
					 case 223675:
						  return AvoidanceType.Frozen;
					 case 5212:
						  return AvoidanceType.BeeProjectile;
					 case 3865:
						  return AvoidanceType.PlagueHand;
					 case 123124:
						  return AvoidanceType.AzmodanPool;
					 case 123842:
						  return AvoidanceType.AzmodanFireball;
					 case 123839:
						  return AvoidanceType.AzmodanBodies;
					 case 161822:
					 case 161833:
					 case 60108:
						  return AvoidanceType.BelialGround;
					 case 168031:
						  return AvoidanceType.DiabloPrison;
					 case 214845:
						  return AvoidanceType.DiabloMetor;
					 case 432:
						  return AvoidanceType.MageFirePool;
					 case 4546:
						  return AvoidanceType.LacuniBomb;
					 case 164829:
						  return AvoidanceType.SuccubusProjectile;
				}
				return AvoidanceType.Unknown;
		  }


		  // The rough radius of each avoidance thing (from centre to edge!) in feet
		  private static readonly Dictionary<AvoidanceType, float> dictAvoidanceRadiusDefaultsType=new Dictionary<AvoidanceType, float>
            {

                {AvoidanceType.ArcaneSentry, 10},{AvoidanceType.Dececrator, 13},{AvoidanceType.MoltenCore, 20},{AvoidanceType.MoltenTrail, 6},{AvoidanceType.Frozen, 17},{AvoidanceType.PlagueCloud, 17},    
                                                                                      
                {AvoidanceType.BeeProjectile, 3},{AvoidanceType.LacuniBomb,5},{AvoidanceType.ShamanFireBall,3}, {AvoidanceType.SuccubusProjectile, 3},

					 {AvoidanceType.PoisonTree, 14},{AvoidanceType.PlagueHand, 12},{AvoidanceType.MageFirePool, 10},	
  
					 {AvoidanceType.AzmodanPool, 54},{AvoidanceType.AzmodanFireball, 16},{AvoidanceType.AzmodanBodies, 47},  
					 {AvoidanceType.BelialGround, 25},
                {AvoidanceType.DiabloPrison, 15},{AvoidanceType.DiabloMetor, 28},

            };


		  private static readonly Dictionary<AvoidanceType, double> dictAvoidanceHealthBarbDefaultsType=new Dictionary<AvoidanceType, double>
            {

                {AvoidanceType.ArcaneSentry, 1},{AvoidanceType.Dececrator, 0.80},{AvoidanceType.MoltenCore, 0.75},{AvoidanceType.MoltenTrail, 0.35},{AvoidanceType.Frozen, 1},{AvoidanceType.PlagueCloud, 0.25},    
                                                                                      
                {AvoidanceType.BeeProjectile, 0.60},{AvoidanceType.LacuniBomb,0},{AvoidanceType.ShamanFireBall,0.25},{AvoidanceType.SuccubusProjectile,0.25},

					 {AvoidanceType.PoisonTree, 0.75},{AvoidanceType.PlagueHand, 0.75},{AvoidanceType.MageFirePool, 0.95},	
  
					 {AvoidanceType.AzmodanPool, 0.75},{AvoidanceType.AzmodanFireball, 0.5},{AvoidanceType.AzmodanBodies, 0.75},  
					 {AvoidanceType.BelialGround, 1},
                {AvoidanceType.DiabloPrison, 1},{AvoidanceType.DiabloMetor, 0.5},

            };

		  private static readonly Dictionary<AvoidanceType, double> dictAvoidanceHealthMonkDefaultsType=new Dictionary<AvoidanceType, double>
            {

                 {AvoidanceType.ArcaneSentry, 1},{AvoidanceType.Dececrator, 0.80},{AvoidanceType.MoltenCore, 0.75},{AvoidanceType.MoltenTrail, 0.35},{AvoidanceType.Frozen, 1},{AvoidanceType.PlagueCloud, 0.25},    
                                                                                      
                {AvoidanceType.BeeProjectile, 0.60},{AvoidanceType.LacuniBomb,0},{AvoidanceType.ShamanFireBall,0.25},{AvoidanceType.SuccubusProjectile,0.25},

					 {AvoidanceType.PoisonTree, 0.75},{AvoidanceType.PlagueHand, 0.75},{AvoidanceType.MageFirePool, 0.95},	
  
					 {AvoidanceType.AzmodanPool, 0.75},{AvoidanceType.AzmodanFireball, 0.5},{AvoidanceType.AzmodanBodies, 0.75},  
					 {AvoidanceType.BelialGround, 1},
                {AvoidanceType.DiabloPrison, 1},{AvoidanceType.DiabloMetor, 0.5},

            };

		  private static readonly Dictionary<AvoidanceType, double> dictAvoidanceHealthWizardDefaultsType=new Dictionary<AvoidanceType, double>
            {

                {AvoidanceType.ArcaneSentry, 1},{AvoidanceType.Dececrator, 1},{AvoidanceType.MoltenCore, 1},{AvoidanceType.MoltenTrail, 0.5},{AvoidanceType.Frozen, 1},{AvoidanceType.PlagueCloud, 0.5},    
                                                                                      
                {AvoidanceType.BeeProjectile,0.75},{AvoidanceType.LacuniBomb,0.25},{AvoidanceType.ShamanFireBall,0.10},{AvoidanceType.SuccubusProjectile,0.25},

					 {AvoidanceType.PoisonTree, 0.75},{AvoidanceType.PlagueHand, 0.75},{AvoidanceType.MageFirePool, 0.25},	
  
					 {AvoidanceType.AzmodanPool, 1},{AvoidanceType.AzmodanFireball, 1},{AvoidanceType.AzmodanBodies, 1},  
					 {AvoidanceType.BelialGround, 1},
                {AvoidanceType.DiabloPrison, 1},{AvoidanceType.DiabloMetor, 0.8},

            };

		  private static readonly Dictionary<AvoidanceType, double> dictAvoidanceHealthWitchDefaultsType=new Dictionary<AvoidanceType, double>
            {

                {AvoidanceType.ArcaneSentry, 1},{AvoidanceType.Dececrator, 1},{AvoidanceType.MoltenCore, 0.9},{AvoidanceType.MoltenTrail, 0.5},{AvoidanceType.Frozen, 1},{AvoidanceType.PlagueCloud, 0.9},    
                                                                                      
                {AvoidanceType.BeeProjectile,0.50},{AvoidanceType.LacuniBomb,0.25},{AvoidanceType.ShamanFireBall,0.10},{AvoidanceType.SuccubusProjectile,0.25},

					 {AvoidanceType.PoisonTree, 0.9},{AvoidanceType.PlagueHand, 1},{AvoidanceType.MageFirePool, 0.95},	
  
					 {AvoidanceType.AzmodanPool, 1},{AvoidanceType.AzmodanFireball, 1},{AvoidanceType.AzmodanBodies, 1},  
					 {AvoidanceType.BelialGround, 1},
                {AvoidanceType.DiabloPrison, 1},{AvoidanceType.DiabloMetor, 0.8},

            };

		  private static readonly Dictionary<AvoidanceType, double> dictAvoidanceHealthDemonDefaultsType=new Dictionary<AvoidanceType, double>
            {

                {AvoidanceType.ArcaneSentry, 1},{AvoidanceType.Dececrator, 1},{AvoidanceType.MoltenCore, 0.9},{AvoidanceType.MoltenTrail, 0.5},{AvoidanceType.Frozen, 1},{AvoidanceType.PlagueCloud, 0.9},    
                                                                                      
                {AvoidanceType.BeeProjectile,0.50},{AvoidanceType.LacuniBomb,0.25},{AvoidanceType.ShamanFireBall,0.10},{AvoidanceType.SuccubusProjectile,0.25},

					 {AvoidanceType.PoisonTree, 0.75},{AvoidanceType.PlagueHand, 0.75},{AvoidanceType.MageFirePool, 0.95},	
  
					 {AvoidanceType.AzmodanPool, 1},{AvoidanceType.AzmodanFireball, 1},{AvoidanceType.AzmodanBodies, 1},  
					 {AvoidanceType.BelialGround, 1},
                {AvoidanceType.DiabloPrison, 1},{AvoidanceType.DiabloMetor, 0.8},

            };

		  private static readonly Dictionary<AvoidanceType, double> dictAvoidanceHealthOOCIDBehaviorDefaults=new Dictionary<AvoidanceType, double>
            {

                {AvoidanceType.ArcaneSentry, 1},{AvoidanceType.Dececrator, 1},{AvoidanceType.MoltenCore,1},{AvoidanceType.MoltenTrail, 1},{AvoidanceType.Frozen, 1},{AvoidanceType.PlagueCloud, 1},    
                                                                                      
                {AvoidanceType.BeeProjectile,1},{AvoidanceType.LacuniBomb,1},{AvoidanceType.ShamanFireBall,1},{AvoidanceType.SuccubusProjectile,1},

					 {AvoidanceType.PoisonTree, 1},{AvoidanceType.PlagueHand, 1},{AvoidanceType.MageFirePool, 1},	
  
					 {AvoidanceType.AzmodanPool, 1},{AvoidanceType.AzmodanFireball, 1},{AvoidanceType.AzmodanBodies, 1},  
					 {AvoidanceType.BelialGround, 1},
                {AvoidanceType.DiabloPrison, 1},{AvoidanceType.DiabloMetor, 0.8},

            };


		  // ****************************************************************
		  // *****    Avoidance-related dictionaries/defaults           *****
		  // ****************************************************************
		  #region Avoidance
		  // A list of all the SNO's to avoid - you could add 
		  private static readonly HashSet<int> hashAvoidanceSNOList=new HashSet<int>
            {
                // Arcane        Arcane 2      Desecrator   Poison Tree    Molten Core   Molten Trail   Plague Cloud   Ice Balls     
                219702,          221225,       84608,       5482,6578,     4803, 4804,   95868,         108869,        402, 223675,             
                // Bees-Wasps    Plague-Hands  Azmo Pools   Azmo fireball  Azmo bodies   Belial 1       Belial 2      
                5212,            3865,         123124,      123842,        123839,       161822,        161833, 
                // Sha-Ball      Mol Ball      Mage Fire    Diablo Prison  Diablo Meteor Ice-trail      CaveLarva
                4103,            160154,       432,         168031,        214845,       260377,        4176,
					 //lacuni bomb		Succubus Bloodstar
					 4546,			   164829,
            };

		  // A list of SNO's that are projectiles (so constantly look for new locations while avoiding)
		  private static readonly HashSet<int> hashAvoidanceSNOProjectiles=new HashSet<int>
            {
                // Bees-Wasps  Sha-Ball   Mol Ball   Azmo fireball
                5212,          4103,      160154,    123842,		164829, 
            };

		  private static Dictionary<AvoidanceType, float> dictAvoidanceRadius=new Dictionary<AvoidanceType, float>(dictAvoidanceRadiusDefaultsType);

		  private static Dictionary<AvoidanceType, double> dictAvoidanceHealthBarb=new Dictionary<AvoidanceType, double>(dictAvoidanceHealthBarbDefaultsType);

		  private static Dictionary<AvoidanceType, double> dictAvoidanceHealthMonk=new Dictionary<AvoidanceType, double>(dictAvoidanceHealthMonkDefaultsType);

		  private static Dictionary<AvoidanceType, double> dictAvoidanceHealthWizard=new Dictionary<AvoidanceType, double>(dictAvoidanceHealthWizardDefaultsType);

		  private static Dictionary<AvoidanceType, double> dictAvoidanceHealthWitch=new Dictionary<AvoidanceType, double>(dictAvoidanceHealthWitchDefaultsType);

		  private static Dictionary<AvoidanceType, double> dictAvoidanceHealthDemon=new Dictionary<AvoidanceType, double>(dictAvoidanceHealthDemonDefaultsType);

		  private static Dictionary<AvoidanceType, double> ReturnDictionaryUsingActorClass(Zeta.Internals.Actors.ActorClass AC)
		  {
				switch (ActorClass)
				{
					 case Zeta.Internals.Actors.ActorClass.Barbarian:
						  return dictAvoidanceHealthBarb;
					 case Zeta.Internals.Actors.ActorClass.DemonHunter:
						  return dictAvoidanceHealthDemon;
					 case Zeta.Internals.Actors.ActorClass.Monk:
						  return dictAvoidanceHealthMonk;
					 case Zeta.Internals.Actors.ActorClass.WitchDoctor:
						  return dictAvoidanceHealthWitch;
					 case Zeta.Internals.Actors.ActorClass.Wizard:
						  return dictAvoidanceHealthWizard;
				}
				return null;
		  }
		  
		  #endregion
    }
}