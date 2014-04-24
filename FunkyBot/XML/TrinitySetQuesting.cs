using FunkyBot.Game;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;

namespace FunkyBot.XMLTags
{
	 [XmlElement("TrinitySetQuesting")]
    public class TrinitySetQuesting : ProfileBehavior
    {
        private bool isDone = false;

        public override bool IsDone
        {
            get { return isDone; }
        }

        protected override Composite CreateBehavior()
        {
            return new Action(ret =>
            {
				ProfileCache.QuestMode = true;
                Logger.DBLog.Info("Setting QUESTING for the current profile.");
                isDone = true;   
            });
        }

        public override void ResetCachedDone()
        {
            isDone = false;
            base.ResetCachedDone();
        }
		
	}
}
