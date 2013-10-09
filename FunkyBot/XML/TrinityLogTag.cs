using System.Runtime.InteropServices;
using Zeta.Common;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityLog")]
	public class TrinityLogTag : ProfileBehavior
	{
		private bool m_IsDone=false;
		private string sLogOutput;
		private string sLogLevel;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			return new Zeta.TreeSharp.Action(ret =>
			{
				if (Level!=null&&Level.ToLower()=="diagnostic")
					Logging.WriteDiagnostic(Output);
				else
					Logging.Write(Output);
				m_IsDone=true;
			});
		}

		[XmlAttribute("level", true)]
		public string Level
		{
			get
			{
				return sLogLevel;
			}
			set
			{
				sLogLevel=value;
			}
		}

		[XmlAttribute("output", true)]
		public string Output
		{
			get
			{
				return sLogOutput;
			}
			set
			{
				sLogOutput=value;
			}
		}

		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}