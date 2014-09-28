using System;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Enums;
using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero.Class;
using Zeta.Bot;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;

namespace fBaseXtensions.Targeting
{
	public partial class TargetingClass
	{
		public TargetingClass() 
		{
			Cache = new TargetingCache();
		
			cMovement=new TargetMovement();

			//Reset Vars on target change!
			Cache.TargetChanged += cMovement.OnTargetChanged;
			
		}
		///<summary>
		///Contains all Targeting related Cache Properties and Methods
		///</summary>
		public TargetingCache Cache { get; set; }
		///<summary>
		///Movement Class
		///</summary>
		public TargetMovement cMovement { get; set; }

		public void ResetTargetHandling()
		{
			Cache.ResetTargetHandling();
			cMovement.ResetTargetMovementVars();
		}
		internal void CheckPrecombat()
		{
			if (FunkyGame.Hero.Class == null)
			{//Null?
				PlayerClass.ShouldRecreatePlayerClass = true;
			}

			//Should we recreate class?
			if (PlayerClass.ShouldRecreatePlayerClass)
				PlayerClass.CreateBotClass();

			//Seconday Hotbar Check
			FunkyGame.Hero.Class.SecondaryHotbarBuffPresent();

			//Check if we should trim our SNO cache..
			if (DateTime.Now.Subtract(ObjectCache.cacheSnoCollection.lastTrimming).TotalMilliseconds > FunkyBaseExtension.Settings.Plugin.UnusedSNORemovalRate)
				ObjectCache.cacheSnoCollection.TrimOldUnusedEntries();

			ObjectCache.CheckForCacheRemoval();

		}
		private bool CheckedPrecombat = false;
		public RunStatus CheckHandleTarget()
		{
			if (!CheckedPrecombat)
			{
				CheckPrecombat();
				CheckedPrecombat = true;
			}

			//Refresh?
			if (Cache.ShouldRefreshObjectList)
				Cache.Refresh();

			
			Cache.DontMove = true;
			//Check if we have any NEW targets to deal with.. 
			if (Cache.CurrentTarget != null)
			{
				//Directly Handle Target..
				RunStatus targetHandler = HandleThis();

				//Only return failure if handling failed..
				if (targetHandler == RunStatus.Failure)
				{
					return RunStatus.Success;
				}
				if (targetHandler == RunStatus.Success)
				{
					ResetTargetHandling();
				}

				return RunStatus.Running;
			}

			Cache.DontMove = false;
			CheckedPrecombat = false;
			return RunStatus.Success;
		}

		

		internal void UpdateStatusText(string Action)
		{
			FunkyGame.sStatusText = Action + " ";

			FunkyGame.sStatusText += "Target=" + FunkyGame.Targeting.Cache.CurrentTarget.InternalName + " C-Dist=" + Math.Round(FunkyGame.Targeting.Cache.CurrentTarget.CentreDistance, 2) + ". " +
				 "R-Dist=" + Math.Round(FunkyGame.Targeting.Cache.CurrentTarget.RadiusDistance, 2) + ". ";

			if (FunkyGame.Targeting.Cache.CurrentTarget.targetType.Value == TargetType.Unit && FunkyGame.Hero.Class.PowerPrime.Power != SNOPower.None)
				FunkyGame.sStatusText += "Power=" + FunkyGame.Hero.Class.PowerPrime.Power + " (range " + FunkyGame.Hero.Class.PowerPrime.MinimumRange + ") ";

			FunkyGame.sStatusText += "Weight=" + FunkyGame.Targeting.Cache.CurrentTarget.Weight;
			BotMain.StatusText = FunkyGame.sStatusText;
			FunkyGame.bResetStatusText = true;
		}


	}
}
