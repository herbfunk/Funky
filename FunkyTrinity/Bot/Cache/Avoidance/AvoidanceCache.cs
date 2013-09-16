namespace FunkyTrinity.Avoidance
{
	public class AvoidanceCache
	{
		 internal static AvoidanceType FindAvoidanceUsingName(string Name)
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
					if (Name.StartsWith("woodwraith_sporecloud_emitter")) return AvoidanceType.TreeSpore;
			  }

			  return AvoidanceType.None;
		 }

		 internal static AvoidanceType FindAvoidanceUsingSNOID(int SNOID)
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
						 return AvoidanceType.TreeSpore;
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
					case 4176:
						 return AvoidanceType.PoisonGas;
					case 164829:
						 return AvoidanceType.SuccubusProjectile;
			  }
			  return AvoidanceType.None;
		 }

		 internal static readonly AvoidanceValue[] AvoidancesDefault=new AvoidanceValue[]
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
	}
}
