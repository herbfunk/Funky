using System;
using System.Globalization;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using FunkyBot.Cache.Avoidance;
using FunkyBot.Cache.Enums;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;

namespace FunkyBot.Cache.Objects
{



	public abstract class SNO
	{
		#region Constructors
		public SNO(int sno, bool Null = false)
		{
			SNOID = sno;

			//create cache?
			if (!Null)
				ObjectCache.cacheSnoCollection.Add(sno);
		}
		public SNO(int sno)
		{
			SNOID = sno;

			CachedSNOEntry thisEntry = ObjectCache.cacheSnoCollection[sno];

			if (sno > 0)
			{
				SNOID = thisEntry.SNOID;
				_itembasetype = thisEntry.ItemBasetype;
				_actortype = thisEntry.Actortype;
				_targettype = thisEntry.targetType;
				_monstersize = thisEntry.Monstersize;
				_monstertype = thisEntry.Monstertype;
				_collisionradius = thisEntry.CollisionRadius;
				_actorsphereradius = thisEntry.ActorSphereRadius;
				_CanBurrow = thisEntry.CanBurrow;
				_GrantsNoXP = thisEntry.GrantsNoXP;
				_DropsNoLoot = thisEntry.DropsNoLoot;
				_IsBarricade = thisEntry.IsBarricade;
				_internalname = thisEntry.InternalName;
				_obstacletype = thisEntry.Obstacletype;
				_gizmotype = thisEntry.Gizmotype;
				//this._RunningRate=thisEntry.RunningRate;
				IsFinalized = thisEntry.IsFinalized;
			}
		}
		public SNO(int sno, String internalname, ActorType? actortype = null, TargetType? targettype = null, MonsterType? monstertype = null, MonsterSize? monstersize = null, float? collisionradius = null, bool? canburrow = null, bool? grantsnoxp = null, bool? dropsnoloot = null, bool? isbarricade = null, ObstacleType? obstacletype = null, float? actorsphereradius = null, GizmoType? gimzotype = null, PluginBaseItemTypes? baseitemtype=null)
		{
			//Creates the perm data
			SNOID = sno;
			_actortype = actortype;
			_targettype = targettype;
			_collisionradius = collisionradius;
			_monstersize = monstersize;
			_monstertype = monstertype;
			_internalname = internalname;
			_CanBurrow = canburrow;
			_DropsNoLoot = dropsnoloot;
			_GrantsNoXP = grantsnoxp;
			_IsBarricade = isbarricade;
			_obstacletype = obstacletype;
			_actorsphereradius = actorsphereradius;
			_gizmotype = gimzotype;
			_itembasetype = baseitemtype;
			UpdateLookUpFinalValues();
			IsFinalized = true;
		}
		public SNO(SNO sno)
		{
			SNOID = sno.SNOID;
			_actortype = sno.Actortype;
			_targettype = sno.targetType;
			_monstersize = sno.Monstersize;
			_monstertype = sno.Monstertype;
			_collisionradius = sno.CollisionRadius;
			_actorsphereradius = sno.ActorSphereRadius;
			_CanBurrow = sno.CanBurrow;
			_GrantsNoXP = sno.GrantsNoXP;
			_DropsNoLoot = sno.DropsNoLoot;
			_IsBarricade = sno.IsBarricade;
			_internalname = sno.InternalName;
			_obstacletype = sno.Obstacletype;
			_gizmotype = sno.Gizmotype;
			_itembasetype = sno.ItemBasetype;
			//this._RunningRate=sno.RunningRate;
			IsFinalized = sno.IsFinalized;
		}
		#endregion


		public readonly bool IsFinalized = false;
		public readonly int SNOID;
		public DateTime LastUsed = DateTime.Now;

		#region SNO Properties


		private bool? _CanBurrow;
		public bool? CanBurrow
		{
			get
			{
				if (IsFinalized)
					return _CanBurrow;

				if (ObjectCache.dictCanBurrow.ContainsKey(SNOID))
					return ObjectCache.dictCanBurrow[SNOID];
				return null;
			}
			set
			{
				if (!IsFinalized)
					ObjectCache.dictCanBurrow[SNOID] = value;
				else
					_CanBurrow = value;
			}
		}

		private readonly bool? _IsBarricade;
		public bool? IsBarricade
		{
			get
			{
				if (IsFinalized)
					return _IsBarricade;

				if (ObjectCache.dictIsBarricade.ContainsKey(SNOID))
					return ObjectCache.dictIsBarricade[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized)
					return;

				ObjectCache.dictIsBarricade[SNOID] = value;
			}
		}

