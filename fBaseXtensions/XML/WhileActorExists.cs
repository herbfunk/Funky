using System.Linq;
using fBaseXtensions.Cache.Internal;
using fBaseXtensions.Cache.Internal.Collections;
using fBaseXtensions.Cache.Internal.Objects;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Logger = fBaseXtensions.Helpers.Logger;

namespace fBaseXtensions.XML
{
    [XmlElement("FunkyWhileActorPresent")]
    public class WhileActorExists : BaseWhileComplexNodeTag
    {
        [XmlAttribute("sno", true)]
        public int Sno { get; set; }

        [XmlAttribute("present", true)]
        public bool Present { get; set; }

        public WhileActorExists()
        {

        }



        protected override Composite CreateBehavior()
        {
            PrioritySelector p = new PrioritySelector();
            foreach (var behavior in GetNodes().Select(b => b.Behavior))
            {
                p.AddChild(new Decorator(ret => GetConditionExec(), behavior));//
            }
            return new Decorator(ret => !IsDone, p);
        }

        private void OnObjectAdded(CacheObject obj)
        {
            if (obj.SNOID == Sno)
            {
                _ConditionSuccessSkip = false;
                ObjectCache.Objects.OnObjectAddedToCollection -= OnObjectAdded;
                Logger.DBLog.DebugFormat("WhileActorExists Object {0} Added to Collection!", Sno);
            }
        }
        private void OnObjectRemoved(ObjectCollection.ObjectRemovedArgs args)
        {
            if (args.SNO == Sno)
            {
                int objectsPresent = ObjectCache.Objects.Values.Count(o => o.SNOID == Sno);

                if (objectsPresent == 1)
                {
                    _ConditionSuccessSkip = false;
                    ObjectCache.Objects.OnObjectRemovedFromCollection -= OnObjectRemoved;
                    Logger.DBLog.DebugFormat("WhileActorExists Object {0} Removed From Collection!", Sno);
                }
                else
                {
                    Logger.DBLog.DebugFormat("WhileActorExists Object {0} Removed From Collection, but Multiple Objects Exist! {1}", Sno, objectsPresent);
                }
            }
        }

        private bool _InitLogicChecking = false;
        private bool _ConditionSuccessSkip = true;

        public override bool GetConditionExec()
        {
            if (ObjectCache.ShouldUpdateObjectCollection)
                ObjectCache.UpdateCacheObjectCollection();

            if (!_InitLogicChecking)
            {
                Logger.DBLog.DebugFormat("WhileActorExists Init Logic Checks!");
                _InitLogicChecking = true;

                bool objectCurrentlyPresent = ObjectCache.Objects.Values.Any(o => o.SNOID == Sno);

                if (Present && objectCurrentlyPresent)
                {
                    ObjectCache.Objects.OnObjectRemovedFromCollection += OnObjectRemoved;
                    Logger.DBLog.DebugFormat("WhileActorExists Hooked Object Removed Event!");
                }
                else if (!Present && !objectCurrentlyPresent)
                {
                    ObjectCache.Objects.OnObjectAddedToCollection += OnObjectAdded;
                    Logger.DBLog.DebugFormat("WhileActorExists Hooked Object Added Event!");
                }
                else if (!Present && objectCurrentlyPresent)
                {
                    //Object contained in cache -- and we are checking if its not.
                    _ConditionSuccessSkip = false;
                }
                else if (Present && !objectCurrentlyPresent)
                {
                    //Object not contained in cache -- and we are checking if it is.
                    _ConditionSuccessSkip = false;
                }

                if (!_ConditionSuccessSkip)
                    Logger.DBLog.DebugFormat("WhileActorExists Finished during init!");
            }

            return _ConditionSuccessSkip;

            //return !Present ? !ObjectCache.Objects.Values.Any(o => o.SNOID == Sno) : ObjectCache.Objects.Values.Any(o => o.SNOID == Sno);
            //return !FunkyGame.Hero.bIsInTown;
        }
    }
}
