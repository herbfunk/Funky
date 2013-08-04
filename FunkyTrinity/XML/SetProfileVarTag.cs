using System;
using System.Runtime.InteropServices;
using Zeta.Common;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyTrinity.XMLTags
{
	[ComVisible(false)]
	[XmlElement("SetProfileVar")]
	public class SetProfileVarTag : ProfileBehavior
	{
		private bool m_IsDone=false;
		private string sStartProfile;
		private string sLastProfile;

		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		[XmlAttribute("startProfile")]
		public string StartProfile
		{
			get
			{
				return sStartProfile;
			}
			set
			{
				sStartProfile=value;
			}
		}

		[XmlAttribute("lastProfile")]
		public string lastProfile
		{
			get
			{
				return sLastProfile;
			}
			set
			{
				sLastProfile=value;
			}
		}


		protected override Composite CreateBehavior()
		{
			string LogStatusUpdate="[Herbfunk] Setting Profile Vars";

			if (!String.IsNullOrEmpty(sStartProfile))
			{
				FunkyTrinity.Funky.StartProfile=sStartProfile;
				LogStatusUpdate+="\r\n\t"+"Starting Profile: "+sStartProfile;
			}

			if (!String.IsNullOrEmpty(sLastProfile))
			{
				FunkyTrinity.Funky.LastProfile=sLastProfile;
				LogStatusUpdate+="\r\n\t"+"Ending Profile: "+sLastProfile;
			}

			if (FunkyTrinity.Funky.ProfilesSets!=null&&FunkyTrinity.Funky.ProfilesSets.Count>0)
			{
				FunkyTrinity.Funky.ProfilesSets.Clear();
				LogStatusUpdate+="\r\n\t"+"Previous Profile Sets have been cleared.";
			}

			LogStatusUpdate+="\r\n"+"============FuNkY============";

			Logging.Write(LogStatusUpdate);

			return new Zeta.TreeSharp.Action(ret => m_IsDone=true);
		}
	}
}