		private readonly bool? _DropsNoLoot;
		public bool? DropsNoLoot
		{
			get
			{
				if (IsFinalized)
					return _DropsNoLoot;

				if (ObjectCache.dictDropsNoLoot.ContainsKey(SNOID))
					return ObjectCache.dictDropsNoLoot[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized)
					return;

				ObjectCache.dictDropsNoLoot[SNOID] = value;
			}
		}

		private readonly bool? _GrantsNoXP;
		public bool? GrantsNoXP
		{
			get
			{
				if (IsFinalized)
					return _GrantsNoXP;

				if (ObjectCache.dictGrantsNoXp.ContainsKey(SNOID))
					return ObjectCache.dictGrantsNoXp[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized)
					return;

				ObjectCache.dictGrantsNoXp[SNOID] = value;
			}
		}

		private readonly GizmoType? _gizmotype;
		public GizmoType? Gizmotype
		{
			get
			{
				if (IsFinalized)
					return _gizmotype;

				if (ObjectCache.dictGizmoType.ContainsKey(SNOID))
					return ObjectCache.dictGizmoType[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized)
					return;

				ObjectCache.dictGizmoType[SNOID] = value;
			}
		}

		private readonly float? _actorsphereradius;
		public float? ActorSphereRadius
		{
			get
			{
				if (IsFinalized)
					return _actorsphereradius;

				if (ObjectCache.dictActorSphereRadius.ContainsKey(SNOID))
					return ObjectCache.dictActorSphereRadius[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized)
					return;

				ObjectCache.dictActorSphereRadius[SNOID] = value;
			}
		}

		private readonly ObstacleType? _obstacletype;
		public ObstacleType? Obstacletype
		{
			get
			{
				if (IsFinalized)
					return _obstacletype;

				if (ObjectCache.dictObstacleType.ContainsKey(SNOID))
					return ObjectCache.dictObstacleType[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized)
					return;

				ObjectCache.dictObstacleType[SNOID] = value;
			}
		}

		private readonly ActorType? _actortype;
		public ActorType? Actortype
		{
			get
			{
				if (IsFinalized) return _actortype;

				if (ObjectCache.dictActorType.ContainsKey(SNOID)) return ObjectCache.dictActorType[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized) return;

				ObjectCache.dictActorType[SNOID] = value;
			}
		}

		private TargetType? _targettype;
		public TargetType? targetType
		{
			get
			{
				if (IsFinalized) return _targettype;

				if (ObjectCache.dictTargetType.ContainsKey(SNOID)) return ObjectCache.dictTargetType[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized)
				{
					_targettype = value;
					return;
				}

				ObjectCache.dictTargetType[SNOID] = value;
			}
		}

		private readonly float? _collisionradius;
		public float? CollisionRadius
		{
			get
			{
				if (IsFinalized) return _collisionradius;

				if (ObjectCache.dictCollisionRadius.ContainsKey(SNOID)) return ObjectCache.dictCollisionRadius[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized) return;

				ObjectCache.dictCollisionRadius[SNOID] = value;

			}
		}

		private MonsterType? _monstertype;
		public MonsterType? Monstertype
		{
			get
			{
				if (IsFinalized) return _monstertype;

				if (ObjectCache.dictMonstertype.ContainsKey(SNOID)) return ObjectCache.dictMonstertype[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized)
				{
					_monstertype = value;
					return;
				}

				ObjectCache.dictMonstertype[SNOID] = value;
			}
		}

		private readonly MonsterSize? _monstersize;
		public MonsterSize? Monstersize
		{
			get
			{
				if (IsFinalized) return _monstersize;

				if (ObjectCache.dictMonstersize.ContainsKey(SNOID)) return ObjectCache.dictMonstersize[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized) return;

				ObjectCache.dictMonstersize[SNOID] = value;

			}
		}

		private readonly PluginBaseItemTypes? _itembasetype;
		public PluginBaseItemTypes? ItemBasetype
		{
			get
			{
				if (IsFinalized) return _itembasetype;

				if (ObjectCache.dictBaseItemTypes.ContainsKey(SNOID)) return ObjectCache.dictBaseItemTypes[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized) return;

				ObjectCache.dictBaseItemTypes[SNOID] = value;

			}
		}

		private readonly string _internalname;
		public string InternalName
		{
			get
			{
				if (IsFinalized) return _internalname;


				if (ObjectCache.dictInternalName.ContainsKey(SNOID)) return ObjectCache.dictInternalName[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized) return;

				ObjectCache.dictInternalName[SNOID] = value;
			}
		}
		#endregion

		private string internalnamelower;
		internal string internalNameLower
		{
			get
			{
				if (internalnamelower == null)
					internalnamelower = InternalName.ToLower();

				return internalnamelower;
			}
		}



