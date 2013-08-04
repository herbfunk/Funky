using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Zeta;
using Zeta.CommonBot.Profile;
using Zeta.Internals;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyTrinity.XMLTags
{
	///<summary>
	///Takes a given set of IDs which are seperated by ","
	///If any of these IDs are currently present in the scence cache then Scence will be set to this ID.
	///To check Scence, use <Scene>
	///</summary>
	[ComVisible(false)]
	[XmlElement("CheckScenes")]
	public class CheckScenesTag : ProfileBehavior
	{
		private bool m_IsDone=false;
		private string sIDs;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			string[] Values=sIDs.ToString().Split(",".ToCharArray());
			List<int> IDs=new List<int>();
			//Logging.Write("Checking: " + IDs.Count.ToString());

			for (int i=0; i<Values.Count(); i++)
			{
				IDs.Add(Convert.ToInt32(Values[i]));
			}

			FunkyTrinity.Funky.ScenceCheck=0;

			foreach (Scene item in ZetaDia.Scenes.GetScenes())
			{
				//Logging.Write("SID: " + item.SceneInfo.SNOId.ToString());
				if (IDs.Contains(item.SceneInfo.SNOId))
				{
					//Logging.Write("Found Match");
					FunkyTrinity.Funky.ScenceCheck=item.SceneInfo.SNOId;
				}
			}

			return new Zeta.TreeSharp.Action(ret =>
			{
				m_IsDone=true;
			});
		}


		[XmlAttribute("Scenes")]
		public string iScenes
		{
			get
			{
				return sIDs;
			}
			set
			{
				sIDs=value;
			}
		}
	}
}