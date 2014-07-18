using System.Collections.Generic;
using System.Runtime.InteropServices;
using fBaseXtensions.Helpers;
using FunkyBot.Config.Settings;
using FunkyBot.Game;
using FunkyBot.Misc;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("FunkyLineOfSight")]
	public class FunkyLineOfSight : ProfileBehavior
	{
		[XmlElement("SNOIds")]
		public List<SNOId> SNOIds { get; set; }

		[XmlElement("SNOId")]
		public class SNOId 
		{
			[XmlAttribute("ID")]
			public int Id { get; set; }

			public SNOId()
			{
				Id = -1;
			}
			public SNOId(int id)
			{
				Id = id;
			}
		}

		private bool m_IsDone;
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		protected override Composite CreateBehavior()
		{
			return new Action(ret =>
			{
				if (SNOIds!=null)
				{
					foreach (var id in SNOIds)
					{
						if (!ProfileCache.LineOfSightSNOIds.Contains(id.Id))
							ProfileCache.LineOfSightSNOIds.Add(id.Id);
					}
					SettingLOSMovement.LOSSettingsTag.MaximumRange = 1000;
					SettingLOSMovement.LOSSettingsTag.MiniumRangeObjects = 10;

					Logger.DBLog.InfoFormat("[Funky] Added {0} to Line of Sight Ids", ProfileCache.LineOfSightSNOIds.Count);
				}

			
				m_IsDone=true;
			});
		}


		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}