using System;
using System.Linq;
using Zeta.Bot.Profile;
using Zeta.Bot.Profile.Composites;

namespace fBaseXtensions.XML
{
    public abstract class BaseIfComplexNodeTag : ComplexNodeTag
    {
        private bool? _ComplexDoneCheck;
        private bool? _AlreadyCompleted;
        private static Func<ProfileBehavior, bool> _BehaviorProcess;

        protected bool? ComplexDoneCheck
        {
            get
            {
                return _ComplexDoneCheck;
            }
            set
            {
                _ComplexDoneCheck = value;
            }
        }

        public override bool IsDone
        {
            get
            {
                // Make sure we've not already completed this tag
                if (_AlreadyCompleted.GetValueOrDefault(false))
                {
                    return true;
                }
                if (!ComplexDoneCheck.HasValue)
                {
                    ComplexDoneCheck = new bool?(GetConditionExec());
                }
                if (ComplexDoneCheck == false)
                {
                    return true;
                }
                if (_BehaviorProcess == null)
                {
                    _BehaviorProcess = new Func<ProfileBehavior, bool>(p => p.IsDone);
                }
                bool allChildrenDone = Body.All<ProfileBehavior>(_BehaviorProcess);
                if (allChildrenDone)
                {
                    _AlreadyCompleted = true;
                }
                return allChildrenDone;
            }
        }

        public abstract bool GetConditionExec();

        public override void ResetCachedDone()
        {
            foreach (ProfileBehavior behavior in Body)
            {
                behavior.ResetCachedDone();
            }
            ComplexDoneCheck = null;
        }
    }
}
