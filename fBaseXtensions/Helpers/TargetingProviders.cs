using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fBaseXtensions.Game;
using Zeta.Bot;
using Zeta.Game.Internals.Actors;

namespace fBaseXtensions.Helpers
{
    public class PluginLootTargeting : ITargetingProvider
    {
        private static readonly List<DiaObject> listEmptyList = new List<DiaObject>();
        public List<DiaObject> GetObjectsByWeight()
        {
            return listEmptyList;
        }
    }
    public class PluginObstacleTargeting : ITargetingProvider
    {
        private static readonly List<DiaObject> listEmptyList = new List<DiaObject>();
        public List<DiaObject> GetObjectsByWeight()
        {
            return listEmptyList;
        }
    }

    public class PluginCombatTargeting : ITargetingProvider
    {
        private static readonly List<DiaObject> listEmptyList = new List<DiaObject>();
        public List<DiaObject> GetObjectsByWeight()
        {
            if (!FunkyGame.Targeting.Cache.DontMove)
                return listEmptyList;
            List<DiaObject> listFakeList = new List<DiaObject>();
            listFakeList.Add(null);
            return listFakeList;
        }
    }
}
