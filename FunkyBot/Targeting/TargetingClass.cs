using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FunkyBot.Cache.Objects;
using Zeta.TreeSharp;

namespace FunkyBot.Targeting
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

		public RunStatus CheckHandleTarget()
		{
			//Refresh?
			if (Bot.Targeting.Cache.ShouldRefreshObjectList)
				Bot.Targeting.Cache.Refresh();

			//Check if we have any NEW targets to deal with.. 
			if (Bot.Targeting.Cache.CurrentTarget != null)
			{
				//Directly Handle Target..
				RunStatus targetHandler = Bot.Targeting.Handler.HandleThis();

				//Only return failure if handling failed..
				if (targetHandler == RunStatus.Failure)
				{
					return RunStatus.Success;
				}
				if (targetHandler == RunStatus.Success)
				{
					Bot.Targeting.ResetTargetHandling();
				}

				return RunStatus.Running;
			}

			return RunStatus.Success;
		}

	}
}
