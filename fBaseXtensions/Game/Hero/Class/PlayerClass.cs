using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game.Hero.Skills;
using fBaseXtensions.Game.Hero.Skills.Conditions;
using fBaseXtensions.Game.Hero.Skills.SkillObjects;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fBaseXtensions.Game.Hero.Class
{


	///<summary>
	///Used to describe the Current Player -- Class, Hotbar Abilities, Passives, etc..
	///</summary>
	public abstract class PlayerClass
	{
		//Base class for each individual class!
		protected PlayerClass()
		{
			
			Hotbar.RefreshHotbar();
			Hotbar.RefreshPassives();

			Skill healthPotionSkill = new DrinkHealthPotion();
			healthPotionSkill.Initialize();
			Skill.CreateSkillLogicConditions(ref healthPotionSkill);
			HealthPotionAbility = (DrinkHealthPotion)healthPotionSkill;

			

			Equipment.RefreshEquippedItemsList();

			Hotbar.OnSkillsChanged += HotbarSkillsChangedHandler;

			Logger.DBLog.InfoFormat("[Funky] Finished Creating Player Class");
		}




		///<summary>
		///The actor class of this bot.
		///</summary>
		public virtual ActorClass AC { get { return ActorClass.Invalid; } }

		///<summary>
		///This is used to determine things such as how we preform certain checks (I.E. Line Of Sight)
		///</summary>
		internal virtual bool IsMeleeClass
		{
			get { return false; }
		}


		// This is used so we don't use certain skills until we "top up" our primary resource by enough
		internal double iWaitingReservedAmount = 0d;
		internal bool bWaitingForSpecial = false;
		internal bool UsesDOTDPSAbility = false;

		///<summary>
		///Create Ability (Derieved classes override this!)
		///</summary>
		internal virtual Skill CreateAbility(SNOPower P)
		{
			return DefaultAttack;
		}

		internal DrinkHealthPotion HealthPotionAbility = new DrinkHealthPotion();

		internal virtual Skill DefaultAttack
		{
			get { return new WeaponMeleeInsant(); }
		}

		///<summary>
		///
		///</summary>
		internal bool CanUseDefaultAttack { get; set; }

		

		///<summary>
		///Check if Bot should generate a new ZigZag location.
		///</summary>
		internal virtual bool ShouldGenerateNewZigZagPath()
		{
			return true;
		}

		///<summary>
		///Generates a new ZigZag location.
		///</summary>
		internal virtual void GenerateNewZigZagPath() { }

		///<summary>
		///
		///</summary>
		internal virtual int MainPetCount { get { return 0; } }

		///<summary>
		///Animations that determine if character has been "vortexed"
		///</summary>
		internal virtual HashSet<SNOAnim> KnockbackLandAnims
		{
			get { return null; }
		}



		///<summary>
		///Used to check for a secondary hotbar set. Currently only used for wizards with Archon.
		///</summary>
		internal virtual bool SecondaryHotbarBuffPresent()
		{
			return false;
		}

		public bool ContainsNonRangedCombatSkill { get; set; }
		public bool ContainsAnyPrimarySkill { get; set; }

		internal virtual void RecreateAbilities()
		{
			ContainsAnyPrimarySkill = false;
			ContainsNonRangedCombatSkill = false;
			Abilities = new Dictionary<SNOPower, Skill>();


			var uninitalizedSkills=new Dictionary<SNOPower, Skill>();

			//Create the abilities
			foreach (var item in Hotbar.HotbarSkills)
			{
				Skill newAbility = FunkyGame.Hero.Class.CreateAbility(item.Power);

				if (newAbility.IsPrimarySkill) 
					ContainsAnyPrimarySkill = true;

				//combat ability set property
				if (ObjectCache.CheckFlag((SkillExecutionFlags.ClusterLocation | SkillExecutionFlags.ClusterTarget | SkillExecutionFlags.ClusterTargetNearest | SkillExecutionFlags.Location | SkillExecutionFlags.Target), newAbility.ExecutionType))
					newAbility.IsCombat = true;

				if (!ContainsNonRangedCombatSkill && !newAbility.IsRanged && !newAbility.IsProjectile && ObjectCache.CheckFlag((SkillExecutionFlags.Target | SkillExecutionFlags.ClusterTarget), newAbility.ExecutionType))
					ContainsNonRangedCombatSkill = true;

				uninitalizedSkills.Add(item.Power,newAbility);
				Logger.DBLog.DebugFormat("[Funky] Added Skill {0} using RuneIndex {1}", newAbility.Power, newAbility.RuneIndex);
			}

			
			foreach (var item in uninitalizedSkills)
			{
				Skill skill = item.Value;
				skill.Initialize();
				Skill.CreateSkillLogicConditions(ref skill);
				skill.SuccessfullyUsed += AbilitySuccessfullyUsed;
				Abilities.Add(item.Key, skill);
				Logger.DBLog.DebugFormat("[Funky] Skill {0} has been initalized", item.Key);
			}

			//Sort Abilities
			SortedAbilities = Abilities.Values.OrderByDescending(a => a.Priority).ThenBy(a => a.Range).ToList();

			//No default rage generation Ability.. then we add the Instant Melee Ability.
			if (!ContainsAnyPrimarySkill)
			{
				Skill defaultAbility = FunkyGame.Hero.Class.DefaultAttack;
				defaultAbility.Initialize();
				Skill.CreateSkillLogicConditions(ref defaultAbility);
				Abilities.Add(defaultAbility.Power, defaultAbility);
				//Hotbar.RuneIndexCache.Add(defaultAbility.Power, -1);
				Logger.DBLog.DebugFormat("[Funky] Added Skill {0}", defaultAbility.Power);

				//No Primary Skill.. Check if Default Attack can be used!
				if (!FunkyBaseExtension.Settings.Combat.AllowDefaultAttackAlways)
				{
					Logger.DBLog.Warn("**** Warning ****");
					Logger.DBLog.Warn("No Primary Skill Found and Allow Default Attack Always Is Disabled!");
					Logger.DBLog.Warn("This may cause idles to occur.. Enable AllowDefaultAttackAlways setting found under Combat Tab.");
					Logger.DBLog.Warn("**** Warning ****");
				}
			}
			LastUsedAbilities = new Skill[Abilities.Count];
			int indexCount = 0;
			foreach (var ability in Abilities.Values)
			{
				LastUsedAbilities[indexCount] = ability;
				indexCount++;
			}

			//
			if (AC == ActorClass.Monk && Hotbar.HasPower(SNOPower.Monk_ExplodingPalm))
				UsesDOTDPSAbility = true;
			if (AC == ActorClass.Witchdoctor && (Hotbar.HasPower(SNOPower.Witchdoctor_Haunt) || Hotbar.HasPower(SNOPower.Witchdoctor_Locust_Swarm)))
				UsesDOTDPSAbility = true;
			if (AC == ActorClass.Barbarian && Hotbar.HasPower(SNOPower.Barbarian_Rend))
				UsesDOTDPSAbility = true;

			LastUsedAbility = LastUsedAbilities[0];
			PowerPrime = DefaultAttack;
		}



		///<summary>
		///Sets criteria based on object given.
		///</summary>
		internal virtual Skill AbilitySelector(CacheUnit obj, bool IgnoreOutOfRange = false)
		{
			//Reset default attack can use
			CanUseDefaultAttack = !Hotbar.HasPower(DefaultAttack.Power) ? false : true;
			//Reset waiting for special!
			bWaitingForSpecial = false;

			ConditionCriteraTypes criterias = ConditionCriteraTypes.All;

			//Although the unit is a cluster exception.. we should verify it is not a clustered object.
			if (obj.IsClusterException && obj.BeingIgnoredDueToClusterLogic)
			{
				criterias = ConditionCriteraTypes.SingleTarget;
			}

			return AbilitySelector(criterias, IgnoreOutOfRange);
		}
		///<summary>
		///Selects first Ability that is successful in precast and combat testing.
		///</summary>
		private Skill AbilitySelector(ConditionCriteraTypes criteria = ConditionCriteraTypes.All, bool IgnoreOutOfRange = false)
		{
			Skill returnAbility = DefaultAttack;
			foreach (var item in SortedAbilities)
			{
				//Check precast conditions
				if (!item.CheckPreCastConditionMethod()) continue;

				//Check Combat Conditions!
				if (!item.CheckCombatConditionMethod(criteria))
				{
					continue;
				}

				//Check if we can execute or if it requires movement
				if (IgnoreOutOfRange)
				{
					if (item.DestinationVector != FunkyGame.Hero.Position)
						continue;
				}

				returnAbility = item;
				break;
			}

			//Setup Ability (sets vars according to current cache)
			Skill.SetupAbilityForUse(ref returnAbility);
			return returnAbility;
		}

		internal List<Skill> ReturnAllUsableAbilities(CacheUnit obj, bool IgnoreOutOfRange = false)
		{
			//Reset default attack can use
			CanUseDefaultAttack = !Abilities.ContainsKey(DefaultAttack.Power) ? false : true;

			ConditionCriteraTypes criterias = ConditionCriteraTypes.All;

			//Although the unit is a cluster exception.. we should verify it is not a clustered object.
			if (obj.IsClusterException && obj.BeingIgnoredDueToClusterLogic)
			{
				criterias = ConditionCriteraTypes.SingleTarget;
			}

			List<Skill> UsableAbilities = new List<Skill>();
			foreach (var item in SortedAbilities)
			{
				//Check precast conditions
				if (!item.CheckPreCastConditionMethod()) continue;

				//Check Combat Conditions!
				if (!item.CheckCombatConditionMethod(criterias))
				{
					continue;
				}

				//Check if we can execute or if it requires movement
				if (IgnoreOutOfRange)
				{
					if (item.DestinationVector != FunkyGame.Hero.Position)
						continue;
				}

				Skill ability = item;
				Skill.SetupAbilityForUse(ref ability);
				UsableAbilities.Add(ability);
			}

			return UsableAbilities;
		}

		///<summary>
		///Returns Ability used for destructibles
		///</summary>
		internal virtual Skill DestructibleAbility()
		{
			Skill returnAbility = FunkyGame.Hero.Class.DefaultAttack;
			List<Skill> nonDestructibleAbilities = new List<Skill>();
			foreach (var item in Abilities.Values)
			{
				if (item.IsDestructiblePower)
				{

					//Check LOS -- Projectiles
					if (item.IsRanged && !FunkyGame.Targeting.Cache.CurrentTarget.IgnoresLOSCheck)
					{
						LOSInfo LOSINFO = FunkyGame.Targeting.Cache.CurrentTarget.LineOfSight;
						if (LOSINFO.LastLOSCheckMS > 3000)
						{
							if (!LOSINFO.LOSTest(FunkyGame.Hero.Position, true, ServerObjectIntersection: false))
							{
								//Raycast failed.. reset LOS Check -- for valid checking.
								if (!LOSINFO.RayCast.Value) FunkyGame.Targeting.Cache.CurrentTarget.RequiresLOSCheck = true;
								continue;
							}
						}
					}

					if (item.CheckPreCastConditionMethod())
					{
						returnAbility = item;
						Skill.SetupAbilityForUse(ref returnAbility, true);
						return returnAbility;
					}
				}

				else if (ObjectCache.CheckFlag(item.ExecutionType, SkillExecutionFlags.Target) || ObjectCache.CheckFlag(item.ExecutionType, SkillExecutionFlags.Location))
				{

					//Check LOS -- Projectiles
					if (item.IsRanged && !FunkyGame.Targeting.Cache.CurrentTarget.IgnoresLOSCheck)
					{
						LOSInfo LOSINFO = FunkyGame.Targeting.Cache.CurrentTarget.LineOfSight;
						if (LOSINFO.LastLOSCheckMS > 3000 || (item.IsProjectile && !LOSINFO.ObjectIntersection.HasValue) || !LOSINFO.NavCellProjectile.HasValue)
						{
							if (!LOSINFO.LOSTest(FunkyGame.Hero.Position, NavRayCast: true, ServerObjectIntersection: item.IsProjectile, Flags: NavCellFlags.AllowProjectile))
							{
								//Raycast failed.. reset LOS Check -- for valid checking.
								if (!LOSINFO.RayCast.Value) FunkyGame.Targeting.Cache.CurrentTarget.RequiresLOSCheck = true;
								continue;
							}
						}
						else if ((item.IsProjectile && LOSINFO.ObjectIntersection.Value) || !LOSINFO.NavCellProjectile.Value)
						{
							continue;
						}
					}

					//Add this Ability to our list.. incase we cant find an offical Ability to use.
					if (item.CheckPreCastConditionMethod())
					{
						nonDestructibleAbilities.Add(item);
					}
				}
			}

			//Use non-destructible Ability..
			if (nonDestructibleAbilities.Count > 0)
				returnAbility = nonDestructibleAbilities[0];

			Skill.SetupAbilityForUse(ref returnAbility, true);
			return returnAbility;
		}




		internal Dictionary<SNOPower, Skill> Abilities = new Dictionary<SNOPower, Skill>();
		internal List<Skill> SortedAbilities = new List<Skill>();


		///<summary>
		///Searches for any abilities that have set the OutOfCombat Movement Criteria.
		///</summary>
		internal Vector3 FindOutOfCombatMovementPower(out Skill MovementAbility, Vector3 Destination)
		{
			MovementAbility = null;
			foreach (var item in Abilities.Values.Where(A => A.FOutOfCombatMovement != null))
			{

				if (item.CheckPreCastConditionMethod())
				{
					Vector3 AbilityTargetVector = item.FOutOfCombatMovement.Invoke(Destination);
					if (AbilityTargetVector != Vector3.Zero)
					{
						MovementAbility = item;
						return AbilityTargetVector;
					}
				}
			}
			return Vector3.Zero;
		}
		///<summary>
		///Searches for any abilities that have set the Combat Movement Criteria.
		///</summary>
		internal Vector3 FindCombatMovementPower(out Skill MovementAbility, Vector3 Destination)
		{
			MovementAbility = null;
			foreach (var item in Abilities.Values.Where(A => A.FCombatMovement != null))
			{

				if (item.CheckPreCastConditionMethod())
				{
					Vector3 AbilityTargetVector = item.FCombatMovement.Invoke(Destination);
					if (AbilityTargetVector != Vector3.Zero)
					{
						MovementAbility = item;
						return AbilityTargetVector;
					}
				}
			}
			return Vector3.Zero;
		}

		internal bool HasCastableMovementAbility(bool combatOnly = true)
		{
			foreach (var item in Abilities.Values.Where(A => A.FCombatMovement != null && A.IsMovementSkill))
			{

				if (item.CheckPreCastConditionMethod())
				{
					return true;
				}
			}

			return false;
		}


		///<summary>
		///Returns a power for Buffing.
		///</summary>
		internal bool FindBuffPower(out Skill BuffAbility)
		{
			BuffAbility = null;
			foreach (var item in Abilities.Values.Where(A => A.IsBuff))
			{
				if (item.CheckPreCastConditionMethod())
				{
					if (item.CheckBuffConditionMethod())
					{
						BuffAbility = item;
						Skill.SetupAbilityForUse(ref BuffAbility);
						return true;
					}
				}
			}
			return false;
		}

		///<summary>
		///Returns a power for Combat Buffing.
		///</summary>
		internal bool FindCombatBuffPower(out Skill BuffAbility)
		{
			BuffAbility = null;
			foreach (var item in Abilities.Values.Where(A => A.IsBuff && A.UseageType.HasFlag(SkillUseage.Combat | SkillUseage.Anywhere)))
			{
				if (item.CheckPreCastConditionMethod())
				{
					if (item.CheckCombatConditionMethod())
					{
						BuffAbility = item;
						Skill.SetupAbilityForUse(ref BuffAbility);
						return true;
					}
				}
			}
			return false;
		}

		internal bool HasSpecialMovementBuff()
		{
			if (AC== ActorClass.Witchdoctor) return Hotbar.HasBuff(SNOPower.Witchdoctor_SpiritWalk);
			if (AC == ActorClass.Crusader) return Hotbar.HasBuff(SNOPower.X1_Crusader_SteedCharge);

			return false;
		}


		internal string DebugString()
		{
			string Slastusedabilities = LastUsedAbilities.Aggregate("", (current, a) => current + String.Format("Skill: {0} Time: {1}\r\n", a.Power, a.LastUsedMilliseconds));
			string s = String.Format("Class {0} WaitingForSpecial {1} ReserveAmount {2}\r\n" +
			                         "LastUsedSkill: {3}\r\n" +
									 "LastUsedSkills\r\n{4}", AC.ToString(), bWaitingForSpecial, iWaitingReservedAmount, LastUsedAbility.Power.ToString(), Slastusedabilities);
			return s;
		}

		///<summary>
		///Last successful Ability used.
		///</summary>
		internal Skill LastUsedAbility
		{
			get { return _lastUsedAbility; }
			set
			{
				if (!LastUsedAbilities[0].Equals(value))
				{
					updateLastUsedAbilities(value);
				}
				_lastUsedAbility = value;
			}
		}
		private Skill _lastUsedAbility=new Skill();

		internal Skill[] LastUsedAbilities
		{
			get { return _lastUsedAbilities; }
			set { _lastUsedAbilities = value; }
		}
		private Skill[] _lastUsedAbilities = new Skill[5];
		private void updateLastUsedAbilities(Skill lastusedskill)
		{
			int lastusedskillINDEX=LastUsedAbilities.IndexOf(lastusedskill);
			Skill[] clone_lastusedabilities = new Skill[LastUsedAbilities.Length];
			LastUsedAbilities.CopyTo(clone_lastusedabilities, 0);
			LastUsedAbilities[0] = lastusedskill;
			//Increase the index of all skills above the last used skill index.
			for (int i = 0; i < lastusedskillINDEX; i++)
			{
				LastUsedAbilities[i + 1] = clone_lastusedabilities[i];
			}
		}
		internal DateTime LastUsedACombatAbility { get; set; }

		internal void AbilitySuccessfullyUsed(Skill ability, bool reorderAbilities)
		{
			if (ability.IsCombat)
			{
				LastUsedACombatAbility = DateTime.Now;
			}

			LastUsedAbility = ability;


			//Only Sort When Non-Channeling!
			if (reorderAbilities)
				SortedAbilities = Abilities.Values.OrderByDescending(a => a.Priority).ThenByDescending(a => a.LastUsedMilliseconds).ToList();
		}

		internal Skill PowerPrime;

		internal static bool ShouldRecreatePlayerClass = false;
		internal static void CreateBotClass()
		{
			if (FunkyGame.CurrentActorClass != ActorClass.Invalid)
			{
				//Create Specific Player Class
				switch (FunkyGame.CurrentActorClass)
				{
					case ActorClass.Barbarian:
						FunkyGame.Hero.Class = new Barbarian();
						break;
					case ActorClass.DemonHunter:
						FunkyGame.Hero.Class = new DemonHunter();
						break;
					case ActorClass.Monk:
						FunkyGame.Hero.Class = new Monk();
						break;
					case ActorClass.Witchdoctor:
						FunkyGame.Hero.Class = new WitchDoctor();
						break;
					case ActorClass.Wizard:
						FunkyGame.Hero.Class = new Wizard();
						break;
					case ActorClass.Crusader:
						FunkyGame.Hero.Class = new Crusader();
						break;
				}

				FunkyGame.Hero.Class.RecreateAbilities();
				ShouldRecreatePlayerClass = false;
			}
		}

		internal void HotbarSkillsChangedHandler()
		{
			Logger.DBLog.InfoFormat("Hotbar Skills have changed.");
			FunkyGame.Hero.Class.RecreateAbilities();
			ShouldRecreatePlayerClass = true;
		}
	}

}