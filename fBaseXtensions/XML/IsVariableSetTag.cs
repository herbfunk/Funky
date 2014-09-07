using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using fBaseXtensions.Helpers;
using fBaseXtensions.Settings;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace fBaseXtensions.XML
{
	[ComVisible(false)]
	[XmlElement("IsVariableSet")]
	public class IsVariableSetTag : BaseIfComplexNodeTag
	{
		[XmlAttribute("Key")]
		public string Key { get; set; }

		[XmlAttribute("Value")]
		public string Value { get; set; }


		protected override Composite CreateBehavior()
		{
			return
			 new Decorator(ret => !IsDone,
				 new PrioritySelector(
					 base.GetNodes().Select(b => b.Behavior).ToArray()
				 )
			 );
		}

		public override bool GetConditionExec()
		{
			if (!SetVariableTag.VariableDictionary.ContainsKey(Key))
			{
				//Logger.DBLog.DebugFormat("Key {0} not found!", Key);
				return false;
			}

			if (!SetVariableTag.VariableDictionary[Key].Equals(Value))
			{
				//Logger.DBLog.DebugFormat("Key {0} of Value {1} not equal to Value {2} ", Key, Value, SetVariableTag.VariableDictionary[Key]);
				return false;
			}

			return true;
		}

	}
}