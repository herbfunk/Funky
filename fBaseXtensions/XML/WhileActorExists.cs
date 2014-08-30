using System.Linq;
using fBaseXtensions.Cache.Internal;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace fBaseXtensions.XML
{
	[XmlElement("FunkyWhileActorPresent")]
	public class WhileActorExists : BaseWhileComplexNodeTag
	{
		[XmlAttribute("sno", true)]
		public int Sno { get; set; }

		[XmlAttribute("present", true)]
		public bool Present { get; set; }

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
			if (ObjectCache.ShouldUpdateObjectCollection)
				ObjectCache.UpdateCacheObjectCollection();
			
			return !Present ? !ObjectCache.Objects.Values.Any(o => o.SNOID == Sno) : ObjectCache.Objects.Values.Any(o => o.SNOID == Sno);
			//return !FunkyGame.Hero.bIsInTown;
		}
	}
}
