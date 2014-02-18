using System.Linq;

namespace FunkyBot.Cache
{
	///<summary>
	///
	///</summary>
	public class SNOProperties
	{
		public SNOProperties(int SNOID, string internalNameLower)
		{
			_isObstacle = CacheIDLookup.hashSNONavigationObstacles.Contains(SNOID);
			_isHealthWell = SNOID == 138989;
			_isTreasureGoblin = ObjectCache.SnoUnitPropertyCache.GoblinUnits.Contains(SNOID);//CacheIDLookup.hashActorSNOGoblins.Contains(SNOID);
			_isBoss = ObjectCache.SnoUnitPropertyCache.BossUnits.Contains(SNOID);//CacheIDLookup.hashBossSNO.Contains(SNOID);
			_isWormBoss = (SNOID == 218947 || SNOID == 144400);
			_isResplendantChest = CacheIDLookup.hashSNOContainerResplendant.Contains(SNOID);
			_isAvoidance = AvoidanceCache.hashAvoidanceSNOList.Contains(SNOID);
			_isSummonedPet = CacheIDLookup.hashSummonedPets.Contains(SNOID);
			_isRespawnable = ObjectCache.SnoUnitPropertyCache.RevivableUnits.Contains(SNOID);//CacheIDLookup.hashActorSNOSummonedUnit.Contains(SNOID);
			_isProjectileAvoidance = AvoidanceCache.hashAvoidanceSNOProjectiles.Contains(SNOID);
			_ignoresLoSCheck = CacheIDLookup.hashActorSNOIgnoreLOSCheck.Contains(SNOID);
			_isMissileReflecting = ObjectCache.SnoUnitPropertyCache.ReflectiveMissleUnits.Contains(SNOID); //CacheIDLookup.hashActorSNOReflectiveMissleUnits.Contains(SNOID);
			_isStealthableUnit = ObjectCache.SnoUnitPropertyCache.StealthUnits.Contains(SNOID);// CacheIDLookup.hashActorSNOStealthUnits.Contains(SNOID);
			_isBurrowableUnit = ObjectCache.SnoUnitPropertyCache.BurrowableUnits.Contains(SNOID);//CacheIDLookup.hashActorSNOBurrowableUnits.Contains(SNOID);
			_isSucideBomber = ObjectCache.SnoUnitPropertyCache.SucideBomberUnits.Contains(SNOID);//CacheIDLookup.hashActorSNOSucideBomberUnits.Contains(SNOID);
			_isGrotesqueActor = ObjectCache.SnoUnitPropertyCache.GrotesqueUnits.Contains(SNOID); //CacheIDLookup.hashActorSNOCorpulent.Contains(SNOID);
			_isCorruptantGrowth = SNOID == 210120 || SNOID == 210268;
			_isSpawnerUnit = ObjectCache.SnoUnitPropertyCache.SpawnerUnits.Contains(SNOID);//CacheIDLookup.hashSpawnerUnitSNOs.Contains(SNOID);
			_isTransformUnit = CacheIDLookup.hashActorSNOTransforms.Contains(SNOID);
			_isFlyingHoverUnit = CacheIDLookup.hashActorSNOFlying.Contains(SNOID);
			_isCorpseContainer = (internalNameLower.Contains("loottype") || internalNameLower.Contains("corpse"));
			_isChestContainer = (internalNameLower.Contains("chest"));
		}
		public SNOProperties (int SNOID)
		{
		    
			_isObstacle = CacheIDLookup.hashSNONavigationObstacles.Contains(SNOID);
			_isHealthWell = SNOID == 138989;
			_isTreasureGoblin = ObjectCache.SnoUnitPropertyCache.GoblinUnits.Contains(SNOID);//CacheIDLookup.hashActorSNOGoblins.Contains(SNOID);
			_isBoss = ObjectCache.SnoUnitPropertyCache.BossUnits.Contains(SNOID);//CacheIDLookup.hashBossSNO.Contains(SNOID);
			_isWormBoss = (SNOID == 218947 || SNOID == 144400);
			_isResplendantChest = CacheIDLookup.hashSNOContainerResplendant.Contains(SNOID);
			_isAvoidance = AvoidanceCache.hashAvoidanceSNOList.Contains(SNOID);
			_isSummonedPet = CacheIDLookup.hashSummonedPets.Contains(SNOID);
			_isRespawnable = ObjectCache.SnoUnitPropertyCache.RevivableUnits.Contains(SNOID);//CacheIDLookup.hashActorSNOSummonedUnit.Contains(SNOID);
			_isProjectileAvoidance = AvoidanceCache.hashAvoidanceSNOProjectiles.Contains(SNOID);
			_ignoresLoSCheck = CacheIDLookup.hashActorSNOIgnoreLOSCheck.Contains(SNOID);
			_isMissileReflecting = ObjectCache.SnoUnitPropertyCache.ReflectiveMissleUnits.Contains(SNOID); //CacheIDLookup.hashActorSNOReflectiveMissleUnits.Contains(SNOID);
			_isStealthableUnit = ObjectCache.SnoUnitPropertyCache.StealthUnits.Contains(SNOID);// CacheIDLookup.hashActorSNOStealthUnits.Contains(SNOID);
			_isBurrowableUnit = ObjectCache.SnoUnitPropertyCache.BurrowableUnits.Contains(SNOID);//CacheIDLookup.hashActorSNOBurrowableUnits.Contains(SNOID);
			_isSucideBomber = ObjectCache.SnoUnitPropertyCache.SucideBomberUnits.Contains(SNOID);//CacheIDLookup.hashActorSNOSucideBomberUnits.Contains(SNOID);
			_isGrotesqueActor = ObjectCache.SnoUnitPropertyCache.GrotesqueUnits.Contains(SNOID); //CacheIDLookup.hashActorSNOCorpulent.Contains(SNOID);
			_isCorruptantGrowth = SNOID == 210120 || SNOID == 210268;
			_isSpawnerUnit = ObjectCache.SnoUnitPropertyCache.SpawnerUnits.Contains(SNOID);//CacheIDLookup.hashSpawnerUnitSNOs.Contains(SNOID);
			_isTransformUnit = CacheIDLookup.hashActorSNOTransforms.Contains(SNOID);
			_isFlyingHoverUnit = CacheIDLookup.hashActorSNOFlying.Contains(SNOID);
		}
		public SNOProperties() { }

