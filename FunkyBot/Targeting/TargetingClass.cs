using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FunkyBot.Cache.Objects;

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

	}
}
