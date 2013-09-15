using System;
using System.Linq;
using Zeta;
using System.Collections.Generic;
using FunkyTrinity.Enums;
using FunkyTrinity.Cache;

namespace FunkyTrinity
{
    public partial class Funky
    {

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
		  internal static readonly  AvoidanceValue[] AvoidancesDefault=new AvoidanceValue[]
			  {
				  new AvoidanceValue(AvoidanceType.ArcaneSentry, 1, 14), 
				  new AvoidanceValue(AvoidanceType.AzmodanBodies, 1, 47),
				  new AvoidanceValue(AvoidanceType.AzmodanFireball, 1, 16),
				  new AvoidanceValue(AvoidanceType.AzmodanPool, 1, 54),
				  new AvoidanceValue(AvoidanceType.BeeProjectile, 0.5, 2), 
				  new AvoidanceValue(AvoidanceType.BelialGround, 1, 25),
				  new AvoidanceValue(AvoidanceType.Dececrator, 1, 9),
				  new AvoidanceValue(AvoidanceType.DiabloMetor, 0.80, 28),
				  new AvoidanceValue(AvoidanceType.DiabloPrison, 1, 15),
				  new AvoidanceValue(AvoidanceType.Frozen, 1, 19), 
				  new AvoidanceValue(AvoidanceType.GrotesqueExplosion, 0.50, 20),
				  new AvoidanceValue(AvoidanceType.LacuniBomb, 0.25, 2),
				  new AvoidanceValue(AvoidanceType.MageFirePool, 1, 10), 
				  new AvoidanceValue(AvoidanceType.MoltenCore, 1, 20), 
				  new AvoidanceValue(AvoidanceType.MoltenTrail, 0.75, 6),
				  new AvoidanceValue(AvoidanceType.PlagueCloud, 0.75, 19),
				  new AvoidanceValue(AvoidanceType.PlagueHand, 1, 15),
				  new AvoidanceValue(AvoidanceType.PoisonGas, 0.5, 9), 
				  new AvoidanceValue(AvoidanceType.ShamanFireBall, 0.1, 2), 
				  new AvoidanceValue(AvoidanceType.SuccubusProjectile, 0.25, 2),
				  new AvoidanceValue(AvoidanceType.TreeSpore, 1, 13),
				  //?? value never makes it when deseralized, but is seralized.
				  new AvoidanceValue(AvoidanceType.None,0,0),
			  };

		  // The rough radius of each avoidance thing (from centre to edge!) in feet
		  private static readonly Dictionary<AvoidanceType, double> dictAvoidanceRadiusDefaultsType=new Dictionary<AvoidanceType, double>
            {

                {AvoidanceType.ArcaneSentry, 14},
					 {AvoidanceType.Dececrator, 9},
					 {AvoidanceType.MoltenCore, 20},
					 {AvoidanceType.MoltenTrail, 6},
					 {AvoidanceType.Frozen, 19},
					 {AvoidanceType.PlagueCloud, 19},    
					 {AvoidanceType.BeeProjectile, 2},
					 {AvoidanceType.LacuniBomb,2},
					 {AvoidanceType.ShamanFireBall,2}, 
					 {AvoidanceType.SuccubusProjectile, 2},
					 {AvoidanceType.TreeSpore, 13},
					 {AvoidanceType.PlagueHand, 15},
					 {AvoidanceType.MageFirePool, 10},	
					 {AvoidanceType.AzmodanPool, 54},
					 {AvoidanceType.AzmodanFireball, 16},
					 {AvoidanceType.AzmodanBodies, 47},  
					 {AvoidanceType.BelialGround, 25},
                {AvoidanceType.DiabloPrison, 15},
					 {AvoidanceType.DiabloMetor, 28},
					 {AvoidanceType.GrotesqueExplosion, 20},
					 {AvoidanceType.PoisonGas,9},

            };


		  private static readonly Dictionary<AvoidanceType, double> dictAvoidanceHealthBarbDefaultsType=new Dictionary<AvoidanceType, double>
            {

                {AvoidanceType.ArcaneSentry, 1},
					 {AvoidanceType.Dececrator, 0.80},
					 {AvoidanceType.MoltenCore, 0.75},
					 {AvoidanceType.MoltenTrail, 0.35},
					 {AvoidanceType.Frozen, 1},
					 {AvoidanceType.PlagueCloud, 0.25},                                                                        
                {AvoidanceType.BeeProjectile, 0.60},
					 {AvoidanceType.LacuniBomb,0},
					 {AvoidanceType.ShamanFireBall,0.25},
					 {AvoidanceType.SuccubusProjectile,0.25},
					 {AvoidanceType.TreeSpore, 0.75},
					 {AvoidanceType.PlagueHand, 0.75},
					 {AvoidanceType.MageFirePool, 0.95},	
					 {AvoidanceType.GrotesqueExplosion, 0.40}, 
					 {AvoidanceType.PoisonGas, 0.50},
{AvoidanceType.AzmodanPool, 0.75},
					 {AvoidanceType.AzmodanFireball, 0.5},
					 {AvoidanceType.AzmodanBodies, 0.75},  
					 {AvoidanceType.BelialGround, 1},
                {AvoidanceType.DiabloPrison, 1},
					 {AvoidanceType.DiabloMetor, 0.5},

            };