		public string DebugStringSimple
		{
			get
			{
				return String.Format("({0}) SNO {1} ActorType {2} TargetType {3}",
										InternalName, SNOID,
										Actortype.HasValue ? Actortype.Value.ToString() : "Null",
										targetType.HasValue ? targetType.Value.ToString() : "Null");
			}
		}
		public virtual string DebugString
		{
			get
			{
				string debugstring = "SNO: " + SNOID + "(" + InternalName + ")\r\n";
				debugstring += Actortype.HasValue ? "ActorType: " + Actortype.Value.ToString() + " " : "";
				debugstring += targetType.HasValue ? "TargetType: " + targetType.Value.ToString() + " " + "\r\n" : "" + "\r\n";

				debugstring += CollisionRadius.HasValue ? "CollisionRadius: " + CollisionRadius.Value.ToString(CultureInfo.InvariantCulture) + " " : "";
				debugstring += ActorSphereRadius.HasValue ? "ActorSphereRadius: " + ActorSphereRadius.Value.ToString(CultureInfo.InvariantCulture) + " " + "\r\n" : "" + "\r\n";

				debugstring += Monstertype.HasValue ? "Monstertype: " + Monstertype.Value.ToString() + " " : "";
				debugstring += Monstersize.HasValue ? "Monstersize: " + Monstersize.Value.ToString() + " " + "\r\n" : "";
				//debugstring+=RunningRate.HasValue?"RunningRate: "+RunningRate.Value.ToString()+" "+"\r\n":"";

				debugstring += GrantsNoXP.HasValue ? "GrantsNoXP: " + GrantsNoXP.Value.ToString() + " " : "";
				debugstring += DropsNoLoot.HasValue ? "DropsNoLoot: " + DropsNoLoot.Value.ToString() + " " : "";
				debugstring += IsBarricade.HasValue ? "IsBarricade: " + IsBarricade.Value.ToString() + " " + "\r\n" : "";
				debugstring += ItemBasetype.HasValue ? "ItemBaseType: " + ItemBasetype.Value.ToString() + " " + "\r\n" : "";
				return debugstring;

			}
		}


		#region Cache Lookup Properties
		public bool IgnoresLOSCheck { get { return IsBoss; } }

		private bool _IsObstacle;
		public bool IsObstacle { get { if (IsFinalized) return _IsObstacle; return CacheIDLookup.hashSNONavigationObstacles.Contains(SNOID); } }
		
		private bool _IsHealthWell;
		public bool IsHealthWell { get { if (IsFinalized) return _IsHealthWell; return SNOID == 138989; } }
		
		private bool _IsTreasureGoblin;
		public bool IsTreasureGoblin { get { if (IsFinalized) return _IsTreasureGoblin; return ObjectCache.SNOCache.Units.GoblinUnits.Contains(SNOID); } }
		
		private bool _IsBoss;
		public bool IsBoss { get { if (IsFinalized) return _IsBoss; return ObjectCache.SNOCache.Units.BossUnits.Contains(SNOID); } }
		
		private bool _IsWormBoss;
		public bool IsWormBoss { get { if (IsFinalized) return _IsWormBoss; return (SNOID == 218947 || SNOID == 144400); } }
		
		private bool _IsResplendantChest;
		public bool IsResplendantChest { get { if (IsFinalized) return _IsResplendantChest; return ObjectCache.SNOCache.Gizmos.ResplendantChests.Contains(SNOID); } }
		
		private bool _IsAvoidance;
		public bool IsAvoidance { get { if (IsFinalized) return _IsAvoidance; return AvoidanceCache.hashAvoidanceSNOList.Contains(SNOID); } }
		
		private bool _IsSummonedPet;
		public bool IsSummonedPet { get { if (IsFinalized) return _IsSummonedPet; return CacheIDLookup.hashSummonedPets.Contains(SNOID); } }
		
		private bool _IsRespawnable;
		public bool IsRespawnable { get { if (IsFinalized) return _IsRespawnable; return ObjectCache.SNOCache.Units.RevivableUnits.Contains(SNOID); } }
		
		private bool _IsProjectileAvoidance;
		public bool IsProjectileAvoidance { get { if (IsFinalized) return _IsProjectileAvoidance; return AvoidanceCache.hashAvoidanceSNOProjectiles.Contains(SNOID); } }
		
		private bool _IsCorpseContainer;
		public bool IsCorpseContainer { get { if (IsFinalized) return _IsCorpseContainer; return (internalNameLower.Contains("loottype") || internalNameLower.Contains("corpse")); } }
		
		private bool _IsChestContainer;
		public bool IsChestContainer { get { if (IsFinalized) return _IsChestContainer; return (internalNameLower.Contains("chest")); } }

