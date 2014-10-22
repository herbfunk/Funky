using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Logger = fBaseXtensions.Helpers.Logger;
namespace fBaseXtensions.XML
{
    [XmlElement("FunkyWhile")]
    class FunkyWhileTag : BaseWhileComplexNodeTag
    {
        [XmlAttribute("condition")]
        public string Condition { get; set; }

        protected override Composite CreateBehavior()
        {
            PrioritySelector p = new PrioritySelector();
            foreach (var behavior in GetNodes().Select(b => b.Behavior))
            {
                p.AddChild(new Decorator(ret => GetConditionExec(), behavior));//
            }
            return new Decorator(ret => !IsDone, p);
        }

       
        public override bool GetConditionExec()
        {
            return ScriptManager.GetCondition(Condition).Invoke();
        }
    }
}
