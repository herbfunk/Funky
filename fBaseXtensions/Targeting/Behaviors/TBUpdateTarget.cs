using System;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Cache.Internal.Objects;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero.Skills;
using Zeta.Bot.Settings;
using Zeta.Common;
using Logger = fBaseXtensions.Helpers.Logger;
using LogLevel = fBaseXtensions.Helpers.LogLevel;

namespace fBaseXtensions.Targeting.Behaviors
{
	public class TBUpdateTarget : TargetBehavior
	{
		private DateTime lastAvoidanceConnectSearch = DateTime.Today;
		private bool bStayPutDuringAvoidance = false;
		public TBUpdateTarget() : base() { }


		public override TargetBehavioralTypes TargetBehavioralTypeType
		{
			get
			{
				return TargetBehavioralTypes.Target;
			}
		}
		public override void Initialize()
		{
			base.Test = (ref CacheObject obj) =>
			{
				this.bStayPutDuringAvoidance = false;

				//cluster update
				FunkyGame.Targeting.Cache.Clusters.UpdateTargetClusteringVariables();

				//Standard weighting of valid objects -- updates current target.
				this.WeightEvaluationObjList(ref obj);


				//Final Possible Target Check
				if (obj == null)
				{
					// No valid targets but we were told to stay put?
					if (this.bStayPutDuringAvoidance)
					{
						//Lets check our avoidance object list
						if (FunkyGame.Targeting.Cache.objectsIgnoredDueToAvoidance.Count > 0 && DateTime.Now.Subtract(lastAvoidanceConnectSearch).TotalMilliseconds > 2000)
						{
							Logger.DBLog.InfoFormat("Preforming Avoidance Connection Search on Potential Objects");
							lastAvoidanceConnectSearch = DateTime.Now;

							foreach (var o in FunkyGame.Targeting.Cache.objectsIgnoredDueToAvoidance)
							{
								Vector3 safespot;
								if (FunkyGame.Navigation.AttemptFindSafeSpot(out safespot, o.BotMeleeVector, FunkyBaseExtension.Settings.Plugin.AvoidanceFlags))
								{
									obj = new CacheObject(safespot, TargetType.Avoidance, 20000, "AvoidConnection", 2.5f, -1);
									return true;
								}
							}
						}

						if (FunkyGame.Targeting.Cache.Environment.TriggeringAvoidances.Count == 0)
						{
							obj = new CacheObject(FunkyGame.Hero.Position, TargetType.Avoidance, 20000, "StayPutPoint", 2.5f, -1);
							return true;
						}
					}
				}

				return false;
			};
		}

		///<summary>
		///Iterates through Usable objects and sets the Bot.CurrentTarget to the highest weighted object found inside the given list.
		///</summary>
		private void WeightEvaluationObjList(ref CacheObject CurrentTarget)
		{
			// Store if we are ignoring all units this cycle or not
			bool bIgnoreAllUnits = !FunkyGame.Targeting.Cache.Environment.bAnyChampionsPresent
										&& ((!FunkyGame.Targeting.Cache.Environment.bAnyTreasureGoblinsPresent && FunkyBaseExtension.Settings.Targeting.GoblinPriority >= 2) || FunkyBaseExtension.Settings.Targeting.GoblinPriority < 2)
										&& FunkyGame.Hero.dCurrentHealthPct >= 0.85d;


			//clear our last "avoid" list..
			FunkyGame.Targeting.Cache.objectsIgnoredDueToAvoidance.Clear();

			double iHighestWeightFound = 0;

			foreach (CacheObject thisobj in FunkyGame.Targeting.Cache.ValidObjects)
			{
				thisobj.UpdateWeight();

				if (thisobj.Weight == 1)
				{
					// Force the character to stay where it is if there is nothing available that is out of avoidance stuff and we aren't already in avoidance stuff
					thisobj.Weight = 0;
					if (!FunkyGame.Targeting.Cache.RequiresAvoidance)
						this.bStayPutDuringAvoidance = true;

					continue;
				}

				// Is the weight of this one higher than the current-highest weight? Then make this the new primary target!
				if (thisobj.Weight > iHighestWeightFound && thisobj.Weight > 0)
				{
					//Check combat looting (Demonbuddy Setting)
					if (iHighestWeightFound > 0
										 && thisobj.targetType.Value == TargetType.Item
										 && !CharacterSettings.Instance.CombatLooting
										 && CurrentTarget.targetType.Value == TargetType.Unit) continue;


					//cache RAGUID so we can switch back if we need to
					int CurrentTargetRAGUID = CurrentTarget != null ? CurrentTarget.RAGUID : -1;

					//Set our current target to this object!
					CurrentTarget = ObjectCache.Objects[thisobj.RAGUID];

					bool resetTarget = false;


					if (CurrentTarget.targetType.Value == TargetType.Unit && FunkyGame.Targeting.Cache.Environment.NearbyAvoidances.Count > 0)
					{
						//We are checking if this target is valid and will not cause avoidance triggering due to movement.


						//set unit target (for Ability selector).
						FunkyGame.Targeting.Cache.CurrentUnitTarget = (CacheUnit)CurrentTarget;

						//Generate next Ability..
						Skill nextAbility = FunkyGame.Hero.Class.AbilitySelector(FunkyGame.Targeting.Cache.CurrentUnitTarget, FunkyGame.Targeting.Cache.LastCachedTarget.targetType == TargetType.Avoidance);

					
						if (nextAbility.Equals(FunkyGame.Hero.Class.DefaultAttack) && !FunkyGame.Hero.Class.CanUseDefaultAttack)
						{//No valid ability found

							Logger.Write(LogLevel.Target, "Could not find a valid ability for unit {0}", thisobj.InternalName);

							//if (thisobj.ObjectIsSpecial)
							//     ObjectCache.Objects.objectsIgnoredDueToAvoidance.Add(thisobj);
							//else
							resetTarget = true;

						}
						else
						{
							Vector3 destination = nextAbility.DestinationVector;
							if (ObjectCache.Obstacles.TestVectorAgainstAvoidanceZones(FunkyGame.Hero.Position, destination))
							{
								//if (!thisobj.ObjectIsSpecial)
								//	resetTarget = true;
								//else
								FunkyGame.Targeting.Cache.objectsIgnoredDueToAvoidance.Add(thisobj);
							}
						}

						//reset unit target
						FunkyGame.Targeting.Cache.CurrentUnitTarget = null;
					}

					//Avoidance Attempt to find a location where we can attack!
					if (FunkyGame.Targeting.Cache.objectsIgnoredDueToAvoidance.Contains(thisobj))
					{
						//Wait if no valid target found yet.. and no avoidance movement required.
						if (!FunkyGame.Targeting.Cache.RequiresAvoidance)
							this.bStayPutDuringAvoidance = true;

						resetTarget = true;
					}

					if (resetTarget)
					{
						CurrentTarget = CurrentTargetRAGUID != -1 ? ObjectCache.Objects[CurrentTargetRAGUID] : null;
						continue;
					}

					iHighestWeightFound = thisobj.Weight;
				}

			} // Loop through all the objects and give them a weight
		}
	}
}
