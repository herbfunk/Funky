using System.Runtime.InteropServices;
using Zeta.Common;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyTrinity.XMLTags
{
	[ComVisible(false)]
	[XmlElement("AddProfileSet")]
	public class AddRandomProfileSetTag : ProfileBehavior
	{
		private bool m_IsDone=false;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			return new Zeta.TreeSharp.Action(ret => AddNewProfileEntry());
		}

		private void AddNewProfileEntry()
		{

			FunkyTrinity.Funky.ProfilesSets.Add(new FunkyTrinity.Funky.ProfileSet(bRandom, sProfiles));
			Logging.Write("Added profile set. Is using Random: "+bRandom);

			m_IsDone=true;
		}

		private bool randomB=false;
		[XmlAttribute("random")]
		[XmlAttribute("randomize")]
		public bool bRandom
		{
			get
			{
				return randomB;
			}
			set
			{
				randomB=value;
			}
		}

		[XmlAttribute("Profiles")]
		[XmlAttribute("profiles")]
		public string sProfiles { get; set; }
	}
}