		private bool _IsMissileReflecting;
		public bool IsMissileReflecting { get { if (IsFinalized) return _IsMissileReflecting; return ObjectCache.SNOCache.Units.ReflectiveMissleUnits.Contains(SNOID); } }

		private bool _IsStealthableUnit;
		public bool IsStealthableUnit { get { if (IsFinalized) return _IsStealthableUnit; return ObjectCache.SNOCache.Units.StealthUnits.Contains(SNOID); } }

		private bool _IsBurrowableUnit;
		public bool IsBurrowableUnit { get { if (IsFinalized) return _IsBurrowableUnit; return ObjectCache.SNOCache.Units.BurrowableUnits.Contains(SNOID); } }

		private bool _IsSucideBomber;
		public bool IsSucideBomber { get { if (IsFinalized) return _IsSucideBomber; return ObjectCache.SNOCache.Units.SucideBomberUnits.Contains(SNOID); } }

		private bool _IsGrotesqueActor;
		public bool IsGrotesqueActor { get { if (IsFinalized) return _IsGrotesqueActor; return ObjectCache.SNOCache.Units.GrotesqueUnits.Contains(SNOID); } }

		private bool _IsCorruptantGrowth;
		public bool IsCorruptantGrowth { get { if (IsFinalized) return _IsCorruptantGrowth; return SNOID == 210120 || SNOID == 210268; } }

		private bool _IsSpawnerUnit;
		public bool IsSpawnerUnit { get { if (IsFinalized) return _IsSpawnerUnit; return ObjectCache.SNOCache.Units.SpawnerUnits.Contains(SNOID); } }

		private bool _IsTransformUnit;
		public bool IsTransformUnit { get { if (IsFinalized) return _IsTransformUnit; return CacheIDLookup.hashActorSNOTransforms.Contains(SNOID); } }

		private bool _IsFlyingHoverUnit;
		public bool IsFlyingHoverUnit { get { if (IsFinalized) return _IsFlyingHoverUnit; return CacheIDLookup.hashActorSNOFlying.Contains(SNOID); } }

		private bool _IsDemonicForge;
		public bool IsDemonicForge { get { if (IsFinalized) return _IsDemonicForge; return SNOID == 174900 || SNOID == 185391; } }

		private bool _IsCursedChest;
		public bool IsCursedChest { get { if (IsFinalized) return _IsCursedChest; return SNOID == 365097 || SNOID == 364559; } }

		private bool _IsCursedShrine;
		public bool IsCursedShrine { get { if (IsFinalized) return _IsCursedShrine; return SNOID == 364601 || SNOID == 368169; } }

		private bool _IsAvoidanceSpawnerUnit;
		public bool IsAvoidanceSpawnerUnit { get { if (IsFinalized) return _IsAvoidanceSpawnerUnit; return CacheIDLookup.hashUnitAvoidanceSpawner.Contains(SNOID); } }

		private bool _IsMalletLordUnit;
		public bool IsMalletLordUnit { get { if (IsFinalized) return _IsMalletLordUnit; return CacheIDLookup.hashUnitMalletLord.Contains(SNOID); } }

		private bool _IsSlimeGeyser;
		public bool IsSlimeGeyser { get { if (IsFinalized) return _IsSlimeGeyser; return SNOID == 218228; } }


