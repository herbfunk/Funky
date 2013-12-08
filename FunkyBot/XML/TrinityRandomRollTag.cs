using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Zeta.Common;
using Zeta.CommonBot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using FunkyBot.Game;
using Action = Zeta.TreeSharp.Action;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityRandomRoll")]
	public class TrinityRandomRollTag : ProfileBehavior
	{
		private bool m_IsDone;

		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			return new Action(ret =>
			{
				// Generate a random value between the selected min-max range, and assign it to our dictionary of random values
				int iOldValue;
				var rndNum=new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), NumberStyles.HexNumber));
				var iNewRandomValue=(rndNum.Next((Max-Min)+1))+Min;
				Logging.Write("[Funky] Generating RNG for profile between "+Min.ToString(CultureInfo.InvariantCulture)+" and "+Max.ToString(CultureInfo.InvariantCulture)+", result="+iNewRandomValue.ToString(CultureInfo.InvariantCulture));
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
		public int ID { get; set; }

		[XmlAttribute("min")]
		public int Min { get; set; }

		[XmlAttribute("max")]
		public int Max { get; set; }

		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}