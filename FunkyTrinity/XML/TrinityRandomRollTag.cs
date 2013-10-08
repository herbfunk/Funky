using System;
using System.Globalization;
using System.Runtime.InteropServices;
using FunkyBot.Cache;
using Zeta.Common;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityRandomRoll")]
	public class TrinityRandomRollTag : ProfileBehavior
	{
		private bool m_IsDone=false;
		private int iID;
		private int iMin;
		private int iMax;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			return new Zeta.TreeSharp.Action(ret =>
			{
				// Generate a random value between the selected min-max range, and assign it to our dictionary of random values
				int iOldValue;
				Random rndNum=new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), NumberStyles.HexNumber));
				int iNewRandomValue=(rndNum.Next((Max-Min)+1))+Min;
				Logging.Write("[Funky] Generating RNG for profile between "+Min.ToString()+" and "+Max.ToString()+", result="+iNewRandomValue.ToString());
				if (!ProfileCache.dictRandomID.TryGetValue(ID, out iOldValue))
				{
					 ProfileCache.dictRandomID.Add(ID, iNewRandomValue);
				}
				else
				{
					 ProfileCache.dictRandomID[ID]=iNewRandomValue;
				}
				m_IsDone=true;
			});
		}


		[XmlAttribute("id")]
		public int ID
		{
			get
			{
				return iID;
			}
			set
			{
				iID=value;
			}
		}
		[XmlAttribute("min")]
		public int Min
		{
			get
			{
				return iMin;
			}
			set
			{
				iMin=value;
			}
		}
		[XmlAttribute("max")]
		public int Max
		{
			get
			{
				return iMax;
			}
			set
			{
				iMax=value;
			}
		}

		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}