		  private static readonly Dictionary<AvoidanceType, double> dictAvoidanceHealthMonkDefaultsType=new Dictionary<AvoidanceType, double>
            {

                 {AvoidanceType.ArcaneSentry, 1},{AvoidanceType.Dececrator, 0.80},{AvoidanceType.MoltenCore, 0.75},{AvoidanceType.MoltenTrail, 0.35},{AvoidanceType.Frozen, 1},{AvoidanceType.PlagueCloud, 0.25},    
                                                                                      
                {AvoidanceType.BeeProjectile, 0.60},{AvoidanceType.LacuniBomb,0},{AvoidanceType.ShamanFireBall,0.25},{AvoidanceType.SuccubusProjectile,0.25},

					 {AvoidanceType.TreeSpore, 0.75},{AvoidanceType.PlagueHand, 0.75},{AvoidanceType.MageFirePool, 0.95},
					 {AvoidanceType.GrotesqueExplosion, 0.40},{AvoidanceType.PoisonGas, 0.50},
  
					 {AvoidanceType.AzmodanPool, 0.75},{AvoidanceType.AzmodanFireball, 0.5},{AvoidanceType.AzmodanBodies, 0.75},  
					 {AvoidanceType.BelialGround, 1},
                {AvoidanceType.DiabloPrison, 1},{AvoidanceType.DiabloMetor, 0.5},

            };

		  private static readonly Dictionary<AvoidanceType, double> dictAvoidanceHealthWizardDefaultsType=new Dictionary<AvoidanceType, double>
            {

                {AvoidanceType.ArcaneSentry, 1},{AvoidanceType.Dececrator, 1},{AvoidanceType.MoltenCore, 1},{AvoidanceType.MoltenTrail, 0.5},{AvoidanceType.Frozen, 1},{AvoidanceType.PlagueCloud, 0.5},    
                                                                                      
                {AvoidanceType.BeeProjectile,0.75},{AvoidanceType.LacuniBomb,0.25},{AvoidanceType.ShamanFireBall,0.10},{AvoidanceType.SuccubusProjectile,0.25},

					 {AvoidanceType.TreeSpore, 0.75},{AvoidanceType.PlagueHand, 0.75},{AvoidanceType.MageFirePool, 0.25},	
					 {AvoidanceType.GrotesqueExplosion, 0.40},{AvoidanceType.PoisonGas, 0.50},
  
					 {AvoidanceType.AzmodanPool, 1},{AvoidanceType.AzmodanFireball, 1},{AvoidanceType.AzmodanBodies, 1},  
					 {AvoidanceType.BelialGround, 1},
                {AvoidanceType.DiabloPrison, 1},{AvoidanceType.DiabloMetor, 0.8},

            };

		  private static readonly Dictionary<AvoidanceType, double> dictAvoidanceHealthWitchDefaultsType=new Dictionary<AvoidanceType, double>
            {

                {AvoidanceType.ArcaneSentry, 1},{AvoidanceType.Dececrator, 1},{AvoidanceType.MoltenCore, 1},{AvoidanceType.MoltenTrail, 0.5},{AvoidanceType.Frozen, 1},{AvoidanceType.PlagueCloud, 0.9},    
                                                                                      
                {AvoidanceType.BeeProjectile,0.50},{AvoidanceType.LacuniBomb,0.25},{AvoidanceType.ShamanFireBall,0.10},{AvoidanceType.SuccubusProjectile,0.25},

					 {AvoidanceType.TreeSpore, 1},{AvoidanceType.PlagueHand, 1},{AvoidanceType.MageFirePool, 0.95},	
					 {AvoidanceType.GrotesqueExplosion, 0.40},{AvoidanceType.PoisonGas, 0.50},
  
					 {AvoidanceType.AzmodanPool, 1},{AvoidanceType.AzmodanFireball, 1},{AvoidanceType.AzmodanBodies, 1},  
					 {AvoidanceType.BelialGround, 1},
                {AvoidanceType.DiabloPrison, 1},{AvoidanceType.DiabloMetor, 0.8},
					
            };

