using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot.Profile;
using Zeta.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace FunkyBot.XMLTags
{
	[ComVisible(false)]
	[XmlElement("TrinityInteract")]
	public class TrinityInteractTag : ProfileBehavior
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
				float fClosestRange=-1;
				var iACDGuid=-1;
				var vMyLocation=ZetaDia.Me.Position;
				foreach (var thisobject in ZetaDia.Actors.GetActorsOfType<DiaObject>(true).Where<DiaObject>(a => a.ActorSNO==SNOID))
				{
					if (fClosestRange==-1||thisobject.Position.Distance(vMyLocation)<=fClosestRange)
					{
						fClosestRange=thisobject.Position.Distance(vMyLocation);
						iACDGuid=thisobject.ACDGuid;
					}
				}

				if (iACDGuid!=-1)
				{
					try
					{
						ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, Vector3.Zero, 0, iACDGuid);
					} catch
					{
						Logging.WriteDiagnostic("[Funky] There was a memory/DB failure trying to follow the TrinityInteract XML tag on SNO "+SNOID.ToString(CultureInfo.InvariantCulture));
					}
				}
				m_IsDone=true;
			});
		}


		[XmlAttribute("snoid")]
		public int SNOID { get; set; }

		public override void ResetCachedDone()
		{
			m_IsDone=false;
			base.ResetCachedDone();
		}
	}
}