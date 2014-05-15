using System;
using FunkyBot.Cache.Objects;
using FunkyBot.Game.Bounty;
using Zeta.Game.Internals;

namespace FunkyBot.Targeting.Behaviors
{
	public class TBBounty : TargetBehavior
	{
		//Monitor Current Bounty

		public TBBounty() : base() { }

		public override TargetBehavioralTypes TargetBehavioralTypeType { get { return TargetBehavioralTypes.Bounty; } }
		public override bool BehavioralCondition
		{
			get
			{
				return Bot.Settings.AdventureMode.EnableAdventuringMode && Bot.Game.Bounty.ActiveBounty != null && Bot.Game.Bounty.CurrentBountyCacheEntry != null;
			}
		}
		public override void Initialize()
		{
			base.Test = (ref CacheObject obj) =>
			{
				//	Cursed Events that have had interaction with a cursed object in the last nth time frame we check for completed bounty.
				//  Kill/Clear Events that are on the last area level we check for completed bounty.
				if ((Bot.Game.Bounty.CurrentBountyCacheEntry.Type == BountyQuestTypes.CursedEvent && DateTime.Now.Subtract(Bot.Targeting.Cache.lastSeenCursedShrine).TotalSeconds < 45) ||
					((Bot.Game.Bounty.CurrentBountyCacheEntry.Type == BountyQuestTypes.Kill || Bot.Game.Bounty.CurrentBountyCacheEntry.Type == BountyQuestTypes.Clear) && Bot.Character.Data.iCurrentLevelID==Bot.Game.Bounty.CurrentBountyCacheEntry.EndingLevelAreaID))
				{
					//Refresh Quest States.
					Bot.Game.Bounty.RefreshBountyQuestStates();

					//Check Current Quest State!
					if (Bot.Game.Bounty.BountyQuestStates[Bot.Game.Bounty.ActiveBounty.QuestSNO]==QuestState.Completed)
					{
						//Refresh Active Bounty to Verify there is no active bounty still!
						Bot.Game.Bounty.UpdateActiveBounty();
						if (Bot.Game.Bounty.ActiveBounty==null)
						{
							Logger.DBLog.Info("Bounty Is Finished.. Reloading Profile!!!");
							//No Active Bounty.. lets reload profile!
							Zeta.Bot.ProfileManager.Load(Zeta.Bot.ProfileManager.CurrentProfile.Path);
						}
					}
				}

				return false;

			};
		}
	}
}
