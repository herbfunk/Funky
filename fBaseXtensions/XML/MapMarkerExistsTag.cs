using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using fBaseXtensions.Game;
using fBaseXtensions.Helpers;
using fBaseXtensions.Settings;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace fBaseXtensions.XML
{
	[XmlElement("MapMarkerExists")]
	public class MapMarkerExistsTag : BaseIfComplexNodeTag
	{
		[XmlAttribute("x")]
		public float X { get; set; }

		[XmlAttribute("y")]
		public float Y { get; set; }

		[XmlAttribute("z")]
		public float Z { get; set; }

		public Vector3 Position
		{
			get { return new Vector3(X, Y, Z); }
		}

		[XmlAttribute("distance")]
		public float Distance { get; set; }


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
			FunkyGame.Bounty.RefreshBountyMapMarkers();
			return FunkyGame.Bounty.CurrentBountyMapMarkers.Values.Any(m => m.Position.Distance2D(Position) <= Distance);
		}

	}
}