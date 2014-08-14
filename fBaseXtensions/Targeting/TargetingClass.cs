using fBaseXtensions.Game;
using fBaseXtensions.Game.Hero.Class;
using Zeta.TreeSharp;

namespace fBaseXtensions.Targeting
{
	public class TargetingClass
	{
		public TargetingClass() 
		{
			Cache = new TargetingCache();
			Handler=new TargetingHandler();
			Movement=new TargetMovement();

			//Reset Vars on target change!
			Cache.TargetChanged += Movement.OnTargetChanged;
			
		}
		///<summary>
		///Contains all Targeting related Cache Properties and Methods
		///</summary>
		public TargetingCache Cache { get; set; }
		///<summary>
		///Used to handle current target!
		///</summary>
		public TargetingHandler Handler { get; set; }
		///<summary>
		///Movement Class
		///</summary>
		public TargetMovement Movement { get; set; }

		public void ResetTargetHandling()
		{
			Cache.ResetTargetHandling();
			Movement.ResetTargetMovementVars();
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
			if (FunkyGame.Targeting.Cache.ShouldRefreshObjectList)
				FunkyGame.Targeting.Cache.Refresh();

			
			FunkyGame.Targeting.Cache.DontMove = true;
			//Check if we have any NEW targets to deal with.. 
			if (FunkyGame.Targeting.Cache.CurrentTarget != null)
			{
				//Directly Handle Target..
				RunStatus targetHandler = FunkyGame.Targeting.Handler.HandleThis();

				//Only return failure if handling failed..
				if (targetHandler == RunStatus.Failure)
				{
					return RunStatus.Success;
				}
				if (targetHandler == RunStatus.Success)
				{
					FunkyGame.Targeting.ResetTargetHandling();
				}

				return RunStatus.Running;
			}

			FunkyGame.Targeting.Cache.DontMove = false;
			CheckedPrecombat = false;
			return RunStatus.Success;
		}

	}
}