		  private static readonly Dictionary<AvoidanceType, double> dictAvoidanceHealthDemonDefaultsType=new Dictionary<AvoidanceType, double>
            {

                {AvoidanceType.ArcaneSentry, 1},{AvoidanceType.Dececrator, 1},{AvoidanceType.MoltenCore, 0.9},{AvoidanceType.MoltenTrail, 0.5},{AvoidanceType.Frozen, 1},{AvoidanceType.PlagueCloud, 0.9},    
                                                                                      
                {AvoidanceType.BeeProjectile,0.50},{AvoidanceType.LacuniBomb,0.25},{AvoidanceType.ShamanFireBall,0.10},{AvoidanceType.SuccubusProjectile,0.25},

					 {AvoidanceType.TreeSpore, 0.75},{AvoidanceType.PlagueHand, 0.75},{AvoidanceType.MageFirePool, 0.95},	
					 {AvoidanceType.GrotesqueExplosion, 0.40},{AvoidanceType.PoisonGas, 0.50},
  
					 {AvoidanceType.AzmodanPool, 1},{AvoidanceType.AzmodanFireball, 1},{AvoidanceType.AzmodanBodies, 1},  
					 {AvoidanceType.BelialGround, 1},
                {AvoidanceType.DiabloPrison, 1},{AvoidanceType.DiabloMetor, 0.8},
					 
            };

	    internal static readonly Dictionary<AvoidanceType, double> dictAvoidanceHealthOOCIDBehaviorDefaults=new Dictionary<AvoidanceType, double>
            {

                {AvoidanceType.ArcaneSentry, 1},{AvoidanceType.Dececrator, 1},{AvoidanceType.MoltenCore,1},{AvoidanceType.MoltenTrail, 1},{AvoidanceType.Frozen, 1},{AvoidanceType.PlagueCloud, 1},    
                                                                                      
                {AvoidanceType.BeeProjectile,1},{AvoidanceType.LacuniBomb,1},{AvoidanceType.ShamanFireBall,1},{AvoidanceType.SuccubusProjectile,1},

					 {AvoidanceType.TreeSpore, 1},{AvoidanceType.PlagueHand, 1},{AvoidanceType.MageFirePool, 1},
					 {AvoidanceType.GrotesqueExplosion, 0.40},{AvoidanceType.PoisonGas, 0.50},
  
					 {AvoidanceType.AzmodanPool, 1},{AvoidanceType.AzmodanFireball, 1},{AvoidanceType.AzmodanBodies, 1},  
					 {AvoidanceType.BelialGround, 1},
                {AvoidanceType.DiabloPrison, 1},{AvoidanceType.DiabloMetor, 0.8},
            };


		  // ****************************************************************
		  // *****    Avoidance-related dictionaries/defaults           *****
		  // ****************************************************************
		  #region Avoidance

		 internal static Dictionary<AvoidanceType, double> dictAvoidanceRadius=new Dictionary<AvoidanceType, double>(dictAvoidanceRadiusDefaultsType);

		 internal static Dictionary<AvoidanceType, double> dictAvoidanceHealthBarb=new Dictionary<AvoidanceType, double>(dictAvoidanceHealthBarbDefaultsType);

		 internal static Dictionary<AvoidanceType, double> dictAvoidanceHealthMonk=new Dictionary<AvoidanceType, double>(dictAvoidanceHealthMonkDefaultsType);

		 internal static Dictionary<AvoidanceType, double> dictAvoidanceHealthWizard=new Dictionary<AvoidanceType, double>(dictAvoidanceHealthWizardDefaultsType);

		 internal static Dictionary<AvoidanceType, double> dictAvoidanceHealthWitch=new Dictionary<AvoidanceType, double>(dictAvoidanceHealthWitchDefaultsType);

		 internal static Dictionary<AvoidanceType, double> dictAvoidanceHealthDemon=new Dictionary<AvoidanceType, double>(dictAvoidanceHealthDemonDefaultsType);

	    internal static Dictionary<AvoidanceType, double> ReturnDictionaryUsingActorClass(Zeta.Internals.Actors.ActorClass AC)
		  {
				switch (AC)
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

		  internal static List<AvoidanceValue> CreateAvoidanceValueList(Dictionary<AvoidanceType,double> health, Dictionary<AvoidanceType,double> radius)
		  {
				List<AvoidanceValue> returnList=new List<AvoidanceValue>();
				AvoidanceType[] avoidanceTypes=health.Keys.ToArray();

			  foreach (var avoidanceType in avoidanceTypes)
			  {
					returnList.Add(new AvoidanceValue(avoidanceType, health[avoidanceType], radius[avoidanceType]));
			  }

			  return returnList;
		  }
		  #endregion



    }
}