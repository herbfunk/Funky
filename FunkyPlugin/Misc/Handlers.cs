using System;
using Zeta.Common;
using Zeta.CommonBot;
using System.Collections.Generic;
using Zeta.Internals.Actors;
using Zeta.Navigation;

namespace FunkyTrinity
{
    public class TrinityStuckHandler : IStuckHandler
    {
        public Vector3 GetUnstuckPos()
        {
            return Vector3.Zero;
        }

        public bool IsStuck
        {
            get
            {
                return false;
            }
        }
    }
    public class TrinityCombatTargetingReplacer : ITargetingProvider
    {
        private static readonly List<DiaObject> listEmptyList = new List<DiaObject>();
        public List<DiaObject> GetObjectsByWeight()
        {
				if (!Funky.Bot.Combat.DontMove||Funky.thisFakeObject==null)
                return listEmptyList;
            List<DiaObject> listFakeList = new List<DiaObject>();
            listFakeList.Add(Funky.thisFakeObject);
            return listFakeList;
        }
    }
    public class TrinityLootTargetingProvider : ITargetingProvider
    {
        private static readonly List<DiaObject> listEmptyList = new List<DiaObject>();
        public List<DiaObject> GetObjectsByWeight()
        {
            return listEmptyList;
        }
    }
    public class TrinityObstacleTargetingProvider : ITargetingProvider
    {
        private static readonly List<DiaObject> listEmptyList = new List<DiaObject>();
        public List<DiaObject> GetObjectsByWeight()
        {
            return listEmptyList;
        }
    }
}