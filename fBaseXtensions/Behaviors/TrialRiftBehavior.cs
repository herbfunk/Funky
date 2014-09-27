using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fBaseXtensions.Game;
using Zeta.Bot.Navigation;
using Zeta.TreeSharp;

namespace fBaseXtensions.Behaviors
{
	internal static class TrialRiftBehavior
	{
		internal static RunStatus Behavior()
		{
			if (FunkyGame.GameIsInvalid)
				return RunStatus.Success;

			//Handle Targeting..
			if (FunkyGame.Targeting.CheckHandleTarget() == RunStatus.Running)
				return RunStatus.Running;

			//Update Bounty..
			FunkyGame.Bounty.UpdateActiveBounty();

			//Make sure we are still in the correct level
			if (FunkyGame.Hero.iCurrentLevelID!=405915 || !BountyCache.RiftTrialIsActiveQuest)
				return RunStatus.Success;

			if (FunkyGame.Hero.IsMoving)
				return RunStatus.Running;

			//Move to start position..
			if (FunkyGame.Hero.Position.Distance(BountyCache.RiftTrial_StartPosition)>10f)
			{
				Navigator.MoveTo(BountyCache.RiftTrial_StartPosition, "", false);
			}

			return RunStatus.Running;
		}
	}
}