		private void UpdateLookUpFinalValues()
		{
			_IsObstacle = CacheIDLookup.hashSNONavigationObstacles.Contains(SNOID);
			_IsHealthWell = SNOID == 138989;
			_IsTreasureGoblin = ObjectCache.SNOCache.Units.GoblinUnits.Contains(SNOID);
			_IsBoss = ObjectCache.SNOCache.Units.BossUnits.Contains(SNOID);
			_IsWormBoss = (SNOID == 218947 || SNOID == 144400);
			_IsResplendantChest = ObjectCache.SNOCache.Gizmos.ResplendantChests.Contains(SNOID);
			_IsAvoidance = AvoidanceCache.hashAvoidanceSNOList.Contains(SNOID);
			_IsSummonedPet = CacheIDLookup.hashSummonedPets.Contains(SNOID);
			_IsRespawnable = ObjectCache.SNOCache.Units.RevivableUnits.Contains(SNOID);
			_IsProjectileAvoidance = AvoidanceCache.hashAvoidanceSNOProjectiles.Contains(SNOID);
			_IsCorpseContainer = (internalNameLower.Contains("loottype") || internalNameLower.Contains("corpse"));
			_IsChestContainer = (internalNameLower.Contains("chest"));
			_IsMissileReflecting = ObjectCache.SNOCache.Units.ReflectiveMissleUnits.Contains(SNOID);
			_IsStealthableUnit = ObjectCache.SNOCache.Units.StealthUnits.Contains(SNOID);
			_IsBurrowableUnit = ObjectCache.SNOCache.Units.BurrowableUnits.Contains(SNOID);
			_IsSucideBomber = ObjectCache.SNOCache.Units.SucideBomberUnits.Contains(SNOID);
			_IsGrotesqueActor = ObjectCache.SNOCache.Units.GrotesqueUnits.Contains(SNOID);
			_IsCorruptantGrowth = SNOID == 210120 || SNOID == 210268;
			_IsSpawnerUnit = ObjectCache.SNOCache.Units.SpawnerUnits.Contains(SNOID);
			_IsTransformUnit = CacheIDLookup.hashActorSNOTransforms.Contains(SNOID);
			_IsFlyingHoverUnit = CacheIDLookup.hashActorSNOFlying.Contains(SNOID);
			_IsDemonicForge = SNOID == 174900 || SNOID == 185391;
			_IsCursedChest = SNOID == 365097 || SNOID == 364559;
			_IsCursedShrine = SNOID == 364601;
			_IsAvoidanceSpawnerUnit = CacheIDLookup.hashUnitAvoidanceSpawner.Contains(SNOID);
			_IsMalletLordUnit = CacheIDLookup.hashUnitMalletLord.Contains(SNOID);
			_IsSlimeGeyser = SNOID == 218228;
		}

		#endregion

		public bool ContainsNullValues()
		{
			if (IsFinalized || SNOID == 0) return false;

			if (!targetType.HasValue || !Actortype.HasValue || InternalName == null || !Obstacletype.HasValue)
				return true;

			if (targetType.Value == TargetType.Unit)
			{
				if (!Monstertype.HasValue || !Monstersize.HasValue) return true; //||!this.RunningRate.HasValue
			}

			if (targetType.Value != TargetType.Item && targetType.Value != TargetType.Avoidance)
			{
				if (!CollisionRadius.HasValue || !ActorSphereRadius.HasValue) return true;

				if (targetType.Value == TargetType.Destructible || targetType.Value == TargetType.Barricade || targetType.Value == TargetType.Interactable)
				{
					if (!DropsNoLoot.HasValue || !GrantsNoXP.HasValue || !IsBarricade.HasValue || !Gizmotype.HasValue) return true;
				}
			}
			return false;
		}

		public virtual object Clone()
		{
			return MemberwiseClone();
		}
	}

	///<summary>
	///Caches all SNO related data.
	///</summary>
	public class CachedSNOEntry : SNO
	{

		public CachedSNOEntry(int sno, String internalname, ActorType? actortype = null, TargetType? targettype = null, MonsterType? monstertype = null, MonsterSize? monstersize = null, float? collisionradius = null, bool? canburrow = null, bool? grantsnoxp = null, bool? dropsnoloot = null, bool? isbarricade = null, ObstacleType? obstacletype = null, float? actorsphereradius = null, GizmoType? gizmotype = null, PluginBaseItemTypes? baseitemtype=null)
			: base(sno, internalname, actortype, targettype, monstertype, monstersize, collisionradius, canburrow, grantsnoxp, dropsnoloot, isbarricade, obstacletype, actorsphereradius, gizmotype, baseitemtype)
		{
		}

		public CachedSNOEntry(int sno, bool Null = false)
			: base(sno, Null)
		{
		}

		public CachedSNOEntry(CachedSNOEntry parent)
			: base(parent)
		{
		}


		public bool ShouldRefreshMonsterType
		{
			get
			{
				if (!Monstertype.HasValue)
					return true;
				return ((Monstertype == MonsterType.Ally || Monstertype == MonsterType.Scenery ||
						 Monstertype == MonsterType.Helper || Monstertype == MonsterType.Team));
			}
		}
		public bool MonsterTypeIsHostile()
		{
			switch (Monstertype.Value)
			{
				case MonsterType.Ally:
				case MonsterType.Scenery:
				case MonsterType.Helper:
				case MonsterType.Team:
					return false;
			}

			return true;
		}