		private bool _isObstacle;
		private bool _isHealthWell;
		private bool _isTreasureGoblin;
		private bool _isBoss ;
		private bool _isWormBoss;
		private bool _isResplendantChest;
		private bool _isAvoidance;
		private bool _isSummonedPet;
		private bool _isRespawnable;
		private bool _isProjectileAvoidance;
		private bool _isCorpseContainer;
		private bool _isChestContainer;
		private bool _ignoresLoSCheck;
		private bool _isMissileReflecting;
		private bool _isStealthableUnit;
		private bool _isBurrowableUnit;
		private bool _isSucideBomber;
		private bool _isGrotesqueActor ;
		private bool _isCorruptantGrowth ;
		private bool _isSpawnerUnit ;
		private bool _isTransformUnit ;
		private bool _isFlyingHoverUnit ;

		public bool IsObstacle
		{
			get { return _isObstacle; }
			set { _isObstacle = value; }
		}

		public bool IsHealthWell
		{
			get { return _isHealthWell; }
			set { _isHealthWell = value; }
		}

		public bool IsTreasureGoblin
		{
			get { return _isTreasureGoblin; }
			set { _isTreasureGoblin = value; }
		}

		public bool IsBoss
		{
			get { return _isBoss; }
			set { _isBoss = value; }
		}

		public bool IsWormBoss
		{
			get { return _isWormBoss; }
			set { _isWormBoss = value; }
		}

		public bool IsResplendantChest
		{
			get { return _isResplendantChest; }
			set { _isResplendantChest = value; }
		}

		public bool IsAvoidance
		{
			get { return _isAvoidance; }
			set { _isAvoidance = value; }
		}

		public bool IsSummonedPet
		{
			get { return _isSummonedPet; }
			set { _isSummonedPet = value; }
		}

		public bool IsRespawnable
		{
			get { return _isRespawnable; }
			set { _isRespawnable = value; }
		}

		public bool IsProjectileAvoidance
		{
			get { return _isProjectileAvoidance; }
			set { _isProjectileAvoidance = value; }
		}

		public bool IsCorpseContainer
		{
			get { return _isCorpseContainer; }
			set { _isCorpseContainer = value; }
		}

		public bool IsChestContainer
		{
			get { return _isChestContainer; }
			set { _isChestContainer = value; }
		}

		public bool IgnoresLosCheck
		{
			get { return _ignoresLoSCheck; }
			set { _ignoresLoSCheck = value; }
		}

		public bool IsMissileReflecting
		{
			get { return _isMissileReflecting; }
			set { _isMissileReflecting = value; }
		}

		public bool IsStealthableUnit
		{
			get { return _isStealthableUnit; }
			set { _isStealthableUnit = value; }
		}

		public bool IsBurrowableUnit
		{
			get { return _isBurrowableUnit; }
			set { _isBurrowableUnit = value; }
		}

		public bool IsSucideBomber
		{
			get { return _isSucideBomber; }
			set { _isSucideBomber = value; }
		}

		public bool IsGrotesqueActor
		{
			get { return _isGrotesqueActor; }
			set { _isGrotesqueActor = value; }
		}

		public bool IsCorruptantGrowth
		{
			get { return _isCorruptantGrowth; }
			set { _isCorruptantGrowth = value; }
		}

		public bool IsSpawnerUnit
		{
			get { return _isSpawnerUnit; }
			set { _isSpawnerUnit = value; }
		}

		public bool IsTransformUnit
		{
			get { return _isTransformUnit; }
			set { _isTransformUnit = value; }
		}

		public bool IsFlyingHoverUnit
		{
			get { return _isFlyingHoverUnit; }
			set { _isFlyingHoverUnit = value; }
		}


		public SNOProperties Clone()
		{
			return (SNOProperties)MemberwiseClone();
		}
	}
}
