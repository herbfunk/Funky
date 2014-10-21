using System.Linq;
using fBaseXtensions.Game;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using ConditionParser = Zeta.Bot.ConditionParser;

namespace fBaseXtensions.XML
{
    public static class CustomConditions
    {
        public static void Initialize()
        {
            ScriptManager.RegisterShortcutsDefinitions((typeof(CustomConditions)));
        }

        public static bool QuestAndStepActive(int sno, int step)
        {
            FunkyGame.Bounty.RefreshActiveQuests();

            if (!FunkyGame.Bounty.ActiveQuests.ContainsKey(sno))
            {//Quest SNO is not contained in Active Quests Cache!
                return false;
            }

            if (FunkyGame.Bounty.ActiveQuests[sno].State != QuestState.InProgress)
            {//Quest not in progress!
                return false;
            }
                
            if (step != -1 && FunkyGame.Bounty.ActiveQuests[sno].Step != step)
            {//Quest Step does not match!
                return false;
            }

            return true;
        }
    }
}
