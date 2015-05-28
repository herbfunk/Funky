﻿using System;
using System.Globalization;
using fBaseXtensions.Cache.External.Enums;
using fBaseXtensions.Cache.External.Objects;
using fBaseXtensions.Cache.Internal.Avoidance;
using fBaseXtensions.Cache.Internal.Blacklist;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero;
using fBaseXtensions.Items;
using fBaseXtensions.Items.Enums;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;

namespace fBaseXtensions.Cache.Internal.Objects
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

		public SNO(int sno, string internalname, ActorType? actortype = null, TargetType? targettype = null, float? collisionradius = null, int? interactrange=null, bool? canburrow = null, bool? isbarricade = null, ObstacleType? obstacletype = null, float? actorsphereradius = null, GizmoType? gimzotype = null, PluginDroppedItemTypes? baseitemtype = null, UnitFlags? unitflags = null, GizmoTargetTypes? gizmotargettypes = null, CacheEntry snoentry = null)
		{
			//Creates the perm data
			SNOID = sno;
			_actortype = actortype;
			_targettype = targettype;
			_collisionradius = collisionradius;
			_internalname = internalname;
			_CanBurrow = canburrow;
			_IsBarricade = isbarricade;
			_obstacletype = obstacletype;
			_actorsphereradius = actorsphereradius;
			_gizmotype = gimzotype;
			_itemdroptype = baseitemtype;
			_unitflags = unitflags;
			_gizmoTargetTypes = gizmotargettypes;
			_snoentry = snoentry;
			UpdateLookUpFinalValues();
			IsFinalized = true;
		}
		public SNO(SNO sno)
		{
			SNOID = sno.SNOID;
			_actortype = sno.Actortype;
			_targettype = sno.targetType;
			_collisionradius = sno.CollisionRadius;
		    _interactRange = sno.InteractRange;
			_actorsphereradius = sno.ActorSphereRadius;
			_CanBurrow = sno.CanBurrow;
			//_GrantsNoXP = sno.GrantsNoXP;
			//_DropsNoLoot = sno.DropsNoLoot;
			_IsBarricade = sno.IsBarricade;
			_internalname = sno.InternalName;
			_obstacletype = sno.Obstacletype;
			_gizmotype = sno.Gizmotype;
			_itemdroptype = sno.ItemDropType;
			_unitflags = sno.UnitPropertyFlags;
			_gizmoTargetTypes = sno.GizmoTargetTypes;
			_snoentry = sno.snoentry;
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

        private readonly int? _interactRange;
        public int? InteractRange
        {
            get
            {
                if (IsFinalized) return _interactRange;

                if (ObjectCache.dictInteractRange.ContainsKey(SNOID)) return ObjectCache.dictInteractRange[SNOID];
                return null;
            }
            set
            {
                if (IsFinalized) return;

                ObjectCache.dictInteractRange[SNOID] = value;

            }
        }

		private readonly PluginDroppedItemTypes? _itemdroptype;
		public PluginDroppedItemTypes? ItemDropType
		{
			get
			{
				if (IsFinalized) return _itemdroptype;

				if (ObjectCache.dictBaseItemTypes.ContainsKey(SNOID)) return ObjectCache.dictBaseItemTypes[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized) return;

				ObjectCache.dictBaseItemTypes[SNOID] = value;

			}
		}
		private readonly UnitFlags? _unitflags;
		public UnitFlags? UnitPropertyFlags
		{
			get
			{
				if (IsFinalized) return _unitflags;

				if (ObjectCache.dictUnitFlags.ContainsKey(SNOID)) return ObjectCache.dictUnitFlags[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized) return;

				ObjectCache.dictUnitFlags[SNOID] = value;

			}
		}

		private readonly GizmoTargetTypes? _gizmoTargetTypes;
		public GizmoTargetTypes? GizmoTargetTypes
		{
			get
			{
				if (IsFinalized) return _gizmoTargetTypes;

				if (ObjectCache.dictGizmoTargetTypes.ContainsKey(SNOID)) return ObjectCache.dictGizmoTargetTypes[SNOID];
				return null;
			}
			set
			{
				if (IsFinalized) return;

				ObjectCache.dictGizmoTargetTypes[SNOID] = value;

			}
		}

		private readonly CacheEntry _snoentry;
		public CacheEntry snoentry
		{
			get
			{
				if (IsFinalized) return _snoentry;

				CacheEntry outvalue;
				if (TheCache.ObjectIDCache.TryGetCacheValue(SNOID, out outvalue))
					return outvalue;

				return null;
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
				debugstring += Actortype.HasValue ? "ActorType: " + Actortype.Value.ToString() + "   " : "";
                debugstring += targetType.HasValue ? "TargetType: " + targetType.Value.ToString() + "   " : "";
                debugstring += Obstacletype.HasValue ? "Obstacletype: " + Obstacletype.Value.ToString() + "\r\n" : "" + "\r\n";


				debugstring += CollisionRadius.HasValue ? "CollisionRadius: " + CollisionRadius.Value.ToString(CultureInfo.InvariantCulture) + " " : "";
				debugstring += ActorSphereRadius.HasValue ? "ActorSphereRadius: " + ActorSphereRadius.Value.ToString(CultureInfo.InvariantCulture) + " " + "\r\n" : "" + "\r\n";

				
				debugstring += IsBarricade.HasValue ? "IsBarricade: " + IsBarricade.Value.ToString() + " " + "\r\n" : "";
				debugstring += ItemDropType.HasValue ? "ItemBaseType: " + ItemDropType.Value.ToString() + " " + "\r\n" : "";
				debugstring += UnitPropertyFlags.HasValue ? "UnitFlags: " + UnitPropertyFlags.Value.ToString() + " " + "\r\n" : "";
				debugstring += GizmoTargetTypes.HasValue ? "GizmoTargetTypes: " + GizmoTargetTypes.Value.ToString() + " " + "\r\n" : "";
				debugstring += snoentry != null ? "SnoEntry: " + snoentry.ToString() : "";

				//
				return debugstring;

			}
		}


		#region Cache Lookup Properties
		public bool IgnoresLOSCheck 
		{ 
			get 
			{ 
				return IsBoss || IsWormBoss || BountyCache.RiftTrialIsActiveQuest; 
			} 
		}

		private bool _IsObstacle;
		public bool IsObstacle { get { if (IsFinalized) return _IsObstacle; return CacheIDLookup.hashSNONavigationObstacles.Contains(SNOID); } }

		private bool _IsHealthWell;
		public bool IsHealthWell { get { if (IsFinalized) return _IsHealthWell; return GizmoTargetTypes.HasValue && ObjectCache.CheckFlag(GizmoTargetTypes.Value, Enums.GizmoTargetTypes.Healthwell); } }

		private bool _IsTreasureGoblin;
		public bool IsTreasureGoblin { get { if (IsFinalized) return _IsTreasureGoblin; return UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.TreasureGoblin); } }

		private bool _IsBoss;
		public bool IsBoss { get { if (IsFinalized) return _IsBoss; return UnitPropertyFlags.HasValue && (ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Boss) || ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.AdventureModeBoss)); } }

		private bool _IsWormBoss;
		public bool IsWormBoss { get { if (IsFinalized) return _IsWormBoss; return UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Boss|UnitFlags.Worm); } }

		private bool _IsResplendantChest;
		public bool IsResplendantChest { get { if (IsFinalized) return _IsResplendantChest; return GizmoTargetTypes.HasValue && ObjectCache.CheckFlag(GizmoTargetTypes.Value, Enums.GizmoTargetTypes.Resplendant); } }

		private bool _IsAvoidance;
		public bool IsAvoidance { get { if (IsFinalized) return _IsAvoidance; return snoentry != null && snoentry.EntryType == EntryType.Avoidance; } }

		private bool _IsSummonedPet;
		public bool IsSummonedPet { get { if (IsFinalized) return _IsSummonedPet; return snoentry != null && snoentry.EntryType == EntryType.Pet; } }

		private bool _IsRespawnable;
		public bool IsRespawnable { get { if (IsFinalized) return _IsRespawnable; return UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Revivable); } }

		private bool _IsProjectileAvoidance;
		public bool IsProjectileAvoidance { get { if (IsFinalized) return _IsProjectileAvoidance; return IsAvoidance && AvoidanceCache.IsAvoidanceTypeProjectile(SNOID); } }
		
		private bool _IsCorpseContainer;
		public bool IsCorpseContainer { get { if (IsFinalized) return _IsCorpseContainer; return GizmoTargetTypes.HasValue && ObjectCache.CheckFlag(GizmoTargetTypes.Value, Enums.GizmoTargetTypes.Corpse); } }

		private bool _IsChestContainer;
		public bool IsChestContainer { get { if (IsFinalized) return _IsChestContainer; return GizmoTargetTypes.HasValue && ObjectCache.CheckFlag(GizmoTargetTypes.Value, Enums.GizmoTargetTypes.Chest); } }

		private bool _IsMissileReflecting;
		public bool IsMissileReflecting { get { if (IsFinalized) return _IsMissileReflecting; return UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.ReflectiveMissle); } }

		private bool _IsStealthableUnit;
		public bool IsStealthableUnit { get { if (IsFinalized) return _IsStealthableUnit; return UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Stealthable); } }

		private bool _IsBurrowableUnit;
		public bool IsBurrowableUnit { get { if (IsFinalized) return _IsBurrowableUnit; return UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Burrowing); } }

		private bool _IsSucideBomber;
		public bool IsSucideBomber { get { if (IsFinalized) return _IsSucideBomber; return UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.SucideBomber); } }

		private bool _IsGrotesqueActor;
		public bool IsGrotesqueActor { get { if (IsFinalized) return _IsGrotesqueActor; return UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Grotesque); } }

		private bool _IsCorruptantGrowth;
		public bool IsCorruptantGrowth { get { if (IsFinalized) return _IsCorruptantGrowth; return SNOID == 210120 || SNOID == 210268; } }

		private bool _IsSpawnerUnit;
		public bool IsSpawnerUnit { get { if (IsFinalized) return _IsSpawnerUnit; return UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Summoner); } }

		private bool _IsTransformUnit;
		public bool IsTransformUnit { get { if (IsFinalized) return _IsTransformUnit; return UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Transportable); } }

		private bool _IsFlyingHoverUnit;
		public bool IsFlyingHoverUnit { get { if (IsFinalized) return _IsFlyingHoverUnit; return UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Flying); } }

		private bool _IsDemonicForge;
		public bool IsDemonicForge { get { if (IsFinalized) return _IsDemonicForge; return SNOID == 174900 || SNOID == 185391; } }

		private bool _IsCursedChest;
		public bool IsCursedChest { get { if (IsFinalized) return _IsCursedChest; return SNOID == 365097 || SNOID == 364559; } }

		private bool _IsCursedShrine;
		public bool IsCursedShrine { get { if (IsFinalized) return _IsCursedShrine; return SNOID == 364601 || SNOID == 368169; } }

		private bool _IsAvoidanceSpawnerUnit;
		public bool IsAvoidanceSpawnerUnit { get { if (IsFinalized) return _IsAvoidanceSpawnerUnit; return UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.AvoidanceSummoner); } }

		private bool _IsMalletLordUnit;
		public bool IsMalletLordUnit { get { if (IsFinalized) return _IsMalletLordUnit; return UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.MalletLord); } }


		private void UpdateLookUpFinalValues()
		{
			_IsHealthWell = GizmoTargetTypes.HasValue && ObjectCache.CheckFlag(GizmoTargetTypes.Value, Enums.GizmoTargetTypes.Healthwell);
			_IsResplendantChest = GizmoTargetTypes.HasValue && ObjectCache.CheckFlag(GizmoTargetTypes.Value, Enums.GizmoTargetTypes.Resplendant);
			_IsCorpseContainer = GizmoTargetTypes.HasValue && ObjectCache.CheckFlag(GizmoTargetTypes.Value, Enums.GizmoTargetTypes.Corpse);
			_IsChestContainer = GizmoTargetTypes.HasValue && ObjectCache.CheckFlag(GizmoTargetTypes.Value, Enums.GizmoTargetTypes.Chest);

			_IsObstacle = CacheIDLookup.hashSNONavigationObstacles.Contains(SNOID);
			
			_IsTreasureGoblin = UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.TreasureGoblin);
			_IsBoss = UnitPropertyFlags.HasValue && (ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Boss) || ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.AdventureModeBoss));
			_IsWormBoss = UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Boss | UnitFlags.Worm);

			_IsAvoidance = snoentry != null && snoentry.EntryType == EntryType.Avoidance;
			_IsSummonedPet = snoentry != null && snoentry.EntryType == EntryType.Pet;
			_IsRespawnable = UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Revivable);
			_IsProjectileAvoidance = _IsAvoidance && AvoidanceCache.IsAvoidanceTypeProjectile(SNOID);
			
			_IsMissileReflecting = UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.ReflectiveMissle);
			_IsStealthableUnit = UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Stealthable);
			_IsBurrowableUnit = UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Burrowing);
			_IsSucideBomber = UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.SucideBomber);
			_IsGrotesqueActor = UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Grotesque);
			_IsCorruptantGrowth = SNOID == 210120 || SNOID == 210268;
			_IsSpawnerUnit = UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Summoner);
			_IsTransformUnit = UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Transportable);
			_IsFlyingHoverUnit = UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.Flying);
			_IsDemonicForge = SNOID == 174900 || SNOID == 185391;
			_IsCursedChest = SNOID == 365097 || SNOID == 364559;
			_IsCursedShrine = SNOID == 364601;
			_IsAvoidanceSpawnerUnit = UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.AvoidanceSummoner);
			_IsMalletLordUnit = UnitPropertyFlags.HasValue && ObjectCache.CheckFlag(UnitPropertyFlags.Value, UnitFlags.MalletLord);
		}

		#endregion

		public bool ContainsNullValues()
		{
			if (IsFinalized || SNOID == 0) return false;

			if (!targetType.HasValue || !Actortype.HasValue || InternalName == null || !Obstacletype.HasValue)
				return true;

			if (targetType.Value != TargetType.Item && targetType.Value != TargetType.Avoidance)
			{
				if (!CollisionRadius.HasValue || !ActorSphereRadius.HasValue) return true;

				if (targetType.Value == TargetType.Destructible || targetType.Value == TargetType.Barricade || targetType.Value == TargetType.Interactable)
				{
					if (!IsBarricade.HasValue || !Gizmotype.HasValue) return true;
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

		public CachedSNOEntry(int sno, string internalname, ActorType? actortype = null, TargetType? targettype = null, float? collisionradius = null, int? interactrange=null, bool? canburrow = null, bool? isbarricade = null, ObstacleType? obstacletype = null, float? actorsphereradius = null, GizmoType? gizmotype = null, PluginDroppedItemTypes? baseitemtype = null, UnitFlags? unitflags = null, GizmoTargetTypes? gizmotargettypes = null, CacheEntry snoentry = null)
			: base(sno, internalname,  actortype,  targettype, collisionradius, interactrange,  canburrow, isbarricade,  obstacletype,  actorsphereradius,  gizmotype,  baseitemtype,  unitflags, gizmotargettypes, snoentry)
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

			#region ActorType

			if (!Actortype.HasValue)
			{
				if (snoentry!=null)
				{
					Actortype = snoentry.ActorType;
					if (snoentry.EntryType == EntryType.Item)
					{
						CacheDroppedItemEntry droppedItemEntry = (CacheDroppedItemEntry)snoentry;
						ItemDropType = (PluginDroppedItemTypes)droppedItemEntry.ObjectType;
					}
					else if (snoentry.EntryType == EntryType.Gizmo)
					{
						CacheGizmoEntry gizmoEntry = (CacheGizmoEntry)snoentry;
						Gizmotype = (GizmoType)gizmoEntry.ObjectType;
						GizmoTargetTypes = gizmoEntry.GizmotargetType;
					    if (gizmoEntry.CollisionRadius > -1)
					        CollisionRadius = gizmoEntry.CollisionRadius;
					    if (gizmoEntry.InteractRange > -1)
					        InteractRange = gizmoEntry.InteractRange;
					}
					else if (snoentry.EntryType == EntryType.Unit)
					{
						CacheUnitEntry unitEntry = (CacheUnitEntry)snoentry;
						UnitPropertyFlags = (UnitFlags)unitEntry.ObjectType;
                        if (unitEntry.CollisionRadius > -1)
                            CollisionRadius = unitEntry.CollisionRadius;
                        if (unitEntry.InteractRange > -1)
                            InteractRange = unitEntry.InteractRange;
					}
					else if(snoentry.EntryType == EntryType.Pet)
					{
						targetType = TargetType.None;

						CacheUnitPetEntry petEntry = (CacheUnitPetEntry)snoentry;
						PetTypes pettype=(PetTypes)petEntry.ObjectType;
                        if (petEntry.CollisionRadius > -1)
                            CollisionRadius = petEntry.CollisionRadius;
                        if (petEntry.InteractRange > -1)
                            InteractRange = petEntry.InteractRange;
						if (pettype==PetTypes.WIZARD_ArcaneOrbs)
						{
							//Logger.DBLog.Debug("Arcane Orbs CacheSNO update!");
							Obstacletype = ObstacleType.None;
							return true;
						}
					}
				}
				else
				{
					try
					{
						Actortype = thisObj.ActorType;
					}
					catch
					{
						Logger.Write(LogLevel.Cache, "Failure to get actorType for object {0}", DebugStringSimple);
						return false;
					}

					//if (Actortype.Value == ActorType.Item)
					//{
					//	if (thisObj.IsEnvironmentRActor)
					//	{
					//		Logger.DBLog.DebugFormat("Ignoring Enviroment Item {0}", this.DebugStringSimple);
					//		BlacklistCache.IgnoreThisObject(this, raguid);
					//		return false;
					//	}
					//}

					//Ignored actor types..
					if (BlacklistCache.IgnoredActorTypes.Contains(Actortype.Value))
					{
						BlacklistCache.IgnoreThisObject(this,raguid);
						return false;
					}

				}
			}

			#endregion
			

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

							var cacheEntry = TheCache.ObjectIDCache.AvoidanceEntries[SNOID];
							var AT = AvoidanceType.None;
							if (cacheEntry != null) AT = (AvoidanceType)cacheEntry.ObjectType;


							if (IsProjectileAvoidance)
								Obstacletype = ObstacleType.MovingAvoidance;
							else
								Obstacletype = ObstacleType.StaticAvoidance;

							
							//Check if avoidance is enabled or if the avoidance type is set to 0
							if (!FunkyBaseExtension.Settings.Avoidance.AttemptAvoidanceMovements || AT != AvoidanceType.None && AvoidanceCache.IgnoringAvoidanceType(AT))
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
						if (Actortype.Value == ActorType.Monster)
						{
							targetType = TargetType.Unit;

							//No Monster Collision? (Illusionary Boots)
							if (!Equipment.NoMonsterCollision)
								Obstacletype = ObstacleType.Monster;
						}
						else if (Actortype.Value == ActorType.Item)
						{
							if (!ItemDropType.HasValue)
							{
								try
								{
									//Check if this is an item shown on a hero!
									if (thisObj.IsEnvironmentRActor)
									{
										BlacklistCache.IgnoreThisObject(this, raguid);
										return false;
									}
								}
								catch
								{

								}

								ItemStringEntry itemstringentry = ItemFunc.DetermineIsItemActorType(InternalName);
								if (itemstringentry == null)
								{
									Logger.DBLog.InfoFormat("Object {0} is considered Item but does not have a matching item name!", DebugStringSimple);
									BlacklistCache.IgnoreThisObject(this, raguid, true, false);
									return false;
								}

								ItemDropType = (PluginDroppedItemTypes)itemstringentry.ObjectType;
								if (FunkyBaseExtension.Settings.Debugging.DebuggingData)
								{
									ObjectCache.DebuggingData.CheckEntry(this);
								}
							}

							if (ItemDropType.Value == PluginDroppedItemTypes.Gold)
								targetType = TargetType.Gold;
							else if (ItemDropType.Value == PluginDroppedItemTypes.HealthGlobe)
								targetType = TargetType.Globe;
							else if (ItemDropType.Value == PluginDroppedItemTypes.PowerGlobe)
								targetType = TargetType.PowerGlobe;
							else
								targetType = TargetType.Item;


							//if (droppedItem == PluginDroppedItemTypes.Unknown)
							//	Logger.DBLog.InfoFormat("Unknown Item Type {0} -- {1}", SNOID, InternalName);


						}
						else if (Actortype.Value == ActorType.Gizmo)
						{

							GizmoType thisGizmoType;
							if (Gizmotype.HasValue)
								thisGizmoType = Gizmotype.Value;
							else
							{
								try
								{
									thisGizmoType = thisObj.ActorInfo.GizmoType;
								}
								catch
								{

									Logger.Write(LogLevel.Cache, "Failure to get actor Gizmo Type!");
									return false;
								}
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

                                if (FunkyGame.AdventureMode && BountyCache.IsParticipatingInTieredLootRun)
                                { 
                                    BlacklistCache.AddObjectToBlacklist(raguid, BlacklistType.Temporary);
                                    return false;
                                }
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

								    if (Gizmotype.HasValue && Gizmotype.Value.HasFlag(Enums.GizmoTargetTypes.Obstacle))
								    {
                                        Obstacletype = ObstacleType.ServerObject;
								        if (!CacheIDLookup.hashSNONavigationObstacles.Contains(SNOID))
								            CacheIDLookup.hashSNONavigationObstacles.Add(SNOID);
								    }
                                       
								}
							}
							else
							{//All other gizmos should be ignored!
								BlacklistCache.IgnoreThisObject(this, raguid);
								return false;
							}

							//Set Gizmotype Property!
							if (!Gizmotype.HasValue)
							{
								Gizmotype = thisGizmoType;

								if (targetType.Value==TargetType.Container)
								{
									if (IsCorpseContainer)
										GizmoTargetTypes=Enums.GizmoTargetTypes.Corpse;
									else if(IsResplendantChest)
										GizmoTargetTypes = Enums.GizmoTargetTypes.Resplendant;
									else if(IsChestContainer)
										GizmoTargetTypes = Enums.GizmoTargetTypes.Chest;
									else
										GizmoTargetTypes = Enums.GizmoTargetTypes.MiscContainer;
								}
								else if(targetType.Value == TargetType.Shrine)
								{
									if (Gizmotype.Value== GizmoType.PowerUp)
										GizmoTargetTypes = Enums.GizmoTargetTypes.Shrine;
									else if(IsHealthWell)
										GizmoTargetTypes = Enums.GizmoTargetTypes.Healthwell;
									else if(Gizmotype.Value==GizmoType.PoolOfReflection)
										GizmoTargetTypes = Enums.GizmoTargetTypes.PoolOfReflection;
								}
								else
								{
									GizmoTargetTypes = Enums.GizmoTargetTypes.None;
								}

								//We could not find ID using our cache.. lets log it!
								if (FunkyBaseExtension.Settings.Debugging.DebuggingData) //&& ObjectCache.CheckTargetTypeFlag(targetType.Value, TargetType.Container | TargetType.Barricade | TargetType.Destructible | TargetType.Door))
								{
									ObjectCache.DebuggingData.CheckEntry(this);
								}
							}


							if (targetType.Value == TargetType.Destructible || targetType.Value == TargetType.Barricade)
								Obstacletype = ObstacleType.Destructable;
							else if (targetType.Value == TargetType.Shrine || IsChestContainer || targetType.Value == TargetType.Door)
								Obstacletype = ObstacleType.ServerObject;
						}
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
							//else if (TestString.StartsWith("monsteraffix_"))
							//{
							//	AvoidanceType T = AvoidanceCache.FindAvoidanceUsingName(TestString);
							//	if (T == AvoidanceType.Wall)
							//	{
							//		//Add this to the obstacle navigation cache
							//		if (!IsObstacle)
							//			CacheIDLookup.hashSNONavigationObstacles.Add(SNOID);

							//		Obstacletype = ObstacleType.ServerObject;

							//		//Use unknown since we lookup SNO ID for server prop related objects.
							//		targetType = TargetType.None;
							//	}
							//	//else if (Bot.AvoidancesHealth.ContainsKey(T))
							//	//{
							//	//	 Logger.DBLog.InfoFormat("Found Avoidance not recongized by SNO! Name {0} SNO {1}", TestString, this.SNOID);
							//	//	 CacheIDLookup.hashAvoidanceSNOList.Add(this.SNOID);
							//	//	 this.targetType=TargetType.Avoidance;
							//	//}
							//	else
							//	{
							//		//Blacklist all other monster affixes
							//		BlacklistCache.IgnoreThisObject(this, raguid);
							//		return false;
							//	}
							//}
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
				catch (Exception ex)
				{
					Logger.Write(LogLevel.Cache, "Failure to get TargetType for object {0}\r\n{1}\r\n{2}", DebugStringSimple, ex.Message, ex.StackTrace);
					return false;
				}
				#endregion
			}


			if (!Obstacletype.HasValue)
				Obstacletype = ObstacleType.None;


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
				if (ObjectCache.CheckFlag(targetType.Value, TargetType.Destructible | TargetType.Interactable))
				{
					
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