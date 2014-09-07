using System.Collections.Generic;
using System.Runtime.InteropServices;
using fBaseXtensions.Helpers;
using fBaseXtensions.Settings;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace fBaseXtensions.XML
{
	[ComVisible(false)]
	[XmlElement("SetVariable")]
	public class SetVariableTag : ProfileBehavior
	{
		public static Dictionary<string, string> VariableDictionary = new Dictionary<string, string>();

		[XmlAttribute("Key")]
		public string Key { get; set; }

		[XmlAttribute("Value")]
		public string Value { get; set; }

		protected override Composite CreateBehavior()
		{
			return new Action(ret =>
			{
				if (VariableDictionary.ContainsKey(Key))
					VariableDictionary[Key] = Value;
				else
					VariableDictionary.Add(Key, Value);

				//Logger.DBLog.DebugFormat("Set {0} to Value {1}", Key, Value);

				m_IsDone=true;
			});
		}

		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}
		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}