		///<summary>
		///Updates SNO Cache Values
		///</summary>
		public virtual bool UpdateData(DiaObject thisObj, int raguid)
		{
			bool failureDuringUpdate = false;

			if (InternalName == null)
			{
				try
				{
					InternalName = thisObj.Name;
				}
				catch
				{
					Logger.Write(LogLevel.Cache, "Failure to get internal name on object, SNO {0}", SNOID);

					return false;
				}
			}

			if (!Actortype.HasValue)
			{
				#region ActorType
				try
				{
					Actortype = thisObj.ActorType;
				}
				catch
				{
					
					if (ItemFunc.DetermineIsItemActorType(InternalName))
						Actortype = ActorType.Item;
					else
					{
						Logger.Write(LogLevel.Cache, "Failure to get actorType for object {0}", DebugStringSimple);
						return false;
					}
				}
				#endregion
			}

			//Ignored actor types..
			if (BlacklistCache.IgnoredActorTypes.Contains(Actortype.Value))//||!LootBehaviorEnabled&&this.Actortype.Value==ActorType.Item)
			{
				BlacklistCache.IgnoreThisObject(this, raguid);
				return false;
			}

			if (!targetType.HasValue)
			{
				#region EvaluateTargetType
				try
				{
					//Evaluate Target Type..
					// See if it's an avoidance first from the SNO
					if (IsAvoidance || IsObstacle)
					{
						targetType = TargetType.None;

						if (IsAvoidance)
						{
							if (IsProjectileAvoidance)
								Obstacletype = ObstacleType.MovingAvoidance;
							else
								Obstacletype = ObstacleType.StaticAvoidance;

							AvoidanceType AT = AvoidanceCache.FindAvoidanceUsingSNOID(SNOID);

							//Check if avoidance is enabled or if the avoidance type is set to 0
							if (!Bot.Settings.Avoidance.AttemptAvoidanceMovements || AT != AvoidanceType.None && AvoidanceCache.IgnoringAvoidanceType(AT))
							{
								BlacklistCache.AddObjectToBlacklist(raguid, BlacklistType.Temporary);
								return false;
							}

							// Avoidance isn't disabled, so set this object type to avoidance
							targetType = TargetType.Avoidance;
						}
						else
							Obstacletype = ObstacleType.ServerObject;
					}
					else
					{
						// Calculate the object type of this object
						if (Actortype.Value == ActorType.Monster ||
							 CacheIDLookup.hashActorSNOForceTargetUnit.Contains(SNOID))
						{
							targetType = TargetType.Unit;

							//No Monster Collision? (Illusionary Boots)
							if (!Equipment.NoMonsterCollision)
								Obstacletype = ObstacleType.Monster;

							if (CacheIDLookup.hashActorSNOForceTargetUnit.Contains(SNOID))
							{
								//Fill in monster data?
								Actortype = ActorType.Monster;
							}
						}
						else if (Actortype.Value == ActorType.Item ||
							 CacheIDLookup.hashForceSNOToItemList.Contains(SNOID))
						{
							if (!ItemFunc.DetermineIsItemActorType(InternalName))
							{
								Logger.DBLog.InfoFormat("Object {0} is considered Item but does not have a matching item name!", DebugStringSimple);
								BlacklistCache.IgnoreThisObject(this, raguid, true, false);
								return false;
							}

							PluginDroppedItemTypes droppedItem=ItemFunc.DetermineDroppedItemType(this.SNOID);

							//string testname = InternalName.ToLower();
							//Check if this item is gold/globe..
							if (droppedItem== PluginDroppedItemTypes.Gold)
								targetType = TargetType.Gold;
							else if (droppedItem== PluginDroppedItemTypes.HealthGlobe)
								targetType = TargetType.Globe;
							else if (droppedItem== PluginDroppedItemTypes.PowerGlobe)
								targetType = TargetType.PowerGlobe;
							else
								targetType = TargetType.Item;
							//Gold/Globe?


							//if (droppedItem == PluginDroppedItemTypes.Unknown)
							//	Logger.DBLog.InfoFormat("Unknown Item Type {0} -- {1}", SNOID, InternalName);

							if (Bot.Settings.Debug.DebuggingData && droppedItem == PluginDroppedItemTypes.Unknown)
							{
								ObjectCache.DebuggingData.CheckEntry(this);
							}
						}
						else if (Actortype.Value == ActorType.Gizmo)
						{
							//Try Cache Lookup First!
							if (ObjectCache.SNOCache.Gizmos.Doors.Contains(SNOID))
							{
								targetType = TargetType.Door;
								Gizmotype = GizmoType.Door;
							}//Try Cache Lookup First!
							else if (ObjectCache.SNOCache.Gizmos.ResplendantChests.Contains(SNOID))
							{
								targetType = TargetType.Container;
								Gizmotype = GizmoType.Chest;
							}
							else if (ObjectCache.SNOCache.Gizmos.SpecialInteractables.Contains(SNOID))
							{
								targetType = TargetType.Interactable;
								Gizmotype = GizmoType.Switch;
							}
							else
							{
								GizmoType thisGizmoType;
								try
								{
									thisGizmoType = thisObj.ActorInfo.GizmoType;
								}
								catch
								{

									Logger.Write(LogLevel.Cache, "Failure to get actor Gizmo Type!");
									return false;
								}

								if (thisGizmoType == GizmoType.DestroyableObject || thisGizmoType == GizmoType.BreakableChest)
									targetType = TargetType.Destructible;
								else if (thisGizmoType == GizmoType.PowerUp || thisGizmoType == GizmoType.HealingWell || thisGizmoType == GizmoType.PoolOfReflection)
								{
									targetType = TargetType.Shrine;
								}
								else if (thisGizmoType == GizmoType.Chest)
								{
									if (IsCursedChest)
										targetType = TargetType.CursedChest;
									else
										targetType = TargetType.Container;
								}
								else if (thisGizmoType == GizmoType.BreakableDoor)
									targetType = TargetType.Barricade;
								else if (thisGizmoType == GizmoType.Door)
									targetType = TargetType.Door;
								else if (thisGizmoType == GizmoType.Waypoint || thisGizmoType == GizmoType.Portal || thisGizmoType == GizmoType.DungeonPortal || thisGizmoType == GizmoType.BossPortal || thisGizmoType == GizmoType.PortalDestination)
								{//Special Interactive Object -- Add to special cache!
									targetType = TargetType.ServerInteractable;
								}
								else if (thisGizmoType == GizmoType.Switch)
								{
									if (IsCursedShrine)
										targetType = TargetType.CursedShrine;
									else if (IsCursedChest)
										targetType = TargetType.CursedChest;
									else
									{//Misc Gizmos (Sometimes Opening Doors or Paths!)
										targetType = TargetType.Interactable;
									}
								}
								else
								{//All other gizmos should be ignored!
									BlacklistCache.IgnoreThisObject(this, raguid);
									return false;
								}

								//Set Gizmotype Property!
								if (!Gizmotype.HasValue) Gizmotype = thisGizmoType;

								//We could not find ID using our cache.. lets log it!
								//if (Bot.Settings.Debug.DebuggingData && ObjectCache.CheckTargetTypeFlag(targetType.Value, TargetType.Container | TargetType.Barricade | TargetType.Destructible | TargetType.Door))
								//{
								//	ObjectCache.DebuggingData.CheckEntry(this);
								//}
							}


							if (targetType.Value == TargetType.Destructible || targetType.Value == TargetType.Barricade)
								Obstacletype = ObstacleType.Destructable;
							else if (targetType.Value == TargetType.Shrine || IsChestContainer || targetType.Value == TargetType.Door)
								Obstacletype = ObstacleType.ServerObject;
						}
						else if (CacheIDLookup.hashSNOInteractWhitelist.Contains(SNOID))
							targetType = TargetType.Interactable;
						else if (Actortype.Value == ActorType.ServerProp)
						{
							string TestString = InternalName.ToLower();
							//Server props with Base in name are the destructibles "remains" which is considered an obstacle!
							if (TestString.Contains("base") || TestString.Contains("fence"))
							{
								//Add this to the obstacle navigation cache
								if (!IsObstacle)
									CacheIDLookup.hashSNONavigationObstacles.Add(SNOID);

								Obstacletype = ObstacleType.ServerObject;

								//Use unknown since we lookup SNO ID for server prop related objects.
								targetType = TargetType.None;
							}
							else if (TestString.StartsWith("monsteraffix_"))
							{
								AvoidanceType T = AvoidanceCache.FindAvoidanceUsingName(TestString);
								if (T == AvoidanceType.Wall)
								{
									//Add this to the obstacle navigation cache
									if (!IsObstacle)
										CacheIDLookup.hashSNONavigationObstacles.Add(SNOID);

									Obstacletype = ObstacleType.ServerObject;

									//Use unknown since we lookup SNO ID for server prop related objects.
									targetType = TargetType.None;
								}
								//else if (Bot.AvoidancesHealth.ContainsKey(T))
								//{
								//	 Logger.DBLog.InfoFormat("Found Avoidance not recongized by SNO! Name {0} SNO {1}", TestString, this.SNOID);
								//	 CacheIDLookup.hashAvoidanceSNOList.Add(this.SNOID);
								//	 this.targetType=TargetType.Avoidance;
								//}
								else
								{
									//Blacklist all other monster affixes
									BlacklistCache.IgnoreThisObject(this, raguid);
									return false;
								}
							}
							else
							{
								BlacklistCache.IgnoreThisObject(this, raguid);
								return false;
							}


						}
						else
						{//Misc?? Ignore it!
							BlacklistCache.IgnoreThisObject(this, raguid);
							return false;
						}
					}
				}
				catch(Exception ex)
				{
					Logger.Write(LogLevel.Cache, "Failure to get TargetType for object {0}\r\n{1}\r\n{2}", DebugStringSimple,ex.Message,ex.StackTrace);
					return false;
				}
				#endregion
			}


			if (!Obstacletype.HasValue)
				Obstacletype = ObstacleType.None;


			if (ObjectCache.CheckTargetTypeFlag(targetType.Value, TargetType.Unit))
			{
				SNORecordMonster monsterInfo;
				try
				{
					monsterInfo = thisObj.CommonData.MonsterInfo;
				}
				catch
				{
					Logger.Write(LogLevel.Cache, "Safely Handled MonsterInfo Exception for Object {0}", InternalName);
					return false;
				}


				if (!Monstertype.HasValue || ShouldRefreshMonsterType)
				{
					#region MonsterType
					try
					{
						Monstertype = monsterInfo.MonsterType;
					}
					catch
					{
						Logger.Write(LogLevel.Cache, "Failure to get MonsterType for SNO: {0}", SNOID);

						failureDuringUpdate = true;
					}
					#endregion
				}
				if (!Monstersize.HasValue)
				{
					#region MonsterSize
					try
					{
						Monstersize = monsterInfo.MonsterSize;
					}
					catch
					{
						Logger.Write(LogLevel.Cache, "Failure to get MonsterSize for SNO: {0}", SNOID);

						failureDuringUpdate = true;
					}
					#endregion
				}
			}


			if (Actortype.HasValue && targetType.HasValue &&
				 (Actortype.Value != ActorType.Item && targetType.Value != TargetType.Avoidance && targetType.Value != TargetType.ServerInteractable))
			{
				//Validate sphere info
				Sphere sphereInfo = thisObj.CollisionSphere;

				if (!CollisionRadius.HasValue)
				{
					#region CollisionRadius
					try
					{
						CollisionRadius = sphereInfo.Radius;
					}
					catch
					{
						Logger.Write(LogLevel.Cache, "Failure to get CollisionRadius for SNO: {0}", SNOID);

						failureDuringUpdate = true;
					}
					#endregion

					if (CacheIDLookup.dictFixedCollisionRadius.ContainsKey(SNOID))
					{//Override The Default Collision Sphere Value
						CollisionRadius = CacheIDLookup.dictFixedCollisionRadius[SNOID];
					}
				}

				if (!ActorSphereRadius.HasValue)
				{
					#region ActorSphereRadius
					try
					{
						ActorSphereRadius = thisObj.ActorInfo.Sphere.Radius;
					}
					catch
					{
						Logger.Write(LogLevel.Cache, "Safely handled getting attribute Sphere radius for gizmo {0}", InternalName);
						failureDuringUpdate = true;
					}
					#endregion
				}

				#region GizmoProperties
				if (ObjectCache.CheckTargetTypeFlag(targetType.Value, TargetType.Destructible | TargetType.Interactable))
				{
					//No Loot
					if (!DropsNoLoot.HasValue)
					{
						#region DropsNoLoot
						try
						{
							DropsNoLoot = thisObj.CommonData.GetAttribute<float>(ActorAttributeType.DropsNoLoot) <= 0;
						}
						catch
						{
							Logger.Write(LogLevel.Cache, "Safely handled reading DropsNoLoot for gizmo {0}", InternalName);
							failureDuringUpdate = true;
						}
						#endregion
					}
					//No XP
					if (!GrantsNoXP.HasValue)
					{
						#region GrantsNoXP
						try
						{

							GrantsNoXP = thisObj.CommonData.GetAttribute<float>(ActorAttributeType.GrantsNoXP) <= 0;
						}
						catch
						{
							Logger.Write(LogLevel.Cache, "Safely handled reading GrantsNoXp for gizmo {0}", InternalName);
							failureDuringUpdate = true;
						}
						#endregion
					}
					//Barricade flag
					if (!IsBarricade.HasValue)
					{
						#region Barricade
						try
						{
							IsBarricade = ((DiaGizmo)thisObj).IsBarricade;
						}
						catch
						{
							Logger.Write(LogLevel.Cache, "Safely handled getting attribute IsBarricade for gizmo {0}", InternalName);
							failureDuringUpdate = true;
						}
						#endregion
					}
				}
				#endregion
			}

			if (ObjectCache.CheckTargetTypeFlag(targetType.Value, TargetType.Item))
			{
				if (!ItemBasetype.HasValue)
				{
					ItemBasetype = ItemFunc.DetermineBaseItemType(InternalName, SNOID);
				}
			}


			return !failureDuringUpdate;
		}

		public override int GetHashCode()
		{
			return SNOID;
		}

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types. 
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			CachedSNOEntry p = (CachedSNOEntry)obj;
			return SNOID == p.SNOID;
		}


	}


}